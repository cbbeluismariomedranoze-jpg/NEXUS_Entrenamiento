using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusTraining;

internal abstract class Personaje
{
    public string Nombre { get; }
    public int Vida { get; protected set; }
    public int Energia { get; protected set; }
    protected Personaje(string nombre, int vida, int energia)
    { Nombre = nombre; Vida = vida; Energia = energia; }
    public bool Vivo => Vida > 0;
    public virtual string Describir() => $"{Nombre} | Vida {Vida} | Energía {Energia}";
    public virtual void RecibirDanio(int cantidad) => Vida = Math.Max(0, Vida - Math.Max(0, cantidad));
}

internal sealed class Cadete : Personaje
{
    public int Experiencia { get; private set; }
    public int Escudo { get; private set; }
    public int AtaqueExtra { get; private set; }
    public Inventario Mochila { get; } = new();
    public Cadete(string nombre) : base(nombre, 100, 70) { }
    public override void RecibirDanio(int cantidad)
    {
        int absorbido = Math.Min(Escudo, Math.Max(0, cantidad));
        Escudo -= absorbido;
        base.RecibirDanio(cantidad - absorbido);
    }
    public void CurarEnergia(int puntos) => Energia = Math.Min(100, Energia + Math.Max(0, puntos));
    public bool GastarEnergia(int puntos)
    {
        if (Energia < puntos) return false;
        Energia -= puntos;
        return true;
    }
    public void DarEscudo(int puntos) => Escudo = Math.Min(100, Escudo + puntos);
    public void MejorarAtaque(int puntos) => AtaqueExtra += puntos;
    public void DarExperiencia(int puntos) => Experiencia += puntos;
    public override string Describir() => base.Describir() + $" | Escudo {Escudo} | XP {Experiencia}";
}
internal sealed class Robot : Personaje
{
    public Robot() : base("Robot centinela", 45, 35) { }
    public override string Describir() => base.Describir() + " | Defensa automática";
}
internal sealed class Enemigo : Personaje
{
    public Enemigo() : base("Entidad hostil", 55, 45) { }
    public override string Describir() => base.Describir() + " | Hostil";
}
internal enum TipoObjeto { Celula, Escudo, ModuloAtaque, Analizador, Nucleo, ModuloDesconocido }
internal sealed class Inventario
{
    private readonly Dictionary<TipoObjeto,int> objetos = new();
    public int Cantidad(TipoObjeto tipo) => objetos.GetValueOrDefault(tipo);
    public void Agregar(TipoObjeto tipo, int cantidad = 1)
    { if (cantidad > 0) objetos[tipo] = Cantidad(tipo) + cantidad; }
    public bool Quitar(TipoObjeto tipo)
    {
        if (Cantidad(tipo) == 0) return false;
        objetos[tipo]--;
        if (objetos[tipo] == 0) objetos.Remove(tipo);
        return true;
    }
    public string Ver() => objetos.Count == 0 ? "Inventario vacío." :
        string.Join(", ", objetos.Select(x => $"{x.Key}: {x.Value}"));
}
internal sealed class Mision
{
    public bool ZonaAlcanzada { get; set; }
    public bool NucleoEncontrado { get; set; }
    public bool EncuentroSuperado { get; set; }
    public bool Completada { get; private set; }
    public string Estado() => $"La anomalía perdida | Zona: {ZonaAlcanzada} | Núcleo: {NucleoEncontrado} | Encuentro: {EncuentroSuperado} | Completada: {Completada}";
    public bool Finalizar(Cadete cadete, int zona)
    {
        if (Completada || zona != 0 || !ZonaAlcanzada || !NucleoEncontrado || !EncuentroSuperado || !cadete.Vivo) return false;
        Completada = true;
        cadete.DarExperiencia(100);
        cadete.Mochila.Agregar(TipoObjeto.ModuloDesconocido);
        return true;
    }
}
internal sealed class MundoNexus
{
    private readonly Random azar = new();
    private readonly string[] zonas = { "Base de extracción", "Bosque de ecos", "Ruinas orbitales", "Zona de anomalía" };
    private readonly bool[] visitadas = new bool[4];
    private bool robotSuperado;
    private bool enemigoSuperado;
    public Cadete Jugador { get; }
    public Mision Mision { get; } = new();
    public int Zona { get; private set; }
    public Personaje? Encuentro { get; private set; }
    public bool Terminado { get; private set; }
    public MundoNexus(string nombre)
    {
        Jugador = new Cadete(nombre);
        Jugador.Mochila.Agregar(TipoObjeto.Celula, 2);
        Jugador.Mochila.Agregar(TipoObjeto.Analizador);
        visitadas[0] = true;
    }
    public string Mover(int destino)
    {
        if (Encuentro != null) return "Primero resuelve el encuentro (atacar, escapar o analizar).";
        if (destino < 0 || destino >= zonas.Length) return "Movimiento inválido: fuera del mapa.";
        if (Math.Abs(destino - Zona) > 1) return "Solo puedes avanzar a una zona vecina.";
        if (destino == Zona) return "Ya estás en esa zona.";
        if (!Jugador.GastarEnergia(5)) return "No hay energía suficiente para moverse.";
        Zona = destino;
        string mensaje = $"Entraste en {zonas[Zona]}. Energía -5.";
        if (Zona == 3) Mision.ZonaAlcanzada = true;
        if (!visitadas[Zona])
        {
            visitadas[Zona] = true;
            if (Zona == 1)
            { Jugador.Mochila.Agregar(TipoObjeto.Escudo); mensaje += " OBJETO ENCONTRADO: escudo."; }
            else if (Zona == 2)
            { Jugador.Mochila.Agregar(TipoObjeto.ModuloAtaque); mensaje += " OBJETO ENCONTRADO: módulo de ataque."; }
            else if (Zona == 3)
            { Jugador.Mochila.Agregar(TipoObjeto.Nucleo); Mision.NucleoEncontrado = true; mensaje += " ANOMALÍA DETECTADA: núcleo energético recuperado."; }
        }
        if (Zona == 2 && !robotSuperado) { Encuentro = new Robot(); mensaje += " ENTIDAD DETECTADA: robot."; }
        if (Zona == 3 && !enemigoSuperado) { Encuentro = new Enemigo(); mensaje += " ENTIDAD DETECTADA: enemigo."; }
        
        if (Zona == 0 && Mision.Finalizar(Jugador, Zona)) mensaje += " MISIÓN COMPLETADA: +100 XP y módulo desconocido.";
        return mensaje;
    }
    public string Explorar()
    {
        if (Encuentro != null) return "Hay una entidad presente; resuelve el encuentro.";
        if (!Jugador.GastarEnergia(6)) return "Energía insuficiente para explorar.";
        int evento = azar.Next(4);
        if (evento == 0) { Jugador.Mochila.Agregar(TipoObjeto.Celula); return "OBJETO ENCONTRADO: célula de energía."; }
        if (evento == 1) { Jugador.RecibirDanio(8); RevisarVida(); return "SISTEMA DAÑADO: -8 vida."; }
        if (evento == 2) return "IRIS: señales antiguas detectadas. Información recuperada.";
        return "No encontraste nada en esta exploración.";
    }
    public string AccionEncuentro(int accion)
    {
        if (Encuentro == null) return "No hay entidad para interactuar.";
        if (accion == 1)
        {
            if (!Jugador.GastarEnergia(8)) return "Energía insuficiente para atacar.";
            Encuentro.RecibirDanio(20 + Jugador.AtaqueExtra);
            if (!Encuentro.Vivo)
            {
                string nombre = Encuentro.Nombre;
                if (Encuentro is Robot) robotSuperado = true;
                if (Encuentro is Enemigo) { enemigoSuperado = true; Mision.EncuentroSuperado = true; }
                Encuentro = null;
                return $"Has vencido a {nombre}. Objetivo de encuentro superado.";
            }
            Jugador.RecibirDanio(Encuentro is Robot ? 8 : 14);
            RevisarVida();
            return "Ataque realizado. La entidad respondió. " + Jugador.Describir();
        }
        if (accion == 2)
        {
            Jugador.RecibirDanio(5);
            Encuentro = null;
            RevisarVida();
            return "Escapaste, pero recibiste 5 de daño. El objetivo de encuentro sigue pendiente.";
        }
        if (accion == 3)
        {
            if (Jugador.Mochila.Cantidad(TipoObjeto.Analizador) == 0) return "No posees un analizador.";
            return Encuentro.Describir();
        }
        return "Opción inválida. Usa atacar, escapar o analizar.";
    }
    public string UsarObjeto(TipoObjeto tipo)
    {
        if (tipo != TipoObjeto.Celula && tipo != TipoObjeto.Escudo && tipo != TipoObjeto.ModuloAtaque && tipo != TipoObjeto.Analizador)
            return "Ese objeto no es consumible.";
        if (Jugador.Mochila.Cantidad(tipo) == 0) return "No posees ese objeto.";
        if (tipo == TipoObjeto.Analizador)
            return Encuentro == null ? "Analizador: no hay entidad presente." : "Analizador: " + Encuentro.Describir();
        if (tipo == TipoObjeto.Celula && Jugador.Energia == 100) return "Energía al máximo; objeto conservado.";
        Jugador.Mochila.Quitar(tipo);
        if (tipo == TipoObjeto.Celula) { Jugador.CurarEnergia(25); return "Célula utilizada: +25 energía (máximo 100)."; }
        if (tipo == TipoObjeto.Escudo) { Jugador.DarEscudo(25); return "Escudo activado: +25 protección."; }
        Jugador.MejorarAtaque(10);
        return "Módulo de ataque equipado: +10 daño.";
    }
    public string EliminarObjeto(TipoObjeto tipo)
    {
        if (tipo == TipoObjeto.Nucleo) return "No puedes eliminar el núcleo de la misión.";
        return Jugador.Mochila.Quitar(tipo) ? "Objeto eliminado." : "No posees ese objeto.";
    }
    private void RevisarVida() { if (!Jugador.Vivo) Terminado = true; }
    public string Mapa() => string.Join(" | ", zonas.Select((z, i) => $"{i}: {z}{(i == Zona ? " [ACTUAL]" : "")}"));
}
internal static class NexusV2
{
    private static int LeerNumero(string mensaje, int minimo, int maximo)
    {
        while (true)
        {
            Console.Write(mensaje);
            if (int.TryParse(Console.ReadLine(), out int n) && n >= minimo && n <= maximo) return n;
            Console.WriteLine($"Introduce un número entre {minimo} y {maximo}.");
        }
    }
    public static void Ejecutar()
    {
        Console.WriteLine("NEXUS V2.0 | MISIÓN: LA ANOMALÍA PERDIDA | AÑO 2297");
        string nombre = Entrada.Texto("Nombre del cadete: ", 60);
        var mundo = new MundoNexus(nombre);
        while (!mundo.Terminado && !mundo.Mision.Completada)
        {
            Console.WriteLine("\n" + mundo.Jugador.Describir());
            Console.WriteLine(mundo.Mapa());
            Console.WriteLine("1 Mover | 2 Explorar | 3 Inventario | 4 Usar objeto | 5 Eliminar objeto");
            Console.WriteLine("6 Atacar | 7 Escapar | 8 Analizar | 9 Misión | 0 Salir");
            int op = LeerNumero("Opción: ", 0, 9);
            if (op == 0) break;
            string mensaje = op switch
            {
                1 => mundo.Mover(LeerNumero("Zona (0 a 3): ", 0, 3)),
                2 => mundo.Explorar(),
                3 => mundo.Jugador.Mochila.Ver(),
                4 => mundo.UsarObjeto((TipoObjeto)LeerNumero("Objeto: 0 Célula, 1 Escudo, 2 Ataque, 3 Analizador: ", 0, 3)),
                5 => mundo.EliminarObjeto((TipoObjeto)LeerNumero("Eliminar: 0 Célula, 1 Escudo, 2 Ataque, 3 Analizador: ", 0, 3)),
                6 => mundo.AccionEncuentro(1),
                7 => mundo.AccionEncuentro(2),
                8 => mundo.AccionEncuentro(3),
                9 => mundo.Mision.Estado(),
                _ => "Opción inválida."
            };
            Console.WriteLine(mensaje);
        }
        Console.WriteLine(mundo.Mision.Completada ? "¡MISIÓN COMPLETADA! Regresaste con vida." : mundo.Terminado ? "MISIÓN FALLIDA: el cadete cayó." : "Sesión cerrada.");
    }
}
