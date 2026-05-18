using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.Commands
{
    /// <summary>
    /// Comando genérico para borrado lógico de entidades (Sede o Usuario).
    /// Cambia el campo Activo a false sin eliminar físicamente el registro (RN-03).
    /// </summary>
    public class EliminarLogicoComando : IComando
    {
        private readonly int _entidadId;
        private readonly string _tipoEntidad;
        private readonly SedeRepositorio? _sedeRepositorio;
        private readonly UsuarioRepositorio? _usuarioRepositorio;

        /// <summary>
        /// Constructor para borrado lógico de una Sede.
        /// </summary>
        /// <param name="sedeId">Identificador de la sede a desactivar.</param>
        /// <param name="sedeRepositorio">Repositorio de sedes.</param>
        public EliminarLogicoComando(int sedeId, SedeRepositorio sedeRepositorio)
        {
            _entidadId = sedeId;
            _tipoEntidad = nameof(Sede);
            _sedeRepositorio = sedeRepositorio;
        }

        /// <summary>
        /// Constructor para borrado lógico de un Usuario.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario a desactivar.</param>
        /// <param name="usuarioRepositorio">Repositorio de usuarios.</param>
        public EliminarLogicoComando(int usuarioId, UsuarioRepositorio usuarioRepositorio)
        {
            _entidadId = usuarioId;
            _tipoEntidad = nameof(Usuario);
            _usuarioRepositorio = usuarioRepositorio;
        }

        /// <summary>
        /// Ejecuta el borrado lógico poniendo Activo = false en la entidad correspondiente.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si la entidad no existe o ya está inactiva.</exception>
        public async Task EjecutarAsync()
        {
            if (_tipoEntidad == nameof(Sede) && _sedeRepositorio != null)
            {
                var sede = await _sedeRepositorio.ObtenerPorIdAsync(_entidadId)
                    ?? throw new InvalidOperationException($"No se encontró la sede con Id {_entidadId}.");

                if (!sede.Activo)
                    throw new InvalidOperationException("La sede ya se encuentra inactiva.");

                sede.Activo = false;
                _sedeRepositorio.Actualizar(sede);
                await _sedeRepositorio.GuardarAsync();
            }
            else if (_tipoEntidad == nameof(Usuario) && _usuarioRepositorio != null)
            {
                var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(_entidadId)
                    ?? throw new InvalidOperationException($"No se encontró el usuario con Id {_entidadId}.");

                if (!usuario.Activo)
                    throw new InvalidOperationException("El usuario ya se encuentra inactivo.");

                usuario.Activo = false;
                _usuarioRepositorio.Actualizar(usuario);
                await _usuarioRepositorio.GuardarAsync();
            }
        }
    }
}
