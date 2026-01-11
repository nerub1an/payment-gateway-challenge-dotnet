using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Core.Domain;

public readonly record struct BankResult
{
    public PaymentStatus PaymentStatus { get; init; }
    public string? BankCorrelationId { get; init; }
}
