namespace ApiRest_LabWebApp.DTOs
{
    public class ResultadoDto
    {
        public int IdResultado { get; set; }
        public string NumeroResultado { get; set; } = "";
        public string CedulaPaciente { get; set; } = "";
        public string NombrePaciente { get; set; } = "";
        public DateTime FechaResultado { get; set; }
        public bool Anulado { get; set; }
    }
}
