namespace LiveMap.Common;

public abstract class Lang {
    public static string Get(string key, params object[]? args) {
        return Vintagestory.API.Config.Lang.Get($"{LiveMapMod.Id}:{key}", args);
    }

    public static string Error(string key, params object[]? args) {
        return Get("command.error", Get(key, args));
    }

    public static string Success(string key, params object[]? args) {
        return Get("command.success", Get(key, args));
    }
}
