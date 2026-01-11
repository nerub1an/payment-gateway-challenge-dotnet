using PaymentGateway.Core.Domain;

namespace PaymentGateway.Core.Abstract;

public interface IPaymentsService
{
    Task<Payment?> GetPayment(Guid paymentId, CancellationToken cancellationToken);
    Task<Payment> ProcessPayment(Payment payment, CancellationToken cancellationToken);
}
