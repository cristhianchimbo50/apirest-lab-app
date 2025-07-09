using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using ApiRest_LabWebApp.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Global, roles por método
public class OrdenesController : ControllerBase
{
    private readonly BdLabContext _context;

    private readonly PdfTicketService _pdfTicketService;

    public OrdenesController(BdLabContext context, PdfTicketService pdfTicketService)
    {
        _context = context;
        _pdfTicketService = pdfTicketService;
    }

    [HttpGet]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<IEnumerable<OrdenDto>>> GetOrdenes()
    {
        var ordenes = await _context.Ordens
            .Include(o => o.IdPacienteNavigation)
            .Select(o => new OrdenDto
            {
                IdOrden = o.IdOrden,
                NumeroOrden = o.NumeroOrden,
                CedulaPaciente = o.IdPacienteNavigation.CedulaPaciente,
                NombrePaciente = o.IdPacienteNavigation.NombrePaciente,
                FechaOrden = o.FechaOrden,
                Total = o.Total,
                TotalPagado = o.TotalPagado ?? 0,
                SaldoPendiente = o.SaldoPendiente ?? 0,
                EstadoPago = o.EstadoPago,
                Anulado = o.Anulado ?? false
            })
            .ToListAsync();

        return ordenes;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<Orden>> GetOrden(int id)
    {
        var orden = await _context.Ordens
            .Include(o => o.IdPacienteNavigation)
            .Include(o => o.IdMedicoNavigation)
            .Include(o => o.DetalleOrdens!)
                .ThenInclude(d => d.IdExamenNavigation)
            .Include(o => o.DetalleOrdens!)
                .ThenInclude(d => d.IdResultadoNavigation)
            .FirstOrDefaultAsync(o => o.IdOrden == id);

        if (orden == null)
        {
            return NotFound();
        }

        return orden;
    }

    [HttpPost]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<Orden>> PostOrden(Orden orden)
    {
        _context.Ordens.Add(orden);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrden), new { id = orden.IdOrden }, orden);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> PutOrden(int id, Orden orden)
    {
        if (id != orden.IdOrden)
        {
            return BadRequest();
        }

        _context.Entry(orden).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrdenExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> DeleteOrden(int id)
    {
        var orden = await _context.Ordens.FindAsync(id);
        if (orden == null)
        {
            return NotFound();
        }

        _context.Ordens.Remove(orden);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrdenExists(int id)
    {
        return _context.Ordens.Any(e => e.IdOrden == id);
    }

    [HttpGet("paciente-por-cedula/{cedula}")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<PacienteDto>> ObtenerPacientePorCedula(string cedula)
    {
        var paciente = await _context.Pacientes
            .Where(p => p.CedulaPaciente == cedula && !(p.Anulado ?? false))
            .FirstOrDefaultAsync();

        if (paciente == null)
            return NotFound();

        return new PacienteDto
        {
            IdPaciente = paciente.IdPaciente,
            CedulaPaciente = paciente.CedulaPaciente,
            NombrePaciente = paciente.NombrePaciente,
            FechaNacPaciente = DateTime.Parse(paciente.FechaNacPaciente.ToString()),
            EdadPaciente = paciente.EdadPaciente,
            DireccionPaciente = paciente.DireccionPaciente,
            CorreoElectronicoPaciente = paciente.CorreoElectronicoPaciente,
            TelefonoPaciente = paciente.TelefonoPaciente,
            FechaRegistro = paciente.FechaRegistro,
            Anulado = paciente.Anulado ?? false,
            IdUsuario = paciente.IdUsuario
        };
    }

    [HttpPost("completa")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult> RegistrarOrdenCompleta(OrdenCompletaDto dto)
    {
        if (dto == null || dto.Orden == null || dto.IdsExamenes == null || !dto.IdsExamenes.Any())
        {
            return BadRequest("Datos incompletos para registrar la orden.");
        }

        var orden = dto.Orden;

        //Calcular totales
        var examenes = await _context.Examen
            .Where(e => dto.IdsExamenes.Contains(e.IdExamen))
            .ToListAsync();

        decimal total = examenes.Sum(e => e.Precio ?? 0);
        decimal pagado = dto.DineroEfectivo + dto.Transferencia;
        decimal saldo = total - pagado;
        string estado = saldo <= 0 ? "PAGADO" : "PENDIENTE";

        orden.Total = total;
        orden.TotalPagado = pagado;
        orden.SaldoPendiente = saldo;
        orden.EstadoPago = estado;
        orden.FechaOrden = DateOnly.FromDateTime(DateTime.Today);
        orden.Anulado = false;

        _context.Ordens.Add(orden);
        await _context.SaveChangesAsync();

        //generar Número de Orden tipo ORD-00001
        orden.NumeroOrden = $"ORD-{orden.IdOrden:D5}";
        await _context.SaveChangesAsync();

        //guardar detalle de orden
        foreach (var idExamen in dto.IdsExamenes.Distinct())
        {
            var precio = examenes.FirstOrDefault(e => e.IdExamen == idExamen)?.Precio ?? 0;
            _context.DetalleOrdens.Add(new DetalleOrden
            {
                IdOrden = orden.IdOrden,
                IdExamen = idExamen,
                Precio = precio
            });
        }

        //Guardar pago
        var pago = new Pago
        {
            IdOrden = orden.IdOrden,
            FechaPago = DateTime.Now,
            MontoPagado = pagado,
            Observacion = dto.Observaciones,
            Anulado = false,
            IdUsuario = orden.IdUsuario
        };

        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();

        //Guardar detalle de pago
        if (dto.DineroEfectivo > 0)
        {
            _context.DetallePagos.Add(new DetallePago
            {
                IdPago = pago.IdPago,
                TipoPago = "EFECTIVO",
                Monto = dto.DineroEfectivo,
                IdUsuario = orden.IdUsuario
            });
        }

        if (dto.Transferencia > 0)
        {
            _context.DetallePagos.Add(new DetallePago
            {
                IdPago = pago.IdPago,
                TipoPago = "TRANSFERENCIA",
                Monto = dto.Transferencia,
                IdUsuario = orden.IdUsuario
            });
        }

        await _context.SaveChangesAsync();

        return Ok(new { orden.IdOrden, orden.NumeroOrden });
    }

    [HttpGet("detalle/{id}")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<OrdenDetalleDto>> ObtenerDetalleOrden(int id)
    {
        var orden = await _context.Ordens
            .Include(o => o.IdPacienteNavigation)
            .Include(o => o.IdMedicoNavigation)
            .Include(o => o.DetalleOrdens!)
                .ThenInclude(d => d.IdExamenNavigation)
            .Include(o => o.DetalleOrdens!)
                .ThenInclude(d => d.IdResultadoNavigation)
            .FirstOrDefaultAsync(o => o.IdOrden == id);

        if (orden == null)
            return NotFound();

        var dto = new OrdenDetalleDto
        {
            IdOrden = orden.IdOrden,
            IdPaciente = orden.IdPaciente ?? 0,
            IdMedico = orden.IdMedico,
            NombreMedico = orden.IdMedicoNavigation?.NombreMedico ?? "",
            NumeroOrden = orden.NumeroOrden,
            FechaOrden = orden.FechaOrden,
            EstadoPago = orden.EstadoPago,
            CedulaPaciente = orden.IdPacienteNavigation?.CedulaPaciente,
            NombrePaciente = orden.IdPacienteNavigation?.NombrePaciente,
            DireccionPaciente = orden.IdPacienteNavigation?.DireccionPaciente,
            CorreoPaciente = orden.IdPacienteNavigation?.CorreoElectronicoPaciente,
            TelefonoPaciente = orden.IdPacienteNavigation?.TelefonoPaciente,
            Anulado = orden.Anulado,
            TotalOrden = orden.Total,
            TotalPagado = orden.TotalPagado ?? 0,
            SaldoPendiente = orden.SaldoPendiente ?? 0,
            Examenes = orden.DetalleOrdens
                .Where(d => d.IdResultadoNavigation == null || d.IdResultadoNavigation.Anulado == false)
                .Select(d => new ExamenDetalleDto
                {
                    IdExamen = d.IdExamen ?? 0,
                    NombreExamen = d.IdExamenNavigation?.NombreExamen,
                    NombreEstudio = d.IdExamenNavigation?.Estudio,
                    IdResultado = d.IdResultado,
                    NumeroResultado = d.IdResultadoNavigation?.NumeroResultado
                }).ToList()
        };

        return dto;
    }

    [HttpPut("anular/{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> AnularOrden(int id)
    {
        var orden = await _context.Ordens
            .Include(o => o.DetalleOrdens!)
                .ThenInclude(d => d.IdResultadoNavigation)
                    .ThenInclude(r => r.DetalleResultados)
            .FirstOrDefaultAsync(o => o.IdOrden == id);

        if (orden == null) return NotFound();

        orden.Anulado = true;

        // Anular resultados asociados
        var resultados = orden.DetalleOrdens
            .Where(d => d.IdResultadoNavigation != null)
            .Select(d => d.IdResultadoNavigation!)
            .Distinct()
            .ToList();

        foreach (var resultado in resultados)
        {
            resultado.Anulado = true;

            // Anular cada detalle de resultado
            foreach (var detalle in resultado.DetalleResultados)
            {
                detalle.Anulado = true;
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/ticket-pdf")]
[Authorize(Roles = "administrador,recepcionista")]
public async Task<IActionResult> ObtenerTicketPdf(int id)
{
    var orden = await _context.Ordens
        .Include(o => o.IdPacienteNavigation)
        .Include(o => o.IdMedicoNavigation)
        .Include(o => o.DetalleOrdens!)
            .ThenInclude(d => d.IdExamenNavigation)
        .FirstOrDefaultAsync(o => o.IdOrden == id);

    if (orden == null)
        return NotFound("Orden no encontrada.");

    var ordenDto = new OrdenTicketDto
    {
        NumeroOrden = orden.NumeroOrden,
        FechaOrden = orden.FechaOrden.ToDateTime(TimeOnly.MinValue),
        NombrePaciente = orden.IdPacienteNavigation?.NombrePaciente ?? "-",
        CedulaPaciente = orden.IdPacienteNavigation?.CedulaPaciente ?? "-",
        EdadPaciente = orden.IdPacienteNavigation?.EdadPaciente ?? 0,
        NombreMedico = orden.IdMedicoNavigation?.NombreMedico ?? "-",
        Total = orden.Total,
        TotalPagado = orden.TotalPagado ?? 0,
        SaldoPendiente = orden.SaldoPendiente ?? 0,
        TipoPago = orden.EstadoPago,
        Examenes = orden.DetalleOrdens.Select(d => new ExamenTicketDto
        {
            NombreExamen = d.IdExamenNavigation?.NombreExamen ?? "-",
            Precio = d.Precio ?? 0
        }).ToList()
    };

    var pdfBytes = _pdfTicketService.GenerarTicketOrden(ordenDto);

    return File(pdfBytes, "application/pdf", $"orden_{orden.NumeroOrden}_ticket.pdf");
}

}
