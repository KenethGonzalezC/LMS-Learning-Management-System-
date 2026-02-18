using LMS.Data;
using LMS.Models;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // GET Profesor Login
        // ===============================
        [HttpGet]
        public IActionResult ProfesorLogin()
        {
            var model = new ProfesorLoginViewModel
            {
                Sedes = _context.Sedes.ToList()
            };

            return View(model);
        }

        // ===============================
        // POST Profesor Login
        // ===============================
        [HttpPost]
        public async Task<IActionResult> ProfesorLogin(ProfesorLoginViewModel model)
        {
            model.Sedes = await _context.Sedes.ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            var profesor = await _context.Profesores
                .FirstOrDefaultAsync(p =>
                    p.Nombre == model.Nombre &&
                    p.Passwd == model.Passwd);

            if (profesor == null)
            {
                ViewBag.Error = "Nombre o contraseña incorrectos.";
                return View(model);
            }

            // Validar sede
            if (profesor.SedeId != model.SedeId)
            {
                ViewBag.Error = "No pertenece a la sede seleccionada.";
                return View(model);
            }

            // Obtener sede para mostrar nombre
            var sede = await _context.Sedes
                .FirstOrDefaultAsync(s => s.Id == model.SedeId);

            // Guardar sesión
            HttpContext.Session.SetInt32("ProfesorId", profesor.Id); 
            HttpContext.Session.SetString("UsuarioNombre", profesor.Nombre);
            HttpContext.Session.SetString("UsuarioTipo", "Profesor");
            HttpContext.Session.SetInt32("SedeId", sede.Id);               // útil para consultas
            HttpContext.Session.SetString("SedeNombre", sede.Nombre);     // útil para mostrar

            return RedirectToAction("Index", "Profesor");
        }

        // ===============================
        // GET Estudiante Login
        // ===============================
        [HttpGet]
        public IActionResult EstudianteLogin()
        {
            return View();
        }

        // ===============================
        // POST Estudiante Login
        // ===============================
        [HttpPost]
        public async Task<IActionResult> EstudianteLogin(EstudianteLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.Cedula == model.Cedula);

            if (estudiante == null)
            {
                ViewBag.Error = "Estudiante no encontrado.";
                return View(model);
            }

            HttpContext.Session.SetString("UsuarioCedula", estudiante.Cedula);
            HttpContext.Session.SetString("UsuarioTipo", "Estudiante");

            return RedirectToAction("Index", "Alumno");
        }

        // ===============================
        // Logout
        // ===============================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

