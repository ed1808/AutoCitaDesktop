using AutoCita.Enums;

namespace AutoCita.Models
{
    /// <summary>
    /// Representa un vehículo registrado en el taller de mecánica automotriz.
    /// </summary>
    public class Vehiculo
    {
        /// <summary>
        /// Identificador único del vehículo.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Placa del vehículo (máximo 10 caracteres, único).
        /// </summary>
        public string Placa { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de vehículo (Moto, Automóvil, Camioneta, Camión).
        /// </summary>
        public TipoVehiculo TipoVehiculo { get; set; }

        /// <summary>
        /// Identificador del cliente propietario del vehículo.
        /// </summary>
        public int ClienteId { get; set; }

        /// <summary>
        /// Indica si el vehículo está activo (borrado lógico).
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Navegación al cliente propietario del vehículo.
        /// </summary>
        public Cliente? Cliente { get; set; }
    }
}
