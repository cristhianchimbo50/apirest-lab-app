using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly BdLabContext _context;

    public PagosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
    {
        return await _context.Pagos
            .Include(p => p.IdOrdenNavigation)
            .Include(p => p.IdUsuarioNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<Pago>> GetPago(int id)
    {
        var pago = await _context.Pagos
            .Include(p => p.IdOrdenNavigation)
            .Include(p => p.IdUsuarioNavigation)
            .FirstOrDefaultAsync(p => p.IdPago == id);

        if (pago == null)
        {
            return NotFound();
        }

        return pago;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> PutPago(int id, Pago pago)
    {
        if (id != pago.IdPago)
        {
            return BadRequest();
        }

        _context.Entry(pago).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PagoExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> DeletePago(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);
        if (pago == null)
        {
            return NotFound();
        }

        _context.Pagos.Remove(pago);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PagoExists(int id)
    {
        return _context.Pagos.Any(e => e.IdPago == id);
    }

    [HttpGet("orden/{idOrden}")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<IEnumerable<DetallePagoDto>>> GetPagosPorOrden(int idOrden)
    {
        var pagos = await _context.DetallePagos
            .Include(dp => dp.IdPagoNavigation)
            .Where(dp =>
                dp.IdPagoNavigation != null &&
                dp.IdPagoNavigation.IdOrden == idOrden &&
                (dp.IdPagoNavigation.Anulado == null || dp.IdPagoNavigation.Anulado == false))
            .Select(dp => new DetallePagoDto
            {
                FechaPago = dp.IdPagoNavigation.FechaPago,
                TipoPago = dp.TipoPago,
                Monto = dp.Monto
            })
            .ToListAsync();

        return Ok(pagos);
    }
    
    [HttpPost("registrar")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<IActionResult> RegistrarPago([FromBody] PagoDto dto)
    {
        var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type.EndsWith("idUsuario"));
        if (idUsuarioClaim == null)
            return Unauthorized("No se pudo determinar el usuario autenticado.");

        int idUsuario = int.Parse(idUsuarioClaim.Value);

        if (dto.IdOrden == null)
            return BadRequest("Falta el ID de la orden.");

        var orden = await _context.Ordens.FindAsync(dto.IdOrden);
        if (orden == null)
            return NotFound("Orden no encontrada.");

        var total = dto.DineroEfectivo + dto.Transferencia;

        // Crear registro principal del pago
        var pago = new Pago
        {
            IdOrden = dto.IdOrden,
            FechaPago = DateTime.Now,
            MontoPagado = total,
            Observacion = dto.Observacion,
            Anulado = false,
            IdUsuario = idUsuario
        };

        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync(); // genera IdPago

        // Crear detalles de pago
        var detalles = new List<DetallePago>();

        if (dto.DineroEfectivo > 0)
        {
            detalles.Add(new DetallePago
            {
                IdPago = pago.IdPago,
                TipoPago = "EFECTIVO",
                Monto = dto.DineroEfectivo,
                IdUsuario = idUsuario
            });
        }

        if (dto.Transferencia > 0)
        {
            detalles.Add(new DetallePago
            {
                IdPago = pago.IdPago,
                TipoPago = "TRANSFERENCIA",
                Monto = dto.Transferencia,
                IdUsuario = idUsuario
            });
        }

        _context.DetallePagos.AddRange(detalles);

        orden.TotalPagado = (orden.TotalPagado ?? 0) + total;
        orden.SaldoPendiente = (orden.Total) - orden.TotalPagado;
        orden.EstadoPago = orden.SaldoPendiente <= 0 ? "PAGADO" : "PENDIENTE";

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Pago y detalle registrados, orden actualizada correctamente." });
    }
}
