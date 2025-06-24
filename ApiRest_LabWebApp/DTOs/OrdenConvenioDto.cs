namespace ApiRest_LabWebApp.DTOs;

public class OrdenConvenioDto
{
    public int IdOrden { get; set; }
    public string NumeroOrden { get; set; } = "";
    public string Paciente { get; set; } = "";
    public DateOnly? FechaOrden { get; set; }
    public decimal Total { get; set; }
    public string EstadoPago { get; set; } = "";
}
