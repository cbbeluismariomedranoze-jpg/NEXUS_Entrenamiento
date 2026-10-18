namespace NexusTraining;

// Sin paquetes externos: dotnet run --project NexusTraining -- --pruebas
internal static class Pruebas
{
    internal static int Ejecutar()
    {
        int aprobadas = 0, fallidas = 0;
        void Verificar(string nombre, Action prueba)
        {
            try { prueba(); Console.WriteLine("[OK] " + nombre); aprobadas++; }
            catch (Exception ex) { Console.WriteLine("[FALLO] " + nombre + ": " + ex.Message); fallidas++; }
        }
        void Exigir(bool condicion, string mensaje)
        {
            if (!condicion) throw new InvalidOperationException(mensaje);
        }
        Simulacion Nueva(int e = 80, int s = 80, int edad = 20) => new("Luis Mario", edad, "LUMEN-07", e, s);

        Verificar("1 Inmersión autorizada y límites de admisión", () => {
            Exigir(Nueva().Conectada && Nueva(40, 60, 16).Autorizada, "Debe autorizar.");
        });
        Verificar("2 Inmersión rechazada por cada requisito", () => {
            foreach (var x in new[] { Nueva(80,80,15), Nueva(39,80), Nueva(80,59) })
                Exigir(!x.Autorizada && !x.Conectada, "Debe rechazar.");
        });
        Verificar("3 Operaciones de exploración, análisis, consulta y recuperación", () => {
            var x = Nueva(); x.Operar(1);
            Exigir(x.Energia == 68 && x.Estabilidad == 72 && x.Ecos == 1, "Exploración incorrecta.");
            x.Operar(2);
            Exigir(x.Energia == 50 && x.Estabilidad == 60 && x.FragmentosIris == 1, "Análisis incorrecto.");
            string previo = x.Estado(); x.Operar(3);
            Exigir(previo == x.Estado(), "Consulta alteró el estado.");
            x.Operar(4);
            Exigir(x.Energia == 72 && x.Estabilidad == 56 && x.Celdas == 2, "Recarga incorrecta.");
            x.Operar(6);
            Exigir(x.Energia == 62 && x.Estabilidad == 74, "Recalibración incorrecta.");
        });
        Verificar("4 Entrada ABC, -500 y 99 con recuperación", () => {
            TextReader originalIn = Console.In; TextWriter originalOut = Console.Out;
            try {
                Console.SetIn(new StringReader("ABC\n-500\n80\n99\n5\n"));
                Console.SetOut(new StringWriter());
                Exigir(Entrada.Entero("",0,100) == 80, "Falló validación de energía.");
                Exigir(Entrada.Entero("",1,6) == 5, "Falló validación del menú.");
            } finally { Console.SetIn(originalIn); Console.SetOut(originalOut); }
            var x = Nueva(); string estado = x.Estado(); x.Operar(99);
            Exigir(estado == x.Estado(), "Opción inválida modificó estado.");
        });
        Verificar("5 IRIS y desconexión por condición crítica", () => {
            var x = Nueva(100, 60); int eventos = 0;
            x.AnomaliaIris += (_, _) => eventos++;
            x.Operar(2); x.Operar(2); x.Operar(2);
            Exigir(x.Estabilidad == 24 && x.Conectada && eventos == 1, "IRIS no se activó.");
            x.Operar(2); x.Operar(2);
            Exigir(!x.Conectada && x.Estabilidad == 0 && eventos == 1, "No ocurrió corte crítico.");
        });
        Verificar("6 Desconexión voluntaria y bloqueo posterior", () => {
            var x = Nueva(); x.Operar(5); string estado = x.Estado(); x.Operar(1);
            Exigir(!x.Conectada && estado == x.Estado(), "La sesión siguió activa.");
        });
        Verificar("7 Límite 100 y reservas agotadas", () => {
            var x = Nueva(95, 100); x.Operar(4);
            Exigir(x.Energia == 100 && x.Celdas == 2, "Superó el límite.");
            x.Operar(4); Exigir(x.Celdas == 2, "Gastó celda innecesaria.");
            x.Operar(2); x.Operar(4); x.Operar(2); x.Operar(4);
            string previo = x.Estado(); x.Operar(4);
            Exigir(x.Celdas == 0 && previo == x.Estado(), "Reservas inválidas.");
        });
        Verificar("8 Energía insuficiente sin mutación", () => {
            var x = Nueva(40, 100); x.Operar(2); x.Operar(2);
            string previo = x.Estado(); x.Operar(1); x.Operar(2); x.Operar(6);
            Exigir(previo == x.Estado(), "Se ejecutó una acción sin energía.");
        });
        Verificar("9 Regla élite e historial de exploración", () => {
            var x = Nueva(40, 60); x.Operar(1); x.Operar(3); x.Operar(1);
            Exigir(x.Energia == 9 && x.Estabilidad == 38 && x.Alertas == 1 && x.Conectada,
                "Falló regla combinada o consulta reinició la secuencia.");
        });
        Verificar("10 Fin de entrada genera excepción controlable", () => {
            TextReader original = Console.In; bool capturada = false;
            try {
                Console.SetIn(new StringReader(""));
                try { Entrada.Entero("", 0, 100); } catch (EndOfStreamException) { capturada = true; }
            } finally { Console.SetIn(original); }
            Exigir(capturada, "No detectó cierre de entrada.");
        });
        Verificar("11 Sesión completa con entradas redirigidas y menú repetido", () => {
            TextReader originalIn = Console.In; TextWriter originalOut = Console.Out;
            var salida = new StringWriter();
            try {
                Console.SetIn(new StringReader("Luis\n20\nLUMEN-07\n80\n80\n1\n3\n5\n"));
                Console.SetOut(salida); Program.EjecutarSesion();
            } finally { Console.SetIn(originalIn); Console.SetOut(originalOut); }
            string texto = salida.ToString();
            Exigir(texto.Contains("INMERSIÓN AUTORIZADA") && texto.Contains("ENLACE CERRADO") &&
                texto.Split("Operación (1 a 6)").Length == 4, "El ciclo no se repitió 3 veces.");
        });
        Verificar("12 Estado inválido rechazado por el modelo", () => {
            bool capturada = false;
            try { Nueva(-500, 80); } catch (ArgumentOutOfRangeException) { capturada = true; }
            Exigir(capturada, "El modelo aceptó energía negativa.");
        });
        Console.WriteLine($"\nRESULTADO: {aprobadas} aprobadas; {fallidas} fallidas.");
        return fallidas == 0 ? 0 : 1;
    }
}
