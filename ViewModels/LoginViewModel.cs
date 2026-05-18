using AutoCita.Helpers;
using AutoCita.Services;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para la pantalla de inicio de sesión.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthServicio _authServicio;

        private string _username     = string.Empty;
        private string _password     = string.Empty;
        private string _mensajeError = string.Empty;
        private bool   _cargando     = false;

        /// <summary>
        /// Constructor del LoginViewModel.
        /// </summary>
        /// <param name="authServicio">Servicio de autenticación.</param>
        public LoginViewModel(AuthServicio authServicio)
        {
            _authServicio = authServicio;
        }

        /// <summary>
        /// Nombre de usuario ingresado en el formulario.
        /// </summary>
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        /// <summary>
        /// Contraseña ingresada en el formulario.
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Mensaje de error mostrado cuando las credenciales son incorrectas.
        /// </summary>
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        /// <summary>
        /// Indica si hay una operación de autenticación en progreso.
        /// </summary>
        public bool Cargando
        {
            get => _cargando;
            set => SetProperty(ref _cargando, value);
        }

        /// <summary>
        /// Se dispara cuando el inicio de sesión fue exitoso.
        /// </summary>
        public event EventHandler? SesionIniciada;

        /// <summary>
        /// Intenta autenticar al usuario con las credenciales ingresadas.
        /// Si es exitoso, guarda el usuario en SesionActual y dispara SesionIniciada.
        /// </summary>
        public async Task IniciarSesionAsync()
        {
            MensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MensajeError = "Debe ingresar el usuario y la contraseña.";
                return;
            }

            Cargando = true;

            try
            {
                var usuario = await _authServicio.AutenticarAsync(Username, Password);

                if (usuario == null)
                {
                    MensajeError = "Usuario y/o contraseña incorrectos.";
                    return;
                }

                SesionActual.Instancia.UsuarioLogueado = usuario;
                SesionIniciada?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al iniciar sesión: {ex.Message}";
            }
            finally
            {
                Cargando = false;
            }
        }
    }
}
