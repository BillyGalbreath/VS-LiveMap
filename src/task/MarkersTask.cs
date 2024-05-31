using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using livemap.layer;
using livemap.util;
using Newtonsoft.Json;

namespace livemap.task;

public class MarkersTask : AsyncTask {
    private readonly Dictionary<string, long> _lastUpdate = new();

    public MarkersTask(LiveMap server) : base(server) { }

    protected override async Task TickAsync(CancellationToken cancellationToken) {
        List<string> layerIds = new();

        long now = DateTimeOffset.Now.ToUnixTimeSeconds();

        List<Layer> layers = new(_server.LayerRegistry.Values);
        foreach (Layer layer in layers) {
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            // private layers write to special json files
            // we won't be processing these the normal way
            if (!layer.Private) {
                layerIds.Add(layer.Id);
            }

            // check if it's time to write to disk
            long lastUpdate = _lastUpdate.GetValueOrDefault(layer.Id, 0);
            if (now - lastUpdate < Math.Max(layer.Interval ?? 0, 0)) {
                continue;
            }
            _lastUpdate[layer.Id] = now;

            // finally write to disk
            try {
                await layer.WriteToDisk(cancellationToken);
            } catch (Exception e) {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }

        // todo - scan layers from the custom dir
        // we just need the filename as the layer id. we don't touch the actual files as those are handled by the end user
        //

        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        string markersJson = JsonConvert.SerializeObject(new Dictionary<string, List<string>> {
            {
                "markers", layerIds
            }
        });

        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        await Files.WriteJsonAsync(Path.Combine(Files.JsonDir, "markers.json"), markersJson, cancellationToken);
    }
}
