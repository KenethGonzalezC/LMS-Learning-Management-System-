using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entidades
{
    public class Estudiante
    {
        [Key]
        public string Cedula { get; set; } = null!;

        [Required]
        public string Nombre { get; set; } = null!;

        // FK
        public int SedeId { get; set; }

        [ForeignKey("SedeId")]
        public Sede Sede { get; set; } = null!;

        public ICollection<EstudianteCurso> EstudiantesCursos { get; set; } = new List<EstudianteCurso>();
    }
}
