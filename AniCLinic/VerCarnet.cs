using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VerCarnet_Load(object sender, EventArgs e)
        {
            CargarCarnet();
        }

        private void CargarCarnet()
        {
            const string sql = @"
        SELECT 
            m.Nombre                         AS Mascota,
            m.Especie, m.Raza, m.Sexo,
            m.PesoKg,
            m.EdadAnios,
            m.Discapacidad,
            m.Imagen
        FROM dbo.Mascota m
        WHERE m.IdMascota = @id;
    ";

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
                    lblEdad.Text = dr["EdadAnios"] is DBNull ? "-" : dr["EdadAnios"].ToString();
                    lblDiscapacidad.Text = dr["Discapacidad"]?.ToString() ?? "-";

                    // Fechas sugeridas (si no las guardas en DB)
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
                        picFoto.Image = null; // o una imagen por defecto
                    }

                }
            }
        }
    }
}
