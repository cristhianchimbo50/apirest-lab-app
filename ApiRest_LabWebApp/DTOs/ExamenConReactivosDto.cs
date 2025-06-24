using ApiRest_LabWebApp.DTOs;

namespace ApiRest_LabWebApp.Dto
{
    public class ExamenConReactivosDto
    {
        public ExamenDto Examen { get; set; } = new();
        public List<ExamenReactivoDto> Reactivos { get; set; } = new();
    }
}
