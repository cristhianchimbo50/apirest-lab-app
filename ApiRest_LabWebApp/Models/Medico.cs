using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Medico
{
    public int IdMedico { get; set; }

    public string NombreMedico { get; set; } = null!;

    public string Especialidad { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public bool? Anulado { get; set; }

    public int? IdUsuario { get; set; }

    public virtual ICollection<Convenio> Convenios { get; set; } = new List<Convenio>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    public virtual ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
}
