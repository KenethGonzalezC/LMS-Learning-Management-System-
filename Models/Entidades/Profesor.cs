using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entidades
{
    public class Profesor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Passwd { get; set; } = null!;

        // FK
        public int SedeId { get; set; }

        [ForeignKey("SedeId")]
        public Sede Sede { get; set; } = null!;

        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}
