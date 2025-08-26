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
        private int? _idPropietarioEdit;
        private int? _idMascotaEdit;

        public AgregarPaciente()
        {
            InitializeComponent();
        }

        public AgregarPaciente(int? idPropietario = null, int? idMascota = null)
        {
            InitializeComponent();
            _idPropietarioEdit = idPropietario;
            _idMascotaEdit = idMascota;
        }

        private void btncancelar2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ====================== Helpers ======================

        // Detecta si estamos dentro del diseñador de VS
        private static bool IsInDesigner()
        {
            try
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;
            }
            catch { }
            var p = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            return string.Equals(p, "devenv", StringComparison.OrdinalIgnoreCase);
        }

        private static byte[] BytesFromPicture(PictureBox pic)
        {
            if (pic.Image == null) return null;
            using (var ms = new MemoryStream())
            {
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
            return hoy.AddYears(-e);      // años (por defecto)
        }

        private static string LimpiarSn(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            return s.Equals("(s/n)", StringComparison.OrdinalIgnoreCase) ? "" : s;
        }

        private static string SoloDigitos(string s) => new string((s ?? "").Where(char.IsDigit).ToArray());

        private static bool ValidarCelularEcuador(string celular)
        {
            var d = SoloDigitos(celular);
            return Regex.IsMatch(d, @"^09\d{8}$");
        }

        // Cédula natural: 10 dígitos, coeficientes 2-1-2-1-2-1-2-1-2 y provincia 01..24
        private static bool ValidarCedulaEcuador(string ci)
        {
            var d = SoloDigitos(ci);
            if (!Regex.IsMatch(d, @"^\d{10}$")) return false;

            int prov = int.Parse(d.Substring(0, 2));
            int tercer = d[2] - '0';
            if (prov < 1 || prov > 24) return false;
            if (tercer >= 6) return false;

            int[] coef = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int val = (d[i] - '0') * coef[i];
                if (val >= 10) val -= 9;
                suma += val;
            }
            int verificador = (10 - (suma % 10)) % 10;
            return verificador == (d[9] - '0');
        }

        private static bool ValidarCorreo(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true; // opcional
            try { var _ = new MailAddress(email); return true; }
            catch { return false; }
        }

        private static char SexoFromComboText(string text)
        {
            var s = (text ?? "").Trim().ToUpperInvariant();
            return (s.StartsWith("H") || s.StartsWith("F")) ? 'F' : 'M'; // Hembra/Femenino -> 'F'
        }

        // Peso (kg): acepta coma/punto y 'kg' opcional al final
        private static bool TryParseKg(string input, out decimal kg)
        {
            kg = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;

            var s = input.Trim();
            // quita "kg" final con o sin espacio (case-insensitive)
            s = Regex.Replace(s, @"\s*kg\s*$", "", RegexOptions.IgnoreCase);

            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out kg)
                || decimal.TryParse(s, NumberStyles.Number, CultureInfo.GetCultureInfo("es-EC"), out kg)
                || decimal.TryParse(s.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out kg);
        }

        private bool Fail(string msg, out string error) { error = msg; return false; }

        private bool ValidarMascotaForm(out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(txtMascotaNombre.Text))
                return Fail("El nombre de la mascota es obligatorio.", out error);

            if (string.IsNullOrWhiteSpace(cmbEspecie.Text))
                return Fail("La especie es obligatoria.", out error);

            // Raza obligatoria
            if (string.IsNullOrWhiteSpace(txtRaza.Text))
                return Fail("La raza es obligatoria. Si no se conoce, escribe 'Mestizo' o 'Desconocida'.", out error);

            if (string.IsNullOrWhiteSpace(cmbSexo.Text))
                return Fail("El sexo es obligatorio.", out error);

            if (!int.TryParse(txtEdad.Text.Trim(), out int edad) || edad <= 0)
                return Fail("La edad debe ser un número mayor a 0.", out error);

            if (string.IsNullOrWhiteSpace(cmbDiscapacidad.Text))
                return Fail("La discapacidad es obligatoria (si no aplica, selecciona 'Ninguna').", out error);

            return true;
        }

        private bool ValidarDuenoForm(out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(txtDuenoNombreCompleto.Text))
                return Fail("El nombre del propietario es obligatorio.", out error);

            if (!ValidarCelularEcuador(txtCelular.Text))
                return Fail("Celular inválido. Formato EC: 09######## (10 dígitos).", out error);

            if (!ValidarCedulaEcuador(txtCedula.Text))
                return Fail("Cédula inválida. Verifica el número.", out error);

            if (!ValidarCorreo(txtCorreo.Text))
                return Fail("Correo inválido. Ejemplo: usuario@dominio.com", out error);

            return true;
        }

        // ====================== NUEVOS helpers de BD ======================

        // Reusa o crea Propietario a partir de la C.I. (devuelve IdPropietario y saca IdPersona por out)
        private int GetOrCreatePropietarioPorCedula(
            SqlConnection cn, SqlTransaction tx,
            string cedula,
            string nombre, string apellido, string celular,
            string correo, string direccion, byte[] imgPersona,
            out int idPersona)
        {
            // 1) Buscar Persona por C.I.
            using (var cmd = new SqlCommand(
                "SELECT IdPersona FROM Persona WHERE Cedula=@ci;", cn, tx))
            {
                cmd.Parameters.AddWithValue("@ci", cedula);
                var obj = cmd.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                {
                    idPersona = Convert.ToInt32(obj);

                    // Actualiza datos de contacto por si cambiaron
                    using (var up = new SqlCommand(@"
                        UPDATE Persona SET
                            Nombre=@n, Apellido=@a, Celular=@cel, Correo=@cor, DireccionDomiciliaria=@dir, Imagen=@img
                        WHERE IdPersona=@id;", cn, tx))
                    {
                        up.Parameters.AddWithValue("@n", nombre);
                        up.Parameters.AddWithValue("@a", string.IsNullOrWhiteSpace(apellido) ? "" : apellido);
                        up.Parameters.AddWithValue("@cel", celular);
                        up.Parameters.AddWithValue("@cor", string.IsNullOrWhiteSpace(correo) ? (object)DBNull.Value : correo);
                        up.Parameters.AddWithValue("@dir", string.IsNullOrWhiteSpace(direccion) ? (object)DBNull.Value : direccion);
                        var pImg = up.Parameters.Add("@img", SqlDbType.VarBinary);
                        pImg.Value = (object)imgPersona ?? DBNull.Value;
                        up.Parameters.AddWithValue("@id", idPersona);
                        up.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Crear nueva Persona
                    using (var ins = new SqlCommand(@"
                        INSERT INTO Persona (Nombre, Apellido, Celular, Cedula, Correo, DireccionDomiciliaria, Imagen)
                        VALUES (@n,@a,@cel,@ci,@cor,@dir,@img);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx))
                    {
                        ins.Parameters.AddWithValue("@n", nombre);
                        ins.Parameters.AddWithValue("@a", string.IsNullOrWhiteSpace(apellido) ? "" : apellido);
                        ins.Parameters.AddWithValue("@cel", celular);
                        ins.Parameters.AddWithValue("@ci", cedula);
                        ins.Parameters.AddWithValue("@cor", string.IsNullOrWhiteSpace(correo) ? (object)DBNull.Value : correo);
                        ins.Parameters.AddWithValue("@dir", string.IsNullOrWhiteSpace(direccion) ? (object)DBNull.Value : direccion);
                        var pImg = ins.Parameters.Add("@img", SqlDbType.VarBinary);
                        pImg.Value = (object)imgPersona ?? DBNull.Value;

                        idPersona = (int)ins.ExecuteScalar();
                    }
                }
            }

            // 2) Propietario para esa persona
            using (var cmd = new SqlCommand(
                "SELECT IdPropietario FROM Propietario WHERE IdPersona=@id;", cn, tx))
            {
                cmd.Parameters.AddWithValue("@id", idPersona);
                var obj = cmd.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                    return Convert.ToInt32(obj);
            }

            using (var ins = new SqlCommand(
                "INSERT INTO Propietario (IdPersona) VALUES (@id); SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx))
            {
                ins.Parameters.AddWithValue("@id", idPersona);
                return (int)ins.ExecuteScalar();
            }
        }

        // ¿Ya existe una mascota con los campos obligatorios para ese propietario?
        private bool ExisteMascotaDuplicada(
            SqlConnection cn, SqlTransaction tx,
            int idPropietario, string nombre, string especie, char sexo,
            object fechaNacimientoDb, string discapacidad)
        {
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 m.IdMascota
                FROM Mascota m
                JOIN PropietarioMascota pm ON pm.IdMascota = m.IdMascota AND pm.IdPropietario = @p
                WHERE UPPER(m.Nombre) = UPPER(@n)
                  AND UPPER(m.Especie) = UPPER(@e)
                  AND m.Sexo = @s
                  AND (
                       (@fn IS NULL AND m.FechaNacimiento IS NULL) OR
                       (m.FechaNacimiento = @fn)
                  )
                  AND (
                       (@dis IS NULL AND m.Discapacidad IS NULL) OR
                       (UPPER(m.Discapacidad) = UPPER(@dis))
                  );", cn, tx))
            {
                cmd.Parameters.AddWithValue("@p", idPropietario);
                cmd.Parameters.AddWithValue("@n", nombre ?? "");
                cmd.Parameters.AddWithValue("@e", especie ?? "");
                cmd.Parameters.AddWithValue("@s", sexo);
                var pFn = cmd.Parameters.Add("@fn", SqlDbType.Date);
                pFn.Value = fechaNacimientoDb ?? DBNull.Value;
                var pDis = cmd.Parameters.Add("@dis", SqlDbType.NVarChar, 200);
                pDis.Value = string.IsNullOrWhiteSpace(discapacidad) ? (object)DBNull.Value : discapacidad;

                var obj = cmd.ExecuteScalar();
                return obj != null && obj != DBNull.Value;
            }
        }

        // Versión para edición: excluye una mascota por Id
        private bool ExisteMascotaDuplicadaExcluyendoId(
            SqlConnection cn, SqlTransaction tx,
            int idMascotaExcluir, int idPropietario, string nombre, string especie, char sexo,
            object fechaNacimientoDb, string discapacidad)
        {
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 m.IdMascota
                FROM Mascota m
                JOIN PropietarioMascota pm ON pm.IdMascota = m.IdMascota AND pm.IdPropietario = @p
                WHERE m.IdMascota <> @ex
                  AND UPPER(m.Nombre) = UPPER(@n)
                  AND UPPER(m.Especie) = UPPER(@e)
                  AND m.Sexo = @s
                  AND (
                       (@fn IS NULL AND m.FechaNacimiento IS NULL) OR
                       (m.FechaNacimiento = @fn)
                  )
                  AND (
                       (@dis IS NULL AND m.Discapacidad IS NULL) OR
                       (UPPER(m.Discapacidad) = UPPER(@dis))
                  );", cn, tx))
            {
                cmd.Parameters.AddWithValue("@ex", idMascotaExcluir);
                cmd.Parameters.AddWithValue("@p", idPropietario);
                cmd.Parameters.AddWithValue("@n", nombre ?? "");
                cmd.Parameters.AddWithValue("@e", especie ?? "");
                cmd.Parameters.AddWithValue("@s", sexo);
                var pFn = cmd.Parameters.Add("@fn", SqlDbType.Date);
                pFn.Value = fechaNacimientoDb ?? DBNull.Value;
                var pDis = cmd.Parameters.Add("@dis", SqlDbType.NVarChar, 200);
                pDis.Value = string.IsNullOrWhiteSpace(discapacidad) ? (object)DBNull.Value : discapacidad;

                var obj = cmd.ExecuteScalar();
                return obj != null && obj != DBNull.Value;
            }
        }

        // ====================== Eventos ======================

        private void AgregarPaciente_Load(object sender, EventArgs e)
        {
            if (IsInDesigner()) return;

            // Unidad de edad
            cmbEdadUnidad.Items.Clear();
            cmbEdadUnidad.Items.AddRange(new[] { "Años", "Meses" });
            cmbEdadUnidad.SelectedIndex = 0;

            // Discapacidad: opciones base
            cmbDiscapacidad.DropDownStyle = ComboBoxStyle.DropDown; // o DropDownList si no quieres que escriban
            if (cmbDiscapacidad.Items.Count == 0)
            {
                cmbDiscapacidad.Items.AddRange(new object[]
                { "Ninguna", "Ceguera", "Sordera", "Amputación", "Parálisis", "Otra" });
                cmbDiscapacidad.SelectedIndex = 0;
            }

            if (_idMascotaEdit.HasValue && _idPropietarioEdit.HasValue)
                CargarParaEditar(_idPropietarioEdit.Value, _idMascotaEdit.Value);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (!ValidarMascotaForm(out var err1)) { MessageBox.Show(err1, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!ValidarDuenoForm(out var err2)) { MessageBox.Show(err2, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            // Si es edición → actualizar
            if (_idMascotaEdit.HasValue && _idPropietarioEdit.HasValue)
            {
                GuardarEdicion(_idPropietarioEdit.Value, _idMascotaEdit.Value);
                return;
            }

            // Edad -> FechaNacimiento (aprox)
            var fnac = FechaNacDesdeEdad(txtEdad.Text, cmbEdadUnidad.Text);
            object fechaNacDb = (object)fnac ?? DBNull.Value;

            // Sexo M/F
            char sexo = SexoFromComboText(cmbSexo.Text);

            // Peso (opcional, en kg)
            object pesoDb = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                if (!TryParseKg(txtPeso.Text, out var kg) || kg < 0 || kg > 2000)
                {
                    MessageBox.Show("Peso inválido. Ingrese kilogramos (ej: 4,2 o 4.2).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                pesoDb = kg;
            }

            // Propietario: separar nombre/apellido (apellido opcional → "")
            string nombre = "", apellido = "";
            var partes = txtDuenoNombreCompleto.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length >= 1) nombre = partes[0];
            if (partes.Length >= 2) apellido = string.Join(" ", partes.Skip(1));

            // Fotos
            var imgPersona = BytesFromPicture(picPropietario);
            var imgMascota = BytesFromPicture(picMascota);

            var cn = _db.AbrirConexion();
            var tx = cn.BeginTransaction();
            try
            {
                // ===== 1) Resolver propietario por C.I. (reusar o crear) =====
                int idPersona;
                int idPropietario = GetOrCreatePropietarioPorCedula(
                    cn, tx,
                    txtCedula.Text.Trim(),
                    nombre, apellido, txtCelular.Text.Trim(),
                    txtCorreo.Text.Trim(), txtDireccion.Text.Trim(), imgPersona,
                    out idPersona);

                // ===== 2) Preparar datos mascota =====
                string discap = (cmbDiscapacidad.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(discap)) discap = null;

                // ===== 3) Chequeo de duplicados (campos obligatorios + propietario) =====
                if (ExisteMascotaDuplicada(cn, tx, idPropietario,
                                           txtMascotaNombre.Text.Trim(),
                                           cmbEspecie.Text.Trim(),
                                           sexo, fechaNacDb, discap))
                {
                    tx.Rollback();
                    MessageBox.Show("Ya existe una mascota con los mismos datos obligatorios para este propietario. " +
                                    "Modifica algún dato (por ejemplo nombre, edad/fecha o discapacidad) para continuar.",
                                    "Registro duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ===== 4) Insertar MASCOTA =====
                int idMascota;
                using (var cmd = new SqlCommand(@"
                    INSERT INTO Mascota (Imagen, Nombre, Especie, Raza, Sexo, PesoKg, FechaNacimiento, Discapacidad)
                    VALUES (@Imagen, @Nombre, @Especie, @Raza, @Sexo, @PesoKg, @FechaNacimiento, @Discapacidad);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx))
                {
                    var pImgM = cmd.Parameters.Add("@Imagen", SqlDbType.VarBinary);
                    pImgM.Value = (object)imgMascota ?? DBNull.Value;

                    cmd.Parameters.AddWithValue("@Nombre", txtMascotaNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Especie", cmbEspecie.Text.Trim());
                    cmd.Parameters.AddWithValue("@Raza", string.IsNullOrWhiteSpace(txtRaza.Text) ? (object)DBNull.Value : txtRaza.Text.Trim());
                    cmd.Parameters.AddWithValue("@Sexo", sexo);
                    cmd.Parameters.AddWithValue("@PesoKg", pesoDb);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacDb);
                    cmd.Parameters.AddWithValue("@Discapacidad", (object)discap ?? DBNull.Value);

                    idMascota = (int)cmd.ExecuteScalar();
                }

                // ===== 5) Enlace Propietario–Mascota =====
                using (var cmd = new SqlCommand(@"
                    INSERT INTO PropietarioMascota (IdPropietario, IdMascota, EsPrincipal)
                    VALUES (@IdPropietario, @IdMascota, 1);", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@IdPropietario", idPropietario);
                    cmd.Parameters.AddWithValue("@IdMascota", idMascota);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                MessageBox.Show("Guardado correctamente ✅", "AniClinic");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException ex) { try { tx.Rollback(); } catch { } MessageBox.Show("Error SQL: " + ex.Message); }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show("Error: " + ex.Message); }
            finally { _db.CerrarConexion(); }
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
                            // ---------- Mascota ----------
                            txtMascotaNombre.Text = rd["MascotaNombre"]?.ToString();
                            cmbEspecie.Text = rd["Especie"]?.ToString();
                            txtRaza.Text = rd["Raza"] as string;

                            // sexo: en BD guardas 'M'/'F' → mostrar "Macho"/"Hembra"
                            var sexoDb = rd["Sexo"]?.ToString();
                            cmbSexo.Text = (sexoDb == "F") ? "Hembra" : "Macho";

                            // peso (formateado)
                            txtPeso.Text = rd["PesoKg"] == DBNull.Value
                                ? ""
                                : Convert.ToDecimal(rd["PesoKg"]).ToString("0.###", CultureInfo.CurrentCulture);

                            // discapacidad
                            var discDb = rd["Discapacidad"] as string;
                            if (!string.IsNullOrWhiteSpace(discDb))
                            {
                                if (!cmbDiscapacidad.Items.Cast<object>()
                                      .Any(i => string.Equals(i.ToString(), discDb, StringComparison.OrdinalIgnoreCase)))
                                    cmbDiscapacidad.Items.Add(discDb);
                                cmbDiscapacidad.SelectedItem = discDb;
                            }
                            else if (cmbDiscapacidad.Items.Count > 0)
                            {
                                cmbDiscapacidad.SelectedIndex = 0; // "Ninguna"
                            }

                            // edad desde FechaNacimiento → meses (<24) o años (≥24)
                            if (rd["FechaNacimiento"] != DBNull.Value)
                            {
                                var fn = Convert.ToDateTime(rd["FechaNacimiento"]);
                                var hoy = DateTime.Today;

                                int totalMeses = (hoy.Year - fn.Year) * 12 + (hoy.Month - fn.Month);
                                if (hoy.Day < fn.Day) totalMeses -= 1;

                                if (totalMeses >= 0 && totalMeses < 24)
                                {
                                    txtEdad.Text = totalMeses.ToString();
                                    cmbEdadUnidad.Text = "Meses";
                                }
                                else
                                {
                                    int años = hoy.Year - fn.Year - (fn > hoy.AddYears(-(hoy.Year - fn.Year)) ? 1 : 0);
                                    txtEdad.Text = Math.Max(años, 0).ToString();
                                    cmbEdadUnidad.Text = "Años";
                                }
                            }
                            else
                            {
                                txtEdad.Text = "0";
                                cmbEdadUnidad.Text = "Años";
                            }

                            // imagen mascota
                            if (rd["ImgMascota"] != DBNull.Value)
                            {
                                var bytes = (byte[])rd["ImgMascota"];
                                using (var ms = new MemoryStream(bytes))
                                    picMascota.Image = Image.FromStream(ms);
                            }

                            // ---------- Propietario ----------
                            string nom = LimpiarSn(rd["Nombre"]?.ToString());
                            string ape = LimpiarSn(rd["Apellido"]?.ToString());
                            txtDuenoNombreCompleto.Text = string.IsNullOrEmpty(ape) ? nom : (nom + " " + ape);

                            txtCelular.Text = rd["Celular"] as string;
                            txtCedula.Text = rd["Cedula"] as string;
                            txtCorreo.Text = rd["Correo"] as string;
                            txtDireccion.Text = rd["DireccionDomiciliaria"] as string;

                            if (rd["ImgPersona"] != DBNull.Value)
                            {
                                var bytes = (byte[])rd["ImgPersona"];
                                using (var ms = new MemoryStream(bytes))
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
            if (!ValidarMascotaForm(out var err1)) { MessageBox.Show(err1, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!ValidarDuenoForm(out var err2)) { MessageBox.Show(err2, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            char sexo = SexoFromComboText(cmbSexo.Text);

            // Peso (opcional, en kg)
            object pesoDb = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                if (!TryParseKg(txtPeso.Text, out var kg) || kg < 0 || kg > 2000)
                {
                    MessageBox.Show("Peso inválido. Ingrese kilogramos (ej: 4,2 o 4.2).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                pesoDb = kg;
            }

            object fechaNacDb = DBNull.Value;
            var fn = FechaNacDesdeEdad(txtEdad.Text, cmbEdadUnidad.Text);
            if (fn.HasValue) fechaNacDb = fn.Value;

            // separar nombre/apellido (sin "(s/n)")
            string nombre = "", apellido = "";
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

                // Verificar que la C.I. no esté usada por otra persona
                using (var chk = new SqlCommand(@"
                    SELECT COUNT(*) FROM Persona
                    WHERE Cedula=@ci AND IdPersona<>@id;", cn, tx))
                {
                    chk.Parameters.AddWithValue("@ci", txtCedula.Text.Trim());
                    chk.Parameters.AddWithValue("@id", idPersona);
                    int repetida = (int)chk.ExecuteScalar();
                    if (repetida > 0)
                    {
                        MessageBox.Show("La C.I. ingresada ya está asociada a otra persona. No se puede actualizar.",
                            "Cédula duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx.Rollback();
                        return;
                    }
                }

                // Persona (UPDATE)
                using (var cmd = new SqlCommand(@"
                    UPDATE Persona SET
                        Nombre=@Nombre, Apellido=@Apellido, Celular=@Celular, Cedula=@Cedula,
                        Correo=@Correo, DireccionDomiciliaria=@Dir, Imagen=@Imagen
                    WHERE IdPersona=@Id;", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Apellido", string.IsNullOrWhiteSpace(apellido) ? "" : apellido);
                    cmd.Parameters.AddWithValue("@Celular", txtCelular.Text.Trim());
                    cmd.Parameters.AddWithValue("@Cedula", string.IsNullOrWhiteSpace(txtCedula.Text) ? (object)DBNull.Value : txtCedula.Text.Trim());
                    cmd.Parameters.AddWithValue("@Correo", string.IsNullOrWhiteSpace(txtCorreo.Text) ? (object)DBNull.Value : txtCorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Dir", string.IsNullOrWhiteSpace(txtDireccion.Text) ? (object)DBNull.Value : txtDireccion.Text.Trim());
                    var pImg = cmd.Parameters.Add("@Imagen", SqlDbType.VarBinary);
                    pImg.Value = (object)imgPersona ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@Id", idPersona);
                    cmd.ExecuteNonQuery();
                }

                // Chequeo duplicado de mascota (excluyendo la actual)
                string discap = (cmbDiscapacidad.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(discap)) discap = null;

                if (ExisteMascotaDuplicadaExcluyendoId(cn, tx, idMascota, idPropietario,
                                                       txtMascotaNombre.Text.Trim(),
                                                       cmbEspecie.Text.Trim(),
                                                       sexo, fechaNacDb, discap))
                {
                    tx.Rollback();
                    MessageBox.Show("Ya existe otra mascota del mismo propietario con los mismos datos obligatorios.",
                        "Registro duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mascota (UPDATE)
                using (var cmd = new SqlCommand(@"
                    UPDATE Mascota SET
                        Imagen=@Imagen, Nombre=@Nombre, Especie=@Especie, Raza=@Raza, Sexo=@Sexo,
                        PesoKg=@PesoKg, FechaNacimiento=@FechaNacimiento, Discapacidad=@Discapacidad
                    WHERE IdMascota=@Id;", cn, tx))
                {
                    var pImgM = cmd.Parameters.Add("@Imagen", SqlDbType.VarBinary);
                    pImgM.Value = (object)imgMascota ?? DBNull.Value;

                    cmd.Parameters.AddWithValue("@Nombre", txtMascotaNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Especie", cmbEspecie.Text.Trim());
                    cmd.Parameters.AddWithValue("@Raza", string.IsNullOrWhiteSpace(txtRaza.Text) ? (object)DBNull.Value : txtRaza.Text.Trim());
                    cmd.Parameters.AddWithValue("@Sexo", sexo);
                    cmd.Parameters.AddWithValue("@PesoKg", pesoDb);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacDb);
                    cmd.Parameters.AddWithValue("@Discapacidad", (object)discap ?? DBNull.Value);
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

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {
            // no-op
        }
    }
}
