using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

namespace RabbitMQSender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        const string OutputInfoTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}{NewLine}";
        const string OutputPropTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Properties:l}{NewLine}{Exception}{NewLine}";

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
#if DEBUG
                .UseEnvironment(Environments.Development)
#else
                .UseEnvironment(Environments.Staging)
                //.UseEnvironment(Environments.Production)
#endif
                .UseContentRoot(AppContext.BaseDirectory)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.WithExceptionDetails()
                        .Enrich.FromLogContext()
                        .WriteTo.Debug()
                        .WriteTo.Console(theme: SystemConsoleTheme.Colored)
                        .WriteTo.File(AppContext.BaseDirectory + "log/log.txt",
                            outputTemplate: OutputInfoTemplate, rollingInterval: RollingInterval.Day)
                        .WriteTo.File(AppContext.BaseDirectory + "log/error.txt", LogEventLevel.Warning,
                            outputTemplate: OutputPropTemplate, rollingInterval: RollingInterval.Day);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.RegisterEasyNetQ(hostContext.Configuration.GetConnectionString("RabbitMQ"));

                    services.AddHostedService<Worker>();

                });
    }
}
