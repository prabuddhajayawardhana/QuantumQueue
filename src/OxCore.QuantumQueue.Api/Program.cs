using OxCore.QuantumQueue.Application;
using OxCore.QuantumQueue.Application.Extensions;
using OxCore.QuantumQueue.SharedKernel.Helper;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using OxCore.QuantumQueue.Api.Middleware;
using OxCore.QuantumQueue.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Logging Configuration
// Ensure Logs directory exists
var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
Directory.CreateDirectory(logDirectory);

// Configure Serilog globally for normal services (controllers, services)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logDirectory, "app_log.log"), rollingInterval: RollingInterval.Day)
.CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddSingleton<Func<string, ILogger>>(sp => LoggerFactoryExtension.GetLoggerForService);

// Add services to the container.
builder.Services.OxCoreService(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add React Dev Server Service
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<ReactDevServerService>();
}

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add React app middleware before routing
app.UseMiddleware<ReactAppMiddleware>();

app.UseJobScheduler();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
