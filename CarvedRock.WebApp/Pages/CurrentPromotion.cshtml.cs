using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarvedRock.WebApp.Pages
{
    public class CurrentPromotionModel : PageModel
    {
        public string RemoteContent { get; set; }

        public async Task OnGet()
        {
            RemoteContent = await GetRemoteContentAsync();
        }

        public Task<string> GetRemoteContentAsync()
        {
            return Task.FromResult("Some content from remote API or Db call");
        }
    }
}
