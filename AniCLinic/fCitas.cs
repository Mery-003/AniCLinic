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
    public partial class fCitas : Form
    {
        public fCitas()
        {
            InitializeComponent();
        }

        private void btnNuvCita_Click(object sender, EventArgs e)
        {
            AggCita frm = new AggCita();  
            frm.Show();
        }
    }
}
