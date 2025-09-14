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
    public partial class DatosUsuario : Form
    {
        FUsuariosAdmin fUsuarios;
        csCRUD crud = new csCRUD();
        csPersona persona;
        Image image;
        bool edicion = false;
        byte[] foto = null;
        int idEmpleado = 0;
        public DatosUsuario()
        {
            InitializeComponent();
        }
        public DatosUsuario(FUsuariosAdmin us, int IdEmpleado)
        {
            InitializeComponent();
            fUsuarios = us;
            edicion = true;
            idEmpleado = IdEmpleado;
            if (idEmpleado > 0)
            {
                persona = CargarPersona(idEmpleado);

                if(persona == null)
                {
                    MessageBox.Show("No se pudo cargar el empleado");
                    return;
                }
                txtNombre.Text = persona.Nombre;
                txtApellido.Text = persona.Apellido;
                txtCedula.Text = persona.Cedula;
                txtCelular.Text = persona.Celular;
                txtCorreo.Text = persona.Correo;
                txtDireccion.Text = persona.Direccion;
            }
        }
        private csPersona CargarPersona(int id)
        {
            persona = null;
            string sentencia = "SELECT * FROM Persona P inner join Empleados E " +
                "on P.IdPersona=E.IdPersona WHERE E.IdEmpleado = " + id;

            using (SqlDataReader reader = crud.EjecutarQuery(sentencia))
            {
                if (reader != null && reader.Read())
                {
                    byte[] imagenBytes = null;
                    if (!(reader["Imagen"] is DBNull))
                    {
                        imagenBytes = (byte[])reader["Imagen"];
                    }

                    persona = new csPersona(
                        reader["Nombre"].ToString(),
                        reader["Apellido"].ToString(),
                        reader["Celular"].ToString(),
                        reader["Cedula"].ToString(),
                        reader["Correo"].ToString(),
                        reader["DireccionDomiciliaria"].ToString(),
                        imagenBytes
                    );
                }
            }
            return persona;
        }
        private bool validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Llene todos los campos.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Llene todos los campos.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCelular.Text))
            {
                MessageBox.Show("Llene todos los campos.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("Llene todos los campos.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MessageBox.Show("Llene todos los campos.");
                return false;
            }
            return true;
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!validar())
                return;
            persona = new csPersona(
                txtNombre.Text,
                txtApellido.Text,
                txtCelular.Text,
                txtCedula.Text,
                txtCorreo.Text,
                txtDireccion.Text,
                foto
                );

            if (!edicion) 
            {
                if (persona.agregarPersona())
                    MessageBox.Show("Persona agregada correctamente");
                else
                {
                    MessageBox.Show("Error al guardar la persona.");
                    return;
                }
            }
            else 
            {
                if (persona.editarPersona(idEmpleado))
                    MessageBox.Show("Persona editada correctamente");
                else
                {
                    MessageBox.Show("Error al editar la persona.");
                    return;
                }
            }

            DatosAcceso datos = new DatosAcceso(idEmpleado, edicion);
            this.Visible = false;
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

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
        }

        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
        }

        private void txtCelular_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCelular.Text.Length >= 10) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCedula.Text.Length >= 10) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
    }
}
