namespace LMS.Models.Entidades
{
    public class EstudianteModulo
    {
        public string EstudianteCedula { get; set; }   // FK a Estudiante
        public int ModuloId { get; set; }             // FK a Modulo
        public int Puntaje { get; set; }              // Puntaje obtenido
    }
}