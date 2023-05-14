using UrlChanger.Models;

namespace UrlChanger.Services
{
    public class BlogDataService
    {
        public List<BlogPost> _blogPosts = new List<BlogPost>()
        {
            new BlogPost() { Day = 1, Month = 1, Year = 2022, Subject = "IOT1", Title = "Title1"},
            new BlogPost() { Day = 2, Month = 1, Year = 2022, Subject = "IOT2", Title = "Title1"},
            new BlogPost() { Day = 3, Month = 1, Year = 2022, Subject = "IOT3", Title = "Title1"},
            new BlogPost() { Day = 4, Month = 1, Year = 2022, Subject = "IOT4", Title = "Title1"},
            new BlogPost() { Day = 5, Month = 2, Year = 2022, Subject = "IOT1", Title = "Title1"},
            new BlogPost() { Day = 6, Month = 2, Year = 2022, Subject = "IOT1", Title = "Title1"},
            new BlogPost() { Day = 7, Month = 2, Year = 2022, Subject = "IOT2", Title = "Title1"},
            new BlogPost() { Day = 8, Month = 2, Year = 2022, Subject = "IOT2", Title = "Title1"},
            new BlogPost() { Day = 9, Month = 2, Year = 2022, Subject = "IOT9", Title = "Title1"},
            new BlogPost() { Day = 10, Month = 3, Year = 2022, Subject = "IOT3", Title = "Title1"},
            new BlogPost() { Day = 11, Month = 3, Year = 2022, Subject = "IOT3", Title = "Title1"},
            new BlogPost() { Day = 12, Month = 3, Year = 2022, Subject = "IOT12", Title = "Title1"},
            new BlogPost() { Day = 13, Month = 0, Year = 2022, Subject = "IOT13", Title = "Title1"},
            new BlogPost() { Day = 0, Month = 0, Year = 0, Subject = "IOT14", Title = "Title1"},
            new BlogPost() { Day = 0, Month = 0, Year = 0, Subject = "IOT15", Title = "Title1"}
        };

        public List<BlogPost> FetchBlogsForMonth(int month) =>
            _blogPosts.Where(r => r.Month == month).ToList();

        public List<BlogPost> FetchBlogsForSubject(string subject) =>
            _blogPosts.Where(r => r.Subject.Equals(subject)).ToList();
    }
}