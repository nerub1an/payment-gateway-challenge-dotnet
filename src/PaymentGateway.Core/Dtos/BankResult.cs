using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Core.Dtos;

public readonly record struct BankResult
{
    public PaymentStatus PaymentStatus { get; init; }
    public string? BankCorrelationId { get; init; }
}
