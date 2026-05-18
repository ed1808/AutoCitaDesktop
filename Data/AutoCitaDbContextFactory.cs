using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoCita.Data
{
    /// <summary>
    /// Factory para crear instancias de AutoCitaDbContext en tiempo de diseño.
    /// Utilizada por las herramientas de EF Core para generar migraciones.
    /// </summary>
    public class AutoCitaDbContextFactory : IDesignTimeDbContextFactory<AutoCitaDbContext>
    {
        /// <summary>
        /// Crea una instancia del contexto para operaciones de diseño (migraciones).
        /// </summary>
        /// <param name="args">Argumentos de línea de comandos.</param>
        /// <returns>Instancia configurada de AutoCitaDbContext.</returns>
        public AutoCitaDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AutoCitaDbContext>();

            // Cadena de conexión temporal para generación de migraciones
            // No se usa en runtime - solo para herramientas de diseño
            optionsBuilder.UseNpgsql("Host=localhost;Database=autocita_design;Username=postgres;Password=postgres");

            return new AutoCitaDbContext(optionsBuilder.Options);
        }
    }
}
