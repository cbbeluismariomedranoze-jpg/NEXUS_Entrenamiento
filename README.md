# NEXUS — Academia de Exploradores

NEXUS es un videojuego de ciencia ficción desarrollado en C# y .NET 8, ambientado en el año 2297. El jugador interpreta a un cadete que explora diferentes realidades, administra recursos, investiga anomalías mediante IRIS y completa misiones.

## Estado actual del proyecto — Hito 4

El proyecto cuenta con una aplicación de consola, una interfaz gráfica desarrollada con Windows Forms y un sistema de memoria persistente conectado a SQL Server.

La finalidad del Hito 4 es permitir que el progreso del jugador permanezca almacenado incluso después de cerrar completamente la aplicación.

## Funcionalidades implementadas

### Videojuego
- Registro y validación del cadete.
- Administración de vida, energía y experiencia.
- Exploración de cuatro zonas.
- Encuentros con entidades.
- Inventario y utilización de objetos.
- Sistema de misiones y progreso.
- Interacción con IRIS.

### Interfaz gráfica Windows Forms
- Menú principal de NEXUS.
- Creación de nuevas partidas.
- Consulta de partidas guardadas.
- Carga y continuación de partidas.
- Guardado manual y automático del progreso.
- Eliminación de partidas.
- Comprobación de conexión con SQL Server.

### Memoria persistente
- Registro de cadetes.
- Almacenamiento de partidas.
- Recuperación del estado del jugador.
- Persistencia del inventario.
- Almacenamiento de zonas visitadas.
- Recuperación del progreso de las misiones.
- Conservación del estado de los encuentros.

Se realizaron pruebas cerrando y abriendo nuevamente la aplicación para comprobar la recuperación de los datos almacenados.

## Tecnologías utilizadas

- C#
- .NET 8
- Windows Forms
- Microsoft SQL Server Express
- SQL Server Management Studio (SSMS)
- Microsoft.Data.SqlClient
- Visual Studio
- Git y GitHub

## Base de datos NEXUS_DB

La base de datos contiene seis tablas relacionadas:

| Tabla | Función |
|---|---|
| Cadetes | Información de los exploradores |
| Partidas | Estado y recursos de cada partida |
| Objetos | Catálogo de objetos del juego |
| InventarioPartida | Objetos y cantidades de cada partida |
| ProgresoMision | Estado de las misiones |
| ZonasVisitadas | Registro de zonas exploradas |

Se utilizan claves primarias, claves foráneas, restricciones y relaciones para mantener la integridad de los datos.

## Requisitos de instalación

- Windows.
- .NET 8 SDK.
- Visual Studio con soporte para Windows Forms.
- SQL Server Express.
- SQL Server Management Studio.

La aplicación utiliza autenticación de Windows para conectarse a la instancia local de SQL Server.

La cadena de conexión debe corresponder a la instancia configurada en cada equipo.

## Instalación y ejecución

Clonar el repositorio:

```bash
git clone https://github.com/cbbeluismariomedranoze-jpg/NEXUS_Entrenamiento.git
```

Abrir el archivo `NEXUS_Entrenamiento.sln` en Visual Studio.

### Configurar la base de datos

1. Abrir SQL Server Management Studio.
2. Conectarse a la instancia de SQL Server.
3. Crear la base de datos `NEXUS_DB`.
4. Ejecutar el script de creación de tablas y datos iniciales.
5. Verificar la cadena de conexión en `ConexionBD.cs`.

### Ejecutar Windows Forms

1. Abrir el Explorador de soluciones.
2. Seleccionar `NexusWindowsForms`.
3. Establecerlo como proyecto de inicio.
4. Ejecutar con F5.

### Ejecutar la versión de consola

```bash
dotnet run --project NexusTraining
```

Modo de misión:

```bash
dotnet run --project NexusTraining -- --v2
```

Pruebas automatizadas:

```bash
dotnet run --project NexusTraining -- --pruebas
```

## Estructura del proyecto

- `NexusTraining/`: lógica principal del videojuego.
- `NexusTraining/NexusV2.cs`: personajes, inventario, misiones, mapa y encuentros.
- `NexusTraining/Program.cs`: punto de entrada de la aplicación de consola.
- `NexusTraining/Simulacion.cs`: reglas del entrenamiento.
- `NexusTraining/Pruebas.cs`: pruebas automatizadas.
- `NexusWindowsForms/`: interfaz gráfica.
- `NexusWindowsForms/MainForm.cs`: menú principal y gestión de las partidas.
- `NexusWindowsForms/ConexionBD.cs`: conexión con SQL Server.
- `NexusWindowsForms/RepositorioNexus.cs`: operaciones de creación, guardado, carga y eliminación de partidas.
- `docs/`: documentación técnica y diagramas.

## Prueba de memoria persistente

Se realizaron las siguientes comprobaciones:

1. Crear una nueva partida.
2. Modificar la zona y los recursos del jugador.
3. Guardar el progreso en SQL Server.
4. Cerrar completamente la aplicación.
5. Abrir nuevamente NEXUS.
6. Cargar la partida guardada.
7. Verificar la recuperación de energía, zona, inventario y progreso.
8. Comprobar la eliminación de partidas.

Los resultados confirmaron que los datos se conservan después de cerrar y volver a abrir la aplicación.

## Documentación del proyecto

1. [Diseño del videojuego](docs/diseno-del-juego.md).
2. [Arquitectura actual](docs/arquitectura-actual.md).
3. [Arquitectura propuesta](docs/arquitectura-propuesta.md).
4. [Modelo de datos](docs/modelo-de-datos-propuesto.md).
5. [Análisis técnico](docs/analisis-tecnico.md).
6. [Bocetos y diagramas](docs/evidencias/bocetos-y-diagramas.md).

## Seguridad

No se deben publicar contraseñas, credenciales ni datos privados de conexión. Cada usuario debe configurar su propia instancia de SQL Server.

## Autor

**Luis Mario Medrano Zeballos**

Materia: Programación II

---

**NEXUS — Hito 4: Operación Memoria**

Sistema de exploración con almacenamiento persistente de partidas mediante C#, Windows Forms y SQL Server.
