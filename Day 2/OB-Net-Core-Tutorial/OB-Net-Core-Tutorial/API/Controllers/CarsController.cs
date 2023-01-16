using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OB_Net_Core_Tutorial.DAL.Data;
using OB_Net_Core_Tutorial.DAL.Models;
using OB_Net_Core_Tutorial.DAL.Repository;
using OB_Net_Core_Tutorial.BLL.DTO;
using Org.BouncyCastle.Crypto;
using AutoMapper;

namespace OB_Net_Core_Tutorial.API.Controllers
{

    public class CarsController : Controller
    {
        private readonly OB_Net_Core_TutorialContext _context;
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public CarsController(OB_Net_Core_TutorialContext context, UnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;

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
        [Route("")]
        public IEnumerable<Car> Index()
        {
            return _unitOfWork.CarRepository.GetAll();

            //var wrapModel = new WrapModel();

            //if (_context.Car != null)
            //{
            //    wrapModel.Cars = await _context.Car.ToListAsync();
            //    return (View(wrapModel));
            //}
            //return Problem("Entity set 'OB_Net_Core_TutorialContext.Car'  is null.");
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
        public IEnumerable<Car> Details([FromRoute] Guid? id)
        {
            return _unitOfWork.CarRepository.GetAll().Where(x => x.Id == id);
            //if (id == null || _context.Car == null)
            //{
            //    return NotFound();
            //}

            //var car = await _context.Car
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (car == null)
            //{
            //    return NotFound();
            //}

            //return View(car);
        }

        // GET: Cars/Create
        /// <summary>
        /// Retrieves create view
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">not found</response>
        //[HttpGet]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //[Route("Create")]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Cars/Create

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Add new car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        /// <response code="415">method not allowed</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(415)]
        [Route("Create")]
        public ActionResult Create([FromBody] CarDTO carDTO)
        {
            //var newCar = _mapper.Map<Car>(carDTO);
            Car newCar = new Car();
            newCar.Name= carDTO.Name;
            newCar.TypeId=carDTO.TypeId;

            _unitOfWork.CarRepository.Add(newCar);
            _unitOfWork.Save();
            return new OkObjectResult(newCar);

            //_context.Add(newCar);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        // GET: Cars/Edit/5
        /// <summary>
        /// Retrieves edit view
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">not found</response>
        //[HttpGet]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //[Route("Edit/{id}")]
        //public async Task<IActionResult> Edit([FromRoute] Guid id)
        //{
        //    if (id == null || _context.Car == null)
        //    {
        //        return NotFound();
        //    }

        //    var car = _unitOfWork.CarRepository.GetByIdGuid(id);
        //    if (car == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(car);
        //}

        // PUT: Cars/Edit/5

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Edit car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        /// <response code="415">Unsupported media type</response>
        [HttpPut]
        //[ValidateAntiForgeryToken]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(415)]
        [Route("Edit/{id}")]
        public IActionResult Edit([FromRoute] Guid id, [Bind("Id,Name,TypeId")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }
            var Editcar = _unitOfWork.CarRepository.GetByIdGuid(id);
            Editcar.Name = car.Name;

            _unitOfWork.CarRepository.Edit(Editcar);
            _unitOfWork.Save();

            //_context.Update(car);
            //_context.SaveChanges();

            return new OkObjectResult(car);

        }

        // GET: Cars/Delete/5

        /// <summary>
        /// Retrieve delete view
        /// </summary>
        /// <param name="id">id</param>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        //[HttpGet]
        //[ProducesResponseType(200)]
        //[Route("Delete/{id}")]
        //public async Task<IActionResult> Delete([FromRoute] Guid id)
        //{
        //    if (id == null || _context.Car == null)
        //    {
        //        return NotFound();
        //    }

        //    var car = _unitOfWork.CarRepository.GetByIdGuid(id);

        //    if (car == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(car);
        //}

        // POST: Cars/Delete/5
        /// <summary>
        /// Delete car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(405)]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] Guid id)
        {
            //if (_context.Car == null)
            //{
            //    return Problem("Entity set 'OB_Net_Core_TutorialContext.Car'  is null.");
            //}
            //var car = await _context.Car.FindAsync(id);
            //if (car != null)
            //{
            //    _context.Car.Remove(car);
            //}

            var car = _unitOfWork.CarRepository.GetByIdGuid(id);
            _unitOfWork.CarRepository.Delete(car);
            _unitOfWork.Save();

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));

            return new OkObjectResult(car);
        }

        private bool CarExists(Guid id)
        {
            return (_context.Car?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
