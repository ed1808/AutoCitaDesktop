using System.Text.Json;
using Npgsql;

namespace AutoCita.Helpers
{
    /// <summary>
    /// Helper para gestionar la configuración de la aplicación (cadena de conexión a la base de datos).
    /// </summary>
    public static class ConfiguracionHelper
    {
        private static readonly string CarpetaConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoCita");
        private static readonly string ArchivoConfig = Path.Combine(CarpetaConfig, "config.json");

        /// <summary>
        /// Lee la cadena de conexión guardada en el archivo de configuración.
        /// </summary>
        /// <returns>Cadena de conexión o null si no existe.</returns>
        public static string? LeerCadenaConexion()
        {
            try
            {
                if (!File.Exists(ArchivoConfig))
                    return null;

                var json = File.ReadAllText(ArchivoConfig);
                var config = JsonSerializer.Deserialize<ConfiguracionModel>(json);
                return config?.CadenaConexion;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Guarda la cadena de conexión en el archivo de configuración.
        /// </summary>
        /// <param name="cadenaConexion">Cadena de conexión a guardar.</param>
        public static void GuardarCadenaConexion(string cadenaConexion)
        {
            try
            {
                if (!Directory.Exists(CarpetaConfig))
                    Directory.CreateDirectory(CarpetaConfig);

                var config = new ConfiguracionModel { CadenaConexion = cadenaConexion };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ArchivoConfig, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al guardar la configuración: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida que la cadena de conexión contenga los campos mínimos requeridos.
        /// </summary>
        /// <param name="cadena">Cadena de conexión a validar.</param>
        /// <returns>Mensaje de error si falta algún campo, o null si la cadena es válida.</returns>
        public static string? ValidarFormatoCadenaConexion(string cadena)
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(cadena);

                if (string.IsNullOrWhiteSpace(builder.Host))
                    return "La cadena de conexión debe incluir el parámetro 'Host' (servidor de base de datos).";

                if (string.IsNullOrWhiteSpace(builder.Database))
                    return "La cadena de conexión debe incluir el parámetro 'Database' (nombre de la base de datos).";

                if (string.IsNullOrWhiteSpace(builder.Username))
                    return "La cadena de conexión debe incluir el parámetro 'Username' (usuario de PostgreSQL).";

                if (string.IsNullOrWhiteSpace(builder.Password))
                    return "La cadena de conexión debe incluir el parámetro 'Password' (contraseña del usuario).";

                return null; // Cadena válida
            }
            catch (Exception ex)
            {
                return $"Formato de cadena de conexión inválido: {ex.Message}";
            }
        }

        /// <summary>
        /// Modelo interno para serialización/deserialización de la configuración.
        /// </summary>
        private class ConfiguracionModel
        {
            public string CadenaConexion { get; set; } = string.Empty;
        }
    }
}
