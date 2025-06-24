namespace ApiRest_LabWebApp.DTOs;

public class MovimientoReactivoView
{
    public string Reactivo { get; set; } = "";
    public string TipoMovimiento { get; set; } = "";
    public decimal Cantidad { get; set; }
    public DateTime FechaMovimiento { get; set; }
    public string NumeroOrden { get; set; } = "";
    public string Observacion { get; set; } = "";
}
