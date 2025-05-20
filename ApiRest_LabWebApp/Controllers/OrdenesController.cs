using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class OrdenesController : ControllerBase
{
    private readonly BdLabContext _context;

    public OrdenesController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Orden>>> GetOrdenes()
    {
        return await _context.Ordens
            .Include(o => o.IdPacienteNavigation)
            .Include(o => o.IdMedicoNavigation)
            .Include(o => o.IdUsuarioNavigation)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Orden>> GetOrden(int id)
    {
        var orden = await _context.Ordens
            .Include(o => o.IdPacienteNavigation)
            .Include(o => o.IdMedicoNavigation)
            .Include(o => o.IdUsuarioNavigation)
            .FirstOrDefaultAsync(o => o.IdOrden == id);

        if (orden == null)
        {
            return NotFound();
        }

        return orden;
    }

    [HttpPost]
    public async Task<ActionResult<Orden>> PostOrden(Orden orden)
    {
        _context.Ordens.Add(orden);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrden), new { id = orden.IdOrden }, orden);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrden(int id, Orden orden)
    {
        if (id != orden.IdOrden)
        {
            return BadRequest();
        }

        _context.Entry(orden).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrdenExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrden(int id)
    {
        var orden = await _context.Ordens.FindAsync(id);
        if (orden == null)
        {
            return NotFound();
        }

        _context.Ordens.Remove(orden);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrdenExists(int id)
    {
        return _context.Ordens.Any(e => e.IdOrden == id);
    }

    [HttpGet("paciente-por-cedula/{cedula}")]
    [Authorize]
    public async Task<ActionResult<PacienteDto>> ObtenerPacientePorCedula(string cedula)
    {
        var paciente = await _context.Pacientes
            .Where(p => p.CedulaPaciente == cedula && !(p.Anulado ?? false))
            .FirstOrDefaultAsync();

        if (paciente == null)
            return NotFound();

        return new PacienteDto
        {
            IdPaciente = paciente.IdPaciente,
            CedulaPaciente = paciente.CedulaPaciente,
            NombrePaciente = paciente.NombrePaciente,
            FechaNacPaciente = DateTime.Parse(paciente.FechaNacPaciente.ToString()),
            EdadPaciente = paciente.EdadPaciente,
            DireccionPaciente = paciente.DireccionPaciente,
            CorreoElectronicoPaciente = paciente.CorreoElectronicoPaciente,
            TelefonoPaciente = paciente.TelefonoPaciente,
            FechaRegistro = paciente.FechaRegistro,
            Anulado = paciente.Anulado ?? false,
            IdUsuario = paciente.IdUsuario
        };
    }

}
