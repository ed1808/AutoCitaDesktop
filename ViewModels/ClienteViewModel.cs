using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para la gestión y búsqueda de clientes.
    /// </summary>
    public class ClienteViewModel : BaseViewModel
    {
        private readonly ClienteRepositorio _clienteRepositorio;

        private string   _documentoBusqueda  = string.Empty;
        private string   _documentoIdentidad = string.Empty;
        private string   _nombreCompleto     = string.Empty;
        private string   _telefono           = string.Empty;
        private string   _mensajeError       = string.Empty;
        private string   _mensajeExito       = string.Empty;
        private bool     _modoCreacion       = false;
        private Cliente? _clienteEncontrado  = null;

        /// <summary>
        /// Constructor del ClienteViewModel.
        /// </summary>
        /// <param name="clienteRepositorio">Repositorio de clientes.</param>
        public ClienteViewModel(ClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        /// <summary>
        /// Documento de identidad usado para la búsqueda de cliente.
        /// </summary>
        public string DocumentoBusqueda
        {
            get => _documentoBusqueda;
            set => SetProperty(ref _documentoBusqueda, value);
        }

        /// <summary>
        /// Número de documento de identidad del cliente en el formulario.
        /// </summary>
        public string DocumentoIdentidad
        {
            get => _documentoIdentidad;
            set => SetProperty(ref _documentoIdentidad, value);
        }

        /// <summary>
        /// Nombre completo del cliente.
        /// </summary>
        public string NombreCompleto
        {
            get => _nombreCompleto;
            set => SetProperty(ref _nombreCompleto, value);
        }

        /// <summary>
        /// Número de teléfono del cliente para contacto vía WhatsApp.
        /// </summary>
        public string Telefono
        {
            get => _telefono;
            set => SetProperty(ref _telefono, value);
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
        /// Mensaje de éxito tras guardar o encontrar el cliente.
        /// </summary>
        public string MensajeExito
        {
            get => _mensajeExito;
            set => SetProperty(ref _mensajeExito, value);
        }

        /// <summary>
        /// Indica si el formulario está en modo de creación de nuevo cliente.
        /// </summary>
        public bool ModoCreacion
        {
            get => _modoCreacion;
            set => SetProperty(ref _modoCreacion, value);
        }

        /// <summary>
        /// Cliente encontrado tras la búsqueda por documento.
        /// </summary>
        public Cliente? ClienteEncontrado
        {
            get => _clienteEncontrado;
            set => SetProperty(ref _clienteEncontrado, value);
        }

        /// <summary>
        /// Se dispara cuando se encontró o creó un cliente correctamente.
        /// </summary>
        public event EventHandler<Cliente>? ClienteSeleccionado;

        /// <summary>
        /// Busca un cliente por número de documento de identidad.
        /// Si no existe, activa el modo creación para registrarlo.
        /// </summary>
        public async Task BuscarPorDocumentoAsync()
        {
            MensajeError  = string.Empty;
            MensajeExito  = string.Empty;
            ClienteEncontrado = null;

            if (string.IsNullOrWhiteSpace(DocumentoBusqueda))
            {
                MensajeError = "Ingrese el número de documento para buscar.";
                return;
            }

            var cliente = await _clienteRepositorio.ObtenerPorDocumentoAsync(DocumentoBusqueda.Trim());

            if (cliente != null)
            {
                ClienteEncontrado   = cliente;
                DocumentoIdentidad  = cliente.DocumentoIdentidad;
                NombreCompleto      = cliente.NombreCompleto;
                Telefono            = cliente.Telefono;
                ModoCreacion        = false;
                MensajeExito        = $"Cliente encontrado: {cliente.NombreCompleto}";
                ClienteSeleccionado?.Invoke(this, cliente);
            }
            else
            {
                DocumentoIdentidad = DocumentoBusqueda.Trim();
                NombreCompleto     = string.Empty;
                Telefono           = string.Empty;
                ModoCreacion       = true;
                MensajeError       = "Cliente no encontrado. Complete los datos para registrarlo.";
            }
        }

        /// <summary>
        /// Guarda un nuevo cliente en el sistema.
        /// </summary>
        public async Task GuardarAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            if (string.IsNullOrWhiteSpace(DocumentoIdentidad))
            {
                MensajeError = "El documento de identidad es obligatorio.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NombreCompleto))
            {
                MensajeError = "El nombre completo es obligatorio.";
                return;
            }
            if (string.IsNullOrWhiteSpace(Telefono))
            {
                MensajeError = "El teléfono es obligatorio.";
                return;
            }
            if (DocumentoIdentidad.Trim().Length > 20)
            {
                MensajeError = "El documento de identidad no puede exceder los 20 caracteres.";
                return;
            }
            if (NombreCompleto.Trim().Length > 150)
            {
                MensajeError = "El nombre completo no puede exceder los 150 caracteres.";
                return;
            }

            try
            {
                var existente = await _clienteRepositorio.ObtenerPorDocumentoAsync(DocumentoIdentidad.Trim());
                if (existente != null)
                {
                    MensajeError = "Ya existe un cliente registrado con ese número de documento.";
                    return;
                }

                var cliente = new Cliente
                {
                    DocumentoIdentidad = DocumentoIdentidad.Trim(),
                    NombreCompleto     = NombreCompleto.Trim(),
                    Telefono           = Telefono.Trim(),
                    FechaRegistro      = DateTime.UtcNow
                };

                await _clienteRepositorio.AgregarAsync(cliente);
                await _clienteRepositorio.GuardarAsync();

                ClienteEncontrado = cliente;
                ModoCreacion      = false;
                MensajeExito      = "Cliente registrado exitosamente.";
                ClienteSeleccionado?.Invoke(this, cliente);
            }
            catch (Exception ex)
            {
                var mensajeDetalle = ex.InnerException?.Message ?? ex.Message;
                MensajeError = $"Error al guardar el cliente: {mensajeDetalle}";
            }
        }

        /// <summary>
        /// Limpia el formulario y reinicia el ViewModel para una nueva búsqueda.
        /// </summary>
        public void Limpiar()
        {
            DocumentoBusqueda  = string.Empty;
            DocumentoIdentidad = string.Empty;
            NombreCompleto     = string.Empty;
            Telefono           = string.Empty;
            MensajeError       = string.Empty;
            MensajeExito       = string.Empty;
            ModoCreacion       = false;
            ClienteEncontrado  = null;
        }
    }
}
