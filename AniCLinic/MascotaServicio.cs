using System;
using System.Data;
using System.Data.SqlClient;

namespace AniCLinic
{
    /// <summary>
    /// Servicio reusable para listar/buscar mascotas enlazadas a la cédula del propietario.
    /// Puedes usarlo desde cualquier formulario.
    /// </summary>
    internal class MascotaServicio
    {
        private readonly csConexionBD _db = new csConexionBD();

        /// <summary>
        /// Lista todas las mascotas con datos del propietario.
        /// </summary>
        public DataTable ListarTodo()
        {
            const string sql = @"
SELECT 
    m.IdMascota,
    m.Nombre,
    m.Especie,
    m.Raza,
    m.Sexo,
    m.Edad,
    m.PesoKg,
    m.Discapacidad,
    p.Cedula,
    (p.Nombres + ' ' + p.Apellidos) AS Propietario
FROM Mascota AS m
LEFT JOIN Propietario AS p ON p.IdPropietario = m.IdPropietario   -- <- ajusta si tu FK tiene otro nombre
ORDER BY m.IdMascota DESC;";

            using (var da = new SqlDataAdapter(sql, _db.obtenerConexion()))
            {
                var dt = new DataTable();
                _db.abrirConexion();
                da.Fill(dt);
                _db.cerrarConexion();
                return dt;
            }
        }

        /// <summary>
        /// Busca por cédula del propietario, nombre del propietario o nombre de la mascota.
        /// </summary>
        public DataTable Buscar(string filtro)
        {
            // Compatibilidad C# 7.3 (sin operador ??=)
            if (filtro == null) filtro = string.Empty;
            filtro = filtro.Trim();

            const string sql = @"
SELECT 
    m.IdMascota,
    m.Nombre,
    m.Especie,
    m.Raza,
    m.Sexo,
    m.Edad,
    m.PesoKg,
    m.Discapacidad,
    p.Cedula,
    (p.Nombres + ' ' + p.Apellidos) AS Propietario
FROM Mascota AS m
LEFT JOIN Propietario AS p ON p.IdPropietario = m.IdPropietario
WHERE (@q = '')
   OR p.Cedula LIKE '%' + @q + '%'
   OR (p.Nombres + ' ' + p.Apellidos) LIKE '%' + @q + '%'
   OR m.Nombre LIKE '%' + @q + '%'
ORDER BY m.IdMascota DESC;";

            using (var cmd = new SqlCommand(sql, _db.obtenerConexion()))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@q", filtro);
                var dt = new DataTable();
                _db.abrirConexion();
                da.Fill(dt);
                _db.cerrarConexion();
                return dt;
            }
        }

    }
}
