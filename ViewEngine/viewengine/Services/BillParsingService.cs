using viewengine.Models;

namespace viewengine.Services;

public class BillParsingService
{
    private List<string> _billContents = new List<string>();

    private GeneratorRecord? _generator;
    private CustomerRecord? _customer;
    private AccountRecord? _account;
    private SubscriptionRecord? _subscription;
    private readonly List<BundleRecord> _bundles = new List<BundleRecord>();
    private readonly List<ProductCostRecord> _productCosts = new List<ProductCostRecord>();
    private readonly List<CallDetailRecord> _callDetailRecords = new List<CallDetailRecord>();

    public void ParseBill(string billFile)
    {
        _billContents = File.ReadAllLines(billFile).ToList();

        foreach (var billContent in _billContents.TakeWhile(billContent => !billContent.StartsWith("ENDINV")))
        {
            if (billContent.StartsWith("PROG_R")) { _generator = new GeneratorRecord(billContent); continue; }
            if (billContent.StartsWith("CUST_R")) { _customer = new CustomerRecord(billContent); continue; }
            if (billContent.StartsWith("ACC__R")) { _account = new AccountRecord(billContent); continue; }
            if (billContent.StartsWith("SUBS_R")) { _subscription = new SubscriptionRecord(billContent); continue; }
            if (billContent.StartsWith("BDHD_R")) { _bundles.Add(new BundleRecord(billContent)); continue; }
            if (billContent.StartsWith("PROD_R")) { _productCosts.Add(new ProductCostRecord(billContent)); continue; }
            if (billContent.StartsWith("CDR__R")) { _callDetailRecords.Add(new CallDetailRecord(billContent)); }
        }
    }

    public BillData FetchBillData()
     => new BillData
     {
         Generator = _generator,
         Customer = _customer,
         Account = _account,
         Subscription = _subscription,
         Bundles = _bundles,
         BundleCosts = _productCosts,
         Items = _callDetailRecords,
         GeneratorValid = _generator != null,
         CustomerValid = _customer != null,
         AccountValid = _account != null,
         SubscriptionValid = _subscription != null,
         BundlesValid = _bundles.Count > 0,
         BundleCostsValid = _productCosts.Count > 0,
         ItemsValid = _callDetailRecords.Count > 0
     };
}