namespace AutoCita.Helpers
{
    /// <summary>
    /// Helper para el manejo seguro de contraseñas mediante BCrypt.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Genera un hash seguro de una contraseña en texto plano usando BCrypt.
        /// </summary>
        /// <param name="passwordTextoPlano">Contraseña en texto plano.</param>
        /// <returns>Hash de la contraseña.</returns>
        public static string Hashear(string passwordTextoPlano)
        {
            return BCrypt.Net.BCrypt.HashPassword(passwordTextoPlano);
        }

        /// <summary>
        /// Verifica si una contraseña en texto plano coincide con un hash almacenado.
        /// </summary>
        /// <param name="passwordTextoPlano">Contraseña en texto plano.</param>
        /// <param name="passwordHash">Hash almacenado de la contraseña.</param>
        /// <returns>True si coinciden, False en caso contrario.</returns>
        public static bool Verificar(string passwordTextoPlano, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(passwordTextoPlano, passwordHash);
        }
    }
}
