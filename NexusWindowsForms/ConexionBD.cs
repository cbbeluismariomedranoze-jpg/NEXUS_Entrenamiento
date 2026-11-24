
using Microsoft.Data.SqlClient;

namespace NexusWindowsForms;

public static class ConexionBD
{
    private static readonly string cadenaConexion =
        @"Server=.\SQLEXPRESS;Database=NEXUS_DB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection ObtenerConexion()
    {
        return new SqlConnection(cadenaConexion);
    }

    public static bool ProbarConexion()
    {
        try
        {
            using var conexion = ObtenerConexion();
            conexion.Open();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Error al conectar con SQL Server:\n" + ex.Message,
                "Error de conexión",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }
    }
}
