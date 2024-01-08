namespace LiveMap.Common;

public abstract class Lang {
    public static string Get(string key, params object[]? args) {
        return Vintagestory.API.Config.Lang.Get($"{LiveMapMod.Id}:{key}", args);
    }

    public static string Error(string key, params object[]? args) {
        return Get("error-format", Get(key, args));
    }

    public static string Success(string key, params object[]? args) {
        return Get("success-format", Get(key, args));
    }
}
