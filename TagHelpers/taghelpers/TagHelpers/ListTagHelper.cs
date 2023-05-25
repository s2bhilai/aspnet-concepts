using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace taghelpers.TagHelpers;

public class ListTagHelper: TagHelper
{

    //Since we have to read child content, so used async version
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "ul";

        var existingContent = await output.GetChildContentAsync();//For this , used async process method
        var allContent = existingContent.GetContent();
        var items = allContent.Trim().Split(',');

        var outputHtml = new StringBuilder();

        foreach (var item in items)
        {
            outputHtml.Append($"<li>{item}</li>");
        }

        output.Content.SetHtmlContent(outputHtml.ToString());
    }
}