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
    public partial class ValorCita : Form
    {
        public ValorCita()
        {
            InitializeComponent();
            
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtValorCita.Text.Length <= 0)
            {
                MessageBox.Show("Ingrese el valor de la cita.", "", MessageBoxButtons.OK);
            }
            Menu menu = Application.OpenForms["Menu"] as Menu;
            menu.AbrirEnPanel(menu.pnlMenu1, new Ventas(menu.idEmpleado, Convert.ToDecimal(txtValorCita.Text), true));
            this.Close();
        }

        private void txtValorCita_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((char.IsDigit(e.KeyChar) && txtValorCita.Text.Replace(",", "").Replace(".", "").Length >= 5)
                || (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != ',')
                || (e.KeyChar == ',' && txtValorCita.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }
    }
}
