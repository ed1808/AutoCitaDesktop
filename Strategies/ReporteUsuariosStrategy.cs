using AutoCita.Repositories;

namespace AutoCita.Strategies
{
    /// <summary>
    /// Estrategia de reporte que retorna el consolidado de usuarios activos por sede o globalmente.
    /// </summary>
    public class ReporteUsuariosStrategy : IReporteStrategy
    {
        private readonly UsuarioRepositorio _usuarioRepositorio;

        /// <summary>
        /// Constructor de la estrategia de reporte de usuarios.
        /// </summary>
        /// <param name="usuarioRepositorio">Repositorio de usuarios.</param>
        public ReporteUsuariosStrategy(UsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        /// <summary>
        /// Nombre descriptivo del reporte.
        /// </summary>
        public string NombreReporte => "Consolidado de usuarios del sistema";

        /// <summary>
        /// Genera el reporte de usuarios activos, agrupados o filtrados por sede.
        /// </summary>
        /// <param name="desde">No utilizado en esta estrategia.</param>
        /// <param name="hasta">No utilizado en esta estrategia.</param>
        /// <param name="filtroId">SedeId opcional. Si es null retorna todos los usuarios activos.</param>
        /// <returns>Lista de objetos con: Id, Username, Rol, Sede, Activo.</returns>
        public async Task<List<object>> GenerarAsync(DateTime desde, DateTime hasta, int? filtroId = null)
        {
            var usuarios = await _usuarioRepositorio.ObtenerActivosAsync();

            if (filtroId.HasValue)
                usuarios = usuarios.Where(u => u.SedeId == filtroId.Value).ToList();

            var resultado = usuarios
                .Select(u => (object)new
                {
                    u.Id,
                    u.Username,
                    Rol    = u.Rol.ToString(),
                    Sede   = u.Sede?.Nombre ?? "Global",
                    u.Activo
                })
                .ToList();

            return resultado;
        }
    }
}
