using AutoCita.Enums;
using AutoCita.Models;

namespace AutoCita.Strategies
{
    /// <summary>
    /// Estrategia de disponibilidad de vehículo que implementa las reglas de negocio RN-02.
    /// Valida que el vehículo no tenga cita el mismo día en otra sede ni colisión de horario.
    /// </summary>
    public class DisponibilidadVehiculoStrategy : IDisponibilidadStrategy
    {
        private string _mensajeConflicto = string.Empty;

        /// <summary>
        /// Verifica disponibilidad para el vehículo aplicando las reglas RN-02:
        /// - Un vehículo no puede tener citas en distintas sedes el mismo día.
        /// - Un vehículo no puede tener dos citas a la misma hora.
        /// </summary>
        /// <param name="citaNueva">Cita que se desea agendar o reprogramar.</param>
        /// <param name="citasExistentes">Citas activas del mismo vehículo.</param>
        /// <returns>True si hay disponibilidad, False si existe conflicto.</returns>
        /// <summary>
        /// Trunca un DateTime a minutos, eliminando segundos y fracciones residuales.
        /// Necesario porque EF Core puede persistir milisegundos al guardar DateTime.Now.
        /// </summary>
        private static DateTime TruncarAMinutos(DateTime dt) =>
            new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, dt.Kind);

        public bool HayDisponibilidad(Cita citaNueva, IEnumerable<Cita> citasExistentes)
        {
            _mensajeConflicto = string.Empty;

            // Normalizar citaNueva.FechaHora a UTC y truncar a minutos para comparar
            // con las citas existentes (que pueden tener milisegundos residuales de la BD)
            var fechaHoraUtc = TruncarAMinutos(
                DateTime.SpecifyKind(citaNueva.FechaHora, DateTimeKind.Local).ToUniversalTime());

            // Filtrar citas activas (no canceladas) y excluir la misma cita en caso de reprogramación
            var citasActivas = citasExistentes
                .Where(c => c.Estado != EstadoCita.Cancelada && c.Id != citaNueva.Id)
                .ToList();

            // RN-02 Regla 1: mismo día, diferente sede
            // Se normaliza c.FechaHora a Utc porque EF Core + SQLite devuelve DateTimeKind.Unspecified
            var conflictoSede = citasActivas.FirstOrDefault(c =>
                DateTime.SpecifyKind(c.FechaHora, DateTimeKind.Utc).Date == fechaHoraUtc.Date &&
                c.SedeId != citaNueva.SedeId);

            if (conflictoSede != null)
            {
                _mensajeConflicto = $"El vehículo ya tiene una cita el {citaNueva.FechaHora:dd/MM/yyyy} " +
                                    $"en otra sede. No se pueden agendar citas en sedes diferentes el mismo día.";
                return false;
            }

            // RN-02 Regla 2: misma hora exacta (colisión de horario)
            // Se normaliza c.FechaHora a Utc y se trunca a minutos para evitar fallos
            // por milisegundos residuales almacenados en la base de datos
            var conflictoHora = citasActivas.FirstOrDefault(c =>
                TruncarAMinutos(DateTime.SpecifyKind(c.FechaHora, DateTimeKind.Utc)) == fechaHoraUtc);

            if (conflictoHora != null)
            {
                _mensajeConflicto = $"El vehículo ya tiene una cita programada para el " +
                                    $"{citaNueva.FechaHora:dd/MM/yyyy} a las {citaNueva.FechaHora:HH:mm}. " +
                                    $"No se pueden agendar dos citas a la misma hora.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtiene el mensaje de conflicto detectado en la última validación.
        /// </summary>
        /// <returns>Descripción del conflicto o cadena vacía si no hay conflicto.</returns>
        public string ObtenerMensajeConflicto()
        {
            return _mensajeConflicto;
        }
    }
}
