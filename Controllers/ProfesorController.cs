using LMS.Data;
using LMS.Models.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class ProfesorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfesorController(ApplicationDbContext context)
        {
            _context = context;
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
        // INDEX - LISTA DE CURSOS
        // ===============================
        public async Task<IActionResult> Index()
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            int profesorId = HttpContext.Session.GetInt32("ProfesorId")!.Value;

            var cursos = await _context.Cursos
                .Where(c => c.ProfesorId == profesorId)
                .Include(c => c.Modulos)
                .Include(c => c.EstudiantesCursos)
                .ToListAsync();

            return View(cursos);
        }

        // ===============================
        // CREAR CURSO
        // ===============================
        [HttpPost]
        public async Task<IActionResult> CrearCurso(string nombre, string descripcion)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            int profesorId = HttpContext.Session.GetInt32("ProfesorId")!.Value;
            int sedeId = HttpContext.Session.GetInt32("SedeId")!.Value;

            var curso = new Curso
            {
                Nombre = nombre,
                Descripcion = descripcion,
                ProfesorId = profesorId,
                SedeId = sedeId
            };

            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Curso creado correctamente.";

            return RedirectToAction("Index");
        }

        // ===============================
        // DETALLE CURSO
        // ===============================
        public async Task<IActionResult> Detalle(int id)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            int profesorId = HttpContext.Session.GetInt32("ProfesorId")!.Value;
            int sedeId = HttpContext.Session.GetInt32("SedeId")!.Value;

            var curso = await _context.Cursos
                .Include(c => c.Modulos)
                .Include(c => c.EstudiantesCursos)
                    .ThenInclude(ec => ec.Estudiante)
                .FirstOrDefaultAsync(c =>
                    c.CursoId == id &&
                    c.ProfesorId == profesorId &&
                    c.SedeId == sedeId);

            if (curso == null)
                return RedirectToAction("Index");

            return View(curso);
        }

        // ===============================
        // ELIMINAR CURSO (FÍSICO TOTAL)
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Eliminar(int cursoId)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            int profesorId = HttpContext.Session.GetInt32("ProfesorId")!.Value;
            int sedeId = HttpContext.Session.GetInt32("SedeId")!.Value;

            var curso = await _context.Cursos
                .Include(c => c.Modulos)
                    .ThenInclude(m => m.Preguntas)
                .FirstOrDefaultAsync(c =>
                    c.CursoId == cursoId &&
                    c.ProfesorId == profesorId &&
                    c.SedeId == sedeId);

            if (curso == null)
                return RedirectToAction("Index");

            // 1️⃣ Progreso módulos
            var modulosIds = curso.Modulos.Select(m => m.ModuloId).ToList();

            var progreso = await _context.EstudiantesModulos
                .Where(em => modulosIds.Contains(em.ModuloId))
                .ToListAsync();

            _context.EstudiantesModulos.RemoveRange(progreso);

            // 2️⃣ Matrículas
            var matriculas = await _context.EstudiantesCursos
                .Where(ec => ec.CursoId == cursoId)
                .ToListAsync();

            _context.EstudiantesCursos.RemoveRange(matriculas);

            // 3️⃣ Preguntas
            foreach (var modulo in curso.Modulos)
            {
                _context.Preguntas.RemoveRange(modulo.Preguntas);
            }

            // 4️⃣ Módulos
            _context.Modulos.RemoveRange(curso.Modulos);

            // 5️⃣ Curso
            _context.Cursos.Remove(curso);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Curso '{curso.Nombre}' eliminado correctamente.";

            return RedirectToAction("Index");
        }
    }
}
