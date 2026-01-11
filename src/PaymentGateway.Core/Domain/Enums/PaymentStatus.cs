using System.Text.Json.Serialization;

namespace PaymentGateway.Core.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    Initiated,
    Authorized,
    Declined,
    Rejected,
    Failed
}