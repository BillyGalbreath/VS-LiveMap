using livemap.common.util;

namespace livemap.client.util;

internal class ClientLoggerImpl : LoggerImpl {
    protected override bool ColorConsole() => false;
    protected override bool DebugToConsole() => false;
    protected override bool DebugToEventFile() => false;
}
