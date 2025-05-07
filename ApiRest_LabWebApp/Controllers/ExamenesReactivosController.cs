using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class ExamenesReactivosController : ControllerBase
{
    private readonly BdLabContext _context;

    public ExamenesReactivosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExamenReactivo>>> GetExamenesReactivos()
    {
        return await _context.ExamenReactivos
            .Include(er => er.IdExamenNavigation)
            .Include(er => er.IdReactivoNavigation)
            .Include(er => er.IdUsuarioNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExamenReactivo>> GetExamenReactivo(int id)
    {
        var examenReactivo = await _context.ExamenReactivos
            .Include(er => er.IdExamenNavigation)
            .Include(er => er.IdReactivoNavigation)
            .Include(er => er.IdUsuarioNavigation)
            .FirstOrDefaultAsync(er => er.IdExamenReactivo == id);

        if (examenReactivo == null)
        {
            return NotFound();
        }

        return examenReactivo;
    }

    [HttpPost]
    public async Task<ActionResult<ExamenReactivo>> PostExamenReactivo(ExamenReactivo examenReactivo)
    {
        _context.ExamenReactivos.Add(examenReactivo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExamenReactivo), new { id = examenReactivo.IdExamenReactivo }, examenReactivo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutExamenReactivo(int id, ExamenReactivo examenReactivo)
    {
        if (id != examenReactivo.IdExamenReactivo)
        {
            return BadRequest();
        }

        _context.Entry(examenReactivo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExamenReactivoExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExamenReactivo(int id)
    {
        var examenReactivo = await _context.ExamenReactivos.FindAsync(id);
        if (examenReactivo == null)
        {
            return NotFound();
        }

        _context.ExamenReactivos.Remove(examenReactivo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExamenReactivoExists(int id)
    {
        return _context.ExamenReactivos.Any(e => e.IdExamenReactivo == id);
    }
}
