using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Global, pero roles específicos por método
public class MedicosController : ControllerBase
{
    private readonly BdLabContext _context;

    public MedicosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<IEnumerable<MedicoDto>>> GetMedicos()
    {
        var medicos = await _context.Medicos
            .Select(m => new MedicoDto
            {
                IdMedico = m.IdMedico,
                NombreMedico = m.NombreMedico,
                Especialidad = m.Especialidad,
                Telefono = m.Telefono,
                Correo = m.Correo
            }).ToListAsync();

        return Ok(medicos);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<Medico>> GetMedico(int id)
    {
        var medico = await _context.Medicos
            .Include(m => m.IdUsuarioNavigation)
            .FirstOrDefaultAsync(m => m.IdMedico == id);

        if (medico == null)
        {
            return NotFound();
        }

        return medico;
    }

    [HttpPost]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<Medico>> PostMedico(Medico medico)
    {
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMedico), new { id = medico.IdMedico }, medico);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<IActionResult> PutMedico(int id, Medico medico)
    {
        if (id != medico.IdMedico)
        {
            return BadRequest();
        }

        _context.Entry(medico).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MedicoExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> DeleteMedico(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);
        if (medico == null)
        {
            return NotFound();
        }

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MedicoExists(int id)
    {
        return _context.Medicos.Any(e => e.IdMedico == id);
    }

    [HttpGet("buscar")]
    [Authorize(Roles = "administrador,recepcionista")]
    public async Task<ActionResult<IEnumerable<MedicoDto>>> Buscar(string criterio, string valor)
    {
        var query = _context.Medicos.AsQueryable();

        valor = valor.ToLower();

        query = criterio switch
        {
            "nombre" => query.Where(m => m.NombreMedico.ToLower().Contains(valor)),
            "especialidad" => query.Where(m => m.Especialidad.ToLower().Contains(valor)),
            "telefono" => query.Where(m => m.Telefono.Contains(valor)),
            "correo" => query.Where(m => m.Correo.ToLower().Contains(valor)),
            _ => query
        };

        var resultado = await query
            .Select(m => new MedicoDto
            {
                IdMedico = m.IdMedico,
                NombreMedico = m.NombreMedico,
                Especialidad = m.Especialidad,
                Telefono = m.Telefono,
                Correo = m.Correo,
                Anulado = m.Anulado
            }).ToListAsync();

        return Ok(resultado);
    }

    [HttpPut("anular/{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> AnularMedico(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);
        if (medico == null)
            return NotFound();

        medico.Anulado = true;
        await _context.SaveChangesAsync();

        return Ok();
    }
}
