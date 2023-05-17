using ModelBinder.Attributes;

namespace ModelBinder.Models;

public class YearObject
{
    [ValidYear(1980)]
    public int YearAccepted { get; set; }
    public string State { get; set; } = "NOT PROCESSED";
    public bool Validated { get; set; }
}