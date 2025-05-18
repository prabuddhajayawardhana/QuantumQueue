using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace OxCore.QuantumQueue.Api.Middleware;

public class ReactAppMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public ReactAppMiddleware(RequestDelegate next, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _next = next;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip if the request is for an API endpoint
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        // In development, proxy to the React dev server
        if (_environment.IsDevelopment())
        {
            var reactDevUrl = _configuration["ReactApp:DevelopmentUrl"];
            if (!string.IsNullOrEmpty(reactDevUrl))
            {
                context.Response.Redirect($"{reactDevUrl}{context.Request.Path}{context.Request.QueryString}");
                return;
            }
        }

        // In production, serve the static files
        var buildPath = _configuration["ReactApp:BuildPath"];
        if (!string.IsNullOrEmpty(buildPath))
        {
            var fileProvider = new PhysicalFileProvider(Path.GetFullPath(buildPath));
            var fileInfo = fileProvider.GetFileInfo(context.Request.Path.Value ?? "/index.html");

            if (fileInfo.Exists)
            {
                context.Response.ContentType = GetContentType(fileInfo.Name);
                await context.Response.SendFileAsync(fileInfo);
                return;
            }
        }

        await _next(context);
    }

    private string GetContentType(string fileName)
    {
        return fileName switch
        {
            var f when f.EndsWith(".html") => "text/html",
            var f when f.EndsWith(".css") => "text/css",
            var f when f.EndsWith(".js") => "application/javascript",
            var f when f.EndsWith(".json") => "application/json",
            var f when f.EndsWith(".png") => "image/png",
            var f when f.EndsWith(".jpg") || f.EndsWith(".jpeg") => "image/jpeg",
            var f when f.EndsWith(".gif") => "image/gif",
            var f when f.EndsWith(".svg") => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
} 