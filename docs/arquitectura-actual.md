# Arquitectura actual de NEXUS H3

## 1. Evidencia revisada
Se revisaron los archivos de la rama main del repositorio, especialmente NexusTraining.csproj, Program.cs, Simulacion.cs, NexusV2.cs y Pruebas.cs. El proyecto configura .NET 8, salida ejecutable de consola, referencias implícitas y tipos anulables. No contiene formularios WinForms, acceso a datos ni conexión SQL Server.

## 2. Archivos y responsabilidades

| Archivo | Responsabilidad comprobada |
|---|---|
| NexusTraining/Program.cs | Punto de entrada. Ejecuta entrenamiento por defecto; activa NexusV2 con --v2 y pruebas con --pruebas. Contiene Entrada para validar texto y números. |
| NexusTraining/Simulacion.cs | Estado y reglas del entrenamiento: cadete, realidad, energía, estabilidad, ecos, fragmentos IRIS, celdas, alertas, autorización, conexión y evento AnomaliaIris. |
| NexusTraining/NexusV2.cs | Contiene clases de dominio de la misión y la interfaz de consola de esa misión. |
| NexusTraining/Pruebas.cs | Pruebas automatizadas sin paquetes externos. |
| NexusTraining/NexusTraining.csproj | Configuración de proyecto ejecutable .NET 8. |
| NEXUS_Entrenamiento.sln | Solución para Visual Studio. |

## 3. Clases reales
En Simulacion.cs:
- **Simulacion:** valida los datos; aplica requisitos de inmersión; administra energía, estabilidad, ecos, fragmentos IRIS, celdas y alertas; dispara AnomaliaIris y realiza desconexión o corte de emergencia.
- **Entrada** (Program.cs): valida texto no vacío y números dentro de rangos.

En NexusV2.cs:
- **Personaje:** clase abstracta con nombre, vida, energía, estado vivo, descripción y daño.
- **Cadete:** hereda de Personaje; añade experiencia, escudo, ataque extra y mochila; gestiona energía, protección y experiencia.
- **Robot** y **Enemigo:** tipos concretos de personaje con valores iniciales y descripción.
- **Inventario:** diccionario de cantidades; consulta, agrega, quita y muestra objetos.
- **TipoObjeto:** enum Celula, Escudo, ModuloAtaque, Analizador, Nucleo y ModuloDesconocido.
- **Mision:** registra zona alcanzada, núcleo encontrado, encuentro superado y finalización; entrega experiencia y recompensa.
- **MundoNexus:** controla cuatro zonas, movimiento, exploración aleatoria, encuentros, uso/eliminación de objetos y estado de partida.
- **NexusV2:** menú de consola que traduce opciones del jugador a operaciones de MundoNexus.

## 4. Flujo actual
```mermaid
flowchart TD
 A[Program.Main] --> B{Argumentos}
 B -- sin argumentos --> C[EjecutarSesion]
 C --> D[Entrada de datos]
 D --> E[Simulacion]
 E --> F[Operar y evaluar seguridad]
 B -- --v2 --> G[NexusV2.Ejecutar]
 G --> H[MundoNexus]
 H --> I[Cadete / Mision / Inventario / Personaje]
 B -- --pruebas --> J[Pruebas.Ejecutar]
```

## 5. Limitaciones reales
1. Interfaz textual con Console.ReadLine/WriteLine; no hay ventanas ni controles gráficos.
2. Estado en memoria: no se conserva la partida en SQL Server.
3. Hay dos recorridos relacionados pero distintos: Simulacion (energía, estabilidad, IRIS) y MundoNexus (zonas, encuentros, inventario y experiencia). La evolución debe definir cómo se presentan sin mezclar reglas incompatibles.
4. NexusV2.cs reúne varias clases y la interfaz de consola; conviene separar responsabilidades gradualmente.
5. No existe capa de persistencia, repositorios, cadena de conexión ni esquema de datos.
6. Las pruebas actuales se ejecutan en consola; faltan pruebas de integración con base de datos.
7. MundoNexus.Explorar usa Random, por lo que las pruebas de resultados específicos deben aislar la variabilidad.

## 6. Continuidad
Primero se preservarán las reglas de Simulacion y MundoNexus. Se recomienda separar las clases a archivos individuales sin alterar su comportamiento, introducir servicios de juego y luego crear la interfaz WinForms. La consola puede mantenerse para comprobar regresiones.
