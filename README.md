# NEXUS — Academia de Exploradores

NEXUS es una aventura de ciencia ficción en C# ambientada en el año 2297. El jugador interpreta a un cadete que explora realidades, administra recursos, investiga anomalías de IRIS y resuelve misiones.

> **Estado actual:** el código H3 es una aplicación de consola en .NET 8. Windows Forms y SQL Server forman parte de la arquitectura propuesta y todavía deben implementarse.

## Funcionalidades implementadas en H3

- Registro y validación de datos del cadete.
- Entrenamiento de ecos temporales, energía, estabilidad, fragmentos de IRIS y celdas de reserva.
- Alertas de anomalía y desconexión automática por estado crítico.
- Modo de misión disponible mediante `--v2`: mapa de cuatro zonas, exploración, encuentros, inventario, objetos, experiencia y misión “La anomalía perdida”.
- Pruebas automatizadas ejecutables mediante `--pruebas`.

## Tecnologías actuales

- C# y .NET 8
- Visual Studio
- Git y GitHub

## Requisitos y ejecución

Instalar el SDK de .NET 8 o Visual Studio con soporte para ese SDK. Clonar el repositorio:

```bash
git clone https://github.com/cbbeluismariomedranoze-jpg/NEXUS_Entrenamiento.git
cd NEXUS_Entrenamiento
```

Abrir `NEXUS_Entrenamiento.sln` en Visual Studio y ejecutar con F5 o Ctrl+F5. También puede usarse la terminal:

```bash
# Modo de entrenamiento H3
dotnet run --project NexusTraining

# Misión "La anomalía perdida"
dotnet run --project NexusTraining -- --v2

# Pruebas automatizadas
dotnet run --project NexusTraining -- --pruebas
```

## Estructura relevante

- `NexusTraining/Program.cs`: punto de entrada, menús de consola y validación de entrada.
- `NexusTraining/Simulacion.cs`: reglas del entrenamiento, energía, estabilidad y eventos IRIS.
- `NexusTraining/NexusV2.cs`: personajes, inventario, misión, mapa y modo de consola de la misión.
- `NexusTraining/Pruebas.cs`: pruebas automatizadas.
- `NexusTraining/NexusTraining.csproj`: configuración del proyecto .NET 8.
- `docs/`: documentación técnica, diseño de juego, diagramas y wireframes.

## Documentación de esta evolución

1. [Diseño del videojuego](docs/diseno-del-juego.md): visión, ciclo, reglas, pantallas, UX y alcance.
2. [Arquitectura actual](docs/arquitectura-actual.md): archivos, clases, funcionalidades y limitaciones reales de H3.
3. [Arquitectura propuesta](docs/arquitectura-propuesta.md): capas, componentes, diagramas y plan de migración.
4. [Modelo de datos propuesto](docs/modelo-de-datos-propuesto.md): entidades, relaciones y estrategia de persistencia SQL Server.
5. [Análisis técnico](docs/analisis-tecnico.md): alternativas, decisiones, riesgos, verificación de IA y fuentes.
6. [Bocetos y diagramas](docs/evidencias/bocetos-y-diagramas.md): cuatro pantallas esquemáticas y navegación.

## Próximas etapas

1. Ejecutar y preservar las pruebas de regresión.
2. Separar las clases del archivo `NexusV2.cs` sin alterar reglas.
3. Crear servicios de aplicación y formularios Windows Forms.
4. Implementar el esquema SQL Server y un repositorio para guardar/cargar partidas.
5. Probar el recorrido completo, desde iniciar partida hasta guardar y continuar.

No se debe guardar ninguna contraseña ni cadena de conexión privada en el repositorio. La base de datos y la interfaz gráfica aún no forman parte del código H3 publicado.

## Autor

Luis Mario Medrano Zeballos  
Materia: Programación II

## H4.01 — Primera interfaz gráfica Windows Forms

La solución existente ahora incluye el proyecto `NexusWindowsForms` junto con el proyecto de consola `NexusTraining`. No se reemplazó la versión de consola.

### Ejecutar la interfaz gráfica

Requisito: Windows y .NET 8 SDK con soporte para `net8.0-windows`.

1. Abrir `NEXUS_Entrenamiento.sln` en Visual Studio.
2. En el Explorador de soluciones, hacer clic derecho en `NexusWindowsForms` y elegir **Establecer como proyecto de inicio**.
3. Ejecutar con F5.
4. Desde el menú, usar **Iniciar entrenamiento** o **La anomalía perdida** para abrir el modo de consola original. El botón de documentación abre los documentos publicados en GitHub.

También se puede ejecutar desde la terminal de Windows:

```powershell
dotnet build NEXUS_Entrenamiento.sln
dotnet run --project NexusWindowsForms/NexusWindowsForms.csproj
```

Para ejecutar la consola original:

```powershell
dotnet run --project NexusTraining/NexusTraining.csproj
dotnet run --project NexusTraining/NexusTraining.csproj -- --v2
dotnet run --project NexusTraining/NexusTraining.csproj -- --pruebas
```

**Nota de validación:** los archivos se han publicado en el repositorio; antes de entregar, compilar y ejecutar en Visual Studio en Windows y realizar la clonación limpia exigida por la práctica. Las capturas del laboratorio deben ser reales, con fecha y hora visibles.

