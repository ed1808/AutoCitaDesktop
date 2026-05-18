using AutoCita.Enums;
using AutoCita.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoCita.Data
{
    /// <summary>
    /// Contexto de base de datos para el sistema AutoCita.
    /// </summary>
    public class AutoCitaDbContext : DbContext
    {
        /// <summary>
        /// Constructor del contexto con opciones de configuración.
        /// </summary>
        /// <param name="options">Opciones de configuración del contexto.</param>
        public AutoCitaDbContext(DbContextOptions<AutoCitaDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Conjunto de entidades Sede.
        /// </summary>
        public DbSet<Sede> Sedes { get; set; }

        /// <summary>
        /// Conjunto de entidades Usuario.
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// Conjunto de entidades Cliente.
        /// </summary>
        public DbSet<Cliente> Clientes { get; set; }

        /// <summary>
        /// Conjunto de entidades Vehiculo.
        /// </summary>
        public DbSet<Vehiculo> Vehiculos { get; set; }

        /// <summary>
        /// Conjunto de entidades Cita.
        /// </summary>
        public DbSet<Cita> Citas { get; set; }

        /// <summary>
        /// Conjunto de entidades MetaProductividad.
        /// </summary>
        public DbSet<MetaProductividad> MetasProductividad { get; set; }

        /// <summary>
        /// Configuración de las entidades mediante Fluent API.
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelos de EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Sede
            modelBuilder.Entity<Sede>(entity =>
            {
                entity.ToTable("Sedes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Direccion).IsRequired();
                entity.Property(e => e.Activo).IsRequired().HasDefaultValue(true);
            });

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Rol).IsRequired().HasConversion<string>();
                entity.Property(e => e.Activo).IsRequired().HasDefaultValue(true);
                entity.HasOne(e => e.Sede).WithMany().HasForeignKey(e => e.SedeId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DocumentoIdentidad).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.DocumentoIdentidad).IsUnique();
                entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Telefono).IsRequired();
                entity.Property(e => e.FechaRegistro).IsRequired();
            });

            // Configuración de Vehiculo
            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.ToTable("Vehiculos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Placa).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.Placa).IsUnique();
                entity.Property(e => e.TipoVehiculo).IsRequired().HasConversion<string>();
                entity.Property(e => e.Activo).IsRequired().HasDefaultValue(true);
                entity.HasOne(e => e.Cliente).WithMany().HasForeignKey(e => e.ClienteId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Cita
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.ToTable("Citas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaHora).IsRequired();
                entity.Property(e => e.Motivo).IsRequired().HasMaxLength(250);
                entity.Property(e => e.Observaciones).IsRequired(false);
                entity.Property(e => e.Estado).IsRequired().HasConversion<string>().HasDefaultValue(EstadoCita.Programada);
                entity.Property(e => e.RecordatorioRealizado).IsRequired().HasDefaultValue(false);
                entity.HasOne(e => e.Vehiculo).WithMany().HasForeignKey(e => e.VehiculoId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Sede).WithMany().HasForeignKey(e => e.SedeId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de MetaProductividad
            modelBuilder.Entity<MetaProductividad>(entity =>
            {
                entity.ToTable("MetasProductividad");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MetaLlamadas).IsRequired();
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin).IsRequired(false);
                entity.HasOne(e => e.Sede).WithMany().HasForeignKey(e => e.SedeId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
