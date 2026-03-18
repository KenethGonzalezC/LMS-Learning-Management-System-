using System.ComponentModel.DataAnnotations;

namespace LMS.Models.Entidades
{
    public class Clase
    {
        public int ClaseId { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string ContenidoTexto { get; set; }

        public string? VideoPath { get; set; }

        // 🔹 Tipo de entrega
        public bool RequiereTexto { get; set; }
        public bool RequiereImagen { get; set; }

        // 🔹 Orden dentro del curso
        public int Orden { get; set; }

        // 🔹 Relación con curso
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
    }
}