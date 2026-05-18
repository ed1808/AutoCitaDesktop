using AutoCita.Data;
using AutoCita.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Cliente.
    /// </summary>
    public class ClienteRepositorio : IRepositorio<Cliente>
    {
        private readonly AutoCitaDbContext _context;

        /// <summary>
        /// Constructor del repositorio de clientes.
        /// </summary>
        /// <param name="context">Contexto de base de datos.</param>
        public ClienteRepositorio(AutoCitaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        /// <summary>
        /// Obtiene un cliente por su identificador.
        /// </summary>
        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        /// <summary>
        /// Agrega un nuevo cliente.
        /// </summary>
        public async Task AgregarAsync(Cliente entidad)
        {
            await _context.Clientes.AddAsync(entidad);
        }

        /// <summary>
        /// Actualiza un cliente existente.
        /// </summary>
        public void Actualizar(Cliente entidad)
        {
            _context.Clientes.Update(entidad);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene un cliente por su número de documento de identidad.
        /// </summary>
        /// <param name="documentoIdentidad">Número de documento de identidad.</param>
        /// <returns>Cliente encontrado o null.</returns>
        public async Task<Cliente?> ObtenerPorDocumentoAsync(string documentoIdentidad)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentidad == documentoIdentidad);
        }
    }
}
