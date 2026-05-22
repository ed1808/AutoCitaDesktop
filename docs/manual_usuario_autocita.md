# Manual de Usuario — AutoCita 🚗🔧

**Sistema de Gestión de Citas para Talleres de Mecánica Automotriz**

---

## Tabla de contenidos

1. [Introducción](#1-introducción)
2. [Requisitos del sistema](#2-requisitos-del-sistema)
3. [Instalación y configuración inicial](#3-instalación-y-configuración-inicial)
4. [Inicio de sesión](#4-inicio-de-sesión)
5. [Pantalla principal y navegación](#5-pantalla-principal-y-navegación)
6. [Roles de usuario y permisos](#6-roles-de-usuario-y-permisos)
7. [Módulo de Citas](#7-módulo-de-citas)
8. [Módulo de Sedes](#8-módulo-de-sedes)
9. [Módulo de Recordatorios](#9-módulo-de-recordatorios)
10. [Módulo de Reportes](#10-módulo-de-reportes)
11. [Módulo de Usuarios (Administrador)](#11-módulo-de-usuarios-administrador)
12. [Estados de una cita](#12-estados-de-una-cita)
13. [Preguntas frecuentes](#13-preguntas-frecuentes)
14. [Glosario](#14-glosario)

---

## 1. Introducción

**AutoCita** es una aplicación de escritorio desarrollada en .NET 10 con Windows Forms, diseñada para la administración integral de citas en talleres de mecánica automotriz. Permite gestionar de manera eficiente clientes, vehículos, sedes y citas, con un sistema de roles que garantiza el control de acceso según las responsabilidades de cada usuario.

### ¿Qué puede hacer AutoCita?

- Registrar y gestionar clientes con sus vehículos asociados.
- Agendar, reprogramar y cancelar citas de servicio.
- Administrar múltiples sedes del taller.
- Enviar recordatorios automáticos sobre citas próximas.
- Generar reportes de productividad y análisis por agente, sede y usuario.
- Controlar el acceso de cada colaborador según su rol en la organización.

---

## 2. Requisitos del sistema

Antes de instalar AutoCita, verifique que su equipo cumpla con los siguientes requisitos:

| Componente | Requisito mínimo |
|---|---|
| **Sistema operativo** | Windows 10 (64-bit) o superior |
| **Base de datos** | PostgreSQL 13 o superior (opcional) |
| **Espacio en disco** | 500 MB libres |
| **Conexión de red** | Requerida para conectar con el servidor de base de datos |

> ⚠️ **Importante:** PostgreSQL debe estar instalado y en ejecución antes de iniciar AutoCita por primera vez si se va a trabajar en local.

---

## 3. Instalación y configuración inicial

### 3.1 Primera ejecución — Asistente de Onboarding

Al ejecutar AutoCita por primera vez, se abrirá automáticamente el **Asistente de Configuración Inicial (Onboarding)**. Este asistente le guiará paso a paso para dejar la aplicación lista para su uso.

**Pasos del asistente:**

**Paso 1 — Conexión a la base de datos**

Complete los datos de conexión a su servidor PostgreSQL:

| Campo | Descripción |
|---|---|
| **Servidor** | Dirección del servidor PostgreSQL (ej. `localhost` o IP del servidor) |
| **Puerto** | Puerto de conexión (por defecto: `5432`) |
| **Base de datos** | Nombre de la base de datos que utilizará AutoCita |
| **Usuario** | Nombre de usuario de PostgreSQL |
| **Contraseña** | Contraseña del usuario de PostgreSQL |

Una vez completados los campos, haga clic en **Verificar conexión** para comprobar que los datos son correctos.

**Paso 2 — Creación de la base de datos**

Si la conexión es exitosa, el asistente aplicará automáticamente las migraciones necesarias para crear las tablas y estructuras de la base de datos. Este proceso puede tardar algunos segundos.

**Paso 3 — Creación del usuario Administrador inicial**

Configure el primer usuario con rol de Administrador:

| Campo | Descripción |
|---|---|
| **Nombre de usuario** | Identificador único para iniciar sesión |
| **Contraseña** | Contraseña segura (mínimo 8 caracteres) |
| **Confirmar contraseña** | Repita la contraseña para verificarla |

Haga clic en **Finalizar configuración** para completar el proceso.

> ✅ Una vez finalizado el onboarding, la aplicación se reiniciará automáticamente y mostrará la pantalla de inicio de sesión.

### 3.2 Configuraciones subsiguientes

En ejecuciones posteriores, el asistente de onboarding no volverá a aparecer. Si necesita modificar la cadena de conexión a la base de datos, puede hacerlo desde el panel de configuración (disponible para el rol Administrador).

---

## 4. Inicio de sesión

Al abrir AutoCita encontrará la pantalla de **Inicio de Sesión**.

### Cómo iniciar sesión

1. Ingrese su **nombre de usuario** en el campo correspondiente.
2. Ingrese su **contraseña**.
3. Haga clic en el botón **Ingresar**.

Si las credenciales son correctas, la aplicación lo redirigirá al **menú principal** con las opciones disponibles según su rol.

### Errores comunes al iniciar sesión

| Mensaje | Posible causa | Solución |
|---|---|---|
| *Usuario o contraseña incorrectos* | Credenciales inválidas | Verifique que su usuario y contraseña sean correctos |
| *No se puede conectar a la base de datos* | El servidor PostgreSQL no está activo | Verifique que el servicio de PostgreSQL esté en ejecución |
| *Usuario desactivado* | Su cuenta fue desactivada por un administrador | Contacte al administrador del sistema |

> 🔒 Por seguridad, las contraseñas están protegidas con cifrado BCrypt. AutoCita nunca almacena contraseñas en texto plano.

---

## 5. Pantalla principal y navegación

Luego del inicio de sesión exitoso, verá la **pantalla principal (FrmPrincipal)**. Esta es el centro de navegación de la aplicación y muestra los módulos disponibles para su rol.

### Elementos de la pantalla principal

- **Barra de menú lateral:** Acceso a todos los módulos del sistema (Citas, Sedes, Recordatorios, Reportes, Usuarios).
- **Panel de información de sesión:** Muestra el nombre del usuario activo y su rol.
- **Botón de cierre de sesión:** Permite cerrar la sesión actual y volver a la pantalla de inicio de sesión.

> Los módulos disponibles en la barra de menú variarán según el rol del usuario autenticado.

---

## 6. Roles de usuario y permisos

AutoCita cuenta con tres roles, cada uno con un nivel de acceso diferente:

| Rol | Descripción general |
|---|---|
| **Administrador** | Control total del sistema |
| **Director** | Gestión del contact center y reportes |
| **Agente** | Operaciones básicas de agendamiento |

### Detalle de permisos por módulo

| Módulo | Administrador | Director | Agente |
|---|:---:|:---:|:---:|
| Crear cita | ✅ | ✅ | ✅ |
| Reprogramar cita | ✅ | ✅ | ✅ |
| Cancelar cita | ✅ | ✅ | ✅ |
| Ver lista de citas | ✅ | ✅ | ✅ |
| Gestión de clientes | ✅ | ✅ | ✅ |
| Gestión de vehículos | ✅ | ✅ | ✅ |
| Módulo de recordatorios | ✅ | ✅ | ✅ |
| Reportes de productividad | ✅ | ✅ | ❌ |
| Reportes por sede | ✅ | ✅ | ❌ |
| Reportes por usuario | ✅ | ✅ | ❌ |
| Gestión de sedes | ✅ | ❌ | ❌ |
| Gestión de usuarios | ✅ | ❌ | ❌ |
| Configuración del sistema | ✅ | ❌ | ❌ |

---

## 7. Módulo de Citas

El módulo de Citas es el núcleo de AutoCita. Permite gestionar todo el ciclo de vida de una cita de servicio automotriz.

### 7.1 Ver lista de citas

Acceda desde el menú principal a **Citas → Lista de Citas**. Se mostrará una tabla con todas las citas del sistema, con los siguientes filtros disponibles:

- **Filtro por estado:** Programada, Reprogramada, Cancelada, Realizada.
- **Filtro por fecha:** Rango de fechas de inicio y fin.
- **Filtro por sede:** Seleccione la sede del taller.
- **Filtro por agente:** Filtre las citas asignadas a un agente específico.

Haga clic en una cita de la lista para ver su detalle completo o acceder a las acciones disponibles.

### 7.2 Crear una nueva cita

1. En el menú principal, vaya a **Citas → Nueva Cita** (o haga clic en el botón **+ Nueva Cita** desde la lista de citas).
2. Complete el formulario con los siguientes datos:

| Campo | Descripción |
|---|---|
| **Cliente** | Seleccione el cliente de la lista o registre uno nuevo |
| **Vehículo** | Seleccione el vehículo asociado al cliente |
| **Sede** | Seleccione la sede del taller donde se realizará el servicio |
| **Fecha** | Ingrese la fecha de la cita |
| **Hora** | Ingrese la hora de atención |
| **Descripción del servicio** | Describa brevemente el trabajo a realizar |

3. El sistema verificará automáticamente la **disponibilidad del vehículo** para la fecha y hora seleccionadas. Si el vehículo ya tiene una cita activa en ese horario, recibirá una advertencia.
4. Haga clic en **Guardar** para confirmar la cita.

> ✅ Al guardar, la cita quedará en estado **Programada**.

### 7.3 Reprogramar una cita

1. Abra la lista de citas y seleccione la cita que desea reprogramar.
2. Haga clic en el botón **Reprogramar**.
3. Seleccione la nueva fecha y hora.
4. Haga clic en **Confirmar reprogramación**.

> La cita cambiará su estado a **Reprogramada** y se registrará el historial del cambio.

### 7.4 Cancelar una cita

1. Abra la lista de citas y seleccione la cita a cancelar.
2. Haga clic en el botón **Cancelar cita**.
3. Confirme la acción en el diálogo de confirmación.

> La cita cambiará su estado a **Cancelada**. Esta acción no puede deshacerse.

### 7.5 Marcar una cita como realizada

Una vez atendido el vehículo, el sistema permite registrar la cita como completada:

1. Seleccione la cita de la lista.
2. Haga clic en **Marcar como realizada**.
3. Confirme la acción.

> La cita pasará al estado **Realizada**, lo que indica que el servicio fue prestado correctamente.

---

## 8. Módulo de Sedes

> 🔐 Este módulo está disponible únicamente para el rol **Administrador**.

Gestione las sedes o puntos de atención del taller automotriz.

### 8.1 Consultar sedes

Acceda desde el menú a **Sedes** para ver la lista de todas las sedes activas del taller.

### 8.2 Registrar una nueva sede

1. Haga clic en **+ Nueva Sede**.
2. Complete el formulario:

| Campo | Descripción |
|---|---|
| **Nombre** | Nombre de la sede (ej. "Sede Norte", "Sede Centro") |
| **Dirección** | Dirección física de la sede |
| **Teléfono** | Número de contacto de la sede |
| **Ciudad** | Ciudad donde está ubicada |

3. Haga clic en **Guardar**.

### 8.3 Desactivar una sede

AutoCita utiliza **eliminación lógica**: las sedes no se eliminan permanentemente, sino que se marcan como inactivas. Esto preserva el historial de citas asociadas a esa sede.

1. Seleccione la sede de la lista.
2. Haga clic en **Desactivar**.
3. Confirme la acción.

> Las sedes desactivadas no aparecerán como opción al crear nuevas citas.

---

## 9. Módulo de Recordatorios

El módulo de Recordatorios permite consultar las citas próximas que requieren atención o seguimiento.

### Cómo usar los recordatorios

1. Acceda desde el menú a **Recordatorios**.
2. El sistema mostrará automáticamente las citas programadas para los próximos días.
3. Puede filtrar por rango de fechas para ampliar o reducir el horizonte de consulta.
4. Haga clic en cualquier cita para ver su detalle completo o tomar acciones sobre ella (reprogramar, cancelar, etc.).

> 💡 **Consejo:** Revise el módulo de recordatorios al inicio de cada jornada para anticiparse a las citas del día y de los próximos días.

---

## 10. Módulo de Reportes

> 🔐 Este módulo está disponible para los roles **Administrador** y **Director**.

AutoCita ofrece cuatro tipos de reportes para el análisis de la operación del taller.

### 10.1 Tipos de reportes disponibles

| Reporte | Descripción |
|---|---|
| **Reporte de Citas** | Listado y análisis de citas en un período determinado, con desglose por estado |
| **Reporte por Usuarios/Agentes** | Productividad individual de cada agente: citas atendidas, canceladas y reprogramadas |
| **Reporte por Sedes** | Volumen de citas y productividad por sede del taller |
| **Reporte de Productividad** | Análisis consolidado de metas vs. resultados del contact center |

### 10.2 Cómo generar un reporte

1. Acceda desde el menú a **Reportes**.
2. Seleccione el **tipo de reporte** deseado del listado.
3. Configure los **parámetros del reporte**:
   - **Fecha desde:** Fecha de inicio del período a analizar.
   - **Fecha hasta:** Fecha de fin del período.
   - **Filtro adicional** (según el tipo): agente específico o sede específica.
4. Haga clic en **Generar Reporte**.
5. Los resultados se mostrarán en pantalla en formato tabular.

> 💡 Seleccione rangos de fecha razonables para obtener reportes con tiempo de carga óptimo.

---

## 11. Módulo de Usuarios (Administrador)

> 🔐 Este módulo está disponible únicamente para el rol **Administrador**.

Gestione las cuentas de usuario del sistema.

### 11.1 Consultar usuarios

Acceda desde el menú a **Usuarios** para ver la lista de todos los usuarios activos del sistema, con su nombre, rol y estado.

### 11.2 Crear un nuevo usuario

1. Haga clic en **+ Nuevo Usuario**.
2. Complete el formulario:

| Campo | Descripción |
|---|---|
| **Nombre completo** | Nombre y apellido del colaborador |
| **Nombre de usuario** | Identificador único para inicio de sesión |
| **Contraseña** | Contraseña inicial del usuario |
| **Confirmar contraseña** | Repetir la contraseña |
| **Rol** | Seleccione: Administrador, Director o Agente |
| **Sede** | Sede a la que pertenece el usuario |

3. Haga clic en **Guardar**.

> El usuario recibirá sus credenciales y podrá iniciar sesión inmediatamente.

### 11.3 Desactivar un usuario

Al igual que las sedes, los usuarios no se eliminan permanentemente del sistema. Se desactivan de manera lógica para conservar el historial de sus acciones.

1. Seleccione el usuario de la lista.
2. Haga clic en **Desactivar usuario**.
3. Confirme la acción.

> Un usuario desactivado no podrá iniciar sesión, pero su historial de citas y actividad permanecerá intacto.

### 11.4 Consideraciones de seguridad

- Las contraseñas se almacenan cifradas con BCrypt; ningún administrador puede verlas.
- Si un usuario olvida su contraseña, el administrador puede **restablecerla** desde el formulario de edición del usuario.
- Se recomienda que cada usuario tenga credenciales únicas y no las comparta.

---

## 12. Estados de una cita

Una cita puede encontrarse en uno de los siguientes cuatro estados a lo largo de su ciclo de vida:

| Estado | Descripción | Acciones disponibles |
|---|---|---|
| **Programada** | La cita está activa y pendiente de atención | Reprogramar, Cancelar, Marcar como Realizada |
| **Reprogramada** | La cita fue modificada a una nueva fecha u hora | Reprogramar nuevamente, Cancelar, Marcar como Realizada |
| **Cancelada** | La cita fue anulada por el cliente o el taller | Ninguna (estado final) |
| **Realizada** | El vehículo fue atendido exitosamente | Ninguna (estado final) |

### Diagrama de transición de estados

```
            [PROGRAMADA]
           /             \
          ↓               ↓
   [REPROGRAMADA]      [CANCELADA]
          |
          ↓
      [REALIZADA]
```

---

## 13. Preguntas frecuentes

**¿Puedo registrar el mismo vehículo para dos clientes diferentes?**
No. Cada placa de vehículo es única en el sistema y solo puede estar asociada a un cliente a la vez.

**¿Qué pasa si intento agendar una cita para un vehículo que ya tiene una cita activa el mismo día?**
El sistema mostrará una advertencia indicando que el vehículo ya tiene disponibilidad comprometida para ese horario. Puede elegir una fecha u hora diferente.

**¿Puedo recuperar una cita que fue cancelada?**
No. El estado **Cancelada** es un estado final. Si necesita reagendar al cliente, deberá crear una nueva cita.

**¿Cómo cambio la contraseña de mi cuenta?**
Contacte al administrador del sistema para que restablezca su contraseña. Próximamente esta función estará disponible desde el perfil del usuario.

**¿La aplicación funciona sin conexión a Internet?**
AutoCita requiere conexión al servidor de base de datos PostgreSQL. Si el servidor está en la misma red local (intranet), no necesita conexión a Internet, pero sí acceso a la red interna.

**¿Cómo agrego una nueva sede al sistema?**
Solo los usuarios con rol **Administrador** pueden registrar nuevas sedes desde el menú **Sedes → Nueva Sede**.

**¿Qué ocurre con las citas si desactivo una sede?**
Las citas existentes de una sede desactivada conservan su historial completo. La sede simplemente deja de estar disponible para nuevas citas.

---

## 14. Glosario

| Término | Definición |
|---|---|
| **Cita** | Evento programado en el que un vehículo acude al taller para recibir un servicio |
| **Sede** | Sucursal o punto de atención física del taller automotriz |
| **Agente** | Usuario del contact center responsable de agendar y gestionar citas |
| **Director** | Usuario con rol de supervisión del contact center y acceso a reportes |
| **Administrador** | Usuario con control total del sistema, incluyendo usuarios y sedes |
| **Onboarding** | Proceso de configuración inicial de la aplicación en su primera ejecución |
| **Reprogramación** | Cambio de la fecha u hora de una cita ya existente |
| **Eliminación lógica** | Desactivación de un registro sin borrarlo físicamente de la base de datos |
| **BCrypt** | Algoritmo de cifrado utilizado para proteger las contraseñas de los usuarios |
| **PostgreSQL** | Sistema de gestión de base de datos utilizado por AutoCita |
| **EF Core** | Entity Framework Core, librería ORM para el acceso a la base de datos |

---

*Manual de Usuario — AutoCita v1.0 · © 2026 · Sistema de gestión de citas para talleres automotrices*
