using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class ServiceTypePartConfiguration : IEntityTypeConfiguration<ServiceTypePart>
    {
        public void Configure(EntityTypeBuilder<ServiceTypePart> builder)
        {
            builder.ToTable("ServiceTypeParts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Supplier)
                .HasMaxLength(100);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(x => x.ServiceType)
                .WithMany(st => st.Parts)
                .HasForeignKey(x => x.ServiceTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
