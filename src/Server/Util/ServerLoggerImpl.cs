using LiveMap.Common.Util;
using LiveMap.Server.Configuration;

namespace LiveMap.Server.Util;

internal class ServerLoggerImpl : LoggerImpl {
    protected override bool ColorConsole() => Config.Instance.Logger.ColorConsole;
    protected override bool DebugToConsole() => Config.Instance.Logger.DebugToConsole;
    protected override bool DebugToEventFile() => Config.Instance.Logger.DebugToEventFile;
}
