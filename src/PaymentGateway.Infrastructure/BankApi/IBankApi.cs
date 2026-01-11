using Refit;

namespace PaymentGateway.Infrastructure.BankApi;

public interface IBankApi
{
    [Post("/payments")]
    Task<ApiResponse<BankResponse>> ProcessPayment(BankRequest bankRequest, CancellationToken cancellationToken);
}
