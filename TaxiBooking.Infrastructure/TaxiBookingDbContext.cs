using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaxiBooking.Domain;
#nullable disable

namespace TaxiBooking.Infrastructure
{
    public class TaxiBookingDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public TaxiBookingDbContext(DbContextOptions<TaxiBookingDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use the UserConfiguration class for User entity configuration
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CarConfiguration());
            modelBuilder.ApplyConfiguration(new AttachmentConfiguration());

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
    }
}
