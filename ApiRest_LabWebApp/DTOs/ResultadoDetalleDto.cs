namespace ApiRest_LabWebApp.DTOs
{
    public class ResultadoDetalleDto
    {
        public int IdResultado { get; set; }
        public string NumeroResultado { get; set; } = "";
        public string CedulaPaciente { get; set; } = "";
        public string NombrePaciente { get; set; } = "";
        public DateTime FechaResultado { get; set; }
        public bool Anulado { get; set; }

        public List<ResultadoExamenDto> Examenes { get; set; } = new();
    }

    public class ResultadoExamenDto
    {
        public int IdExamen { get; set; }
        public string NombreExamen { get; set; } = "";
        public decimal Valor { get; set; }
        public string? Unidad { get; set; }
        public string? Observacion { get; set; }
        public string ValorReferencia { get; set; } = "";
        public string? TituloExamen { get; set; }
    }
}
