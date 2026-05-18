using AutoCita.Enums;

namespace AutoCita.Models
{
    /// <summary>
    /// Representa un usuario del sistema AutoCita.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario único (máximo 50 caracteres).
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña encriptada con BCrypt.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario en el sistema.
        /// </summary>
        public RolUsuario Rol { get; set; }

        /// <summary>
        /// Identificador de la sede asignada al usuario (nullable, nulo si es global).
        /// </summary>
        public int? SedeId { get; set; }

        /// <summary>
        /// Indica si el usuario está activo (borrado lógico).
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Navegación a la sede asignada.
        /// </summary>
        public Sede? Sede { get; set; }
    }
}
