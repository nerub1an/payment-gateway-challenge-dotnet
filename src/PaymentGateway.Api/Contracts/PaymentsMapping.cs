using PaymentGateway.Api.Contracts.Requests;
using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Core.Domain;
using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Api.Contracts;

public static class PaymentsMapping
{
    public static Payment ToDomain(this PostPaymentRequest request)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Initiated,
            CardNumber = request.CardNumber,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            Amount = request.Amount,
            Cvv = request.Cvv,
            MerchantCorrelationId = $"{request.MerchantId}#{request.IdempotancyId}",
            BankCorrelationId = null
        };
    }

    public static PaymentResponse ToDto(this Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumber.GetLastFour(),
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }
}
