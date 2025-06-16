namespace ApiRest_LabWebApp.DTOs
{
    public class ResultadoGuardarDto
    {
        public int IdOrden { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime? FechaResultado { get; set; }
        public string? ObservacionesGenerales { get; set; }
        public List<ResultadoExamenDto> Examenes { get; set; } = new();
    }

}

