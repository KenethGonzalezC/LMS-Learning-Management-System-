using System.ComponentModel.DataAnnotations;

namespace LMS.Models.Entidades
{
    public class Modulo
    {
        public int ModuloId { get; set; }
        [Required]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string VideoPath { get; set; }
        public string ContenidoTexto { get; set; }
        public decimal NotaMinima { get; set; }
        public int Orden { get; set; }
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }

        [Required]
        [Range(3, 10)]
        public int CantidadPreguntas { get; set; }
        public ICollection<Pregunta>? Preguntas { get; set; }
    }
}