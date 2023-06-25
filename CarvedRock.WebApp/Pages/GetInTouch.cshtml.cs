using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarvedRock.WebApp.Pages
{
    public class GetInTouchModel : PageModel
    {
        [BindProperty]
        public string Content { get; set; }

        public void OnGet()
        {
        }

        public async Task OnPost()
        {
            //sync over async - causes trouble - threadpool starvation
            //var content = Request.Form["content"];
            //var emailAddress = Request.Form["emailaddress"];

            //Using Async alternatives - ReadFormAsync
            var form = await Request.ReadFormAsync();

            var betterContent = form["content"];
            var betterEmail = form["emailaddress"];

            //Using Model binding features of framework
            var bestContent = Content;
        }
    }
}
