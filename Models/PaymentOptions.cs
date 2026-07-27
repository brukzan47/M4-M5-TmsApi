using System.ComponentModel.DataAnnotations;

namespace TmsApi.Models;

public sealed class PaymentOptions
{
    public const string SectionName = "Payments";

    [Required, Url]
    public required string GatewayUrl { get; init; }

    [Range(100, 100_000)]
    public decimal MaxDepositBirr { get; init; }
}
