namespace PaymentGateway.Infrastructure.BankApi;

public record BankConfiguration
{
    public required string BaseUrl { get; init; }
}
