using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using viewengine.Models;
using viewengine.Services;

namespace viewengine.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BillParsingService _billParsingService;

    public HomeController(ILogger<HomeController> logger,BillParsingService billParsingService)
    {
        _logger = logger;
        _billParsingService = billParsingService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    public IActionResult PhoneBill()
    {
        ViewData["Message"] = "Message Data";

        _billParsingService.ParseBill("telephonebill.txt");

        ViewData["BillingData"] = _billParsingService.FetchBillData();

        return View();
    }
}
