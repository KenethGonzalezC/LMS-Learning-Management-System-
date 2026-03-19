using System.ComponentModel.DataAnnotations;

namespace LMS.Models.Entidades
{
    public class EntregaClase
    {
        public int EntregaClaseId { get; set; }

        public int ClaseId { get; set; }
        public Clase Clase { get; set; }

        public string EstudianteCedula { get; set; }
        public Estudiante Estudiante { get; set; }

        // 🔹 Entrega del estudiante
        public string? Texto { get; set; }
        public string? ImagenPath { get; set; }

        // 🔹 Revisión del profesor
        public string? ComentarioProfesor { get; set; }

        public DateTime FechaEntrega { get; set; } = DateTime.Now;
    }
}