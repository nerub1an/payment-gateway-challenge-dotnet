using PaymentGateway.Core.Domain;
using PaymentGateway.Infrastructure.DataAccess.Entities;

namespace PaymentGateway.Infrastructure.DataAccess;

public static class PaymentEntitiesMapping
{
    public static Payment ToDomain(this PaymentEntity entity)
    {
        return new Payment
        {
            Id = entity.Id,
            Status = entity.Status,
            CardNumber = entity.CardNumberLastFour.ToString(),
            ExpiryMonth = entity.ExpiryMonth,
            ExpiryYear = entity.ExpiryYear,
            Currency = entity.Currency,
            Amount = entity.Amount,
            MerchantCorrelationId = entity.MerchantCorrelationId,
            BankCorrelationId = entity.BankCorrelationId
        };
    }

    public static PaymentEntity ToEntity(this Payment payment)
    {
        return new PaymentEntity
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumber.GetLastFour(),
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount,
            MerchantCorrelationId = payment.MerchantCorrelationId,
            BankCorrelationId = payment.BankCorrelationId
        };
    }
}