namespace viewengine.Models;

public class BundleRecord
{
  public BundleRecord(string billLine)
  {
    var parts = billLine.Split('|');
    BundleName = parts[1].Trim();
    ProductCode = parts[2].Trim();
    InvoiceNumber = parts[4].Trim();
    
    if (int.TryParse(parts[3], out int priorityResult))
    {
      Priority = priorityResult;
    }

  }
  
  public string BundleName { get; } = "";
  public string ProductCode { get; } = "";
  public int Priority { get; }
  public string InvoiceNumber { get; } = "";
}