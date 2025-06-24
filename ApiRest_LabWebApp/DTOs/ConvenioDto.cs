namespace ApiRest_LabWebApp.DTOs
{
    public class ConvenioDto
    {
        public int IdConvenio { get; set; }
        public DateOnly FechaConvenio { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal PorcentajeComision { get; set; }
        public string NombreMedico { get; set; } = "";
        public bool Anulado { get; set; }
    }
}