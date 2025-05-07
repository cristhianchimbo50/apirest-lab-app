using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class DetalleResultadosController : ControllerBase
{
    private readonly BdLabContext _context;

    public DetalleResultadosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleResultado>>> GetDetalleResultados()
    {
        return await _context.DetalleResultados
            .Include(dr => dr.IdResultadoNavigation)
            .Include(dr => dr.IdExamenNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DetalleResultado>> GetDetalleResultado(int id)
    {
        var detalleResultado = await _context.DetalleResultados
            .Include(dr => dr.IdResultadoNavigation)
            .Include(dr => dr.IdExamenNavigation)
            .FirstOrDefaultAsync(dr => dr.IdDetalleResultado == id);

        if (detalleResultado == null)
        {
            return NotFound();
        }

        return detalleResultado;
    }

    [HttpPost]
    public async Task<ActionResult<DetalleResultado>> PostDetalleResultado(DetalleResultado detalleResultado)
    {
        _context.DetalleResultados.Add(detalleResultado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetalleResultado), new { id = detalleResultado.IdDetalleResultado }, detalleResultado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetalleResultado(int id, DetalleResultado detalleResultado)
    {
        if (id != detalleResultado.IdDetalleResultado)
        {
            return BadRequest();
        }

        _context.Entry(detalleResultado).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DetalleResultadoExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetalleResultado(int id)
    {
        var detalleResultado = await _context.DetalleResultados.FindAsync(id);
        if (detalleResultado == null)
        {
            return NotFound();
        }

        _context.DetalleResultados.Remove(detalleResultado);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DetalleResultadoExists(int id)
    {
        return _context.DetalleResultados.Any(e => e.IdDetalleResultado == id);
    }
}
