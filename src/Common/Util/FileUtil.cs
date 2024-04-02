using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace LiveMap.Common.Util;

public static class FileUtil {
    public static readonly string DataDir = Path.Combine(GamePaths.DataPath, "ModData", LiveMapMod.Api.World.SavegameIdentifier, "LiveMap");
    public static readonly string ColormapFile = Path.Combine(DataDir, "colormap.yaml");
    public static readonly string WebDir = Path.Combine(DataDir, "web");
    public static readonly string TilesDir = Path.Combine(WebDir, "tiles");

    public static void ExtractWebFiles(ICoreAPI api) {
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
