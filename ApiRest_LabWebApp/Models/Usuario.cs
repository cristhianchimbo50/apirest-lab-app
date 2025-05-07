using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string CorreoUsuario { get; set; } = null!;

    public string ClaveUsuario { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public bool? EsContraseñaTemporal { get; set; }

    public bool? EstadoRegistro { get; set; }

    public virtual ICollection<Convenio> Convenios { get; set; } = new List<Convenio>();

    public virtual ICollection<DetallePago> DetallePagos { get; set; } = new List<DetallePago>();

    public virtual ICollection<Examen> Examen { get; set; } = new List<Examen>();

    public virtual ICollection<ExamenReactivo> ExamenReactivos { get; set; } = new List<ExamenReactivo>();

    public virtual ICollection<Medico> Medicos { get; set; } = new List<Medico>();

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    public virtual ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
