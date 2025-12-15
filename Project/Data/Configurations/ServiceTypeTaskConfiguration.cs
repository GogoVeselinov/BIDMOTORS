using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class ServiceTypeTaskConfiguration : IEntityTypeConfiguration<ServiceTypeTask>
    {
        public void Configure(EntityTypeBuilder<ServiceTypeTask> builder)
        {
            builder.ToTable("ServiceTypeTasks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.Property(x => x.OrderIndex)
                .IsRequired()
                .HasDefaultValue(0);

            // Relationships
            builder.HasOne(x => x.ServiceType)
                .WithMany(st => st.Tasks)
                .HasForeignKey(x => x.ServiceTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
