using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class frReportes : Form
    {
        public frReportes()
        {
            InitializeComponent();
        }

        private void txtAño_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtAño.Text.Length >= 4) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void btnAño_Click(object sender, EventArgs e)
        {
            frReporteFinanciero reporte = new frReporteFinanciero(txtAño.Text, "Mes");
            reporte.ShowDialog();
        }

        private void btnProducto_Click(object sender, EventArgs e)
        {
            frReporteFinanciero reporte = new frReporteFinanciero(txtAño.Text, "Producto");
            reporte.ShowDialog();
        }

        private void btnListaProductos_Click(object sender, EventArgs e)
        {
            frReporteFinanciero reporte = new frReporteFinanciero(txtAño.Text, "ListaProducto");
            reporte.ShowDialog();
        }

        private void guna2TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtFactura.Text.Length >= 4) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFactura.Text))
            {
                MessageBox.Show("Ingrese un id de factura.");
                return;
            }
            frFactura factura = new frFactura(Convert.ToInt32(txtFactura.Text));
            factura.ShowDialog();
        }
    }
}
