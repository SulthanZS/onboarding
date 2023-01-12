using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OB_Net_Core_Tutorial.Data;
using OB_Net_Core_Tutorial.Models;

namespace OB_Net_Core_Tutorial.Controllers
{

    public class CarsController : Controller
    {
        private readonly OB_Net_Core_TutorialContext _context;

        public CarsController(OB_Net_Core_TutorialContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Index()
        {
              return _context.Car != null ? 
                          View(await _context.Car.ToListAsync()) :
                          Problem("Entity set 'OB_Net_Core_TutorialContext.Car'  is null.");
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
        public async Task<IActionResult> Details([FromRoute] int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        /// <summary>
        /// Retrieves create view
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

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
        [ValidateAntiForgeryToken]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(415)]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        /// <summary>
        /// Retrieves edit view
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="404">not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute] int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Edit car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        /// <response code="415">Unsupported media type</response>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(415)]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id,Name")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Delete/5

        /// <summary>
        /// Retrieve delete view
        /// </summary>
        /// <param name="id">id</param>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        /// <summary>
        /// Delete car
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(405)]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            if (_context.Car == null)
            {
                return Problem("Entity set 'OB_Net_Core_TutorialContext.Car'  is null.");
            }
            var car = await _context.Car.FindAsync(id);
            if (car != null)
            {
                _context.Car.Remove(car);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
          return (_context.Car?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
