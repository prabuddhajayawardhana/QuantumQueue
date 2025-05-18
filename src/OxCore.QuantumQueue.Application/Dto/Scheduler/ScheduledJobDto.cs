namespace OxCore.QuantumQueue.Application.Dto.Scheduler;

public class ScheduledJobDto
{
    public string JobName { get; set; } = string.Empty;
    public string Interval { get; set; } = string.Empty;
    public bool IsPaused { get; set; }
}