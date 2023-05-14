
namespace UrlChanger.Models
{
    public class BlogPost
    {
        public int Month { get; set; }
        public int Day { get; set; }
        public int Year { get; set; }
        public string Title { get; set; } = "";
        public string Subject { get; set; } = "";
    }
}