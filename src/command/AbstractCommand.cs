using JetBrains.Annotations;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command;

[PublicAPI]
public abstract class AbstractCommand {
    protected readonly LiveMap _server;

    public string[] Name { get; }
    public string Description { get; }
    public string Privilege { get; }
    public bool RequiresPlayer;
    public ICommandArgumentParser[] ArgParsers;

    protected AbstractCommand(LiveMap server, string[] name, string? privilege = null, bool requiresPlayer = false, params ICommandArgumentParser[] argParsers) {
        _server = server;

        Name = name;
        Description = $"command.{Name[0]}.description".ToLang();
        Privilege = privilege ?? Vintagestory.API.Server.Privilege.root;
        RequiresPlayer = requiresPlayer;
        ArgParsers = argParsers;
    }

    public abstract TextCommandResult Execute(TextCommandCallingArgs args);
}
