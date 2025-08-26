using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;

            txtPassword.UseSystemPasswordChar = true;

        }



        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            csConexionBD conexionBD = new csConexionBD();
            int inicio = conexionBD.login("Select * from Usuario", txtUsuario.Text, txtPassword.Text);
            if (inicio > 0)
            {
                Menu menu = new Menu(this);
                this.Hide();
                menu.ShowDialog();
            }
            else
                MessageBox.Show("Usuario o contraseña incorrectos");

        }

        private void btnVerPass_MouseDown(object sender, MouseEventArgs e)
        {
            txtPassword.UseSystemPasswordChar = false;
        }

        private void btnVerPass_MouseUp(object sender, MouseEventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
        }
    }
}
