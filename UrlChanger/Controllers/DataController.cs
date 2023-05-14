using Microsoft.AspNetCore.Mvc;
using UrlChanger.Services;

namespace UrlChanger.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataController: Controller
    {
        private ILogger<DataController> _logger;
        private BlogDataService _blogService;

        public DataController(ILogger<DataController> logger, BlogDataService blogDataService)
        {
            _logger = logger;
            _blogService = blogDataService;
        }

        [HttpGet("monthlyblogs/{month:int}")]
        [Produces("application/json")]
        public IActionResult GetMonthlyBlogs(int month)
        {
            return Ok(_blogService.FetchBlogsForMonth(month));
        }

        [HttpGet("subjectblogs/{subject}")]
        [Produces("application/json")]
        public IActionResult GetSubjectBlogs(string subject)
        {
            var blogs = _blogService.FetchBlogsForSubject(subject);

            return Ok(blogs);
        }
    }
}