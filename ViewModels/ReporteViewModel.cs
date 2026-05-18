using AutoCita.Models;
using AutoCita.Repositories;
using AutoCita.Services;
using AutoCita.Strategies;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para la pantalla de generación de reportes del sistema.
    /// Aplica el patrón Strategy para seleccionar el tipo de reporte en tiempo de ejecución.
    /// </summary>
    public class ReporteViewModel : BaseViewModel
    {
        private readonly ReporteServicio            _reporteServicio;
        private readonly CitaRepositorio            _citaRepositorio;
        private readonly UsuarioRepositorio         _usuarioRepositorio;
        private readonly SedeRepositorio            _sedeRepositorio;
        private readonly MetaProductividadRepositorio _metaRepositorio;

        private DateTime      _fechaDesde         = DateTime.Now.Date;
        private DateTime      _fechaHasta         = DateTime.Now.Date;
        private int?          _sedeId             = null;
        private int           _tipoReporteIndice  = 0;
        private List<object>  _resultados         = [];
        private List<Sede>    _sedes              = [];
        private string        _mensajeError       = string.Empty;
        private string        _tituloReporte      = string.Empty;
        private bool          _cargando           = false;

        /// <summary>
        /// Constructor del ReporteViewModel.
        /// </summary>
        public ReporteViewModel(ReporteServicio reporteServicio, CitaRepositorio citaRepositorio,
            UsuarioRepositorio usuarioRepositorio, SedeRepositorio sedeRepositorio,
            MetaProductividadRepositorio metaRepositorio)
        {
            _reporteServicio    = reporteServicio;
            _citaRepositorio    = citaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _sedeRepositorio    = sedeRepositorio;
            _metaRepositorio    = metaRepositorio;
        }

        /// <summary>Fecha de inicio del rango del reporte.</summary>
        public DateTime FechaDesde { get => _fechaDesde; set => SetProperty(ref _fechaDesde, value); }

        /// <summary>Fecha de fin del rango del reporte.</summary>
        public DateTime FechaHasta { get => _fechaHasta; set => SetProperty(ref _fechaHasta, value); }

        /// <summary>Identificador de sede para filtrar (nullable = global).</summary>
        public int? SedeId { get => _sedeId; set => SetProperty(ref _sedeId, value); }

        /// <summary>
        /// Índice del tipo de reporte seleccionado en el ComboBox:
        /// 0=Citas por agente, 1=Productividad, 2=Usuarios, 3=Sedes.
        /// </summary>
        public int TipoReporteIndice
        {
            get => _tipoReporteIndice;
            set => SetProperty(ref _tipoReporteIndice, value);
        }

        /// <summary>Resultados del reporte generado.</summary>
        public List<object> Resultados { get => _resultados; set => SetProperty(ref _resultados, value); }

        /// <summary>Lista de sedes activas para el filtro.</summary>
        public List<Sede> Sedes { get => _sedes; set => SetProperty(ref _sedes, value); }

        /// <summary>Mensaje de error al generar el reporte.</summary>
        public string MensajeError { get => _mensajeError; set => SetProperty(ref _mensajeError, value); }

        /// <summary>Título del reporte generado.</summary>
        public string TituloReporte { get => _tituloReporte; set => SetProperty(ref _tituloReporte, value); }

        /// <summary>Indica si hay una operación en progreso.</summary>
        public bool Cargando { get => _cargando; set => SetProperty(ref _cargando, value); }

        /// <summary>Nombres de los tipos de reporte disponibles para el ComboBox.</summary>
        public List<string> TiposReporte =>
        [
            "Citas agendadas por agente",
            "Productividad de agentes vs meta",
            "Consolidado de usuarios",
            "Listado de sedes"
        ];

        /// <summary>
        /// Carga las sedes activas disponibles para el filtro.
        /// </summary>
        public async Task CargarSedesAsync()
        {
            Sedes = await _sedeRepositorio.ObtenerActivasAsync();
        }

        /// <summary>
        /// Genera el reporte seleccionado usando el patrón Strategy.
        /// </summary>
        public async Task GenerarAsync()
        {
            MensajeError = string.Empty;
            Resultados   = [];

            if (FechaDesde > FechaHasta)
            {
                MensajeError = "La fecha de inicio no puede ser mayor a la fecha de fin.";
                return;
            }

            Cargando = true;
            try
            {
                IReporteStrategy estrategia = TipoReporteIndice switch
                {
                    0 => new ReporteCitasPorAgenteStrategy(_citaRepositorio),
                    1 => new ReporteProductividadStrategy(_citaRepositorio, _metaRepositorio),
                    2 => new ReporteUsuariosStrategy(_usuarioRepositorio),
                    3 => new ReporteSedesStrategy(_sedeRepositorio),
                    _ => new ReporteCitasPorAgenteStrategy(_citaRepositorio)
                };

                _reporteServicio.EstablecerEstrategia(estrategia);
                TituloReporte = _reporteServicio.NombreReporteActual;
                Resultados    = await _reporteServicio.GenerarAsync(FechaDesde, FechaHasta, SedeId);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al generar el reporte: {ex.Message}";
            }
            finally
            {
                Cargando = false;
            }
        }
    }
}
