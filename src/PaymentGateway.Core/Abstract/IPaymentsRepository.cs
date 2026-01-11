using PaymentGateway.Core.Domain;

namespace PaymentGateway.Core.Abstract;

public interface IPaymentsRepository
{
    Task UpsertPayment(Payment payment, CancellationToken cancellationToken);

    Task<Payment?> GetPayment(Guid paymentId, CancellationToken cancellationToken);
}
