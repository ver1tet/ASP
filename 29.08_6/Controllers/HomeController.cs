using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspHomework.Models;

namespace AspHomework.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger) { _logger = logger; }

    public IActionResult Index() { return View(); }
    public IActionResult Privacy() { return View(); }
    public IActionResult Principles() { return View(); }
    public IActionResult Products() { return View(); }
    public IActionResult TimeService() { return View(); }
    [HttpGet] public IActionResult Models() { return View(new ExtendedFormModel()); }
    [HttpPost] public IActionResult Models(ExtendedFormModel model) { ViewBag.Submitted = true; return View(model); }
    public IActionResult EFCore() { return View(); }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
