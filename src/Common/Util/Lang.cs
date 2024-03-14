using LiveMap.Common.Command;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace LiveMap.Common.Util;

public static class Lang {
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

public static class LangExtensions {
    public static void SendMessage(this Caller caller, CommandResult result) {
        if (caller.Player != null) {
            caller.Player.SendMessage(result);
            return;
        }

        Logger.Info($"{(result.Status == EnumCommandStatus.Error ? "&c" : "&a")}{Lang.Get(result.Message, result.Args)}&r");
    }

    public static void SendMessage(this IPlayer player, CommandResult result) {
        player.SendMessage(result.Status == EnumCommandStatus.Error ? Lang.Error(result.Message, result.Args) : Lang.Success(result.Message, result.Args));
    }

    public static void SendMessage(this IPlayer player, string message) {
        if (string.IsNullOrEmpty(message)) {
            return;
        }

        switch (player) {
            case IServerPlayer sPlayer:
                sPlayer.SendMessage(GlobalConstants.CurrentChatGroup, message, EnumChatType.Notification);
                break;
            case IClientPlayer cPlayer:
                ((ICoreClientAPI)cPlayer.Entity.World.Api).ShowChatMessage(message);
                break;
        }
    }
}
