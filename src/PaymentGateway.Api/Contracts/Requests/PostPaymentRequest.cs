namespace PaymentGateway.Api.Contracts.Requests;

public record PostPaymentRequest
{
    public required string CardNumber { get; init; }
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
    public required int Cvv { get; init; }

    public required Guid MerchantId { get; init; }
    public required Guid IdempotancyId { get; init; }
}
