using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fRegistroClinico : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        public fRegistroClinico()
        {
            InitializeComponent();
        }

        private void fRegistroClinico_Load(object sender, EventArgs e) 
        {
        }
        private void dvgHoy_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dvgProxima_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dvgAnteriores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

    }
}
