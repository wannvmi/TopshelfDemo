using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RabbitMQSender
{
    public class Worker : BackgroundService
    {
        const string QUEUE = "demo_queue";

        private readonly IBus _bus;

        private readonly ILogger<Worker> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public Worker(IBus bus, ILogger<Worker> logger, IHostEnvironment hostEnvironment)
        {
            _bus = bus;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Environment: {@_hostEnvironment}", _hostEnvironment);

            while (!stoppingToken.IsCancellationRequested)
            {
                _bus.Send(QUEUE, $"Worker running at: {DateTime.Now}");

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
