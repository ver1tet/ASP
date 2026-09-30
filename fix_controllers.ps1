$folders = @("18.07_1", "11.08_2", "13.08_3", "15.08_4", "18.08_5", "29.08_6", "03.09_7", "05.09_8")

for ($i = 0; $i -lt $folders.Length; $i++) {
    $f = $folders[$i]
    $code = @"
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
"@

    if ($i -ge 1) { $code += "`n    public IActionResult Products() { return View(); }" }
    if ($i -ge 2) { $code += "`n    public IActionResult TimeService() { return View(); }" }
    if ($i -ge 3) { $code += "`n    [HttpGet] public IActionResult Models() { return View(new ExtendedFormModel()); }`n    [HttpPost] public IActionResult Models(ExtendedFormModel model) { ViewBag.Submitted = true; return View(model); }" }
    if ($i -ge 4) { $code += "`n    public IActionResult EFCore() { return View(); }" }
    if ($i -ge 6) { $code += "`n    [HttpGet] public IActionResult Register() { return View(new RegisterModel()); }`n    [HttpPost] public IActionResult Register(RegisterModel model) { if (ModelState.IsValid) { ViewBag.Success = true; } return View(model); }" }

    $code += @"

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
"@
    Set-Content -Path "C:\Users\Admin\.gemini\antigravity\scratch\ASP_HW\$f\Controllers\HomeController.cs" -Value $code
}
