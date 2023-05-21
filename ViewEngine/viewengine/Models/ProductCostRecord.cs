using System.Globalization;

namespace viewengine.Models;

public class ProductCostRecord
{
  public ProductCostRecord(string billLine)
  {
    var parts = billLine.Split('|');
    ProductName = parts[1].Trim();
    InvoiceNumber = parts[4].Trim();
    AccountNumber = parts[8].Trim();
    ProductCode = parts[9].Trim();
    
    if (DateTime.TryParseExact(parts[6], 
          "yyyyMMdd HH:mm", CultureInfo.InvariantCulture, 
          DateTimeStyles.None, out DateTime startDateResult ))
    {
      DateStart = startDateResult;
    }

    if (DateTime.TryParseExact(parts[7], 
          "yyyyMMdd HH:mm", CultureInfo.InvariantCulture, 
          DateTimeStyles.None, out DateTime endDateResult ))
    {
      DateEnd = endDateResult;
    }
    
    if (decimal.TryParse(parts[2], out decimal costResult))
    {
      ProductCost = costResult;
    }
    
  }
  
  public string ProductName { get; } = "";
  public decimal ProductCost { get; }
  public string InvoiceNumber { get; } = "";
  public DateTime DateStart { get; }
  public DateTime DateEnd { get; }
  public string AccountNumber { get; } = "";
  public string ProductCode { get; } = "";

}