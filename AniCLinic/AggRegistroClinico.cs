using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggRegistroClinico : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        public AggRegistroClinico()
        {
            InitializeComponent();

        }
        private void AggRegistroClinico_Load(object sender, EventArgs e)
        {

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
        }

        private void btnCancelar_Click(object sender, EventArgs e) => Close();
        private void txtPropietario_TextChanged(object sender, EventArgs e) { }
        private void txtMascota_TextChanged(object sender, EventArgs e) { }
        private void txtDiagnostico_TextChanged(object sender, EventArgs e) { }
        private void txtMotivo_TextChanged(object sender, EventArgs e) { }
        private void txtTratamiento_TextChanged(object sender, EventArgs e) { }
        private void txtReceta_TextChanged(object sender, EventArgs e) { }
        private void txtVeterinario_TextChanged(object sender, EventArgs e) { }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }
    }
}
