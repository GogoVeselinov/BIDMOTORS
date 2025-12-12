using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.Id);

            // Връзка към ServiceRequest (много към едно)
            builder.HasOne(s => s.ServiceRequest)
                .WithMany()
                .HasForeignKey(s => s.ServiceRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Връзка към ServiceType (много към едно)
            builder.HasOne(s => s.ServiceType)
                .WithMany()
                .HasForeignKey(s => s.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Връзка към AssignedEmployee (много към едно, nullable)
            builder.HasOne(s => s.AssignedEmployee)
                .WithMany()
                .HasForeignKey(s => s.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Връзка към Tasks (едно към много)
            builder.HasMany(s => s.Tasks)
                .WithOne(st => st.Service)
                .HasForeignKey(st => st.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Връзка към PartLinks (едно към много)
            builder.HasMany(s => s.PartLinks)
                .WithOne(spl => spl.Service)
                .HasForeignKey(spl => spl.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Properties
            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.Notes)
                .HasMaxLength(2000);

            builder.Property(s => s.Result)
                .HasMaxLength(2000);

            builder.Property(s => s.StartedOn)
                .IsRequired(false);

            builder.Property(s => s.CompletedOn)
                .IsRequired(false);
        }
    }
}
