using AutoCita.Models;
using AutoCita.Repositories;
using AutoCita.Strategies;

namespace AutoCita.Commands
{
    /// <summary>
    /// Comando para crear una nueva cita, aplicando validaciones de negocio RN-01 y RN-02.
    /// </summary>
    public class CrearCitaComando : IComando
    {
        private readonly Cita _cita;
        private readonly CitaRepositorio _citaRepositorio;
        private readonly IDisponibilidadStrategy _disponibilidadStrategy;

        /// <summary>
        /// Constructor del comando de creación de cita.
        /// </summary>
        /// <param name="cita">Cita a crear.</param>
        /// <param name="citaRepositorio">Repositorio de citas.</param>
        /// <param name="disponibilidadStrategy">Estrategia de validación de disponibilidad.</param>
        public CrearCitaComando(Cita cita, CitaRepositorio citaRepositorio, IDisponibilidadStrategy disponibilidadStrategy)
        {
            _cita = cita;
            _citaRepositorio = citaRepositorio;
            _disponibilidadStrategy = disponibilidadStrategy;
        }

        /// <summary>
        /// Ejecuta la creación de la cita validando RN-01 (no en el pasado) y RN-02 (exclusividad del vehículo).
        /// </summary>
        /// <exception cref="InvalidOperationException">Si la fecha es en el pasado o existe conflicto de agenda.</exception>
        public async Task EjecutarAsync()
        {
            // RN-01: La fecha/hora no puede ser en el pasado
            if (_cita.FechaHora <= DateTime.UtcNow)
                throw new InvalidOperationException("No se pueden agendar citas en fechas u horas anteriores a la actual.");

            // RN-02: Validar exclusividad de agenda del vehículo
            var citasExistentes = await _citaRepositorio.ObtenerPorVehiculoYFechaAsync(_cita.VehiculoId, _cita.FechaHora);

            if (!_disponibilidadStrategy.HayDisponibilidad(_cita, citasExistentes))
                throw new InvalidOperationException(_disponibilidadStrategy.ObtenerMensajeConflicto());

            await _citaRepositorio.AgregarAsync(_cita);
            await _citaRepositorio.GuardarAsync();
        }
    }
}
