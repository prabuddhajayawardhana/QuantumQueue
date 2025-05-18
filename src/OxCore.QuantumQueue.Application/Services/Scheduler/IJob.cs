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
    /// <remarks>This method performs an operation asynchronously and supports cancellation through the
    /// provided <paramref name="cancellationToken"/>. Callers can use the returned <see cref="Task"/> to await the
    /// completion of the operation or handle exceptions that may occur during execution.</remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation before it completes.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task ExecuteAsync(CancellationToken cancellationToken);
}