# NEXUS Entrenamiento

Aplicación de consola en C# para la misión de entrenamiento del Hito 2.

## Abrir y ejecutar

1. Extraer todo el ZIP.
2. Abrir `NEXUS_Entrenamiento.sln` en Visual Studio con soporte para .NET 8 y la carga de trabajo de desarrollo de escritorio de .NET.
3. Ejecutar con Ctrl+F5.
4. Ejemplo: Luis Mario Medrano Zeballos; edad 20; realidad LUMEN-07; energía 80; estabilidad 80. La edad es un dato de prueba, no una afirmación sobre el estudiante.

Con el SDK de .NET 8 también se puede ejecutar desde esta carpeta:

```bash
dotnet run --project NexusTraining
```

## Pruebas reproducibles

```bash
dotnet run --project NexusTraining -- --pruebas
```

La batería incluye las seis pruebas obligatorias y seis comprobaciones adicionales. No requiere paquetes NuGet de terceros. El archivo RESULTADOS_PRUEBAS.txt contiene la salida de la ejecución realizada durante la preparación.

## Archivos

- `Program.cs`: interfaz, registro, ciclo del menú, validación y control de fin de entrada.
- `Simulacion.cs`: estado, reglas, evento IRIS y corte de emergencia.
- `Pruebas.cs`: pruebas automáticas y una sesión completa con entrada redirigida.
- `NEXUS_Documentacion.pdf`: documentación breve con reglas, explicación y resultados.

## Reglas

- Admisión: edad mínima 16, energía mínima 40 y estabilidad mínima 60.
- Nombre y realidad: 1 a 60 caracteres no vacíos. Edad de 1 a 120; energía y estabilidad de 0 a 100.
- Cartografiar: energía -12, estabilidad -8. Desde la segunda exploración seguida: energía -19 y estabilidad -14. Consultar estado no rompe esta secuencia; analizar, recuperar o recalibrar con éxito sí la rompe.
- Decodificar: energía -18, estabilidad -12; revela un fragmento de IRIS.
- Consultar: no modifica datos.
- Celda: energía hasta +22 (máximo 100), estabilidad -4. Hay tres celdas. No se consume una celda cuando la energía está en 100.
- Recalibrar: energía -10, estabilidad hasta +18 (máximo 100). No consume energía si la estabilidad ya está en 100.
- Toda operación con energía insuficiente se rechaza sin cambiar el estado.
- IRIS: estabilidad <= 30 O (energía <= 20 Y estabilidad <= 45). Emite una alerta por episodio; puede reactivarse si primero se sale del riesgo.
- Corte: estabilidad <= 10 O (energía == 0 Y estabilidad <= 25).
- Opción 5: cierre voluntario. El fin de la entrada también cierra de forma controlada.

Todos los umbrales son reglas ficticias de diseño, no protocolos reales de seguridad. No hay azar: las mismas entradas producen los mismos estados, lo que facilita explicar y probar el programa.

## Recorrido para provocar IRIS

Registrar edad 20, energía 100 y estabilidad 60. Seleccionar 2 tres veces: energía 46, estabilidad 24 y alerta IRIS. Seleccionar 2 otras dos veces: estabilidad 0 y corte automático.

## Alcance

Entrega correspondiente al enunciado de NEXUS. No contiene una parte Written no proporcionada ni capturas inventadas de consultas a IA. La documentación identifica el apoyo de IA y las fuentes consultadas. No incluye binarios ni las carpetas generadas bin/obj: Visual Studio los crea al compilar.
