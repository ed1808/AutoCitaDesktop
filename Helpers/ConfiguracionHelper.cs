using System.Text.Json;

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
        /// Modelo interno para serialización/deserialización de la configuración.
        /// </summary>
        private class ConfiguracionModel
        {
            public string CadenaConexion { get; set; } = string.Empty;
        }
    }
}
