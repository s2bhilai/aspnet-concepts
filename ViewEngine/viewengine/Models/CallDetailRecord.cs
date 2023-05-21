using System.Globalization;

namespace viewengine.Models;

public class CallDetailRecord
{
  public CallDetailRecord(string billLine)
  {
    var parts = billLine.Split('|');
    DestinationNumber = parts[4].Trim();
    TransactionType = parts[5].Trim();
    BillableDuration = parts[7].Trim();
    BillableCost = parts[8].Trim();
    InvoiceNumber = parts[9].Trim();
    Bundle = parts[18].Trim();
    
    if (DateTime.TryParseExact($"{parts[2]} {parts[3]}", 
          "yyyyMMdd HH:mm", CultureInfo.InvariantCulture, 
          DateTimeStyles.None, out DateTime dateResult ))
    {
      DateAndTime = dateResult;
    }


  }
  
  public DateTime DateAndTime { get; }
  public string DestinationNumber { get; } = "";
  public string TransactionType { get; } = "";
  public string BillableDuration { get; } = "";
  public string BillableCost { get; } = "";
  public string InvoiceNumber { get; } = "";
  public string Bundle { get; } = "";
}