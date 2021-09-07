using DevCars.API.Entities;
using DevCars.API.InputModels;
using DevCars.API.Persistence;
using DevCars.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DevCars.API.Controllers
{
    [Route("api/cars")]
    public class CarsController : ControllerBase
    {

        //Parei no video em 01:29:16 quando ele vai gerar as migrations

        private readonly DevCarsDbContext _dbContext;
        public CarsController(DevCarsDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var cars = _dbContext.Cars;

            var carsViewModel = cars
                .Where(c => c.Status == CarStatusEnum.Available)
                .Select(c => new CarItemViewModel(c.Id, c.Brand, c.Model, c.Price))
                .ToList();

            return Ok(carsViewModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var car = _dbContext.Cars.SingleOrDefault(c => c.Id == id);

            if (car == null)
                return NotFound();

            var CarDetailsViewModel = new CarDetailsViewModel(
                car.Id,
                car.Brand,
                car.Model,
                car.VinCode,
                car.Year,
                car.Price,
                car.Color,
                car.ProductionDate);

            return Ok(CarDetailsViewModel);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AddCarInputModel model)
        {
            var car = new Car(model.VinCode, model.Brand, model.Model, model.Year, model.Price, model.Color, model.ProductionDate);

            _dbContext.Cars.Add(car);
            _dbContext.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new {id = car.Id},
                model
                );
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateCarInputModel model)
        {

            var car = _dbContext.Cars.SingleOrDefault(c => c.Id == id);

            if (car == null)
                return NotFound();

            car.Update(model.Color, model.Price);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var car = _dbContext.Cars.SingleOrDefault(c => c.Id == id);

            if (car == null)
                return NotFound();

            car.SetAsSuspended();

            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
