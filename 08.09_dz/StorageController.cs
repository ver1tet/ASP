using ASP_P42.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Controllers
{
    public class StorageController(IStorageService storageService) : Controller
    {
        private readonly IStorageService _storageService = storageService;

        [HttpGet]
        public IActionResult Image(String id)
        {
            // id = filename, для того щоб повернути файл необхідно зазначити
            // його тип, знов аналізуємо розширення
            int dotPosition = id.LastIndexOf('.');
            String ext = "";
            if (dotPosition > 0)
            {
                ext = id[dotPosition..].ToLower();
            }

            // Д.З. 08.09.2026 - Підкоригувати Content-Type для всіх поширених форматів зображень
            String contentType = ext switch
            {
                ".jpg" or ".jpeg"   => "image/jpeg",
                ".png"              => "image/png",
                ".webp"             => "image/webp",
                ".gif"              => "image/gif",
                ".bmp"              => "image/bmp",
                ".tiff" or ".tif"   => "image/tiff",
                ".svg"              => "image/svg+xml",
                ".ico"              => "image/x-icon",
                ".avif"             => "image/avif",
                ".heic"             => "image/heic",
                ".heif"             => "image/heif",
                _                   => "application/octet-stream"
            };

            try
            {
                return File(_storageService.Load(id), contentType);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
