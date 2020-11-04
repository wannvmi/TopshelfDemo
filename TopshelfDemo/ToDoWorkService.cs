using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TopshelfDemo
{
    public class ToDoWorkService
    {
        private readonly ILogger<ToDoWorkService> _logger;

        public ToDoWorkService(ILogger<ToDoWorkService> logger)
        {
            _logger = logger;
        }

        public void SaveMessage(string message)
        {
            // TODO some work 

            // TODO some work 

            _logger.LogInformation("Save Message {@message}", message);
        }
    }
}
