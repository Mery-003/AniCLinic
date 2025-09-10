using System.Data;
using System.Data.SqlClient;

namespace AniCLinic
{
    internal static class SesionRepo
    {
        // Carga IdEmpleado y Nombre del veterinario según el usuario
        public static bool CargarPorUsuario(string usuario)
        {
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var da = new SqlDataAdapter(@"
                    SELECT TOP 1 
                           e.IdEmpleado,
                           (p.Nombre + ' ' + p.Apellido) AS NombreCompleto,
                           p.Imagen
                    FROM Empleados e Inner Join Persona p ON e.IdPersona = p.IdPersona
                    WHERE e.Usuario = @u AND e.Activo = 1", db.obtenerConexion()))
                {
                    da.SelectCommand.Parameters.AddWithValue("@u", usuario);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) return false;

                    SesionActual.IdEmpleado = (int)dt.Rows[0]["IdEmpleado"];
                    SesionActual.NombreEmpleado = dt.Rows[0]["NombreCompleto"].ToString();
                    return true;
                }
            }
            finally { db.cerrarConexion(); }
        }
    }
}
