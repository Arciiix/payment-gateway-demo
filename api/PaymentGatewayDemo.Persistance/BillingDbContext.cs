using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PaymentGatewayDemo.Domain.Models;

namespace PaymentGatewayDemo.Persistance;

public class BillingDbContext : IdentityDbContext<User>
{
    // TODO: To be changed
    private static readonly string DbPath = "PaymentGatewayDemo.db";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}