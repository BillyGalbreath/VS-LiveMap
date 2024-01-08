using System.Collections.Generic;

namespace LiveMap.Server.Render;

public class RenderQueue {
    private readonly LiveMapServer server;
    private readonly Queue<long> queuedRegions = new();
    private readonly object objLock = new();

    public RenderQueue(LiveMapServer server) {
        this.server = server;
    }

    public int Count {
        get {
            lock (objLock) {
                return queuedRegions.Count;
            }
        }
    }

    public void Enqueue(Region region) {
        lock (objLock) {
            if (queuedRegions.Contains(region.Index)) {
                return;
            }

            server.Logger.Event($"Added to Queue: {region.PosX},{region.PosZ}");
            queuedRegions.Enqueue(region.Index);
        }
    }

    public Region Dequeue() {
        lock (objLock) {
            return new Region(queuedRegions.Dequeue());
        }
    }

    public void Dispose() {
        lock (objLock) {
            queuedRegions.Clear();
        }
    }
}
