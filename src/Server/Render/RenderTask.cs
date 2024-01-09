using System.Threading;
using LiveMap.Common;

namespace LiveMap.Server.Render;

public class RenderTask {
    public LiveMapServer Server { get; }

    private Renderer Renderer { get; }
    private RenderQueue RenderQueue { get; }

    public bool Stopped { get; private set; }

    public RenderTask(LiveMapServer server) {
        Server = server;

        Renderer = new BasicRenderer(this);
        RenderQueue = new RenderQueue(server);
    }

    public void Queue(int regionX, int regionZ) {
        RenderQueue.Enqueue(new Region(regionX, regionZ));
    }

    public void Stop() {
        if (Stopped) {
            Logger.Warn("##### Cannot stop a stopped render task!");
            return;
        }

        Logger.Info("##### Stop");
        Stopped = true;
    }

    public void Run() {
        Logger.Info("##### Run");

        if (Stopped) {
            Logger.Warn("##### Cannot run a stopped render task!");
            return;
        }

        if (Server.Colormap == null) {
            Logger.Warn("##### Cannot run render task! No block colors found...");
            return;
        }

        Logger.Info("##### Process " + RenderQueue.Count);

        ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThreads);
        Logger.Info($"MIN workerThreads: {workerThreads} completionPortThreads: {completionPortThreads}");
        ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
        Logger.Info($"MAX workerThreads: {workerThreads} completionPortThreads: {completionPortThreads}");

        while (RenderQueue.Count > 0) {
            Region region = RenderQueue.Dequeue();
            Logger.Info($"!!! Starting queued region {region.PosX},{region.PosZ})");
            ThreadPool.QueueUserWorkItem(_ => Renderer.ScanRegion(region));
        }
    }

    public void Dispose() {
        if (!Stopped) {
            Stop();
        }

        RenderQueue.Dispose();
    }
}
