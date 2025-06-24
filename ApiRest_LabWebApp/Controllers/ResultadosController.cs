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
            // 1. Crear resultado
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

            nuevoResultado.NumeroResultado = $"RES-{nuevoResultado.IdResultado:D5}";
            await _context.SaveChangesAsync();

            // 2. Crear detalles
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

            // 3. Actualizar detalle_orden
            var idsExamenes = dto.Examenes.Select(e => e.IdExamen).ToList();
            var detallesOrden = await _context.DetalleOrdens
                .Where(d => d.IdOrden == dto.IdOrden && d.IdExamen.HasValue && idsExamenes.Contains(d.IdExamen.Value))
                .ToListAsync();

            foreach (var det in detallesOrden)
            {
                det.IdResultado = nuevoResultado.IdResultado;
            }

            // 4. Descontar reactivos y registrar movimiento
            foreach (var ex in dto.Examenes)
            {
                var asociaciones = await _context.ExamenReactivos
                    .Where(er => er.IdExamen == ex.IdExamen)
                    .ToListAsync();

                foreach (var ar in asociaciones)
                {
                    var reactivo = await _context.Reactivos.FindAsync(ar.IdReactivo);
                    if (reactivo == null) continue;

                    // Validar stock suficiente
                    if (reactivo.CantidadDisponible < ar.CantidadUsada)
                    {
                        await trans.RollbackAsync();
                        return BadRequest(new
                        {
                            mensaje = $"Stock insuficiente para el reactivo '{reactivo.NombreReactivo}'. Disponible: {reactivo.CantidadDisponible}, requerido: {ar.CantidadUsada}"
                        });
                    }

                    // Descontar stock
                    reactivo.CantidadDisponible -= ar.CantidadUsada;

                    // Registrar movimiento
                    _context.MovimientoReactivos.Add(new MovimientoReactivo
                    {
                        IdReactivo = ar.IdReactivo,
                        TipoMovimiento = "EGRESO",
                        Cantidad = ar.CantidadUsada,
                        FechaMovimiento = DateTime.Now,
                        IdOrden = dto.IdOrden,
                        Observacion = $"Consumo por resultado de examen ID {ex.IdExamen} - Resultado #{nuevoResultado.NumeroResultado}"
                    });
                }
            }

            await _context.SaveChangesAsync();
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
        var resultado = await _context.Resultados
            .Include(r => r.DetalleResultados)
            .FirstOrDefaultAsync(r => r.IdResultado == id);

        if (resultado == null) return NotFound();

        resultado.Anulado = true;

        // Anular todos los detalles del resultado
        foreach (var detalle in resultado.DetalleResultados)
        {
            detalle.Anulado = true;
        }

        // Quitar referencia en DetalleOrden
        var detallesOrden = await _context.DetalleOrdens
            .Where(d => d.IdResultado == id)
            .ToListAsync();

        foreach (var d in detallesOrden)
        {
            d.IdResultado = null;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }




}
