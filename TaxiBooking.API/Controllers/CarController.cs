using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxiBooking.Application.Cars;
using TaxiBooking.Application.DTO;

namespace TaxiBooking.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCars(int pageNumber = 1, int pageSize = 10)
        {
            var paginatedCars = await _carService.GetAllCars(pageNumber, pageSize);
            return Ok(paginatedCars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _carService.GetCarById(id);
            return car != null ? Ok(car) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] CarViewModel car)
        {
            if (car == null)
            {
                return BadRequest("Invalid car data");
            }
            var result = await _carService.AddCar(car);
            return CreatedAtAction(nameof(GetCarById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] CarViewModel car)
        {
            if (car == null || id != car.Id)
            {
                return BadRequest("Invalid car data");
            }
            await _carService.UpdateCar(car);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(long id)
        {
            await _carService.DeleteCar(id);
            return NoContent();
        }
    }
}