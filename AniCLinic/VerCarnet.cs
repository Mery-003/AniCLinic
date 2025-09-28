using AniCLinic.Properties;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class VerCarnet : Form
    {
        public VerCarnet() { }

        public VerCarnet(SqlDataReader reader)
        {
            InitializeComponent();
            DateTime fecha = DateTime.Now;

            try
            {
                if (reader.Read())
                {
                    lblNombres.Text = reader["Nombre"].ToString();
                    lblDiscapacidad.Text = reader["Discapacidad"].ToString();

                    string fechaNacTxt = "";
                    int idx = reader.GetOrdinal("FechaNacimiento");
                    if (idx >= 0 && !reader.IsDBNull(idx))
                    {
                        DateTime fn = Convert.ToDateTime(reader["FechaNacimiento"]);
                        fechaNacTxt = fn.ToString("yyyy/MM/dd");
                    }
                    lblEdad.Text = fechaNacTxt; 

                    lblEspecie.Text = reader["Especie"].ToString();
                    lblFechaEmision.Text = fecha.ToString("yyyy/MM/dd");
                    lblRaza.Text = reader["Raza"].ToString();
                    lblSexo.Text = reader["Sexo"].ToString();

                    if (!reader.IsDBNull(reader.GetOrdinal("Imagen")))
                    {
                        picFoto.Image = Image.FromStream(new MemoryStream((byte[])reader["Imagen"]));
                        picFoto.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        picFoto.Image = Resources._1084899;
                        picFoto.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
