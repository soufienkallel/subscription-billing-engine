namespace Application.Configuration;

public class BillingOptions
{
    public const string SectionName = "Billing";

    public string DefaultCurrency { get; set; } = "TND";
}
