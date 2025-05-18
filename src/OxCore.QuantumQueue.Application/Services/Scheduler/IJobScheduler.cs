using OxCore.QuantumQueue.Application.Dto.Scheduler;

namespace OxCore.QuantumQueue.Application.Services.Scheduler;

public interface IJobScheduler
{
    /// <summary>
    /// Registers a new job of type T.
    /// </summary>
    void RegisterJob<T>() where T : IJob;

    /// <summary>
    /// Manually triggers a job by name.
    /// <param name="jobName"></param>
    /// </summary>
    Task Run(string jobName);

    /// <summary>
    /// Returns a list of all scheduled jobs with their metadata.
    /// </summary>
    List<ScheduledJobDto> GetScheduledJobs();

    /// <summary>
    /// Pauses the execution of a job by name.
    /// <param name="jobName"></param>
    /// </summary>
    void PauseJob(string jobName);

    /// <summary>
    /// Resumes the execution of a previously paused job.
    /// <param name="jobName"></param>
    /// </summary>
    void ResumeJob(string jobName);

    /// <summary>
    /// Completely removes a job from the scheduler.
    /// <param name="jobName"></param>
    /// </summary>
    void DeregisterJob(string jobName);

    /// <summary>
    /// Returns the next scheduled run time for a job by name.
    /// </summary>
    /// <param name="jobName"></param>
    /// <returns></returns>
    DateTime? GetNextRunTimeInJob(string jobName);
}