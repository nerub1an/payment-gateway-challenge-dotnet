using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Contracts;
using PaymentGateway.Api.Contracts.Requests;
using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Core.Abstract;
using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsService paymentsService, IValidator<PostPaymentRequest> paymentsValidator)
    : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentResponse?>> GetPayment(Guid id, CancellationToken cancellationToken)
    {
        var payment = await paymentsService.GetPayment(id, cancellationToken);

        return payment is null
            ? new NotFoundResult()
            : new OkObjectResult(payment.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(
        PostPaymentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await paymentsValidator.ValidateAsync(request, cancellationToken);

        var payment = request.ToDomain();

        if (validationResult.IsValid)
        {
            payment = await paymentsService.ProcessPayment(payment, cancellationToken);
        }
        else
        {
            payment.Status = PaymentStatus.Rejected;
        }

        var response = payment.ToDto();

        return payment.Status switch
        {
            PaymentStatus.Authorized => new CreatedResult(nameof(ProcessPayment), response),
            PaymentStatus.Rejected => BadRequest(response),
            PaymentStatus.Declined => Ok(response),
            PaymentStatus.Failed => StatusCode(StatusCodes.Status424FailedDependency),
            _ => StatusCode(StatusCodes.Status500InternalServerError) // TODO: not sure
        };
    }
}
