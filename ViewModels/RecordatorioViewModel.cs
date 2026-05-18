using AutoCita.Models;
using AutoCita.Services;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para la pantalla de recordatorios de llamadas del día anterior a la cita (RN-05).
    /// </summary>
    public class RecordatorioViewModel : BaseViewModel
    {
        private readonly RecordatorioServicio _recordatorioServicio;

        private List<Cita> _citas           = [];
        private string     _mensajeError    = string.Empty;
        private string     _mensajeExito    = string.Empty;
        private int        _totalPendientes = 0;

        /// <summary>
        /// Constructor del RecordatorioViewModel.
        /// </summary>
        /// <param name="recordatorioServicio">Servicio de recordatorios.</param>
        public RecordatorioViewModel(RecordatorioServicio recordatorioServicio)
        {
            _recordatorioServicio = recordatorioServicio;
        }

        /// <summary>
        /// Lista de citas programadas para mañana cuyo recordatorio no ha sido realizado.
        /// </summary>
        public List<Cita> Citas
        {
            get => _citas;
            set
            {
                SetProperty(ref _citas, value);
                TotalPendientes = value.Count;
            }
        }

        /// <summary>
        /// Mensaje de error del módulo de recordatorios.
        /// </summary>
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        /// <summary>
        /// Mensaje de éxito tras marcar una llamada como realizada.
        /// </summary>
        public string MensajeExito
        {
            get => _mensajeExito;
            set => SetProperty(ref _mensajeExito, value);
        }

        /// <summary>
        /// Total de llamadas de recordatorio pendientes (para mostrar badge de alerta).
        /// </summary>
        public int TotalPendientes
        {
            get => _totalPendientes;
            set => SetProperty(ref _totalPendientes, value);
        }

        /// <summary>
        /// Se dispara cuando cambia el total de citas pendientes (para actualizar el badge).
        /// </summary>
        public event EventHandler<int>? TotalPendientesCambiado;

        /// <summary>
        /// Carga todas las citas del día siguiente sin recordatorio realizado.
        /// </summary>
        public async Task CargarAsync()
        {
            MensajeError = string.Empty;
            try
            {
                Citas = await _recordatorioServicio.ObtenerCitasParaRecordatorioAsync();
                TotalPendientesCambiado?.Invoke(this, TotalPendientes);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar recordatorios: {ex.Message}";
            }
        }

        /// <summary>
        /// Marca como realizada la llamada de recordatorio para la cita indicada.
        /// </summary>
        /// <param name="citaId">Identificador de la cita.</param>
        public async Task MarcarLlamadaRealizadaAsync(int citaId)
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            try
            {
                await _recordatorioServicio.MarcarRecordatorioRealizadoAsync(citaId);
                MensajeExito = "Llamada de recordatorio marcada como realizada.";
                await CargarAsync();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }
    }
}
