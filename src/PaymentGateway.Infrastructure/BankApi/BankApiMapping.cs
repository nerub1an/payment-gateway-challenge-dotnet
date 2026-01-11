using PaymentGateway.Core.Domain;

namespace PaymentGateway.Infrastructure.BankApi;

public static class BankApiMapping
{
    public static BankRequest ToBankRequest(this Payment payment)
    {
        return new BankRequest
        {
            CardNumber = payment.CardNumber,
            ExpiryDate = $"{payment.ExpiryMonth}/{payment.ExpiryYear}",
            Currency = payment.Currency,
            Amount = payment.Amount,
            Cvv = payment.Cvv.ToString()
        };
    }
}
