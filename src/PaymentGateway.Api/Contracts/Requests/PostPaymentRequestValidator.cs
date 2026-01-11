using FluentValidation;

namespace PaymentGateway.Api.Contracts.Requests;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    private static readonly HashSet<string> AllowedCurrencies =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "USD",
            "EUR",
            "GBP"
        };

    public PostPaymentRequestValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Length(14, 19)
            .Matches("^[0-9]+$")
            .WithMessage("Card number must contain only digits");

        RuleFor(x => x.ExpiryMonth)
            .InclusiveBetween(1, 12);

        RuleFor(x => x.ExpiryYear)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x)
            .Must(BeInTheFuture)
            .WithMessage("Card expiry date must be in the future")
            .When(x => x.ExpiryMonth > 0 && x.ExpiryYear >= DateTime.UtcNow.Year);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Must(c => AllowedCurrencies.Contains(c))
            .WithMessage($"Currency must be one of: {string.Join(", ", AllowedCurrencies)}");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");

        RuleFor(x => x.Cvv)
            .InclusiveBetween(0, 9999)
            .Must(cvv => cvv.ToString().Length is 3 or 4)
            .WithMessage("CVV must be 3 or 4 digits");

        RuleFor(x => x.MerchantId)
            .NotEmpty();

        RuleFor(x => x.IdempotancyId)
            .NotEmpty();
    }

    private static bool BeInTheFuture(PostPaymentRequest request)
    {
        // TODO: TimeProvider
        var now = DateTime.UtcNow;

        return request.ExpiryYear > now.Year || request.ExpiryYear == now.Year && request.ExpiryMonth > now.Month;
    }
}