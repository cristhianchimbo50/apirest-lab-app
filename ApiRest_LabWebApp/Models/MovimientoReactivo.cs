using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class MovimientoReactivo
{
    public int IdMovimientoReactivo { get; set; }

    public int? IdReactivo { get; set; }

    public string? TipoMovimiento { get; set; }

    public decimal? Cantidad { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public int? IdOrden { get; set; }

    public string? Observacion { get; set; }

    public virtual Orden? IdOrdenNavigation { get; set; }

    public virtual Reactivo? IdReactivoNavigation { get; set; }
}
