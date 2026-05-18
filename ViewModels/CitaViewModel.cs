using AutoCita.Enums;
using AutoCita.Helpers;
using AutoCita.Models;
using AutoCita.Repositories;
using AutoCita.Services;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para el flujo completo de gestión de citas:
    /// agendamiento, cancelación y reprogramación.
    /// </summary>
    public class CitaViewModel : BaseViewModel
    {
        private readonly CitaServicio      _citaServicio;
        private readonly SedeRepositorio   _sedeRepositorio;

        // ─── Campos de respaldo ────────────────────────────────────────────────
        private int       _citaId             = 0;
        private int       _vehiculoId         = 0;
        private string    _placaVehiculo      = string.Empty;
        private string    _nombreCliente      = string.Empty;
        private int       _sedeId             = 0;
        private DateTime  _fecha              = DateTime.Now.Date.AddDays(1);
        private TimeSpan  _hora               = new TimeSpan(8, 0, 0);
        private string    _motivo             = string.Empty;
        private string    _observaciones      = string.Empty;
        private EstadoCita _estado            = EstadoCita.Programada;
        private string    _mensajeError       = string.Empty;
        private string    _mensajeExito       = string.Empty;
        private string    _mensajeDisponibilidad = string.Empty;
        private bool      _hayDisponibilidad  = true;
        private bool      _cargando           = false;
        private List<Sede> _sedes             = [];

        /// <summary>
        /// Constructor del CitaViewModel.
        /// </summary>
        /// <param name="citaServicio">Servicio de gestión de citas.</param>
        /// <param name="sedeRepositorio">Repositorio de sedes para cargar el listado.</param>
        public CitaViewModel(CitaServicio citaServicio, SedeRepositorio sedeRepositorio)
        {
            _citaServicio    = citaServicio;
            _sedeRepositorio = sedeRepositorio;
        }

        // ─── Propiedades ───────────────────────────────────────────────────────

        /// <summary>Identificador de la cita (0 si es nueva).</summary>
        public int CitaId { get => _citaId; set => SetProperty(ref _citaId, value); }

        /// <summary>Identificador del vehículo asociado.</summary>
        public int VehiculoId { get => _vehiculoId; set => SetProperty(ref _vehiculoId, value); }

        /// <summary>Placa del vehículo (solo lectura en pantalla).</summary>
        public string PlacaVehiculo { get => _placaVehiculo; set => SetProperty(ref _placaVehiculo, value); }

        /// <summary>Nombre del cliente propietario (solo lectura en pantalla).</summary>
        public string NombreCliente { get => _nombreCliente; set => SetProperty(ref _nombreCliente, value); }

        /// <summary>Identificador de la sede seleccionada.</summary>
        public int SedeId { get => _sedeId; set => SetProperty(ref _sedeId, value); }

        /// <summary>Fecha seleccionada para la cita.</summary>
        public DateTime Fecha { get => _fecha; set => SetProperty(ref _fecha, value); }

        /// <summary>Hora seleccionada para la cita.</summary>
        public TimeSpan Hora { get => _hora; set => SetProperty(ref _hora, value); }

        /// <summary>Fecha y hora combinadas de la cita.</summary>
        public DateTime FechaHora => Fecha.Date.Add(Hora);

        /// <summary>Motivo del ingreso al taller.</summary>
        public string Motivo { get => _motivo; set => SetProperty(ref _motivo, value); }

        /// <summary>Observaciones adicionales (opcional).</summary>
        public string Observaciones { get => _observaciones; set => SetProperty(ref _observaciones, value); }

        /// <summary>Estado actual de la cita.</summary>
        public EstadoCita Estado { get => _estado; set => SetProperty(ref _estado, value); }

        /// <summary>Mensaje de error del formulario.</summary>
        public string MensajeError { get => _mensajeError; set => SetProperty(ref _mensajeError, value); }

        /// <summary>Mensaje de éxito tras la operación.</summary>
        public string MensajeExito { get => _mensajeExito; set => SetProperty(ref _mensajeExito, value); }

        /// <summary>Mensaje del resultado de la verificación de disponibilidad.</summary>
        public string MensajeDisponibilidad
        {
            get => _mensajeDisponibilidad;
            set => SetProperty(ref _mensajeDisponibilidad, value);
        }

        /// <summary>Indica si el horario seleccionado está disponible (RF-02, RN-02).</summary>
        public bool HayDisponibilidad
        {
            get => _hayDisponibilidad;
            set => SetProperty(ref _hayDisponibilidad, value);
        }

        /// <summary>Indica si hay una operación en progreso.</summary>
        public bool Cargando { get => _cargando; set => SetProperty(ref _cargando, value); }

        /// <summary>Lista de sedes activas para el ComboBox.</summary>
        public List<Sede> Sedes { get => _sedes; set => SetProperty(ref _sedes, value); }

        // ─── Eventos ───────────────────────────────────────────────────────────

        /// <summary>Se dispara cuando la cita fue guardada, cancelada o reprogramada exitosamente.</summary>
        public event EventHandler? OperacionExitosa;

        // ─── Métodos ───────────────────────────────────────────────────────────

        /// <summary>
        /// Carga las sedes activas desde la base de datos.
        /// </summary>
        public async Task CargarSedesAsync()
        {
            Sedes = await _sedeRepositorio.ObtenerActivasAsync();
        }

        /// <summary>
        /// Carga los datos de una cita existente en el formulario para edición.
        /// </summary>
        /// <param name="cita">Cita a cargar.</param>
        public void CargarCita(Cita cita)
        {
            CitaId        = cita.Id;
            VehiculoId    = cita.VehiculoId;
            PlacaVehiculo = cita.Vehiculo?.Placa ?? string.Empty;
            NombreCliente = cita.Vehiculo?.Cliente?.NombreCompleto ?? string.Empty;
            SedeId        = cita.SedeId;
            var fechaLocal = cita.FechaHora.ToLocalTime();
            Fecha         = fechaLocal.Date;
            Hora          = fechaLocal.TimeOfDay;
            Motivo        = cita.Motivo;
            Observaciones = cita.Observaciones ?? string.Empty;
            Estado        = cita.Estado;
        }

        /// <summary>
        /// Verifica la disponibilidad del horario seleccionado sin persistir la cita (RF-02, RN-02).
        /// </summary>
        public async Task VerificarDisponibilidadAsync()
        {
            if (VehiculoId <= 0 || SedeId <= 0) return;

            var citaProvisional = new Cita
            {
                Id         = CitaId,
                VehiculoId = VehiculoId,
                SedeId     = SedeId,
                FechaHora  = DateTime.SpecifyKind(FechaHora, DateTimeKind.Local).ToUniversalTime()
            };

            var (disponible, mensaje) = await _citaServicio.ObtenerDisponibilidadAsync(citaProvisional);
            HayDisponibilidad      = disponible;
            MensajeDisponibilidad  = mensaje;
        }

        /// <summary>
        /// Agenda una nueva cita aplicando todas las validaciones de negocio.
        /// </summary>
        public async Task GuardarAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            if (!ValidarFormulario()) return;

            Cargando = true;
            try
            {
                var cita = new Cita
                {
                    VehiculoId    = VehiculoId,
                    SedeId        = SedeId,
                    UsuarioId     = SesionActual.Instancia.UsuarioLogueado!.Id,
                    FechaHora     = DateTime.SpecifyKind(FechaHora, DateTimeKind.Local).ToUniversalTime(),
                    Motivo        = Motivo.Trim(),
                    Observaciones = string.IsNullOrWhiteSpace(Observaciones) ? null : Observaciones.Trim(),
                    Estado        = EstadoCita.Programada
                };

                await _citaServicio.AgendarAsync(cita);
                MensajeExito = "Cita agendada exitosamente.";
                OperacionExitosa?.Invoke(this, EventArgs.Empty);
            }
            catch (InvalidOperationException ex)
            {
                MensajeError = ex.Message;
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al agendar la cita: {ex.Message}";
            }
            finally
            {
                Cargando = false;
            }
        }

        /// <summary>
        /// Cancela la cita actual.
        /// </summary>
        public async Task CancelarCitaAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            if (CitaId <= 0)
            {
                MensajeError = "No hay una cita seleccionada para cancelar.";
                return;
            }

            Cargando = true;
            try
            {
                await _citaServicio.CancelarAsync(CitaId);
                MensajeExito = "Cita cancelada exitosamente.";
                OperacionExitosa?.Invoke(this, EventArgs.Empty);
            }
            catch (InvalidOperationException ex)
            {
                MensajeError = ex.Message;
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cancelar la cita: {ex.Message}";
            }
            finally
            {
                Cargando = false;
            }
        }

        /// <summary>
        /// Reprograma la cita actual con la nueva fecha/hora y sede seleccionadas.
        /// </summary>
        public async Task ReprogramarCitaAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            if (CitaId <= 0)
            {
                MensajeError = "No hay una cita seleccionada para reprogramar.";
                return;
            }

            if (!ValidarFormulario()) return;

            Cargando = true;
            try
            {
                await _citaServicio.ReprogramarAsync(CitaId, DateTime.SpecifyKind(FechaHora, DateTimeKind.Local).ToUniversalTime(), SedeId);
                MensajeExito = "Cita reprogramada exitosamente.";
                OperacionExitosa?.Invoke(this, EventArgs.Empty);
            }
            catch (InvalidOperationException ex)
            {
                MensajeError = ex.Message;
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al reprogramar la cita: {ex.Message}";
            }
            finally
            {
                Cargando = false;
            }
        }

        /// <summary>
        /// Valida los campos obligatorios del formulario de cita.
        /// </summary>
        /// <returns>True si es válido, False si hay errores.</returns>
        private bool ValidarFormulario()
        {
            if (VehiculoId <= 0)
            {
                MensajeError = "Debe seleccionar un vehículo.";
                return false;
            }
            if (SedeId <= 0)
            {
                MensajeError = "Debe seleccionar una sede.";
                return false;
            }
            if (FechaHora <= DateTime.Now)
            {
                MensajeError = "La fecha y hora deben ser posteriores a la hora actual.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Motivo))
            {
                MensajeError = "El motivo de ingreso es obligatorio.";
                return false;
            }
            return true;
        }
    }
}
