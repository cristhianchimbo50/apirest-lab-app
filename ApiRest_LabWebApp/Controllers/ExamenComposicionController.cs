using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;

namespace ApiRest_LabWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamenComposicionController : ControllerBase
    {
        private readonly BdLabContext _context;

        public ExamenComposicionController(BdLabContext context)
        {
            _context = context;
        }

        // GET: api/ExamenComposicion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamenComposicion>>> GetExamenComposiciones()
        {
            return await _context.ExamenComposiciones.ToListAsync();
        }

        // GET: api/ExamenComposicion/5
        [HttpGet("{id}")]
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
    }
}
