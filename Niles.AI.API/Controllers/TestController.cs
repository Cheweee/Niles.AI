using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Niles.AI.Models.Settings;
using Niles.AI.Services;
using RabbitMQ.Client;

namespace Niles.AI.API.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly ILogger<TestController> _logger;

        public TestController(RabbitMQService rabbitMQService, ILogger<TestController> logger)
        {
            _rabbitMQService = rabbitMQService ?? throw new ArgumentNullException(nameof(rabbitMQService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [AllowAnonymous]
        public void SendMessage()
        {
            _rabbitMQService.Send(new RabbitMQQueueOptions
            {
                Name = "test_queue",
            }, "test test test");
        }
    }
}