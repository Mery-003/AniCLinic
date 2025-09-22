using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggMascota : Form
    {
        private readonly csCRUD _crud = new csCRUD();
        private readonly fPacientes _parent;
        private readonly int _idMascota;
        private int _idPersonaSeleccionada;

        public AggMascota(fPacientes parent, int idMascota = 0)
        {
            InitializeComponent();
            _parent = parent;
            _idMascota = idMascota;

            btnAbrirListaPropietario.Click += btnAbrirListaPropietario_Click;
            btnSeleccionarFotoMascota.Click += btnSeleccionarFotoMascota_Click;
            btnGuardarMascota.Click += btnGuardarMascota_Click;
            btnCancelarMascota.Click += (s, e) => this.Close();
            
            txtPeso.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
                    e.Handled = true;
                Control tb = s as Control;
                if (tb != null && (e.KeyChar == '.' || e.KeyChar == ',') && tb.Text.IndexOfAny(new[] { '.', ',' }) > -1)
                    e.Handled = true;
            };

            CargarCombos();
            if (_idMascota > 0) CargarMascota(_idMascota);
            else LimpiarNuevo();
        }

        private void LimpiarNuevo()
        {
            _idPersonaSeleccionada = 0;
            txtPropietarioNombre.Text = "";
            txtPropietarioCI.Text = "";
            txtMascotaNombre.Text = "";
            txtPeso.Text = "";
            picMascota.Image = null;

            cmbSexo.Items.Clear(); cmbSexo.Items.Add("Macho"); cmbSexo.Items.Add("Hembra"); cmbSexo.SelectedIndex = -1;
            if (cmbDiscapacidad.Items.Count == 0)
            {
                cmbDiscapacidad.Items.Add("Ninguna");
                cmbDiscapacidad.Items.Add("Visual");
                cmbDiscapacidad.Items.Add("Auditiva");
                cmbDiscapacidad.Items.Add("Motora");
                cmbDiscapacidad.Items.Add("Cognitiva");
            }
            cmbDiscapacidad.SelectedIndex = 0;

            cmbEspecie.SelectedIndex = -1;
            cmbRaza.DataSource = CrearTablaPH("IdRaza");
            cmbRaza.DisplayMember = "Texto";
            cmbRaza.ValueMember = "IdRaza";
            cmbRaza.SelectedIndex = -1;

            dtpFechaNacimiento.Value = DateTime.Today;
        }

        private static DataTable CrearTablaPH(string id)
        {
            DataTable t = new DataTable();
            t.Columns.Add(id, typeof(int));
            t.Columns.Add("Texto", typeof(string));
            DataRow r = t.NewRow();
            r[id] = DBNull.Value;
            r["Texto"] = "-- Seleccione --";
            t.Rows.Add(r);
            return t;
        }

        // detectar columna de texto en Especie/Raza
        private string ColTextoTabla(string tabla)
        {
            DataTable dt = _crud.cargarBDData(
                "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@t",
                new SqlParameter("@t", tabla));
            if (dt == null) return null;

            string[] pref = { "Nombre", "Descripcion", "NombreEspecie", "NombreRaza" };
            for (int i = 0; i < pref.Length; i++) if (dt.Select("COLUMN_NAME='" + pref[i] + "'").Length > 0) return pref[i];

            foreach (DataRow r in dt.Rows)
            {
                string tipo = (r["DATA_TYPE"] + "").ToLower();
                if (tipo.Contains("char") || tipo.Contains("text")) return r["COLUMN_NAME"] + "";
            }
            return null;
        }

        private void CargarCombos()
        {
            // Especie
            using (SqlDataReader dr = _crud.EjecutarQuery("SELECT IdEspecie, Especie FROM Especie ORDER BY Especie"))
            {
                DataTable dt = new DataTable();
                if (dr != null) dt.Load(dr);
                if (dr != null) dr.Close(); try { _crud.conexion.cerrarConexion(); } catch { }

                cmbEspecie.DisplayMember = "Especie";   // Mostrar nombre
                cmbEspecie.ValueMember = "IdEspecie";   // Guardar Id
                cmbEspecie.DataSource = dt;
            }

            // Inicializar combo raza vacío
            cmbRaza.DataSource = CrearTablaPH("IdRaza");
            cmbRaza.DisplayMember = "Raza";
            cmbRaza.ValueMember = "IdRaza";

            cmbEspecie.SelectedIndexChanged += (s, e) => CargarRazas();
        }

        private void CargarRazas()
        {
            if (cmbEspecie.SelectedIndex < 0 || cmbEspecie.SelectedValue == null || cmbEspecie.SelectedValue == DBNull.Value)
            {
                cmbRaza.DataSource = CrearTablaPH("IdRaza");
                cmbRaza.SelectedIndex = -1;
                return;
            }

            int idEsp = Convert.ToInt32(cmbEspecie.SelectedValue);

            using (SqlDataReader dr = _crud.EjecutarQuery("SELECT IdRaza, Raza FROM Raza WHERE IdEspecie=" + idEsp + " ORDER BY Raza"))
            {
                DataTable dt = new DataTable();
                if (dr != null) dt.Load(dr);
                if (dr != null) dr.Close(); try { _crud.conexion.cerrarConexion(); } catch { }

                cmbRaza.DisplayMember = "Raza";   // Mostrar nombre
                cmbRaza.ValueMember = "IdRaza";   // Guardar Id
                cmbRaza.DataSource = dt;
            }
        }

        private void CargarMascota(int id)
        {
            string sql = @"
SELECT m.IdMascota, m.IdPersona, m.Imagen, m.Nombre, m.Sexo, m.PesoKg, m.Discapacidad,
       e. IdEspecie, e.Especie, r.IdRaza, r.Raza, m.FechaNacimiento,
       p.Nombre AS PNombre, p.Apellido AS PApellido, p.Cedula AS PCedula
FROM Mascota m
INNER JOIN Persona p ON p.IdPersona = m.IdPersona
INNER JOIN Especie e ON e.IdEspecie = m.IdEspecie 
INNER JOIN Raza r ON r.IdRaza = m.IdRaza
WHERE m.IdMascota = " + id;

            using (SqlDataReader rd = _crud.EjecutarQuery(sql))
            {
                if (rd != null && rd.Read())
                {
                    _idPersonaSeleccionada = Convert.ToInt32(rd["IdPersona"]);
                    txtPropietarioNombre.Text = (rd["PNombre"] + " " + rd["PApellido"]).Trim();
                    txtPropietarioCI.Text = rd["PCedula"] + "";

                    txtMascotaNombre.Text = rd["Nombre"] + "";
                    cmbSexo.Items.Clear(); cmbSexo.Items.Add("Macho"); cmbSexo.Items.Add("Hembra");
                    cmbSexo.Text = rd["Sexo"] + "";
                    txtPeso.Text = rd["PesoKg"] == DBNull.Value ? "" : Convert.ToDecimal(rd["PesoKg"]).ToString(CultureInfo.CurrentCulture);

                    if (cmbDiscapacidad.Items.Count == 0)
                    {
                        cmbDiscapacidad.Items.Add("Ninguna");
                        cmbDiscapacidad.Items.Add("Visual");
                        cmbDiscapacidad.Items.Add("Auditiva");
                        cmbDiscapacidad.Items.Add("Motora");
                        cmbDiscapacidad.Items.Add("Cognitiva");
                    }
                    string discapacidad = rd["Discapacidad"] == DBNull.Value ? "Ninguna" : rd["Discapacidad"].ToString();

                    if (cmbDiscapacidad.Items.Contains(discapacidad))
                        cmbDiscapacidad.SelectedItem = discapacidad;
                    else
                        cmbDiscapacidad.SelectedItem = "Ninguna";

                    if (rd["FechaNacimiento"] != DBNull.Value)
                        dtpFechaNacimiento.Value = Convert.ToDateTime(rd["FechaNacimiento"]);

                    if (rd["IdEspecie"] != DBNull.Value)
                    {
                        cmbEspecie.SelectedValue = rd["IdEspecie"]; 
                        CargarRazas();

                        if (rd["IdRaza"] != DBNull.Value)
                            cmbRaza.SelectedValue = rd["IdRaza"];
                    }

                    if (rd["Imagen"] != DBNull.Value)
                    {
                        try { using (var ms = new MemoryStream((byte[])rd["Imagen"])) { picMascota.Image = Image.FromStream(ms); picMascota.SizeMode = PictureBoxSizeMode.StretchImage; } }
                        catch { picMascota.Image = null; }
                    }
                    else picMascota.Image = null;
                }
                else MessageBox.Show("No se encontró la mascota.");
            }
        }

        private void btnAbrirListaPropietario_Click(object sender, EventArgs e)
        {
            using (var frm = new FMRListaPropietario())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    _idPersonaSeleccionada = frm.IdPersonaSel;
                    txtPropietarioNombre.Text = (frm.NombreSel + " " + frm.ApellidoSel).Trim();
                    txtPropietarioCI.Text = frm.CedulaSel;
                }
            }
        }

        private void btnSeleccionarFotoMascota_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try { picMascota.Image = Image.FromFile(ofd.FileName); picMascota.SizeMode = PictureBoxSizeMode.StretchImage; }
                    catch { MessageBox.Show("No se pudo cargar la imagen."); }
                }
            }
        }

        private static byte[] ImgToBytes(Image img)
        {
            if (img == null) return null;
            using (var ms = new MemoryStream()) { img.Save(ms, img.RawFormat); return ms.ToArray(); }
        }

        private bool Validar()
        {
            if (_idPersonaSeleccionada <= 0) { MessageBox.Show("Seleccione un propietario."); return false; }
            if (string.IsNullOrWhiteSpace(txtMascotaNombre.Text)) { MessageBox.Show("Ingrese el nombre de la mascota."); return false; }
            int tmp;
            if (cmbEspecie.SelectedIndex < 0 || cmbEspecie.SelectedValue == null || cmbEspecie.SelectedValue == DBNull.Value ||
                !int.TryParse(cmbEspecie.SelectedValue.ToString(), out tmp)) { MessageBox.Show("Seleccione la especie."); return false; }
            if (cmbRaza.SelectedIndex < 0 || cmbRaza.SelectedValue == null || cmbRaza.SelectedValue == DBNull.Value ||
                !int.TryParse(cmbRaza.SelectedValue.ToString(), out tmp)) { MessageBox.Show("Seleccione la raza."); return false; }
            if (cmbSexo.SelectedIndex < 0) { MessageBox.Show("Seleccione el sexo."); return false; }
            if (!string.IsNullOrWhiteSpace(txtPeso.Text))
            {
                decimal d;
                if (!decimal.TryParse(txtPeso.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                { MessageBox.Show("Peso inválido."); return false; }
            }
            return true;
        }

        private void btnGuardarMascota_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;

            int idEsp = Convert.ToInt32(cmbEspecie.SelectedValue);
            int idRaza = Convert.ToInt32(cmbRaza.SelectedValue);
            string nombre = txtMascotaNombre.Text.Trim();
            string sexo = cmbSexo.Text.Trim();
            decimal? peso = string.IsNullOrWhiteSpace(txtPeso.Text) ? (decimal?)null
                : Convert.ToDecimal(txtPeso.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            string disc = (cmbDiscapacidad.Text ?? "Ninguna");
            DateTime fnac = dtpFechaNacimiento.Value.Date;
            SqlParameter pImg = new SqlParameter("@Imagen", SqlDbType.VarBinary) { Value = (object)ImgToBytes(picMascota.Image) ?? DBNull.Value };

            if (_idMascota > 0)
            {
                string up = @"UPDATE Mascota SET
                              IdPersona=@per, Imagen=@Imagen, Nombre=@n, Sexo=@s, PesoKg=@p,
                              Discapacidad=@d, IdEspecie=@e, IdRaza=@r, FechaNacimiento=@f
                              WHERE IdMascota=@id;";
                bool ok = _crud.editarBD(up,
                    new SqlParameter("@per", _idPersonaSeleccionada), pImg,
                    new SqlParameter("@n", nombre), new SqlParameter("@s", sexo),
                    new SqlParameter("@p", (object)peso ?? DBNull.Value),
                    new SqlParameter("@d", disc),
                    new SqlParameter("@e", idEsp), new SqlParameter("@r", idRaza),
                    new SqlParameter("@f", fnac),
                    new SqlParameter("@id", _idMascota));
                if (!ok) { MessageBox.Show("No se pudo actualizar."); return; }
                MessageBox.Show("Mascota actualizada.");
            }
            else
            {
                string ins = @"INSERT INTO Mascota
                               (IdPersona, Imagen, Nombre, Sexo, PesoKg, Discapacidad, IdEspecie, IdRaza, FechaNacimiento)
                               VALUES (@per, @Imagen, @n, @s, @p, @d, @e, @r, @f);";
                bool ok = _crud.agregarBD(ins,
                    new SqlParameter("@per", _idPersonaSeleccionada), pImg,
                    new SqlParameter("@n", nombre), new SqlParameter("@s", sexo),
                    new SqlParameter("@p", (object)peso ?? DBNull.Value),
                    new SqlParameter("@d", disc),
                    new SqlParameter("@e", idEsp), new SqlParameter("@r", idRaza),
                    new SqlParameter("@f", fnac));
                if (!ok) { MessageBox.Show("No se pudo registrar."); return; }
                MessageBox.Show("Mascota registrada.");
            }

            try 
            { 
                _parent.RefrescarMascotas();
                _parent.RefrescarPropietarios();
            } catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message);
            }
            this.Close();
        }
    }
}
