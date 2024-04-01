namespace LiveMap.Common.Util;

public abstract class Logger {
    private static readonly LoggerImpl _loggerImpl = null!;

    public static void Debug(string message) {
        _loggerImpl.Debug(message);
    }

    public static void Info(string message) {
        _loggerImpl.Event(message);
    }

    public static void Warn(string message) {
        _loggerImpl.Warning(message);
    }

    public static void Error(string message) {
        _loggerImpl.Error(message);
    }
}
