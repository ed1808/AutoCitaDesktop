using AutoCita.Enums;
using AutoCita.Repositories;
using AutoCita.Strategies;

namespace AutoCita.Commands
{
    /// <summary>
    /// Comando para reprogramar una cita existente, validando las reglas RN-01 y RN-02.
    /// </summary>
    public class ReprogramarCitaComando : IComando
    {
        private readonly int _citaId;
        private readonly DateTime _nuevaFechaHora;
        private readonly int _nuevaSedeId;
        private readonly CitaRepositorio _citaRepositorio;
        private readonly IDisponibilidadStrategy _disponibilidadStrategy;

        /// <summary>
        /// Constructor del comando de reprogramación de cita.
        /// </summary>
        /// <param name="citaId">Identificador de la cita a reprogramar.</param>
        /// <param name="nuevaFechaHora">Nueva fecha y hora para la cita.</param>
        /// <param name="nuevaSedeId">Identificador de la nueva sede (puede ser la misma).</param>
        /// <param name="citaRepositorio">Repositorio de citas.</param>
        /// <param name="disponibilidadStrategy">Estrategia de validación de disponibilidad.</param>
        public ReprogramarCitaComando(int citaId, DateTime nuevaFechaHora, int nuevaSedeId,
            CitaRepositorio citaRepositorio, IDisponibilidadStrategy disponibilidadStrategy)
        {
            _citaId = citaId;
            _nuevaFechaHora = nuevaFechaHora;
            _nuevaSedeId = nuevaSedeId;
            _citaRepositorio = citaRepositorio;
            _disponibilidadStrategy = disponibilidadStrategy;
        }

        /// <summary>
        /// Ejecuta la reprogramación validando RN-01 (no pasado) y RN-02 (exclusividad del vehículo).
        /// </summary>
        /// <exception cref="InvalidOperationException">Si la fecha es pasada, hay conflicto de agenda o la cita no existe.</exception>
        public async Task EjecutarAsync()
        {
            // RN-01: La nueva fecha/hora no puede ser en el pasado
            if (_nuevaFechaHora <= DateTime.UtcNow)
                throw new InvalidOperationException("No se puede reprogramar una cita a una fecha u hora anterior a la actual.");

            var cita = await _citaRepositorio.ObtenerPorIdAsync(_citaId)
                ?? throw new InvalidOperationException($"No se encontró la cita con Id {_citaId}.");

            if (cita.Estado == EstadoCita.Cancelada)
                throw new InvalidOperationException("No se puede reprogramar una cita cancelada.");

            // Preparar cita provisional para validar RN-02 (excluye la propia cita)
            var citaProvisional = new Models.Cita
            {
                Id         = cita.Id,
                VehiculoId = cita.VehiculoId,
                SedeId     = _nuevaSedeId,
                FechaHora  = _nuevaFechaHora
            };

            // RN-02: Validar exclusividad del vehículo en la nueva fecha/hora
            var citasExistentes = await _citaRepositorio.ObtenerPorVehiculoYFechaAsync(cita.VehiculoId, _nuevaFechaHora);

            if (!_disponibilidadStrategy.HayDisponibilidad(citaProvisional, citasExistentes))
                throw new InvalidOperationException(_disponibilidadStrategy.ObtenerMensajeConflicto());

            cita.FechaHora  = _nuevaFechaHora;
            cita.SedeId     = _nuevaSedeId;
            cita.Estado     = EstadoCita.Reprogramada;
            cita.RecordatorioRealizado = false;

            _citaRepositorio.Actualizar(cita);
            await _citaRepositorio.GuardarAsync();
        }
    }
}
