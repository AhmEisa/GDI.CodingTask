using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaxiBooking.Domain;

namespace TaxiBooking.Infrastructure
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.ToTable("Cars");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.CarNumber)
                .IsRequired()
                .HasMaxLength(20);
            builder.HasIndex(e => e.CarNumber).IsUnique();

            builder.Property(e => e.CarColor)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.CarModel)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.RegistrationExpiryDate)
                .IsRequired();

            builder.Property(e => e.YearOfManufacture)
                .IsRequired();

            builder.Property(e => e.OwnerName)
                .IsRequired()
                .HasMaxLength(100);

           
            // Configure the relationship with Attachment
            builder.HasMany(c => c.Attachments)
                .WithOne()
                .HasForeignKey(a => a.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}