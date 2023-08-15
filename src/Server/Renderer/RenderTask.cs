namespace LiveMap.Server.Renderer;

public class RenderTask {
    private readonly RenderQueue renderQueue;

    private bool running = false;

    public RenderQueue RenderQueue {
        get {
            return renderQueue;
        }
    }

    public RenderTask() {
        renderQueue = new RenderQueue();
    }

    public void Start() {
        running = true;
    }

    public void Stop() {
        running = false;
    }

    public void Run() {
        if (!running) {
            return;
        }

        // todo - process queue
        //
    }

    public void Dispose() {
        Stop();

        renderQueue.Dispose();
    }
}
