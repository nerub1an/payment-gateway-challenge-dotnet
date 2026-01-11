using PaymentGateway.Core.Domain;
using PaymentGateway.Core.Dtos;

namespace PaymentGateway.Core.Abstract;

public interface IBankService
{
    Task<BankResult> ProcessPayment(Payment payment, CancellationToken cancellationToken);
}
