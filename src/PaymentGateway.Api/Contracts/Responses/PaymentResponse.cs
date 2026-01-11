using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Api.Contracts.Responses;

public record PaymentResponse
{
    public required Guid Id { get; init; }
    public required PaymentStatus Status { get; init; }
    public required int CardNumberLastFour { get; init; }
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
}