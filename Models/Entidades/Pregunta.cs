namespace LMS.Models.Entidades
{
    public class Pregunta
    {
        public int PreguntaId { get; set; }

        public string Enunciado { get; set; }

        public string OpcionA { get; set; }
        public string OpcionB { get; set; }
        public string OpcionC { get; set; }
        public string OpcionD { get; set; }
        public string RespuestaCorrecta { get; set; }

        public int ModuloId { get; set; }
        public Modulo Modulo { get; set; }
    }
}