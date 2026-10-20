namespace ASP_P42.Services.Storage
{
    public class LocalStorageService : IStorageService
    {
        // Д.З. 08.09.2026 - Розширити перелік дозволених розширень на усі поширені
        // формати зображень, підкоригувати Content-Type для них
        private readonly String[] allowedExtensions =
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif",
            ".bmp",
            ".tiff",
            ".tif",
            ".svg",
            ".ico",
            ".avif",
            ".heic",
            ".heif",
        ];

        private readonly String localFolder = "LocalStorage";

        public byte[] Load(string filename)
        {
            return File.ReadAllBytes(
                Path.Combine(localFolder, filename)
            );
        }

        public string Save(IFormFile file)
        {
            // виконуємо перевірку на наявність та валідність даних
            if (file == null) throw new ArgumentNullException(
                nameof(file), "Data not received");

            if (file.Length < 256) throw new ArgumentException("File too short");
            if (file.Length > 1e7) throw new ArgumentException("File too long");

            // визначаємо розширення, з нього тип файлу
            int dotPosition = file.FileName.LastIndexOf('.');
            if (dotPosition < 0) throw new ArgumentException("File must have extension");
            String ext = file.FileName[dotPosition..].ToLower();
            if (!allowedExtensions.Contains(ext))
            {
                throw new ArgumentException("File type not allowed");
            }

            // генеруємо нове ім'я для файлу, розширення зберігаємо
            String savedName = Guid.NewGuid() + ext;
            using FileStream stream = File.OpenWrite(
                Path.Combine(localFolder, savedName)
            );
            file.CopyTo(stream);
            return savedName;
        }
    }
}
