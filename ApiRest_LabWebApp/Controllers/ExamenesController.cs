using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using ApiRest_LabWebApp.Dto;

namespace ApiRest_LabWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamenesController : ControllerBase
    {
        private readonly BdLabContext _context;

        public ExamenesController(BdLabContext context)
        {
            _context = context;
        }

        // GET: api/examenes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamenDto>>> GetExamenes()
        {
            var examenes = await _context.Examen
                .Where(e => e.Anulado != true)
                .ToListAsync();

            var dtoList = examenes.Select(e => new ExamenDto
            {
                IdExamen = e.IdExamen,
                NombreExamen = e.NombreExamen,
                ValorReferencia = e.ValorReferencia,
                Unidad = e.Unidad,
                Precio = e.Precio,
                Anulado = e.Anulado,
                Estudio = e.Estudio,
                TipoMuestra = e.TipoMuestra,
                TiempoEntrega = e.TiempoEntrega,
                TipoExamen = e.TipoExamen,
                Tecnica = e.Tecnica,
                TituloExamen = e.TituloExamen
            });

            return Ok(dtoList);
        }

        // GET: api/examenes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamenDto>> GetExamen(int id)
        {
            var e = await _context.Examen.FindAsync(id);

            if (e == null || e.Anulado == true)
                return NotFound();

            return new ExamenDto
            {
                IdExamen = e.IdExamen,
                NombreExamen = e.NombreExamen,
                ValorReferencia = e.ValorReferencia,
                Unidad = e.Unidad,
                Precio = e.Precio,
                Anulado = e.Anulado,
                Estudio = e.Estudio,
                TipoMuestra = e.TipoMuestra,
                TiempoEntrega = e.TiempoEntrega,
                TipoExamen = e.TipoExamen,
                Tecnica = e.Tecnica,
                TituloExamen = e.TituloExamen
            };
        }

        // POST: api/examenes
        [HttpPost]
        public async Task<ActionResult<ExamenDto>> PostExamen(ExamenDto dto)
        {
            var examen = new Examen
            {
                NombreExamen = dto.NombreExamen,
                ValorReferencia = dto.ValorReferencia,
                Unidad = dto.Unidad,
                Precio = dto.Precio,
                Anulado = dto.Anulado,
                Estudio = dto.Estudio,
                TipoMuestra = dto.TipoMuestra,
                TiempoEntrega = dto.TiempoEntrega,
                TipoExamen = dto.TipoExamen,
                Tecnica = dto.Tecnica,
                TituloExamen = dto.TituloExamen
            };

            _context.Examen.Add(examen);
            await _context.SaveChangesAsync();

            dto.IdExamen = examen.IdExamen;
            return CreatedAtAction(nameof(GetExamen), new { id = examen.IdExamen }, dto);
        }

        [HttpPost("con-hijos")]
        public async Task<ActionResult<ExamenDto>> PostExamenConHijos(ExamenConComposicionDto dto)
        {
            var examen = new Examen
            {
                NombreExamen = dto.Examen.NombreExamen,
                ValorReferencia = dto.Examen.ValorReferencia,
                Unidad = dto.Examen.Unidad,
                Precio = dto.Examen.Precio,
                Anulado = false,
                Estudio = dto.Examen.Estudio,
                TipoMuestra = dto.Examen.TipoMuestra,
                TiempoEntrega = dto.Examen.TiempoEntrega,
                TipoExamen = dto.Examen.TipoExamen,
                Tecnica = dto.Examen.Tecnica,
                TituloExamen = dto.Examen.TituloExamen
            };

            _context.Examen.Add(examen);
            await _context.SaveChangesAsync();

            // Guardar hijos
            foreach (var idHijo in dto.IdExamenesHijos.Distinct())
            {
                var composicion = new ExamenComposicion
                {
                    IdExamenPadre = examen.IdExamen,
                    IdExamenHijo = idHijo
                };
                _context.ExamenComposiciones.Add(composicion);
            }

            await _context.SaveChangesAsync();

            dto.Examen.IdExamen = examen.IdExamen;
            return CreatedAtAction(nameof(GetExamen), new { id = examen.IdExamen }, dto.Examen);
        }


        // PUT: api/examenes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamen(int id, ExamenDto dto)
        {
            var examen = await _context.Examen.FindAsync(id);
            if (examen == null)
                return NotFound();

            examen.NombreExamen = dto.NombreExamen;
            examen.ValorReferencia = dto.ValorReferencia;
            examen.Unidad = dto.Unidad;
            examen.Precio = dto.Precio;
            examen.Anulado = dto.Anulado;
            examen.Estudio = dto.Estudio;
            examen.TipoMuestra = dto.TipoMuestra;
            examen.TiempoEntrega = dto.TiempoEntrega;
            examen.TipoExamen = dto.TipoExamen;
            examen.Tecnica = dto.Tecnica;
            examen.TituloExamen = dto.TituloExamen;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/examenes/anular/5
        [HttpPut("anular/{id}")]
        public async Task<IActionResult> AnularExamen(int id)
        {
            var examen = await _context.Examen.FindAsync(id);
            if (examen == null)
                return NotFound();

            examen.Anulado = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamenExists(int id)
        {
            return _context.Examen.Any(e => e.IdExamen == id);
        }

        [HttpGet("con-reactivos/{id}")]
        public async Task<ActionResult<ExamenConReactivosDto>> GetExamenConReactivos(int id)
        {
            var examen = await _context.Examen
                .FirstOrDefaultAsync(e => e.IdExamen == id && e.Anulado != true);

            if (examen == null)
                return NotFound();

            var dto = new ExamenDto
            {
                IdExamen = examen.IdExamen,
                NombreExamen = examen.NombreExamen,
                ValorReferencia = examen.ValorReferencia,
                Unidad = examen.Unidad,
                Precio = examen.Precio,
                Anulado = examen.Anulado,
                Estudio = examen.Estudio,
                TipoMuestra = examen.TipoMuestra,
                TiempoEntrega = examen.TiempoEntrega,
                TipoExamen = examen.TipoExamen,
                Tecnica = examen.Tecnica,
                TituloExamen = examen.TituloExamen
            };

            var reactivos = await _context.ExamenReactivos
                .Where(r => r.IdExamen == id)
                .Include(r => r.IdReactivoNavigation)
                .Select(r => new ExamenReactivoDto
                {
                    IdReactivo = r.IdReactivo ?? 0,
                    NombreReactivo = r.IdReactivoNavigation!.NombreReactivo,
                    Unidad = r.Unidad ?? r.IdReactivoNavigation.Unidad ?? "",
                    CantidadUsada = r.CantidadUsada ?? 0
                }).ToListAsync();

            return Ok(new ExamenConReactivosDto
            {
                Examen = dto,
                Reactivos = reactivos
            });
        }


    }
}
