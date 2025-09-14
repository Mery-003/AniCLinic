using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class DatosAcceso : Form
    {
        csCRUD crud = new csCRUD();
        csUsuario usuario;
        bool admin;
        bool edicion = false;  
        int idEmpleado;         

        public DatosAcceso()
        {
            InitializeComponent();
            cmbCargo.Items.AddRange(new string[] { "Administrador", "Empleado" });
        }

        public DatosAcceso(int idEmpleado, bool ed)
        {
            InitializeComponent();
            cmbCargo.Items.AddRange(new string[] { "Administrador", "Empleado" });
            edicion = ed;
            this.idEmpleado = idEmpleado;

            SqlDataReader reader = crud.EjecutarQuery(
                "SELECT Usuario, Password, Sueldo, Administrador " +
                "FROM Empleados WHERE IdEmpleado = " + idEmpleado);

            if (reader != null && reader.Read())
            {
                txtUsuario.Text = reader["Usuario"].ToString();
                txtContraseña.Text = reader["Password"].ToString();
                txtSueldo.Text = reader["Sueldo"].ToString();
                
                bool esAdmin = Convert.ToBoolean(reader["Administrador"]);
                cmbCargo.SelectedIndex = esAdmin ? 0 : 1;
            }
        }

        private void btnGuardarR_Click(object sender, EventArgs e)
        {
            if (!validar())
                return;

            admin = (cmbCargo.SelectedIndex == 0);

            usuario = new csUsuario(
                txtUsuario.Text,
                txtContraseña.Text,
                Convert.ToDecimal(txtSueldo.Text),
                admin
            );

            if (!edicion) 
            {
                SqlDataReader reader = crud.EjecutarQuery(
                    "SELECT TOP 1 IdPersona FROM Persona ORDER BY IdPersona DESC");

                if (reader != null && reader.Read())
                {
                    int idPersona = Convert.ToInt32(reader["IdPersona"]);
                    if (usuario.agregarUsuario(idPersona))
                        MessageBox.Show("Usuario agregado correctamente.");
                    else
                        MessageBox.Show("Error al agregar usuario.");
                }
            }
            else 
            {
                if (usuario.editarUsuario(idEmpleado))
                    MessageBox.Show("Usuario editado correctamente.");
                else
                    MessageBox.Show("Error al editar usuario.");
            }

            this.Close();
        }
        private bool validar()
        {
            if (txtUsuario.Text.Length < 5)
            {
                MessageBox.Show("Usuario demasiado corto.");
                return false;
            }
            if (txtContraseña.Text.Length < 8)
            {
                MessageBox.Show("Contraseña muy debil.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Llene todos los campos.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                MessageBox.Show("Llene todos los campos.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSueldo.Text))
            {
                MessageBox.Show("Llene todos los campos.");
                return false;
            }
            if (cmbCargo.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un cargo.");
                return false;
            }
            return true;
        }
    }
}
