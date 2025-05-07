using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class DetalleConveniosController : ControllerBase
{
    private readonly BdLabContext _context;

    public DetalleConveniosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleConvenio>>> GetDetalleConvenios()
    {
        return await _context.DetalleConvenios
            .Include(d => d.IdConvenioNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DetalleConvenio>> GetDetalleConvenio(int id)
    {
        var detalleConvenio = await _context.DetalleConvenios
            .Include(d => d.IdConvenioNavigation)
            .FirstOrDefaultAsync(d => d.IdDetalleConvenio == id);

        if (detalleConvenio == null)
        {
            return NotFound();
        }

        return detalleConvenio;
    }

    [HttpPost]
    public async Task<ActionResult<DetalleConvenio>> PostDetalleConvenio(DetalleConvenio detalleConvenio)
    {
        _context.DetalleConvenios.Add(detalleConvenio);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetalleConvenio), new { id = detalleConvenio.IdDetalleConvenio }, detalleConvenio);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetalleConvenio(int id, DetalleConvenio detalleConvenio)
    {
        if (id != detalleConvenio.IdDetalleConvenio)
        {
            return BadRequest();
        }

        _context.Entry(detalleConvenio).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DetalleConvenioExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetalleConvenio(int id)
    {
        var detalleConvenio = await _context.DetalleConvenios.FindAsync(id);
        if (detalleConvenio == null)
        {
            return NotFound();
        }

        _context.DetalleConvenios.Remove(detalleConvenio);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DetalleConvenioExists(int id)
    {
        return _context.DetalleConvenios.Any(e => e.IdDetalleConvenio == id);
    }
}
