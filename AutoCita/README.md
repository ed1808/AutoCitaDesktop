# AutoCita 🚗🔧

Sistema de gestión de citas para talleres de mecánica automotriz. Desarrollado en **.NET 10** con **Windows Forms** y **Entity Framework Core** sobre **PostgreSQL**.

---

## Características principales

- **Gestión de citas**: Crear, reprogramar y cancelar citas de vehículos.
- **Gestión de clientes y vehículos**: Registro y administración de clientes con sus vehículos asociados.
- **Gestión de sedes**: Administración de las sedes del taller.
- **Roles de usuario**: Control de acceso basado en tres roles.
- **Recordatorios**: Módulo de recordatorios para citas próximas.
- **Reportes**: Generación de informes de productividad, citas por agente, sedes y usuarios.
- **Onboarding inicial**: Asistente de configuración en la primera ejecución.

---

## Roles de usuario

| Rol | Permisos |
|---|---|
| **Administrador** | Acceso total: CRUD de usuarios, sedes e informes completos |
| **Director** | Gestión del contact center: agendamiento, modificación, cancelación y reportes de productividad |
| **Agente** | Operaciones básicas: agendamiento, modificación y cancelación de citas |

---

## Estados de una cita

| Estado | Descripción |
|---|---|
| `Programada` | Cita activa, pendiente de atención |
| `Reprogramada` | La cita cambió de fecha u hora |
| `Cancelada` | Cancelada por el cliente o el taller |
| `Realizada` | El vehículo fue atendido |

---

## Tecnologías utilizadas

| Tecnología | Versión |
|---|---|
| .NET | 10.0 |
| Windows Forms | — |
| Entity Framework Core | 9.x |
| Npgsql (PostgreSQL) | 9.0.2 |
| BCrypt.Net-Next | 4.0.3 |

---

## Arquitectura

El proyecto sigue una arquitectura en capas con los siguientes componentes:

```
AutoCita/
├── Commands/        # Patrón Command (crear, cancelar, reprogramar citas/usuarios)
├── Data/            # DbContext y factory de EF Core
├── Enums/           # Enumeraciones (EstadoCita, RolUsuario, TipoVehiculo)
├── Helpers/         # Utilidades (configuración, contraseñas, sesión activa)
├── Migrations/      # Migraciones de base de datos
├── Models/          # Entidades del dominio (Cita, Cliente, Vehiculo, etc.)
├── Repositories/    # Patrón Repository con interfaz genérica
├── Services/        # Lógica de negocio (Auth, Citas, Reportes, Recordatorios)
├── Strategies/      # Patrón Strategy para disponibilidad y generación de reportes
├── ViewModels/      # Patrón MVVM
└── Views/           # Formularios Windows Forms
```

---

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (versión 13 o superior)
- Visual Studio 2026 o superior

---

## Configuración y ejecución

1. **Clonar el repositorio**
   ```bash
   git clone <url-del-repositorio>
   cd AutoCita
   ```

2. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

3. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```
   En la primera ejecución se mostrará el asistente de **onboarding** para configurar la cadena de conexión a PostgreSQL y crear el usuario administrador inicial.

4. **Aplicar migraciones manualmente** (opcional, si se omite el onboarding)
   ```bash
   dotnet ef database update
   ```

---

## Patrones de diseño aplicados

- **Repository** – Abstracción del acceso a datos mediante `IRepositorio<T>`.
- **Command** – Encapsulación de operaciones como crear/cancelar/reprogramar citas.
- **Strategy** – Algoritmos intercambiables para disponibilidad de vehículos y generación de reportes.
- **MVVM** – Separación entre la lógica de presentación (`ViewModels`) y los formularios (`Views`).
