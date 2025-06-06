namespace OxCore.QuantumQueue.SharedKernel.Constants;

public static class CronJobSchedulerConstants
{
    public const string InvalidCronExpression = "Invalid cron expression format.";
    public const string InvalidStepValue = "Invalid step value in cron expression: {0}";
    public const string AlreadyRegistered = "Job '{0}' is already registered.";
    public const string RegisteringJob = "Registering job: {0}";
    public const string JobPaused = "Job '{0}' has been paused.";
    public const string JobResumed = "Job '{0}' has been resumed.";
    public const string JobDeregistered = "Job '{0}' has been deregistered.";
    public const string JobNotFoundDeregister = "Job '{0}' was not found during deregistration.";
    public const string JobNotFound = "Job '{0}' not found.";
    public const string TriggeringJob = "Manually triggering job: {0}";
    public const string JobRunning = "Job '{0}' is already running. Skipping execution.";
    public const string ExecutingJob = "Executing job: {0}";
    public const string JobCompleted = "Job '{0}' completed successfully.";
    public const string ErrorExecutingJob = "Error executing job '{0}'.";
    public const string JobIsPaused = "Job '{0}' is paused. Skipping.";
    public const string ConfigurableJobBaseException = "Interval for job '{Name}' not found in JobSettings.";
    
}