using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DetallePagosController : ControllerBase
{
    private readonly BdLabContext _context;

    public DetallePagosController(BdLabContext context)
    {       
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<IEnumerable<DetallePago>>> GetDetallePagos()
    {
        return await _context.DetallePagos
            .Include(d => d.IdPagoNavigation)
            .Include(d => d.IdUsuarioNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<DetallePago>> GetDetallePago(int id)
    {
        var detallePago = await _context.DetallePagos
            .Include(d => d.IdPagoNavigation)
            .Include(d => d.IdUsuarioNavigation)
            .FirstOrDefaultAsync(d => d.IdDetallePago == id);

        if (detallePago == null)
        {
            return NotFound();
        }

        return detallePago;
    }

    [HttpPost]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<DetallePago>> PostDetallePago(DetallePago detallePago)
    {
        _context.DetallePagos.Add(detallePago);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDetallePago), new { id = detallePago.IdDetallePago }, detallePago);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> PutDetallePago(int id, DetallePago detallePago)
    {
        if (id != detallePago.IdDetallePago)
        {
            return BadRequest();
        }

        _context.Entry(detallePago).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DetallePagoExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> DeleteDetallePago(int id)
    {
        var detallePago = await _context.DetallePagos.FindAsync(id);
        if (detallePago == null)
        {
            return NotFound();
        }

        _context.DetallePagos.Remove(detallePago);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DetallePagoExists(int id)
    {
        return _context.DetallePagos.Any(e => e.IdDetallePago == id);
    }
}
