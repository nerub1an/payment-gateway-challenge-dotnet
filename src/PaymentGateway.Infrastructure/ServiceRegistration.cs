using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Core.Abstract;
using PaymentGateway.Infrastructure.BankApi;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Retry;
using Refit;

namespace PaymentGateway.Infrastructure;

public static class ServiceRegistration
{
    private static readonly RefitSettings Settings = new(
        new SystemTextJsonContentSerializer(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            }));

    public static IServiceCollection AddBank(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BankConfiguration>(configuration.GetRequiredSection(nameof(BankConfiguration)));
        var bankConfiguration = configuration.GetRequiredSection(nameof(BankConfiguration)).Get<BankConfiguration>()!;

        services.AddScoped<IBankService, BankService>();

        services.AddRefitClient<IBankApi>(Settings)
            .ConfigureHttpClient(x => x.BaseAddress = new Uri(bankConfiguration.BaseUrl))
            .AddPolicyHandler(RetryHttpWithJitterBackoffAsync(3, TimeSpan.FromMilliseconds(350)));

        return services;
        
    }

    private static AsyncRetryPolicy<HttpResponseMessage> RetryHttpWithJitterBackoffAsync(
        int retryCount, TimeSpan backOffInterval)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => (int)msg.StatusCode >= 499)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(backOffInterval, retryCount: retryCount, fastFirst: true));
    }
}
