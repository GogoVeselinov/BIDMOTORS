using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class PartConfiguration : IEntityTypeConfiguration<Part>
    {
        public void Configure(EntityTypeBuilder<Part> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Category)
                .HasMaxLength(100);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.QuantityInStock)
                .IsRequired();

            builder.Property(p => p.Supplier)
                .HasMaxLength(200);

            builder.HasMany(p => p.UsedParts)
                .WithOne(up => up.Part)
                .HasForeignKey(up => up.PartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
