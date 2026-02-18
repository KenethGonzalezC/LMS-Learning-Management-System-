using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entidades
{
    public class EstudianteCurso
    {
        public int Id { get; set; }

        public string EstudianteCedula { get; set; }
        public Estudiante Estudiante { get; set; }

        public int CursoId { get; set; }
        public Curso Curso { get; set; }
    }
}