using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class ServiceTaskConfiguration : IEntityTypeConfiguration<ServiceTask>
    {
        public void Configure(EntityTypeBuilder<ServiceTask> builder)
        {
            builder.HasKey(st => st.Id);

            // Връзка към Service (много към едно)
            builder.HasOne(st => st.Service)
                .WithMany(s => s.Tasks)
                .HasForeignKey(st => st.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Връзка към CompletedByEmployee (много към едно, nullable)
            builder.HasOne(st => st.CompletedByEmployee)
                .WithMany()
                .HasForeignKey(st => st.CompletedByEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Properties
            builder.Property(st => st.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(st => st.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(st => st.CompletedOn)
                .IsRequired(false);

            builder.Property(st => st.Notes)
                .HasMaxLength(1000);
        }
    }
}
