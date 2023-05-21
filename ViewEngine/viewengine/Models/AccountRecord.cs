namespace viewengine.Models;

public class AccountRecord
{
  public AccountRecord(string billLine)
  {
    var parts = billLine.Split('|');
    AccountNumber = parts[1].Trim();
    
    if (decimal.TryParse(parts[3], out decimal chargesResult))
    {
      ChargesThisPeriod = chargesResult;
    }

    if (decimal.TryParse(parts[4], out decimal discountResult))
    {
      Discounts = discountResult;
    }

    if (decimal.TryParse(parts[5], out decimal balanceResult))
    {
      OutstandingBalance = balanceResult;
    }
  }

  public string AccountNumber { get; } = "";
  public decimal ChargesThisPeriod { get; }
  public decimal Discounts { get; }
  public decimal OutstandingBalance { get; }

}