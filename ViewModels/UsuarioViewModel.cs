using AutoCita.Commands;
using AutoCita.Enums;
using AutoCita.Helpers;
using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para la gestión de usuarios del sistema (solo rol Administrador).
    /// </summary>
    public class UsuarioViewModel : BaseViewModel
    {
        private readonly UsuarioRepositorio _usuarioRepositorio;
        private readonly SedeRepositorio    _sedeRepositorio;

        private List<Usuario> _usuarios     = [];
        private List<Sede>    _sedes        = [];
        private Usuario?      _seleccionado = null;

        private int       _id              = 0;
        private string    _username        = string.Empty;
        private string    _password        = string.Empty;
        private RolUsuario _rol            = RolUsuario.Agente;
        private int?      _sedeId          = null;
        private string    _mensajeError    = string.Empty;
        private string    _mensajeExito    = string.Empty;
        private bool      _modoEdicion     = false;

        /// <summary>
        /// Constructor del UsuarioViewModel.
        /// </summary>
        public UsuarioViewModel(UsuarioRepositorio usuarioRepositorio, SedeRepositorio sedeRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sedeRepositorio    = sedeRepositorio;
        }

        /// <summary>Lista de usuarios activos del sistema.</summary>
        public List<Usuario> Usuarios { get => _usuarios; set => SetProperty(ref _usuarios, value); }

        /// <summary>Lista de sedes activas para asignar al usuario.</summary>
        public List<Sede> Sedes { get => _sedes; set => SetProperty(ref _sedes, value); }

        /// <summary>Usuario seleccionado en el grid.</summary>
        public Usuario? Seleccionado { get => _seleccionado; set => SetProperty(ref _seleccionado, value); }

        /// <summary>Identificador del usuario en edición.</summary>
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        /// <summary>Nombre de usuario.</summary>
        public string Username { get => _username; set => SetProperty(ref _username, value); }

        /// <summary>Contraseña (solo al crear; vacío al editar mantiene la actual).</summary>
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        /// <summary>Rol del usuario.</summary>
        public RolUsuario Rol { get => _rol; set => SetProperty(ref _rol, value); }

        /// <summary>Sede asignada al usuario (nullable para global).</summary>
        public int? SedeId { get => _sedeId; set => SetProperty(ref _sedeId, value); }

        /// <summary>Mensaje de error del formulario.</summary>
        public string MensajeError { get => _mensajeError; set => SetProperty(ref _mensajeError, value); }

        /// <summary>Mensaje de éxito tras la operación.</summary>
        public string MensajeExito { get => _mensajeExito; set => SetProperty(ref _mensajeExito, value); }

        /// <summary>Indica si el formulario está en modo edición (vs. creación).</summary>
        public bool ModoEdicion { get => _modoEdicion; set => SetProperty(ref _modoEdicion, value); }

        /// <summary>Lista de roles disponibles para el ComboBox.</summary>
        public IEnumerable<RolUsuario> Roles => Enum.GetValues<RolUsuario>();

        /// <summary>
        /// Carga los usuarios activos y las sedes activas desde la base de datos.
        /// </summary>
        public async Task CargarAsync()
        {
            Usuarios = await _usuarioRepositorio.ObtenerActivosAsync();
            Sedes    = await _sedeRepositorio.ObtenerActivasAsync();
        }

        /// <summary>
        /// Prepara el formulario para crear un nuevo usuario.
        /// </summary>
        public void NuevoUsuario()
        {
            Id           = 0;
            Username     = string.Empty;
            Password     = string.Empty;
            Rol          = RolUsuario.Agente;
            SedeId       = null;
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            ModoEdicion  = false;
            Seleccionado = null;
        }

        /// <summary>
        /// Carga los datos del usuario seleccionado en el formulario para edición.
        /// </summary>
        /// <param name="usuario">Usuario a editar.</param>
        public void EditarUsuario(Usuario usuario)
        {
            Id           = usuario.Id;
            Username     = usuario.Username;
            Password     = string.Empty;
            Rol          = usuario.Rol;
            SedeId       = usuario.SedeId;
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            ModoEdicion  = true;
            Seleccionado = usuario;
        }

        /// <summary>
        /// Guarda el usuario (crea o actualiza según ModoEdicion).
        /// </summary>
        public async Task GuardarAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            if (string.IsNullOrWhiteSpace(Username))
            {
                MensajeError = "El nombre de usuario es obligatorio.";
                return;
            }

            try
            {
                if (!ModoEdicion)
                {
                    if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
                    {
                        MensajeError = "La contraseña debe tener al menos 6 caracteres.";
                        return;
                    }

                    var usuario = new Usuario
                    {
                        Username = Username.Trim(),
                        Rol      = Rol,
                        SedeId   = SedeId,
                        Activo   = true
                    };

                    var comando = new CrearUsuarioComando(usuario, Password, _usuarioRepositorio);
                    await comando.EjecutarAsync();
                    MensajeExito = "Usuario creado exitosamente.";
                }
                else
                {
                    var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(Id)
                        ?? throw new InvalidOperationException("Usuario no encontrado.");

                    usuario.Username = Username.Trim();
                    usuario.Rol      = Rol;
                    usuario.SedeId   = SedeId;

                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        if (Password.Length < 6)
                        {
                            MensajeError = "La nueva contraseña debe tener al menos 6 caracteres.";
                            return;
                        }
                        usuario.PasswordHash = PasswordHelper.Hashear(Password);
                    }

                    _usuarioRepositorio.Actualizar(usuario);
                    await _usuarioRepositorio.GuardarAsync();
                    MensajeExito = "Usuario actualizado exitosamente.";
                }

                await CargarAsync();
                NuevoUsuario();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        /// <summary>
        /// Realiza el borrado lógico del usuario seleccionado (Activo = false).
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario a desactivar.</param>
        public async Task DesactivarAsync(int usuarioId)
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            try
            {
                var comando = new EliminarLogicoComando(usuarioId, _usuarioRepositorio);
                await comando.EjecutarAsync();
                MensajeExito = "Usuario desactivado exitosamente.";
                await CargarAsync();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }
    }
}
