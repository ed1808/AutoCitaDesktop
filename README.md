# AutoCita 🚗🔧

Sistema de gestión de citas para talleres de mecánica automotriz. Desarrollado en **.NET 10** con **Windows Forms** y **Entity Framework Core** sobre **PostgreSQL**.

---

## 📋 Descripción del proyecto

**AutoCita** es una aplicación de escritorio diseñada para la administración integral de citas en talleres de mecánica automotriz. Permite gestionar clientes, vehículos, sedes y citas de manera eficiente, con un sistema de roles que garantiza el control de acceso según las responsabilidades de cada usuario.

El sistema incluye funcionalidades avanzadas como:
- Recordatorios automáticos para citas próximas
- Generación de reportes de productividad y análisis por agente, sede y usuario
- Asistente de configuración inicial (onboarding) para la primera ejecución
- Validación de disponibilidad de vehículos mediante estrategias intercambiables

---

## ✨ Características principales

- **Gestión de citas**: Crear, reprogramar y cancelar citas de vehículos.
- **Gestión de clientes y vehículos**: Registro y administración de clientes con sus vehículos asociados.
- **Gestión de sedes**: Administración de las sedes del taller.
- **Roles de usuario**: Control de acceso basado en tres roles.
- **Recordatorios**: Módulo de recordatorios para citas próximas.
- **Reportes**: Generación de informes de productividad, citas por agente, sedes y usuarios.
- **Onboarding inicial**: Asistente de configuración en la primera ejecución.

---

## 👥 Roles de usuario

| Rol | Permisos |
|---|---|
| **Administrador** | Acceso total: CRUD de usuarios, sedes e informes completos |
| **Director** | Gestión del contact center: agendamiento, modificación, cancelación y reportes de productividad |
| **Agente** | Operaciones básicas: agendamiento, modificación y cancelación de citas |

---

## 📊 Estados de una cita

| Estado | Descripción |
|---|---|
| `Programada` | Cita activa, pendiente de atención |
| `Reprogramada` | La cita cambió de fecha u hora |
| `Cancelada` | Cancelada por el cliente o el taller |
| `Realizada` | El vehículo fue atendido |

---

## 🛠️ Tecnologías utilizadas

| Tecnología | Versión | Propósito |
|---|---|---|
| **.NET** | 10.0 | Framework principal de la aplicación |
| **Windows Forms** | — | Interfaz gráfica de usuario |
| **Entity Framework Core** | 9.x | ORM para acceso a base de datos |
| **Npgsql (PostgreSQL)** | 9.0.2 | Proveedor de base de datos PostgreSQL |
| **BCrypt.Net-Next** | 4.0.3 | Hash seguro de contraseñas |

---

## 🏗️ Patrón de arquitectura

El proyecto implementa el patrón **MVVM (Model-View-ViewModel)** como arquitectura principal, donde cada formulario WinForms (View) delega toda su lógica a un ViewModel independiente que implementa `INotifyPropertyChanged`. La organización en carpetas establece una separación de responsabilidades estructurada que da soporte a este patrón:

```
AutoCita/
├── Views/           [Vista — Formularios Windows Forms]
│   └── Formularios Windows Forms (FrmLogin, FrmPrincipal, FrmCita, etc.)
│
├── ViewModels/      [ViewModel — Lógica de presentación]
│   └── ViewModels con INotifyPropertyChanged (LoginViewModel, CitaViewModel, etc.)
│
├── Commands/        [Lógica de negocio — Operaciones encapsuladas]
│   └── Comandos encapsulados (CrearCitaComando, ReprogramarCitaComando, etc.)
│
├── Services/        [Lógica de negocio — Servicios]
│   └── Servicios de negocio (CitaServicio, AuthServicio, ReporteServicio, etc.)
│
├── Strategies/      [Lógica de negocio — Algoritmos intercambiables]
│   └── Estrategias intercambiables (IDisponibilidadStrategy, IReporteStrategy, etc.)
│
├── Repositories/    [Acceso a datos]
│   └── Repositorios con patrón Repository (CitaRepositorio, UsuarioRepositorio, etc.)
│
├── Data/            [Acceso a datos — Contexto]
│   └── DbContext y configuraciones de EF Core (AutoCitaDbContext)
│
├── Models/          [Modelo — Entidades del dominio]
│   └── Entidades del dominio (Cita, Cliente, Vehiculo, Usuario, Sede)
│
├── Enums/           [Modelo — Tipos del dominio]
│   └── Enumeraciones (EstadoCita, RolUsuario, TipoVehiculo)
│
├── Helpers/         [Transversal]
│   └── Utilidades (ConfiguracionHelper, PasswordHelper, SesionActual)
│
└── Migrations/      [Infraestructura]
	└── Migraciones de Entity Framework Core
```

### Flujo de dependencias
```
Views → ViewModels → Services/Commands → Repositories → Data/Models
```

**Principios de la arquitectura:**
- Las Views no contienen lógica; la delegan completamente a sus ViewModels
- Los ViewModels exponen propiedades y comandos observables mediante `INotifyPropertyChanged`
- Cada capa tiene una responsabilidad claramente definida
- La capa de dominio (Models/Enums) no tiene dependencias externas

---

## 🎨 Patrones de diseño aplicados

| Patrón | Dónde se aplica | Clase/Interfaz clave | Beneficio |
|---|---|---|---|
| **Repository** | Capa de acceso a datos | `IRepositorio<T>`<br>`CitaRepositorio`<br>`UsuarioRepositorio`<br>`SedeRepositorio` | Abstrae el acceso a datos de Entity Framework Core, permitiendo cambiar la implementación sin afectar la lógica de negocio. |
| **Command** | Operaciones de negocio encapsuladas | `IComando`<br>`CrearCitaComando`<br>`ReprogramarCitaComando`<br>`CancelarCitaComando`<br>`CrearUsuarioComando`<br>`EliminarLogicoComando` | Encapsula cada operación como un objeto, facilitando el deshacer/rehacer, logging y validaciones centralizadas. |
| **Strategy** | Algoritmos intercambiables | `IDisponibilidadStrategy`<br>`DisponibilidadVehiculoStrategy`<br>`IReporteStrategy`<br>`ReporteUsuariosStrategy`<br>`ReporteCitasStrategy` | Permite cambiar el algoritmo de validación de disponibilidad o generación de reportes en tiempo de ejecución sin modificar el código cliente. |
| **MVVM** | Separación de lógica de presentación | `BaseViewModel`<br>`LoginViewModel`<br>`CitaViewModel`<br>`ReporteViewModel`<br>`OnboardingViewModel` | Desacopla la lógica de presentación de los formularios mediante `INotifyPropertyChanged`, facilitando el testing y la reutilización. |
| **Singleton** | Instancia única de sesión | `SesionActual` (con `Lazy<T>`) | Garantiza una única instancia de la sesión de usuario en toda la aplicación con inicialización thread-safe. |
| **Observer** | Eventos de comunicación | `FrmLogin.cs` → evento `LoginExitoso`<br>`FrmOnboarding.cs` → evento `OnboardingFinalizado`<br>Suscritos en `Program.cs` | Desacopla la comunicación entre formularios mediante eventos de C#, permitiendo notificaciones asíncronas. |
| **Factory** | Creación de DbContext | `AutoCitaDbContextFactory` | Facilita la creación de instancias de DbContext con la configuración correcta para Entity Framework Core y migraciones. |

---

## 🧩 Principios SOLID — Mapeo al código

### **S — Single Responsibility Principle (Responsabilidad Única)**

Cada clase tiene una única razón para cambiar:

| Clase | Responsabilidad única |
|---|---|
| `AuthServicio` | Autenticar usuarios validando credenciales |
| `CitaServicio` | Gestionar el ciclo de vida de las citas |
| `ReporteServicio` | Generar reportes del sistema |
| `PasswordHelper` | Hashear y verificar contraseñas con BCrypt |
| `ConfiguracionHelper` | Leer y escribir la configuración de la aplicación |
| `SesionActual` | Mantener el estado de la sesión del usuario autenticado |

**Ejemplo en código:**
```csharp
// AuthServicio solo se encarga de autenticar
public class AuthServicio
{
	public async Task<Usuario?> AutenticarAsync(string username, string password)
	{
		// Solo validación de credenciales
	}
}
```

---

### **O — Open/Closed Principle (Abierto/Cerrado)**

El sistema está abierto a extensión pero cerrado a modificación:

| Extensión mediante abstracción | Implementaciones |
|---|---|
| `IReporteStrategy` | `ReporteUsuariosStrategy`<br>`ReporteCitasStrategy`<br>`ReporteSedesStrategy`<br>`ReporteProductividadStrategy` |
| `IDisponibilidadStrategy` | Permite agregar nuevas validaciones de disponibilidad sin modificar `CitaServicio` |
| `IComando` | `CrearCitaComando`<br>`ReprogramarCitaComando`<br>`CancelarCitaComando` |

**Ejemplo en código:**
```csharp
// Agregar un nuevo tipo de reporte sin modificar ReporteViewModel
public class ReporteNuevoTipoStrategy : IReporteStrategy
{
	public string NombreReporte => "Nuevo Tipo de Reporte";
	public Task<List<object>> GenerarAsync(DateTime desde, DateTime hasta, int? filtroId = null)
	{
		// Nueva implementación
	}
}
```

---

### **L — Liskov Substitution Principle (Sustitución de Liskov)**

Las implementaciones pueden sustituirse por sus abstracciones sin alterar el comportamiento:

| Abstracción | Implementaciones sustituibles |
|---|---|
| `IRepositorio<T>` | `CitaRepositorio`, `UsuarioRepositorio`, `SedeRepositorio`, `ClienteRepositorio`, `VehiculoRepositorio` |
| `IComando` | Cualquier comando implementa `EjecutarAsync()` de forma consistente |
| `IDisponibilidadStrategy` | Cualquier estrategia de disponibilidad es intercambiable en `CitaServicio` |

**Ejemplo en código:**
```csharp
// CitaServicio acepta cualquier implementación de IDisponibilidadStrategy
public class CitaServicio
{
	private readonly IDisponibilidadStrategy _disponibilidadStrategy;

	public CitaServicio(CitaRepositorio citaRepositorio, IDisponibilidadStrategy disponibilidadStrategy)
	{
		_disponibilidadStrategy = disponibilidadStrategy;
	}
}
```

---

### **I — Interface Segregation Principle (Segregación de Interfaces)**

Las interfaces son pequeñas y específicas, evitando métodos innecesarios:

| Interfaz | Métodos expuestos | Propósito |
|---|---|---|
| `IRepositorio<T>` | `ObtenerTodosAsync()`<br>`ObtenerPorIdAsync()`<br>`AgregarAsync()`<br>`Actualizar()`<br>`GuardarAsync()` | Solo operaciones CRUD genéricas |
| `IComando` | `EjecutarAsync()` | Solo ejecución de comandos |
| `IReporteStrategy` | `GenerarAsync()`<br>`NombreReporte` | Solo generación de reportes |
| `IDisponibilidadStrategy` | `HayDisponibilidad()`<br>`ObtenerMensajeConflicto()` | Solo validación de disponibilidad |

**Ejemplo en código:**
```csharp
// IComando solo expone lo necesario para ejecutar un comando
public interface IComando
{
	Task EjecutarAsync();
}
```

---

### **D — Dependency Inversion Principle (Inversión de Dependencias)**

Las clases de alto nivel dependen de abstracciones, no de implementaciones concretas:

| Clase de alto nivel | Depende de (abstracción) | No depende de (implementación) |
|---|---|---|
| `CitaServicio` | `IDisponibilidadStrategy` | Implementación concreta de estrategia |
| `ReporteViewModel` | `IReporteStrategy` | `ReporteUsuariosStrategy` específica |
| `LoginViewModel` | `AuthServicio` (clase concreta) | Implementación de repositorio |
| `Program.cs` | `IRepositorio<Usuario>` | `UsuarioRepositorio` directamente |

**Ejemplo en código:**
```csharp
// CitaServicio depende de la abstracción IDisponibilidadStrategy
public class CitaServicio
{
	private readonly IDisponibilidadStrategy _disponibilidadStrategy;

	// La estrategia se inyecta por constructor
	public CitaServicio(CitaRepositorio citaRepositorio, IDisponibilidadStrategy disponibilidadStrategy)
	{
		_disponibilidadStrategy = disponibilidadStrategy;
	}
}
```

**Inyección de dependencias manual en `Program.cs`:**
```csharp
// Las dependencias se construyen manualmente en Program.cs
var ctx = new AutoCitaDbContext(opciones);
var usuarioRepo = new UsuarioRepositorio(ctx);
var authServicio = new AuthServicio(usuarioRepo);
var loginVm = new LoginViewModel(authServicio);
```

---

## 📁 Estructura del proyecto

```
AutoCita/
│
├── Commands/                          # Patrón Command
│   ├── IComando.cs                    # Interfaz base de comandos
│   ├── CancelarCitaComando.cs
│   ├── CrearCitaComando.cs
│   ├── CrearUsuarioComando.cs
│   ├── EliminarLogicoComando.cs
│   └── ReprogramarCitaComando.cs
│
├── Data/                              # Contexto de Entity Framework Core
│   ├── AutoCitaDbContext.cs           # DbContext principal
│   └── AutoCitaDbContextFactory.cs    # Factory para migraciones
│
├── Enums/                             # Enumeraciones del dominio
│   ├── EstadoCita.cs
│   ├── RolUsuario.cs
│   └── TipoVehiculo.cs
│
├── Helpers/                           # Utilidades transversales
│   ├── ConfiguracionHelper.cs         # Gestión de configuración
│   ├── PasswordHelper.cs              # Hash de contraseñas
│   └── SesionActual.cs                # Singleton de sesión
│
├── Migrations/                        # Migraciones de EF Core
│   └── 20260518160642_InitialCreate.cs
│
├── Models/                            # Entidades del dominio
│   ├── Cita.cs
│   ├── Cliente.cs
│   ├── MetaProductividad.cs
│   ├── Sede.cs
│   ├── Usuario.cs
│   └── Vehiculo.cs
│
├── Repositories/                      # Patrón Repository
│   ├── IRepositorio.cs                # Interfaz genérica
│   ├── CitaRepositorio.cs
│   ├── ClienteRepositorio.cs
│   ├── MetaProductividadRepositorio.cs
│   ├── SedeRepositorio.cs
│   ├── UsuarioRepositorio.cs
│   └── VehiculoRepositorio.cs
│
├── Services/                          # Lógica de negocio
│   ├── AuthServicio.cs                # Autenticación
│   ├── CitaServicio.cs                # Gestión de citas
│   ├── RecordatorioServicio.cs        # Recordatorios
│   └── ReporteServicio.cs             # Generación de reportes
│
├── Strategies/                        # Patrón Strategy
│   ├── IDisponibilidadStrategy.cs     # Interfaz de disponibilidad
│   ├── IReporteStrategy.cs            # Interfaz de reportes
│   ├── DisponibilidadVehiculoStrategy.cs
│   ├── ReporteCitasStrategy.cs
│   ├── ReporteProductividadStrategy.cs
│   ├── ReporteSedesStrategy.cs
│   └── ReporteUsuariosStrategy.cs
│
├── ViewModels/                        # MVVM - Lógica de presentación
│   ├── BaseViewModel.cs               # ViewModel base con INotifyPropertyChanged
│   ├── CitaViewModel.cs
│   ├── ClienteViewModel.cs
│   ├── ListaCitasViewModel.cs
│   ├── LoginViewModel.cs
│   ├── OnboardingViewModel.cs
│   ├── RecordatorioViewModel.cs
│   ├── ReporteViewModel.cs
│   ├── SedeViewModel.cs
│   ├── UsuarioViewModel.cs
│   └── VehiculoViewModel.cs
│
├── Views/                             # Windows Forms
│   ├── FrmCita.cs
│   ├── FrmCliente.cs
│   ├── FrmListaCitas.cs
│   ├── FrmLogin.cs
│   ├── FrmOnboarding.cs
│   ├── FrmPrincipal.cs
│   ├── FrmRecordatorio.cs
│   ├── FrmReporte.cs
│   ├── FrmSede.cs
│   ├── FrmUsuario.cs
│   └── FrmVehiculo.cs
│
├── Program.cs                         # Punto de entrada de la aplicación
└── AutoCita.csproj                    # Archivo de proyecto
```

---

## ⚙️ Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (versión 13 o superior)
- Visual Studio 2026 o superior (recomendado)

---

## 🚀 Configuración y ejecución

### 1. Clonar el repositorio
```bash
git clone https://github.com/ed1808/AutoCitaDesktop.git
cd AutoCitaDesktop
```

### 2. Restaurar dependencias
```bash
dotnet restore
```

### 3. Ejecutar la aplicación
```bash
dotnet run --project AutoCita
```

En la **primera ejecución** se mostrará el asistente de **onboarding** para:
- Configurar la cadena de conexión a PostgreSQL
- Aplicar las migraciones de base de datos
- Crear el usuario administrador inicial

### 4. Aplicar migraciones manualmente (opcional)
Si decides omitir el onboarding, puedes aplicar las migraciones manualmente:

```bash
dotnet ef database update --project AutoCita
```

### 5. Credenciales por defecto
Después del onboarding, el usuario administrador inicial tendrá las credenciales que hayas configurado durante el proceso.

---

## 📖 Uso del sistema

### Flujo de trabajo típico

1. **Login**: Autenticarse con usuario y contraseña
2. **Menú principal**: Acceso a módulos según el rol del usuario
3. **Gestión de citas**:
   - Crear nueva cita (seleccionar cliente, vehículo, fecha y sede)
   - Reprogramar cita existente
   - Cancelar cita
   - Ver lista de citas con filtros
4. **Recordatorios**: Consultar citas próximas
5. **Reportes**: Generar informes de productividad (según rol)
6. **Administración**: Gestionar usuarios y sedes (solo Administrador)

---

## 🧪 Testing

El proyecto está preparado para testing mediante:
- **Unit tests**: Probar servicios y comandos de forma aislada
- **Integration tests**: Probar repositorios con base de datos en memoria
- **UI tests**: Probar ViewModels con validación de `INotifyPropertyChanged`

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Haz un fork del repositorio
2. Crea una rama para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Haz commit de tus cambios (`git commit -m 'Agrega nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Abre un Pull Request

---

## 📄 Licencia

Este proyecto es de uso educativo y no tiene una licencia formal definida.

---

## 📞 Soporte

Para reportar bugs o solicitar nuevas funcionalidades, por favor abre un [issue](https://github.com/ed1808/AutoCitaDesktop/issues) en GitHub.

---

**AutoCita** © 2026 - Sistema de gestión de citas para talleres automotrices
