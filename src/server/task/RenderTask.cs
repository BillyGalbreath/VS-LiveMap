﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using livemap.common.render;
using livemap.common.util;
using Vintagestory.API.Server;

namespace livemap.server.task;

public sealed class RenderTask {
    private readonly LiveMapServer _server;

    private readonly ConcurrentQueue<long> _bufferQueue = new();
    private readonly BlockingCollection<long> _processQueue = new();

    private readonly Dictionary<string, Renderer> _renderers = new();

    private Thread? _thread;
    private bool _running;
    private bool _stopped;

    public RenderTask(LiveMapServer server, ICoreServerAPI api) {
        _server = server;
        foreach ((string? id, Renderer.Builder? builder) in server.RendererRegistry) {
            _renderers.Add(id, builder.Func.Invoke(server, api));
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

        Logger.Debug($"##### Queueing region {regionX},{regionZ} (total in queue: {_bufferQueue.Count}+{_processQueue.Count}={_bufferQueue.Count + _processQueue.Count}) {Environment.CurrentManagedThreadId}");
    }

    public void ProcessQueue() {
        if (_stopped) {
            return;
        }

        // we need a colormap
        if (_server.Colormap.Count == 0) {
            Logger.Warn("Cannot process render queue. No colormap loaded.");
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

                    // process the region through all the renderers
                    foreach ((string? _, Renderer? renderer) in _renderers) {
                        renderer.ScanRegion(region);
                    }
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

        foreach ((string? _, Renderer renderer) in _renderers) {
            renderer.Dispose();
        }
        _renderers.Clear();

        if (cancelled) {
            Logger.Warn("Render task cancelled!");
        }
    }
}
