using AutoCita.Helpers;
using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.Commands
{
    /// <summary>
    /// Comando para crear un nuevo usuario, hasheando la contraseña de forma segura con BCrypt.
    /// </summary>
    public class CrearUsuarioComando : IComando
    {
        private readonly Usuario _usuario;
        private readonly string _passwordTextoPlano;
        private readonly UsuarioRepositorio _usuarioRepositorio;

        /// <summary>
        /// Constructor del comando de creación de usuario.
        /// </summary>
        /// <param name="usuario">Usuario a crear (sin PasswordHash aún).</param>
        /// <param name="passwordTextoPlano">Contraseña en texto plano que será hasheada.</param>
        /// <param name="usuarioRepositorio">Repositorio de usuarios.</param>
        public CrearUsuarioComando(Usuario usuario, string passwordTextoPlano, UsuarioRepositorio usuarioRepositorio)
        {
            _usuario = usuario;
            _passwordTextoPlano = passwordTextoPlano;
            _usuarioRepositorio = usuarioRepositorio;
        }

        /// <summary>
        /// Ejecuta la creación del usuario, validando unicidad del username y hasheando la contraseña.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si el username ya existe en el sistema.</exception>
        public async Task EjecutarAsync()
        {
            var existente = await _usuarioRepositorio.ObtenerPorUsernameAsync(_usuario.Username);
            if (existente != null)
                throw new InvalidOperationException($"El nombre de usuario '{_usuario.Username}' ya está en uso.");

            _usuario.PasswordHash = PasswordHelper.Hashear(_passwordTextoPlano);

            await _usuarioRepositorio.AgregarAsync(_usuario);
            await _usuarioRepositorio.GuardarAsync();
        }
    }
}
