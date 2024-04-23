using livemap.common.util;
using livemap.server.configuration;

namespace livemap.server.util;

internal class ServerLoggerImpl : LoggerImpl {
    protected override bool ColorConsole() => Config.Instance.Logger.ColorConsole;
    protected override bool DebugToConsole() => Config.Instance.Logger.DebugToConsole;
    protected override bool DebugToEventFile() => Config.Instance.Logger.DebugToEventFile;
}
