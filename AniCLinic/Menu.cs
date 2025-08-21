using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AniCLinic
{
    public partial class Menu : Form
    {
        Login nLogin;
        
        public Menu(Login login)
        {
            InitializeComponent();
            nLogin = login;

        }
        
        private void OcultarSubMenus(object sender, EventArgs e)
        {
            
        }



        private void PanelMenu_Paint(object sender, PaintEventArgs e)
        {

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

        private void Menu_Load(object sender, EventArgs e)
        {

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

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
         
        }

        private void btnPacientes_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new fPacientes());
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void btnCitas_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new fCitas());
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            nLogin.Visible = true;
            this.Close();
        }

        private void btnhisto_Click(object sender, EventArgs e)
        {
            AbrirEnPanel(pnlMenu1, new fHistorialVeterinario());
        }

        private void btnReporteria_Click(object sender, EventArgs e)
        {
            
        }
        private void PosicionarSubmenuJuntoA(Control anchor, Panel submenu)
        {
            // Ubicar el panel justo al lado derecho del botón
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
            AbrirEnPanel(pnlMenu1, new Carnet());
        }
    }

}
