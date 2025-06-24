namespace ApiRest_LabWebApp.DTOs;

public class ConvenioDetalleDto
{
    public int IdConvenio { get; set; }
    public DateOnly FechaConvenio { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal PorcentajeComision { get; set; }
    public string NombreMedico { get; set; } = "";
    public string NombreUsuario { get; set; } = "";
    public List<OrdenConvenioDto> Ordenes { get; set; } = new();
}
