using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class VerCarnet : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        public VerCarnet(int idMascota)
        {
            InitializeComponent();
        }

        private void btnCerrar_Click(object sender, EventArgs e) => Close();

        private void VerCarnet_Load(object sender, EventArgs e)
        {
        }

    }
}
