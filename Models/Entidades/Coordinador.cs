using System.ComponentModel.DataAnnotations;

namespace LMS.Models.Entidades
{
    public class Coordinador
    {
        [Key]
        public int CoordinadorId { get; set; }

        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Password { get; set; }
    }
}