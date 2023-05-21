using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Text.RegularExpressions;
using viewengine.Models;

namespace viewengine.ViewEngines;

public class BillView : IView
{
    public BillView(string path)
    {
        Path = path;
    }

    //View Engine will populate Path after finding the template
    public string Path { get; private set; }

    public Task RenderAsync(ViewContext context)
    {
        var template = File.ReadAllText(Path);

        var regExPattern = @"\[([a-z]{1,})\s([A-Z]{1,})\|(.*)\|(.*)\]"; // Pattern to find square bracket directives in doc template
        var matches = Regex.Matches(template, regExPattern);

        var billData = context.ViewData["BillingData"] as BillData;

        // Process and generate template output here
        foreach (Match rexMatch in matches)
        {
            // There should really be better error handling here, but this is just an example, input data in a
            // production app should always be checked for safety and other problem causing issues.
            var groups = rexMatch.Groups;
            var keyWord = groups[1].Value;
            var objName = groups[2].Value;
            var position = groups[3].Value;
            var size = groups[4].Value;

            switch (keyWord)
            {
                case "comment":
                    // We dont process comments, but we do remove them from the output
                    template = template.Replace(rexMatch.Value, String.Empty);
                    continue;

                case "section":
                    {
                        // Here we process a "draw section" directive
                        // TODO: extract the bill data, use a map to find the object name in the data etc maybe?

                        var positionParts = position.Trim().Split(',');
                        var sizeParts = size.Trim().Split(',');

                        //<div style="left: 0px; top: 0px; width: 0px; height: 0px;"/>

                        if (billData != null)
                        {
                            string outputLine = $"<div class=\"section\" style=\"left: {positionParts[0]}; " +
                                                $"top: {positionParts[1]}; width: {sizeParts[0]}; height: {sizeParts[1]}\">" +
                                                $"{getSectionContent(objName, billData)}</div>";

                            template = template.Replace(rexMatch.Value, outputLine);
                        }

                        break;
                    }

                default:
                    {
                        // Replace any directive we don't recognize with empty string
                        template = template.Replace(rexMatch.Value, String.Empty);
                        continue;
                    }

            }
        }

        return context.Writer.WriteAsync(template);
    }

    private string getSectionContent(string objName, BillData billData)
    {
        // Sections here are hard coded, but they could just as easy be taken from
        // template files themselves ...
        string result = string.Empty;

        switch (objName)
        {
            case "GENERATOR" when billData.GeneratorValid:
                result = $"<div style=\"text-align: center; font-size: 0.8rem\">" +
                         $"<span>Generated by: {billData.Generator.GeneratedBy} at {billData.Generator.GeneratedAt.ToShortTimeString()} " +
                         $"on {billData.Generator.GeneratedAt.ToShortDateString()}</span> " +
                         $"<span>Bill issue number: {billData.Generator.IssueNumber}</span></div>";
                break;

            case "CUSTOMER" when billData.CustomerValid:
                result = $"<div style=\"text-align: right;\">" +
                         $"<div>{billData.Customer.Title} {billData.Customer.Forename} {billData.Customer.Surname}</div>" +
                         $"<div>{billData.Customer.HouseAddress} {billData.Customer.Street}</div>" +
                         $"<div>{billData.Customer.Town}</div>" +
                         $"<div>{billData.Customer.County}</div>" +
                         $"<div>{billData.Customer.PostCode}</div>" +
                         $"<div>&nbsp;</div>" +
                         $"<div>Account Number: {billData.Customer.AccountNumber}</div>" +
                         $"<div>Phone Number: {billData.Subscription.SubscriberPhoneNumber}</div>" +
                         $"</div>";
                break;

            case "ACCOUNT" when billData.AccountValid:
                result = $"<div style=\"text-align: left;\">" +
                         $"<table>" +
                         $"<tr><td>Account:</td><td>{billData.Account.AccountNumber}</td></tr>" +
                         $"<tr><td>Discounts:</td><td>�{billData.Account.Discounts}</td></tr>" +
                         $"<tr><td>Call Charges:</td><td>�{billData.Account.ChargesThisPeriod}</td></tr>" +
                         $"<tr><td>Outstatnding Balance:</td><td>�{billData.Account.OutstandingBalance}</td></tr>" +
                         $"</table>" +
                         $"</div>";
                break;

            case "BUNDLES" when billData.BundlesValid:
                result = $"<div style=\"text-align: left;\"><table style=\"width: 100%;\">" +
                         $"<tr><th>Bundle Name</th><th>Product Code</th></tr>";

                foreach (var bundle in billData.Bundles)
                {
                    result = result + $"<tr><td>{bundle.BundleName}</td><td>{bundle.ProductCode}</td></tr>";
                }

                result = result + $"</table></div>";
                break;

            case "ITEMS" when billData.ItemsValid:
                result = $"<div style=\"text-align: left;\"><table style=\"width: 100%;\">" +
                         $"<tr>" +
                         $"<th>Destination</th>" +
                         $"<th>Type</th>" +
                         $"<th>When</th>" +
                         $"<th>Duration</th>" +
                         $"<th>Cost</th>" +
                         $"</tr>";

                foreach (var item in billData.Items)
                {
                    result = result + $"<tr>" +
                             $"<td>{item.DestinationNumber}</td>" +
                             $"<td>{item.TransactionType}</td>" +
                             $"<td>{item.DateAndTime}</td>" +
                             $"<td>{item.BillableDuration}</td>" +
                             $"<td>�{item.BillableCost}</td>" +
                             $"</tr>";
                }

                result = result + $"</table></div>";
                break;

            case "SUBSCRIPTION" when billData.SubscriptionValid:
                result = $"<div style=\"text-align: right;\">" +
                         $"<div>Invoice Number: {billData.Subscription.InvoiceNumber}</div>" +
                         $"<div>Care Charges: �{billData.Subscription.CareCharges}</div>" +
                         $"<div>VAT: �{billData.Subscription.Vat}</div>" +
                         $"<div>Total Charges: �{billData.Subscription.TotalCharges}</div>" +
                         $"</div>";
                break;
        }

        return result;
    }
}
