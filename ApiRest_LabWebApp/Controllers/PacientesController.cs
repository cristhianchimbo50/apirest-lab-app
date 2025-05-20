using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly BdLabContext _context;

    public PacientesController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
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
                Anulado = p.Anulado ?? false,
                IdUsuario = p.IdUsuario
            })
            .ToListAsync();

        return pacientes;
    }

    [HttpGet("{id}")]
    [Authorize]
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


    [HttpPut("{id}")]
    [Authorize]
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
    [Authorize]
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

    [HttpGet("buscar")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PacienteDto>>> BuscarPacientes([FromQuery] string campo, [FromQuery] string valor)
    {
        if (string.IsNullOrWhiteSpace(campo) || string.IsNullOrWhiteSpace(valor))
        {
            // Retorna todos si no hay filtro
            return await GetPacientes();
        }

        valor = valor.ToLower();

        var query = _context.Pacientes.AsQueryable();

        query = campo.ToLower() switch
        {
            "cedula" => query.Where(p => p.CedulaPaciente.ToLower().Contains(valor)),
            "nombre" => query.Where(p => p.NombrePaciente.ToLower().Contains(valor)),
            "edad" => int.TryParse(valor, out var edad) ? query.Where(p => p.EdadPaciente == edad) : query.Where(p => false),
            "direccion" => query.Where(p => p.DireccionPaciente.ToLower().Contains(valor)),
            "correo" => query.Where(p => p.CorreoElectronicoPaciente.ToLower().Contains(valor)),
            "telefono" => query.Where(p => p.TelefonoPaciente.ToLower().Contains(valor)),
            _ => query.Where(p => false)
        };

        var pacientes = await query
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
                FechaRegistro = p.FechaRegistro,
                Anulado = p.Anulado ?? false,
                IdUsuario = p.IdUsuario
            })
            .ToListAsync();

        return pacientes;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PacienteDto>> PostPaciente(PacienteDto dto)
    {
        var correo = User.Identity?.Name?.Trim().ToLower();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.CorreoUsuario.ToLower() == correo);

        if (usuario == null)
            return Unauthorized("Usuario no válido.");

        var paciente = new Paciente
        {
            CedulaPaciente = dto.CedulaPaciente,
            NombrePaciente = dto.NombrePaciente,
            FechaNacPaciente = DateOnly.FromDateTime(dto.FechaNacPaciente),
            EdadPaciente = dto.EdadPaciente,
            DireccionPaciente = dto.DireccionPaciente,
            CorreoElectronicoPaciente = dto.CorreoElectronicoPaciente,
            TelefonoPaciente = dto.TelefonoPaciente,
            FechaRegistro = DateTime.Now,
            Anulado = false,
            IdUsuario = usuario.IdUsuario
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        dto.IdPaciente = paciente.IdPaciente;
        return CreatedAtAction(nameof(GetPaciente), new { id = dto.IdPaciente }, dto);
    }

    [HttpPut("anular/{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> AnularPaciente(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
            return NotFound();

        paciente.Anulado = true;
        await _context.SaveChangesAsync();

        return Ok("Paciente anulado.");
    }


}
