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
        // INDEX - MIS CURSOS
        // ===============================
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
