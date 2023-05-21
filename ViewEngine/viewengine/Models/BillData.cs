namespace viewengine.Models;

public class BillData
{
  public GeneratorRecord? Generator { get; set;} = null;
  public CustomerRecord? Customer { get; set;} = null; 
  public AccountRecord? Account { get; set;} = null;
  public SubscriptionRecord? Subscription { get; set; } = null;
  public List<BundleRecord> Bundles { get; set; } = new List<BundleRecord>();
  public List<ProductCostRecord> BundleCosts { get; set; } = new List<ProductCostRecord>();
  public List<CallDetailRecord> Items { get; set; } = new List<CallDetailRecord>();
  
  public bool GeneratorValid { get; set; } = false;
  public bool CustomerValid { get; set; } = false;
  public bool AccountValid { get; set; } = false; 
  public bool SubscriptionValid { get; set; } = false;
  public bool BundlesValid { get; set; } = false;
  public bool BundleCostsValid { get; set; } = false;
  public bool ItemsValid { get; set; } = false;

}