using ApiRest_LabWebApp.Models;

namespace ApiRest_LabWebApp.DTOs
{
    public class OrdenCompletaDto
    {
        public Orden Orden { get; set; } = new();
        public List<int> IdsExamenes { get; set; } = new();
        public decimal DineroEfectivo { get; set; }
        public decimal Transferencia { get; set; }
        public string? Observaciones { get; set; }
    }
}
