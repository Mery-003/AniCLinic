using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fPacientes : Form
    {
        public fPacientes()
        {
            InitializeComponent();
            csConexionBD conexionBD = new csConexionBD();
            dgvPacientes.DataSource = conexionBD.retornaRegistro("Select * from tblMascotas");
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            AgregarPaciente frm = new AgregarPaciente();
            frm.Show();
        }

        private void fPacientes_Load(object sender, EventArgs e)
        {
           
        }

        private void guna2GradientButton1_Click(object sender, EventArgs e) { }
        private void eliminarpaciente_Click(object sender, EventArgs e) { }
    }
}
