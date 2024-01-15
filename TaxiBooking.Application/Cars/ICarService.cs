using TaxiBooking.Application.DTO;

namespace TaxiBooking.Application.Cars
{
    public interface ICarService
    {
        Task<PaginatedCars> GetAllCars(int page, int pageSize);
        Task<CarViewModel?> GetCarById(long carId);
        Task<CarViewModel> AddCar(CarViewModel car);
        Task UpdateCar(CarViewModel car);
        Task DeleteCar(long carId);
    }

}
