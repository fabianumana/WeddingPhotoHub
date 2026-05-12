using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using WeddingPhotoHub.Data;
using WeddingPhotoHub.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace WeddingPhotoHub.Controllers
{
    public class UploadController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public UploadController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public IActionResult Index()
        {
            var fotos = _context.Fotos
            .OrderByDescending(x => x.FechaSubida)
            .ToList();

            return View(fotos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> Subir(List<IFormFile> archivos)
        {
            if (archivos == null || !archivos.Any())
            {
                TempData["Error"] = "No seleccionaste imágenes.";
                return RedirectToAction("Index");
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var errores = new List<string>();
            var exitos = 0;

            foreach (var archivo in archivos)
            {
                if (archivo == null || archivo.Length == 0)
                    continue;

                var extension = Path.GetExtension(archivo.FileName).ToLower();

                if (!allowedTypes.Contains(archivo.ContentType) ||
                    !allowedExtensions.Contains(extension))
                {
                    errores.Add($"{archivo.FileName} no es válido");
                    continue;
                }

                if (archivo.Length > 10 * 1024 * 1024)
                {
                    errores.Add($"{archivo.FileName} supera 10MB");
                    continue;
                }

                var result = await _cloudinaryService.UploadImageAsync(archivo);

                if (string.IsNullOrWhiteSpace(result.Url) ||
                    string.IsNullOrWhiteSpace(result.PublicId))
                {
                    errores.Add($"{archivo.FileName} falló en Cloudinary");
                    continue;
                }

                _context.Fotos.Add(new Foto
                {
                    Url = result.Url,
                    PublicId = result.PublicId,
                    FechaSubida = DateTime.UtcNow,
                    Tamaño = archivo.Length,
                    NombreArchivo = archivo.FileName
                });

                exitos++;
            }

            await _context.SaveChangesAsync();

            if (exitos > 0)
                TempData["Mensaje"] = $"{exitos} fotos subidas correctamente.";

            if (errores.Any())
                TempData["Error"] = string.Join(" | ", errores);

            return RedirectToAction("Index");
        }
    }
}
