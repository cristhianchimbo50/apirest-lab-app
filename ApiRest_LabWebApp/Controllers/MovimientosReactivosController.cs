using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Global, pero roles finos en cada método
public class MovimientosReactivosController : ControllerBase
{
    private readonly BdLabContext _context;

    public MovimientosReactivosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<ActionResult<IEnumerable<MovimientoReactivo>>> GetMovimientosReactivos()
    {
        return await _context.MovimientoReactivos
            .Include(m => m.IdReactivoNavigation)
            .Include(m => m.IdOrdenNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<ActionResult<MovimientoReactivo>> GetMovimientoReactivo(int id)
    {
        var movimiento = await _context.MovimientoReactivos
            .Include(m => m.IdReactivoNavigation)
            .Include(m => m.IdOrdenNavigation)
            .FirstOrDefaultAsync(m => m.IdMovimientoReactivo == id);

        if (movimiento == null)
        {
            return NotFound();
        }

        return movimiento;
    }

    [HttpPost]
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<ActionResult<MovimientoReactivo>> PostMovimientoReactivo(MovimientoReactivo movimiento)
    {
        _context.MovimientoReactivos.Add(movimiento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMovimientoReactivo), new { id = movimiento.IdMovimientoReactivo }, movimiento);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> PutMovimientoReactivo(int id, MovimientoReactivo movimiento)
    {
        if (id != movimiento.IdMovimientoReactivo)
        {
            return BadRequest();
        }

        _context.Entry(movimiento).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MovimientoReactivoExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> DeleteMovimientoReactivo(int id)
    {
        var movimiento = await _context.MovimientoReactivos.FindAsync(id);
        if (movimiento == null)
        {
            return NotFound();
        }

        _context.MovimientoReactivos.Remove(movimiento);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MovimientoReactivoExists(int id)
    {
        return _context.MovimientoReactivos.Any(e => e.IdMovimientoReactivo == id);
    }

    [HttpPost("lote")]
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<IActionResult> PostMovimientosReactivoLote(List<MovimientoReactivo> movimientos)
    {
        foreach (var movimiento in movimientos)
        {
            var reactivo = await _context.Reactivos.FirstOrDefaultAsync(r => r.IdReactivo == movimiento.IdReactivo);
            if (reactivo == null) continue;

            if (movimiento.TipoMovimiento == "INGRESO")
                reactivo.CantidadDisponible += movimiento.Cantidad ?? 0;
            else if (movimiento.TipoMovimiento == "EGRESO")
                reactivo.CantidadDisponible -= movimiento.Cantidad ?? 0;

            _context.MovimientoReactivos.Add(movimiento);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("filtrar")]
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<ActionResult<IEnumerable<MovimientoReactivoView>>> FiltrarMovimientos(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] string? tipoMovimiento)
    {
        var query = _context.MovimientoReactivos
            .Include(m => m.IdReactivoNavigation)
            .Include(m => m.IdOrdenNavigation)
            .AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(m => m.FechaMovimiento >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(m => m.FechaMovimiento <= fechaFin.Value.Date.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrEmpty(tipoMovimiento))
            query = query.Where(m => m.TipoMovimiento == tipoMovimiento);

        var lista = await query
            .OrderByDescending(m => m.FechaMovimiento)
            .Select(m => new MovimientoReactivoView
            {
                Reactivo = m.IdReactivoNavigation != null ? m.IdReactivoNavigation.NombreReactivo : "(No asignado)",
                TipoMovimiento = m.TipoMovimiento ?? "",
                Cantidad = m.Cantidad ?? 0,
                FechaMovimiento = m.FechaMovimiento,
                NumeroOrden = m.IdOrdenNavigation != null ? m.IdOrdenNavigation.NumeroOrden : "",
                Observacion = m.Observacion ?? ""
            })
            .ToListAsync();

        return lista;
    }
}
