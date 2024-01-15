using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaxiBooking.Domain;

namespace TaxiBooking.Infrastructure
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users"); 

            builder.HasKey(e => e.Id); 

            builder.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.PasswordHash)
                .IsRequired();

            builder.HasData(new User { Id = 1, Username = "user@gdi.com", PasswordHash = "$2a$11$2fuGVTwW3JdiCuR/NInAK.BUuPdepLHTGJxyyv6x7Rt3dSWNi3d9e" });
        }
    }
}
