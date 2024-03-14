using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LiveMap.Common.Util;

namespace LiveMap.Server.Render;

public sealed class RenderTask {
    public LiveMapServer Server { get; }

    private readonly Renderer _renderer;

    private readonly Queue<long> _bufferQueue = new();
    private readonly BlockingCollection<long> _processQueue = new();

    private Thread? _thread;
    private bool _running;

    public bool Stopped { get; private set; }

    public RenderTask(LiveMapServer server) {
        Server = server;
        _renderer = new BasicRenderer(this);
    }

    public void Queue(int regionX, int regionZ) {
        long index = Mathf.AsLong(regionX, regionZ);
        if (_bufferQueue.Contains(index) || _processQueue.Contains(index)) {
            return;
        }

        _bufferQueue.Enqueue(index);

        Logger.Debug($"##### Queueing region {regionX},{regionZ} (total in queue: {_bufferQueue.Count}+{_processQueue.Count}={_bufferQueue.Count + _processQueue.Count}) {Environment.CurrentManagedThreadId}");
    }

    public void Run() {
        if (_running || Stopped) {
            return;
        }

        _running = true;

        (_thread = new Thread(_ => {
            try {
                _renderer.ScanAllRegions();
            } catch (Exception) {
                // ignored
            }

            _running = false;
        })).Start();
    }

    public void Dispose() {
        bool cancelled = !Stopped && _running;

        Stopped = true;

        _thread?.Interrupt();
        _thread = null;

        _bufferQueue.Clear();
        while (_processQueue.TryTake(out _)) { }

        if (cancelled) {
            Logger.Warn("Render task cancelled!");
        }
    }
}
