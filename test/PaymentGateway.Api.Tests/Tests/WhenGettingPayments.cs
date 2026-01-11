using System.Net;
using System.Net.Http.Json;
using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Core.Abstract;
using PaymentGateway.Core.Domain;
using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Api.Tests.Tests;

public class WhenGettingPayments : IClassFixture<PaymentsApiWebApplicationFactory>
{
    private const string BasePath = "/api/v1/payments";

    private readonly PaymentsApiWebApplicationFactory _fixture;
    private readonly Random _random = new();
    private readonly HttpClient _httpClient;

    public WhenGettingPayments(PaymentsApiWebApplicationFactory fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.CreateClient();
    }

    [Fact]
    public async Task AndPaymentExist_ThenReturns200OKWithPayment()
    {
        // Arrange
        var repository = _fixture.GetService<IPaymentsRepository>();

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2027, 2035),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = Helper.GenerateCardNumber(),
            Currency = "GBP",
            MerchantCorrelationId = Guid.NewGuid().ToString(),
            BankCorrelationId = null,
            Status = PaymentStatus.Initiated
        };

        await repository.UpsertPayment(payment, CancellationToken.None);

        // Act
        var response = await _httpClient.GetAsync($"{BasePath}/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task AndPaymentsNotFound_Returns404NotFound()
    {
        // Act
        var response = await _httpClient.GetAsync($"{BasePath}/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
