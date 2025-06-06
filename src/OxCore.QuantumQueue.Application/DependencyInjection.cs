using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OxCore.QuantumQueue.Application.Services.Scheduler;
using OxCore.QuantumQueue.Application.Services.Scheduler.Implementation;
using OxCore.QuantumQueue.SharedKernel.Configuration;
using OxCore.QuantumQueue.SharedKernel.Constants;

namespace OxCore.QuantumQueue.Application;

public static class DependencyInjection
{
    public static IServiceCollection OxCoreService(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<JobSettings>(configuration.GetSection(SystemConstants.JobSettings));
        // Register logger factory
        services.AddSingleton<Func<string, ILogger>>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return category => loggerFactory.CreateLogger(category);
        });

        // Auto-register jobs on startup
        // Discover and register all IJob implementations as Transient
        var jobTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        foreach (var type in jobTypes)
        {
            services.AddScoped(type);
            services.AddScoped(typeof(IJob), type);
        }

        // Register scheduler 
        services.AddSingleton<IJobScheduler, CronJobScheduler>();
        services.AddHostedService<CronJobScheduler>();

        return services;
    }
}
