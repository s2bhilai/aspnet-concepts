namespace viewengine.Models;

public class CustomerRecord
{
  public CustomerRecord(string billLine)
  {
    var parts = billLine.Split('|');
    Surname = parts[2].Trim();
    Forename = parts[3].Trim();
    Title = parts[5].Trim();
    HouseAddress = parts[10].Trim();
    Street = parts[13].Trim();
    Town = parts[16].Trim();
    County = parts[17].Trim();
    PostCode = parts[18].Trim();
    AccountNumber = parts[21].Trim();
  }

  public string Surname { get; } = "";
  public string Forename { get; } = "";
  public string Title { get; } = "";
  public string HouseAddress { get; } = "";
  public string Street { get; } = "";
  public string Town { get; } = "";
  public string County { get; } = "";
  public string PostCode { get; } = "";
  public string AccountNumber { get; } = "";
}