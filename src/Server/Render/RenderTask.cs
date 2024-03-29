using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using LiveMap.Common.Util;

namespace LiveMap.Server.Render;

public sealed class RenderTask {
    public LiveMapServer Server { get; }

    private readonly Renderer _renderer;

    private readonly ConcurrentQueue<long> _bufferQueue = new();
    private readonly BlockingCollection<long> _processQueue = new();

    private Thread? _thread;
    private bool _running;
    private bool _runningAll;

    public bool Stopped { get; private set; }

    public RenderTask(LiveMapServer server) {
        Server = server;
        _renderer = new BasicRenderer(this);
    }

    public void Queue(int regionX, int regionZ) {
        if (Stopped) {
            return;
        }

        long index = Mathf.AsLong(regionX, regionZ);
        if (_bufferQueue.Contains(index) || _processQueue.Contains(index)) {
            return;
        }

        _bufferQueue.Enqueue(index);

        Logger.Debug($"##### Queueing region {regionX},{regionZ} (total in queue: {_bufferQueue.Count}+{_processQueue.Count}={_bufferQueue.Count + _processQueue.Count}) {Environment.CurrentManagedThreadId}");
    }

    public bool ProcessAllRegions() {
        if (_runningAll || Stopped) {
            return false;
        }

        _runningAll = true;

        (_thread = new Thread(_ => {
            try {
                _renderer.ScanAllRegions();
            } catch (Exception) {
                // ignored
            }

            _runningAll = false;
        })).Start();

        return true;
    }

    public void ProcessQueue() {
        if (Stopped || _runningAll) {
            return;
        }

        while (_bufferQueue.TryDequeue(out long region)) {
            _processQueue.Add(region);
        }

        if (_running) {
            return;
        }

        _running = true;
        (_thread = new Thread(_ => {
            try {
                while (_running) {
                    _renderer.ScanRegion(_processQueue.Take());
                    if (_runningAll) {
                        break;
                    }
                }
            } catch (Exception) {
                // ignore
            }

            _running = false;
        })).Start();
    }

    public void Dispose() {
        bool cancelled = !Stopped && (_running || _runningAll);

        Stopped = true;

        _thread?.Join();
        _thread = null;

        _bufferQueue.Clear();
        while (_processQueue.TryTake(out _)) { }

        if (cancelled) {
            Logger.Warn("Render task cancelled!");
        }
    }
}
