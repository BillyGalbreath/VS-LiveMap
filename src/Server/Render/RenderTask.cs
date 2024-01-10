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

    public void Stop() {
        if (Stopped) {
            return;
        }

        Logger.Debug("##### Stopping async infinite loop");
        Stopped = true;
    }

    public void Run() {
        if (Server.Colormap == null) {
            Logger.Debug("##### Cannot run render task! No colormap found...");
            return;
        }

        if (Stopped) {
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
        Logger.Debug("##### Starting async infinite loop");

        running = true;

        (thread = new Thread(_ => {
            while (running) {
                Logger.Debug("START");

                try {
                    long region = processQueue.Take();
                    renderer.ScanRegion(region);
                } catch (ThreadInterruptedException) {
                    Logger.Debug("##### Interrupted async infinite loop");
                    running = false;
                    Thread.CurrentThread.Interrupt();
                }

                Logger.Debug("END");
            }

            Logger.Debug("##### Finished async infinite loop");
        })).Start();
    }

    public void Dispose() {
        thread?.Interrupt();
        thread = null;
    }
}
