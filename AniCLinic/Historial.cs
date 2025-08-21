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
    public partial class Historial : Form
    {
        public Historial()
        {
            InitializeComponent();
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            VerHistorial frm = new VerHistorial();
            frm.Show();
        }
    }
}
