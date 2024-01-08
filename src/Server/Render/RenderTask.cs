using System.Threading;

namespace LiveMap.Server.Render;

public class RenderTask {
    public LiveMapServer Server { get; }

    private Renderer Renderer { get; }
    private RenderQueue RenderQueue { get; }

    public bool Stopped { get; private set; } = true;

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
            Server.Logger.Warning("##### Cannot stop a stopped render task!");
            return;
        }

        Server.Logger.Event("##### Stop");
        Stopped = true;
    }

    public void Run() {
        Server.Logger.Event("##### Run");

        if (Stopped) {
            Server.Logger.Warning("##### Cannot run a stopped render task!");
            return;
        }

        if (Server.BlockColors == null) {
            Server.Logger.Warning("##### Cannot run render task! No block colors found...");
            return;
        }

        Server.Logger.Event("##### Process " + RenderQueue.Count);

        ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThreads);
        Server.Logger.Event($"MIN workerThreads: {workerThreads} completionPortThreads: {completionPortThreads}");
        ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
        Server.Logger.Event($"MAX workerThreads: {workerThreads} completionPortThreads: {completionPortThreads}");

        while (RenderQueue.Count > 0) {
            Region region = RenderQueue.Dequeue();
            Server.Logger.Event($"!!! Starting queued region {region.PosX},{region.PosZ})");
            ThreadPool.QueueUserWorkItem(_ => Renderer.ScanRegion(region));
        }
    }

    public void Dispose() {
        if (!Stopped) {
            Stop();
        }

        Renderer.Dispose();
        RenderQueue.Dispose();
    }
}
