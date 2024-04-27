using Vintagestory.API.Config;

namespace livemap.common.extensions;

public static class LangExtensions {
    public static string ToLang(this string key, params object[]? args) {
        return Lang.Get($"{LiveMapMod.Id}:{key}", args);
    }
}
