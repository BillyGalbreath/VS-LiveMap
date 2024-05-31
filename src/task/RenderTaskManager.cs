using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using livemap.data;
using livemap.util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Common.Database;

namespace livemap.task;

[PublicAPI]
public sealed class RenderTaskManager {
    private readonly LiveMap _server;

    public readonly ChunkLoader ChunkLoader;
    public RenderTask RenderTask { get; }

    public HashSet<int> MicroBlocks { get; }
    public HashSet<int> BlocksToIgnore { get; }
    public int LandBlock { get; }

    private readonly ConcurrentQueue<long> _bufferQueue = new();
    private readonly BlockingCollection<long> _processQueue = new();

    private Thread? _thread;
    private bool _running;
    private bool _stopped;

    public RenderTaskManager(LiveMap server) {
        _server = server;

        ChunkLoader = new ChunkLoader(server.Sapi);
        RenderTask = new RenderTask(server, this);

        MicroBlocks = server.Sapi.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                block.Code.Path.StartsWith("chiseledblock") ||
                block.Code.Path.StartsWith("microblock"))
            .Select(block => block.Id)
            .ToHashSet();

        BlocksToIgnore = server.Sapi.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                (block.Code.Path.EndsWith("-snow") && !MicroBlocks.Contains(block.Id)) ||
                block.Code.Path.EndsWith("-snow2") ||
                block.Code.Path.EndsWith("-snow3") ||
                block.Code.Path.Equals("snowblock") ||
                block.Code.Path.Contains("snowlayer-"))
            .Select(block => block.Id).ToHashSet();

        LandBlock = server.Sapi.World.GetBlock(new AssetLocation("game", "soil-low-normal")).Id;
    }

    public void QueueAll(Vec2i? minChunk = null, Vec2i? maxChunk = null) {
        // queue up all existing chunks within range
        foreach (ChunkPos chunk in ChunkLoader.GetAllMapChunkPositions()) {
            if (minChunk != null && (chunk.X < minChunk.X || chunk.Z < minChunk.Y)) {
                continue;
            }
            if (maxChunk != null && (chunk.X > maxChunk.X || chunk.Z > maxChunk.Y)) {
                continue;
            }
            Queue(chunk.X >> 4, chunk.Z >> 4);
        }
    }

    public void Queue(int regionX, int regionZ) {
        if (_stopped) {
            return;
        }

        // convert region coordinates to long
        long index = Mathf.AsLong(regionX, regionZ);

        // ensure this region hasn't already been queued up
        if (_bufferQueue.Contains(index) || _processQueue.Contains(index)) {
            return;
        }

        // queue it up to the buffer, so it doesn't get process immediately
        _bufferQueue.Enqueue(index);

        Logger.Debug($"Queueing region {regionX},{regionZ} (buffer: {_bufferQueue.Count} process:{_processQueue.Count})");
    }

    public void ProcessQueue() {
        if (_stopped) {
            return;
        }

        // we need a colormap
        if (_server.Colormap.Count == 0) {
            Logger.Warn("Cannot process render queue. No colormap loaded");
            return;
        }

        // pass all regions from buffer queue to the process queue
        while (_bufferQueue.TryDequeue(out long region)) {
            _processQueue.Add(region);
        }

        if (_running) {
            // this task is still running, no need to restart it
            return;
        }

        _running = true;

        (_thread = new Thread(_ => {
            try {
                while (_running) {
                    // wait until we have a region to process
                    long region = _processQueue.Take();

                    long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    int regionX = Mathf.LongToX(region);
                    int regionZ = Mathf.LongToZ(region);

                    RenderTask.ScanRegion(regionX, regionZ);

                    long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    Logger.Debug($"Region {regionX},{regionZ} finished ({end - start}ms) - Regions remaining: {_processQueue.Count}");
                }
            } catch (Exception) {
                // ignore
            }

            _running = false;
        })).Start();
    }

    public void Dispose() {
        bool cancelled = !_stopped && _running;

        _stopped = true;

        _thread?.Interrupt();
        _thread = null;

        _bufferQueue.Clear();
        while (_processQueue.TryTake(out _)) { }

        if (cancelled) {
            Logger.Warn("Render task cancelled");
        }

        MicroBlocks.Clear();
        BlocksToIgnore.Clear();

        ChunkLoader.Dispose();
    }

    public (int, int) GetCounts() {
        return (_bufferQueue.Count, _processQueue.Count);
    }
}
