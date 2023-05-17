namespace ModelBinder.Models;

public class NatsRecordList
{
    public int RecordCount => Records.Count;

    public List<Dictionary<string, object>> Records { get; set; } = new();
}