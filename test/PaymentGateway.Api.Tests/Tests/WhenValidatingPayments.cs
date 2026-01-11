using FluentValidation.TestHelper;

using PaymentGateway.Api.Contracts.Requests;

namespace PaymentGateway.Api.Tests.Tests;

public class WhenValidatingPayments
{
    private readonly PostPaymentRequestValidator _validator = new();

    public static IEnumerable<object[]> PostPaymentRequests =>
    [
        [ "Payment is Valid", ValidRequest() ],
        [ "CardNumber is Empty", ValidRequest() with { CardNumber = "" }, nameof(PostPaymentRequest.CardNumber) ],
        [ "CardNumber is Invalid", ValidRequest() with { CardNumber = "1234abcd5678" }, nameof(PostPaymentRequest.CardNumber) ],
        [ "ExpiryMonth is Invalid", ValidRequest() with { ExpiryMonth = 0 }, nameof(PostPaymentRequest.ExpiryMonth) ],
        [ "ExpiryYear is Invalid", ValidRequest() with { ExpiryYear = DateTime.UtcNow.Year - 1 }, nameof(PostPaymentRequest.ExpiryYear) ],
        [
            "Card is Expired",
            ValidRequest() with
            {
                ExpiryYear = DateTime.UtcNow.Year,
                ExpiryMonth = DateTime.UtcNow.Month
            },
            string.Empty // cross-property rule
        ],
        [ "Currency is Invalid", ValidRequest() with { Currency = "JPY" }, nameof(PostPaymentRequest.Currency) ],
        [ "Amount is Invalid", ValidRequest() with { Amount = 0 }, nameof(PostPaymentRequest.Amount) ],
        [ "CVV is Invalid", ValidRequest() with { Cvv = 12 }, nameof(PostPaymentRequest.Cvv) ],
        [ "Idempotency is Invalid", ValidRequest() with { IdempotancyId = Guid.Empty }, nameof(PostPaymentRequest.IdempotancyId) ]
    ];

#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
    [Theory]
    [MemberData(nameof(PostPaymentRequests))]
    public void ThenProperErrorsAreValidated(string testName, PostPaymentRequest request, string? expectedProperty = null)
    {
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        if (expectedProperty is null)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(expectedProperty);
        }
    }
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters

    private static PostPaymentRequest ValidRequest()
    {
        var now = DateTime.UtcNow.AddMonths(1);

        return new PostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = now.Month,
            ExpiryYear = now.Year,
            Currency = "USD",
            Amount = 1000,
            Cvv = 123,
            MerchantId = Guid.NewGuid(),
            IdempotancyId = Guid.NewGuid()
        };
    }
}
