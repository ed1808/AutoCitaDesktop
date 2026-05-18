using AutoCita.Helpers;
using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.Services
{
    /// <summary>
    /// Servicio de autenticación de usuarios en el sistema AutoCita.
    /// </summary>
    public class AuthServicio
    {
        private readonly UsuarioRepositorio _usuarioRepositorio;

        /// <summary>
        /// Constructor del servicio de autenticación.
        /// </summary>
        /// <param name="usuarioRepositorio">Repositorio de usuarios.</param>
        public AuthServicio(UsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        /// <summary>
        /// Autentica un usuario validando sus credenciales contra la base de datos.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="password">Contraseña en texto plano.</param>
        /// <returns>El usuario autenticado si las credenciales son correctas, null en caso contrario.</returns>
        public async Task<Usuario?> AutenticarAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var usuario = await _usuarioRepositorio.ObtenerPorUsernameAsync(username);

            if (usuario == null)
                return null;

            if (!PasswordHelper.Verificar(password, usuario.PasswordHash))
                return null;

            return usuario;
        }
    }
}
