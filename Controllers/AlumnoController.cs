using LMS.Data;
using LMS.Models.Entidades;
using LMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CertificadoService _certificadoService;

        public AlumnoController(ApplicationDbContext context, CertificadoService certificadoService)
        {
            _context = context;
            _certificadoService = certificadoService;
        }

        // =====================================
        // MIS CURSOS
        // =====================================
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "Estudiante")
                return RedirectToAction("EstudianteLogin", "Auth");

            var cedula = HttpContext.Session.GetString("UsuarioCedula");

            var cursos = _context.EstudiantesCursos
                .Where(ec => ec.EstudianteCedula == cedula)
                .Include(ec => ec.Curso)
                .Select(ec => ec.Curso)
                .ToList();

            return View(cursos);
        }

        // =====================================
        // VER CURSO (CON MÓDULOS Y DESBLOQUEO)
        // =====================================
        public IActionResult VerCurso(int cursoId)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "Estudiante")
                return RedirectToAction("EstudianteLogin", "Auth");

            var cedula = HttpContext.Session.GetString("UsuarioCedula");

            var curso = _context.Cursos
                .Include(c => c.Modulos)
                .FirstOrDefault(c => c.CursoId == cursoId);

            if (curso == null)
                return NotFound();

            var pertenece = _context.EstudiantesCursos
                .Any(ec => ec.CursoId == cursoId && ec.EstudianteCedula == cedula);

            if (!pertenece)
                return RedirectToAction("Index");

            // Registrar asistencia
            var yaRegistradoHoy = _context.Asistencias.Any(a =>
                a.CursoId == cursoId &&
                a.EstudianteCedula == cedula &&
                a.FechaHora.Date == DateTime.Today);

            if (!yaRegistradoHoy)
            {
                var asistencia = new Asistencia
                {
                    CursoId = cursoId,
                    EstudianteCedula = cedula,
                    FechaHora = DateTime.Now
                };

                _context.Asistencias.Add(asistencia);
                _context.SaveChanges();
            }

            var puntajes = _context.EstudiantesModulos
                .Where(em => em.EstudianteCedula == cedula)
                .ToList();

            ViewBag.Puntajes = puntajes;

            return View(curso);
        }

        [HttpPost]
        public IActionResult Resolver(int moduloId, Dictionary<int, string> respuestas)
        {
            var cedula = HttpContext.Session.GetString("UsuarioCedula");
            if (string.IsNullOrEmpty(cedula))
                return RedirectToAction("EstudianteLogin", "Auth");

            var modulo = _context.Modulos
                .Include(m => m.Preguntas)
                .FirstOrDefault(m => m.ModuloId == moduloId);

            if (modulo == null)
                return NotFound();

            //puntaje
            int correctas = 0;

            foreach (var pregunta in modulo.Preguntas)
            {
                if (respuestas.ContainsKey(pregunta.PreguntaId) &&
                    respuestas[pregunta.PreguntaId] == pregunta.RespuestaCorrecta)
                {
                    correctas++;
                }
            }

            int total = modulo.Preguntas.Count;
            int puntaje = total > 0
                ? (int)Math.Round((double)correctas / total * 100)
                : 0;

            // Guardar en EstudianteModulo
            var registro = _context.EstudiantesModulos
                .FirstOrDefault(em => em.EstudianteCedula == cedula
                                   && em.ModuloId == moduloId);

            if (registro == null)
            {
                registro = new EstudianteModulo
                {
                    EstudianteCedula = cedula,
                    ModuloId = moduloId,
                    Puntaje = puntaje
                };
                _context.EstudiantesModulos.Add(registro);
            }
            else
            {
                // 🔥 Solo actualizar si mejora
                if (puntaje > registro.Puntaje)
                {
                    registro.Puntaje = puntaje;
                }
            }

            // 🔹 Guardar en ResultadoModulo para Coordinador
            var resultado = _context.ResultadosModulos
                .FirstOrDefault(r => r.EstudianteCedula == cedula && r.ModuloId == moduloId);

            bool aprobado = puntaje >= modulo.NotaMinima;

            if (resultado == null)
            {
                resultado = new ResultadoModulo
                {
                    EstudianteCedula = cedula,
                    ModuloId = moduloId,
                    NotaObtenida = puntaje,
                    Aprobado = aprobado
                };
                _context.ResultadosModulos.Add(resultado);
            }
            else if (puntaje > resultado.NotaObtenida)
            {
                resultado.NotaObtenida = puntaje;
                resultado.Aprobado = aprobado;
            }

            _context.SaveChanges();

            TempData["Mensaje"] = $"Obtuviste {puntaje}%";

            return RedirectToAction("VerCurso", new { cursoId = modulo.CursoId });
        }

        public IActionResult VerModulo(int moduloId)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "Estudiante")
                return RedirectToAction("EstudianteLogin", "Auth");

            var cedula = HttpContext.Session.GetString("UsuarioCedula");

            var modulo = _context.Modulos
                .Include(m => m.Preguntas)
                .Include(m => m.Curso)
                .ThenInclude(c => c.Modulos)
                .FirstOrDefault(m => m.ModuloId == moduloId);

            if (modulo == null)
                return NotFound();

            var puntajes = _context.EstudiantesModulos
                .Where(em => em.EstudianteCedula == cedula)
                .ToList();

            // 🔒 Validar desbloqueo
            var modulosOrdenados = modulo.Curso.Modulos
                .OrderBy(m => m.Orden)
                .ToList();

            var indexActual = modulosOrdenados.FindIndex(m => m.ModuloId == moduloId);

            bool desbloqueado = false;

            if (indexActual == 0)
            {
                desbloqueado = true;
            }
            else
            {
                var anterior = modulosOrdenados[indexActual - 1];

                var registroAnterior = puntajes
                    .FirstOrDefault(p => p.ModuloId == anterior.ModuloId);

                if (registroAnterior != null &&
                    registroAnterior.Puntaje >= anterior.NotaMinima)
                {
                    desbloqueado = true;
                }
            }

            if (!desbloqueado)
            {
                TempData["Error"] = "Este módulo está bloqueado.";
                return RedirectToAction("VerCurso", new { cursoId = modulo.CursoId });
            }

            ViewBag.Puntaje = puntajes
                .FirstOrDefault(p => p.ModuloId == moduloId);

            return View(modulo);
        }

        public IActionResult DescargarCertificado(int cursoId)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != "Estudiante")
                return RedirectToAction("EstudianteLogin", "Auth");

            var estudianteCedula = HttpContext.Session.GetString("UsuarioCedula");

            var estudiante = _context.Estudiantes
                .FirstOrDefault(e => e.Cedula == estudianteCedula);

            var curso = _context.Cursos
                .Include(c => c.Modulos)
                .FirstOrDefault(c => c.CursoId == cursoId);

            if (estudiante == null || curso == null)
                return NotFound();

            var registrosEstudiante = _context.EstudiantesModulos
                .Where(em => em.EstudianteCedula == estudianteCedula &&
                             curso.Modulos.Select(m => m.ModuloId).Contains(em.ModuloId))
                .ToList();

            bool cursoAprobado = curso.Modulos.All(m =>
            {
                var registro = registrosEstudiante
                    .FirstOrDefault(r => r.ModuloId == m.ModuloId);

                return registro != null && registro.Puntaje >= m.NotaMinima;
            });

            if (!cursoAprobado)
            {
                TempData["Error"] = "Debes aprobar todos los módulos para descargar el certificado.";
                return RedirectToAction("VerCurso", new { cursoId = cursoId });
            }

            var pdf = _certificadoService.GenerarCertificado(
                estudiante.Nombre,
                curso.Nombre
            );

            return File(pdf, "application/pdf", $"Certificado_{curso.Nombre}.pdf");
        }
    }
}
