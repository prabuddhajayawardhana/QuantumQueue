namespace OxCore.QuantumQueue.Application.Services.Scheduler;

public interface IJob
{
    string Name => GetType().Name;
    string Interval { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}