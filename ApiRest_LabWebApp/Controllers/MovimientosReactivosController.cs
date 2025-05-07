using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class MovimientosReactivosController : ControllerBase
{
    private readonly BdLabContext _context;

    public MovimientosReactivosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimientoReactivo>>> GetMovimientosReactivos()
    {
        return await _context.MovimientoReactivos
            .Include(m => m.IdReactivoNavigation)
            .Include(m => m.IdOrdenNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
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
    public async Task<ActionResult<MovimientoReactivo>> PostMovimientoReactivo(MovimientoReactivo movimiento)
    {
        _context.MovimientoReactivos.Add(movimiento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMovimientoReactivo), new { id = movimiento.IdMovimientoReactivo }, movimiento);
    }

    [HttpPut("{id}")]
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
}
