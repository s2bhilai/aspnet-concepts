using Microsoft.AspNetCore.Razor.TagHelpers;

namespace taghelpers.TagHelpers;

public class EmailTagHelper : TagHelper
{
    public string Addresss { get; set; } = "";
    public string Domain { get; set; } = "";
    public string Name { get; set; } = "";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.Attributes.SetAttribute("href", $"mailto:{Addresss}@{Domain}");
        output.Content.SetContent($"Send Email to {Name}");
    }
}