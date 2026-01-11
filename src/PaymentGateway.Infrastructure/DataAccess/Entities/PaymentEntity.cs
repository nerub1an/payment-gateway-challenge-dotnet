using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Infrastructure.DataAccess.Entities;

public class PaymentEntity
{
    public required Guid Id { get; init; }
    public required PaymentStatus Status { get; init; }
    public required int CardNumberLastFour { get; init; }
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }

    public required string? BankCorrelationId { get; init; }

    public required string MerchantCorrelationId { get; init; }
    public DateTimeOffset CreatedAt { get; set; }
}