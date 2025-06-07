# 🌀 OxCore.QuantumQueue

**OxCore.QuantumQueue** is a lightweight, CRON-based job scheduler for .NET applications. Designed to be modular and developer-friendly, it lets you easily run background tasks with precise control over execution timing.

---

## 📦 Installation

Install the NuGet package:

### .NET CLI

```bash
dotnet add package OxCore.QuantumQueue.Core --version 9.0.3
```

### Package Manager

```powershell
Install-Package OxCore.QuantumQueue.Core --version 9.0.3
```

---

## ⚙️ Getting Started

To start using `OxCore.QuantumQueue`, follow these steps in your `.NET Core` app.

---

### 1⃣ Register Services

Add the following in your `Program.cs`:

```csharp
builder.Services.OxCoreService();
```

This registers all required services and job dependencies into the DI container.

---

### 2⃣ Enable Job Scheduler Middleware

After configuring your app and building it, add this line:

```csharp
app.UseJobScheduler();
```

This activates the job scheduler and runs all registered jobs based on their defined schedules.

---

```csharp
Configure Jobs in appsettings.json
```

Use a simplified format to declare CRON intervals:

```csharp
"JobSettings": {
   "SampleJob": "*/2 * * * *",
   "DataSyncJob": "0 */5 * * * *"
}
```

### 3⃣ Create a Job

Implement the `IJob` interface to define a recurring background task:

```csharp
using OxCore.QuantumQueue;

public class SampleJob : ConfigurableJobBase<FirstJob>, IJob
{
    public SampleJob(Func<string, ILogger> getLogger, IOptions<JobSettings> jobOptions)
        : base(getLogger, jobOptions) { }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"[SampleJob] Executed at: {DateTime.Now} with interval: {Interval}");
        await Task.Delay(30000, cancellationToken);
    }
}
```

---

## 🧪 Example: Data Sync Job

```csharp
public class DataSyncJob : ConfigurableJobBase<FirstJob>, IJob
{
    public DataSyncJob(Func<string, ILogger> getLogger, IOptions<JobSettings> jobOptions)
        : base(getLogger, jobOptions) { }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"[DataSyncJob] Executed at: {DateTime.Now} with interval: {Interval}");
        await MySyncService.SynchronizeAsync();
    }
}
```

### 4⃣ Register the Job

You do not need to manually register your job; it is automatically discovered and registered by the application.

---

## ⏰ CRON Expression Guide

| Field        | Allowed Values  | Description               |
| ------------ | --------------- | ------------------------- |
| Minute       | 0–59            | Minute of the hour        |
| Hour         | 0–23            | Hour of the day           |
| Day of month | 1–31            | Day of the month          |
| Month        | 1–12 or JAN–DEC | Month                     |
| Day of week  | 0–6 or SUN–SAT  | Day of the week (0 = Sun) |

### Examples:

* `*/5 * * * *` – every 5 minutes
* `0 0 * * *` – every day at midnight
* `0 12 * * 1` – every Monday at 12:00 PM

Use [crontab.guru](https://crontab.guru) to generate or validate CRON expressions.

---

## 🔐 Scoped Job Dependency Support

You can inject services inside your jobs using constructor injection:

```csharp
public class EmailJob : ConfigurableJobBase<FirstJob>, IJob
{
    private readonly IEmailService _emailService;

    public EmailJob(Func<string, ILogger> getLogger, IOptions<JobSettings> jobOptions, IEmailService emailService)
        : base(getLogger, jobOptions) 
    {
        _emailService = emailService;    
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _emailService.SendScheduledEmailsAsync();
    }
}
```

Make sure to register the dependency in `Program.cs`:

```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

---

## 🧹 Features

* ✅ Simple setup and lightweight design
* ⏱️ Flexible CRON-based scheduling
* ♻️ Supports scoped DI with 'ConfigurableJobBase<FirstJob>' and `IJob`
* 🥵 Thread-safe and cancellation-aware
* 🔄 Automatically discovers and runs all registered jobs
* 🧪 Testable architecture with modular components

---

## 🛠️ Advanced Configuration (coming soon)

Custom options for:

* Retry policies
* Error handling strategies
* Dynamic job enable/disable
* Job health monitoring

Stay tuned for future releases!

---

## 🧼 Best Practices

* Avoid blocking threads (use `async`/`await`)
* Use logging to trace job runs
* Wrap your job logic in try-catch blocks to prevent silent failures

---

## 📄 License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

---


## 🤝 Contributing

We welcome community contributions!

To contribute:

1. Fork the repo
2. Create a new feature branch
3. Submit a pull request

---

### Created with ❤️ by OxCore Team (Prabuddha Jayawardhana)
