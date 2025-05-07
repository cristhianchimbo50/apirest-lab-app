using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;

[Route("api/[controller]")]
[ApiController]
public class PacientesController : ControllerBase
{
    private readonly BdLabContext _context;

    public PacientesController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PacienteDto>>> GetPacientes()
    {
        var pacientes = await _context.Pacientes
            .Select(p => new PacienteDto
            {
                IdPaciente = p.IdPaciente,
                CedulaPaciente = p.CedulaPaciente,
                NombrePaciente = p.NombrePaciente,
                FechaNacPaciente = DateTime.Parse(p.FechaNacPaciente.ToString()),
                EdadPaciente = p.EdadPaciente,
                DireccionPaciente = p.DireccionPaciente,
                CorreoElectronicoPaciente = p.CorreoElectronicoPaciente,
                TelefonoPaciente = p.TelefonoPaciente,
                FechaRegistro = p.FechaRegistro.HasValue ? DateTime.Parse(p.FechaRegistro.Value.ToString()) : null,
                Anulado = p.Anulado ?? false, // Fix for CS0266 and CS8629
                IdUsuario = p.IdUsuario
            })
            .ToListAsync();

        return pacientes;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PacienteDto>> GetPaciente(int id)
    {
        var p = await _context.Pacientes.FindAsync(id);

        if (p == null)
        {
            return NotFound();
        }

        var dto = new PacienteDto
        {
            IdPaciente = p.IdPaciente,
            CedulaPaciente = p.CedulaPaciente,
            NombrePaciente = p.NombrePaciente,
            FechaNacPaciente = DateTime.Parse(p.FechaNacPaciente.ToString()),
            EdadPaciente = p.EdadPaciente,
            DireccionPaciente = p.DireccionPaciente,
            CorreoElectronicoPaciente = p.CorreoElectronicoPaciente,
            TelefonoPaciente = p.TelefonoPaciente,
            FechaRegistro = p.FechaRegistro.HasValue ? DateTime.Parse(p.FechaRegistro.Value.ToString()) : null,
            Anulado = p.Anulado ?? false, // Fix for CS0266 and CS8629
            IdUsuario = p.IdUsuario
        };

        return dto;
    }

    [HttpPost]
    public async Task<ActionResult<PacienteDto>> PostPaciente(PacienteDto dto)
    {
        var paciente = new Paciente
        {
            CedulaPaciente = dto.CedulaPaciente,
            NombrePaciente = dto.NombrePaciente,

            FechaNacPaciente = DateOnly.FromDateTime(dto.FechaNacPaciente),
            EdadPaciente = dto.EdadPaciente,
            DireccionPaciente = dto.DireccionPaciente,
            CorreoElectronicoPaciente = dto.CorreoElectronicoPaciente,
            TelefonoPaciente = dto.TelefonoPaciente,
            FechaRegistro = dto.FechaRegistro,
            Anulado = dto.Anulado,
            IdUsuario = dto.IdUsuario
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        dto.IdPaciente = paciente.IdPaciente;
        return CreatedAtAction(nameof(GetPaciente), new { id = dto.IdPaciente }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPaciente(int id, PacienteDto dto)
    {
        if (id != dto.IdPaciente)
        {
            return BadRequest();
        }

        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
        {
            return NotFound();
        }

        paciente.CedulaPaciente = dto.CedulaPaciente;
        paciente.NombrePaciente = dto.NombrePaciente;
        paciente.FechaNacPaciente = DateOnly.FromDateTime(dto.FechaNacPaciente);
        paciente.EdadPaciente = dto.EdadPaciente;
        paciente.DireccionPaciente = dto.DireccionPaciente;
        paciente.CorreoElectronicoPaciente = dto.CorreoElectronicoPaciente;
        paciente.TelefonoPaciente = dto.TelefonoPaciente;
        paciente.Anulado = dto.Anulado;
        paciente.IdUsuario = dto.IdUsuario;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PacienteExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
        {
            return NotFound();
        }

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PacienteExists(int id)
    {
        return _context.Pacientes.Any(e => e.IdPaciente == id);
    }
}
