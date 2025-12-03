using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class RepairConfiguration : IEntityTypeConfiguration<Repair>
    {
        public void Configure(EntityTypeBuilder<Repair> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.WorkDescription)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.Price)
                .HasPrecision(18, 2);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            builder.HasOne(r => r.Client)
                .WithMany(c => c.Repairs)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Car)
                .WithMany(car => car.Repairs)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ServiceRequest)
                .WithOne(sr => sr.LinkedRepair)
                .HasForeignKey<Repair>(r => r.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.UsedParts)
                .WithOne(up => up.Repair)
                .HasForeignKey(up => up.RepairId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
