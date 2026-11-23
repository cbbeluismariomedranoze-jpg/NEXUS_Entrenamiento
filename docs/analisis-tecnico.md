# Análisis técnico de la evolución de NEXUS

## 1. Punto de partida verificado
El proyecto existente usa C#/.NET 8 y es una aplicación de consola. Las clases principales están en Simulacion.cs y NexusV2.cs; Pruebas.cs permite ejecutar pruebas con --pruebas. No hay WinForms ni SQL Server configurados. La interfaz gráfica y la persistencia son trabajo futuro, no funcionalidades ya implementadas.

## 2. Alternativas de interfaz

| Alternativa | Ventajas | Desventajas | Decisión |
|---|---|---|---|
| **Windows Forms** | Diseñador visual en Visual Studio, controles de escritorio, curva razonable y coincide con el enunciado. | Orientado a Windows; gráficos avanzados requieren trabajo adicional. | **Recomendada para el MVP.** |
| WPF | XAML y opciones flexibles de presentación/enlace de datos. | Añade curva de aprendizaje y no es necesario para la primera entrega. | Alternativa futura. |
| Unity | Herramientas de juego, escenas, animación y gráficos 2D/3D. | Más complejidad y no cumple el requisito específico de Windows Forms. | No recomendado en este hito. |

Microsoft documenta WinForms como marco para aplicaciones cliente de escritorio Windows, con controles, eventos y diseñador visual en Visual Studio [1]. Es adecuado para menú, mapa, misiones e inventario.

## 3. Alternativas de persistencia

| Alternativa | Ventajas | Desventajas | Decisión |
|---|---|---|---|
| **ADO.NET + Microsoft.Data.SqlClient** | Acceso explícito a SQL Server, parámetros y transacciones, control claro para una tarea académica. | Hay que escribir SQL y mapear resultados. | **Recomendada inicialmente.** |
| Entity Framework Core | ORM, entidades y consultas LINQ; reduce código repetitivo. | Añade configuración, seguimiento de entidades y migraciones. | Posible evolución. |
| JSON local | Útil para prototipos sin servidor. | No cumple el requisito final de SQL Server ni facilita consultas relacionales. | Solo pruebas temporales. |

Microsoft identifica Microsoft.Data.SqlClient como proveedor de datos .NET para SQL Server y documenta conexiones, comandos y transacciones [2]. SQL Server almacena datos estructurados en tablas relacionales [3].

## 4. Arquitectura recomendada
Windows Forms → servicios de aplicación → dominio → interfaces de repositorio → infraestructura SQL Server. La interfaz no implementa reglas de daño, costo, inventario o recompensa. Los servicios coordinan acciones; el dominio determina si son válidas. Así se pueden probar las reglas sin formularios ni base de datos.

## 5. Riesgos y mitigación

| Riesgo | Mitigación |
|---|---|
| Cambiar reglas al extraer clases | Registrar pruebas de regresión antes y después. |
| Mezclar Simulacion y MundoNexus | Servicios separados y modo de juego explícito. |
| Perder progreso | Transacciones, guardado de acciones aceptadas y pruebas tras reiniciar. |
| Inyección SQL o credenciales expuestas | Consultas parametrizadas, permisos mínimos y cadena externa a Git. |
| Fallo de conexión | Manejo de excepciones y conservación del último estado confirmado. |
| Alcance excesivo | Completar primero cuatro pantallas y una misión completa. |

## 6. Verificación técnica
1. Ejecutar entrenamiento: dotnet run --project NexusTraining
2. Ejecutar misión: dotnet run --project NexusTraining -- --v2
3. Ejecutar pruebas: dotnet run --project NexusTraining -- --pruebas
4. Registrar pruebas de admisión, energía, alertas IRIS, movimiento, encuentros, objetos y finalización.
5. Tras crear WinForms, verificar navegación, teclado, validaciones y mensajes.
6. En SQL Server, probar guardar/cargar, fallo de conexión, reversión de transacción y continuación tras cerrar.
7. Revisar commits para evitar secretos y carpetas bin/obj.

## 7. Verificación de información obtenida con IA
La IA se utilizó como apoyo para estructurar alternativas, redactar y preparar diagramas. Las afirmaciones sobre el sistema actual se contrastaron con los archivos reales Program.cs, Simulacion.cs, NexusV2.cs, Pruebas.cs y NexusTraining.csproj. Las recomendaciones técnicas se contrastaron con documentación oficial de Microsoft [1–4]. El esquema de datos es una propuesta, no una base implementada. No se afirma que la conexión o las pantallas estén probadas; se deben adjuntar evidencias reales después de ejecutarlas.

## 8. Fuentes técnicas
1. Microsoft Learn, Windows Forms overview: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/
2. Microsoft Learn, Microsoft.Data.SqlClient for SQL Server: https://learn.microsoft.com/en-us/sql/connect/ado-net/microsoft-ado-net-sql-server
3. Microsoft Learn, What is SQL Server?: https://learn.microsoft.com/en-us/sql/sql-server/what-is-sql-server
4. Microsoft Learn, Windows Forms documentation: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/
5. Código fuente revisado: https://github.com/cbbeluismariomedranoze-jpg/NEXUS_Entrenamiento

Fuentes consultadas el 23 de noviembre de 2026. Volver a verificar versiones de SDK y paquetes al implementar, porque la documentación oficial se actualiza.
