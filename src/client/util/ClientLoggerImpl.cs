using livemap.common.util;

namespace livemap.client.util;

internal class ClientLoggerImpl : LoggerImpl {
    protected override bool ColorConsole => true;
    protected override bool DebugToConsole => true;
    protected override bool DebugToEventFile => false;
}
