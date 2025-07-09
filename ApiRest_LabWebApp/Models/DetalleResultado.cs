using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class DetalleResultado
{
    public int IdDetalleResultado { get; set; }

    public int? IdResultado { get; set; }

    public int? IdExamen { get; set; }

    public decimal Valor { get; set; }

    public string? Unidad { get; set; }

    public string? Observacion { get; set; }

    public bool? Anulado { get; set; }

    public string? ValorReferencia { get; set; }


    public virtual Examen? IdExamenNavigation { get; set; }

    public virtual Resultado? IdResultadoNavigation { get; set; }
}
