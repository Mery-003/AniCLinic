using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace AniCLinic
{
    internal static class CedulaUtils
    {
        private static readonly Regex RxCedula = new Regex(@"^\d{10}$");
        private static readonly csCRUD crud = new csCRUD();

        public static bool CedulaValida(string cedula)
        {
            return !string.IsNullOrWhiteSpace(cedula) && RxCedula.IsMatch(cedula);
        }

        public static string VeterinarioDeSesion()
        {
            if (!string.IsNullOrWhiteSpace(SesionActual.NombreEmpleado))
                return SesionActual.NombreEmpleado;

            if (SesionActual.IdEmpleado <= 0)
                return "";

            string sql = @"
            SELECT TOP 1 (p.Nombre + ' ' + p.Apellido)
            FROM dbo.Empleado e
            JOIN dbo.Persona  p ON p.IdPersona = e.IdPersona
            WHERE e.IdEmpleado = @id;";

            DataTable dt = crud.cargarBDData(sql, new SqlParameter("@id", SesionActual.IdEmpleado));
            return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : "";
        }

        public static DataTable MascotasPorCedula(string cedula)
        {
            string sql = @"
            SELECT  m.IdMascota,
                    m.Nombre     AS Mascota,
                    m.Especie,
                    m.Raza,
                    (p.Nombre + ' ' + p.Apellido) AS Propietario
            FROM dbo.Persona  p
            JOIN dbo.Mascota  m ON m.IdPersona = p.IdPersona
            WHERE p.Cedula = @cedula
            ORDER BY m.Nombre;";

            return crud.cargarBDData(sql, new SqlParameter("@cedula", cedula));
        }

        public static DataTable CitasListado()
        {
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

            return crud.cargarBDData(sql);
        }

        public static DataTable HorasOcupadas(DateTime dia)
        {
            string sql = @"
            SELECT DISTINCT CONVERT(varchar(5), g.FechaHora, 108) AS Hora
            FROM dbo.GestionCita g
            WHERE CONVERT(date, g.FechaHora) = @d;";

            return crud.cargarBDData(sql, new SqlParameter("@d", dia.Date));
        }

        public static bool ExisteChoqueHorario(DateTime fecha, string horaHHmm, int? excluirIdCita = null)
        {
            string sql = @"
            SELECT COUNT(1)
            FROM dbo.GestionCita
            WHERE CONVERT(date, FechaHora) = @f
              AND CONVERT(varchar(5), FechaHora, 108) = @h";

            DataTable dt;

            if (excluirIdCita.HasValue)
            {
                sql += " AND IdCita <> @id";
                dt = crud.cargarBDData(sql,
                    new SqlParameter("@f", fecha.Date),
                    new SqlParameter("@h", horaHHmm),
                    new SqlParameter("@id", excluirIdCita.Value));
            }
            else
            {
                dt = crud.cargarBDData(sql,
                    new SqlParameter("@f", fecha.Date),
                    new SqlParameter("@h", horaHHmm));
            }

            return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0][0]) > 0;
        }
    }
}
