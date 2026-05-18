namespace AutoCita.Models
{
    /// <summary>
    /// Representa un cliente del taller de mecánica automotriz.
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Identificador único del cliente.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Número de documento de identidad del cliente (máximo 20 caracteres, único).
        /// </summary>
        public string DocumentoIdentidad { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo del cliente (máximo 150 caracteres).
        /// </summary>
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Número de teléfono del cliente para contacto vía WhatsApp.
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora de registro del cliente en el sistema.
        /// </summary>
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
