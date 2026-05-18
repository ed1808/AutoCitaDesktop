using AutoCita.Data;
using AutoCita.Enums;
using AutoCita.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Cita.
    /// </summary>
    public class CitaRepositorio : IRepositorio<Cita>
    {
        private readonly AutoCitaDbContext _context;

        /// <summary>
        /// Constructor del repositorio de citas.
        /// </summary>
        /// <param name="context">Contexto de base de datos.</param>
        public CitaRepositorio(AutoCitaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las citas.
        /// </summary>
        public async Task<List<Cita>> ObtenerTodosAsync()
        {
            return await _context.Citas
                .Include(c => c.Vehiculo).ThenInclude(v => v!.Cliente)
                .Include(c => c.Sede)
                .Include(c => c.Usuario)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una cita por su identificador.
        /// </summary>
        public async Task<Cita?> ObtenerPorIdAsync(int id)
        {
            return await _context.Citas
                .Include(c => c.Vehiculo).ThenInclude(v => v!.Cliente)
                .Include(c => c.Sede)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Agrega una nueva cita.
        /// </summary>
        public async Task AgregarAsync(Cita entidad)
        {
            await _context.Citas.AddAsync(entidad);
        }

        /// <summary>
        /// Actualiza una cita existente.
        /// </summary>
        public void Actualizar(Cita entidad)
        {
            _context.Citas.Update(entidad);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene todas las citas activas de un vehículo en una fecha específica.
        /// </summary>
        /// <param name="vehiculoId">Identificador del vehículo.</param>
        /// <param name="fecha">Fecha a consultar.</param>
        /// <returns>Lista de citas activas del vehículo en la fecha.</returns>
        public async Task<List<Cita>> ObtenerPorVehiculoYFechaAsync(int vehiculoId, DateTime fecha)
        {
            var fechaSoloFecha = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);
            return await _context.Citas
                .Include(c => c.Sede)
                .Where(c => c.VehiculoId == vehiculoId
                    && c.FechaHora.Date == fechaSoloFecha
                    && c.Estado != EstadoCita.Cancelada)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todas las citas programadas para el día siguiente (Hoy + 1).
        /// </summary>
        /// <returns>Lista de citas de mañana sin recordatorio realizado.</returns>
        public async Task<List<Cita>> ObtenerCitasMananaAsync()
        {
            var manana = DateTime.SpecifyKind(DateTime.Now.Date.AddDays(1), DateTimeKind.Utc);
            return await _context.Citas
                .Include(c => c.Vehiculo).ThenInclude(v => v!.Cliente)
                .Include(c => c.Sede)
                .Where(c => c.FechaHora.Date == manana
                    && c.Estado == EstadoCita.Programada
                    && !c.RecordatorioRealizado)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todas las citas gestionadas por un agente específico.
        /// </summary>
        /// <param name="usuarioId">Identificador del agente/usuario.</param>
        /// <returns>Lista de citas del agente.</returns>
        public async Task<List<Cita>> ObtenerPorAgenteAsync(int usuarioId)
        {
            return await _context.Citas
                .Include(c => c.Vehiculo).ThenInclude(v => v!.Cliente)
                .Include(c => c.Sede)
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todas las citas en un rango de fechas.
        /// </summary>
        /// <param name="desde">Fecha de inicio del rango.</param>
        /// <param name="hasta">Fecha de fin del rango.</param>
        /// <returns>Lista de citas en el rango.</returns>
        public async Task<List<Cita>> ObtenerPorRangoFechasAsync(DateTime desde, DateTime hasta)
        {
            var desdeUtc = DateTime.SpecifyKind(desde.Date, DateTimeKind.Utc);
            var hastaUtc = DateTime.SpecifyKind(hasta.Date, DateTimeKind.Utc);
            return await _context.Citas
                .Include(c => c.Vehiculo).ThenInclude(v => v!.Cliente)
                .Include(c => c.Sede)
                .Include(c => c.Usuario)
                .Where(c => c.FechaHora.Date >= desdeUtc && c.FechaHora.Date <= hastaUtc)
                .ToListAsync();
        }
    }
}
