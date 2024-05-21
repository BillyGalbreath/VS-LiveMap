using Vintagestory.API.Common;

namespace livemap.logger;

internal class ClientLoggerImpl : LoggerImpl {
    protected override bool ColorConsole => true;
    protected override bool DebugToConsole => true;
    protected override bool DebugToEventFile => false;

    internal ClientLoggerImpl(string modid, ILogger logger) : base(modid, logger) { }
}
