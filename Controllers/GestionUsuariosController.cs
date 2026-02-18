using LMS.Data;
using LMS.Models.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class GestionUsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GestionUsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // VALIDACIÓN DE ACCESO
        // ===============================
        private bool EsProfesor()
        {
            return HttpContext.Session.GetString("UsuarioTipo") == "Profesor";
        }

        // ===============================
        // INDEX (Vista principal)
        // ===============================
        public async Task<IActionResult> Index(int? sedeFiltro)
        {
            if (!EsProfesor())
                return RedirectToAction("Index", "Home");

            ModelState.Clear();

            var sedes = await _context.Sedes.ToListAsync();

            int sedeSeleccionada = sedeFiltro ??
                HttpContext.Session.GetInt32("SedeId") ?? 0;

            var profesores = await _context.Profesores
                .Where(p => p.SedeId == sedeSeleccionada)
                .ToListAsync();

            var alumnos = await _context.Estudiantes
                .Where(a => a.SedeId == sedeSeleccionada)
                .ToListAsync();

            ViewBag.Sedes = sedes;
            ViewBag.SedeSeleccionada = sedeSeleccionada;
            ViewBag.SedeActiva = HttpContext.Session.GetInt32("SedeId");

            return View(new Tuple<List<Profesor>, List<Estudiante>>(profesores, alumnos));
        }

        // ===============================
        // CREAR PROFESOR
        // ===============================
        [HttpPost]
        public async Task<IActionResult> CrearProfesor(string profesorNombre, string profesorPasswd)
        {
            if (!EsProfesor())
                return RedirectToAction("Index", "Home");

            int sedeActiva = HttpContext.Session.GetInt32("SedeId") ?? 0;

            var profesor = new Profesor
            {
                Nombre = profesorNombre,
                Passwd = profesorPasswd,
                SedeId = sedeActiva
            };

            _context.Profesores.Add(profesor);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ===============================
        // CREAR ALUMNO
        // ===============================
        [HttpPost]
        public async Task<IActionResult> CrearAlumno(string cedula, string nombre)
        {
            if (!EsProfesor())
                return RedirectToAction("Index", "Home");

            int sedeActiva = HttpContext.Session.GetInt32("SedeId") ?? 0;

            var alumno = new Estudiante
            {
                Cedula = cedula,
                Nombre = nombre,
                SedeId = sedeActiva
            };

            _context.Estudiantes.Add(alumno);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ===============================
        // ELIMINAR PROFESOR (CASCADA)
        // ===============================
        public async Task<IActionResult> EliminarProfesor(int id)
        {
            if (!EsProfesor())
                return RedirectToAction("Index", "Home");

            var profesor = await _context.Profesores
                .Include(p => p.Cursos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profesor != null)
            {
                _context.Profesores.Remove(profesor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // ===============================
        // ELIMINAR ALUMNO
        // ===============================
        public async Task<IActionResult> EliminarAlumno(string cedula)
        {
            if (!EsProfesor())
                return RedirectToAction("Index", "Home");

            var alumno = await _context.Estudiantes.FindAsync(cedula);

            if (alumno != null)
            {
                _context.Estudiantes.Remove(alumno);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
