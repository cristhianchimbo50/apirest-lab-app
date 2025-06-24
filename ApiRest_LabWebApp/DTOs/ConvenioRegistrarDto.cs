using System.ComponentModel.DataAnnotations;

namespace ApiRest_LabWebApp.DTOs
{
    public class ConvenioRegistrarDto
    {
        [Required]
        public int IdMedico { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public decimal Comision { get; set; } 

        [Required]
        public decimal TotalPagar { get; set; }

        [Required]
        public List<OrdenConvenioDto> Ordenes { get; set; } = new();
    }
}