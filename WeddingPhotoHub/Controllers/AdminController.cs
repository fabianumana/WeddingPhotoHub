using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingPhotoHub.Data;

namespace WeddingPhotoHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public AdminController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public IActionResult Dashboard()
        {
            var fotos = _context.Fotos
                .OrderByDescending(x => x.FechaSubida)
                .ToList();

            return View(fotos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Descargar(int id)
        {
            var foto = _context.Fotos.FirstOrDefault(x => x.Id == id);

            if (foto == null || string.IsNullOrWhiteSpace(foto.Url))
            {
                TempData["Error"] = "Imagen no encontrada.";
                return RedirectToAction("Dashboard");
            }

            return Redirect(foto.Url + "?fl_attachment=true");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            var foto = _context.Fotos.FirstOrDefault(x => x.Id == id);

            if (foto == null)
            {
                TempData["Error"] = "Imagen no encontrada.";
                return RedirectToAction("Dashboard");
            }

            if (!string.IsNullOrWhiteSpace(foto.PublicId))
            {
                _cloudinaryService.DeleteImage(foto.PublicId);
            }

            _context.Fotos.Remove(foto);
            _context.SaveChanges();

            TempData["Mensaje"] = "Imagen eliminada correctamente.";

            return RedirectToAction("Dashboard");
        }
    }
}
