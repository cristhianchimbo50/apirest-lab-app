using ApiRest_LabWebApp.Dto;

namespace ApiRest_LabWebApp.DTOs
{
    public class ExamenConComposicionDto
    {
        public ExamenDto Examen { get; set; } = new();
        public List<int> IdExamenesHijos { get; set; } = new();
    }
}
