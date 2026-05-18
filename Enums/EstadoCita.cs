namespace AutoCita.Enums
{
    /// <summary>
    /// Estados posibles de una cita en el sistema.
    /// </summary>
    public enum EstadoCita
    {
        /// <summary>
        /// Cita programada y pendiente de atención.
        /// </summary>
        Programada = 1,

        /// <summary>
        /// Cita cancelada por el cliente o el taller.
        /// </summary>
        Cancelada = 2,

        /// <summary>
        /// Cita reprogramada (cambió fecha u hora).
        /// </summary>
        Reprogramada = 3,

        /// <summary>
        /// Cita realizada (vehículo fue atendido).
        /// </summary>
        Realizada = 4
    }
}
