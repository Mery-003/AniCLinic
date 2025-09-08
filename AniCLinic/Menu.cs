using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AniCLinic.Login;

namespace AniCLinic
{
    public partial class Menu : Form
    {
        Login login;
        public Menu()
        {
            InitializeComponent();
        }
        public Menu(Login l, string nom, byte[] foto)
        {
            InitializeComponent();
            login = l;
            lblEmpleado.Text = nom;

            if (foto != null && foto.Length > 0)
            {
                using (var ms = new MemoryStream(foto))
                {
                    pcbEmpleado.Image = Image.FromStream(ms);
                }
            }
            else
            {
                pcbEmpleado.Image = Properties.Resources.user_fill;
            }

            pcbEmpleado.SizeMode = PictureBoxSizeMode.CenterImage; 
        }

        private void btnMaximizarMenu_Click(object sender, EventArgs e)
        {
            btnMaximizarMenu.Visible = false;
            btnMinimizarMenu.Visible = true;
            PanelMenu.Visible = false;
            PanelMenu.Width = 248;
            guna2Transition1.ShowSync(PanelMenu);
        }

        private void btnMinimizarMenu_Click(object sender, EventArgs e)
        {
            PanelMenu.Visible = false;
            btnMinimizarMenu.Visible = false;
            btnMaximizarMenu.Visible = true;
            PanelMenu.Width = 50;
            guna2Transition1.ShowSync(PanelMenu);
        }

        private void AbrirEnPanel(Panel contenedor, Form hijo)
        {
            foreach (Control c in contenedor.Controls) c.Dispose();
            contenedor.Controls.Clear();

            hijo.TopLevel = false;
            hijo.FormBorderStyle = FormBorderStyle.None;
            hijo.Dock = DockStyle.Fill;   
            hijo.AutoScroll = true;   
            hijo.AutoScaleMode = AutoScaleMode.None; 

            contenedor.Controls.Add(hijo);
            hijo.BringToFront();
            hijo.Show();
        }


        private void btnPacientes_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new fPacientes());
        }

        private void btnCitas_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new fCitas());
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            login.Show();
            this.Close();
        }

        private void btnhisto_Click(object sender, EventArgs e)
        {
             AbrirEnPanel(pnlMenu1, new fRegistroClinico());
        }


        private void btnReporteria_Click(object sender, EventArgs e)
        {
            panelReporteriaSubmenu.Visible = !panelReporteriaSubmenu.Visible;
        }
        private void PosicionarSubmenuJuntoA(Control anchor, Panel submenu)
        {
            var screenPoint = anchor.Parent.PointToScreen(anchor.Bounds.Location);
            var formPoint = this.PointToClient(new Point(screenPoint.X + anchor.Width, screenPoint.Y));
            submenu.Location = formPoint;
            submenu.BringToFront();
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new fRegistroClinico());
        }

        private void btncarnet_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new FormularioCarnet());
        }

        private void btnhisto_Click_1(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new Historial());
        }

        private void btncarnet_Click_1(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new FormularioCarnet());
        }

        private void btnStock_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new Inventario());
        }

        private void btnProveedor_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new Proveedores());
        }
    }

}
