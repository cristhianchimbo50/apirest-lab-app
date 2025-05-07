using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class DetalleConvenio
{
    public int IdDetalleConvenio { get; set; }

    public int? IdConvenio { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Convenio? IdConvenioNavigation { get; set; }
}
