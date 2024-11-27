using Microsoft.EntityFrameworkCore;
using PaymentGatewayDemo.Api;
using PaymentGatewayDemo.Api.ExceptionHandlers;
using PaymentGatewayDemo.Domain.Entities.Configuration;
using PaymentGatewayDemo.Persistance;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddConfiguration();
services.RegisterValidators();

var config = builder.Configuration.GetSection(GlobalConfiguration.SectionName).Get<GlobalConfiguration>()!;
services.AddAuthenticationLayer(config);

services.ConfigureControllers();
services.ConfigureSwaggerExplorer();
services.AddDataLayer();
services.AddServices();
services.AddHttpClients();

services.AddHttpLogging();
services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Use((context, next) =>
{
    context.Request
        .EnableBuffering();
    return next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFrontend();
app.MapControllers();
app.UseHttpLogging();

// For development purposes - ensure that the database is created
using var scope = app.Services.CreateScope();
await using var context = scope.ServiceProvider.GetService<BillingDbContext>();

ArgumentNullException.ThrowIfNull(context);
await context.Database.MigrateAsync();

app.Run();