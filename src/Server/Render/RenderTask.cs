using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LiveMap.Common.Util;

namespace LiveMap.Server.Render;

public class RenderTask {
    public LiveMapServer Server { get; }

    private readonly Renderer renderer;

    private readonly Queue<long> bufferQueue = new();
    private readonly BlockingCollection<long> processQueue = new();

    private Thread? thread;
    private bool running;

    public bool Stopped { get; private set; }

    public RenderTask(LiveMapServer server) {
        Server = server;
        renderer = new BasicRenderer(this);
    }

    public void Queue(int regionX, int regionZ) {
        long index = Mathf.AsLong(regionX, regionZ);
        if (bufferQueue.Contains(index) || processQueue.Contains(index)) {
            return;
        }

        Logger.Debug($"##### Queueing region {regionX},{regionZ}");
        bufferQueue.Enqueue(index);
    }

    public void Run() {
        if (Stopped || Server.Colormap == null) {
            return;
        }

        while (bufferQueue.Count > 0) {
            processQueue.Add(bufferQueue.Dequeue());
        }

        if (!running) {
            RunAsyncInfiniteLoop();
        }
    }

    private void RunAsyncInfiniteLoop() {
        running = true;

        (thread = new Thread(_ => {
            while (running) {
                try {
                    long region = processQueue.Take();
                    renderer.ScanRegion(region);
                } catch (ThreadInterruptedException) {
                    running = false;
                    Thread.CurrentThread.Interrupt();
                }
            }
        })).Start();
    }

    public void Dispose() {
        Stopped = true;

        thread?.Interrupt();
        thread = null;
    }
}
