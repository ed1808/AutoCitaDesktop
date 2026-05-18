namespace AutoCita.Strategies
{
    /// <summary>
    /// Interfaz de la estrategia para la generación de reportes del sistema.
    /// </summary>
    public interface IReporteStrategy
    {
        /// <summary>
        /// Nombre descriptivo del tipo de reporte.
        /// </summary>
        string NombreReporte { get; }

        /// <summary>
        /// Genera el reporte para el rango de fechas y filtro indicados.
        /// </summary>
        /// <param name="desde">Fecha de inicio del rango.</param>
        /// <param name="hasta">Fecha de fin del rango.</param>
        /// <param name="filtroId">Identificador de filtro adicional (sedeId, usuarioId, etc.). Opcional.</param>
        /// <returns>Resultado del reporte como lista de objetos.</returns>
        Task<List<object>> GenerarAsync(DateTime desde, DateTime hasta, int? filtroId = null);
    }
}
