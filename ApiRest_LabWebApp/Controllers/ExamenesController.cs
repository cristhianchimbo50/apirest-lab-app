using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class ExamenesController : ControllerBase
{
    private readonly BdLabContext _context;

    public ExamenesController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Examen>>> GetExamenes()
    {
        return await _context.Examen
            .Include(e => e.IdUsuarioNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Examen>> GetExamen(int id)
    {
        var examen = await _context.Examen
            .Include(e => e.IdUsuarioNavigation)
            .FirstOrDefaultAsync(e => e.IdExamen == id);

        if (examen == null)
        {
            return NotFound();
        }

        return examen;
    }

    [HttpPost]
    public async Task<ActionResult<Examen>> PostExamen(Examen examen)
    {
        _context.Examen.Add(examen);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExamen), new { id = examen.IdExamen }, examen);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutExamen(int id, Examen examen)
    {
        if (id != examen.IdExamen)
        {
            return BadRequest();
        }

        _context.Entry(examen).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExamenExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExamen(int id)
    {
        var examen = await _context.Examen.FindAsync(id);
        if (examen == null)
        {
            return NotFound();
        }

        _context.Examen.Remove(examen);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExamenExists(int id)
    {
        return _context.Examen.Any(e => e.IdExamen == id);
    }
}
