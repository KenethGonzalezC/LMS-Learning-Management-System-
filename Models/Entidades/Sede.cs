using System.ComponentModel.DataAnnotations;

namespace LMS.Models.Entidades
{
    public class Sede
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
        public ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}
