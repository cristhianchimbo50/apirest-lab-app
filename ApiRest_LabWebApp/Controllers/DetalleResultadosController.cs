using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DetalleResultadosController : ControllerBase
{
    private readonly BdLabContext _context;

    public DetalleResultadosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "administrador,laboratorista,recepcionista")]
    public async Task<ActionResult<IEnumerable<DetalleResultado>>> GetDetalleResultados()
    {
        return await _context.DetalleResultados
            .Include(dr => dr.IdResultadoNavigation)
            .Include(dr => dr.IdExamenNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "administrador,laboratorista,recepcionista")]
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
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<ActionResult<DetalleResultado>> PostDetalleResultado(DetalleResultado detalleResultado)
    {
        _context.DetalleResultados.Add(detalleResultado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetalleResultado), new { id = detalleResultado.IdDetalleResultado }, detalleResultado);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "administrador,laboratorista")]
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
    [Authorize(Roles = "administrador")]
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
