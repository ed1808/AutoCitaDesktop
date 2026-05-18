namespace AutoCita.Models
{
    /// <summary>
    /// Representa una meta de productividad para los agentes del contact center.
    /// </summary>
    public class MetaProductividad
    {
        /// <summary>
        /// Identificador único de la meta.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Cantidad mínima de llamadas diarias que deben gestionar los agentes.
        /// </summary>
        public int MetaLlamadas { get; set; }

        /// <summary>
        /// Fecha de inicio de vigencia de la meta.
        /// </summary>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Fecha de fin de vigencia de la meta (nullable, si es nula es la meta vigente actual).
        /// </summary>
        public DateTime? FechaFin { get; set; }

        /// <summary>
        /// Identificador de la sede a la que aplica la meta (nullable, si es nulo aplica globalmente).
        /// </summary>
        public int? SedeId { get; set; }

        /// <summary>
        /// Navegación a la sede asociada (opcional).
        /// </summary>
        public Sede? Sede { get; set; }
    }
}
