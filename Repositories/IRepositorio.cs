namespace AutoCita.Repositories
{
    /// <summary>
    /// Interfaz genérica para el patrón Repository con operaciones CRUD básicas.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad.</typeparam>
    public interface IRepositorio<T> where T : class
    {
        /// <summary>
        /// Obtiene todas las entidades del tipo especificado.
        /// </summary>
        /// <returns>Lista de entidades.</returns>
        Task<List<T>> ObtenerTodosAsync();

        /// <summary>
        /// Obtiene una entidad por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <returns>Entidad encontrada o null.</returns>
        Task<T?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Agrega una nueva entidad.
        /// </summary>
        /// <param name="entidad">Entidad a agregar.</param>
        Task AgregarAsync(T entidad);

        /// <summary>
        /// Actualiza una entidad existente.
        /// </summary>
        /// <param name="entidad">Entidad a actualizar.</param>
        void Actualizar(T entidad);

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        Task GuardarAsync();
    }
}
