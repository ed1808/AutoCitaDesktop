using AutoCita.Models;

namespace AutoCita.Helpers
{
    /// <summary>
    /// Singleton que mantiene la información del usuario actualmente autenticado en el sistema.
    /// </summary>
    public sealed class SesionActual
    {
        private static readonly Lazy<SesionActual> instancia = new Lazy<SesionActual>(() => new SesionActual());

        /// <summary>
        /// Obtiene la instancia única del singleton SesionActual.
        /// </summary>
        public static SesionActual Instancia => instancia.Value;

        /// <summary>
        /// Usuario actualmente autenticado en el sistema.
        /// </summary>
        public Usuario? UsuarioLogueado { get; set; }

        /// <summary>
        /// Constructor privado para evitar instanciación externa.
        /// </summary>
        private SesionActual()
        {
        }

        /// <summary>
        /// Cierra la sesión actual limpiando el usuario logueado.
        /// </summary>
        public void CerrarSesion()
        {
            UsuarioLogueado = null;
        }
    }
}
