using OpenAI_DotNet_Integration.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace OpenAI_DotNet_Integration.Data;

public class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(customer => customer.Id);

            entity.Property(customer => customer.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(customer => customer.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(customer => customer.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasIndex(customer => customer.Email)
                .IsUnique();

            entity.Property(customer => customer.PhoneNumber)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(customer => customer.CreatedAt)
                .IsRequired();

            entity.Property(customer => customer.UpdatedAt)
                .IsRequired();
        });
    }
}
