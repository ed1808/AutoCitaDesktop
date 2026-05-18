namespace AutoCita.Commands
{
    /// <summary>
    /// Interfaz base del patrón Command para encapsular operaciones de negocio como objetos.
    /// </summary>
    public interface IComando
    {
        /// <summary>
        /// Ejecuta la operación encapsulada por el comando de forma asíncrona.
        /// </summary>
        Task EjecutarAsync();
    }
}
