using System.Text;

namespace NexusTraining;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        try
        {
            if (args.Contains("--pruebas")) return Pruebas.Ejecutar();
            EjecutarSesion();
            return 0;
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine("\nEntrada cerrada. NEXUS finaliza la sesión de forma segura.");
            return 0;
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine("No se pudo continuar con la entrada/salida: " + ex.Message);
            return 1;
        }
    }

    internal static void EjecutarSesion()
    {
        Console.WriteLine("================================================");
        Console.WriteLine(" NEXUS | ACADEMIA DE EXPLORADORES | AÑO 2297");
        Console.WriteLine(" PROTOCOLO UMBRAL: entrenamiento supervisado");
        Console.WriteLine("================================================");
        Console.WriteLine("Realidad sugerida: LUMEN-07, un archivo de ecos temporales.");
        Console.WriteLine("Ingreso: edad >= 16, energía >= 40, estabilidad >= 60.");
        string nombre = Entrada.Texto("Nombre del cadete: ", 60);
        int edad = Entrada.Entero("Edad (1 a 120): ", 1, 120);
        string realidad = Entrada.Texto("Realidad asignada: ", 60);
        int energia = Entrada.Entero("Energía (0 a 100): ", 0, 100);
        int estabilidad = Entrada.Entero("Estabilidad (0 a 100): ", 0, 100);
        var sesion = new Simulacion(nombre, edad, realidad, energia, estabilidad);

        if (!sesion.Autorizada)
        {
            Console.WriteLine("INMERSIÓN RECHAZADA: " + sesion.Motivo);
            return;
        }

        // El publicador es Simulacion; este manejador reacciona al evento opcional.
        sesion.AnomaliaIris += (_, _) =>
        {
            Console.WriteLine("\n******** ANOMALÍA IRIS: INTERFERENCIA ********");
            Console.WriteLine("IRIS: tus ecos están alterando la conexión.");
            Console.WriteLine("Recomendación: recalibrar (6) o desconectar (5).");
            Console.WriteLine("********************************************");
        };
        Console.WriteLine("INMERSIÓN AUTORIZADA. Enlace de entrenamiento establecido.");
        while (sesion.Conectada)
        {
            Console.WriteLine("\n---------------- NEXUS UMBRAL ----------------");
            Console.WriteLine(sesion.Estado());
            Console.WriteLine("1. Cartografiar un eco temporal");
            Console.WriteLine("2. Decodificar una señal de IRIS");
            Console.WriteLine("3. Consultar bitácora de estado");
            Console.WriteLine("4. Usar una celda de reserva");
            Console.WriteLine("5. Cerrar el enlace de entrenamiento");
            Console.WriteLine("6. Recalibrar el ancla dimensional");
            int opcion = Entrada.Entero("Operación (1 a 6): ", 1, 6);
            Console.WriteLine(sesion.Operar(opcion));
            if (sesion.Conectada && (sesion.Energia <= 20 || sesion.Estabilidad <= 45))
                Console.WriteLine("ADVERTENCIA: reservas o enlace bajos; revisa tu estrategia.");
        }
        Console.WriteLine("\nRESUMEN FINAL\n" + sesion.Estado());
        Console.WriteLine("Motivo de cierre: " + sesion.Motivo);
        Console.WriteLine(sesion.Alertas > 0
            ? "TRANSMISIÓN NARRATIVA: IRIS dejó un eco fuera del registro. ACTIVIDAD DETECTADA."
            : "TRANSMISIÓN FINAL: entrenamiento concluido. NEXUS mantiene la vigilancia.");
    }
}

internal static class Entrada
{
    internal static string Texto(string mensaje, int maximo)
    {
        while (true)
        {
            Console.Write(mensaje);
            string texto = (Console.ReadLine() ?? throw new EndOfStreamException()).Trim();
            if (texto.Length >= 1 && texto.Length <= maximo) return texto;
            Console.WriteLine($"Escribe entre 1 y {maximo} caracteres; no dejes el campo vacío.");
        }
    }

    internal static int Entero(string mensaje, int minimo, int maximo)
    {
        while (true)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine() ?? throw new EndOfStreamException();
            if (int.TryParse(texto, out int numero) && numero >= minimo && numero <= maximo)
                return numero;
            Console.WriteLine($"Entrada inválida. Introduce un entero entre {minimo} y {maximo}.");
        }
    }
}
