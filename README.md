# Design
The Payment Gateway is a lightweight service responsible for processing payments from merchants to upstream banking providers.

* Clean Architecture is used to clearly separate domain, application, and infrastructure concerns, enabling easier testing, replacement of dependencies, and long-term maintainability.
* Idempotency is implemented using a composite key of MerchantId and MerchantPaymentId, ensuring that duplicate payment requests from merchants are safely rejected.
* Bank connectivity is protected with a resilient retry mechanism using JitterBackoffV2, reducing the risk of thundering-herd effects and improving stability when upstream services are unstable.
* Failed or rejected payments are not persisted. Instead, the system assumes that failure statistics and diagnostics are derived from observability tooling (logs, metrics, traces), keeping the primary data model minimal.
* Idempotency storage is treated as a critical dependency, as it is the primary guard against duplicate payments.
  * One potential improvement would be to persist payments in an Initiated state in primary storage alongside idempotency records. This could reduce the cost and durability requirements of the idempotency store (e.g. allowing Redis without AOF), at the expense of slightly increased latency — an acceptable trade-off in a payments context.

Testing strategy

* Integration tests cover end-to-end behaviour and critical flows.
* Unit tests are intentionally limited to validation logic, where fast, isolated feedback provides the most value.




# Instructions for candidates

This is the .NET version of the Payment Gateway challenge. If you haven't already read this [README.md](https://github.com/cko-recruitment/) on the details of this exercise, please do so now. 

## Template structure
```
src/
    PaymentGateway.Api - a skeleton ASP.NET Core Web API
test/
    PaymentGateway.Api.Tests - an empty xUnit test project
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

Feel free to change the structure of the solution, use a different test library etc.