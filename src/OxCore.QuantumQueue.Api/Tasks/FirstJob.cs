using OxCore.QuantumQueue.Application.Services.Scheduler;

namespace OxCore.QuantumQueue.Api.Tasks;

public class FirstJob : IJob
{
    public string Name => "FirstJob";
    public string Interval => "*/1 * * * *"; // Runs every 2 minutes
    private readonly ILogger _logger;

    public FirstJob(Func<string, ILogger> getLogger)
    {
        _logger = getLogger(Name);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("FirstJob started.");
        await Task.Delay(30000, cancellationToken);
        _logger.LogInformation("FirstJob completed.");
    }
}