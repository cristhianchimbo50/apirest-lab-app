namespace ApiRest_LabWebApp.DTOs
{
    public class OrdenDto
    {
        public int IdOrden { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string CedulaPaciente { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public DateTime FechaOrden { get; set; }
        public decimal Total { get; set; }
        public decimal SaldoPendiente { get; set; }
        public decimal TotalPagado { get; set; }
        public string EstadoPago { get; set; } = string.Empty;
        public bool Anulado { get; set; }
    }
}
