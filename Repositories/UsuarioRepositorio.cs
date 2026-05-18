using AutoCita.Data;
using AutoCita.Enums;
using AutoCita.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Usuario.
    /// </summary>
    public class UsuarioRepositorio : IRepositorio<Usuario>
    {
        private readonly AutoCitaDbContext _context;

        /// <summary>
        /// Constructor del repositorio de usuarios.
        /// </summary>
        /// <param name="context">Contexto de base de datos.</param>
        public UsuarioRepositorio(AutoCitaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios.Include(u => u.Sede).ToListAsync();
        }

        /// <summary>
        /// Obtiene un usuario por su identificador.
        /// </summary>
        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.Include(u => u.Sede).FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Agrega un nuevo usuario.
        /// </summary>
        public async Task AgregarAsync(Usuario entidad)
        {
            await _context.Usuarios.AddAsync(entidad);
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        public void Actualizar(Usuario entidad)
        {
            _context.Usuarios.Update(entidad);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario (username).
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <returns>Usuario encontrado o null.</returns>
        public async Task<Usuario?> ObtenerPorUsernameAsync(string username)
        {
            return await _context.Usuarios.Include(u => u.Sede)
                .FirstOrDefaultAsync(u => u.Username == username && u.Activo);
        }

        /// <summary>
        /// Verifica si existe al menos un usuario con rol Administrador en el sistema.
        /// </summary>
        /// <returns>True si existe al menos un administrador, False en caso contrario.</returns>
        public async Task<bool> ExisteAdminAsync()
        {
            return await _context.Usuarios.AnyAsync(u => u.Rol == RolUsuario.Administrador && u.Activo);
        }

        /// <summary>
        /// Obtiene todos los usuarios activos (no eliminados lógicamente).
        /// </summary>
        /// <returns>Lista de usuarios activos.</returns>
        public async Task<List<Usuario>> ObtenerActivosAsync()
        {
            return await _context.Usuarios.Include(u => u.Sede).Where(u => u.Activo).ToListAsync();
        }
    }
}
