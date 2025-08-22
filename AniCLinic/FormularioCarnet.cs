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
    public partial class FormularioCarnet : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        public FormularioCarnet(int idRegistro)
        {
            InitializeComponent();

        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            Carnet frm = new Carnet();
            frm.Show();
        }

        private void FormularioCarnet_Load(object sender, EventArgs e)
        {
            //CargarCarnets();
            //dgvCarnet.CellClick += dgvCarnet_CellClick; // engancha el evento
        }
    }
}
