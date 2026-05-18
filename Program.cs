using AutoCita.Data;
using AutoCita.Helpers;
using AutoCita.Repositories;
using AutoCita.Services;
using AutoCita.ViewModels;
using AutoCita.Views;
using Microsoft.EntityFrameworkCore;

namespace AutoCita
{
    /// <summary>
    /// Punto de entrada de la aplicación AutoCita.
    /// Determina si se debe mostrar el onboarding inicial o ir directamente al login.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal de la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // ── 1. Verificar si ya existe configuración ───────────────────────
            var cadenaConexion = ConfiguracionHelper.LeerCadenaConexion();

            if (string.IsNullOrWhiteSpace(cadenaConexion))
            {
                // Primera ejecución: mostrar onboarding
                var onboardingVm = new OnboardingViewModel();
                var onboarding   = new FrmOnboarding(onboardingVm);
                bool finalizado  = false;

                onboarding.OnboardingFinalizado += (_, _) => { finalizado = true; onboarding.Close(); };
                Application.Run(onboarding);

                if (!finalizado) return; // El usuario cerró sin completar

                cadenaConexion = ConfiguracionHelper.LeerCadenaConexion();
            }

            // ── 2. Verificar conexión a la base de datos ──────────────────────
            if (string.IsNullOrWhiteSpace(cadenaConexion))
            {
                MessageBox.Show("No se encontró la configuración de la base de datos.\nEjecute nuevamente para completar el onboarding.",
                    "AutoCita", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── 3. Flujo de login ─────────────────────────────────────────────
            bool loginExitoso = false;

            var opciones     = new DbContextOptionsBuilder<AutoCitaDbContext>().UseNpgsql(cadenaConexion).Options;
            var ctx          = new AutoCitaDbContext(opciones);
            var usuarioRepo  = new UsuarioRepositorio(ctx);
            var authServicio = new AuthServicio(usuarioRepo);
            var loginVm      = new LoginViewModel(authServicio);
            var login        = new FrmLogin(loginVm);

            login.LoginExitoso += (_, _) => { loginExitoso = true; login.Close(); };
            Application.Run(login);
            ctx.Dispose();

            if (!loginExitoso) return;

            // ── 4. Formulario principal ───────────────────────────────────────
            Application.Run(FrmPrincipal.Crear());
        }
    }
}
