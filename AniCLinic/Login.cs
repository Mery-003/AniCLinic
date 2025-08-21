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
        private readonly ConexionBD _db = new ConexionBD();
        public Login()
        {
            InitializeComponent();
            // Enter hace click en LOGIN
            this.AcceptButton = btnLogin;

            // Oculta contraseña por defecto
            txtPassword.UseSystemPasswordChar = true;

            // Mostrar/ocultar contraseña
            chkVer.CheckedChanged += (s, e) =>
            {
                // Para TextBox normal:
                txtPassword.UseSystemPasswordChar = !chkVer.Checked;

                // Si usas Guna2 únicamente:
                // txtPassword.PasswordChar = chkVer.Checked ? '\0' : '●';
            };

        }

        private static string Sha256Hex(string text)
        {
            using (var sha = SHA256.Create())
            {
                // IMPORTANTE: usar Unicode (UTF-16 LE) para que coincida con HASHBYTES sobre NVARCHAR
                var bytes = sha.ComputeHash(Encoding.Unicode.GetBytes(text));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
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
            var user = txtUsuario.Text.Trim();
            var pass = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Ingresa usuario y contraseña.");
                return;
            }

            UsuarioSesion sesion = null;
            var cn = _db.AbrirConexion();

            try
            {
                using (var cmd = new SqlCommand(@"
SELECT TOP 1 
    u.IdUsuario, u.Usuario, u.PasswordHash, u.Activo,
    e.IdEmpleado, e.Cargo,
    p.Nombre, p.Apellido
FROM Usuario u
JOIN Empleado e ON e.IdEmpleado = u.IdEmpleado
JOIN Persona  p ON p.IdPersona  = e.IdPersona
WHERE u.Usuario = @u;", cn))
                {
                    cmd.Parameters.AddWithValue("@u", user);

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.");
                            return;
                        }

                        if (!(bool)rd["Activo"])
                        {
                            MessageBox.Show("El usuario está inactivo.");
                            return;
                        }

                        var hashDb = rd["PasswordHash"].ToString();
                        var hashIn = Sha256Hex(pass);

                        if (!hashIn.Equals(hashDb, StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.");
                            return;
                        }

                        // Construir la sesión
                        sesion = new UsuarioSesion
                        {
                            IdUsuario = Convert.ToInt32(rd["IdUsuario"]),
                            IdEmpleado = Convert.ToInt32(rd["IdEmpleado"]),
                            Usuario = rd["Usuario"].ToString(),
                            NombreCompleto = $"{rd["Nombre"]} {rd["Apellido"]}",
                            Cargo = rd["Cargo"]?.ToString()
                        };
                    }
                }

                // Abrir Menu si autenticó
                if (sesion != null)
                {
                    this.Hide();
                    using (var f = new Menu(sesion))
                    {
                        f.ShowDialog();
                    }
                    this.Show();
                    txtPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message);
            }
            finally
            {
                _db.CerrarConexion();
            }
        }

        public class UsuarioSesion
        {
            public int IdUsuario { get; set; }
            public int IdEmpleado { get; set; }
            public string Usuario { get; set; }
            public string NombreCompleto { get; set; }
            public string Cargo { get; set; }
        }

    }
}
