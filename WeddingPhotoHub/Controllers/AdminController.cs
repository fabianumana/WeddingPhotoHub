using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingPhotoHub.Data;

namespace WeddingPhotoHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Dashboard()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Descargar(string nombreArchivo)
        {
            if (string.IsNullOrEmpty(nombreArchivo))
                return RedirectToAction("Dashboard");

            var nombreSeguro = Path.GetFileName(nombreArchivo);
            var ruta = Path.Combine(_env.WebRootPath, "uploads", nombreSeguro);

            if (!System.IO.File.Exists(ruta))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(ruta);

            return File(bytes, "application/octet-stream", nombreSeguro);
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Eliminar(string nombreArchivo)
        {
            var nombreSeguro = Path.GetFileName(nombreArchivo);
            var ruta = Path.Combine(_env.WebRootPath, "uploads", nombreSeguro);

            if (string.IsNullOrEmpty(nombreArchivo))
                return RedirectToAction("Dashboard");

            if (System.IO.File.Exists(ruta))
            {
                System.IO.File.Delete(ruta);
                TempData["Mensaje"] = "Archivo eliminado exitosamente.";
            }
            else
            {
                TempData["Error"] = "Archivo no encontrado.";
            }

            return RedirectToAction("Dashboard");
        }
    }
}
