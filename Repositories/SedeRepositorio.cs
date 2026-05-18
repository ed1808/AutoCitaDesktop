using AutoCita.Data;
using AutoCita.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Sede.
    /// </summary>
    public class SedeRepositorio : IRepositorio<Sede>
    {
        private readonly AutoCitaDbContext _context;

        /// <summary>
        /// Constructor del repositorio de sedes.
        /// </summary>
        /// <param name="context">Contexto de base de datos.</param>
        public SedeRepositorio(AutoCitaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las sedes.
        /// </summary>
        public async Task<List<Sede>> ObtenerTodosAsync()
        {
            return await _context.Sedes.ToListAsync();
        }

        /// <summary>
        /// Obtiene una sede por su identificador.
        /// </summary>
        public async Task<Sede?> ObtenerPorIdAsync(int id)
        {
            return await _context.Sedes.FindAsync(id);
        }

        /// <summary>
        /// Agrega una nueva sede.
        /// </summary>
        public async Task AgregarAsync(Sede entidad)
        {
            await _context.Sedes.AddAsync(entidad);
        }

        /// <summary>
        /// Actualiza una sede existente.
        /// </summary>
        public void Actualizar(Sede entidad)
        {
            _context.Sedes.Update(entidad);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene todas las sedes activas (no eliminadas lógicamente).
        /// </summary>
        /// <returns>Lista de sedes activas.</returns>
        public async Task<List<Sede>> ObtenerActivasAsync()
        {
            return await _context.Sedes.Where(s => s.Activo).ToListAsync();
        }
    }
}
