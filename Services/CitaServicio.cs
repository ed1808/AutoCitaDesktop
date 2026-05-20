using AutoCita.Commands;
using AutoCita.Models;
using AutoCita.Repositories;
using AutoCita.Strategies;

namespace AutoCita.Services
{
    /// <summary>
    /// Servicio principal de gestión de citas. Orquesta Commands y Strategies para
    /// agendar, cancelar y reprogramar citas cumpliendo las reglas de negocio.
    /// </summary>
    public class CitaServicio
    {
        private readonly CitaRepositorio _citaRepositorio;
        private readonly IDisponibilidadStrategy _disponibilidadStrategy;

        /// <summary>
        /// Constructor del servicio de citas.
        /// </summary>
        /// <param name="citaRepositorio">Repositorio de citas.</param>
        /// <param name="disponibilidadStrategy">Estrategia de validación de disponibilidad.</param>
        public CitaServicio(CitaRepositorio citaRepositorio, IDisponibilidadStrategy disponibilidadStrategy)
        {
            _citaRepositorio = citaRepositorio;
            _disponibilidadStrategy = disponibilidadStrategy;
        }

        /// <summary>
        /// Agenda una nueva cita ejecutando las validaciones RN-01 y RN-02.
        /// </summary>
        /// <param name="cita">Datos de la cita a agendar.</param>
        /// <exception cref="InvalidOperationException">Si no se puede agendar por reglas de negocio.</exception>
        public async Task AgendarAsync(Cita cita)
        {
            var comando = new CrearCitaComando(cita, _citaRepositorio, _disponibilidadStrategy);
            await comando.EjecutarAsync();
        }

        /// <summary>
        /// Cancela una cita existente, liberando su espacio en la agenda.
        /// </summary>
        /// <param name="citaId">Identificador de la cita a cancelar.</param>
        /// <exception cref="InvalidOperationException">Si la cita no existe o ya fue cancelada.</exception>
        public async Task CancelarAsync(int citaId)
        {
            var comando = new CancelarCitaComando(citaId, _citaRepositorio);
            await comando.EjecutarAsync();
        }

        /// <summary>
        /// Reprograma una cita a una nueva fecha/hora y/o sede, validando RN-01 y RN-02.
        /// </summary>
        /// <param name="citaId">Identificador de la cita a reprogramar.</param>
        /// <param name="nuevaFechaHora">Nueva fecha y hora para la cita.</param>
        /// <param name="nuevaSedeId">Identificador de la nueva sede.</param>
        /// <exception cref="InvalidOperationException">Si la nueva fecha es inválida o existe conflicto.</exception>
        public async Task ReprogramarAsync(int citaId, DateTime nuevaFechaHora, int nuevaSedeId)
        {
            var comando = new ReprogramarCitaComando(citaId, nuevaFechaHora, nuevaSedeId,
                _citaRepositorio, _disponibilidadStrategy);
            await comando.EjecutarAsync();
        }

        /// <summary>
        /// Verifica si hay disponibilidad para agendar una cita sin persistirla.
        /// </summary>
        /// <param name="citaProvisional">Cita provisional a evaluar.</param>
        /// <returns>True si hay disponibilidad, False si existe conflicto.</returns>
        public async Task<(bool disponible, string mensaje)> ObtenerDisponibilidadAsync(Cita citaProvisional)
        {
            if (DateTime.SpecifyKind(citaProvisional.FechaHora, DateTimeKind.Local).ToUniversalTime() <= DateTime.UtcNow)
                return (false, "La fecha y hora seleccionadas son anteriores a la fecha y hora actual.");

            var citasExistentes = await _citaRepositorio.ObtenerPorVehiculoYFechaAsync(
                citaProvisional.VehiculoId, citaProvisional.FechaHora);

            bool disponible = _disponibilidadStrategy.HayDisponibilidad(citaProvisional, citasExistentes);
            string mensaje = disponible ? "Horario disponible." : _disponibilidadStrategy.ObtenerMensajeConflicto();

            return (disponible, mensaje);
        }

        /// <summary>
        /// Obtiene todas las citas en un rango de fechas.
        /// </summary>
        /// <param name="desde">Fecha de inicio.</param>
        /// <param name="hasta">Fecha de fin.</param>
        /// <returns>Lista de citas en el rango.</returns>
        public async Task<List<Cita>> ObtenerPorRangoFechasAsync(DateTime desde, DateTime hasta)
        {
            return await _citaRepositorio.ObtenerPorRangoFechasAsync(desde, hasta);
        }
    }
}
