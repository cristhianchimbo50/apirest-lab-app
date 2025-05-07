namespace ApiRest_LabWebApp.DTOs
{
    public class PacienteDto
    {
        public int IdPaciente { get; set; }
        public string CedulaPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public DateTime FechaNacPaciente { get; set; }
        public int? EdadPaciente { get; set; }
        public string DireccionPaciente { get; set; }
        public string CorreoElectronicoPaciente { get; set; }
        public string TelefonoPaciente { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public bool Anulado { get; set; }
        public int? IdUsuario { get; set; }
    }
}
