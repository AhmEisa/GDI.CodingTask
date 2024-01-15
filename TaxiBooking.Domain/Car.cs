#nullable disable

namespace TaxiBooking.Domain
{
    public class Car
    {
        public long Id { get; set; }
        public string CarNumber { get; set; }
        public string CarColor { get; set; }
        public string CarModel { get; set; }
        public DateTime RegistrationExpiryDate { get; set; }
        public int YearOfManufacture { get; set; }
        public string OwnerName { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}