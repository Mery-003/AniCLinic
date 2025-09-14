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
    public partial class FMenuAdmin : Form
    {
        Menu menu;
        Panel panel;
        public FMenuAdmin()
        {
            InitializeComponent();
        }
        public FMenuAdmin(Menu m, Panel p)
        {
            InitializeComponent();
            menu = m;
            panel = p;
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            menu.AbrirEnPanel(panel, new Proveedores());
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            menu.AbrirEnPanel(panel, new FUsuariosAdmin());
        }

        private void btnDiseño_Click(object sender, EventArgs e)
        {
            menu.AbrirEnPanel(panel, new frReportes());
        }
    }

}
