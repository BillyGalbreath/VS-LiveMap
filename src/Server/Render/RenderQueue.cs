using System.Collections.Generic;
using LiveMap.Common.Util;

namespace LiveMap.Server.Render;

public class RenderQueue {
    private readonly Queue<long> queuedRegions = new();
    private readonly object objLock = new();

    public int Count {
        get {
            lock (objLock) {
                return queuedRegions.Count;
            }
        }
    }

    public void Enqueue(long region) {
        lock (objLock) {
            if (queuedRegions.Contains(region)) {
                return;
            }

            Logger.Info($"Added to Queue: {Mathf.LongToX(region)},{Mathf.LongToZ(region)}");
            queuedRegions.Enqueue(region);
        }
    }

    public long Dequeue() {
        lock (objLock) {
            return queuedRegions.Dequeue();
        }
    }

    public void Dispose() {
        lock (objLock) {
            queuedRegions.Clear();
        }
    }
}
