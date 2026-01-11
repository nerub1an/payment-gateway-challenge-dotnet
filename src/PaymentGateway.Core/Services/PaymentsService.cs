using PaymentGateway.Core.Abstract;
using PaymentGateway.Core.Domain;
using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Core.Services;

public class PaymentsService(
    IPaymentsRepository paymentsRepository,
    IdempotancyService idempotancyService,
    IBankService bankService)
    : IPaymentsService
{
    public async Task<Payment?> GetPayment(Guid paymentId, CancellationToken cancellationToken)
    {
        return await paymentsRepository.GetPayment(paymentId, cancellationToken);
    }

    public async Task<Payment> ProcessPayment(Payment payment, CancellationToken cancellationToken)
    {
        if (idempotancyService.IfIdempotancyCheckFailed(payment.MerchantCorrelationId))
        {
            payment.Status = PaymentStatus.Rejected;
            return payment;
        }

        // await paymentsRepository.UpsertPayment(payment, cancellationToken);

        var bankResult = await bankService.ProcessPayment(payment, cancellationToken);

        payment.Status = bankResult.PaymentStatus;
        payment.BankCorrelationId = bankResult.BankCorrelationId;

        if (bankResult.PaymentStatus != PaymentStatus.Failed)
        {
            await paymentsRepository.UpsertPayment(payment, cancellationToken);
        }

        return payment;
    }
}