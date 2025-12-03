using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.ServiceType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(sr => sr.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(sr => sr.Comment)
                .HasMaxLength(1000);

            builder.Property(sr => sr.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            builder.Property(sr => sr.InternalNotes)
                .HasMaxLength(2000);

            builder.HasOne(sr => sr.Client)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sr => sr.Car)
                .WithMany(car => car.ServiceRequests)
                .HasForeignKey(sr => sr.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sr => sr.LinkedRepair)
                .WithOne(r => r.ServiceRequest)
                .HasForeignKey<ServiceRequest>(sr => sr.LinkedRepairId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
