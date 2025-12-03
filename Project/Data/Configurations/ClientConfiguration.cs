using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Models.Entities;

namespace Project.Data.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(c => c.Email)
                .IsUnique();

            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            builder.Property(c => c.PasswordHash)
                .IsRequired();

            builder.HasMany(c => c.Cars)
                .WithOne(car => car.Client)
                .HasForeignKey(car => car.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.ServiceRequests)
                .WithOne(sr => sr.Client)
                .HasForeignKey(sr => sr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Notifications)
                .WithOne(n => n.Client)
                .HasForeignKey(n => n.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
