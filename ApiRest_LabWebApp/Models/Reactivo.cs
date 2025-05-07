using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Reactivo
{
    public int IdReactivo { get; set; }

    public string NombreReactivo { get; set; } = null!;

    public string? Fabricante { get; set; }

    public string? Unidad { get; set; }

    public bool? Anulado { get; set; }

    public decimal? CantidadDisponible { get; set; }

    public virtual ICollection<ExamenReactivo> ExamenReactivos { get; set; } = new List<ExamenReactivo>();

    public virtual ICollection<MovimientoReactivo> MovimientoReactivos { get; set; } = new List<MovimientoReactivo>();
}
