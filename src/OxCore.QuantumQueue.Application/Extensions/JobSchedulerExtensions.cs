using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OxCore.QuantumQueue.Application.Services.Scheduler;
using OxCore.QuantumQueue.Application.Services.Scheduler.Implementation;

namespace OxCore.QuantumQueue.Application.Extensions;

public static class JobSchedulerExtensions
{
    public static void UseJobScheduler(this WebApplication app)
    {
        var jobTypes = AppDomain.CurrentDomain.GetAssemblies()
           .SelectMany(a => a.GetTypes())
           .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
           .ToList();

        // Auto-register jobs on startup
        using (var scope = app.Services.CreateScope())
        {
            var scheduler = scope.ServiceProvider.GetRequiredService<IJobScheduler>();

            foreach (var jobType in jobTypes)
            {
                var method = typeof(IJobScheduler).GetMethod(nameof(CronJobScheduler.RegisterJob))!.MakeGenericMethod(jobType);
                method.Invoke(scheduler, null);
            }
        }
    }
}