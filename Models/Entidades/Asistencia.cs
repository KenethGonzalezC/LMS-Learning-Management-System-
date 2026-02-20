using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entidades
{
    public class Asistencia
    {
        [Key]
        public int AsistenciaId { get; set; }

        [Required]
        public string EstudianteCedula { get; set; }

        [Required]
        public int CursoId { get; set; }

        public DateTime FechaHora { get; set; }

        // Relaciones
        [ForeignKey("EstudianteCedula")]
        public Estudiante Estudiante { get; set; }

        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }
    }
}