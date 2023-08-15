using LiveMap.src.Common.Util;
using System.Collections.Generic;
using Vintagestory.API.MathTools;

namespace LiveMap.Server.Renderer;

public class RenderQueue {
    private readonly Queue<long> queuedChunks = new();
    private readonly object objLock = new();

    public void Enqueue(Vec3i chunk) {
        Enqueue(chunk.X, chunk.Z);
    }

    public void Enqueue(Vec2i chunk) {
        Enqueue(chunk.X, chunk.Y);
    }

    public void Enqueue(int x, int y) {
        Enqueue(Math.AsLong(x, y));
    }

    public void Enqueue(long chunk) {
        lock (objLock) {
            if (queuedChunks.Contains(chunk)) {
                queuedChunks.Enqueue(chunk);
            }
        }
    }

    public long Dequeue() {
        lock (objLock) {
            return queuedChunks.Dequeue();
        }
    }

    public void Dispose() {
        lock (objLock) {
            queuedChunks.Clear();
        }
    }
}
