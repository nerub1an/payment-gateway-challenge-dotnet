using System.Text.Json.Serialization;

namespace PaymentGateway.Infrastructure.BankApi;

public readonly record struct BankResponse
{
    public bool Authorized { get; init; }

    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; init; }
}
