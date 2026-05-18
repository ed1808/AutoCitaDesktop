using AutoCita.Enums;
using AutoCita.Repositories;

namespace AutoCita.Commands
{
    /// <summary>
    /// Comando para cancelar una cita existente, liberando el espacio en la agenda.
    /// </summary>
    public class CancelarCitaComando : IComando
    {
        private readonly int _citaId;
        private readonly CitaRepositorio _citaRepositorio;

        /// <summary>
        /// Constructor del comando de cancelación de cita.
        /// </summary>
        /// <param name="citaId">Identificador de la cita a cancelar.</param>
        /// <param name="citaRepositorio">Repositorio de citas.</param>
        public CancelarCitaComando(int citaId, CitaRepositorio citaRepositorio)
        {
            _citaId = citaId;
            _citaRepositorio = citaRepositorio;
        }

        /// <summary>
        /// Ejecuta la cancelación de la cita cambiando su estado a Cancelada.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si la cita no existe o ya fue cancelada.</exception>
        public async Task EjecutarAsync()
        {
            var cita = await _citaRepositorio.ObtenerPorIdAsync(_citaId)
                ?? throw new InvalidOperationException($"No se encontró la cita con Id {_citaId}.");

            if (cita.Estado == EstadoCita.Cancelada)
                throw new InvalidOperationException("La cita ya se encuentra cancelada.");

            cita.Estado = EstadoCita.Cancelada;
            _citaRepositorio.Actualizar(cita);
            await _citaRepositorio.GuardarAsync();
        }
    }
}
