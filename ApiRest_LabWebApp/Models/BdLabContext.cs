using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiRest_LabWebApp.Models;

public partial class BdLabContext : DbContext
{
    public BdLabContext(DbContextOptions<BdLabContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Convenio> Convenios { get; set; }

    public virtual DbSet<DetalleConvenio> DetalleConvenios { get; set; }

    public virtual DbSet<DetalleOrden> DetalleOrdens { get; set; }

    public virtual DbSet<DetallePago> DetallePagos { get; set; }

    public virtual DbSet<DetalleResultado> DetalleResultados { get; set; }

    public virtual DbSet<Examen> Examen { get; set; }

    public virtual DbSet<ExamenComposicion> ExamenComposiciones { get; set; }

    public virtual DbSet<ExamenReactivo> ExamenReactivos { get; set; }

    public virtual DbSet<Medico> Medicos { get; set; }

    public virtual DbSet<MovimientoReactivo> MovimientoReactivos { get; set; }

    public virtual DbSet<Orden> Ordens { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Reactivo> Reactivos { get; set; }

    public virtual DbSet<Resultado> Resultados { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Convenio>(entity =>
        {
            entity.HasKey(e => e.IdConvenio).HasName("PK__convenio__177BD43EA106A132");

            entity.ToTable("convenio");

            entity.Property(e => e.IdConvenio).HasColumnName("id_convenio");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.FechaConvenio).HasColumnName("fecha_convenio");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_total");
            entity.Property(e => e.PorcentajeComision)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentaje_comision");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Convenios)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__convenio__id_med__52593CB8");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Convenios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__convenio__id_usu__5441852A");
        });

        modelBuilder.Entity<DetalleConvenio>(entity =>
        {
            entity.HasKey(e => e.IdDetalleConvenio).HasName("PK__detalle___8AD8083EE71E99F4");

            entity.ToTable("detalle_convenio");

            entity.Property(e => e.IdDetalleConvenio).HasColumnName("id_detalle_convenio");
            entity.Property(e => e.IdConvenio).HasColumnName("id_convenio");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdConvenioNavigation).WithMany(p => p.DetalleConvenios)
                .HasForeignKey(d => d.IdConvenio)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__detalle_c__id_co__5EBF139D");

            entity.HasOne(d => d.Orden)
                .WithMany(p => p.DetalleConvenios)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_detalle_convenio_orden");
        });


        modelBuilder.Entity<DetalleOrden>(entity =>
        {
            entity.HasKey(e => e.IdDetalleOrden).HasName("PK__detalle___D2FC3FD781609604");

            entity.ToTable("detalle_orden");

            entity.Property(e => e.IdDetalleOrden).HasColumnName("id_detalle_orden");
            entity.Property(e => e.IdExamen).HasColumnName("id_examen");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdResultado).HasColumnName("id_resultado");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");

            entity.HasOne(d => d.IdExamenNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdExamen)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__detalle_o__id_ex__0D7A0286");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__detalle_o__id_or__0C85DE4D");

            entity.HasOne(d => d.IdResultadoNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdResultado)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__detalle_o__id_re__0E6E26BF");
        });

        modelBuilder.Entity<DetallePago>(entity =>
        {
            entity.HasKey(e => e.IdDetallePago).HasName("PK__detalle___55C3EFACDE063256");

            entity.ToTable("detalle_pago");

            entity.Property(e => e.IdDetallePago).HasColumnName("id_detalle_pago");
            entity.Property(e => e.FechaAnulacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_anulacion");
            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.TipoPago)
                .HasMaxLength(50)
                .HasColumnName("tipo_pago");

            entity.HasOne(d => d.IdPagoNavigation).WithMany(p => p.DetallePagos)
                .HasForeignKey(d => d.IdPago)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__detalle_p__id_pa__00200768");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.DetallePagos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__detalle_p__id_us__01142BA1");
        });

        modelBuilder.Entity<DetalleResultado>(entity =>
        {
            entity.HasKey(e => e.IdDetalleResultado).HasName("PK__detalle___E4307FE10B18D938");

            entity.ToTable("detalle_resultado");

            entity.HasIndex(e => e.IdExamen, "idx_detalle_resultado_examen");

            entity.Property(e => e.IdDetalleResultado).HasColumnName("id_detalle_resultado");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.IdExamen).HasColumnName("id_examen");
            entity.Property(e => e.IdResultado).HasColumnName("id_resultado");
            entity.Property(e => e.Observacion)
                .HasMaxLength(255)
                .HasColumnName("observacion");
            entity.Property(e => e.Unidad)
                .HasMaxLength(50)
                .HasColumnName("unidad");
            entity.Property(e => e.Valor)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("valor");

            entity.HasOne(d => d.IdExamenNavigation).WithMany(p => p.DetalleResultados)
                .HasForeignKey(d => d.IdExamen)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__detalle_r__id_ex__1CBC4616");

            entity.HasOne(d => d.IdResultadoNavigation).WithMany(p => p.DetalleResultados)
                .HasForeignKey(d => d.IdResultado)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__detalle_r__id_re__1BC821DD");
        });

        modelBuilder.Entity<Examen>(entity =>
        {
            entity.HasKey(e => e.IdExamen).HasName("PK__examen__D16A231D8ED13AFD");

            entity.ToTable("examen");

            entity.Property(e => e.IdExamen).HasColumnName("id_examen");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.Estudio)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("estudio");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreExamen)
                .HasMaxLength(100)
                .HasColumnName("nombre_examen");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Tecnica)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tecnica");
            entity.Property(e => e.TiempoEntrega)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tiempo_entrega");
            entity.Property(e => e.TipoExamen)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tipo_examen");
            entity.Property(e => e.TipoMuestra)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tipo_muestra");
            entity.Property(e => e.TituloExamen)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("titulo_examen");
            entity.Property(e => e.Unidad)
                .HasMaxLength(50)
                .HasColumnName("unidad");
            entity.Property(e => e.ValorReferencia)
                .HasMaxLength(100)
                .HasColumnName("valor_referencia");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Examen)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__examen__id_usuar__5812160E");

            
        });

        modelBuilder.Entity<ExamenComposicion>(entity =>
        {
            entity.HasKey(e => new { e.IdExamenPadre, e.IdExamenHijo }).HasName("PK_examen_composicion");

            entity.ToTable("examen_composicion");

            entity.Property(e => e.IdExamenPadre).HasColumnName("id_examen_padre");
            entity.Property(e => e.IdExamenHijo).HasColumnName("id_examen_hijo");

            entity.HasOne(d => d.ExamenPadre)
                .WithMany()
                .HasForeignKey(d => d.IdExamenPadre)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_examen_composicion_padre");

            entity.HasOne(d => d.ExamenHijo)
                .WithMany()
                .HasForeignKey(d => d.IdExamenHijo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_examen_composicion_hijo");
        });

        modelBuilder.Entity<ExamenReactivo>(entity =>
        {
            entity.HasKey(e => e.IdExamenReactivo).HasName("PK__examen_r__ECE10F430B823FC7");

            entity.ToTable("examen_reactivo");

            entity.Property(e => e.IdExamenReactivo).HasColumnName("id_examen_reactivo");
            entity.Property(e => e.CantidadUsada)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cantidad_usada");
            entity.Property(e => e.IdExamen).HasColumnName("id_examen");
            entity.Property(e => e.IdReactivo).HasColumnName("id_reactivo");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Unidad)
                .HasMaxLength(50)
                .HasColumnName("unidad");

            entity.HasOne(d => d.IdExamenNavigation).WithMany(p => p.ExamenReactivos)
                .HasForeignKey(d => d.IdExamen)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__examen_re__id_ex__07C12930");

            entity.HasOne(d => d.IdReactivoNavigation).WithMany(p => p.ExamenReactivos)
                .HasForeignKey(d => d.IdReactivo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__examen_re__id_re__08B54D69");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ExamenReactivos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__examen_re__id_us__09A971A2");
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.IdMedico).HasName("PK__medico__E038EB43167721F0");

            entity.ToTable("medico");

            entity.HasIndex(e => e.Correo, "UQ__medico__2A586E0B86B58ECE").IsUnique();

            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.Especialidad)
                .HasMaxLength(100)
                .HasColumnName("especialidad");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreMedico)
                .HasMaxLength(100)
                .HasColumnName("nombre_medico");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Medicos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__medico__id_usuar__3F466844");
        });

        modelBuilder.Entity<MovimientoReactivo>(entity =>
        {
            entity.HasKey(e => e.IdMovimientoReactivo).HasName("PK__movimien__6CA263B2FDAE2E0C");

            entity.ToTable("movimiento_reactivo");

            entity.Property(e => e.IdMovimientoReactivo).HasColumnName("id_movimiento_reactivo");
            entity.Property(e => e.Cantidad)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cantidad");
            entity.Property(e => e.FechaMovimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdReactivo).HasColumnName("id_reactivo");
            entity.Property(e => e.Observacion)
                .HasMaxLength(255)
                .HasColumnName("observacion");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(50)
                .HasColumnName("tipo_movimiento");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.MovimientoReactivos)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__movimient__id_or__04E4BC85");

            entity.HasOne(d => d.IdReactivoNavigation).WithMany(p => p.MovimientoReactivos)
                .HasForeignKey(d => d.IdReactivo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__movimient__id_re__03F0984C");
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PK__orden__DD5B8F3315A23942");

            entity.ToTable("orden");

            entity.HasIndex(e => e.NumeroOrden, "UQ__orden__3706711555B8ACCE").IsUnique();

            entity.HasIndex(e => e.EstadoPago, "idx_orden_estado_pago");

            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.EstadoPago)
                .HasMaxLength(20)
                .HasColumnName("estado_pago");
            entity.Property(e => e.FechaOrden).HasColumnName("fecha_orden");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.LiquidadoConvenio)
                .HasDefaultValue(false)
                .HasColumnName("liquidado_convenio");
            entity.Property(e => e.NumeroOrden)
                .HasMaxLength(50)
                .HasColumnName("numero_orden");
            entity.Property(e => e.Observacion)
                .HasMaxLength(255)
                .HasColumnName("observacion");
            entity.Property(e => e.SaldoPendiente)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("saldo_pendiente");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");
            entity.Property(e => e.TotalPagado)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_pagado");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__orden__id_medico__4E88ABD4");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__orden__id_pacien__48CFD27E");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__orden__id_usuari__4F7CD00D");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.IdPaciente).HasName("PK__paciente__2C2C72BB0EBF26E8");

            entity.ToTable("paciente");

            entity.HasIndex(e => e.CedulaPaciente, "UQ__paciente__4DE187B18A0DA1EB").IsUnique();

            entity.HasIndex(e => e.NombrePaciente, "idx_paciente_nombre");

            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.CedulaPaciente)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("cedula_paciente");
            entity.Property(e => e.CorreoElectronicoPaciente)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo_electronico_paciente");
            entity.Property(e => e.DireccionPaciente)
                .HasMaxLength(150)
                .HasColumnName("direccion_paciente");
            entity.Property(e => e.EdadPaciente).HasColumnName("edad_paciente");
            entity.Property(e => e.FechaNacPaciente).HasColumnName("fecha_nac_paciente");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombrePaciente)
                .HasMaxLength(100)
                .HasColumnName("nombre_paciente");
            entity.Property(e => e.TelefonoPaciente)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono_paciente");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pacientes)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__paciente__id_usu__44FF419A");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__pago__0941B074D5DF9BAD");

            entity.ToTable("pago");

            entity.HasIndex(e => e.FechaPago, "idx_pago_fecha");

            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.MontoPagado)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_pagado");
            entity.Property(e => e.Observacion)
                .HasMaxLength(255)
                .HasColumnName("observacion");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__pago__id_orden__619B8048");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__pago__id_usuario__6477ECF3");
        });

        modelBuilder.Entity<Reactivo>(entity =>
        {
            entity.HasKey(e => e.IdReactivo).HasName("PK__reactivo__EC691887291B11E4");

            entity.ToTable("reactivo");

            entity.Property(e => e.IdReactivo).HasColumnName("id_reactivo");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.CantidadDisponible)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cantidad_disponible");
            entity.Property(e => e.Fabricante)
                .HasMaxLength(100)
                .HasColumnName("fabricante");
            entity.Property(e => e.NombreReactivo)
                .HasMaxLength(100)
                .HasColumnName("nombre_reactivo");
            entity.Property(e => e.Unidad)
                .HasMaxLength(50)
                .HasColumnName("unidad");
        });

        modelBuilder.Entity<Resultado>(entity =>
        {
            entity.HasKey(e => e.IdResultado).HasName("PK__resultad__33A42B30A58D4867");

            entity.ToTable("resultado");

            entity.Property(e => e.IdResultado).HasColumnName("id_resultado");
            entity.Property(e => e.Anulado)
                .HasDefaultValue(false)
                .HasColumnName("anulado");
            entity.Property(e => e.FechaResultado)
                .HasColumnType("datetime")
                .HasColumnName("fecha_resultado");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.NumeroResultado)
                .HasMaxLength(50)
                .HasColumnName("numero_resultado");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(255)
                .HasColumnName("observaciones");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Resultados)
                .HasForeignKey(d => d.IdMedico)
                .HasConstraintName("FK__resultado__id_me__7B5B524B");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Resultados)
                .HasForeignKey(d => d.IdOrden)
                .HasConstraintName("FK__resultado__id_or__7C4F7684");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Resultados)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("FK__resultado__id_pa__7A672E12");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuario__4E3E04AD8F3B8033");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.CorreoUsuario, "UQ__usuario__CD54AB1C29B4F762").IsUnique();

            entity.HasIndex(e => e.CorreoUsuario, "idx_usuario_correo");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.ClaveUsuario)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("clave_usuario");
            entity.Property(e => e.CorreoUsuario)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo_usuario");
            entity.Property(e => e.EsContraseñaTemporal)
                .HasDefaultValue(true)
                .HasColumnName("es_contraseña_temporal");
            entity.Property(e => e.EstadoRegistro)
                .HasDefaultValue(false)
                .HasColumnName("estado_registro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
