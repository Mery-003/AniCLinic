using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

            Menu menu = new Menu(this);
            this.Visible = false;
            menu.ShowDialog();
        }
    }
}
