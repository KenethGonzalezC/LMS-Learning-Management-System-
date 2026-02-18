using System.ComponentModel.DataAnnotations;

namespace LMS.Models.ViewModels
{
    public class EstudianteLoginViewModel
    {
        [Required]
        public string Cedula { get; set; } = null!;
    }
}
