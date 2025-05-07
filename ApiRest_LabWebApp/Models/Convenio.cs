using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Convenio
{
    public int IdConvenio { get; set; }

    public int? IdMedico { get; set; }

    public DateOnly FechaConvenio { get; set; }

    public decimal PorcentajeComision { get; set; }

    public decimal MontoTotal { get; set; }

    public bool? Anulado { get; set; }

    public int? IdUsuario { get; set; }

    public virtual ICollection<DetalleConvenio> DetalleConvenios { get; set; } = new List<DetalleConvenio>();

    public virtual Medico? IdMedicoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
