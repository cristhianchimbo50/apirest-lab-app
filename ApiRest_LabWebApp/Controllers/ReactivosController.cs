using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;

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

    [HttpGet("filtrar")]
    public async Task<ActionResult<IEnumerable<ReactivoDto>>> FiltrarReactivos(
        [FromQuery] string? nombre,
        [FromQuery] string? fabricante,
        [FromQuery] string? unidad)
    {
        var query = _context.Reactivos.AsQueryable();

        query = query.Where(r => !(r.Anulado ?? false)); // excluir anulados

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(r => r.NombreReactivo.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(fabricante))
            query = query.Where(r => r.Fabricante.Contains(fabricante));

        if (!string.IsNullOrWhiteSpace(unidad))
            query = query.Where(r => r.Unidad.Contains(unidad));

        var resultado = await query
            .OrderBy(r => r.NombreReactivo)
            .Select(r => new ReactivoDto
            {
                IdReactivo = r.IdReactivo,
                NombreReactivo = r.NombreReactivo,
                Fabricante = r.Fabricante,
                Unidad = r.Unidad,
                CantidadDisponible = r.CantidadDisponible ?? 0,
                Anulado = r.Anulado
            })
            .ToListAsync();

        return Ok(resultado);
    }


    [HttpPost]
    public async Task<ActionResult<Reactivo>> PostReactivo(Reactivo reactivo)
    {
        _context.Reactivos.Add(reactivo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReactivo), new { id = reactivo.IdReactivo }, reactivo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutReactivo(int id, [FromBody] ReactivoDto dto)
{
    var existente = await _context.Reactivos.FindAsync(id);
    if (existente == null)
        return NotFound();

    existente.NombreReactivo = dto.NombreReactivo.Trim().ToUpper();
    existente.Fabricante = dto.Fabricante.Trim().ToUpper();
    existente.Unidad = dto.Unidad.Trim();

    await _context.SaveChangesAsync();

    return Ok(new { mensaje = "Reactivo actualizado correctamente" });
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

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarReactivo([FromBody] ReactivoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NombreReactivo) ||
            string.IsNullOrWhiteSpace(dto.Fabricante) ||
            string.IsNullOrWhiteSpace(dto.Unidad))
        {
            return BadRequest("Todos los campos son obligatorios.");
        }

        var nuevo = new Reactivo
        {
            NombreReactivo = dto.NombreReactivo.Trim().ToUpper(),
            Fabricante = dto.Fabricante.Trim().ToUpper(),
            Unidad = dto.Unidad.Trim(),
            CantidadDisponible = 0,
            Anulado = false
        };

        _context.Reactivos.Add(nuevo);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Reactivo registrado", nuevo.IdReactivo });
    }
    
    [HttpPut("anular/{id}")]
    public async Task<IActionResult> AnularReactivo(int id)
    {
        var reactivo = await _context.Reactivos.FindAsync(id);
        if (reactivo == null)
            return NotFound();

        reactivo.Anulado = true;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Reactivo anulado correctamente." });
    }


}
