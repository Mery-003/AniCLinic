using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggProveedor : Form
    {
        csCRUD crud = new csCRUD();
        csProveedor proveedor;
        Proveedores pro;
        bool edicion = false;
        int idProv;
        public AggProveedor()
        {
            InitializeComponent();
        }
        public AggProveedor(Proveedores proveedores, int id)
        {
            InitializeComponent();
            pro = proveedores;
            idProv = id;
            edicion = true;

            if (id > 0)
            {
                proveedor = CargarProveedor(id);

                if (proveedor == null)
                {
                    MessageBox.Show("No se pudo encontrar el proveedor.");
                    return;
                }

                txtNombre.Text = proveedor.NombreProveedor;
                txtCedula.Text = proveedor.RUC;
                txtTelefono.Text = proveedor.Telefono;
                txtCorreo.Text = proveedor.Correo;
                txtDireccion.Text = proveedor.Direccion;
            }
        }

        private csProveedor CargarProveedor(int id)
        {
            proveedor = null;
            string sentencia = "Select * from Proveedor Where IdProveedor = " + id;
            using (SqlDataReader reader = crud.EjecutarQuery(sentencia))
            {
                if (reader.Read() && reader != null)
                {
                    proveedor = new csProveedor(
                        reader["NombreProveedor"].ToString(),
                        reader["RUC"].ToString(),
                        reader["Telefono"].ToString(),
                        reader["Correo"].ToString(),
                        reader["Direccion"].ToString()
                        );
                }
            }
            return proveedor;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!edicion)
            {
                proveedor = new csProveedor(
                    txtNombre.Text,
                    txtCedula.Text,
                    txtTelefono.Text,
                    txtCorreo.Text,
                    txtDireccion.Text
                    );
                if (proveedor.agregarProveedor())
                    MessageBox.Show("Proveedor añadido correctamente.");
                else
                    MessageBox.Show("Error al añadir el Proveedor.");
            }
            else
            {
                proveedor = new csProveedor(
                    txtNombre.Text,
                    txtCedula.Text,
                    txtTelefono.Text,
                    txtCorreo.Text,
                    txtDireccion.Text
                    );
                if (proveedor.editarProveedor(idProv))
                    MessageBox.Show("Proveedor editado correctamente.");
                else
                    MessageBox.Show("Error al editar el Proveedor.");
            }
        }
    }
}
