using System.Globalization;

namespace viewengine.Models;

public class GeneratorRecord
{
  public GeneratorRecord(string billLine)
  {
    var parts = billLine.Split('|');
    
    GeneratedBy = parts[1].Trim();
    
    if (int.TryParse(parts[3], out int issueResult))
    {
      IssueNumber = issueResult;
    }

    if (DateTime.TryParseExact(parts[2], 
          "yyyyMMdd HH:mm", CultureInfo.InvariantCulture, 
          DateTimeStyles.None, out DateTime dateResult ))
    {
      GeneratedAt = dateResult;
    }

  }

  public string GeneratedBy { get; }
  public int IssueNumber { get; } = 0;
  public DateTime GeneratedAt { get; } = DateTime.MinValue;

}