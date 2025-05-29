using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//// Ocelot Basic setup
//builder.Configuration
//    .SetBasePath(builder.Environment.ContentRootPath)
//    .AddOcelot("ocelot-configuration", builder.Environment);

builder.Configuration
                   .SetBasePath(builder.Environment.ContentRootPath)
                   .AddJsonFile("appsettings.json", true, true)
                   .AddJsonFile($"appsettings.{builder.Environment}.json", true, true)
                   .AddJsonFile("ocelot.json")
                   .AddEnvironmentVariables();

builder.Services
    .AddOcelot();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddSerilog(logger);

// Add middlewares aka app.Use*()
var app = builder.Build();
await app.UseOcelot();
await app.RunAsync();