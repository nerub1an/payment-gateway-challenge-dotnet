namespace PaymentGateway.Core.Domain;

public static class CardNumberHelper
{
    public static int GetLastFour(this string cardNumber)
    {
        return int.TryParse(new string([.. cardNumber.TakeLast(4)]), out int result) ? result : 0;
    }
}
