using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AgregarPaciente : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        public AgregarPaciente()
        {
            InitializeComponent();
        }
        private void btncancelar2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AgregarPaciente_Load(object sender, EventArgs e)
        {
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
        }

        private void btnFotoPropietario_Click(object sender, EventArgs e)
        {
        }

        private void btnFotoMascota_Click(object sender, EventArgs e)
        {
        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {
        }
    }
}
