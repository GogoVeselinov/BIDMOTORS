using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class UsedPartConfiguration : IEntityTypeConfiguration<UsedPart>
    {
        public void Configure(EntityTypeBuilder<UsedPart> builder)
        {
            builder.HasKey(up => up.Id);

            builder.Property(up => up.QuantityUsed)
                .IsRequired();

            builder.Property(up => up.UnitPriceAtMoment)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(up => up.TotalPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(up => up.Repair)
                .WithMany(r => r.UsedParts)
                .HasForeignKey(up => up.RepairId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.Part)
                .WithMany(p => p.UsedParts)
                .HasForeignKey(up => up.PartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
