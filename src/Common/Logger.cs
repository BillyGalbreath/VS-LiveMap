using System;
using System.Diagnostics;
using Vintagestory.API.Common;
using Vintagestory.Common;

namespace LiveMap.Common;

public abstract class Logger {
    private static ILogger? _logger;

    private static ILogger Log() {
        return _logger ??= new LoggerImpl();
    }

    public static void Info(string message) {
        Log().Event(message);
    }

    public static void Warn(string message) {
        Log().Warning(message);
    }

    public static void Error(string message) {
        Log().Error(message);
    }
}

internal class LoggerImpl : LoggerBase {
    private const string Cyan = "\u001b[36m";
    private const string Red = "\u001b[91m";
    private const string Yellow = "\u001b[33m";
    private const string Reset = "\u001b[0m";

    private bool canUseColor = true;

    protected override void LogImpl(EnumLogType logType, string format, params object[] args) {
        string formatted = $"[{LiveMapMod.Id}] {format}";
        
        Vintagestory.Logger parent = (Vintagestory.Logger)((ModLogger)LiveMapMod.Instance.Mod.Logger).Parent;

        string logFile = parent.getLogFile(logType);
        if (logFile != null) {
            try {
                parent.LogToFile(logFile, logType, formatted, args);
            } catch (NotSupportedException) {
                Console.WriteLine("Unable to write to log file " + logFile);
            } catch (ObjectDisposedException) {
                Console.WriteLine("Unable to write to log file " + logFile);
            }
        }

        switch (logType) {
            case EnumLogType.Error or EnumLogType.Fatal:
                parent.LogToFile(parent.getLogFile(EnumLogType.Event), logType, formatted, args);
                break;
            case EnumLogType.Event:
                parent.LogToFile(parent.getLogFile(EnumLogType.Notification), logType, formatted, args);
                break;
        }

        if (!parent.printToConsole(logType)) {
            return;
        }

        if (parent.TraceLog) {
            Trace.WriteLine(parent.FormatLogEntry(logType, formatted, args));
        }

        if (canUseColor) {
            try {
                Console.ForegroundColor = logType switch {
                    EnumLogType.Warning => ConsoleColor.DarkYellow,
                    EnumLogType.Error or EnumLogType.Fatal => ConsoleColor.Red,
                    _ => Console.ForegroundColor
                };
            } catch (Exception) {
                canUseColor = false;
            }
        }

        if (!canUseColor) {
            Console.WriteLine(parent.FormatLogEntry(logType, formatted, args));
            return;
        }

        Console.WriteLine(parent.FormatLogEntry(logType, $"[{Cyan}{LiveMapMod.Id}{logType switch {
            EnumLogType.Error or EnumLogType.Fatal => Red,
            EnumLogType.Warning => Yellow,
            _ => Reset
        }}] {format}", args));


        Console.ResetColor();
    }
}
