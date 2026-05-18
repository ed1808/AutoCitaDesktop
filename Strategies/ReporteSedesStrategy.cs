using AutoCita.Repositories;

namespace AutoCita.Strategies
{
    /// <summary>
    /// Estrategia de reporte que retorna el listado de todas las sedes activas del sistema.
    /// </summary>
    public class ReporteSedesStrategy : IReporteStrategy
    {
        private readonly SedeRepositorio _sedeRepositorio;

        /// <summary>
        /// Constructor de la estrategia de reporte de sedes.
        /// </summary>
        /// <param name="sedeRepositorio">Repositorio de sedes.</param>
        public ReporteSedesStrategy(SedeRepositorio sedeRepositorio)
        {
            _sedeRepositorio = sedeRepositorio;
        }

        /// <summary>
        /// Nombre descriptivo del reporte.
        /// </summary>
        public string NombreReporte => "Listado de sedes del sistema";

        /// <summary>
        /// Genera el reporte con todas las sedes activas existentes.
        /// </summary>
        /// <param name="desde">No utilizado en esta estrategia.</param>
        /// <param name="hasta">No utilizado en esta estrategia.</param>
        /// <param name="filtroId">No utilizado en esta estrategia.</param>
        /// <returns>Lista de objetos con: Id, Nombre, Direccion, Activo.</returns>
        public async Task<List<object>> GenerarAsync(DateTime desde, DateTime hasta, int? filtroId = null)
        {
            var sedes = await _sedeRepositorio.ObtenerActivasAsync();

            var resultado = sedes
                .Select(s => (object)new
                {
                    s.Id,
                    s.Nombre,
                    s.Direccion,
                    s.Activo
                })
                .ToList();

            return resultado;
        }
    }
}
