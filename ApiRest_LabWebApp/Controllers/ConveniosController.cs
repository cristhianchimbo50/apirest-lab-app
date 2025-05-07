using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class ConveniosController : ControllerBase
{
    private readonly BdLabContext _context;

    public ConveniosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Convenio>>> GetConvenios()
    {
        return await _context.Convenios
            .Include(c => c.IdMedicoNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Convenio>> GetConvenio(int id)
    {
        var convenio = await _context.Convenios
            .Include(c => c.IdMedicoNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .FirstOrDefaultAsync(c => c.IdConvenio == id);

        if (convenio == null)
        {
            return NotFound();
        }

        return convenio;
    }

    [HttpPost]
    public async Task<ActionResult<Convenio>> PostConvenio(Convenio convenio)
    {
        _context.Convenios.Add(convenio);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetConvenio), new { id = convenio.IdConvenio }, convenio);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutConvenio(int id, Convenio convenio)
    {
        if (id != convenio.IdConvenio)
        {
            return BadRequest();
        }

        _context.Entry(convenio).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ConvenioExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConvenio(int id)
    {
        var convenio = await _context.Convenios.FindAsync(id);
        if (convenio == null)
        {
            return NotFound();
        }

        _context.Convenios.Remove(convenio);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ConvenioExists(int id)
    {
        return _context.Convenios.Any(e => e.IdConvenio == id);
    }
}
