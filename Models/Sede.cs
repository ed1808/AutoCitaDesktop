namespace AutoCita.Models
{
    /// <summary>
    /// Representa una sede del taller de mecánica automotriz.
    /// </summary>
    public class Sede
    {
        /// <summary>
        /// Identificador único de la sede.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la sede (máximo 100 caracteres, único).
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Dirección física de la sede.
        /// </summary>
        public string Direccion { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la sede está activa (borrado lógico).
        /// </summary>
        public bool Activo { get; set; } = true;
    }
}
