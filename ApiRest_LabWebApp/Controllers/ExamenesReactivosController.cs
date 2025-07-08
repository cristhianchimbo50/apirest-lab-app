using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExamenesReactivosController : ControllerBase
{
    private readonly BdLabContext _context;

    public ExamenesReactivosController(BdLabContext context)
    {
        _context = context;
    }

    // Obtener todos los vínculos examen-reactivo
    [HttpGet]
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<ActionResult<IEnumerable<ExamenReactivo>>> GetExamenesReactivos()
    {
        return await _context.ExamenReactivos
            .Include(er => er.IdExamenNavigation)
            .Include(er => er.IdReactivoNavigation)
            .Include(er => er.IdUsuarioNavigation)
            .ToListAsync();
    }

    // Obtener vínculo por ID
    [HttpGet("{id}")]
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<ActionResult<ExamenReactivo>> GetExamenReactivo(int id)
    {
        var examenReactivo = await _context.ExamenReactivos
            .Include(er => er.IdExamenNavigation)
            .Include(er => er.IdReactivoNavigation)
            .Include(er => er.IdUsuarioNavigation)
            .FirstOrDefaultAsync(er => er.IdExamenReactivo == id);

        if (examenReactivo == null)
            return NotFound();

        return examenReactivo;
    }

    // Obtener todos los reactivos asociados a un examen
    [HttpGet("por-examen/{idExamen}")]
    [Authorize(Roles = "administrador,laboratorista")]
    public async Task<ActionResult<IEnumerable<ExamenReactivoDto>>> ObtenerPorExamen(int idExamen)
    {
        var lista = await _context.ExamenReactivos
            .Where(er => er.IdExamen == idExamen)
            .Include(er => er.IdReactivoNavigation)
            .Select(er => new ExamenReactivoDto
            {
                IdExamenReactivo = er.IdExamenReactivo,
                IdExamen = er.IdExamen ?? 0,
                IdReactivo = er.IdReactivo ?? 0,
                CantidadUsada = er.CantidadUsada ?? 0,
                Unidad = er.Unidad ?? "",
                NombreReactivo = er.IdReactivoNavigation!.NombreReactivo ?? ""
            })
            .ToListAsync();

        return Ok(lista);
    }

    // Registrar un conjunto de asociaciones examen-reactivo
    [HttpPost("registrar-lote")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> RegistrarLote([FromBody] List<ExamenReactivoDto> lista)
    {
        if (lista == null || lista.Count == 0)
            return BadRequest("Lista vacía.");

        // Eliminar asociaciones previas
        var idExamen = lista.First().IdExamen;
        var anteriores = await _context.ExamenReactivos.Where(er => er.IdExamen == idExamen).ToListAsync();
        _context.ExamenReactivos.RemoveRange(anteriores);

        // Agregar nuevas asociaciones
        foreach (var dto in lista)
        {
            _context.ExamenReactivos.Add(new ExamenReactivo
            {
                IdExamen = dto.IdExamen,
                IdReactivo = dto.IdReactivo,
                CantidadUsada = dto.CantidadUsada,
                Unidad = dto.Unidad,
                IdUsuario = dto.IdUsuario
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Reactivos asociados correctamente al examen." });
    }

    // Eliminar asociación individual
    [HttpDelete("{id}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> DeleteExamenReactivo(int id)
    {
        var examenReactivo = await _context.ExamenReactivos.FindAsync(id);
        if (examenReactivo == null)
            return NotFound();

        _context.ExamenReactivos.Remove(examenReactivo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExamenReactivoExists(int id)
    {
        return _context.ExamenReactivos.Any(e => e.IdExamenReactivo == id);
    }

    [HttpPost("examen/{idExamen}")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> GuardarReactivosExamen(int idExamen, [FromBody] List<ReactivoAsociadoDto> lista)
    {
        var existentes = await _context.ExamenReactivos
            .Where(x => x.IdExamen == idExamen)
            .ToListAsync();

        _context.ExamenReactivos.RemoveRange(existentes);

        // Obtener el ID del usuario autenticado
        var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claimId, out int idUsuario))
        {
            return Unauthorized("No se pudo obtener el ID del usuario autenticado.");
        }

        foreach (var dto in lista)
        {
            var entidad = new ExamenReactivo
            {
                IdExamen = idExamen,
                IdReactivo = dto.IdReactivo,
                CantidadUsada = (decimal?)dto.CantidadUsada,
                Unidad = dto.Unidad,
                IdUsuario = idUsuario
            };

            _context.ExamenReactivos.Add(entidad);
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Asociaciones guardadas correctamente" });
    }

}
