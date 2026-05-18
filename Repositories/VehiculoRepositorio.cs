using AutoCita.Data;
using AutoCita.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Vehiculo.
    /// </summary>
    public class VehiculoRepositorio : IRepositorio<Vehiculo>
    {
        private readonly AutoCitaDbContext _context;

        /// <summary>
        /// Constructor del repositorio de vehículos.
        /// </summary>
        /// <param name="context">Contexto de base de datos.</param>
        public VehiculoRepositorio(AutoCitaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los vehículos.
        /// </summary>
        public async Task<List<Vehiculo>> ObtenerTodosAsync()
        {
            return await _context.Vehiculos.Include(v => v.Cliente).ToListAsync();
        }

        /// <summary>
        /// Obtiene un vehículo por su identificador.
        /// </summary>
        public async Task<Vehiculo?> ObtenerPorIdAsync(int id)
        {
            return await _context.Vehiculos.Include(v => v.Cliente).FirstOrDefaultAsync(v => v.Id == id);
        }

        /// <summary>
        /// Agrega un nuevo vehículo.
        /// </summary>
        public async Task AgregarAsync(Vehiculo entidad)
        {
            await _context.Vehiculos.AddAsync(entidad);
        }

        /// <summary>
        /// Actualiza un vehículo existente.
        /// </summary>
        public void Actualizar(Vehiculo entidad)
        {
            _context.Vehiculos.Update(entidad);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene un vehículo por su placa.
        /// </summary>
        /// <param name="placa">Placa del vehículo.</param>
        /// <returns>Vehículo encontrado o null.</returns>
        public async Task<Vehiculo?> ObtenerPorPlacaAsync(string placa)
        {
            return await _context.Vehiculos.Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);
        }
    }
}
