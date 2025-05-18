using Serilog;
using Serilog.Extensions.Logging;
using System.Collections.Concurrent;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OxCore.QuantumQueue.SharedKernel.Helper;

public static class LoggerFactoryExtension
{
    private static readonly string LogDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    private static readonly ConcurrentDictionary<string, ILogger> Loggers = new();

    static LoggerFactoryExtension()
    {
        Directory.CreateDirectory(LogDirectory);
    }

    public static ILogger GetLoggerForService(string serviceName)
    {
        var serviceLogDirectory = Path.Combine(LogDirectory, serviceName);
        Directory.CreateDirectory(serviceLogDirectory);
        string logFileName = $"{serviceName}_log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";

        return Loggers.GetOrAdd(serviceName, name =>
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(serviceLogDirectory, logFileName), rollingInterval: RollingInterval.Infinite)
                .CreateLogger();
            return new SerilogLoggerFactory(logger).CreateLogger(name);
        });
    }
}