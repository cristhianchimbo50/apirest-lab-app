using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int? IdOrden { get; set; }

    public DateTime? FechaPago { get; set; }

    public decimal? MontoPagado { get; set; }

    public string? Observacion { get; set; }

    public bool? Anulado { get; set; }

    public int? IdUsuario { get; set; }

    public virtual ICollection<DetallePago> DetallePagos { get; set; } = new List<DetallePago>();

    public virtual Orden? IdOrdenNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
