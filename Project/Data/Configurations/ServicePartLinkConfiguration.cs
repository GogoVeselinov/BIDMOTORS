using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class ServicePartLinkConfiguration : IEntityTypeConfiguration<ServicePartLink>
    {
        public void Configure(EntityTypeBuilder<ServicePartLink> builder)
        {
            builder.HasKey(spl => spl.Id);

            // Връзка към Service (много към едно)
            builder.HasOne(spl => spl.Service)
                .WithMany(s => s.PartLinks)
                .HasForeignKey(spl => spl.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Properties
            builder.Property(spl => spl.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(spl => spl.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(spl => spl.Supplier)
                .HasMaxLength(100);

            builder.Property(spl => spl.Notes)
                .HasMaxLength(1000);
        }
    }
}
