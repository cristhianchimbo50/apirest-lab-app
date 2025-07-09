using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Orden
{
    public int IdOrden { get; set; }

    public string NumeroOrden { get; set; } = null!;

    public int? IdPaciente { get; set; }

    public DateOnly FechaOrden { get; set; }

    public decimal Total { get; set; }

    public decimal? SaldoPendiente { get; set; }

    public decimal? TotalPagado { get; set; }

    public string EstadoPago { get; set; } = null!;

    public bool? Anulado { get; set; }

    public bool? LiquidadoConvenio { get; set; }

    public int? IdMedico { get; set; }

    public string? Observacion { get; set; }

    public int? IdUsuario { get; set; }

    public virtual ICollection<DetalleConvenio> DetalleConvenios { get; set; } = new List<DetalleConvenio>();

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual Medico? IdMedicoNavigation { get; set; }

    public virtual Paciente? IdPacienteNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<MovimientoReactivo> MovimientoReactivos { get; set; } = new List<MovimientoReactivo>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
}
