using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OxCore.QuantumQueue.Application.Dto.Scheduler;
using OxCore.QuantumQueue.SharedKernel.Constants;
using System.Collections.Concurrent;
using System.Globalization;

namespace OxCore.QuantumQueue.Application.Services.Scheduler.Implementation;

public class CronJobScheduler : BackgroundService, IJobScheduler
{
    private readonly ConcurrentDictionary<string, IJob> _jobs = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _jobLocks = new();
    private readonly ConcurrentDictionary<string, DateTime> _nextRunTimes = new();
    private readonly ConcurrentDictionary<string, bool> _pausedJobs = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly Func<string, ILogger> _getLogger;

    public CronJobScheduler(IServiceProvider serviceProvider, Func<string, ILogger> getLogger)
    {
        _serviceProvider = serviceProvider;
        _getLogger = getLogger;
    }

    public void RegisterJob<T>() where T : IJob
    {
        using var scope = _serviceProvider.CreateScope();
        var job = scope.ServiceProvider.GetRequiredService<T>();

        string jobName = string.IsNullOrWhiteSpace(job.Name) ? typeof(T).Name : job.Name;
        var logger = _getLogger(jobName);

        if (_jobs.ContainsKey(jobName))
        {
            logger.LogWarning(string.Format(CronJobSchedulerConstants.AlreadyRegistered, jobName));
            return;
        }

        logger.LogInformation(string.Format(CronJobSchedulerConstants.RegisteringJob, jobName));
        _jobLocks.TryAdd(jobName, new SemaphoreSlim(1, 1));
        _jobs.TryAdd(jobName, job);
        _nextRunTimes.TryAdd(jobName, GetNextRunTime(job.Interval));
        _pausedJobs.TryAdd(jobName, false);
    }

    public void PauseJob(string jobName)
    {
        if (_jobs.ContainsKey(jobName))
        {
            _pausedJobs[jobName] = true;
            _getLogger(jobName).LogInformation(string.Format(CronJobSchedulerConstants.JobPaused, jobName));
        }
    }

    public void ResumeJob(string jobName)
    {
        if (_jobs.ContainsKey(jobName))
        {
            _pausedJobs[jobName] = false;
            _getLogger(jobName).LogInformation(string.Format(CronJobSchedulerConstants.JobResumed, jobName));
        }
    }

    public void DeregisterJob(string jobName)
    {
        if (_jobs.TryRemove(jobName, out _))
        {
            _jobLocks.TryRemove(jobName, out _);
            _nextRunTimes.TryRemove(jobName, out _);
            _pausedJobs.TryRemove(jobName, out _);
            _getLogger(jobName).LogInformation(string.Format(CronJobSchedulerConstants.JobDeregistered, jobName));
        }
        else
        {
            _getLogger(jobName).LogWarning(string.Format(CronJobSchedulerConstants.JobNotFoundDeregister, jobName));
        }
    }

    public List<ScheduledJobDto> GetScheduledJobs()
    {
        return _jobs.Select(j => new ScheduledJobDto
        {
            JobName = j.Key,
            Interval = j.Value.Interval,
            IsPaused = _pausedJobs.TryGetValue(j.Key, out var paused) && paused
        }).ToList();
    }

    public DateTime? GetNextRunTimeInJob(string jobName)
    {
        if (_nextRunTimes.TryGetValue(jobName, out var nextRun))
            return nextRun;

        return null;
    }

    public Task Run(string jobName)
    {
        if (!_jobs.TryGetValue(jobName, out var job))
            throw new InvalidOperationException(string.Format(CronJobSchedulerConstants.JobNotFound, jobName));

        return Task.Run(async () =>
        {
            var logger = _getLogger(job.Name);
            logger.LogInformation(string.Format(CronJobSchedulerConstants.TriggeringJob, jobName));
            await DoWork(job, logger, CancellationToken.None);
        });
    }

    private async Task DoWork(IJob job, ILogger logger, CancellationToken stoppingToken)
    {
        var jobLock = _jobLocks[job.Name];

        if (!await jobLock.WaitAsync(0, stoppingToken))
        {
            logger.LogWarning(string.Format(CronJobSchedulerConstants.JobRunning, job.Name));
            return;
        }

        try
        {
            logger.LogInformation(string.Format(CronJobSchedulerConstants.ExecutingJob, job.Name));
            _nextRunTimes[job.Name] = GetNextRunTime(job.Interval);
            await job.ExecuteAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, string.Format(CronJobSchedulerConstants.ErrorExecutingJob, job.Name));
        }
        finally
        {
            jobLock.Release();
            logger.LogInformation(string.Format(CronJobSchedulerConstants.JobCompleted, job.Name));
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (_jobs.Count == 0)
                continue;

            foreach (var job in _jobs.Values)
            {
                if (_pausedJobs.TryGetValue(job.Name, out var isPaused) && isPaused)
                {
                    var logger = _getLogger(job.Name);
                    logger.LogInformation(string.Format(CronJobSchedulerConstants.JobIsPaused, job.Name));
                    continue;
                }

                if (!_nextRunTimes.TryGetValue(job.Name, out var nextRunTime))
                    continue;

                if (DateTime.UtcNow >= nextRunTime)
                {
                    var logger = _getLogger(job.Name);
                    _ = Task.Run(async () => await DoWork(job, logger, stoppingToken), stoppingToken);
                }
            }
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var jobTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        foreach (var jobType in jobTypes)
        {
            var method = typeof(IJobScheduler).GetMethod(nameof(CronJobScheduler.RegisterJob))!
                .MakeGenericMethod(jobType);
            method.Invoke(this, null);
        }

        await base.StartAsync(cancellationToken);
    }

    private DateTime GetNextRunTime(string cronExpression)
    {
        var parts = cronExpression.Split(' ');
        if (parts.Length != 5)
            throw new ArgumentException(CronJobSchedulerConstants.InvalidCronExpression);

        var now = DateTime.UtcNow;
        int minute = ParseCronPart(parts[0], now.Minute, 0, 59);
        int hour = ParseCronPart(parts[1], now.Hour, 0, 23);
        int day = ParseCronPart(parts[2], now.Day, 1, DateTime.DaysInMonth(now.Year, now.Month));
        int month = ParseCronPart(parts[3], now.Month, 1, 12);
        int dayOfWeek = parts[4] == "*" ? (int)now.DayOfWeek : int.Parse(parts[4], CultureInfo.InvariantCulture);

        if (day > DateTime.DaysInMonth(now.Year, month))
        {
            day = 1;
            month++;
            if (month > 12) month = 1;
        }

        var nextRun = new DateTime(now.Year, month, day, hour, minute, 0, DateTimeKind.Utc);
        if (nextRun <= now)
        {
            nextRun = nextRun.AddMinutes(1);
        }
        return nextRun;
    }

    private int ParseCronPart(string part, int currentValue, int minValue, int maxValue)
    {
        if (part == "*") return currentValue;
        
        // Handle comma-separated values
        if (part.Contains(","))
        {
            var values = part.Split(',')
                .Select(p => int.Parse(p.Trim(), CultureInfo.InvariantCulture))
                .Where(v => v >= minValue && v <= maxValue)
                .OrderBy(v => v)
                .ToList();

            if (!values.Any())
                throw new ArgumentException(string.Format(CronJobSchedulerConstants.InvalidStepValue, part));

            // Find the next value that is greater than or equal to current value
            var nextValue = values.FirstOrDefault(v => v >= currentValue);
            return nextValue != 0 ? nextValue : values[0];
        }

        if (part.StartsWith("*/"))
        {
            if (int.TryParse(part.Substring(2), out int step))
            {
                int nextValue = ((currentValue / step) + 1) * step;
                return nextValue <= maxValue ? nextValue : minValue;
            }
            throw new ArgumentException(string.Format(CronJobSchedulerConstants.InvalidStepValue, part));
        }

        int parsedValue = int.Parse(part, CultureInfo.InvariantCulture);
        return Math.Clamp(parsedValue, minValue, maxValue);
    }
}
