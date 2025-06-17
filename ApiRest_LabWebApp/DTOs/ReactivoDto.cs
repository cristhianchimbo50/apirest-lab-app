namespace ApiRest_LabWebApp.DTOs
{
    public class ReactivoDto
    {
        public int IdReactivo { get; set; }
        public string? NombreReactivo { get; set; }
        public string? Fabricante { get; set; }
        public string? Unidad { get; set; }
        public decimal CantidadDisponible { get; set; }
        public bool? Anulado { get; set; }
    }
}
