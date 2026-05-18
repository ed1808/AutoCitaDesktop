using AutoCita.Commands;
using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para la gestión de sedes del sistema (solo rol Administrador).
    /// </summary>
    public class SedeViewModel : BaseViewModel
    {
        private readonly SedeRepositorio _sedeRepositorio;

        private List<Sede> _sedes        = [];
        private Sede?      _seleccionada = null;
        private int        _id           = 0;
        private string     _nombre       = string.Empty;
        private string     _direccion    = string.Empty;
        private string     _mensajeError = string.Empty;
        private string     _mensajeExito = string.Empty;
        private bool       _modoEdicion  = false;

        /// <summary>
        /// Constructor del SedeViewModel.
        /// </summary>
        /// <param name="sedeRepositorio">Repositorio de sedes.</param>
        public SedeViewModel(SedeRepositorio sedeRepositorio)
        {
            _sedeRepositorio = sedeRepositorio;
        }

        /// <summary>Lista de sedes activas del sistema.</summary>
        public List<Sede> Sedes { get => _sedes; set => SetProperty(ref _sedes, value); }

        /// <summary>Sede seleccionada en el grid.</summary>
        public Sede? Seleccionada { get => _seleccionada; set => SetProperty(ref _seleccionada, value); }

        /// <summary>Identificador de la sede en edición.</summary>
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        /// <summary>Nombre de la sede.</summary>
        public string Nombre { get => _nombre; set => SetProperty(ref _nombre, value); }

        /// <summary>Dirección física de la sede.</summary>
        public string Direccion { get => _direccion; set => SetProperty(ref _direccion, value); }

        /// <summary>Mensaje de error del formulario.</summary>
        public string MensajeError { get => _mensajeError; set => SetProperty(ref _mensajeError, value); }

        /// <summary>Mensaje de éxito tras la operación.</summary>
        public string MensajeExito { get => _mensajeExito; set => SetProperty(ref _mensajeExito, value); }

        /// <summary>Indica si el formulario está en modo edición.</summary>
        public bool ModoEdicion { get => _modoEdicion; set => SetProperty(ref _modoEdicion, value); }

        /// <summary>
        /// Carga todas las sedes activas desde la base de datos.
        /// </summary>
        public async Task CargarAsync()
        {
            Sedes = await _sedeRepositorio.ObtenerActivasAsync();
        }

        /// <summary>
        /// Prepara el formulario para crear una nueva sede.
        /// </summary>
        public void NuevaSede()
        {
            Id           = 0;
            Nombre       = string.Empty;
            Direccion    = string.Empty;
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            ModoEdicion  = false;
            Seleccionada = null;
        }

        /// <summary>
        /// Carga los datos de la sede seleccionada en el formulario para edición.
        /// </summary>
        /// <param name="sede">Sede a editar.</param>
        public void EditarSede(Sede sede)
        {
            Id           = sede.Id;
            Nombre       = sede.Nombre;
            Direccion    = sede.Direccion;
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            ModoEdicion  = true;
            Seleccionada = sede;
        }

        /// <summary>
        /// Guarda la sede (crea o actualiza según ModoEdicion).
        /// </summary>
        public async Task GuardarAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MensajeError = "El nombre de la sede es obligatorio.";
                return;
            }
            if (string.IsNullOrWhiteSpace(Direccion))
            {
                MensajeError = "La dirección de la sede es obligatoria.";
                return;
            }

            try
            {
                if (!ModoEdicion)
                {
                    var sede = new Sede
                    {
                        Nombre    = Nombre.Trim(),
                        Direccion = Direccion.Trim(),
                        Activo    = true
                    };
                    await _sedeRepositorio.AgregarAsync(sede);
                    await _sedeRepositorio.GuardarAsync();
                    MensajeExito = "Sede creada exitosamente.";
                }
                else
                {
                    var sede = await _sedeRepositorio.ObtenerPorIdAsync(Id)
                        ?? throw new InvalidOperationException("Sede no encontrada.");

                    sede.Nombre    = Nombre.Trim();
                    sede.Direccion = Direccion.Trim();
                    _sedeRepositorio.Actualizar(sede);
                    await _sedeRepositorio.GuardarAsync();
                    MensajeExito = "Sede actualizada exitosamente.";
                }

                await CargarAsync();
                NuevaSede();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        /// <summary>
        /// Realiza el borrado lógico de la sede seleccionada (Activo = false).
        /// </summary>
        /// <param name="sedeId">Identificador de la sede a desactivar.</param>
        public async Task DesactivarAsync(int sedeId)
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            try
            {
                var comando = new EliminarLogicoComando(sedeId, _sedeRepositorio);
                await comando.EjecutarAsync();
                MensajeExito = "Sede desactivada exitosamente.";
                await CargarAsync();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }
    }
}
