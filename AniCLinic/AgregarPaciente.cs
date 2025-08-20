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
    public partial class AgregarPaciente : Form
    {
        csMascota mascota;
        csConexionBD conexionBD;
        public AgregarPaciente()
        {
            InitializeComponent();
        }

        private void btncancelar2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            conexionBD = new csConexionBD();
            using (MemoryStream ms = new MemoryStream())
            {
                ptbMascota.Image.Save(ms, ptbMascota.Image.RawFormat);
                byte[] imagenBytes = ms.ToArray();
                mascota = new csMascota(conexionBD.devolverID(),txtMascotaNombre.Text, txtPropietarioNombre.Text, cmbEspecie.Text, txtRaza.Text, cmbSexo.Text, txtEdad.Text + " " + cmbTipo.Text, cmbDiscapacidad.Text, imagenBytes);
                int comp = mascota.agregarMascota();
                if (comp == 1)
                    MessageBox.Show("Datos guardados correctamente", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                else
                    MessageBox.Show("No se pudieron guardar los datos","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txtMascotaNombre.Text = "";
            txtCorreo.Text = "";
            txtDireccion.Text = "";
            txtEdad.Text = "";
            txtNumeroVivienda.Text = "";
            txtOcupacion.Text = "";
            txtPropietarioNombre.Text = "";
            txtRaza.Text = "";
            cmbDiscapacidad.SelectedItem = -1;
            cmbTipo.SelectedItem = -1;
            cmbSexo.SelectedItem = -1;
            cmbEspecie.SelectedItem = -1;
        }
    }
}
