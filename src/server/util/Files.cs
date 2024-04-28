using System.Collections.Generic;
using System.IO;
using livemap.common.util;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace livemap.server.util;

public abstract class Files {
    public static string DataDir { get; internal set; } = null!;
    public static string ColormapFile { get; internal set; } = null!;
    public static string WebDir { get; internal set; } = null!;
    public static string TilesDir { get; internal set; } = null!;

    internal static void ExtractWebFiles(ICoreAPI api) {
        GamePaths.EnsurePathExists(DataDir);
        // load web assets to disk
        // stored in "config" to allow the game to automatically load them for us
        List<IAsset> assets = api.Assets.GetMany("config", "livemap");
        foreach (IAsset asset in assets) {
            // strip leading "config/" from the path
            string path = asset.Location.Path[7..];

            // check if we've already saved this file to disk
            string destPath = Path.Combine(WebDir, path);
            if (File.Exists(destPath)) {
                Logger.Debug($"Asset already exists on disk &6{path}");
                continue;
            }

            if (asset.Data == null) {
                Logger.Error($"Error loading asset from zip &4{path}");
                continue;
            }

            Logger.Debug($"Saving asset from zip to disk &6{path}");
            GamePaths.EnsurePathExists(Path.GetDirectoryName(destPath));
            File.WriteAllBytes(destPath, asset.Data);
        }
    }
}
