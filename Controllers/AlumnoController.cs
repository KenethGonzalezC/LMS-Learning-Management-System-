using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class AlumnoController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "Estudiante")
                return RedirectToAction("EstudianteLogin", "Auth");

            return View();
        }
    }
}
