using System.Globalization;
using System.Net;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PaymentGatewayDemo.Api.Filters.Exception;
using PaymentGatewayDemo.Api.Helpers;
using PaymentGatewayDemo.Application.DTOs.Auth.Requests;
using PaymentGatewayDemo.Application.Services.Auth;
using PaymentGatewayDemo.Domain.Entities.Configuration;
using PaymentGatewayDemo.Domain.Extensions;
using PaymentGatewayDemo.Domain.Models;
using PaymentGatewayDemo.Infrastructure.Services.Auth;
using PaymentGatewayDemo.Persistance;

namespace PaymentGatewayDemo.Api;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddSingleton<IValidator<GlobalConfiguration>, GlobalConfigurationValidator>();
        services.AddValidatorsFromAssemblyContaining<GlobalConfigurationValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

        return services;
    }

    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(opts => opts.RegisterFilters())
            .AddMyJsonLibrary()
            .ConfigureApiBehaviorOptions(opts =>
            {
                opts.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.Values.Select(x => x.Errors.Select(e => e.ErrorMessage))
                        .Distinct()
                        .SelectMany(e => e);

                    return new HttpErrorResponse(StatusCodes.Status400BadRequest, "Validation error",
                        string.Join(", ", errors)).ToActionResult();
                };
            });

        services.Configure<RouteOptions>(opts =>
        {
            opts.LowercaseUrls = true;
            opts.LowercaseQueryStrings = true;
        });
        return services;
    }

    public static IMvcBuilder AddMyJsonLibrary(this IMvcBuilder builder)
    {
        builder.AddNewtonsoftJson(opts =>
        {
            opts.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true
                }
            };
            opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            opts.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            opts.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            var converter = new IsoDateTimeConverter
            {
                DateTimeStyles = DateTimeStyles.AdjustToUniversal,
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK"
            };

            opts.SerializerSettings.Converters.Add(converter);
        });

        return builder;
    }

    public static void RegisterFilters(this MvcOptions opts)
    {
        // For Swashbuckle
        opts.Filters.Add(new ConsumesAttribute("application/json"));
        opts.Filters.Add(new ProducesAttribute("application/json"));
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        services.AddOptions<GlobalConfiguration>()
            .BindConfiguration(GlobalConfiguration.SectionName)
            .ValidateFluently()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddAuthenticationLayer(this IServiceCollection services,
        GlobalConfiguration configuration)
    {
        services.AddAuthorizationBuilder();

        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BillingDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = configuration.JwtConfiguration.Audience,
                    ValidIssuer = configuration.JwtConfiguration.Issuer,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.JwtConfiguration.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        var httpContext = context.HttpContext;
                        var statusCode = HttpStatusCode.Unauthorized;

                        var routeData = httpContext.GetRouteData();
                        var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

                        var problemDetails = new ProblemDetails
                        {
                            Status = (int)statusCode,
                            Title = "Unauthorized"
                        };

                        var result = new ObjectResult(problemDetails) { StatusCode = (int)statusCode };
                        await result.ExecuteResultAsync(actionContext);
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = AuthHelpers.GetAccessToken(context.Request);

                        context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            options.Lockout.AllowedForNewUsers = false;
        });


        return services;
    }

    public static IServiceCollection ConfigureSwaggerExplorer(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerGeneralExceptionOperationFilter>();

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Moni.Api", Version = "v1" });
            c.EnableAnnotations();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddDataLayer(this IServiceCollection services)
    {
        services.AddDbContext<BillingDbContext>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}