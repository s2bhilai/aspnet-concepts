using Microsoft.AspNetCore.Http.Extensions;
using UrlChanger.Services;

namespace UrlChanger.Middleware
{
    public class UrlChangeMiddleware
    {
        private RequestDelegate _next;

        public UrlChangeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var url = context.Request.Path.Value;

            if(url.StartsWith("/api/data/monthlyblogs"))
            {
                Uri uri = new Uri(context.Request.GetDisplayUrl());

                var parameter = uri.Segments.Last();

                if(int.TryParse(parameter,out int monthNumber))
                {
                    var subject = MonthMapService.MonthToSubject(monthNumber);
                    if(string.IsNullOrEmpty(subject))
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }

                    context.Request.Path = $"/api/data/subjectblogs/{subject}";
                }
            }

            await _next(context);
        }
    }

    public static class UrlChangeMiddlewareExtension
    {
        public static IApplicationBuilder UseUrlChangeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UrlChangeMiddleware>();
        }
    }
}