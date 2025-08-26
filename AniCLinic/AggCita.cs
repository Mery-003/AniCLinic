using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggCita : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        public AggCita()
        {
            InitializeComponent();

        }

        public AggCita(int idEmpleadoSesion, string nombreEmpleadoSesion) : this()
        {

        }

        public AggCita(int idCita) : this()
        {

        }

        private sealed class MascotaItem
        {
        }


        private void AggCita_Load(object sender, EventArgs e)
        {

        }

        private void AggCita_Shown(object sender, EventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, EventArgs e) => Close();

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscarVet_Click(object sender, EventArgs e) { }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { }
    }
}
