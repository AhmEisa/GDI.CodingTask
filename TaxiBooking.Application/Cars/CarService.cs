using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using TaxiBooking.Application.DTO;
using TaxiBooking.Domain;
using TaxiBooking.Infrastructure;

namespace TaxiBooking.Application.Cars
{
    public class CarService : ICarService
    {
        private readonly TaxiBookingDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<CarViewModel> _validator;
        public CarService(TaxiBookingDbContext dbContext, IMapper mapper, IValidator<CarViewModel> validator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<PaginatedCars> GetAllCars(int page, int pageSize)
        {
            var totalRecords = await _dbContext.Cars.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var cars = await _dbContext.Cars
                .Include(c => c.Attachments).AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedCars
            {
                Cars = _mapper.Map<IEnumerable<CarViewModel>>(cars),
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<CarViewModel?> GetCarById(long carId)
        {
            return _mapper.Map<CarViewModel>(await _dbContext.Cars.Include(c => c.Attachments).AsNoTracking().FirstOrDefaultAsync(c => c.Id == carId));
        }

        public async Task<CarViewModel> AddCar(CarViewModel carViewModel)
        {
            ValidationResult validationResult = _validator.Validate(carViewModel);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var carEntity = _mapper.Map<Car>(carViewModel);
            if (carEntity == null)
            {
                throw new ArgumentNullException(nameof(carEntity));
            }

            await _dbContext.Cars.AddAsync(carEntity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CarViewModel>(carEntity);
        }

        public async Task UpdateCar(CarViewModel carModel)
        {
            var carEntity = await _dbContext.Cars.AsNoTracking().FirstOrDefaultAsync(c => c.Id == carModel.Id);
            if (carEntity == null)
            {
                throw new ArgumentNullException(nameof(carEntity));
            }
            carEntity = _mapper.Map<Car>(carModel);
            _dbContext.Cars.Update(carEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCar(long carId)
        {
            var carToDelete = _dbContext.Cars.Find(carId);

            if (carToDelete != null)
            {
                _dbContext.Cars.Remove(carToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
