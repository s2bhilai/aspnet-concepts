using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace actionfilters.Filters;

public class SimpleActionFilters : IActionFilter
{
    private readonly Stopwatch _watch = new();

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _watch.Stop();

        var viewData = ((Controller)context.Controller).ViewData;

        var viewDataCount = viewData.Count;

        var count = 0;
        foreach (var key in viewData.Keys)
        {
            context.HttpContext.Response.Headers.Add($"x-action-viewdata-{count}", $"{key}:{viewData[key]}");
            count++;
        }

        var msTaken = _watch.Elapsed.Milliseconds;
        var timeString = $"{msTaken}ms";

        context.HttpContext.Response.Headers.Add("x-action-execution-time", $"{msTaken}ms");

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _watch.Start();
    }
}