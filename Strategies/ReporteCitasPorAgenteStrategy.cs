using AutoCita.Repositories;

namespace AutoCita.Strategies
{
    /// <summary>
    /// Estrategia de reporte que agrupa las citas agendadas por agente en un rango de fechas.
    /// Usada por el director del contact center para supervisar la gestión de su equipo.
    /// </summary>
    public class ReporteCitasPorAgenteStrategy : IReporteStrategy
    {
        private readonly CitaRepositorio _citaRepositorio;

        /// <summary>
        /// Constructor de la estrategia de reporte de citas por agente.
        /// </summary>
        /// <param name="citaRepositorio">Repositorio de citas.</param>
        public ReporteCitasPorAgenteStrategy(CitaRepositorio citaRepositorio)
        {
            _citaRepositorio = citaRepositorio;
        }

        /// <summary>
        /// Nombre descriptivo del reporte.
        /// </summary>
        public string NombreReporte => "Citas agendadas por agente";

        /// <summary>
        /// Genera el reporte de citas agrupadas por agente para el rango de fechas indicado.
        /// </summary>
        /// <param name="desde">Fecha de inicio del rango.</param>
        /// <param name="hasta">Fecha de fin del rango.</param>
        /// <param name="filtroId">No utilizado en esta estrategia.</param>
        /// <returns>Lista de objetos anónimos con: NombreAgente, Username, TotalCitas.</returns>
        public async Task<List<object>> GenerarAsync(DateTime desde, DateTime hasta, int? filtroId = null)
        {
            var citas = await _citaRepositorio.ObtenerPorRangoFechasAsync(desde, hasta);

            var resultado = citas
                .GroupBy(c => new { c.UsuarioId, c.Usuario!.Username })
                .Select(g => (object)new
                {
                    AgenteId     = g.Key.UsuarioId,
                    NombreAgente = g.Key.Username,
                    TotalCitas   = g.Count()
                })
                .OrderByDescending(x => ((dynamic)x).TotalCitas)
                .ToList();

            return resultado;
        }
    }
}
