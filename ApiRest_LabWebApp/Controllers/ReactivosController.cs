using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class ReactivosController : ControllerBase
{
    private readonly BdLabContext _context;

    public ReactivosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reactivo>>> GetReactivos()
    {
        return await _context.Reactivos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reactivo>> GetReactivo(int id)
    {
        var reactivo = await _context.Reactivos.FindAsync(id);

        if (reactivo == null)
        {
            return NotFound();
        }

        return reactivo;
    }

    [HttpPost]
    public async Task<ActionResult<Reactivo>> PostReactivo(Reactivo reactivo)
    {
        _context.Reactivos.Add(reactivo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReactivo), new { id = reactivo.IdReactivo }, reactivo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutReactivo(int id, Reactivo reactivo)
    {
        if (id != reactivo.IdReactivo)
        {
            return BadRequest();
        }

        _context.Entry(reactivo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReactivoExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReactivo(int id)
    {
        var reactivo = await _context.Reactivos.FindAsync(id);
        if (reactivo == null)
        {
            return NotFound();
        }

        _context.Reactivos.Remove(reactivo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ReactivoExists(int id)
    {
        return _context.Reactivos.Any(e => e.IdReactivo == id);
    }
}
