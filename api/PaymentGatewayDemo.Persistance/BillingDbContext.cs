using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PaymentGatewayDemo.Domain.Models;

namespace PaymentGatewayDemo.Persistance;

public class BillingDbContext : IdentityDbContext<User>
{
    // TODO: To be changed
    private static readonly string DbPath = "PaymentGatewayDemo.db";

    public DbSet<Product> Products { get; set; }
    public DbSet<Billing> Billings { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .HasOne(p => p.User)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.UserId);

        builder.Entity<Product>()
            .HasMany(p => p.Billings)
            .WithOne(b => b.Product)
            .HasForeignKey(b => b.ProductKeyId)
            .IsRequired(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}