using AutoCita.Enums;
using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para la gestión y búsqueda de vehículos.
    /// </summary>
    public class VehiculoViewModel : BaseViewModel
    {
        private readonly VehiculoRepositorio _vehiculoRepositorio;

        private string       _placaBusqueda    = string.Empty;
        private string       _placa            = string.Empty;
        private TipoVehiculo _tipoVehiculo     = TipoVehiculo.Automovil;
        private int          _clienteId        = 0;
        private string       _nombreCliente    = string.Empty;
        private string       _mensajeError     = string.Empty;
        private string       _mensajeExito     = string.Empty;
        private bool         _modoCreacion     = false;
        private Vehiculo?    _vehiculoEncontrado = null;

        /// <summary>
        /// Constructor del VehiculoViewModel.
        /// </summary>
        /// <param name="vehiculoRepositorio">Repositorio de vehículos.</param>
        public VehiculoViewModel(VehiculoRepositorio vehiculoRepositorio)
        {
            _vehiculoRepositorio = vehiculoRepositorio;
        }

        /// <summary>
        /// Placa usada para la búsqueda de vehículo.
        /// </summary>
        public string PlacaBusqueda
        {
            get => _placaBusqueda;
            set => SetProperty(ref _placaBusqueda, value);
        }

        /// <summary>
        /// Placa del vehículo en el formulario de creación/edición.
        /// </summary>
        public string Placa
        {
            get => _placa;
            set => SetProperty(ref _placa, value.ToUpperInvariant());
        }

        /// <summary>
        /// Tipo de vehículo seleccionado.
        /// </summary>
        public TipoVehiculo TipoVehiculo
        {
            get => _tipoVehiculo;
            set => SetProperty(ref _tipoVehiculo, value);
        }

        /// <summary>
        /// Identificador del cliente propietario del vehículo.
        /// </summary>
        public int ClienteId
        {
            get => _clienteId;
            set => SetProperty(ref _clienteId, value);
        }

        /// <summary>
        /// Nombre del cliente propietario (para mostrar en pantalla).
        /// </summary>
        public string NombreCliente
        {
            get => _nombreCliente;
            set => SetProperty(ref _nombreCliente, value);
        }

        /// <summary>
        /// Mensaje de error del formulario.
        /// </summary>
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        /// <summary>
        /// Mensaje de éxito tras guardar o encontrar el vehículo.
        /// </summary>
        public string MensajeExito
        {
            get => _mensajeExito;
            set => SetProperty(ref _mensajeExito, value);
        }

        /// <summary>
        /// Indica si el formulario está en modo de creación de nuevo vehículo.
        /// </summary>
        public bool ModoCreacion
        {
            get => _modoCreacion;
            set => SetProperty(ref _modoCreacion, value);
        }

        /// <summary>
        /// Vehículo encontrado tras la búsqueda por placa.
        /// </summary>
        public Vehiculo? VehiculoEncontrado
        {
            get => _vehiculoEncontrado;
            set => SetProperty(ref _vehiculoEncontrado, value);
        }

        /// <summary>
        /// Lista de tipos de vehículos disponibles para el ComboBox.
        /// </summary>
        public IEnumerable<TipoVehiculo> TiposVehiculo =>
            Enum.GetValues<TipoVehiculo>();

        /// <summary>
        /// Se dispara cuando se encontró o creó un vehículo correctamente.
        /// </summary>
        public event EventHandler<Vehiculo>? VehiculoSeleccionado;

        /// <summary>
        /// Busca un vehículo por placa. Si no existe, activa el modo creación.
        /// </summary>
        public async Task BuscarPorPlacaAsync()
        {
            MensajeError      = string.Empty;
            MensajeExito      = string.Empty;
            VehiculoEncontrado = null;

            if (string.IsNullOrWhiteSpace(PlacaBusqueda))
            {
                MensajeError = "Ingrese la placa para buscar.";
                return;
            }

            var vehiculo = await _vehiculoRepositorio.ObtenerPorPlacaAsync(PlacaBusqueda.Trim().ToUpperInvariant());

            if (vehiculo != null)
            {
                VehiculoEncontrado = vehiculo;
                Placa              = vehiculo.Placa;
                TipoVehiculo       = vehiculo.TipoVehiculo;
                ClienteId          = vehiculo.ClienteId;
                NombreCliente      = vehiculo.Cliente?.NombreCompleto ?? string.Empty;
                ModoCreacion       = false;
                MensajeExito       = $"Vehículo encontrado: {vehiculo.Placa} — {vehiculo.TipoVehiculo}";
                VehiculoSeleccionado?.Invoke(this, vehiculo);
            }
            else
            {
                Placa        = PlacaBusqueda.Trim().ToUpperInvariant();
                TipoVehiculo = TipoVehiculo.Automovil;
                ModoCreacion = true;
                MensajeError = "Vehículo no encontrado. Complete los datos para registrarlo.";
            }
        }

        /// <summary>
        /// Guarda un nuevo vehículo asignado al cliente actual.
        /// </summary>
        public async Task GuardarAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            if (string.IsNullOrWhiteSpace(Placa))
            {
                MensajeError = "La placa es obligatoria.";
                return;
            }
            if (ClienteId <= 0)
            {
                MensajeError = "Debe asociar un cliente al vehículo.";
                return;
            }

            try
            {
                var vehiculo = new Vehiculo
                {
                    Placa        = Placa.Trim().ToUpperInvariant(),
                    TipoVehiculo = TipoVehiculo,
                    ClienteId    = ClienteId,
                    Activo       = true
                };

                await _vehiculoRepositorio.AgregarAsync(vehiculo);
                await _vehiculoRepositorio.GuardarAsync();

                VehiculoEncontrado = vehiculo;
                ModoCreacion       = false;
                MensajeExito       = "Vehículo registrado exitosamente.";
                VehiculoSeleccionado?.Invoke(this, vehiculo);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al guardar el vehículo: {ex.Message}";
            }
        }

        /// <summary>
        /// Limpia el formulario y reinicia el ViewModel para una nueva búsqueda.
        /// </summary>
        public void Limpiar()
        {
            PlacaBusqueda      = string.Empty;
            Placa              = string.Empty;
            TipoVehiculo       = TipoVehiculo.Automovil;
            ClienteId          = 0;
            NombreCliente      = string.Empty;
            MensajeError       = string.Empty;
            MensajeExito       = string.Empty;
            ModoCreacion       = false;
            VehiculoEncontrado = null;
        }
    }
}
