namespace LiveMap.Common.Util;

public abstract class Logger {
    internal static LoggerImpl LoggerImpl { get; set; } = null!;

    public static void Debug(string message) {
        LoggerImpl.Debug(message);
    }

    public static void Info(string message) {
        LoggerImpl.Event(message);
    }

    public static void Warn(string message) {
        LoggerImpl.Warning(message);
    }

    public static void Error(string message) {
        LoggerImpl.Error(message);
    }
}
