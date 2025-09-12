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
    public partial class Ventas : Form
    {
        public Ventas()
        {
            InitializeComponent();
            visible(false);
        }
        private void visible(bool bvisible)
        {
            lblIVA.Visible = bvisible;
            lblIVAno.Visible = bvisible;
            lblTotalno.Visible = bvisible;
            lblTotal.Visible = bvisible;
            lblTtlVenta.Visible = bvisible;
            lblTtlVno.Visible = bvisible;
            btnImprimir.Visible = bvisible;
        }
        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            btnFinalizar.Location = new Point(873, 407);
            dgvVentas.Size = new Size(811, 255);
            visible(true);
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
