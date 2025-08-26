using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fPacientes : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        public fPacientes()
        {
            InitializeComponent();
        }

        private void fPacientes_Load(object sender, EventArgs e)
        {
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e) { }
        private void btnAgregar_Click(object sender, EventArgs e) { }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
        }

        private void dgvDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void guna2GradientButton1_Click(object sender, EventArgs e) { }
        private void eliminarpaciente_Click(object sender, EventArgs e) { }
    }
}
