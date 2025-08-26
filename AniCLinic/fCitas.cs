using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fCitas : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        public fCitas()
        {
            InitializeComponent();
        }

        private void btnNuvCita_Click(object sender, EventArgs e)
        {
        }

        private void fCitas_Load(object sender, EventArgs e)
        {
        }
        private void dgvCita_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

    }
}
