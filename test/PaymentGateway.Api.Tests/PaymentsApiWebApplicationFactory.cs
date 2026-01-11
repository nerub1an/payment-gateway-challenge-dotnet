
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentGateway.Api.Tests;

public class PaymentsApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public T GetService<T>() where T : class
    {
        return Services.GetRequiredService<T>();
    }
}
