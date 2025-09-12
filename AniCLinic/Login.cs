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

        public static class SesionActual
        {
            public static int IdEmpleado { get; set; }
            public static string NombreEmpleado { get; set; }
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
            bool admin = false;
            string nombre = null;
            byte[] foto = null;
            csCRUD crud = new csCRUD();
            int idUsuario = crud.login("Select * from Empleados", txtUsuario.Text, txtPassword.Text);

            if (idUsuario > 0)
            {

                SqlDataReader oDTR = crud.EjecutarQuery("SELECT E.IdEmpleado, E.IdPersona, P.Nombre, P.Apellido, P.Imagen, E.Administrador " +
                    "FROM Empleados E INNER JOIN Persona P ON E.IdPersona = P.IdPersona " +
"                   WHERE E.IdEmpleado = " + idUsuario);
                if (oDTR.Read())
                {
                    nombre = "Dr. " + oDTR["Nombre"].ToString() + " " + oDTR["Apellido"].ToString();
                    admin = (bool)oDTR["Administrador"];
                    if (oDTR["Imagen"] != DBNull.Value && oDTR["Imagen"] is byte[])
                    {
                         foto = (byte[])oDTR["Imagen"];
                    }
                    else
                    {
                         foto = (byte[])new ImageConverter().ConvertTo(Properties.Resources.user_fill, typeof(byte[]));
                    }
                }
                Menu menu = new Menu(this, nombre, foto, admin);
                txtUsuario.Text = "";
                txtPassword.Text = "";
                this.Hide();
                menu.ShowDialog();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos");
            }
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
