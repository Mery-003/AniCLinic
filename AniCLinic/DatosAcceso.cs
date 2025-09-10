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
    public partial class DatosAcceso : Form
    {
        public DatosAcceso()
        {
            InitializeComponent();
            cmbCargo.Items.AddRange(new string[] {"Administrador", "Empleado"});

        }

        private void btnGuardarR_Click(object sender, EventArgs e)
        {

        }
    }
}
