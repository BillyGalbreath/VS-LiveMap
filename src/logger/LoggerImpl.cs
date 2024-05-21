using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Vintagestory.API.Common;
using Vintagestory.Common;

namespace livemap.logger;

public abstract partial class LoggerImpl : LoggerBase {
    [GeneratedRegex("(?i)&([a-f0-9k-or])", RegexOptions.None, "en-US")]
    private static partial Regex ColorCodesRegex();

    [GeneratedRegex(@"(?i)\u001b\[[\d]{1,2}m", RegexOptions.None, "en-US")]
    private static partial Regex AnsiCodesRegex();

    private static readonly Dictionary<string, int> _ansiCodes = new() {
        { "0", 30 },
        { "1", 34 },
        { "2", 32 },
        { "3", 36 },
        { "4", 31 },
        { "5", 35 },
        { "6", 33 },
        { "7", 37 },
        { "8", 90 },
        { "9", 94 },
        { "a", 92 },
        { "b", 96 },
        { "c", 91 },
        { "d", 95 },
        { "e", 93 },
        { "f", 97 },
        { "k", 8 },
        { "l", 1 },
        { "m", 9 },
        { "n", 4 },
        { "o", 3 },
        { "r", 0 }
    };

    protected abstract bool ColorConsole { get; }
    protected abstract bool DebugToConsole { get; }
    protected abstract bool DebugToEventFile { get; }

    private readonly string _modid;
    protected readonly Vintagestory.Logger _parent;

    private bool _canUseColor = true;

    protected LoggerImpl(string modid, ILogger logger) {
        _modid = modid;
        _parent = (Vintagestory.Logger)((ModLogger)logger).Parent;
    }

    protected override void LogImpl(EnumLogType logType, string format, params object[] args) {
        string stripped = $"[{_modid}] {Strip(format)}";

        PrintToCorrectLogFile(_parent, logType, stripped, args);

        if (!_parent.printToConsole(logType)) {
            return;
        }

        if (logType == EnumLogType.Debug && !DebugToConsole) {
            return;
        }

        if (_parent.TraceLog) {
            Trace.WriteLine(_parent.FormatLogEntry(logType, stripped, args));
        }

        SetupColorsOrNot(logType);

        WriteToLog(_parent, logType, format, stripped, args);

        Console.ResetColor();
    }

    private void PrintToCorrectLogFile(Vintagestory.Logger parent, EnumLogType logType, string stripped, params object[] args) {
        string? logFile = parent.getLogFile(logType);
        if (logFile != null) {
            try {
                parent.LogToFile(logFile, logType, stripped, args);
            } catch (Exception e) when (e is NotSupportedException or ObjectDisposedException) {
                Console.WriteLine("Unable to write to log file " + logFile);
            }
        }

        switch (logType) {
            case EnumLogType.Error or EnumLogType.Fatal:
                parent.LogToFile(parent.getLogFile(EnumLogType.Event), logType, stripped, args);
                break;
            case EnumLogType.Event:
            case EnumLogType.Debug when DebugToEventFile:
                parent.LogToFile(parent.getLogFile(EnumLogType.Notification), logType, stripped, args);
                break;
        }
    }

    private void SetupColorsOrNot(EnumLogType logType) {
        if (!ColorConsole || !_canUseColor) {
            return;
        }
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

    private void WriteToLog(Vintagestory.Logger parent, EnumLogType logType, string format, string stripped, params object[] args) {
        if (!ColorConsole || !_canUseColor) {
            Console.WriteLine(parent.FormatLogEntry(logType, stripped, args));
            return;
        }

        Console.WriteLine(parent.FormatLogEntry(logType, Parse(
            $"[&3{_modid}{logType switch {
                EnumLogType.Debug => "&e",
                EnumLogType.Error or EnumLogType.Fatal => "&c",
                EnumLogType.Warning => "&6",
                _ => "&r"
            }}] {format}&r"
        ), args));
    }

    protected static string Strip(string message) {
        return ColorCodesRegex().Replace(AnsiCodesRegex().Replace(message, ""), "");
    }

    private static string Parse(string message) {
        return ColorCodesRegex().Matches(message).Aggregate(message, (current, match) =>
            current.Replace(match.Value, $"\u001b[{_ansiCodes[match.Groups[1].Value]}m")
        );
    }
}
