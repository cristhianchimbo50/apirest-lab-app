using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class ExamenReactivo
{
    public int IdExamenReactivo { get; set; }

    public int? IdExamen { get; set; }

    public int? IdReactivo { get; set; }

    public decimal? CantidadUsada { get; set; }

    public string? Unidad { get; set; }

    public int? IdUsuario { get; set; }

    public virtual Examen? IdExamenNavigation { get; set; }

    public virtual Reactivo? IdReactivoNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}

