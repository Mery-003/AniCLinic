using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class FormularioCarnet : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        public FormularioCarnet()
        {
            InitializeComponent();
        }
        private void FormularioCarnet_Load(object sender, EventArgs e)
        {
           
        }
        private void dgvCarnet_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
        }
    }
}
