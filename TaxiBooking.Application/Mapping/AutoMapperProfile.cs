using AutoMapper;
using TaxiBooking.Application.DTO;
using TaxiBooking.Domain;

namespace TaxiBooking.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Car, CarViewModel>().ReverseMap();
            CreateMap<Attachment, AttachmentViewModel>().ReverseMap();
        }
    }
}
