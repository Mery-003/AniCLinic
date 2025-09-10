using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace AniCLinic
{
    internal static class CedulaUtils
    {
        private static readonly Regex RxCedula = new Regex(@"^\d{10}$");

        public static bool CedulaValida(string cedula)
            => !string.IsNullOrWhiteSpace(cedula) && RxCedula.IsMatch(cedula);

        // Nombre del veterinario (de la sesión; si no hay nombre, lo obtiene por Id)
        public static string VeterinarioDeSesion()
        {
            if (!string.IsNullOrWhiteSpace(SesionActual.NombreEmpleado))
                return SesionActual.NombreEmpleado;

            if (SesionActual.IdEmpleado <= 0) return "";

            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 (p.Nombre + ' ' + p.Apellido)
                    FROM dbo.Empleado e
                    JOIN dbo.Persona  p ON p.IdPersona = e.IdPersona
                    WHERE e.IdEmpleado = @id;", db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@id", SesionActual.IdEmpleado);
                    var r = cmd.ExecuteScalar();
                    return r == null ? "" : r.ToString();
                }
            }
            finally { db.cerrarConexion(); }
        }

        // Mascotas por cédula del propietario
        public static DataTable MascotasPorCedula(string cedula)
        {
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var da = new SqlDataAdapter(@"
                    SELECT  m.IdMascota,
                            m.Nombre     AS Mascota,
                            m.Especie,
                            m.Raza,
                            (p.Nombre + ' ' + p.Apellido) AS Propietario
                    FROM dbo.Persona  p
                    JOIN dbo.Mascota  m ON m.IdPersona = p.IdPersona
                    WHERE p.Cedula = @cedula
                    ORDER BY m.Nombre;", db.obtenerConexion()))
                {
                    da.SelectCommand.Parameters.AddWithValue("@cedula", cedula);
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            finally { db.cerrarConexion(); }
        }

        public static string NombrePropietario(string cedula)
        {
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 (Nombre + ' ' + Apellido)
                    FROM dbo.Persona
                    WHERE Cedula=@c;", db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@c", cedula);
                    var r = cmd.ExecuteScalar();
                    return r == null ? string.Empty : r.ToString();
                }
            }
            finally { db.cerrarConexion(); }
        }

        // Listado del grid
        public static DataTable CitasListado()
        {
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                string sql = @"
                SELECT  g.IdCita,
                        m.Nombre                                  AS Mascota,
                        m.Especie, m.Raza,
                        CONVERT(date, g.FechaHora)                AS Fecha,
                        CONVERT(varchar(5), g.FechaHora, 108)     AS Hora,
                        g.Motivo,
                        (p.Nombre + ' ' + p.Apellido)             AS Propietario
                FROM dbo.GestionCita g
                JOIN dbo.Mascota     m ON m.IdMascota = g.IdMascota
                JOIN dbo.Persona     p ON p.IdPersona = m.IdPersona
                ORDER BY g.FechaHora DESC;";

                using (var da = new SqlDataAdapter(sql, db.obtenerConexion()))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            finally { db.cerrarConexion(); }
        }

        // Horas ocupadas para el día
        public static DataTable HorasOcupadas(DateTime dia)
        {
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var da = new SqlDataAdapter(@"
                    SELECT DISTINCT CONVERT(varchar(5), g.FechaHora, 108) AS Hora
                    FROM dbo.GestionCita g
                    WHERE CONVERT(date, g.FechaHora) = @d;", db.obtenerConexion()))
                {
                    da.SelectCommand.Parameters.AddWithValue("@d", dia.Date);
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            finally { db.cerrarConexion(); }
        }

        // Choque de horario
        public static bool ExisteChoqueHorario(DateTime fecha, string horaHHmm, int? excluirIdCita = null)
        {
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();

                string sql = @"
                    SELECT COUNT(1)
                    FROM dbo.GestionCita
                    WHERE CONVERT(date, FechaHora) = @f
                      AND CONVERT(varchar(5), FechaHora, 108) = @h";
                if (excluirIdCita.HasValue) sql += " AND IdCita <> @id";

                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@f", fecha.Date);
                    cmd.Parameters.AddWithValue("@h", horaHHmm);
                    if (excluirIdCita.HasValue)
                        cmd.Parameters.AddWithValue("@id", excluirIdCita.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            finally { db.cerrarConexion(); }
        }
    }
}
