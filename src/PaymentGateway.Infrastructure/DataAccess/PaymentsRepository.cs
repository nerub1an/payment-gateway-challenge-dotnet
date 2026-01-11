using System.Collections.Concurrent;
using PaymentGateway.Core.Abstract;
using PaymentGateway.Core.Domain;
using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.Infrastructure.DataAccess.Entities;

namespace PaymentGateway.Infrastructure.DataAccess;

public class PaymentsRepository(TimeProvider timeProvider) : IPaymentsRepository
{
    internal readonly ConcurrentDictionary<Guid, PaymentEntity> Payments = [];
    
    public Task UpsertPayment(Payment payment, CancellationToken cancellationToken)
    {
        var entity = payment.ToEntity();

        var now = timeProvider.GetUtcNow();
        if (payment.Status == PaymentStatus.Initiated)
        {
            entity.CreatedAt = now;
        }
        entity.UpdatedAt = now;

        Payments[payment.Id] = entity;

        return Task.CompletedTask;
    }

    public Task<Payment?> GetPayment(Guid paymentId, CancellationToken cancellationToken)
    {
        Payments.TryGetValue(paymentId, out var entity);

        return Task.FromResult(entity?.ToDomain());
    }
}
