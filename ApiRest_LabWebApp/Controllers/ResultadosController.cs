using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

[Route("api/[controller]")]
[ApiController]
public class ResultadosController : ControllerBase
{
    private readonly BdLabContext _context;

    public ResultadosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Resultado>>> GetResultados()
    {
        return await _context.Resultados
            .Include(r => r.IdPacienteNavigation)
            .Include(r => r.IdMedicoNavigation)
            .Include(r => r.IdOrdenNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Resultado>> GetResultado(int id)
    {
        var resultado = await _context.Resultados
            .Include(r => r.IdPacienteNavigation)
            .Include(r => r.IdMedicoNavigation)
            .Include(r => r.IdOrdenNavigation)
            .FirstOrDefaultAsync(r => r.IdResultado == id);

        if (resultado == null)
        {
            return NotFound();
        }

        return resultado;
    }

    [HttpPost]
    public async Task<ActionResult<Resultado>> PostResultado(Resultado resultado)
    {
        _context.Resultados.Add(resultado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetResultado), new { id = resultado.IdResultado }, resultado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutResultado(int id, Resultado resultado)
    {
        if (id != resultado.IdResultado)
        {
            return BadRequest();
        }

        _context.Entry(resultado).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ResultadoExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResultado(int id)
    {
        var resultado = await _context.Resultados.FindAsync(id);
        if (resultado == null)
        {
            return NotFound();
        }

        _context.Resultados.Remove(resultado);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ResultadoExists(int id)
    {
        return _context.Resultados.Any(e => e.IdResultado == id);
    }
}
