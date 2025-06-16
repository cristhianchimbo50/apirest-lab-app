namespace ApiRest_LabWebApp.DTOs
{
    public class OrdenDto
    {
        public int IdOrden { get; set; }
        public string NumeroOrden { get; set; }
        public string CedulaPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public DateOnly FechaOrden { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string EstadoPago { get; set; }
        public bool Anulado { get; set; }
    }

}
