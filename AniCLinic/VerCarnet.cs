using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class VerCarnet : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        private readonly int _idMascota;

        public VerCarnet(int idMascota)
        {
            InitializeComponent();
            _idMascota = idMascota;
        }

        private void btnCerrar_Click(object sender, EventArgs e) => Close();

        private void VerCarnet_Load(object sender, EventArgs e)
        {
            CargarCarnet();
        }

        private void CargarCarnet()
        {
            // ¡OJO! No referenciamos columnas que pueden no existir (como EdadMeses).
            const string sql = @"
SELECT 
    m.Nombre                         AS Mascota,
    m.Especie, 
    m.Raza, 
    m.Sexo,
    m.PesoKg,
    CASE WHEN COL_LENGTH('dbo.Mascota','EdadAnios') IS NOT NULL THEN m.EdadAnios ELSE NULL END       AS EdadAnios,
    CASE WHEN COL_LENGTH('dbo.Mascota','EdadTexto') IS NOT NULL THEN m.EdadTexto ELSE NULL END       AS EdadTexto,
    CASE WHEN COL_LENGTH('dbo.Mascota','FechaNacimiento') IS NOT NULL THEN m.FechaNacimiento ELSE NULL END AS FechaNacimiento,
    m.Discapacidad,
    m.Imagen
FROM dbo.Mascota m
WHERE m.IdMascota = @id;";

            using (SqlConnection con = _db.AbrirConexion())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", _idMascota);

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        MessageBox.Show("No se encontró la mascota.", "Aviso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                        return;
                    }

                    lblNombres.Text = dr["Mascota"]?.ToString() ?? "-";
                    lblEspecie.Text = dr["Especie"]?.ToString() ?? "-";
                    lblRaza.Text = dr["Raza"]?.ToString() ?? "-";
                    lblSexo.Text = dr["Sexo"]?.ToString() ?? "-";
                    lblDiscapacidad.Text = dr["Discapacidad"]?.ToString() ?? "-";

                    // ----- Edad -----
                    string edadTexto = dr["EdadTexto"] is DBNull ? null : Convert.ToString(dr["EdadTexto"]);
                    int? edadAnios = dr["EdadAnios"] is DBNull ? (int?)null : Convert.ToInt32(dr["EdadAnios"]);
                    DateTime? fechaNac = dr["FechaNacimiento"] is DBNull ? (DateTime?)null : Convert.ToDateTime(dr["FechaNacimiento"]);

                    lblEdad.Text = ConstruirEdad(edadTexto, edadAnios, fechaNac);

                    // Fechas del carnet (si no las guardas en DB)
                    lblFechaEmision.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    lblFechaVencimiento.Text = DateTime.Now.AddYears(1).ToString("dd/MM/yyyy");

                    // Foto
                    if (!(dr["Imagen"] is DBNull))
                    {
                        byte[] bytes = (byte[])dr["Imagen"];
                        using (var ms = new MemoryStream(bytes))
                        {
                            picFoto.Image = Image.FromStream(ms);
                            picFoto.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                    else
                    {
                        picFoto.Image = null; // opcional: imagen por defecto
                    }
                }
            }
        }

        private string ConstruirEdad(string edadTexto, int? anios, DateTime? fechaNac)
        {
            // 1) Si guardaste EdadTexto (ej. “8 meses”), úsalo tal cual
            if (!string.IsNullOrWhiteSpace(edadTexto))
                return edadTexto;

            // 2) Si tienes sólo años
            if (anios.HasValue && anios.Value > 0)
                return $"{anios.Value} años";

            // 3) Si tienes fecha de nacimiento, calculamos años y meses
            if (fechaNac.HasValue)
            {
                var hoy = DateTime.Today;
                int a = hoy.Year - fechaNac.Value.Year;
                int m = hoy.Month - fechaNac.Value.Month;
                if (hoy.Day < fechaNac.Value.Day) m--;
                if (m < 0) { a--; m += 12; }

                if (a <= 0 && m > 0) return $"{m} meses";
                if (a > 0 && m > 0) return $"{a} años {m} meses";
                if (a > 0) return $"{a} años";
            }

            return "-";
        }
    }
}
