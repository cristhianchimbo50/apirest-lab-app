using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Examen
{
    public int IdExamen { get; set; }

    public string NombreExamen { get; set; } = null!;

    public string? ValorReferencia { get; set; }

    public string? Unidad { get; set; }

    public decimal? Precio { get; set; }

    public bool? Anulado { get; set; }

    public int? IdUsuario { get; set; }

    public string? Estudio { get; set; }

    public string? TipoMuestra { get; set; }

    public string? TiempoEntrega { get; set; }

    public string? TipoExamen { get; set; }

    public string? Tecnica { get; set; }

    public string? TituloExamen { get; set; }

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual ICollection<DetalleResultado> DetalleResultados { get; set; } = new List<DetalleResultado>();

    public virtual ICollection<ExamenReactivo> ExamenReactivos { get; set; } = new List<ExamenReactivo>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<Examen> IdExamenHijos { get; set; } = new List<Examen>();

    public virtual ICollection<Examen> IdExamenPadres { get; set; } = new List<Examen>();
}
