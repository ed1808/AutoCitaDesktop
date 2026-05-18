using AutoCita.Strategies;

namespace AutoCita.Services
{
    /// <summary>
    /// Servicio de generación de reportes. Aplica el patrón Strategy delegando la lógica
    /// de cada tipo de reporte a su implementación correspondiente.
    /// </summary>
    public class ReporteServicio
    {
        private IReporteStrategy _estrategia;

        /// <summary>
        /// Constructor del servicio de reportes.
        /// </summary>
        /// <param name="estrategia">Estrategia inicial de generación de reporte.</param>
        public ReporteServicio(IReporteStrategy estrategia)
        {
            _estrategia = estrategia;
        }

        /// <summary>
        /// Cambia en tiempo de ejecución la estrategia de generación de reporte.
        /// </summary>
        /// <param name="estrategia">Nueva estrategia a aplicar.</param>
        public void EstablecerEstrategia(IReporteStrategy estrategia)
        {
            _estrategia = estrategia;
        }

        /// <summary>
        /// Nombre descriptivo del reporte actualmente configurado.
        /// </summary>
        public string NombreReporteActual => _estrategia.NombreReporte;

        /// <summary>
        /// Genera el reporte usando la estrategia actualmente configurada.
        /// </summary>
        /// <param name="desde">Fecha de inicio del rango.</param>
        /// <param name="hasta">Fecha de fin del rango.</param>
        /// <param name="filtroId">Filtro adicional opcional (SedeId, UsuarioId, etc.).</param>
        /// <returns>Lista de objetos con los datos del reporte.</returns>
        public async Task<List<object>> GenerarAsync(DateTime desde, DateTime hasta, int? filtroId = null)
        {
            return await _estrategia.GenerarAsync(desde, hasta, filtroId);
        }
    }
}
