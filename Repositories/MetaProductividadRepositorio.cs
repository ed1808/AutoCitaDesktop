using AutoCita.Data;
using AutoCita.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Repositories
{
    /// <summary>
    /// Repositorio para la entidad MetaProductividad.
    /// </summary>
    public class MetaProductividadRepositorio : IRepositorio<MetaProductividad>
    {
        private readonly AutoCitaDbContext _context;

        /// <summary>
        /// Constructor del repositorio de metas de productividad.
        /// </summary>
        /// <param name="context">Contexto de base de datos.</param>
        public MetaProductividadRepositorio(AutoCitaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las metas de productividad.
        /// </summary>
        public async Task<List<MetaProductividad>> ObtenerTodosAsync()
        {
            return await _context.MetasProductividad.Include(m => m.Sede).ToListAsync();
        }

        /// <summary>
        /// Obtiene una meta de productividad por su identificador.
        /// </summary>
        public async Task<MetaProductividad?> ObtenerPorIdAsync(int id)
        {
            return await _context.MetasProductividad.Include(m => m.Sede)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Agrega una nueva meta de productividad.
        /// </summary>
        public async Task AgregarAsync(MetaProductividad entidad)
        {
            await _context.MetasProductividad.AddAsync(entidad);
        }

        /// <summary>
        /// Actualiza una meta de productividad existente.
        /// </summary>
        public void Actualizar(MetaProductividad entidad)
        {
            _context.MetasProductividad.Update(entidad);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene la meta de productividad vigente para una sede específica o global.
        /// </summary>
        /// <param name="sedeId">Identificador de la sede (nullable). Si es null, busca la meta global.</param>
        /// <returns>Meta vigente o null si no existe ninguna.</returns>
        public async Task<MetaProductividad?> ObtenerMetaVigenteAsync(int? sedeId)
        {
            var hoy = DateTime.Now.Date;
            return await _context.MetasProductividad
                .Where(m => m.SedeId == sedeId
                    && m.FechaInicio.Date <= hoy
                    && (m.FechaFin == null || m.FechaFin.Value.Date >= hoy))
                .OrderByDescending(m => m.FechaInicio)
                .FirstOrDefaultAsync();
        }
    }
}
