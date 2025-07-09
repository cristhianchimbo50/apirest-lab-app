using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "administrador")]
public class ConveniosController : ControllerBase
{
    private readonly BdLabContext _context;

    public ConveniosController(BdLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConvenioDto>>> GetConvenios()
    {
        var convenios = await _context.Convenios
            .Include(c => c.IdMedicoNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .Select(c => new ConvenioDto
            {
                IdConvenio = c.IdConvenio,
                FechaConvenio = c.FechaConvenio,
                MontoTotal = c.MontoTotal,
                PorcentajeComision = c.PorcentajeComision,
                NombreMedico = c.IdMedicoNavigation.NombreMedico,
                Anulado = c.Anulado ?? false
            })
            .ToListAsync();

        return Ok(convenios);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<ConvenioDetalleDto>> GetConvenio(int id)
    {
        var convenio = await _context.Convenios
            .Include(c => c.IdMedicoNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .Include(c => c.DetalleConvenios)
                .ThenInclude(dc => dc.Orden)
                    .ThenInclude(o => o.IdPacienteNavigation)
            .FirstOrDefaultAsync(c => c.IdConvenio == id);

        if (convenio == null)
            return NotFound();

        var dto = new ConvenioDetalleDto
        {
            IdConvenio = convenio.IdConvenio,
            FechaConvenio = convenio.FechaConvenio,
            MontoTotal = convenio.MontoTotal,
            PorcentajeComision = convenio.PorcentajeComision,
            NombreMedico = convenio.IdMedicoNavigation?.NombreMedico ?? "",
            NombreUsuario = convenio.IdUsuarioNavigation?.Nombre ?? "",
            Ordenes = convenio.DetalleConvenios.Select(dc => new OrdenConvenioDto
            {
                IdOrden = dc.Orden.IdOrden,
                NumeroOrden = dc.Orden.NumeroOrden,
                Paciente = dc.Orden.IdPacienteNavigation?.NombrePaciente ?? "",
                Total = dc.Orden.Total,
                EstadoPago = dc.Orden.EstadoPago, FechaOrden = dc.Orden.FechaOrden
            }).ToList()
        };

        return Ok(dto);
    }


    [HttpGet("ordenes-disponibles/{idMedico}")]
    public async Task<IActionResult> ObtenerOrdenesPorMedico(int idMedico)
    {
        var ordenes = await _context.Ordens
            .Include(o => o.IdPacienteNavigation)
            .Where(o => o.IdMedico == idMedico && o.Anulado == false && o.LiquidadoConvenio == false)
            .Select(o => new
            {
                o.IdOrden,
                o.NumeroOrden,
                Paciente = o.IdPacienteNavigation.NombrePaciente,
                o.Total,
                o.EstadoPago
            })
            .ToListAsync();

        return Ok(ordenes);
    }

    [HttpPost("registrar-convenio")]
    public async Task<IActionResult> RegistrarConvenio([FromBody] ConvenioRegistrarDto dto)
    {
        if (dto == null || dto.Ordenes == null || !dto.Ordenes.Any())
            return BadRequest("Datos de convenio incompletos.");

        var convenio = new Convenio
        {
            IdMedico = dto.IdMedico,
            IdUsuario = dto.IdUsuario,
            FechaConvenio = DateOnly.FromDateTime(DateTime.Now),
            MontoTotal = dto.Ordenes.Sum(o => o.Total),
            PorcentajeComision = dto.Comision,
            Anulado = false
        };

        _context.Convenios.Add(convenio);
        await _context.SaveChangesAsync();

        foreach (var orden in dto.Ordenes)
        {
            _context.DetalleConvenios.Add(new DetalleConvenio
            {
                IdConvenio = convenio.IdConvenio,
                IdOrden = orden.IdOrden,
                Subtotal = orden.Total
            });

            var ordenDb = await _context.Ordens.FindAsync(orden.IdOrden);
            if (ordenDb != null)
            {
                ordenDb.LiquidadoConvenio = true;
                _context.Ordens.Update(ordenDb);
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Convenio registrado correctamente." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutConvenio(int id, Convenio convenio)
    {
        if (id != convenio.IdConvenio)
            return BadRequest();

        _context.Entry(convenio).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ConvenioExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConvenio(int id)
    {
        var convenio = await _context.Convenios.FindAsync(id);
        if (convenio == null)
            return NotFound();

        convenio.Anulado = true;
        _context.Convenios.Update(convenio);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ConvenioExists(int id)
    {
        return _context.Convenios.Any(e => e.IdConvenio == id);
    }
}
