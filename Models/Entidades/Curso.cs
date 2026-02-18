using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LMS.Models.Entidades
{
    public class Curso
    {
        [Key]
        public int CursoId { get; set; }

        public string Nombre { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public int ProfesorId { get; set; }

        public Profesor Profesor { get; set; } = null!;

        public ICollection<Modulo> Modulos { get; set; } = new List<Modulo>();

        public ICollection<EstudianteCurso> EstudiantesCursos { get; set; } = new List<EstudianteCurso>();
        public int SedeId { get; set; }
        public Sede Sede { get; set; }

    }

}