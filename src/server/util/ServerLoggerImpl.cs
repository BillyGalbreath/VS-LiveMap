using livemap.common.util;

namespace livemap.server.util;

internal class ServerLoggerImpl : LoggerImpl {
    protected override bool ColorConsole => true;
    protected override bool DebugToConsole => true;
    protected override bool DebugToEventFile => true;
}
