using ApiRest_LabWebApp.DTOs;
using ApiRest_LabWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRest_LabWebApp.Services
{
    public class ResultadoService
    {
        private readonly BdLabContext _context;

        public ResultadoService(BdLabContext context)
        {
            _context = context;
        }

        public async Task<ResultadoCompletoDto?> ObtenerResultadoCompletoAsync(int idResultado)
        {
            var resultado = await _context.Resultados
                .Include(r => r.IdPacienteNavigation)
                .Include(r => r.IdMedicoNavigation)
                .Include(r => r.IdOrdenNavigation) // <- Faltaba esto
                .Include(r => r.DetalleResultados)
                    .ThenInclude(d => d.IdExamenNavigation)
                .FirstOrDefaultAsync(r => r.IdResultado == idResultado);

            if (resultado == null)
                return null;

            return new ResultadoCompletoDto
            {
                NumeroOrden = resultado.IdOrdenNavigation?.NumeroOrden ?? "",
                NumeroResultado = resultado.NumeroResultado,
                FechaResultado = resultado.FechaResultado,
                NombrePaciente = resultado.IdPacienteNavigation?.NombrePaciente ?? "",
                CedulaPaciente = resultado.IdPacienteNavigation?.CedulaPaciente ?? "",
                EdadPaciente = resultado.IdPacienteNavigation != null ? CalcularEdad(resultado.IdPacienteNavigation.FechaNacPaciente.ToDateTime(TimeOnly.MinValue)) : 0,
                MedicoSolicitante = resultado.IdMedicoNavigation?.NombreMedico ?? "",
                Detalles = new List<ResultadoDetalleDto>
                {
                    new ResultadoDetalleDto
                    {
                        IdResultado = resultado.IdResultado,
                        NumeroResultado = resultado.NumeroResultado,
                        CedulaPaciente = resultado.IdPacienteNavigation?.CedulaPaciente ?? "",
                        NombrePaciente = resultado.IdPacienteNavigation?.NombrePaciente ?? "",
                        FechaResultado = resultado.FechaResultado,
                        Anulado = resultado.Anulado ?? false,
                        Examenes = resultado.DetalleResultados.Select(d => new ResultadoExamenDto
                        {
                            IdExamen = d.IdExamen ?? 0,
                            NombreExamen = d.IdExamenNavigation.NombreExamen,
                            Valor = d.Valor,
                            Unidad = d.Unidad,
                            Observacion = d.Observacion,
                            TituloExamen = d.IdExamenNavigation.TituloExamen,
                            ValorReferencia = d.ValorReferencia
                        }).ToList()
                    }
                }
            };
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }
    }
}
