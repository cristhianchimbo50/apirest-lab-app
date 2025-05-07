using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class DetalleOrden
{
    public int IdDetalleOrden { get; set; }

    public int? IdOrden { get; set; }

    public int? IdExamen { get; set; }

    public decimal? Precio { get; set; }

    public int? IdResultado { get; set; }

    public virtual Examen? IdExamenNavigation { get; set; }

    public virtual Orden? IdOrdenNavigation { get; set; }

    public virtual Resultado? IdResultadoNavigation { get; set; }
}
