using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Resultado
{
    public int IdResultado { get; set; }

    public string NumeroResultado { get; set; } = null!;

    public int? IdPaciente { get; set; }

    public int? IdMedico { get; set; }

    public DateTime FechaResultado { get; set; }

    public string? Observaciones { get; set; }

    public int? IdOrden { get; set; }

    public bool? Anulado { get; set; }

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual ICollection<DetalleResultado> DetalleResultados { get; set; } = new List<DetalleResultado>();

    public virtual Medico? IdMedicoNavigation { get; set; }

    public virtual Orden? IdOrdenNavigation { get; set; }

    public virtual Paciente? IdPacienteNavigation { get; set; }
}
