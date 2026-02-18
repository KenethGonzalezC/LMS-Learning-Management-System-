using LMS.Data;
using LMS.Models.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CursoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // VALIDACIÓN
        // ===============================
        private bool SesionValida()
        {
            return HttpContext.Session.GetInt32("ProfesorId") != null
                && HttpContext.Session.GetInt32("SedeId") != null;
        }

        // ===============================
        // INDEX - MIS CURSOS
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
    }
}
