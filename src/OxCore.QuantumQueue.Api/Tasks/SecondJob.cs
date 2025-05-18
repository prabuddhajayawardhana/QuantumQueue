using OxCore.QuantumQueue.Application.Services.Scheduler;

namespace OxCore.QuantumQueue.Api.Tasks;

public class SecondJob : IJob
{
    public string Name => "SecondJob";
    public string Interval => "*/1 * * * *"; // Runs every 2 minutes
    private readonly ILogger _logger;

    public SecondJob(Func<string, ILogger> getLogger)
    {
        _logger = getLogger(Name);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SecondJob started.");
        await Task.Delay(30000, cancellationToken);
        _logger.LogInformation("SecondJob completed.");
    }
}