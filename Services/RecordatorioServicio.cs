using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.Services
{
    /// <summary>
    /// Servicio para la gestión de recordatorios de llamadas del día anterior a la cita (RN-05).
    /// </summary>
    public class RecordatorioServicio
    {
        private readonly CitaRepositorio _citaRepositorio;

        /// <summary>
        /// Constructor del servicio de recordatorios.
        /// </summary>
        /// <param name="citaRepositorio">Repositorio de citas.</param>
        public RecordatorioServicio(CitaRepositorio citaRepositorio)
        {
            _citaRepositorio = citaRepositorio;
        }

        /// <summary>
        /// Obtiene todas las citas programadas para el día siguiente (Hoy + 1)
        /// cuyo recordatorio aún no ha sido realizado.
        /// </summary>
        /// <returns>Lista de citas pendientes de recordatorio.</returns>
        public async Task<List<Cita>> ObtenerCitasParaRecordatorioAsync()
        {
            return await _citaRepositorio.ObtenerCitasMananaAsync();
        }

        /// <summary>
        /// Marca como realizada la llamada de recordatorio para una cita específica.
        /// </summary>
        /// <param name="citaId">Identificador de la cita.</param>
        /// <exception cref="InvalidOperationException">Si la cita no existe.</exception>
        public async Task MarcarRecordatorioRealizadoAsync(int citaId)
        {
            var cita = await _citaRepositorio.ObtenerPorIdAsync(citaId)
                ?? throw new InvalidOperationException($"No se encontró la cita con Id {citaId}.");

            cita.RecordatorioRealizado = true;
            _citaRepositorio.Actualizar(cita);
            await _citaRepositorio.GuardarAsync();
        }
    }
}
