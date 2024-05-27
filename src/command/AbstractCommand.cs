using JetBrains.Annotations;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command;

[PublicAPI]
public abstract class AbstractCommand {
    public LiveMap Server { get; }

    public string[] Name { get; }
    public string Description { get; }
    public string Privilege { get; }

    protected AbstractCommand(LiveMap server, string[] name, string? privilege = null) {
        Server = server;

        Name = name;
        Description = $"command.{Name[0]}.description".ToLang();
        Privilege = privilege ?? Vintagestory.API.Server.Privilege.root;
    }

    public abstract CommandResult Execute(TextCommandCallingArgs args);
}
