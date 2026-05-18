using AutoCita.Enums;

namespace AutoCita.Models
{
    /// <summary>
    /// Representa una cita programada en el taller de mecánica automotriz.
    /// </summary>
    public class Cita
    {
        /// <summary>
        /// Identificador único de la cita.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador del vehículo asociado a la cita.
        /// </summary>
        public int VehiculoId { get; set; }

        /// <summary>
        /// Identificador de la sede donde se realizará la cita.
        /// </summary>
        public int SedeId { get; set; }

        /// <summary>
        /// Identificador del usuario (agente o director) que creó o modificó la cita.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Fecha y hora programada para la cita (no puede ser en el pasado).
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// Motivo del ingreso del vehículo al taller (máximo 250 caracteres).
        /// </summary>
        public string Motivo { get; set; } = string.Empty;

        /// <summary>
        /// Observaciones adicionales sobre la cita (opcional).
        /// </summary>
        public string? Observaciones { get; set; }

        /// <summary>
        /// Estado actual de la cita.
        /// </summary>
        public EstadoCita Estado { get; set; } = EstadoCita.Programada;

        /// <summary>
        /// Indica si se realizó la llamada de recordatorio el día anterior.
        /// </summary>
        public bool RecordatorioRealizado { get; set; } = false;

        /// <summary>
        /// Navegación al vehículo asociado.
        /// </summary>
        public Vehiculo? Vehiculo { get; set; }

        /// <summary>
        /// Navegación a la sede donde se realizará la cita.
        /// </summary>
        public Sede? Sede { get; set; }

        /// <summary>
        /// Navegación al usuario que creó o modificó la cita.
        /// </summary>
        public Usuario? Usuario { get; set; }
    }
}
