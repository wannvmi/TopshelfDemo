using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TopshelfDemo
{
    public class Worker : BackgroundService
    {
        const string QUEUE = "demo_queue";
        private static int _countTime = 1;

        private readonly IServiceProvider _serviceProvider;
        private readonly IBus _bus;

        private readonly ILogger<Worker> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public Worker(IServiceProvider serviceProvider, IBus bus, ILogger<Worker> logger, IHostEnvironment hostEnvironment)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Environment: {@_hostEnvironment}", _hostEnvironment);

            _bus.Receive<string>(QUEUE, message => SaveMessage(message));

            await Task.CompletedTask;
        }

        void SaveMessage(string message)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var toDoWorkService = scope.ServiceProvider.GetService<ToDoWorkService>();
                    toDoWorkService.SaveMessage(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                _logger.LogInformation("Worker.SaveMessage 已保存{@_countTime}条数据", _countTime++);
            }
        }


    }
}
