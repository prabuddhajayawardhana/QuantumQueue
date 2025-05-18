namespace OxCore.QuantumQueue.Application.Services.Scheduler;

public interface IJob
{
    /// <summary>
    /// Gets the name of the current type.
    /// </summary>
    string Name => GetType().Name;

    /// <summary>
    /// Gets the interval value represented as a string.
    /// </summary>
    string Interval { get; }

    /// <summary>
    /// Executes an asynchronous operation.
    /// </summary>
    /// <remarks>The operation will respect the provided <paramref name="cancellationToken"/> and terminate
    /// early if cancellation is requested.</remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task ExecuteAsync(CancellationToken cancellationToken);
}