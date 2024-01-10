using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Vintagestory.API.Common;
using Vintagestory.Common;

namespace LiveMap.Common.Util;

public abstract class Logger {
    private static readonly LoggerImpl Log = new();

    public static void Debug(string message) {
        Log.Event($"[&eDEBUG&r] &e{message}");
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

[SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to \'GeneratedRegexAttribute\'.")]
internal class LoggerImpl : LoggerBase {
    private static readonly Regex Regex = new("(?i)&([a-f0-9k-or])");

    private bool canUseColor = true;

    private static readonly Dictionary<string, int> AnsiCodes = new() {
        { "0", 30 }, { "1", 34 }, { "2", 32 }, { "3", 36 }, { "4", 31 }, { "5", 35 },
        { "6", 33 }, { "7", 37 }, { "8", 90 }, { "9", 94 }, { "a", 92 }, { "b", 96 },
        { "c", 91 }, { "d", 95 }, { "e", 93 }, { "f", 97 }, { "k", 8 }, { "l", 1 },
        { "m", 9 }, { "n", 4 }, { "o", 3 }, { "r", 0 }
    };

    private static string Strip(string message) {
        message = Regex.Replace(message, "(?i)\u001b\\[[\\d]{1,2}m", "");
        return Regex.Replace(message, "(?i)&([a-f0-9k-or])", "");
    }

    private static string Parse(string message) {
        MatchCollection results = Regex.Matches(message);

        foreach (Match match in results) {
            message = message.Replace(
                match.Value,
                $"\u001b[{AnsiCodes[match.Groups[1].Value]}m"
            );
        }

        return message;
    }

    protected override void LogImpl(EnumLogType logType, string format, params object[] args) {
        string stripped = $"[{LiveMapMod.Id}] {Strip(format)}";

        Vintagestory.Logger parent = (Vintagestory.Logger)((ModLogger)LiveMapMod.Instance.Mod.Logger).Parent;

        string logFile = parent.getLogFile(logType);
        if (logFile != null) {
            try {
                parent.LogToFile(logFile, logType, stripped, args);
            } catch (NotSupportedException) {
                Console.WriteLine("Unable to write to log file " + logFile);
            } catch (ObjectDisposedException) {
                Console.WriteLine("Unable to write to log file " + logFile);
            }
        }

        switch (logType) {
            case EnumLogType.Error or EnumLogType.Fatal:
                parent.LogToFile(parent.getLogFile(EnumLogType.Event), logType, stripped, args);
                break;
            case EnumLogType.Event:
                parent.LogToFile(parent.getLogFile(EnumLogType.Notification), logType, stripped, args);
                break;
        }

        if (!parent.printToConsole(logType)) {
            return;
        }

        if (parent.TraceLog) {
            Trace.WriteLine(parent.FormatLogEntry(logType, stripped, args));
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
            Console.WriteLine(parent.FormatLogEntry(logType, stripped, args));
            return;
        }

        Console.WriteLine(parent.FormatLogEntry(logType, Parse(
            $"[&3{LiveMapMod.Id}{logType switch {
                EnumLogType.Error or EnumLogType.Fatal => "&c",
                EnumLogType.Warning => "&e",
                _ => "&r"
            }}] {format}"
        ), args));


        Console.ResetColor();
    }
}
