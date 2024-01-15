using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaxiBooking.Domain;

namespace TaxiBooking.Infrastructure
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable("Attachments");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
          
            builder.Property(e => e.Url)
               .IsRequired()
               .HasMaxLength(255);
        }
    }

}
