# 🚀 NEXUS — Academia de Exploradores

## Descripción del proyecto

NEXUS es un videojuego de consola desarrollado en C# con Visual Studio para la materia de Programación II, Hito 3.

La historia se desarrolla en el año 2297. El jugador asume el papel de un cadete explorador que debe ingresar a una realidad desconocida, administrar su energía y estabilidad, explorar ecos temporales y tomar decisiones para completar su entrenamiento.

## Objetivo

Aplicar conceptos de programación en C# mediante un sistema interactivo que permita controlar el estado de un explorador y responder a diferentes situaciones.

## Funcionalidades principales

- Registro del cadete y validación de ingreso.
- Exploración de ecos temporales.
- Decodificación de señales de IRIS.
- Consulta del estado del explorador.
- Recuperación de energía mediante celdas.
- Recalibración de estabilidad.
- Alertas de anomalías y desconexión automática.
- Pruebas automatizadas del funcionamiento.

## Tecnologías utilizadas

- C#
- .NET 8
- Visual Studio
- Git y GitHub

## Estructura del proyecto

- `Program.cs`: inicia el programa, recibe datos y muestra el menú.
- `Simulacion.cs`: administra las reglas y el estado del entrenamiento.
- `Pruebas.cs`: contiene 12 pruebas automatizadas.
- `NexusTraining.csproj`: configuración del proyecto.
- `NEXUS_Entrenamiento.sln`: solución de Visual Studio.

## Cómo ejecutar NEXUS

1. Descargar o clonar el repositorio.
2. Abrir `NEXUS_Entrenamiento.sln` en Visual Studio.
3. Tener instalado el SDK de .NET 8.
4. Presionar `F5` o el botón de iniciar.
5. Ingresar los datos del cadete y seleccionar las operaciones del menú.

Para ejecutar las pruebas desde la carpeta raíz del repositorio:

```bash
dotnet run --project NexusTraining -- --pruebas
```

## Conceptos de Programación II

El proyecto utiliza clases, objetos, constructores, encapsulamiento, propiedades, métodos, colecciones genéricas, expresiones lambda, eventos y manejo de excepciones.

Los conceptos de herencia, polimorfismo, interfaces y LINQ se desarrollan como ejemplos complementarios en el manual técnico.

## Documentación

## Documentación del proyecto

El proyecto **NEXUS_Entrenamiento** cuenta con documentación técnica y evidencias de funcionamiento.

En la carpeta `Docs` se encuentran:

- **NEXUS_Documentacion.pdf:** documentación técnica del proyecto.
- **Evidencia_Pruebas_NEXUS.png:** captura de los resultados de las pruebas automatizadas.
- **README.md:** información de la documentación.

El sistema fue desarrollado en **C# con Visual Studio** y cuenta con seis operaciones funcionales.

Se ejecutaron **12 pruebas automatizadas**, obteniendo 12 aprobadas y 0 fallidas.

## Autor

Luis Mario Medrano Zeballos

Carrera: Ingeniería en Sistemas

Materia: Programación II
