using LMS.Models.Entidades;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models.ViewModels
{
    public class ProfesorLoginViewModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Passwd { get; set; }

        [Required]
        [Display(Name = "Sede")]
        public int SedeId { get; set; }

        public List<Sede>? Sedes { get; set; }
    }

}
