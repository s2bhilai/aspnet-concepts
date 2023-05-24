using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using actionfilters.Models;
using actionfilters.Filters;

namespace actionfilters.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    //[ServiceFilter(typeof(SimpleActionFilters))]
    [SimpleActionFilter]
    public IActionResult Privacy()
    {
        ViewBag.BagData = "From View Bag";
        ViewData["DataData"] = "From View Data";

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
