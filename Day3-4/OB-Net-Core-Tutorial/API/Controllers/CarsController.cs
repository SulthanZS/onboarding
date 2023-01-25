using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using AutoMapper;
using StackExchange.Redis;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using OB_Net_Core_Tutorial.DAL.Data;
using OB_Net_Core_Tutorial.DAL.Models;
using OB_Net_Core_Tutorial.DAL.Repository;
using OB_Net_Core_Tutorial.BLL.DTO;
using OB_Net_Core_Tutorial.BLL.Service;
using OB_Net_Core_Tutorial.BLL.Interface;
using Azure.Messaging.EventHubs.Producer;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;
using OBNetCoreTutorial.BLL.Interface;

namespace OB_Net_Core_Tutorial.API.Controllers
{

    public class CarsController : Controller
    {
        private readonly OB_Net_Core_TutorialContext _context;
        private UnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private readonly IPublisherService _publisherService;
        private readonly ICarService _carService;
        private IConfiguration _config;



        public CarsController(OB_Net_Core_TutorialContext context, UnitOfWork unitOfWork, IRedisService redisService, IConfiguration config, IPublisherService publisherService, ICarService carService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _redisService = redisService;
            _config = config;
            _publisherService = publisherService;
            _carService = carService;

        }

        
        // GET: Cars
        /// <summary>
        /// Retrieves index
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Route("")]
        public String Index()
        {
            return "test";
        }


        // GET: Cars
        /// <summary>
        /// Retrieves all car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Route("/GetAllCars")]
        public async Task<Object> GetAllCar()
        {
            return _carService.GetAllCar();
        }


        // GET: Cars/Details/5
        /// <summary>
        /// Retrieves car by Id
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Route("Details/{id}")]
        public async Task<Object> Details([FromRoute] Guid id)
        {
            return _carService.GetCarById(id);
        }


        // POST: Cars/Create
        /// <summary>
        /// Add new car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Route("Create")]
        public async Task<ActionResult> Create([FromBody] CarDTO carDTO)
        {
            await _carService.CreateCarAsync(carDTO);
            return Ok(carDTO);
        }


        /// <summary>
        /// Edit car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Route("Edit/{id}")]
        public Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] CarDTO carDTO)
        {
            return _carService.EditCar(id, carDTO);
        }


        // DELETE: Cars/Delete/5
        /// <summary>
        /// Delete car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] Guid id)
        {
            _carService.DeleteCar(id);
            return new OkResult();
        }

        private bool CarExists(Guid id)
        {
            return (_context.Car?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
