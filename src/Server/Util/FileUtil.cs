using System.IO;
using Vintagestory.API.Config;

namespace LiveMap.Server.Util;

public abstract class FileUtil {
    public static readonly string ConfigFile = Path.Combine(GamePaths.ModConfig, $"{LiveMapMod.Id}.yml");
    public static readonly string WebDir = Path.Combine(GamePaths.DataPath, "ModData", "LiveMap");
}
