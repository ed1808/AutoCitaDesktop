using AutoCita.Repositories;

namespace AutoCita.Strategies
{
    /// <summary>
    /// Estrategia de reporte que compara las citas gestionadas por cada agente
    /// contra la meta de productividad establecida, identificando los más y menos productivos.
    /// </summary>
    public class ReporteProductividadStrategy : IReporteStrategy
    {
        private readonly CitaRepositorio _citaRepositorio;
        private readonly MetaProductividadRepositorio _metaRepositorio;

        /// <summary>
        /// Constructor de la estrategia de reporte de productividad.
        /// </summary>
        /// <param name="citaRepositorio">Repositorio de citas.</param>
        /// <param name="metaRepositorio">Repositorio de metas de productividad.</param>
        public ReporteProductividadStrategy(CitaRepositorio citaRepositorio, MetaProductividadRepositorio metaRepositorio)
        {
            _citaRepositorio = citaRepositorio;
            _metaRepositorio = metaRepositorio;
        }

        /// <summary>
        /// Nombre descriptivo del reporte.
        /// </summary>
        public string NombreReporte => "Productividad de agentes vs meta";

        /// <summary>
        /// Genera el reporte de productividad comparando citas gestionadas vs meta diaria por agente.
        /// </summary>
        /// <param name="desde">Fecha de inicio del rango.</param>
        /// <param name="hasta">Fecha de fin del rango.</param>
        /// <param name="filtroId">SedeId opcional para filtrar por sede.</param>
        /// <returns>Lista de objetos con: NombreAgente, TotalCitas, MetaDiaria, DiasHabiles, MetaTotal, Cumplimiento (%), Resultado.</returns>
        public async Task<List<object>> GenerarAsync(DateTime desde, DateTime hasta, int? filtroId = null)
        {
            var citas = await _citaRepositorio.ObtenerPorRangoFechasAsync(desde, hasta);
            var meta = await _metaRepositorio.ObtenerMetaVigenteAsync(filtroId);

            int metaLlamadas = meta?.MetaLlamadas ?? 0;
            int diasHabiles = (int)(hasta.Date - desde.Date).TotalDays + 1;
            int metaTotal = metaLlamadas * diasHabiles;

            var resultado = citas
                .GroupBy(c => new { c.UsuarioId, c.Usuario!.Username })
                .Select(g =>
                {
                    int total = g.Count();
                    double cumplimiento = metaTotal > 0 ? Math.Round((double)total / metaTotal * 100, 2) : 0;
                    string evaluacion = cumplimiento >= 100 ? "Productivo" : "Por debajo de la meta";

                    return (object)new
                    {
                        AgenteId     = g.Key.UsuarioId,
                        NombreAgente = g.Key.Username,
                        TotalCitas   = total,
                        MetaDiaria   = metaLlamadas,
                        MetaTotal    = metaTotal,
                        Cumplimiento = cumplimiento,
                        Evaluacion   = evaluacion
                    };
                })
                .OrderByDescending(x => ((dynamic)x).Cumplimiento)
                .ToList();

            return resultado;
        }
    }
}
