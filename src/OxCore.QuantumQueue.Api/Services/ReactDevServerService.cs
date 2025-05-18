using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace OxCore.QuantumQueue.Api.Services;

public class ReactDevServerService : IHostedService
{
    private readonly ILogger<ReactDevServerService> _logger;
    private Process? _reactProcess;

    public ReactDevServerService(ILogger<ReactDevServerService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Get the solution root directory (two levels up from the API project)
            var solutionRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "OxCore.QuantumQueue.Api", ".."));
            var clientPath = Path.Combine(solutionRoot, "Client");
            
            if (!Directory.Exists(clientPath))
            {
                _logger.LogWarning("React client directory not found at: {ClientPath}", clientPath);
                return Task.CompletedTask;
            }

            _logger.LogInformation("Starting React development server...");
            
            // First, ensure dependencies are installed
            var installInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c cd /d {clientPath} && npm install",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var installProcess = new Process { StartInfo = installInfo })
            {
                installProcess.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        _logger.LogInformation("npm install: {Message}", args.Data);
                };
                installProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        _logger.LogError("npm install Error: {Message}", args.Data);
                };

                installProcess.Start();
                installProcess.BeginOutputReadLine();
                installProcess.BeginErrorReadLine();
                installProcess.WaitForExit();
            }

            // Then start the development server
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c cd /d {clientPath} && npm start",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _reactProcess = new Process { StartInfo = startInfo };
            _reactProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    _logger.LogInformation("React: {Message}", args.Data);
            };
            _reactProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    _logger.LogError("React Error: {Message}", args.Data);
            };

            _reactProcess.Start();
            _reactProcess.BeginOutputReadLine();
            _reactProcess.BeginErrorReadLine();

            _logger.LogInformation("React development server started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start React development server");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_reactProcess != null && !_reactProcess.HasExited)
            {
                _logger.LogInformation("Stopping React development server...");
                _reactProcess.Kill(true);
                _reactProcess.Dispose();
                _logger.LogInformation("React development server stopped");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping React development server");
        }

        return Task.CompletedTask;
    }
} 