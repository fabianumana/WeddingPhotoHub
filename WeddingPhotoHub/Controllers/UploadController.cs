using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using WeddingPhotoHub.Data;

namespace WeddingPhotoHub.Controllers
{
    public class UploadController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UploadController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var ruta = Path.Combine(_env.WebRootPath, "uploads");

            var fotos = new List<string>();

            if (Directory.Exists(ruta))
            {
                var archivos = Directory.GetFiles(ruta)
                .OrderByDescending(f => System.IO.File.GetCreationTime(f));

                foreach (var archivo in archivos)
                {
                    var nombreArchivo = Path.GetFileName(archivo);
                    fotos.Add("/uploads/" + nombreArchivo);
                }
            }

            return View(fotos);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [RequestSizeLimit(52428800)] // 50MB total
        public async Task<IActionResult> Subir(List<IFormFile> archivos)
        {
            if (archivos == null || !archivos.Any())
            {
                TempData["Error"] = "No seleccionaste imágenes.";
                return RedirectToAction("Index");
            }

            var ruta = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }

            var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/webp" };

            foreach (var archivo in archivos)
            {
                if (archivo == null || archivo.Length == 0)
                    continue;

                if (!tiposPermitidos.Contains(archivo.ContentType))
                    continue;

                if (archivo.Length > 10 * 1024 * 1024)
                    continue;

                var extension = Path.GetExtension(archivo.FileName);

                if (string.IsNullOrEmpty(extension))
                    continue;

                var nombreUnico = Guid.NewGuid().ToString() + extension;

                var filePath = Path.Combine(ruta, nombreUnico);

                using (var image = await Image.LoadAsync(archivo.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(1920, 1080)
                    }));

                    var ext = extension.ToLower();

                    if (ext == ".jpg" || ext == ".jpeg")
                    {
                        await image.SaveAsync(filePath, new JpegEncoder
                        {
                            Quality = 75
                        });
                    }
                    else if (ext == ".png")
                    {
                        await image.SaveAsync(filePath, new PngEncoder());
                    }
                    else if (ext == ".webp")
                    {
                        await image.SaveAsync(filePath, new WebpEncoder());
                    }
                    else
                    {
                        await image.SaveAsync(filePath);
                    }
                }
            }

            TempData["Mensaje"] = "Fotos subidas exitosamente.";

            return RedirectToAction("Index");
        }
    }
}
