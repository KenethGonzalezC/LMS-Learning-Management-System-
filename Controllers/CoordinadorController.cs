using LMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoordinadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string sede = "Todos")
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "Coordinador")
                return RedirectToAction("CoordinadorLogin", "Auth");

            var cursos = _context.Cursos
                .Include(c => c.Modulos)
                .Include(c => c.Sede)
                .AsQueryable();

            if (sede != "Todos")
            {
                cursos = cursos.Where(c => c.Sede.Nombre == sede);
            }

            ViewBag.SedeSeleccionada = sede;

            return View(cursos.ToList());
        }

        public IActionResult VerCurso(int id)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "Coordinador")
                return RedirectToAction("CoordinadorLogin", "Auth");

            var curso = _context.Cursos
                .Include(c => c.Sede)
                .Include(c => c.Modulos)
                    .ThenInclude(m => m.Preguntas)
                .FirstOrDefault(c => c.CursoId == id);

            if (curso == null)
                return NotFound();

            return View(curso);
        }

        public IActionResult VerModulo(int id)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "Coordinador")
                return RedirectToAction("CoordinadorLogin", "Auth");

            var modulo = _context.Modulos
                .Include(m => m.Preguntas)
                .FirstOrDefault(m => m.ModuloId == id);

            if (modulo == null)
                return NotFound();

            return View(modulo);
        }

        // GET: /Coordinador/ResultadosModulo/5
        public IActionResult ResultadosModulo(int moduloId)
        {
            var modulo = _context.Modulos
                .Include(m => m.Curso)
                .Include(m => m.Resultados)
                .ThenInclude(r => r.Estudiante) // si agregaste la relación
                .FirstOrDefault(m => m.ModuloId == moduloId);

            if (modulo == null)
                return NotFound();

            return View(modulo); // la vista recibirá el Modulo con los resultados
        }
    }
}