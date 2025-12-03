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

            builder.Property(sr => sr.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(sr => sr.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(sr => sr.RequestDate)
                .IsRequired();

            builder.HasOne(sr => sr.Client)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sr => sr.Car)
                .WithMany(car => car.ServiceRequests)
                .HasForeignKey(sr => sr.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sr => sr.ServiceType)
                .WithMany(st => st.ServiceRequests)
                .HasForeignKey(sr => sr.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(sr => sr.Repairs)
                .WithOne(r => r.ServiceRequest)
                .HasForeignKey(r => r.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
