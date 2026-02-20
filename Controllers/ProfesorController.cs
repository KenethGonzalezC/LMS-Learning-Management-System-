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

            var curso = _context.Cursos
                .Include(c => c.Modulos)
                    .ThenInclude(m => m.Preguntas) // para que modulo.Preguntas no sea null
                .Include(c => c.EstudiantesCursos)
                    .ThenInclude(ec => ec.Estudiante)
                .FirstOrDefault(c => c.CursoId == id && c.ProfesorId == profesorId);

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

        // ===============================
        // AGREGAR ESTUDIANTE
        // ===============================
        [HttpPost]
        public async Task<IActionResult> AgregarEstudiante(int cursoId, string cedula)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            int profesorId = HttpContext.Session.GetInt32("ProfesorId")!.Value;
            int sedeId = HttpContext.Session.GetInt32("SedeId")!.Value;

            var curso = await _context.Cursos
                .Include(c => c.EstudiantesCursos)
                .FirstOrDefaultAsync(c => c.CursoId == cursoId && c.ProfesorId == profesorId && c.SedeId == sedeId);

            if (curso == null)
            {
                TempData["Error"] = "Curso no encontrado.";
                return RedirectToAction("Index");
            }

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.Cedula == cedula && e.SedeId == sedeId);

            if (estudiante == null)
            {
                TempData["Error"] = "Estudiante no existe en esta sede.";
                return RedirectToAction("Detalle", new { id = cursoId });
            }

            if (!curso.EstudiantesCursos.Any(ec => ec.EstudianteCedula == cedula))
            {
                var relacion = new EstudianteCurso
                {
                    CursoId = cursoId,
                    EstudianteCedula = cedula
                };

                _context.EstudiantesCursos.Add(relacion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"Estudiante '{estudiante.Nombre}' agregado al curso.";
            }

            return RedirectToAction("Detalle", new { id = cursoId });
        }

        // ===============================
        // QUITAR ESTUDIANTE
        // ===============================
        [HttpPost]
        public async Task<IActionResult> QuitarEstudiante(int cursoId, string cedula)
        {
            if (!SesionValida())
                return RedirectToAction("ProfesorLogin", "Auth");

            var relacion = await _context.EstudiantesCursos
                .FirstOrDefaultAsync(ec => ec.CursoId == cursoId && ec.EstudianteCedula == cedula);

            if (relacion != null)
            {
                _context.EstudiantesCursos.Remove(relacion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = $"Estudiante eliminado del curso.";
            }

            return RedirectToAction("Detalle", new { id = cursoId });
        }

        //Asistencia
        public IActionResult Asistencia(int cursoId)
        {
            var curso = _context.Cursos
                .Include(c => c.EstudiantesCursos)
                    .ThenInclude(ec => ec.Estudiante)
                .FirstOrDefault(c => c.CursoId == cursoId);

            if (curso == null)
                return NotFound();

            return View(curso);
        }
        public IActionResult AsistenciaDetalle(int cursoId, string cedula)
        {
            var estudiante = _context.Estudiantes
                .FirstOrDefault(e => e.Cedula == cedula);

            if (estudiante == null)
                return NotFound();

            var asistencias = _context.Asistencias
                .Where(a => a.CursoId == cursoId && a.EstudianteCedula == cedula)
                .OrderByDescending(a => a.FechaHora)
                .ToList();

            ViewBag.CursoId = cursoId;
            ViewBag.NombreCurso = _context.Cursos
                .Where(c => c.CursoId == cursoId)
                .Select(c => c.Nombre)
                .FirstOrDefault();

            return View((estudiante, asistencias));
        }

        //resultados
        public IActionResult ResultadosModulo(int moduloId)
        {
            // Validar sesión de profesor
            if (HttpContext.Session.GetString("UsuarioTipo") != "Profesor")
                return RedirectToAction("ProfesorLogin", "Auth");

            // Obtener módulo con su curso
            var modulo = _context.Modulos
                .Include(m => m.Curso)
                .FirstOrDefault(m => m.ModuloId == moduloId);

            if (modulo == null)
                return NotFound();

            // Traer resultados de todos los estudiantes para ese módulo
            var resultados = _context.ResultadosModulos
                .Include(r => r.Estudiante)
                .Where(r => r.ModuloId == moduloId)
                .ToList();

            ViewBag.Modulo = modulo;
            ViewBag.Curso = modulo.Curso;

            return View(resultados); // enviaremos la lista a la vista
        }
    }
}
