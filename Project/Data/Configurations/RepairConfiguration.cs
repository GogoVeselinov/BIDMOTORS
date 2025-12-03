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

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.LaborCost)
                .HasPrecision(18, 2);

            builder.Property(r => r.TotalCost)
                .HasPrecision(18, 2);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.StartDate)
                .IsRequired();

            builder.HasOne(r => r.ServiceRequest)
                .WithMany(sr => sr.Repairs)
                .HasForeignKey(r => r.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Car)
                .WithMany(car => car.Repairs)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Employee)
                .WithMany(e => e.Repairs)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.UsedParts)
                .WithOne(up => up.Repair)
                .HasForeignKey(up => up.RepairId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
