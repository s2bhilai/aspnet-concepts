namespace viewengine.Models;

public class SubscriptionRecord
{
  public SubscriptionRecord(string billLine)
  {
    var parts = billLine.Split('|');
    SubscriberName = parts[1].Trim();
    SubscriberPhoneNumber = parts[3].Trim();
    InvoiceNumber = parts[4].Trim();
    AccountNumber = parts[12].Trim();
    
    if (decimal.TryParse(parts[6], out decimal totalResult))
    {
      TotalCharges = totalResult;
    }

    if (decimal.TryParse(parts[8], out decimal vatResult))
    {
      Vat = vatResult;
    }

    if (decimal.TryParse(parts[10], out decimal careResult))
    {
      CareCharges = careResult;
    }

  }
  
  public string SubscriberName { get; } = "";
  public string SubscriberPhoneNumber { get; } = "";
  public string InvoiceNumber { get; } = "";
  public decimal TotalCharges { get; }
  public decimal Vat { get; }
  public decimal CareCharges { get; }
  public string AccountNumber { get; } = "";

}