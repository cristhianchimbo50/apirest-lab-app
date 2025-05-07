using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class DetalleOrdenesController : ControllerBase
{
    private readonly BdLabContext _context;

    public DetalleOrdenesController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleOrden>>> GetDetalleOrdenes()
    {
        return await _context.DetalleOrdens
            .Include(d => d.IdOrdenNavigation)
            .Include(d => d.IdExamenNavigation)
            .Include(d => d.IdResultadoNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DetalleOrden>> GetDetalleOrden(int id)
    {
        var detalleOrden = await _context.DetalleOrdens
            .Include(d => d.IdOrdenNavigation)
            .Include(d => d.IdExamenNavigation)
            .Include(d => d.IdResultadoNavigation)
            .FirstOrDefaultAsync(d => d.IdDetalleOrden == id);

        if (detalleOrden == null)
        {
            return NotFound();
        }

        return detalleOrden;
    }

    [HttpPost]
    public async Task<ActionResult<DetalleOrden>> PostDetalleOrden(DetalleOrden detalleOrden)
    {
        _context.DetalleOrdens.Add(detalleOrden);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetalleOrden), new { id = detalleOrden.IdDetalleOrden }, detalleOrden);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetalleOrden(int id, DetalleOrden detalleOrden)
    {
        if (id != detalleOrden.IdDetalleOrden)
        {
            return BadRequest();
        }

        _context.Entry(detalleOrden).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DetalleOrdenExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetalleOrden(int id)
    {
        var detalleOrden = await _context.DetalleOrdens.FindAsync(id);
        if (detalleOrden == null)
        {
            return NotFound();
        }

        _context.DetalleOrdens.Remove(detalleOrden);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DetalleOrdenExists(int id)
    {
        return _context.DetalleOrdens.Any(e => e.IdDetalleOrden == id);
    }
}
