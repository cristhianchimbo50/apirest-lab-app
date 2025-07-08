using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.Dto;
using Microsoft.AspNetCore.Authorization;

namespace ApiRest_LabWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamenComposicionController : ControllerBase
    {
        private readonly BdLabContext _context;

        public ExamenComposicionController(BdLabContext context)
        {
            _context = context;
        }

        // GET: api/ExamenComposicion
        [HttpGet]
        [Authorize(Roles = "administrador,laboratorista")]
        public async Task<ActionResult<IEnumerable<ExamenComposicion>>> GetExamenComposiciones()
        {
            return await _context.ExamenComposiciones.ToListAsync();
        }

        // GET: api/ExamenComposicion/5
        [HttpGet("{id}")]
        [Authorize(Roles = "administrador,laboratorista")]
        public async Task<ActionResult<ExamenComposicion>> GetExamenComposicion(int id)
        {
            var examenComposicion = await _context.ExamenComposiciones.FindAsync(id);

            if (examenComposicion == null)
            {
                return NotFound();
            }

            return examenComposicion;
        }

        // PUT: api/ExamenComposicion/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> PutExamenComposicion(int id, ExamenComposicion examenComposicion)
        {
            if (id != examenComposicion.IdExamenPadre)
            {
                return BadRequest();
            }

            _context.Entry(examenComposicion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamenComposicionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ExamenComposicion
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "administrador")]
        public async Task<ActionResult<ExamenComposicion>> PostExamenComposicion(ExamenComposicion examenComposicion)
        {
            _context.ExamenComposiciones.Add(examenComposicion);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ExamenComposicionExists(examenComposicion.IdExamenPadre))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetExamenComposicion", new { id = examenComposicion.IdExamenPadre }, examenComposicion);
        }

        // DELETE: api/ExamenComposicion/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> DeleteExamenComposicion(int id)
        {
            var examenComposicion = await _context.ExamenComposiciones.FindAsync(id);
            if (examenComposicion == null)
            {
                return NotFound();
            }

            _context.ExamenComposiciones.Remove(examenComposicion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamenComposicionExists(int id)
        {
            return _context.ExamenComposiciones.Any(e => e.IdExamenPadre == id);
        }

        // GET: api/ExamenComposicion/padre/5
        [HttpGet("padre/{idExamenPadre}")]
        [Authorize(Roles = "administrador,laboratorista")]
        public async Task<ActionResult<IEnumerable<ExamenDto>>> GetHijosDeExamen(int idExamenPadre)
        {
            var hijos = await _context.ExamenComposiciones
                .Where(x => x.IdExamenPadre == idExamenPadre)
                .Include(x => x.ExamenHijo)
                .Select(x => new ExamenDto
                {
                    IdExamen = x.ExamenHijo!.IdExamen,
                    NombreExamen = x.ExamenHijo.NombreExamen,
                    Estudio = x.ExamenHijo.Estudio,
                    ValorReferencia = x.ExamenHijo.ValorReferencia,
                    Unidad = x.ExamenHijo.Unidad
                })
                .ToListAsync();

            return Ok(hijos);
        }

        // DELETE: api/ExamenComposicion/padre/5/hijo/3
        [HttpDelete("padre/{idPadre}/hijo/{idHijo}")]
    [Authorize(Roles = "administrador")]
        public async Task<IActionResult> DeleteComposicion(int idPadre, int idHijo)
        {
            var composicion = await _context.ExamenComposiciones
                .FirstOrDefaultAsync(x => x.IdExamenPadre == idPadre && x.IdExamenHijo == idHijo);

            if (composicion == null)
                return NotFound();

            _context.ExamenComposiciones.Remove(composicion);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
