using System.IO;
using livemap.server;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace livemap.common.util;

public abstract class Files {
    public static string DataDir { get; internal set; } = null!;
    public static string ColormapFile { get; internal set; } = null!;
    public static string WebDir { get; internal set; } = null!;
    public static string TilesDir { get; internal set; } = null!;

    internal static void ExtractWebFiles(LiveMapServer server) {
        GamePaths.EnsurePathExists(DataDir);
        // copy web assets from zip to disk
        // stored in "config" to allow the game to automatically load them for us
        foreach (IAsset asset in server.Api.Assets.GetMany("config", "livemap")) {
            // strip leading "config/" from the path
            string path = asset.Location.Path[7..];

            // ensure we actually have data
            if (asset.Data == null) {
                Logger.Error($"Error loading asset from zip &4{path}");
                continue;
            }

            // check if we've already saved this file to disk
            string destPath = Path.Combine(WebDir, path);
            if (File.Exists(destPath) && server.Config.Web.ReadOnly) {
                Logger.Debug($"Asset already exists on disk &6{path}");
                continue;
            }

            Logger.Debug($"Saving asset from zip to disk &6{path}");
            GamePaths.EnsurePathExists(Path.GetDirectoryName(destPath));
            File.WriteAllBytes(destPath, asset.Data);
        }
    }
}
