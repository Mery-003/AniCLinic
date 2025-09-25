using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggPropietario : Form
    {
        private readonly csCRUD _crud = new csCRUD();
        private readonly fPacientes _parent;
        private readonly int _idPersona;

        // NUEVO: exponer el Id del propietario guardado
        public int IdPersonaGuardado { get; private set; } = 0;

        public AggPropietario(fPacientes parent, int idPersona = 0)
        {
            InitializeComponent();
            _parent = parent;
            _idPersona = idPersona;

            // Eventos UI
            btnSeleccionarFoto.Click += btnSeleccionarFoto_Click;
            btnGuardarPropietario.Click += btnGuardarPropietario_Click;
            btnCancelarPropietario.Click += (s, e) => this.Close();

            // Solo números y hasta 10 dígitos
            txtCelular.KeyPress += SoloNumero_KeyPress;
            txtCedula.KeyPress += SoloNumero_KeyPress;
            txtCelular.TextChanged += Limitar10;
            txtCedula.TextChanged += Limitar10;

            if (_idPersona > 0) CargarPropietario(_idPersona);
            else Limpiar();
        }

        private void Limpiar()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCelular.Text = "";
            txtCedula.Text = "";
            txtCorreo.Text = "";
            txtDireccion.Text = "";
            if (picPropietario != null) picPropietario.Image = null;
        }

        private static void SoloNumero_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private static void Limitar10(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                int pos = tb.SelectionStart;

                if (tb.Text.Length > 10)
                {
                    tb.Text = tb.Text.Substring(0, 10);
                    tb.SelectionStart = Math.Min(pos, tb.Text.Length);
                }
            }

            if (sender is Guna.UI2.WinForms.Guna2TextBox gtb)
            {
                int pos = gtb.SelectionStart;

                if (gtb.Text.Length > 10)
                {
                    gtb.Text = gtb.Text.Substring(0, 10);
                    gtb.SelectionStart = Math.Min(pos, gtb.Text.Length);
                }
            }
        }

        private static byte[] ImageToBytesOrNull(Image img)
        {
            if (img == null) return null;

            using (var ms = new MemoryStream())
            using (var bmp = new Bitmap(img))
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static Image BytesToImageOrNull(object blob)
        {
            if (blob == null || blob == DBNull.Value) return null;
            try { using (var ms = new MemoryStream((byte[])blob)) return Image.FromStream(ms); }
            catch { return null; }
        }

        private void CargarPropietario(int idPersona)
        {
            string sql = @"
SELECT IdPersona, Nombre, Apellido, Celular, Cedula, Correo, DireccionDomiciliaria, Imagen
FROM Persona WHERE IdPersona = " + idPersona;

            SqlDataReader rd = null;
            try
            {
                rd = _crud.EjecutarQuery(sql);
                if (rd != null && rd.Read())
                {
                    txtNombre.Text = rd["Nombre"] + "";
                    txtApellido.Text = rd["Apellido"] + "";
                    txtCelular.Text = rd["Celular"] + "";
                    txtCedula.Text = rd["Cedula"] + "";
                    txtCorreo.Text = rd["Correo"] + "";
                    txtDireccion.Text = rd["DireccionDomiciliaria"] + "";
                    if (rd["Imagen"] != DBNull.Value)
                    {
                        using (var ms = new MemoryStream((byte[])rd["Imagen"]))
                        using (var tempImg = Image.FromStream(ms))
                        {
                            picPropietario.Image = new Bitmap(tempImg);
                            picPropietario.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                    }
                }
                else MessageBox.Show("No se encontró el propietario.");
            }
            finally
            {
                if (rd != null) rd.Close();
                try { _crud.conexion.cerrarConexion(); } catch { }
            }
        }

        // ✅ Validar: correo obligatorio + formato
        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Ingrese el nombre."); return false; }
            if (string.IsNullOrWhiteSpace(txtApellido.Text)) { MessageBox.Show("Ingrese el apellido."); return false; }
            if (string.IsNullOrWhiteSpace(txtCelular.Text)) { MessageBox.Show("Ingrese el celular."); return false; }
            if (string.IsNullOrWhiteSpace(txtCedula.Text)) { MessageBox.Show("Ingrese la cédula."); return false; }

            if (txtCelular.Text.Trim().Length > 10) { MessageBox.Show("Celular debe tener máx. 10 dígitos."); return false; }
            if (txtCedula.Text.Trim().Length > 10) { MessageBox.Show("C.I. debe tener máx. 10 dígitos."); return false; }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MessageBox.Show("Ingrese el correo electrónico (obligatorio).");
                return false;
            }

            string correo = txtCorreo.Text.Trim();
            if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Correo no válido. Ejemplo: nombre@dominio.com");
                return false;
            }

            return true;
        }

        private void btnSeleccionarFoto_Click(object sender, EventArgs e)
        {
            if (picPropietario == null) return;
            using (var ofd = new OpenFileDialog { Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        picPropietario.Image = Image.FromFile(ofd.FileName);
                        picPropietario.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch { MessageBox.Show("No se pudo cargar la imagen."); }
                }
            }
        }

        private bool CedulaDuplicada(string ci, int excluirId = 0)
        {
            string sql = excluirId > 0
                ? "SELECT COUNT(1) FROM Persona WHERE Cedula=@ci AND IdPersona<>@id"
                : "SELECT COUNT(1) FROM Persona WHERE Cedula=@ci";
            DataTable dt = _crud.cargarBDData(sql, new SqlParameter("@ci", ci), new SqlParameter("@id", excluirId));
            int n = (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : 0;
            return n > 0;
        }

        // ✅ Nuevo: correo duplicado
        private bool CorreoDuplicado(string email, int excluirId = 0)
        {
            string sql = excluirId > 0
                ? "SELECT COUNT(1) FROM Persona WHERE Correo=@co AND IdPersona<>@id"
                : "SELECT COUNT(1) FROM Persona WHERE Correo=@co";
            DataTable dt = _crud.cargarBDData(sql, new SqlParameter("@co", email), new SqlParameter("@id", excluirId));
            int n = (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : 0;
            return n > 0;
        }

        // ✅ Guardar con correo obligatorio + duplicado
        private void btnGuardarPropietario_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;

            if (_idPersona > 0 && CedulaDuplicada(txtCedula.Text.Trim(), _idPersona))
            { MessageBox.Show("La cédula ya pertenece a otro propietario."); return; }
            if (_idPersona == 0 && CedulaDuplicada(txtCedula.Text.Trim()))
            { MessageBox.Show("La cédula ya existe."); return; }

            if (_idPersona > 0 && CorreoDuplicado(txtCorreo.Text.Trim(), _idPersona))
            { MessageBox.Show("El correo ya pertenece a otro propietario."); return; }
            if (_idPersona == 0 && CorreoDuplicado(txtCorreo.Text.Trim()))
            { MessageBox.Show("El correo ya existe."); return; }

            string correo = txtCorreo.Text.Trim(); // obligatorio y validado
            object direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? (object)DBNull.Value : txtDireccion.Text.Trim();
            SqlParameter pImg = new SqlParameter("@Imagen", SqlDbType.VarBinary)
            { Value = (object)ImageToBytesOrNull(picPropietario == null ? null : picPropietario.Image) ?? DBNull.Value };

            if (_idPersona > 0)
            {
                string up = @"UPDATE Persona SET
                              Nombre=@n, Apellido=@a, Celular=@cel, Cedula=@ci,
                              Correo=@co, DireccionDomiciliaria=@dir, Imagen=@Imagen
                              WHERE IdPersona=@id;";
                bool ok = _crud.editarBD(up,
                    new SqlParameter("@n", txtNombre.Text.Trim()),
                    new SqlParameter("@a", txtApellido.Text.Trim()),
                    new SqlParameter("@cel", txtCelular.Text.Trim()),
                    new SqlParameter("@ci", txtCedula.Text.Trim()),
                    new SqlParameter("@co", correo),
                    new SqlParameter("@dir", direccion),
                    pImg,
                    new SqlParameter("@id", _idPersona));
                if (!ok) { MessageBox.Show("No se pudo actualizar."); return; }
                IdPersonaGuardado = _idPersona;
                MessageBox.Show("Propietario actualizado.");
                try { _parent?.RefrescarPropietarios(); } catch { }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                string ins = @"INSERT INTO Persona
                               (Nombre, Apellido, Celular, Cedula, Correo, DireccionDomiciliaria, Imagen)
                               VALUES (@n, @a, @cel, @ci, @co, @dir, @Imagen);";
                bool ok = _crud.agregarBD(ins,
                    new SqlParameter("@n", txtNombre.Text.Trim()),
                    new SqlParameter("@a", txtApellido.Text.Trim()),
                    new SqlParameter("@cel", txtCelular.Text.Trim()),
                    new SqlParameter("@ci", txtCedula.Text.Trim()),
                    new SqlParameter("@co", correo),
                    new SqlParameter("@dir", direccion),
                    pImg);
                if (!ok) { MessageBox.Show("No se pudo registrar."); return; }

                DataTable dt = _crud.cargarBDData("SELECT IdPersona FROM Persona WHERE Cedula=@ci;",
                    new SqlParameter("@ci", txtCedula.Text.Trim()));
                if (dt != null && dt.Rows.Count > 0)
                    IdPersonaGuardado = Convert.ToInt32(dt.Rows[0][0]);

                MessageBox.Show("Propietario registrado.");
                try { _parent?.RefrescarPropietarios(); } catch { }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
