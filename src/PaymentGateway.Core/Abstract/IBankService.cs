using PaymentGateway.Core.Domain;

namespace PaymentGateway.Core.Abstract;

public interface IBankService
{
    Task<BankResult> ProcessPayment(Payment payment, CancellationToken cancellationToken);
}
