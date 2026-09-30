using Microsoft.AspNetCore.Mvc;

namespace AspHomework.Controllers;

public class ProductGroupsController : Controller
{
    // MVC-контролер, що діє як API: повертає JSON
    [HttpGet]
    public IActionResult Index()
    {
        var groups = new[]
        {
            new { Id = 1, Name = "Ноутбуки та ПК", ProductCount = 150 },
            new { Id = 2, Name = "Комплектуючі", ProductCount = 500 },
            new { Id = 3, Name = "Периферія", ProductCount = 300 },
            new { Id = 4, Name = "Мережеве обладнання", ProductCount = 80 }
        };

        return Json(groups);
    }
}
