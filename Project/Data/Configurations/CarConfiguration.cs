using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Make)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Year)
                .IsRequired();

            builder.Property(c => c.VIN)
                .HasMaxLength(17);

            builder.Property(c => c.LicensePlate)
                .HasMaxLength(20);

            builder.HasOne(c => c.Client)
                .WithMany(client => client.Cars)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.ServiceRequests)
                .WithOne(sr => sr.Car)
                .HasForeignKey(sr => sr.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Repairs)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
