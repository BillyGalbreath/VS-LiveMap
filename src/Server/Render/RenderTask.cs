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

        Logger.Debug($"##### Queueing region {regionX},{regionZ}");
        _bufferQueue.Enqueue(index);
    }

    public void Run() {
        if (Stopped || Server.Colormap == null) {
            return;
        }

        while (_bufferQueue.Count > 0) {
            _processQueue.Add(_bufferQueue.Dequeue());
        }

        if (!_running) {
            RunAsyncInfiniteLoop();
        }
    }

    private void RunAsyncInfiniteLoop() {
        _running = true;

        (_thread = new Thread(_ => {
            while (_running) {
                try {
                    long region = _processQueue.Take();
                    _renderer.ScanRegion(region);
                } catch (ThreadInterruptedException) {
                    _running = false;
                    Thread.CurrentThread.Interrupt();
                }
            }
        })).Start();
    }

    public void Dispose() {
        Stopped = true;

        _thread?.Interrupt();
        _thread = null;

        _bufferQueue.Clear();
        while (_processQueue.TryTake(out _)) { }
    }
}
