namespace LMS.Models.Entidades
{
    public class ResultadoModulo
    {
        public int ResultadoModuloId { get; set; }
        public string EstudianteCedula { get; set; }
        public int ModuloId { get; set; }
        public decimal NotaObtenida { get; set; }
        public bool Aprobado { get; set; }
        // Navegation properties
        public Estudiante Estudiante { get; set; } = null!;
        public Modulo Modulo { get; set; } = null!;
    }
}