namespace NexusTraining;

internal sealed class Simulacion
{
    public string Nombre { get; }
    public int Edad { get; }
    public string Realidad { get; }
    public int Energia { get; private set; }
    public int Estabilidad { get; private set; }
    public int Ecos { get; private set; }
    public int FragmentosIris { get; private set; }
    public int Celdas { get; private set; } = 3;
    public int Alertas { get; private set; }
    public bool Autorizada { get; }
    public bool Conectada { get; private set; }
    public string Motivo { get; private set; } = "Sesión activa";
    private int exploracionesSeguidas;
    private bool irisActiva;
    public event EventHandler? AnomaliaIris;

    public Simulacion(string nombre, int edad, string realidad, int energia, int estabilidad)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 60)
            throw new ArgumentException("Nombre inválido.", nameof(nombre));
        if (string.IsNullOrWhiteSpace(realidad) || realidad.Length > 60)
            throw new ArgumentException("Realidad inválida.", nameof(realidad));
        if (edad < 1 || edad > 120) throw new ArgumentOutOfRangeException(nameof(edad));
        if (energia < 0 || energia > 100) throw new ArgumentOutOfRangeException(nameof(energia));
        if (estabilidad < 0 || estabilidad > 100) throw new ArgumentOutOfRangeException(nameof(estabilidad));
        Nombre = nombre.Trim(); Edad = edad; Realidad = realidad.Trim();
        Energia = energia; Estabilidad = estabilidad;
        Autorizada = edad >= 16 && energia >= 40 && estabilidad >= 60;
        Conectada = Autorizada;
        if (!Autorizada)
        {
            var causas = new List<string>();
            if (edad < 16) causas.Add("edad menor de 16");
            if (energia < 40) causas.Add("energía menor de 40");
            if (estabilidad < 60) causas.Add("estabilidad menor de 60");
            Motivo = string.Join("; ", causas);
        }
    }

    public string Estado() =>
        $"EXPLORADOR: {Nombre} | EDAD: {Edad}\nREALIDAD: {Realidad}\n" +
        $"ENERGÍA: {Energia}/100 | ESTABILIDAD: {Estabilidad}/100\n" +
        $"ECOS: {Ecos} | FRAGMENTOS IRIS: {FragmentosIris} | CELDAS: {Celdas}";

    public string Operar(int opcion)
    {
        if (!Conectada) return "Sesión cerrada: no se pueden ejecutar operaciones.";
        string resultado;
        switch (opcion)
        {
            case 1:
                // Segunda exploración consecutiva en adelante: sobrecarga del ancla.
                int costo = exploracionesSeguidas >= 1 ? 19 : 12;
                int perdida = exploracionesSeguidas >= 1 ? 14 : 8;
                if (Energia < costo) return $"Energía insuficiente: se requieren {costo} puntos.";
                Energia -= costo; Estabilidad = Math.Max(0, Estabilidad - perdida);
                exploracionesSeguidas++; Ecos++;
                resultado = $"Eco cartografiado. Energía -{costo}; estabilidad -{perdida}.";
                break;
            case 2:
                if (Energia < 18) return "Energía insuficiente: decodificar requiere 18 puntos.";
                Energia -= 18; Estabilidad = Math.Max(0, Estabilidad - 12);
                FragmentosIris++; exploracionesSeguidas = 0;
                resultado = FragmentosIris == 1
                    ? "Fragmento IRIS 1: los ecos conservan rastros de otros cadetes."
                    : "IRIS: el simulador responde a tus decisiones, no al azar.";
                resultado += " Energía -18; estabilidad -12.";
                break;
            case 3:
                // Consultar no altera el estado ni elimina la penalización por exploración.
                return Estado();
            case 4:
                if (Celdas == 0) return "No quedan celdas de reserva.";
                if (Energia == 100) return "Energía al máximo. No se consume ninguna celda.";
                int recuperada = Math.Min(22, 100 - Energia);
                Energia += recuperada; Estabilidad = Math.Max(0, Estabilidad - 4);
                Celdas--; exploracionesSeguidas = 0;
                resultado = $"Celda utilizada: energía +{recuperada}; estabilidad -4.";
                break;
            case 5:
                Conectada = false; Motivo = "Desconexión voluntaria y segura";
                return "ENLACE CERRADO. Desconexión completada.";
            case 6:
                if (Energia < 10) return "Energía insuficiente: recalibrar requiere 10 puntos.";
                if (Estabilidad == 100) return "El ancla ya está completamente estable.";
                Energia -= 10; Estabilidad = Math.Min(100, Estabilidad + 18);
                exploracionesSeguidas = 0;
                resultado = "Ancla recalibrada: energía -10; estabilidad hasta +18.";
                break;
            default:
                return "Operación inválida: elige una opción del 1 al 6.";
        }
        EvaluarSeguridad();
        if (!Conectada) resultado += "\nCORTE DE EMERGENCIA: " + Motivo;
        return resultado;
    }

    private void EvaluarSeguridad()
    {
        // Nivel élite: intervienen energía Y estabilidad, además del historial de acciones.
        bool riesgoIris = Estabilidad <= 30 || (Energia <= 20 && Estabilidad <= 45);
        if (riesgoIris && !irisActiva)
        {
            Alertas++;
            AnomaliaIris?.Invoke(this, EventArgs.Empty);
        }
        irisActiva = riesgoIris; // Se permite una nueva alerta tras salir y volver a entrar al riesgo.
        if (Estabilidad <= 10 || (Energia == 0 && Estabilidad <= 25))
        {
            Conectada = false;
            Motivo = "Protección automática por estado crítico";
        }
    }
}
