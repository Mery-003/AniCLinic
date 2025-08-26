using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Historial : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        public Historial()
        {
            InitializeComponent();
        }

        private void Historial_Load(object sender, EventArgs e)
        {
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
        }
        private void dgvDatos_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}
