using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoCita.ViewModels
{
    /// <summary>
    /// Clase base para todos los ViewModels del sistema. Implementa INotifyPropertyChanged
    /// para el patrón MVVM con soporte de binding de datos.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Evento que se dispara cuando cambia el valor de una propiedad.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifica a los suscriptores que una propiedad ha cambiado su valor.
        /// </summary>
        /// <param name="nombrePropiedad">Nombre de la propiedad que cambió (inferido automáticamente).</param>
        protected void NotificarCambio([CallerMemberName] string? nombrePropiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }

        /// <summary>
        /// Establece el valor de una propiedad y dispara la notificación de cambio si el valor es diferente.
        /// </summary>
        /// <typeparam name="T">Tipo de la propiedad.</typeparam>
        /// <param name="campo">Referencia al campo de respaldo.</param>
        /// <param name="valor">Nuevo valor a asignar.</param>
        /// <param name="nombrePropiedad">Nombre de la propiedad (inferido automáticamente).</param>
        /// <returns>True si el valor cambió, False si era igual.</returns>
        protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? nombrePropiedad = null)
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor))
                return false;

            campo = valor;
            NotificarCambio(nombrePropiedad);
            return true;
        }
    }
}
