using LMS.Data;
using LMS.Models.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class ModuloController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ModuloController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // =========================
        // CREAR MÓDULO - GET
        // =========================
        [HttpGet]
        public IActionResult CrearModulo(int cursoId)
        {
            var modulo = new Modulo
            {
                CursoId = cursoId
            };

            return View(modulo);
        }

        // =========================
        // CREAR MÓDULO - POST
        // =========================
        [HttpPost]
        public async Task<IActionResult> CrearModulo(Modulo model, IFormFile video)
        {
            ModelState.Remove("VideoPath"); // Evitar validación automática del video

            if (!ModelState.IsValid)
                return View(model);

            // Guardar video si existe
            if (video != null && video.Length > 0)
            {
                var extension = Path.GetExtension(video.FileName).ToLower();
                if (extension != ".mp4")
                {
                    ModelState.AddModelError("", "Solo se permiten archivos MP4.");
                    return View(model);
                }

                if (video.Length > 100 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "El video no puede superar los 100MB.");
                    return View(model);
                }

                var carpeta = Path.Combine(_environment.WebRootPath, "videos");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreUnico = Guid.NewGuid().ToString() + ".mp4";
                var rutaCompleta = Path.Combine(carpeta, nombreUnico);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }

                model.VideoPath = "/videos/" + nombreUnico;
            }

            _context.Modulos.Add(model);
            await _context.SaveChangesAsync();

            // Redirigir a seleccionar preguntas según la cantidad elegida
            return RedirectToAction("Preguntas", new { moduloId = model.ModuloId });
        }


        // =========================
        // ELIMINAR MÓDULO (con preguntas)
        // =========================
        [HttpPost]
        public IActionResult EliminarModulo(int moduloId)
        {
            var modulo = _context.Modulos
                .Include(m => m.Preguntas)
                .Include(m => m.Curso)
                .FirstOrDefault(m => m.ModuloId == moduloId);

            if (modulo == null)
            {
                TempData["Error"] = "No se encontró el módulo.";
                return RedirectToAction("Index", "Profesor");
            }

            try
            {
                if (modulo.Preguntas.Any())
                    _context.Preguntas.RemoveRange(modulo.Preguntas);

                _context.Modulos.Remove(modulo);
                _context.SaveChanges();

                TempData["Mensaje"] = $"Módulo '{modulo.Titulo}' eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error al eliminar el módulo: {ex.Message}";
            }

            return RedirectToAction("Detalle", "Profesor", new { id = modulo.CursoId });
        }

        // =========================
        // LISTAR Y GESTIONAR PREGUNTAS
        // =========================
        public IActionResult Preguntas(int moduloId)
        {
            // Buscar módulo
            var modulo = _context.Modulos
                .Include(m => m.Preguntas)
                .FirstOrDefault(m => m.ModuloId == moduloId);

            if (modulo == null)
                return RedirectToAction("Index", "Profesor"); // Si no existe, vuelve a Profesor/Index

            ViewBag.ModuloId = modulo.ModuloId;
            ViewBag.TituloModulo = modulo.Titulo;
            ViewBag.CantidadRequerida = modulo.CantidadPreguntas;
            ViewBag.CantidadActual = modulo.Preguntas?.Count ?? 0;

            // Retornar lista de preguntas
            var preguntas = modulo.Preguntas?.ToList() ?? new List<Pregunta>();
            return View(preguntas); // Vista: Preguntas.cshtml
        }

        // =========================
        // CREAR PREGUNTA
        // =========================
        [HttpPost]
        public IActionResult CrearPregunta(Pregunta model)
        {
            // Buscar el módulo
            var modulo = _context.Modulos
                .Include(m => m.Preguntas)
                .Include(m => m.Curso) // Necesario para obtener ProfesorId
                .FirstOrDefault(m => m.ModuloId == model.ModuloId);

            if (modulo == null)
            {
                TempData["Error"] = "No se encontró el módulo.";
                return RedirectToAction("Index", "Profesor");
            }

            // Asignar ProfesorId automáticamente
            model.ProfesorId = modulo.Curso.ProfesorId;

            // Verificar que no exceda la cantidad de preguntas requeridas
            if (modulo.Preguntas.Count >= modulo.CantidadPreguntas)
            {
                TempData["Error"] = "Ya se alcanzó la cantidad máxima de preguntas para este módulo.";
                return RedirectToAction("Preguntas", new { moduloId = model.ModuloId });
            }

            // Guardar pregunta
            _context.Preguntas.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Preguntas", new { moduloId = model.ModuloId });
        }

        // =========================
        // VISTA PREVIA DEL MÓDULO
        // =========================
        public IActionResult Preview(int id)
        {
            var modulo = _context.Modulos
                .Include(m => m.Preguntas)
                .FirstOrDefault(m => m.ModuloId == id);

            if (modulo == null)
                return RedirectToAction("Index", "Profesor");

            return View(modulo);
        }

        // =========================
        // VISTA PARA EDITAR EL MÓDULO
        // =========================
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var modulo = _context.Modulos
                .Include(m => m.Preguntas)
                .FirstOrDefault(m => m.ModuloId == id);

            if (modulo == null)
                return RedirectToAction("Index", "Profesor");

            return View(modulo); // Vista: Editar.cshtml
        }

        // POST: Guardar cambios en el módulo
        [HttpPost]
        public async Task<IActionResult> Editar(Modulo model, IFormFile? video)
        {
            var modulo = _context.Modulos.FirstOrDefault(m => m.ModuloId == model.ModuloId);
            if (modulo == null)
            {
                TempData["Error"] = "No se encontró el módulo.";
                return RedirectToAction("Index", "Profesor");
            }

            // Actualizar campos
            modulo.Titulo = model.Titulo;
            modulo.Descripcion = model.Descripcion;
            modulo.ContenidoTexto = model.ContenidoTexto;
            modulo.NotaMinima = model.NotaMinima;
            modulo.Orden = model.Orden;
            modulo.CantidadPreguntas = model.CantidadPreguntas;

            // Manejo de video si suben uno nuevo
            if (video != null && video.Length > 0)
            {
                var extension = Path.GetExtension(video.FileName).ToLower();
                if (extension != ".mp4")
                {
                    ModelState.AddModelError("", "Solo se permiten archivos MP4.");
                    return View(modulo);
                }

                var carpeta = Path.Combine(_environment.WebRootPath, "videos");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreUnico = Guid.NewGuid().ToString() + ".mp4";
                var rutaCompleta = Path.Combine(carpeta, nombreUnico);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }

                modulo.VideoPath = "/videos/" + nombreUnico;
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Módulo actualizado correctamente.";

            // Redirigir a la misma página de edición para continuar agregando preguntas
            return RedirectToAction("Editar", new { id = modulo.ModuloId });
        }

        [HttpPost]
        public IActionResult EditarPregunta(Pregunta model)
        {
            var pregunta = _context.Preguntas.FirstOrDefault(p => p.PreguntaId == model.PreguntaId);

            if (pregunta == null)
            {
                TempData["Error"] = "Pregunta no encontrada.";
                return RedirectToAction("Editar", new { id = model.ModuloId });
            }

            // Actualizar campos
            pregunta.Enunciado = model.Enunciado;
            pregunta.OpcionA = model.OpcionA;
            pregunta.OpcionB = model.OpcionB;
            pregunta.OpcionC = model.OpcionC;
            pregunta.OpcionD = model.OpcionD;
            pregunta.RespuestaCorrecta = model.RespuestaCorrecta;

            _context.SaveChanges();

            TempData["Mensaje"] = "Pregunta actualizada correctamente.";

            return RedirectToAction("Editar", new { id = model.ModuloId });
        }
    }
}
