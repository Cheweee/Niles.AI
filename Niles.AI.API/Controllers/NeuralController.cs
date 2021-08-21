using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Niles.AI.API.Services;
using Niles.AI.Models;
using Niles.AI.Services.Interfaces;

namespace Niles.AI.API
{
    [Route("api/neuralnetwork")]
    [ApiController]
    public class NeuralNetworkController : ControllerBase
    {
        private readonly NeuralNetworkService _service;

        public NeuralNetworkController(NeuralNetworkService service)
        {
            _service = service ?? throw new ArgumentException(nameof(service));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetInstance()
        {
            _service.GetInstance();
            return Ok();
        }


        [AllowAnonymous]
        [HttpPut]
        public IActionResult Build(NeuralNetworkBuildOptions options)
        {
            _service.Build(options);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("train")]
        public IActionResult Train(NeuralNetworkTrainOptions options)
        {
            _service.Train(options);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Activate(NeuralNetworkActivateOptions options)
        {
            _service.Activate(options);

            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete]
        public IActionResult ClearInstance()
        {
            _service.ClearInstance();

            return Ok();
        }
    }
}