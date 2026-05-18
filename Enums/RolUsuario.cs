namespace AutoCita.Enums
{
    /// <summary>
    /// Roles de usuario en el sistema AutoCita.
    /// </summary>
    public enum RolUsuario
    {
        /// <summary>
        /// Usuario con permisos totales: CRUD usuarios, sedes, generación de informes completos.
        /// </summary>
        Administrador = 1,

        /// <summary>
        /// Usuario con permisos de gestión del contact center: agendamiento, modificación, cancelación, reportes de productividad.
        /// </summary>
        Director = 2,

        /// <summary>
        /// Usuario con permisos básicos: agendamiento, modificación, cancelación de citas.
        /// </summary>
        Agente = 3
    }
}
