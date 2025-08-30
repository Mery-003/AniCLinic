using System.Data;
using System.Data.SqlClient;

namespace AniCLinic
{
    public class csMascotasRepo
    {
        // Ajusta la cadena de conexión si corresponde
        private const string CONN = "Server=localhost;Database=AniClinic;Trusted_Connection=True;";

        /// Lista mascotas con su propietario. Filtra por cédula, nombre de mascota o nombre/apellido.
        public DataTable ListarMascotasConPropietario(string filtro)
        {
            using (var cn = new SqlConnection(CONN))
            using (var cmd = cn.CreateCommand())
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandText = @"
SELECT
    m.IdMascota,
    m.Nombre,
    m.Especie,
    m.Raza,
    m.Sexo,
    m.Edad,
    m.PesoKg,
    m.Discapacidad,
    (p.Nombres + ' ' + p.Apellidos) AS Propietario
FROM Mascota m
INNER JOIN Propietario p ON p.IdPropietario = m.IdPropietario
" + (string.IsNullOrWhiteSpace(filtro) ? "" : @"
WHERE  p.Cedula = @cedula
    OR m.Nombre   LIKE @like
    OR p.Nombres  LIKE @like
    OR p.ApellidosLIKE @like
") + @"
ORDER BY m.IdMascota DESC;";

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    var val = filtro.Trim();
                    cmd.Parameters.AddWithValue("@cedula", val);
                    cmd.Parameters.AddWithValue("@like", $"%{val}%");
                }

                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
