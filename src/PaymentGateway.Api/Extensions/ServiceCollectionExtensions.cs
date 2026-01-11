using FluentValidation;
using PaymentGateway.Core.Abstract;
using PaymentGateway.Core.Services;
using PaymentGateway.Infrastructure;
using PaymentGateway.Infrastructure.DataAccess;

namespace PaymentGateway.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddSwaggerGen()
            .AddEndpointsApiExplorer()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddValidatorsFromAssembly(typeof(Program).Assembly)
            .AddHealthChecks();

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<IdempotancyService>()
            .AddScoped<IPaymentsService, PaymentsService>()
            .AddSingleton<IPaymentsRepository, PaymentsRepository>()
            .AddSingleton(_ => TimeProvider.System);
        
        services.AddBank(configuration);

        return services;
    }
}
