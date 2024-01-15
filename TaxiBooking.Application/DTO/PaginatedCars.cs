#nullable disable


namespace TaxiBooking.Application.DTO
{
    public class PaginatedCars
    {
        public IEnumerable<CarViewModel> Cars { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
