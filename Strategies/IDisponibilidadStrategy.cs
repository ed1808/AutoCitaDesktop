using AutoCita.Models;

namespace AutoCita.Strategies
{
    /// <summary>
    /// Interfaz de la estrategia para verificar disponibilidad de agenda antes de agendar una cita.
    /// </summary>
    public interface IDisponibilidadStrategy
    {
        /// <summary>
        /// Verifica si hay disponibilidad para la cita nueva dadas las citas existentes del vehículo.
        /// </summary>
        /// <param name="citaNueva">Cita que se desea agendar o reprogramar.</param>
        /// <param name="citasExistentes">Citas activas existentes del mismo vehículo.</param>
        /// <returns>True si hay disponibilidad, False si existe conflicto.</returns>
        bool HayDisponibilidad(Cita citaNueva, IEnumerable<Cita> citasExistentes);

        /// <summary>
        /// Obtiene el mensaje de conflicto cuando no hay disponibilidad.
        /// </summary>
        /// <returns>Descripción del conflicto detectado.</returns>
        string ObtenerMensajeConflicto();
    }
}
