using AutoCita.Enums;
using AutoCita.Helpers;
using AutoCita.Models;
using AutoCita.Repositories;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// ViewModel para la lista de citas agendadas.
    /// Filtra por agente según el rol del usuario logueado.
    /// </summary>
    public class ListaCitasViewModel : BaseViewModel
    {
        private readonly CitaRepositorio _citaRepositorio;

        private List<Cita> _citas = [];
        private bool _cargando = false;
        private string _mensajeError = string.Empty;

        /// <summary>
        /// Constructor del ViewModel de lista de citas.
        /// </summary>
        /// <param name="citaRepositorio">Repositorio de citas.</param>
        public ListaCitasViewModel(CitaRepositorio citaRepositorio)
        {
            _citaRepositorio = citaRepositorio;
        }

        /// <summary>Lista de citas a mostrar en el grid.</summary>
        public List<Cita> Citas
        {
            get => _citas;
            set => SetProperty(ref _citas, value);
        }

        /// <summary>Indica si hay una carga en progreso.</summary>
        public bool Cargando
        {
            get => _cargando;
            set => SetProperty(ref _cargando, value);
        }

        /// <summary>Mensaje de error si falla la carga.</summary>
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        /// <summary>
        /// Carga las citas desde la base de datos filtrando según el rol del usuario.
        /// - Agente: solo sus propias citas
        /// - Administrador/Director: todas las citas
        /// </summary>
        public async Task CargarCitasAsync()
        {
            MensajeError = string.Empty;
            Cargando = true;

            try
            {
                var usuario = SesionActual.Instancia.UsuarioLogueado;
                if (usuario == null)
                {
                    MensajeError = "No hay usuario logueado.";
                    return;
                }

                if (usuario.Rol == RolUsuario.Agente)
                {
                    Citas = await _citaRepositorio.ObtenerPorAgenteAsync(usuario.Id);
                }
                else
                {
                    Citas = await _citaRepositorio.ObtenerTodosAsync();
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar las citas: {ex.Message}";
                Citas = [];
            }
            finally
            {
                Cargando = false;
            }
        }
    }
}
