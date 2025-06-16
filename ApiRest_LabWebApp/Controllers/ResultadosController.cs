using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;

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
    public async Task<ActionResult<IEnumerable<ResultadoDto>>> GetResultados()
    {
        var resultados = await _context.Resultados
            .Include(r => r.IdPacienteNavigation)
            .Select(r => new ResultadoDto
            {
                IdResultado = r.IdResultado,
                NumeroResultado = r.NumeroResultado,
                CedulaPaciente = r.IdPacienteNavigation!.CedulaPaciente,
                NombrePaciente = r.IdPacienteNavigation.NombrePaciente,
                FechaResultado = r.FechaResultado,
                Anulado = r.Anulado ?? false
            })
            .OrderByDescending(r => r.IdResultado)
            .ToListAsync();

        return resultados;
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

        resultado.NumeroResultado = $"RES-{resultado.IdResultado:D5}";
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerDetalleResultado), new { id = resultado.IdResultado }, resultado);
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

    [HttpGet("detalle/{id}")]
    public async Task<ActionResult<ResultadoDetalleDto>> ObtenerDetalleResultado(int id)
    {
        var resultado = await _context.Resultados
            .Include(r => r.IdPacienteNavigation)
            .Include(r => r.DetalleResultados)
                .ThenInclude(d => d.IdExamenNavigation)
            .FirstOrDefaultAsync(r => r.IdResultado == id);

        if (resultado == null)
            return NotFound();

        var dto = new ResultadoDetalleDto
        {
            IdResultado = resultado.IdResultado,
            NumeroResultado = resultado.NumeroResultado,
            FechaResultado = resultado.FechaResultado,
            CedulaPaciente = resultado.IdPacienteNavigation?.CedulaPaciente ?? "",
            NombrePaciente = resultado.IdPacienteNavigation?.NombrePaciente ?? "",
            Anulado = resultado.Anulado ?? false,
            Examenes = resultado.DetalleResultados.Select(dr => new ResultadoExamenDto
            {
                NombreExamen = dr.IdExamenNavigation?.NombreExamen ?? "",
                Valor = dr.Valor,
                Unidad = dr.Unidad,
                Observacion = dr.Observacion
            }).ToList()
        };

        return dto;
    }


    [HttpPost("guardar-completo")]
    public async Task<IActionResult> GuardarResultadoCompleto([FromBody] ResultadoGuardarDto dto)
    {
        using var trans = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Crear resultado con un número temporal (NO NULL)
            var nuevoResultado = new Resultado
            {
                IdPaciente = dto.IdPaciente,
                IdOrden = dto.IdOrden,
                IdMedico = dto.IdMedico,
                Observaciones = dto.ObservacionesGenerales,
                FechaResultado = dto.FechaResultado ?? DateTime.Now,
                NumeroResultado = "TEMP",
                Anulado = false
            };

            _context.Resultados.Add(nuevoResultado);
            await _context.SaveChangesAsync();

            // 2. Asignar el número final con ID autogenerado
            nuevoResultado.NumeroResultado = $"RES-{nuevoResultado.IdResultado:D5}";
            await _context.SaveChangesAsync();

            // 3. Crear detalles
            foreach (var ex in dto.Examenes)
            {
                var detalle = new DetalleResultado
                {
                    IdResultado = nuevoResultado.IdResultado,
                    IdExamen = ex.IdExamen,
                    Valor = ex.Valor,
                    Unidad = ex.Unidad,
                    Observacion = ex.Observacion,
                    Anulado = false
                };
                _context.DetalleResultados.Add(detalle);
            }

            // 4. Obtener IdExamenPadre (opcionalmente puedes pasarlo desde el DTO)
            var examenPadreId = await _context.ExamenComposiciones
                .Where(ec => dto.Examenes.Select(e => e.IdExamen).Contains(ec.IdExamenHijo))
                .Select(ec => ec.IdExamenPadre)
                .FirstOrDefaultAsync();

            // 5. Guardar id_resultado en detalle_orden
            var detalleOrden = await _context.DetalleOrdens
                .FirstOrDefaultAsync(d => d.IdOrden == dto.IdOrden && d.IdExamen == examenPadreId);

            if (detalleOrden != null)
            {
                detalleOrden.IdResultado = nuevoResultado.IdResultado;
                await _context.SaveChangesAsync();
            }

            await trans.CommitAsync();

            return Ok(new
            {
                mensaje = "Resultado guardado exitosamente",
                resultadoId = nuevoResultado.IdResultado,
                numeroResultado = nuevoResultado.NumeroResultado
            });
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            var inner = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new
            {
                mensaje = "Error al guardar resultado",
                error = inner
            });
        }
    }

    [HttpPut("anular/{id}")]
    public async Task<IActionResult> AnularResultado(int id)
    {
        var resultado = await _context.Resultados.FindAsync(id);
        if (resultado == null) return NotFound();

        resultado.Anulado = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }


}
