using LMS.Data;
using LMS.Models.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class ClaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ClaseController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ===============================
        // VALIDACIÓN SESIÓN
        // ===============================
        private bool SesionValida()
        {
            return HttpContext.Session.GetInt32("ProfesorId") != null
                && HttpContext.Session.GetInt32("SedeId") != null;
        }

        // ===============================
        // CREAR - GET
        // ===============================
        public IActionResult Crear(int cursoId)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            var ultimoOrden = _context.Clases
                .Where(c => c.CursoId == cursoId)
                .Select(c => (int?)c.Orden)
                .Max() ?? 0;

            var model = new Clase
            {
                CursoId = cursoId,
                Orden = ultimoOrden + 1
            };

            return View(model);
        }

        // ===============================
        // CREAR - POST
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Crear(Clase model, IFormFile? imagen)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            //errores
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                throw new Exception("Errores: " + string.Join(" | ", errores));
            }
            if (model.CursoId == 0)
            {
                throw new Exception("CursoId viene en 0");
            }

            // Guardar imagen (opcional)
            if (imagen != null && imagen.Length > 0)
            {
                var extension = Path.GetExtension(imagen.FileName).ToLower();

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    ModelState.AddModelError("", "Solo se permiten imágenes JPG o PNG.");
                    return View(model);
                }

                var carpeta = Path.Combine(_environment.WebRootPath, "entregas");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombre = Guid.NewGuid() + extension;
                var ruta = Path.Combine(carpeta, nombre);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                model.VideoPath = "/entregas/" + nombre; // reutilizamos el campo
            }

            var ultimoOrden = _context.Clases
                .Where(c => c.CursoId == model.CursoId)
                .Select(c => (int?)c.Orden)
                .Max() ?? 0;

            model.Orden = ultimoOrden + 1;

            _context.Clases.Add(model);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Clase creada correctamente.";

            return RedirectToAction("Detalle", "Profesor", new { id = model.CursoId });
        }

        // ===============================
        // EDITAR - GET
        // ===============================
        public async Task<IActionResult> Editar(int id)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            var clase = await _context.Clases.FindAsync(id);

            if (clase == null)
                return RedirectToAction("Index", "Profesor");

            return View(clase);
        }

        // ===============================
        // EDITAR - POST
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Editar(Clase model, IFormFile? imagen)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            var clase = await _context.Clases.FindAsync(model.ClaseId);

            if (clase == null)
                return RedirectToAction("Index", "Profesor");

            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                throw new Exception("Errores: " + string.Join(" | ", errores));
            }

            // Actualizar campos
            clase.Titulo = model.Titulo;
            clase.Descripcion = model.Descripcion;
            clase.ContenidoTexto = model.ContenidoTexto;
            clase.RequiereTexto = model.RequiereTexto;
            clase.RequiereImagen = model.RequiereImagen;

            // Nueva imagen
            if (imagen != null && imagen.Length > 0)
            {
                var extension = Path.GetExtension(imagen.FileName).ToLower();

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    ModelState.AddModelError("", "Solo se permiten imágenes JPG o PNG.");
                    return View(model);
                }

                var carpeta = Path.Combine(_environment.WebRootPath, "entregas");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombre = Guid.NewGuid() + extension;
                var ruta = Path.Combine(carpeta, nombre);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                clase.VideoPath = "/entregas/" + nombre;
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Clase actualizada.";

            return RedirectToAction("Detalle", "Profesor", new { id = clase.CursoId });
        }

        // ===============================
        // ELIMINAR
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Eliminar(int claseId)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            var clase = await _context.Clases.FindAsync(claseId);

            if (clase == null)
                return RedirectToAction("Index", "Profesor");

            int cursoId = clase.CursoId;

            _context.Clases.Remove(clase);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Clase eliminada correctamente.";

            return RedirectToAction("Detalle", "Profesor", new { id = cursoId });
        }

        // ===============================
        // PREVIEW
        // ===============================
        public IActionResult Preview(int id)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            var clase = _context.Clases
                .FirstOrDefault(c => c.ClaseId == id);

            if (clase == null)
                return NotFound();

            return View(clase);
        }
    }
}