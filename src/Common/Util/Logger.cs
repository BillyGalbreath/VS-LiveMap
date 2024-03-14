using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using LiveMap.Common.Configuration;
using Vintagestory.API.Common;
using Vintagestory.Common;

namespace LiveMap.Common.Util;

public static class Logger {
    private static readonly LoggerImpl Log = new();

    public static void Debug(string message) {
        Log.Debug(message);
    }

    public static void Info(string message) {
        Log.Event(message);
    }

    public static void Warn(string message) {
        Log.Warning(message);
    }

    public static void Error(string message) {
        Log.Error(message);
    }
}

internal partial class LoggerImpl : LoggerBase {
    [GeneratedRegex("(?i)&([a-f0-9k-or])", RegexOptions.None, "en-US")]
    private static partial Regex ColorCodesRegex();

    [GeneratedRegex(@"(?i)\u001b\[[\d]{1,2}m", RegexOptions.None, "en-US")]
    private static partial Regex AnsiCodesRegex();

    private static readonly Dictionary<string, int> AnsiCodes = new() {
        { "0", 30 }, { "1", 34 }, { "2", 32 }, { "3", 36 }, { "4", 31 }, { "5", 35 },
        { "6", 33 }, { "7", 37 }, { "8", 90 }, { "9", 94 }, { "a", 92 }, { "b", 96 },
        { "c", 91 }, { "d", 95 }, { "e", 93 }, { "f", 97 }, { "k", 8 }, { "l", 1 },
        { "m", 9 }, { "n", 4 }, { "o", 3 }, { "r", 0 }
    };

    private bool _canUseColor = true;

    private static string Strip(string message) {
        return ColorCodesRegex().Replace(AnsiCodesRegex().Replace(message, ""), "");
    }

    private static string Parse(string message) {
        MatchCollection results = ColorCodesRegex().Matches(message);

        foreach (Match match in results) {
            message = message.Replace(match.Value, $"\u001b[{AnsiCodes[match.Groups[1].Value]}m");
        }

        return message;
    }

    protected override void LogImpl(EnumLogType logType, string format, params object[] args) {
        string stripped = $"[{LiveMapMod.Id}] {Strip(format)}";

        Vintagestory.Logger? parent = (Vintagestory.Logger)((ModLogger)LiveMapMod.Instance.Mod.Logger).Parent;

        // print to the correct log file
        string? logFile = parent.getLogFile(logType);
        if (logFile != null) {
            try {
                parent.LogToFile(logFile, logType, stripped, args);
            } catch (NotSupportedException) {
                Console.WriteLine("Unable to write to log file " + logFile);
            } catch (ObjectDisposedException) {
                Console.WriteLine("Unable to write to log file " + logFile);
            }
        }

        // also print these types to other files
        switch (logType) {
            case EnumLogType.Error or EnumLogType.Fatal:
                parent.LogToFile(parent.getLogFile(EnumLogType.Event), logType, stripped, args);
                break;
            case EnumLogType.Event:
            case EnumLogType.Debug when Config.Instance.Logger.DebugToEventFile:
                parent.LogToFile(parent.getLogFile(EnumLogType.Notification), logType, stripped, args);
                break;
        }

        if (!parent.printToConsole(logType)) {
            return;
        }

        if (logType == EnumLogType.Debug && !Config.Instance.Logger.DebugToConsole) {
            return;
        }

        if (parent.TraceLog) {
            Trace.WriteLine(parent.FormatLogEntry(logType, stripped, args));
        }

        if (Config.Instance.Logger.ColorConsole && _canUseColor) {
            try {
                Console.ForegroundColor = logType switch {
                    EnumLogType.Debug => ConsoleColor.Yellow,
                    EnumLogType.Warning => ConsoleColor.DarkYellow,
                    EnumLogType.Error or EnumLogType.Fatal => ConsoleColor.Red,
                    _ => Console.ForegroundColor
                };
            } catch (Exception) {
                try {
                    Console.ResetColor();
                } catch (Exception) {
                    // ignore
                }

                _canUseColor = false;
            }
        }

        if (!Config.Instance.Logger.ColorConsole || !_canUseColor) {
            Console.WriteLine(parent.FormatLogEntry(logType, stripped, args));
            return;
        }

        Console.WriteLine(parent.FormatLogEntry(logType, Parse(
            $"[&3{LiveMapMod.Id}{logType switch {
                EnumLogType.Debug => "&e",
                EnumLogType.Error or EnumLogType.Fatal => "&c",
                EnumLogType.Warning => "&6",
                _ => "&r"
            }}] {format}&r"
        ), args));

        Console.ResetColor();
    }
}
