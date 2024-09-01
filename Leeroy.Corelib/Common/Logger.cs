using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace Leeroy.Corelib.Common;

public static class Logger {
    private const string _logLevel = "TRACE";
    private const byte MaxCallerNameLength = 50;

    private static Serilog.Core.Logger Log { get; set; } = new LoggerConfiguration()
        .MinimumLevel.ControlledBy(new LoggingLevelSwitch { MinimumLevel = GetLogLevel(_logLevel) })
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "{Level:u3} {CallingSpace} : {Message:lj} {NewLine}{Exception}")
        .CreateLogger();

    public static object[] Args(params object[] args) => args;

    public static void Verbose(
        string                    message,
        object[]?                 args = null,
        [CallerFilePath] string   callingClass = "",
        [CallerMemberName] string callingMethod = "",
        [CallerLineNumber] int    lineNumber = 0
    ) {
        WriteLog(callingClass, callingMethod, lineNumber, LogEventLevel.Verbose, message, args ?? []);
    }
    
    public static void Debug(
        string                    message,
        object[]?                 args = null,
        [CallerFilePath] string   callingClass = "",
        [CallerMemberName] string callingMethod = "",
        [CallerLineNumber] int    lineNumber = 0
    ) {
        WriteLog(callingClass, callingMethod, lineNumber, LogEventLevel.Debug, message, args ?? []);
    }
    
    public static void Information(
        string                    message,
        object[]?                 args = null,
        [CallerFilePath] string   callingClass = "",
        [CallerMemberName] string callingMethod = "",
        [CallerLineNumber] int    lineNumber = 0
    ) {
        WriteLog(callingClass, callingMethod, lineNumber, LogEventLevel.Information, message, args ?? []);
    }
    
    public static void Warning(
        string                    message,
        object[]?                 args = null,
        [CallerFilePath] string   callingClass = "",
        [CallerMemberName] string callingMethod = "",
        [CallerLineNumber] int    lineNumber = 0
    ) {
        WriteLog(callingClass, callingMethod, lineNumber, LogEventLevel.Warning, message, args ?? []);
    }
    
    public static void Error(
        string                    message,
        object[]?                 args = null,
        [CallerFilePath] string   callingClass = "",
        [CallerMemberName] string callingMethod = "",
        [CallerLineNumber] int    lineNumber = 0
    ) {
        WriteLog(callingClass, callingMethod, lineNumber, LogEventLevel.Error, message, args ?? []);
    }
    
    public static void Fatal(
        string                    message,
        object[]?                 args = null,
        [CallerFilePath] string   callingClass = "",
        [CallerMemberName] string callingMethod = "",
        [CallerLineNumber] int    lineNumber = 0
    ) {
        WriteLog(callingClass, callingMethod, lineNumber, LogEventLevel.Fatal, message, args ?? []);
    }

    private static void WriteLog(
        string          callingClass,
        string          callingMethod,
        int             lineNumber,
        LogEventLevel   logLevel,
        string          message,
        params object[] values
    ) {
        callingClass = TrimCallingClass(callingClass);

        var callingSpace = $"{callingClass}.{callingMethod}@L{lineNumber}";
        callingSpace = GetConsistentSpacedName(callingSpace);

        LogContext.PushProperty("CallingSpace", callingSpace);

        var seen = new HashSet<object?>();
        foreach (var value in values) {
            if (!seen.Add(value)) {
                continue;
            }

            if (value is not string and not ValueType) {
                LogContext.PushProperty(value.GetType().Name, value, true);
            } else {
                LogContext.PushProperty(value.GetType().Name, value);
            }
        }

        Log.Write(logLevel, message, values);
    }

    private static LogEventLevel GetLogLevel(string logLevelString) {
        logLevelString = logLevelString.Trim();
        logLevelString = logLevelString.Replace("\"", string.Empty);

        return logLevelString.ToUpper() switch {
            "TRACE" => LogEventLevel.Verbose,
            "DEBUG" => LogEventLevel.Debug,
            "INFO" => LogEventLevel.Information,
            "WARNING" => LogEventLevel.Warning,
            "ERROR" => LogEventLevel.Error,
            "FATAL" => LogEventLevel.Fatal,

            _ => throw new Exception($"Invalid log level: {logLevelString}"),
        };
    }

    private static string TrimCallingClass(string filePath) {
        if (!filePath.Contains(".cs")) {
            return filePath;
        }

        var separatorChar = Path.DirectorySeparatorChar;
        var startIndex = filePath.LastIndexOf(separatorChar) + 1;
        var length = filePath.LastIndexOf(".cs", StringComparison.Ordinal) - startIndex;

        return filePath.Substring(startIndex, length);
    }

    private static string GetConsistentSpacedName(string name) {
        if (!string.IsNullOrEmpty(name)) {
            return name.Length > MaxCallerNameLength
                ? name[..MaxCallerNameLength]
                : name.PadRight(MaxCallerNameLength);
        }

        return "Main".PadRight(MaxCallerNameLength);
    }
}
