using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Core.Domain;

public class Payment
{
    public Guid Id { get; init; }
    public required PaymentStatus Status { get; set; }
    public required string CardNumber { get; init; }
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
    public int Cvv { get; init; }

    public required string MerchantCorrelationId { get; init; }
    public required string? BankCorrelationId { get; set; }
}
