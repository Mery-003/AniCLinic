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
        public VerCarnet(csMascota mascota)
        {
            InitializeComponent();
            DateTime fecha = DateTime.Now;
            lblNombres.Text = mascota.Nombre;
            lblEspecie.Text = mascota.Especie;
            lblRaza.Text = mascota.Raza;
            lblSexo.Text = mascota.Sexo;
            lblEdad.Text = mascota.Edad;
            lblDiscapacidad.Text = mascota.Especie;
            lblFechaEmision.Text = fecha.ToString("dd/MM/yyyy");
            if (mascota.Foto != null && mascota.Foto.Length > 0)
            {
                try
                {
                    picFoto.Image = Image.FromStream(new MemoryStream(mascota.Foto));
                    picFoto.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch
                {
                    picFoto.Image = Resources._1084899;
                }
            }
            else
            {
                picFoto.Image = Resources._1084899;
            }
        }
        

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
