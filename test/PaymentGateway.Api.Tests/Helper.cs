namespace PaymentGateway.Api.Tests;

public class Helper
{
    public static string GenerateCardNumber(int length = 16)
    {
        return string.Concat(
            Enumerable.Range(0, length).Select(_ => Random.Shared.Next(0, 10)));
    }
}
