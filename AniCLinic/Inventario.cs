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
    public partial class Inventario : Form
    {
        public Inventario()
        {
            InitializeComponent();
            cargarDataI();
        }
        public void cargarDataI(string filtro = "")
        {
            csCRUD crud = new csCRUD();
            string sentencia = "Select * from Inventario " +
                "Where NombreProducto like @filtro + '%'";
            dgvInventario.DataSource = crud.cargarBDData(sentencia, new SqlParameter ("@filtro", filtro));
        }

        private void btnMasProd_Click(object sender, EventArgs e)
        {
            MasProductos mProductos = new MasProductos();
            mProductos.ShowDialog();
        }

        private void btnAggProd_Click(object sender, EventArgs e)
        {
            NuvProducto nProducto = new NuvProducto();
            nProducto.ShowDialog();
        }

        private void txtBuscarHoy_TextChanged(object sender, EventArgs e)
        {
            cargarDataI(txtBuscarHoy.Text);
        }
    }
}
