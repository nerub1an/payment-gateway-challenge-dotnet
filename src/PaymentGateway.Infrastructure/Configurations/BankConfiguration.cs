namespace PaymentGateway.Infrastructure.Configurations;

public record BankConfiguration
{
    public required string BaseUrl { get; init; }
}
