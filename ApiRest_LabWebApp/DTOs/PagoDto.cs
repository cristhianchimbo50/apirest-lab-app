namespace ApiRest_LabWebApp.DTOs;

public class PagoDto
{
    public int? IdOrden { get; set; }
    public decimal DineroEfectivo { get; set; }
    public decimal Transferencia { get; set; }
    public string Observacion { get; set; } = string.Empty;
}

