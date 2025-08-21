using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AgregarPaciente : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        private int? _idPropietarioEdit;
        private int? _idMascotaEdit;
        public AgregarPaciente()
        {
            InitializeComponent();
        }

        private void btncancelar2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public AgregarPaciente(int? idPropietario = null, int? idMascota = null)
        {
            InitializeComponent();
            _idPropietarioEdit = idPropietario;
            _idMascotaEdit = idMascota;
        }

        // ============ helpers sencillos ============

        private static byte[] BytesFromPicture(PictureBox pic)
        {
            if (pic.Image == null) return null;
            using (var ms = new MemoryStream())
            {
                // PNG para no perder calidad
                pic.Image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static DateTime? FechaNacDesdeEdad(string edadText, string unidadText)
        {
            if (!int.TryParse(edadText, out int e) || e <= 0) return null;
            var hoy = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(unidadText) && unidadText.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                return hoy.AddMonths(-e);  // meses
            return hoy.AddYears(-e);       // años (por defecto)
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (_idMascotaEdit.HasValue && _idPropietarioEdit.HasValue)
            {
                GuardarEdicion(_idPropietarioEdit.Value, _idMascotaEdit.Value);
                return;
            }

            // ===== Validaciones simples =====
            if (string.IsNullOrWhiteSpace(txtMascotaNombre.Text) ||
                string.IsNullOrWhiteSpace(cmbEspecie.Text) ||
                string.IsNullOrWhiteSpace(cmbSexo.Text) ||
                string.IsNullOrWhiteSpace(txtDuenoNombreCompleto.Text) ||
                string.IsNullOrWhiteSpace(txtCelular.Text))
            {
                MessageBox.Show("Completa: Nombre de mascota, Especie, Sexo, Nombre del dueño y Celular.");
                return;
            }

            // Sexo M/F
            char sexo = cmbSexo.Text.Trim().ToUpper().StartsWith("F") ? 'F' : 'M';

            // Peso (opcional)
            object pesoDb = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                if (decimal.TryParse(txtPeso.Text, out var p)) pesoDb = p;
                else { MessageBox.Show("Peso inválido. Ej: 4.2"); return; }
            }

            // Fecha de nacimiento (opcional; calculada desde edad + unidad)
            object fechaNacDb = DBNull.Value;
            var fnac = FechaNacDesdeEdad(txtEdad.Text, cmbEdadUnidad.Text);
            if (fnac.HasValue) fechaNacDb = fnac.Value;

            // Partir nombre completo en Nombre y Apellido (básico)
            string nombre = "(s/n)", apellido = "(s/n)";
            var partes = txtDuenoNombreCompleto.Text.Trim()
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length >= 1) nombre = partes[0];
            if (partes.Length >= 2) apellido = string.Join(" ", partes.Skip(1));

            // Fotos → byte[] (o null)
            var imgPersona = BytesFromPicture(picPropietario);
            var imgMascota = BytesFromPicture(picMascota);

            var cn = _db.AbrirConexion();

            try
            {
                // 1) Persona (dueño)
                int idPersona;
                using (var cmd = new SqlCommand(
                    "INSERT INTO Persona (Nombre, Apellido, Celular, Cedula, Correo, DireccionDomiciliaria, Imagen) " +
                    "VALUES (@Nombre, @Apellido, @Celular, @Cedula, @Correo, @Dir, @Imagen); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);", cn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Apellido", apellido);
                    cmd.Parameters.AddWithValue("@Celular", txtCelular.Text.Trim());
                    cmd.Parameters.AddWithValue("@Cedula", string.IsNullOrWhiteSpace(txtCedula.Text) ? (object)DBNull.Value : txtCedula.Text.Trim());
                    cmd.Parameters.AddWithValue("@Correo", string.IsNullOrWhiteSpace(txtCorreo.Text) ? (object)DBNull.Value : txtCorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Dir", string.IsNullOrWhiteSpace(txtDireccion.Text) ? (object)DBNull.Value : txtDireccion.Text.Trim());
                    var pImg = cmd.Parameters.Add("@Imagen", System.Data.SqlDbType.VarBinary);
                    pImg.Value = (object)imgPersona ?? DBNull.Value;

                    idPersona = (int)cmd.ExecuteScalar();
                }

                // 2) Propietario
                int idPropietario;
                using (var cmd = new SqlCommand(
                    "INSERT INTO Propietario (IdPersona) VALUES (@IdPersona); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);", cn))
                {
                    cmd.Parameters.AddWithValue("@IdPersona", idPersona);
                    idPropietario = (int)cmd.ExecuteScalar();
                }

                // 3) Mascota  (OJO: SIN EdadAnios en el INSERT)
                int idMascota;
                using (var cmd = new SqlCommand(
                    "INSERT INTO Mascota (Imagen, Nombre, Especie, Raza, Sexo, PesoKg, FechaNacimiento, Discapacidad) " +
                    "VALUES (@Imagen, @Nombre, @Especie, @Raza, @Sexo, @PesoKg, @FechaNacimiento, @Discapacidad); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);", cn))
                {
                    var pImgM = cmd.Parameters.Add("@Imagen", System.Data.SqlDbType.VarBinary);
                    pImgM.Value = (object)imgMascota ?? DBNull.Value;

                    cmd.Parameters.AddWithValue("@Nombre", txtMascotaNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Especie", cmbEspecie.Text.Trim());
                    cmd.Parameters.AddWithValue("@Raza", string.IsNullOrWhiteSpace(txtRaza.Text) ? (object)DBNull.Value : txtRaza.Text.Trim());
                    cmd.Parameters.AddWithValue("@Sexo", sexo);
                    cmd.Parameters.AddWithValue("@PesoKg", pesoDb);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacDb);
                    // usa el control que tengas: txtDiscapacidad o cmbDiscapacidad
                    string discap = null;
                    if (this.Controls.Find("cmbDiscapacidad", true).FirstOrDefault() is ComboBox c)
                        discap = c.Text;
                    else if (this.Controls.Find("txtDiscapacidad", true).FirstOrDefault() is TextBox t)
                        discap = t.Text;
                    cmd.Parameters.AddWithValue("@Discapacidad",
                        string.IsNullOrWhiteSpace(discap) ? (object)DBNull.Value : discap.Trim());

                    idMascota = (int)cmd.ExecuteScalar();
                }

                // 4) ENLACE Propietario–Mascota
                using (var cmd = new SqlCommand(
                    "INSERT INTO PropietarioMascota (IdPropietario, IdMascota, EsPrincipal) " +
                    "VALUES (@IdPropietario, @IdMascota, 1);", cn))
                {
                    cmd.Parameters.AddWithValue("@IdPropietario", idPropietario);
                    cmd.Parameters.AddWithValue("@IdMascota", idMascota);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Guardado correctamente ✅", "AniClinic");
                this.DialogResult = DialogResult.OK; // para refrescar en el form padre
                this.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                _db.CerrarConexion();
            }

        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {

        }

        private void AgregarPaciente_Load(object sender, EventArgs e)
        {
            

            cmbEdadUnidad.Items.Clear();
            cmbEdadUnidad.Items.AddRange(new[] { "Años", "Meses" });
            cmbEdadUnidad.SelectedIndex = 0;

            if (_idMascotaEdit.HasValue && _idPropietarioEdit.HasValue)
                CargarParaEditar(_idPropietarioEdit.Value, _idMascotaEdit.Value);


        }

        private void CargarParaEditar(int idPropietario, int idMascota)
        {
            var cn = _db.AbrirConexion();
            try
            {
                string sql = @"
            SELECT 
                m.Nombre AS MascotaNombre, m.Especie, m.Raza, m.Sexo, m.PesoKg, m.FechaNacimiento, m.Discapacidad, m.Imagen AS ImgMascota,
                pe.Nombre, pe.Apellido, pe.Celular, pe.Cedula, pe.Correo, pe.DireccionDomiciliaria, pe.Imagen AS ImgPersona
            FROM Mascota m
            JOIN PropietarioMascota pm ON pm.IdMascota = m.IdMascota
            JOIN Propietario pr ON pr.IdPropietario = pm.IdPropietario
            JOIN Persona pe ON pe.IdPersona = pr.IdPersona
            WHERE pr.IdPropietario = @p AND m.IdMascota = @m;";

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@p", idPropietario);
                    cmd.Parameters.AddWithValue("@m", idMascota);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            // Mascota
                            txtMascotaNombre.Text = rd["MascotaNombre"]?.ToString();
                            cmbEspecie.Text = rd["Especie"]?.ToString();
                            txtRaza.Text = rd["Raza"] as string;
                            cmbSexo.Text = rd["Sexo"]?.ToString();
                            txtPeso.Text = rd["PesoKg"] == DBNull.Value ? "" : Convert.ToDecimal(rd["PesoKg"]).ToString();
                            cmbDiscapacidad.Text = rd["Discapacidad"] as string;

                            // Edad aprox desde FechaNacimiento
                            if (rd["FechaNacimiento"] != DBNull.Value)
                            {
                                var fn = Convert.ToDateTime(rd["FechaNacimiento"]);
                                var hoy = DateTime.Today;
                                int años = hoy.Year - fn.Year - (fn > hoy.AddYears(-(hoy.Year - fn.Year)) ? 1 : 0);
                                txtEdad.Text = Math.Max(años, 0).ToString();
                                cmbEdadUnidad.Text = "Años";
                            }

                            // Imagen mascota
                            if (rd["ImgMascota"] != DBNull.Value)
                            {
                                var bytes = (byte[])rd["ImgMascota"];
                                using (var ms = new System.IO.MemoryStream(bytes))
                                    picMascota.Image = Image.FromStream(ms);
                            }

                            // Propietario
                            string nom = rd["Nombre"]?.ToString();
                            string ape = rd["Apellido"]?.ToString();
                            txtDuenoNombreCompleto.Text = (nom + " " + ape).Trim();
                            txtCelular.Text = rd["Celular"] as string;
                            txtCedula.Text = rd["Cedula"] as string;
                            txtCorreo.Text = rd["Correo"] as string;
                            txtDireccion.Text = rd["DireccionDomiciliaria"] as string;

                            if (rd["ImgPersona"] != DBNull.Value)
                            {
                                var bytes = (byte[])rd["ImgPersona"];
                                using (var ms = new System.IO.MemoryStream(bytes))
                                    picPropietario.Image = Image.FromStream(ms);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los datos: " + ex.Message);
            }
            finally
            {
                _db.CerrarConexion();
            }
        }


        private void GuardarEdicion(int idPropietario, int idMascota)
        {
            // Validaciones mínimas (puedes reusar las tuyas)
            if (string.IsNullOrWhiteSpace(txtMascotaNombre.Text) ||
                string.IsNullOrWhiteSpace(cmbEspecie.Text) ||
                string.IsNullOrWhiteSpace(cmbSexo.Text) ||
                string.IsNullOrWhiteSpace(txtDuenoNombreCompleto.Text) ||
                string.IsNullOrWhiteSpace(txtCelular.Text))
            {
                MessageBox.Show("Completa: Nombre de mascota, Especie, Sexo, Nombre del dueño y Celular.");
                return;
            }

            char sexo = cmbSexo.Text.Trim().ToUpper().StartsWith("F") ? 'F' : 'M';
            object pesoDb = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(txtPeso.Text) && decimal.TryParse(txtPeso.Text, out var p)) pesoDb = p;

            object fechaNacDb = DBNull.Value;
            var fn = FechaNacDesdeEdad(txtEdad.Text, cmbEdadUnidad.Text);
            if (fn.HasValue) fechaNacDb = fn.Value;

            // separar nombre/apellido
            string nombre = "(s/n)", apellido = "(s/n)";
            var partes = txtDuenoNombreCompleto.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length >= 1) nombre = partes[0];
            if (partes.Length >= 2) apellido = string.Join(" ", partes.Skip(1));

            var imgPersona = BytesFromPicture(picPropietario);
            var imgMascota = BytesFromPicture(picMascota);

            var cn = _db.AbrirConexion();
            var tx = cn.BeginTransaction();

            try
            {
                // IdPersona del propietario
                int idPersona;
                using (var cmd = new SqlCommand("SELECT IdPersona FROM Propietario WHERE IdPropietario=@p", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@p", idPropietario);
                    idPersona = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Persona (UPDATE)
                using (var cmd = new SqlCommand(@"
                UPDATE Persona SET
                    Nombre=@Nombre, Apellido=@Apellido, Celular=@Celular, Cedula=@Cedula,
                    Correo=@Correo, DireccionDomiciliaria=@Dir, Imagen=@Imagen
                WHERE IdPersona=@Id;", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Apellido", apellido);
                    cmd.Parameters.AddWithValue("@Celular", txtCelular.Text.Trim());
                    cmd.Parameters.AddWithValue("@Cedula", string.IsNullOrWhiteSpace(txtCedula.Text) ? (object)DBNull.Value : txtCedula.Text.Trim());
                    cmd.Parameters.AddWithValue("@Correo", string.IsNullOrWhiteSpace(txtCorreo.Text) ? (object)DBNull.Value : txtCorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Dir", string.IsNullOrWhiteSpace(txtDireccion.Text) ? (object)DBNull.Value : txtDireccion.Text.Trim());
                    var pImg = cmd.Parameters.Add("@Imagen", System.Data.SqlDbType.VarBinary);
                    pImg.Value = (object)imgPersona ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@Id", idPersona);
                    cmd.ExecuteNonQuery();
                }

                // Mascota (UPDATE)
                using (var cmd = new SqlCommand(@"
                UPDATE Mascota SET
                    Imagen=@Imagen, Nombre=@Nombre, Especie=@Especie, Raza=@Raza, Sexo=@Sexo,
                    PesoKg=@PesoKg, FechaNacimiento=@FechaNacimiento, Discapacidad=@Discapacidad
                WHERE IdMascota=@Id;", cn, tx))
                {
                    var pImgM = cmd.Parameters.Add("@Imagen", System.Data.SqlDbType.VarBinary);
                    pImgM.Value = (object)imgMascota ?? DBNull.Value;

                    cmd.Parameters.AddWithValue("@Nombre", txtMascotaNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Especie", cmbEspecie.Text.Trim());
                    cmd.Parameters.AddWithValue("@Raza", string.IsNullOrWhiteSpace(txtRaza.Text) ? (object)DBNull.Value : txtRaza.Text.Trim());
                    cmd.Parameters.AddWithValue("@Sexo", sexo);
                    cmd.Parameters.AddWithValue("@PesoKg", pesoDb);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacDb);
                    cmd.Parameters.AddWithValue("@Discapacidad", string.IsNullOrWhiteSpace(cmbDiscapacidad.Text) ? (object)DBNull.Value : cmbDiscapacidad.Text.Trim());
                    cmd.Parameters.AddWithValue("@Id", idMascota);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                MessageBox.Show("Actualizado correctamente ✅");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }
                MessageBox.Show("No se pudo actualizar: " + ex.Message);
            }
            finally
            {
                _db.CerrarConexion();
            }
        }



        private void btnFotoPropietario_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Selecciona una foto del propietario";
                ofd.Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                    picPropietario.Image = Image.FromFile(ofd.FileName);
            }
        }

        private void btnFotoMascota_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Selecciona una foto de la mascota";
                ofd.Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                    picMascota.Image = Image.FromFile(ofd.FileName);
            }
        }
    }
}
