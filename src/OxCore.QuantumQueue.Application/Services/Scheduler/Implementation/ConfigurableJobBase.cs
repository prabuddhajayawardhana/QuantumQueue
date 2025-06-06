using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OxCore.QuantumQueue.SharedKernel.Configuration;
using OxCore.QuantumQueue.SharedKernel.Constants;

namespace OxCore.QuantumQueue.Application.Services.Scheduler.Implementation;
public abstract class ConfigurableJobBase<TJob> where TJob : class
{
    public string Name => typeof(TJob).Name;
    public string Interval { get; }
    protected readonly ILogger Logger;

    protected ConfigurableJobBase(Func<string, ILogger> getLogger, IOptions<JobSettings> jobOptions)
    {
        Logger = getLogger(Name);
        var settings = jobOptions.Value;

        if (settings.TryGetValue(Name, out var interval))
        {
            Interval = interval;
        }
        else
        {
            throw new InvalidOperationException(string.Format(CronJobSchedulerConstants.ConfigurableJobBaseException, Name));
        }
    }
}
