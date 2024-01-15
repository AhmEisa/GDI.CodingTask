#nullable disable

namespace TaxiBooking.Domain
{
    public class Attachment
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public long CarId { get; set; }
    }
}