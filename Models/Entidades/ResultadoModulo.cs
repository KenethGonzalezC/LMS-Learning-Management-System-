namespace LMS.Models.Entidades
{
    public class ResultadoModulo
    {
        public int ResultadoModuloId { get; set; }
        public string EstudianteCedula { get; set; }
        public int ModuloId { get; set; }
        public decimal NotaObtenida { get; set; }
        public bool Aprobado { get; set; }
    }
}