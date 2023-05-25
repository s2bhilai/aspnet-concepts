using Microsoft.AspNetCore.Razor.TagHelpers;

namespace taghelpers.TagHelpers;

public class AlertTagHelper: TagHelper
{
    public string AlertType { get; set; } = "info"; //Default value if nothing specified in UI
    public string InfoLink { get; set; } = "";
    public string Heading { get; set; } = "";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", $"alert alert-{AlertType} text-left");
        output.Attributes.SetAttribute("role", "alert");

        if(!string.IsNullOrEmpty(InfoLink))
        {
            output.PostContent.AppendHtml(
                $"<hr/><a class=\"alert-link\" href=\"{InfoLink}\">Click Here for details</a>");
        }

        if(!string.IsNullOrEmpty(Heading))
        {
            output.PreContent.AppendHtml($"<h4 class=\"alert-heading\">{Heading}</h4>");
        }
    }
}