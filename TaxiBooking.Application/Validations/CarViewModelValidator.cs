using FluentValidation;
using TaxiBooking.Application.DTO;

namespace TaxiBooking.Application.Validations
{
    public class CarViewModelValidator : AbstractValidator<CarViewModel>
    {
        public CarViewModelValidator()
        {
            RuleFor(car => car.CarNumber).NotEmpty().MaximumLength(20);
            RuleFor(car => car.CarColor).NotEmpty().MaximumLength(20);
            RuleFor(car => car.CarModel).NotEmpty().MaximumLength(50);
            RuleFor(car => car.RegistrationExpiryDate).NotEmpty().GreaterThan(DateTime.UtcNow);
            RuleFor(car => car.YearOfManufacture).NotEmpty().Must(BeAValidYear);
            RuleFor(car => car.OwnerName).NotEmpty().MaximumLength(100);
        }

        private bool BeAValidYear(int year)
        {
            return year >= 1900 && year <= DateTime.UtcNow.Year;
        }
    }
}

