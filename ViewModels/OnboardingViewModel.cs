using AutoCita.Commands;
using AutoCita.Data;
using AutoCita.Enums;
using AutoCita.Helpers;
using AutoCita.Models;
using AutoCita.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using System.Net.Sockets;

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

            // Validar formato de cadena de conexión antes de intentar conectar
            var errorValidacion = ConfiguracionHelper.ValidarFormatoCadenaConexion(CadenaConexion);
            if (errorValidacion != null)
            {
                MensajeError  = errorValidacion;
                MensajeEstado = string.Empty;
                return;
            }

            try
            {
                var opciones = new DbContextOptionsBuilder<AutoCitaDbContext>()
                    .UseNpgsql(CadenaConexion)
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
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
                MensajeError  = ObtenerMensajeErrorConexion(ex);
                MensajeEstado = string.Empty;
                ConexionValida = false;
            }
        }

        /// <summary>
        /// Analiza la excepción y retorna un mensaje de error descriptivo y accionable.
        /// </summary>
        private string ObtenerMensajeErrorConexion(Exception ex)
        {
            // 1. Revisar si es un problema de red/DNS (SocketException)
            var socketEx = BuscarExcepcionInterna<SocketException>(ex);
            if (socketEx != null)
            {
                return "No se pudo resolver el host o conectar al servidor. Verifique:\n" +
                       "• Que el nombre del servidor sea correcto (ej: db.abc123xyz.supabase.co)\n" +
                       "• Que tenga acceso a internet\n" +
                       "• Que el puerto sea el correcto (normalmente 5432 para Supabase)";
            }

            // 2. Revisar errores específicos de PostgreSQL
            var npgsqlEx = BuscarExcepcionInterna<NpgsqlException>(ex);
            if (npgsqlEx != null)
            {
                // Código 28P01: autenticación fallida (contraseña incorrecta)
                if (npgsqlEx.SqlState == "28P01")
                {
                    return "Contraseña incorrecta para el usuario especificado. Verifique sus credenciales.";
                }

                // Código 3D000: base de datos no existe
                if (npgsqlEx.SqlState == "3D000")
                {
                    return "La base de datos especificada no existe en el servidor. Verifique el nombre de la base de datos en la cadena de conexión.";
                }

                // Error de SSL
                if (npgsqlEx.Message.Contains("SSL", StringComparison.OrdinalIgnoreCase) ||
                    npgsqlEx.Message.Contains("certificate", StringComparison.OrdinalIgnoreCase))
                {
                    return "Error de SSL. Para conectarse a Supabase, agregue los siguientes parámetros a su cadena de conexión:\n" +
                           "SSL Mode=Require;Trust Server Certificate=true";
                }
            }

            // 3. Timeout / operación cancelada
            if (ex is TimeoutException || ex is OperationCanceledException)
            {
                return "Tiempo de espera agotado al intentar conectar. Verifique:\n" +
                       "• El host y puerto (normalmente 5432 para Supabase)\n" +
                       "• Que el servidor esté disponible\n" +
                       "• Su conexión a internet";
            }

            // 4. Mensaje genérico con detalles de la excepción
            return $"Error al conectar al servidor PostgreSQL: {ex.Message}";
        }

        /// <summary>
        /// Busca una excepción específica en la cadena de InnerException.
        /// </summary>
        private T? BuscarExcepcionInterna<T>(Exception ex) where T : Exception
        {
            var actual = ex;
            while (actual != null)
            {
                if (actual is T resultado)
                    return resultado;
                actual = actual.InnerException;
            }
            return null;
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
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
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
