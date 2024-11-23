using Microsoft.EntityFrameworkCore;
using PaymentGatewayDemo.Domain.Models;
using PaymentGatewayDemo.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<BillingDbContext>();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BillingDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapIdentityApi<User>();


// For development purposes - ensure that the database is created

using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetService<BillingDbContext>();

ArgumentNullException.ThrowIfNull(context);
await context.Database.MigrateAsync();

app.Run();