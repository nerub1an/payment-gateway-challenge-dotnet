using PaymentGateway.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureApplication()
    .ConfigureServices(builder.Configuration);
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.MapHealthChecks("/health");

await app.RunAsync();

public partial class Program
{
}