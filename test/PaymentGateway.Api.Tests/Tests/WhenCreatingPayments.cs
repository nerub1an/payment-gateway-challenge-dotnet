using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using PaymentGateway.Api.Contracts.Requests;
using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Core.Abstract;
using PaymentGateway.Core.Domain;
using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.Infrastructure.DataAccess;

namespace PaymentGateway.Api.Tests.Tests;

public class WhenCreatingPayments : IClassFixture<PaymentsApiWebApplicationFactory>, IAsyncLifetime
{
    private const string BasePath = "/api/v1/payments";

    private readonly PaymentsApiWebApplicationFactory _fixture;
    private readonly Random _random = new();
    private readonly HttpClient _httpClient;

    public WhenCreatingPayments(PaymentsApiWebApplicationFactory fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.CreateClient();
    }

    [Fact]
    public async Task AndPaymentIsValid_ThenReturns201CreatedAndAuthroizedStatus()
    {
        // Arrange
        PostPaymentRequest request = BuildAuthorizedPaymentRequest();

        // Act
        var response = await ProcessPayment(request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        AssertPaymentResponse(request, paymentResponse, PaymentStatus.Authorized);
        await AssertPaymentInsertion(paymentResponse!, PaymentStatus.Authorized);
    }

    [Fact]
    public async Task AndPaymentIsDeclinedByBank_ThenReturns400BadRequestAndDeclinedStatus()
    {
        // Arrange
        PostPaymentRequest request = BuildAuthorizedPaymentRequest(PaymentStatus.Declined);

        // Act
        var response = await ProcessPayment(request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AssertPaymentResponse(request, paymentResponse, PaymentStatus.Declined);
        await AssertPaymentInsertion(paymentResponse!, PaymentStatus.Declined);
    }

    [Fact]
    public async Task AndBankIsUnavailable_ThenReturns424FailedDependencyAndFailedStatus()
    {
        // Arrange
        PostPaymentRequest request = BuildAuthorizedPaymentRequest(PaymentStatus.Failed);

        // Act
        var response = await ProcessPayment(request);

        // Assert
        Assert.Equal(HttpStatusCode.FailedDependency, response.StatusCode);

        var repository = _fixture.GetService<IPaymentsRepository>();
        var explicitRepository = (PaymentsRepository)repository;
        Assert.NotEmpty(explicitRepository.Payments);
        Assert.Equal(PaymentStatus.Failed, explicitRepository.Payments.First().Value.Status);
    }

    [Fact]
    public async Task AndPaymentIsInvalid_ThenReturns400BadRequestAndRejectedStatus()
    {
        // Arrange
        PostPaymentRequest request = BuildAuthorizedPaymentRequest() with { Currency = "" };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await ProcessPayment(request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        AssertPaymentResponse(request, paymentResponse, PaymentStatus.Rejected);
    }

    [Fact]
    public async Task AndPaymentFailedIdempotancy_Returns400BadRequestAndRejectedStatus()
    {
        // Arrange
        PostPaymentRequest request1 = BuildAuthorizedPaymentRequest();
        PostPaymentRequest request2 = BuildAuthorizedPaymentRequest() with
        {
            MerchantId = request1.MerchantId,
            IdempotancyId = request1.IdempotancyId
        };

        // Act
        await ProcessPayment(request1);
        await Task.Delay(100);

        var response = await ProcessPayment(request2);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        AssertPaymentResponse(request2, paymentResponse, PaymentStatus.Rejected);
    }

    private async Task<HttpResponseMessage> ProcessPayment(PostPaymentRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync(BasePath, content);
    }

    private static void AssertPaymentResponse(
        PostPaymentRequest request,
        PaymentResponse? paymentResponse,
        PaymentStatus expectedStatus)
    {
        Assert.NotNull(paymentResponse);
        Assert.Equal(expectedStatus, paymentResponse!.Status);
        Assert.Equal(request.CardNumber.GetLastFour(), paymentResponse.CardNumberLastFour);
        Assert.Equal(request.ExpiryMonth, paymentResponse.ExpiryMonth);
        Assert.Equal(request.ExpiryYear, paymentResponse.ExpiryYear);
        Assert.Equal(request.Currency, paymentResponse.Currency);
        Assert.Equal(request.Amount, paymentResponse.Amount);
    }

    private async Task AssertPaymentInsertion(PaymentResponse paymentResponse, PaymentStatus expectedStatus)
    {
        var repository = _fixture.GetService<IPaymentsRepository>();
        var paymentEntity = await repository.GetPayment(paymentResponse.Id, CancellationToken.None);
        Assert.NotNull(paymentEntity);
        Assert.Equal(expectedStatus, paymentEntity!.Status);
    }

    private PostPaymentRequest BuildAuthorizedPaymentRequest(PaymentStatus paymentStatus = PaymentStatus.Authorized)
    {
        return new PostPaymentRequest
        {
            ExpiryYear = _random.Next(2027, 2035),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = Helper.GenerateCardNumber()[..^1] + paymentStatus switch
            {
                PaymentStatus.Authorized => "1",
                PaymentStatus.Declined => "2",
                PaymentStatus.Failed => "0",
                _ => "1"
            },
            Currency = "GBP",
            Cvv = 123,
            MerchantId = Guid.NewGuid(),
            IdempotancyId = Guid.NewGuid()
        };
    }

    public Task InitializeAsync()
    {
        var repository = _fixture.GetService<IPaymentsRepository>();

        var explicitRepository = (PaymentsRepository)repository;
        explicitRepository.Payments.Clear();

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

}