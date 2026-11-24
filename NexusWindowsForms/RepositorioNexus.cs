
using Microsoft.Data.SqlClient;
using NexusTraining;

namespace NexusWindowsForms;

public sealed class RepositorioNexus
{
    public int CrearNuevaPartida(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del explorador es obligatorio.");

        using var conexion = ConexionBD.ObtenerConexion();
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            int cadeteId;

            using (var comando = new SqlCommand(
                @"INSERT INTO Cadetes (Nombre)
                  OUTPUT INSERTED.CadeteId
                  VALUES (@Nombre);",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Nombre", nombre.Trim());
                cadeteId = (int)comando.ExecuteScalar()!;
            }

            int partidaId;

            using (var comando = new SqlCommand(
                @"INSERT INTO Partidas (CadeteId)
                  OUTPUT INSERTED.PartidaId
                  VALUES (@CadeteId);",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@CadeteId", cadeteId);
                partidaId = (int)comando.ExecuteScalar()!;
            }

            using (var comando = new SqlCommand(
                @"INSERT INTO ProgresoMision (PartidaId)
                  VALUES (@PartidaId);",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@PartidaId", partidaId);
                comando.ExecuteNonQuery();
            }

            using (var comando = new SqlCommand(
                @"INSERT INTO InventarioPartida (PartidaId, ObjetoId, Cantidad)
                  VALUES (@PartidaId, 0, 2),
                         (@PartidaId, 3, 1);",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@PartidaId", partidaId);
                comando.ExecuteNonQuery();
            }

            using (var comando = new SqlCommand(
                @"INSERT INTO ZonasVisitadas (PartidaId, ZonaId, Visitada)
                  VALUES (@PartidaId, 0, 1),
                         (@PartidaId, 1, 0),
                         (@PartidaId, 2, 0),
                         (@PartidaId, 3, 0);",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@PartidaId", partidaId);
                comando.ExecuteNonQuery();
            }

            transaccion.Commit();

            return partidaId;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public void GuardarPartida(int partidaId, MundoNexus mundo)
    {
        using var conexion = ConexionBD.ObtenerConexion();
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            int? tipoEncuentro = mundo.Encuentro is Robot ? 1 :
                                 mundo.Encuentro is Enemigo ? 2 : null;

            int? vidaEncuentro = mundo.Encuentro?.Vida;
            int? energiaEncuentro = mundo.Encuentro?.Energia;

            using (var comando = new SqlCommand(@"
            UPDATE Partidas SET
                Vida = @Vida,
                Energia = @Energia,
                Experiencia = @Experiencia,
                Escudo = @Escudo,
                AtaqueExtra = @AtaqueExtra,
                ZonaActual = @Zona,
                Terminado = @Terminado,
                RobotSuperado = @Robot,
                EnemigoSuperado = @Enemigo,
                EncuentroTipo = @Tipo,
                EncuentroVida = @VidaEncuentro,
                EncuentroEnergia = @EnergiaEncuentro,
                FechaGuardado = SYSDATETIME()
            WHERE PartidaId = @PartidaId;",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@PartidaId", partidaId);
                comando.Parameters.AddWithValue("@Vida", mundo.Jugador.Vida);
                comando.Parameters.AddWithValue("@Energia", mundo.Jugador.Energia);
                comando.Parameters.AddWithValue("@Experiencia", mundo.Jugador.Experiencia);
                comando.Parameters.AddWithValue("@Escudo", mundo.Jugador.Escudo);
                comando.Parameters.AddWithValue("@AtaqueExtra", mundo.Jugador.AtaqueExtra);
                comando.Parameters.AddWithValue("@Zona", mundo.Zona);
                comando.Parameters.AddWithValue("@Terminado", mundo.Terminado);
                comando.Parameters.AddWithValue("@Robot", mundo.RobotSuperado);
                comando.Parameters.AddWithValue("@Enemigo", mundo.EnemigoSuperado);
                comando.Parameters.AddWithValue("@Tipo", (object?)tipoEncuentro ?? DBNull.Value);
                comando.Parameters.AddWithValue("@VidaEncuentro", (object?)vidaEncuentro ?? DBNull.Value);
                comando.Parameters.AddWithValue("@EnergiaEncuentro", (object?)energiaEncuentro ?? DBNull.Value);

                if (comando.ExecuteNonQuery() == 0)
                    throw new Exception("No se encontró la partida indicada.");
            }

            using (var comando = new SqlCommand(@"
            UPDATE ProgresoMision SET
                ZonaAlcanzada = @Zona,
                NucleoEncontrado = @Nucleo,
                EncuentroSuperado = @Encuentro,
                Completada = @Completada
            WHERE PartidaId = @PartidaId;",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@PartidaId", partidaId);
                comando.Parameters.AddWithValue("@Zona", mundo.Mision.ZonaAlcanzada);
                comando.Parameters.AddWithValue("@Nucleo", mundo.Mision.NucleoEncontrado);
                comando.Parameters.AddWithValue("@Encuentro", mundo.Mision.EncuentroSuperado);
                comando.Parameters.AddWithValue("@Completada", mundo.Mision.Completada);
                comando.ExecuteNonQuery();
            }

            using (var comando = new SqlCommand(
                "DELETE FROM InventarioPartida WHERE PartidaId = @Id;",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Id", partidaId);
                comando.ExecuteNonQuery();
            }

            foreach (TipoObjeto tipo in Enum.GetValues<TipoObjeto>())
            {
                int cantidad = mundo.Jugador.Mochila.Cantidad(tipo);

                if (cantidad > 0)
                {
                    using var comando = new SqlCommand(@"
                    INSERT INTO InventarioPartida
                    (PartidaId, ObjetoId, Cantidad)
                    VALUES (@PartidaId, @ObjetoId, @Cantidad);",
                        conexion, transaccion);

                    comando.Parameters.AddWithValue("@PartidaId", partidaId);
                    comando.Parameters.AddWithValue("@ObjetoId", (byte)tipo);
                    comando.Parameters.AddWithValue("@Cantidad", cantidad);
                    comando.ExecuteNonQuery();
                }
            }

            using (var comando = new SqlCommand(
                "DELETE FROM ZonasVisitadas WHERE PartidaId = @Id;",
                conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Id", partidaId);
                comando.ExecuteNonQuery();
            }

            bool[] zonas = mundo.ObtenerZonasVisitadas();

            for (int i = 0; i < zonas.Length; i++)
            {
                using var comando = new SqlCommand(@"
                INSERT INTO ZonasVisitadas
                (PartidaId, ZonaId, Visitada)
                VALUES (@PartidaId, @ZonaId, @Visitada);",
                    conexion, transaccion);

                comando.Parameters.AddWithValue("@PartidaId", partidaId);
                comando.Parameters.AddWithValue("@ZonaId", i);
                comando.Parameters.AddWithValue("@Visitada", zonas[i]);
                comando.ExecuteNonQuery();
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public MundoNexus CargarPartida(int partidaId)
    {
        using var conexion = ConexionBD.ObtenerConexion();
        conexion.Open();

        string consulta = @"
        SELECT C.Nombre, P.Vida, P.Energia, P.Experiencia,
               P.Escudo, P.AtaqueExtra, P.ZonaActual,
               P.Terminado, P.RobotSuperado, P.EnemigoSuperado,
               P.EncuentroTipo, P.EncuentroVida, P.EncuentroEnergia,
               M.ZonaAlcanzada, M.NucleoEncontrado,
               M.EncuentroSuperado, M.Completada
        FROM Partidas P
        INNER JOIN Cadetes C ON P.CadeteId = C.CadeteId
        INNER JOIN ProgresoMision M ON P.PartidaId = M.PartidaId
        WHERE P.PartidaId = @Id;";

        string nombre;
        int vida, energia, experiencia, escudo, ataque, zona;
        bool terminado, robot, enemigo;
        int? tipoEncuentro, vidaEncuentro, energiaEncuentro;
        bool zonaAlcanzada, nucleo, encuentroSuperado, completada;

        using (var comando = new SqlCommand(consulta, conexion))
        {
            comando.Parameters.AddWithValue("@Id", partidaId);

            using var lector = comando.ExecuteReader();

            if (!lector.Read())
                throw new Exception("No se encontró la partida.");

            nombre = lector["Nombre"].ToString()!;
            vida = Convert.ToInt32(lector["Vida"]);
            energia = Convert.ToInt32(lector["Energia"]);
            experiencia = Convert.ToInt32(lector["Experiencia"]);
            escudo = Convert.ToInt32(lector["Escudo"]);
            ataque = Convert.ToInt32(lector["AtaqueExtra"]);
            zona = Convert.ToInt32(lector["ZonaActual"]);

            terminado = Convert.ToBoolean(lector["Terminado"]);
            robot = Convert.ToBoolean(lector["RobotSuperado"]);
            enemigo = Convert.ToBoolean(lector["EnemigoSuperado"]);

            tipoEncuentro = lector["EncuentroTipo"] == DBNull.Value
                ? null : Convert.ToInt32(lector["EncuentroTipo"]);

            vidaEncuentro = lector["EncuentroVida"] == DBNull.Value
                ? null : Convert.ToInt32(lector["EncuentroVida"]);

            energiaEncuentro = lector["EncuentroEnergia"] == DBNull.Value
                ? null : Convert.ToInt32(lector["EncuentroEnergia"]);

            zonaAlcanzada = Convert.ToBoolean(lector["ZonaAlcanzada"]);
            nucleo = Convert.ToBoolean(lector["NucleoEncontrado"]);
            encuentroSuperado = Convert.ToBoolean(lector["EncuentroSuperado"]);
            completada = Convert.ToBoolean(lector["Completada"]);
        }

        var inventario = new Dictionary<TipoObjeto, int>();

        using (var comando = new SqlCommand(
            "SELECT ObjetoId, Cantidad FROM InventarioPartida WHERE PartidaId = @Id;",
            conexion))
        {
            comando.Parameters.AddWithValue("@Id", partidaId);

            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                var tipo = (TipoObjeto)Convert.ToByte(lector["ObjetoId"]);
                inventario[tipo] = Convert.ToInt32(lector["Cantidad"]);
            }
        }

        bool[] zonasVisitadas = new bool[4];

        using (var comando = new SqlCommand(
            "SELECT ZonaId, Visitada FROM ZonasVisitadas WHERE PartidaId = @Id;",
            conexion))
        {
            comando.Parameters.AddWithValue("@Id", partidaId);

            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                int idZona = Convert.ToInt32(lector["ZonaId"]);

                if (idZona >= 0 && idZona < 4)
                    zonasVisitadas[idZona] = Convert.ToBoolean(lector["Visitada"]);
            }
        }

        var mundo = new MundoNexus(nombre);

        mundo.RestaurarPartida(
            vida, energia, experiencia, escudo, ataque, zona,
            terminado, robot, enemigo,
            tipoEncuentro, vidaEncuentro, energiaEncuentro,
            zonaAlcanzada, nucleo, encuentroSuperado, completada,
            inventario, zonasVisitadas);

        return mundo;
    }

    public void EliminarPartida(int partidaId)
    {
        using var conexion = ConexionBD.ObtenerConexion();
        conexion.Open();

        using var comando = new SqlCommand(
            "DELETE FROM Partidas WHERE PartidaId = @Id;",
            conexion);

        comando.Parameters.AddWithValue("@Id", partidaId);

        if (comando.ExecuteNonQuery() == 0)
            throw new Exception("No se encontró la partida para eliminar.");
    }

    public List<string> ObtenerPartidas()
    {
        var partidas = new List<string>();

        using var conexion = ConexionBD.ObtenerConexion();
        conexion.Open();

        string consulta = @"
        SELECT P.PartidaId, C.Nombre, P.Vida,
               P.Energia, P.Experiencia, P.ZonaActual
        FROM Partidas P
        INNER JOIN Cadetes C ON P.CadeteId = C.CadeteId
        WHERE P.Terminado = 0
        ORDER BY P.FechaGuardado DESC;";

        using var comando = new SqlCommand(consulta, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            string partida =
                $"ID: {lector["PartidaId"]} | " +
                $"Explorador: {lector["Nombre"]} | " +
                $"Vida: {lector["Vida"]} | " +
                $"Energía: {lector["Energia"]} | " +
                $"XP: {lector["Experiencia"]} | " +
                $"Zona: {lector["ZonaActual"]}";

            partidas.Add(partida);
        }

        return partidas;
    }

}
