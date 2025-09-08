using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class NuvProducto : Form
    {
        SqlDataReader oDR;
        public NuvProducto()
        {
            InitializeComponent();
            cargarProveedores();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cargarProveedores()
        {
            csCRUD crud = new csCRUD();
            oDR = crud.EjecutarQuery("Select IdProveedor, NombreProveedor from Proveedor");
            if (oDR != null)
            {
                while (oDR.Read())
                {
                    cmbProveedor.Items.Add(oDR.GetInt32(0).ToString() + " - " + oDR.GetString(1));
                }
            }
        }
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            csProducto producto = new csProducto(
                Convert.ToInt32(cmbProveedor.Text), 
                txtNomProducto.Text, 
                txtDescripcion.Text, 
                cmbCategoria.Text, 
                Convert.ToDecimal(txtPrecio.Text), 
                Convert.ToInt32(txtCantidad.Text));
            if (producto.agregarProducto())
                MessageBox.Show("Producto agregado correctamente.");
            else
                MessageBox.Show("Error al guardar un procducto");
        }
    }
}
