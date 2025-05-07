using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Paciente
{
    public int IdPaciente { get; set; }

    public string CedulaPaciente { get; set; } = null!;

    public string NombrePaciente { get; set; } = null!;

    public DateOnly FechaNacPaciente { get; set; }

    public int? EdadPaciente { get; set; }

    public string? DireccionPaciente { get; set; }

    public string? CorreoElectronicoPaciente { get; set; }

    public string? TelefonoPaciente { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Anulado { get; set; }

    public int? IdUsuario { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    public virtual ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
}
