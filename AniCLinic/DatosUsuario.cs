using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class DatosUsuario : Form
    {
        csPersona persona;
        Image image;
        byte[] foto;
        public DatosUsuario()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            persona = new csPersona(
                txtNombre.Text,
                txtApellido.Text,
                txtCelular.Text,
                txtCedula.Text,
                txtCorreo.Text,
                txtDireccion.Text,
                foto
                );
            if (persona.agregarPersona())
                MessageBox.Show("Persona agregada correctamente");
            else
            {
                MessageBox.Show("Error al guardar la persona.");
                return;
            }
            DatosAcceso datos = new DatosAcceso();
            datos.ShowDialog();
            this.Close();
        }

        private void btnFoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Seleccionar Imagen";
            ofd.Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                image = Image.FromFile(ofd.FileName);
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    foto = ms.ToArray();
                }
            }
        }
    }
}
