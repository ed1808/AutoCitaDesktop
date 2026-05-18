using AutoCita.Commands;
using AutoCita.Data;
using AutoCita.Enums;
using AutoCita.Helpers;
using AutoCita.Models;
using AutoCita.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para el asistente de configuración inicial (onboarding).
    /// Gestiona los 3 pasos: validar conexión, crear BD y crear usuario administrador.
    /// </summary>
    public class OnboardingViewModel : BaseViewModel
    {
        // ─── Campos de respaldo ────────────────────────────────────────────────
        private string _cadenaConexion = string.Empty;
        private string _mensajeEstado  = string.Empty;
        private string _mensajeError   = string.Empty;
        private int    _pasoActual     = 1;
        private bool   _conexionValida = false;

        // ─── Campos del formulario Admin ───────────────────────────────────────
        private string _usernameAdmin   = string.Empty;
        private string _passwordAdmin   = string.Empty;
        private string _confirmarPassword = string.Empty;

        // ─── Propiedades ───────────────────────────────────────────────────────

        /// <summary>
        /// Cadena de conexión al servidor PostgreSQL.
        /// </summary>
        public string CadenaConexion
        {
            get => _cadenaConexion;
            set => SetProperty(ref _cadenaConexion, value);
        }

        /// <summary>
        /// Mensaje de estado del proceso de onboarding.
        /// </summary>
        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => SetProperty(ref _mensajeEstado, value);
        }

        /// <summary>
        /// Mensaje de error producido durante algún paso del onboarding.
        /// </summary>
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        /// <summary>
        /// Paso actual del asistente (1 = conexión, 2 = BD, 3 = admin).
        /// </summary>
        public int PasoActual
        {
            get => _pasoActual;
            set => SetProperty(ref _pasoActual, value);
        }

        /// <summary>
        /// Indica si la conexión con la base de datos fue validada exitosamente.
        /// </summary>
        public bool ConexionValida
        {
            get => _conexionValida;
            set => SetProperty(ref _conexionValida, value);
        }

        /// <summary>
        /// Nombre de usuario para el administrador inicial.
        /// </summary>
        public string UsernameAdmin
        {
            get => _usernameAdmin;
            set => SetProperty(ref _usernameAdmin, value);
        }

        /// <summary>
        /// Contraseña del administrador inicial.
        /// </summary>
        public string PasswordAdmin
        {
            get => _passwordAdmin;
            set => SetProperty(ref _passwordAdmin, value);
        }

        /// <summary>
        /// Confirmación de contraseña del administrador inicial.
        /// </summary>
        public string ConfirmarPassword
        {
            get => _confirmarPassword;
            set => SetProperty(ref _confirmarPassword, value);
        }

        // ─── Eventos ───────────────────────────────────────────────────────────

        /// <summary>
        /// Se dispara cuando el onboarding finaliza correctamente.
        /// </summary>
        public event EventHandler? OnboardingCompletado;

        /// <summary>
        /// Se dispara cuando se debe avanzar al siguiente paso del wizard.
        /// </summary>
        public event EventHandler<int>? PasoCambiado;

        // ─── Métodos ───────────────────────────────────────────────────────────

        /// <summary>
        /// Valida la cadena de conexión, crea la base de datos si no existe
        /// y avanza al paso de creación de administrador si no hay uno.
        /// </summary>
        public async Task ValidarConexionAsync()
        {
            MensajeError  = string.Empty;
            MensajeEstado = "Validando conexión...";

            if (string.IsNullOrWhiteSpace(CadenaConexion))
            {
                MensajeError  = "Debe ingresar la cadena de conexión.";
                MensajeEstado = string.Empty;
                return;
            }

            try
            {
                var opciones = new DbContextOptionsBuilder<AutoCitaDbContext>()
                    .UseNpgsql(CadenaConexion)
                    .Options;

                await using var context = new AutoCitaDbContext(opciones);

                // Paso 1: Probar conectividad al servidor (no a la BD específica)
                var builderPrueba = new NpgsqlConnectionStringBuilder(CadenaConexion)
                {
                    Database = "postgres"
                };
                await using var connPrueba = new NpgsqlConnection(builderPrueba.ConnectionString);
                await connPrueba.OpenAsync();
                await connPrueba.CloseAsync();

                // Paso 2: Crear BD y aplicar migraciones pendientes
                MensajeEstado = "Conexión exitosa. Verificando base de datos...";
                var migracionesPendientes = (await context.Database.GetPendingMigrationsAsync()).ToList();
                if (migracionesPendientes.Count > 0)
                {
                    MensajeEstado = $"Aplicando {migracionesPendientes.Count} migración(es) pendiente(s)...";
                    await context.Database.MigrateAsync();
                }

                ConfiguracionHelper.GuardarCadenaConexion(CadenaConexion);
                ConexionValida = true;

                // Paso 3: Verificar si existe usuario Admin
                var usuarioRepo = new UsuarioRepositorio(context);
                bool existeAdmin = await usuarioRepo.ExisteAdminAsync();

                if (existeAdmin)
                {
                    MensajeEstado = "¡Configuración completada exitosamente!";
                    PasoActual = 3;
                    OnboardingCompletado?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MensajeEstado = "Base de datos lista. Ahora debe crear el usuario administrador.";
                    PasoActual = 2;
                    PasoCambiado?.Invoke(this, 2);
                }
            }
            catch (Exception ex)
            {
                MensajeError  = $"Error al conectar al servidor PostgreSQL: {ex.Message}";
                MensajeEstado = string.Empty;
                ConexionValida = false;
            }
        }

        /// <summary>
        /// Crea el usuario administrador inicial del sistema.
        /// </summary>
        public async Task CrearAdminAsync()
        {
            MensajeError  = string.Empty;
            MensajeEstado = string.Empty;

            if (string.IsNullOrWhiteSpace(UsernameAdmin))
            {
                MensajeError = "El nombre de usuario es obligatorio.";
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordAdmin) || PasswordAdmin.Length < 6)
            {
                MensajeError = "La contraseña debe tener al menos 6 caracteres.";
                return;
            }

            if (PasswordAdmin != ConfirmarPassword)
            {
                MensajeError = "Las contraseñas no coinciden.";
                return;
            }

            try
            {
                var opciones = new DbContextOptionsBuilder<AutoCitaDbContext>()
                    .UseNpgsql(CadenaConexion)
                    .Options;

                await using var context = new AutoCitaDbContext(opciones);
                var usuarioRepo = new UsuarioRepositorio(context);

                var admin = new Usuario
                {
                    Username = UsernameAdmin,
                    Rol      = RolUsuario.Administrador,
                    Activo   = true
                };

                var comando = new CrearUsuarioComando(admin, PasswordAdmin, usuarioRepo);
                await comando.EjecutarAsync();

                MensajeEstado = "¡Usuario administrador creado exitosamente!";
                PasoActual    = 3;
                OnboardingCompletado?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al crear el administrador: {ex.Message}";
            }
        }
    }
}
