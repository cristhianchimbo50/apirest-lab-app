using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class DetallePago
{
    public int IdDetallePago { get; set; }

    public int? IdPago { get; set; }

    public string? TipoPago { get; set; }

    public decimal? Monto { get; set; }

    public int? IdUsuario { get; set; }

    public DateTime? FechaAnulacion { get; set; }

    public virtual Pago? IdPagoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
