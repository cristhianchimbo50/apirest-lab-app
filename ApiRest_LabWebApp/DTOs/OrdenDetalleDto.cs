namespace ApiRest_LabWebApp.DTOs
{
    public class OrdenDetalleDto
    {
        public int IdOrden { get; set; }
        public int? IdMedico { get; set; }
        public string NombreMedico { get; set; }
        public string EstadoPago { get; set; } = string.Empty;
        public string NumeroOrden { get; set; }
        public DateOnly FechaOrden { get; set; }
        public string? CedulaPaciente { get; set; }
        public string? NombrePaciente { get; set; }
        public string? DireccionPaciente { get; set; }
        public string? CorreoPaciente { get; set; }
        public string? TelefonoPaciente { get; set; }
        public bool? Anulado { get; set; }
        public int IdPaciente { get; set; }

        public List<ExamenDetalleDto> Examenes { get; set; } = new();

        public decimal TotalOrden { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
    }

    public class ExamenDetalleDto
    {
        public int IdExamen { get; set; }
        public string? NombreExamen { get; set; }
        public string? NombreEstudio { get; set; }
        public int? IdResultado { get; set; }
        public string? NumeroResultado { get; set; }
    }
}
