using Microsoft.Extensions.Logging;
using PaymentGateway.Core.Abstract;
using PaymentGateway.Core.Domain;
using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.Core.Dtos;
using PaymentGateway.Infrastructure.BankApi;

namespace PaymentGateway.Infrastructure;

public class BankService(IBankApi bankApi, ILogger<BankService> logger) : IBankService
{
    public async Task<BankResult> ProcessPayment(Payment payment, CancellationToken cancellationToken)
    {
        var bankRequest = payment.ToBankRequest();
        var bankResponse = await bankApi.ProcessPayment(bankRequest, cancellationToken);

        return bankResponse.IsSuccessful is false
            ? FailPayment(payment, bankResponse)
            : ProcessPayment(payment, bankResponse.Content);
    }
    
    private BankResult FailPayment(Payment payment, Refit.ApiResponse<BankResponse> bankResponse)
    {
        logger.LogError(
            bankResponse.Error,
            "Request to the bank failed with response code {ResponseCode} for payment {PaymentId} with message {ErrorMessage}",
            bankResponse.StatusCode,
            payment.Id,
            bankResponse.Error!.Content);

        return new BankResult
        {
            PaymentStatus = PaymentStatus.Failed,
            BankCorrelationId = null
        };
    }

    private BankResult ProcessPayment(Payment payment, BankResponse bankResponse)
    {
        payment.BankCorrelationId = bankResponse.AuthorizationCode;
        payment.Status = bankResponse.Authorized
            ? PaymentStatus.Authorized
            : PaymentStatus.Declined;

        logger.LogInformation(
            "Payment {PaymentId} processed to the bank with status {PaymentStatus} and code {AuthCode}",
            payment.Id,
            payment.Status,
            payment.BankCorrelationId);

        return new BankResult
        {
            PaymentStatus = bankResponse.Authorized
                ? PaymentStatus.Authorized
                : PaymentStatus.Declined,
            BankCorrelationId = bankResponse.AuthorizationCode
        };
    }
}