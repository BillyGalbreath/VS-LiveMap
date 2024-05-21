using Vintagestory.API.Common;

namespace livemap.logger;

internal class ServerLoggerImpl : LoggerImpl {
    protected override bool ColorConsole => true;
    protected override bool DebugToConsole => true;
    protected override bool DebugToEventFile => true;

    internal ServerLoggerImpl(string modid, ILogger logger) : base(modid, logger) { }
}
