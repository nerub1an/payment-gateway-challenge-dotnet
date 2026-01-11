using System.Text.Json.Serialization;

namespace PaymentGateway.Infrastructure.BankApi;

public record BankRequest
{
    [JsonPropertyName("card_number")]
    public required string CardNumber { get; init; }

    [JsonPropertyName("expiry_date")]
    public required string ExpiryDate { get; init; }

    public required string Currency { get; init; }

    public required int Amount { get; init; }

    public required string Cvv { get; init; }
}