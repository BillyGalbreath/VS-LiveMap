using LiveMap.Common.Util;

namespace LiveMap.Client.Util;

internal class ClientLoggerImpl : LoggerImpl {
    protected override bool ColorConsole() => false;
    protected override bool DebugToConsole() => false;
    protected override bool DebugToEventFile() => false;
}
