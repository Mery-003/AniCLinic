using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fRegistroClinico : Form
    {
        private DataTable _dtHoy, _dtProximas, _dtAnteriores;

        public fRegistroClinico()
        {
            InitializeComponent();

            // SOLO CellContentClick (evita doble apertura)
            try { dgvHoy.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvProximas.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvAnteriores.CellContentClick -= Grid_ButtonClick; } catch { }

            PrepararGrid_Hoy(dgvHoy);
            PrepararGrid_Proximas(dgvProximas);
            PrepararGrid_Anteriores(dgvAnteriores);

            dgvHoy.CellContentClick += Grid_ButtonClick;
            dgvProximas.CellContentClick += Grid_ButtonClick;
            dgvAnteriores.CellContentClick += Grid_ButtonClick;

            // Post-bind
            dgvHoy.DataBindingComplete += (s, e) => { QuitarFilaNueva(dgvHoy); };
            dgvProximas.DataBindingComplete += (s, e) => { QuitarFilaNueva(dgvProximas); };
            dgvAnteriores.DataBindingComplete += (s, e) => { QuitarFilaNueva(dgvAnteriores); DecorarAnterioresSegunRegistro(); };

            WireBusquedas();
            RecargarTodo();
        }

        // ==================== Preparación de grillas ====================
        private void PrepararBase(DataGridView grid)
        {
            grid.DataSource = null;
            grid.AutoGenerateColumns = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            grid.ReadOnly = true;
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.AllowUserToAddRows = false; // sin fila vacía

            grid.RowHeadersVisible = false;
            grid.Columns.Clear();
        }

        private DataGridViewTextBoxColumn MkText(string header, string prop, int width)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = prop,
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = true
            };
        }

        private DataGridViewTextBoxColumn MkHidden(string prop)
        {
            var c = MkText("", prop, 2);
            c.Visible = false;
            return c;
        }

        private static DataGridViewButtonColumn MkBtn(string name, string text, int width = 110)
        {
            return new DataGridViewButtonColumn
            {
                Name = name,
                HeaderText = "",
                Text = text,
                UseColumnTextForButtonValue = true,
                Width = width
            };
        }

        private void PrepararGrid_Hoy(DataGridView grid)
        {
            PrepararBase(grid);
            grid.Columns.Add(MkHidden("IdMascota"));
            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            // HOY NO muestra Estado (lo quitamos del DataTable)
            grid.Columns.Add(MkBtn("colRegistrar", "Registrar", 110));
        }

        private void PrepararGrid_Proximas(DataGridView grid)
        {
            PrepararBase(grid);
            grid.Columns.Add(MkHidden("IdMascota"));
            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            // PRÓXIMAS sí muestra Estado
            grid.Columns.Add(MkText("Estado", "Estado", 100));
        }

        private void PrepararGrid_Anteriores(DataGridView grid)
        {
            PrepararBase(grid);
            grid.Columns.Add(MkHidden("IdMascota"));
            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            // ANTERIORES NO muestra Estado (lo quitamos del DataTable)
            grid.Columns.Add(MkBtn("colEditar", "Editar", 95));
            grid.Columns.Add(MkBtn("colEliminar", "Eliminar", 95));
        }

        private void QuitarFilaNueva(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        // ==================== Carga y separación ====================
        private void RecargarTodo()
        {
            RecargarColeccionesDesdeBD();

            // Búsqueda activa (estilo fCitas)
            AplicarBusquedaTipoCitas(dgvHoy, _dtHoy, txtBuscarHoy == null ? null : txtBuscarHoy.Text);
            AplicarBusquedaTipoCitas(dgvProximas, _dtProximas, txtBuscarProximas == null ? null : txtBuscarProximas.Text);
            AplicarBusquedaTipoCitas(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores == null ? null : txtBuscarAnteriores.Text);
        }

        private void RecargarColeccionesDesdeBD()
        {
            var all = CedulaUtils.CitasListado();

            // Agrega Estado (solo se usará en Próximas)
            if (!all.Columns.Contains("Estado"))
                all.Columns.Add("Estado", typeof(string));

            var hoyDate = DateTime.Today;

            _dtHoy = all.Clone();
            _dtProximas = all.Clone();
            _dtAnteriores = all.Clone();

            foreach (DataRow r in all.Rows)
            {
                DateTime f;
                if (!TryParseFecha(r, out f)) continue;
                var d = f.Date;

                if (d > hoyDate) r["Estado"] = "Próximo";

                int idMascota = ToIntSafe(r, "IdMascota");
                bool tieneReg = RC_ExistePara(idMascota, d);

                if (d == hoyDate)
                {
                    if (tieneReg) _dtAnteriores.Rows.Add((object[])r.ItemArray.Clone());
                    else _dtHoy.Rows.Add((object[])r.ItemArray.Clone());
                }
                else if (d > hoyDate)
                {
                    _dtProximas.Rows.Add((object[])r.ItemArray.Clone());
                }
                else
                {
                    _dtAnteriores.Rows.Add((object[])r.ItemArray.Clone());
                }
            }

            // Quitar 'Estado' en DATA para Hoy y Anteriores (no en el grid)
            if (_dtHoy.Columns.Contains("Estado")) _dtHoy.Columns.Remove("Estado");
            if (_dtAnteriores.Columns.Contains("Estado")) _dtAnteriores.Columns.Remove("Estado");

            // Bind
            dgvHoy.DataSource = _dtHoy;
            dgvProximas.DataSource = _dtProximas;
            dgvAnteriores.DataSource = _dtAnteriores;

            // Decorado inicial de Anteriores
            DecorarAnterioresSegunRegistro();
        }

        private void LimpiarFilasVacias(DataTable dt)
        {
            if (dt == null) return;
            var borrar = dt.AsEnumerable()
                .Where(r =>
                {
                    int id = 0; int.TryParse(Convert.ToString(r["IdCita"]), out id);
                    var masc = Convert.ToString(r.Table.Columns.Contains("Mascota") ? r["Mascota"] : null);
                    return id <= 0 && string.IsNullOrWhiteSpace(masc);
                }).ToList();
            foreach (var r in borrar) dt.Rows.Remove(r);
            dt.AcceptChanges();
        }

        // ==================== Búsquedas ====================
        private void WireBusquedas()
        {
            if (txtBuscarHoy != null)
            {
                txtBuscarHoy.TextChanged -= (s, e) => AplicarBusquedaTipoCitas(dgvHoy, _dtHoy, txtBuscarHoy.Text);
                txtBuscarHoy.TextChanged += (s, e) => AplicarBusquedaTipoCitas(dgvHoy, _dtHoy, txtBuscarHoy.Text);
            }
            if (txtBuscarProximas != null)
            {
                txtBuscarProximas.TextChanged -= (s, e) => AplicarBusquedaTipoCitas(dgvProximas, _dtProximas, txtBuscarProximas.Text);
                txtBuscarProximas.TextChanged += (s, e) => AplicarBusquedaTipoCitas(dgvProximas, _dtProximas, txtBuscarProximas.Text);
            }
            if (txtBuscarAnteriores != null)
            {
                txtBuscarAnteriores.TextChanged -= (s, e) => AplicarBusquedaTipoCitas(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores.Text);
                txtBuscarAnteriores.TextChanged += (s, e) => AplicarBusquedaTipoCitas(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores.Text);
            }
        }

        // ==== NUEVO: búsqueda estilo fCitas (Cedula prefijo; Propietario/Mascota contiene) ====
        private void AplicarBusquedaTipoCitas(DataGridView grid, DataTable baseTable, string term)
        {
            if (grid == null || baseTable == null) return;

            var t = (term ?? string.Empty).Trim();
            if (t.Length == 0) { grid.DataSource = baseTable; return; }

            string[] camposPreferidos = { "CedulaPropietario", "Propietario", "Mascota" };
            var cols = camposPreferidos.Where(c => baseTable.Columns.Contains(c)).ToArray();
            if (cols.Length == 0) { grid.DataSource = baseTable; return; }

            var val = t.Replace("'", "''");

            var condiciones = cols.Select(c =>
            {
                bool esNumero = baseTable.Columns[c].DataType == typeof(int)
                             || baseTable.Columns[c].DataType == typeof(long)
                             || baseTable.Columns[c].DataType == typeof(decimal)
                             || baseTable.Columns[c].DataType == typeof(double);

                // Cedula = prefijo | Propietario/Mascota = contiene
                string patron = c.Equals("CedulaPropietario", StringComparison.OrdinalIgnoreCase)
                                ? $"{val}%"
                                : $"%{val}%";

                return esNumero
                    ? $"CONVERT([{c}], 'System.String') LIKE '{patron}'"
                    : $"([{c}] LIKE '{patron}')";
            });

            grid.DataSource = new DataView(baseTable) { RowFilter = string.Join(" OR ", condiciones) };
        }

        // (Conservo tu método por si lo usas luego; ya no se invoca para los buscadores)
        private void AplicarBusqueda(DataGridView grid, DataTable baseTable, string term)
        {
            if (grid == null || baseTable == null) return;
            var t = (term ?? "").Trim();
            if (t.Length == 0) { grid.DataSource = baseTable; return; }

            string[] campos = { "IdCita", "Mascota", "Especie", "Raza", "Fecha", "Hora", "Motivo", "Propietario", "Estado", "CedulaPropietario" };
            var cols = campos.Where(c => baseTable.Columns.Contains(c)).ToArray();
            var val = t.Replace("'", "''");
            var expr = string.Join(" OR ", cols.Select(c =>
            {
                var col = baseTable.Columns[c];
                bool num = col.DataType == typeof(int) || col.DataType == typeof(decimal) || col.DataType == typeof(double);
                return num ? string.Format("CONVERT([{0}], 'System.String') LIKE '%{1}%'", c, val)
                           : string.Format("([{0}] LIKE '%{1}%')", c, val);
            }).ToArray());
            grid.DataSource = new DataView(baseTable) { RowFilter = expr };
        }

        // ==================== Clicks de botones ====================
        // ==================== Clicks de botones ====================
        private void Grid_ButtonClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = (DataGridView)sender;

            // <<< IMPORTANTE: validar el TIPO DE LA CELDA (no de la columna) >>>
            var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (!(cell is DataGridViewButtonCell)) return;   // C# 7.3

            string colName = grid.Columns[e.ColumnIndex].Name;

            var info = ObtenerCitaInfoDesdeFila(grid, e.RowIndex);
            if (info == null) return;

            if (grid == dgvHoy && colName == "colRegistrar")
            {
                using (var frm = new AggRegistroClinico(info, false))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK) RecargarTodo();
                }
                return;
            }

            if (grid == dgvAnteriores && colName == "colEditar")
            {
                using (var frm = new AggRegistroClinico(info, true))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK) RecargarTodo();
                }
                return;
            }

            if (grid == dgvAnteriores && colName == "colEliminar")
            {
                var ok = MessageBox.Show("¿Eliminar el registro clínico de esta cita?",
                                         "Confirmar eliminación",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ok == DialogResult.Yes)
                {
                    try
                    {
                        RC_DeleteByMascotaFechaOCita(
                            info.IdCita > 0 ? (int?)info.IdCita : null,
                            info.IdMascota,
                            info.FechaHora);
                        RecargarTodo();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo eliminar: " + ex.Message,
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



        // ==================== Decorado de ANTERIORES ====================
        private void DecorarAnterioresSegunRegistro()
        {
            if (dgvAnteriores.Rows.Count == 0) return;

            foreach (DataGridViewRow row in dgvAnteriores.Rows)
            {
                if (row.IsNewRow) continue;

                DateTime fecha = ParseDateFromRow(dgvAnteriores, row.Index);
                int idMascota = 0; int.TryParse(Convert.ToString(row.Cells["IdMascota"].Value), out idMascota);
                bool tiene = RC_ExistePara(idMascota, fecha);

                // ===== Editar =====
                if (row.Cells["colEditar"] is DataGridViewButtonCell || row.Cells["colEditar"] is DataGridViewTextBoxCell)
                {
                    if (!tiene)
                    {
                        // Mostrar texto "Sin registro" (no clickeable)
                        var txtCell = new DataGridViewTextBoxCell { Value = "Sin registro" };
                        row.Cells["colEditar"] = txtCell;
                        row.Cells["colEditar"].ReadOnly = true;
                        row.Cells["colEditar"].Style.ForeColor = Color.DimGray;
                        row.Cells["colEditar"].Style.BackColor = Color.Gainsboro;
                    }
                    else
                    {
                        if (!(row.Cells["colEditar"] is DataGridViewButtonCell))
                            row.Cells["colEditar"] = new DataGridViewButtonCell();

                        row.Cells["colEditar"].ReadOnly = false;
                        row.Cells["colEditar"].Value = "Editar";
                        row.Cells["colEditar"].Style.BackColor = dgvAnteriores.DefaultCellStyle.BackColor;
                        row.Cells["colEditar"].Style.ForeColor = dgvAnteriores.DefaultCellStyle.ForeColor;
                    }
                }

                // ===== Eliminar (SIEMPRE botón) =====
                if (!(row.Cells["colEliminar"] is DataGridViewButtonCell))
                    row.Cells["colEliminar"] = new DataGridViewButtonCell();

                row.Cells["colEliminar"].ReadOnly = false;
                row.Cells["colEliminar"].Value = "Eliminar";
                row.Cells["colEliminar"].Style.BackColor = dgvAnteriores.DefaultCellStyle.BackColor;
                row.Cells["colEliminar"].Style.ForeColor = dgvAnteriores.DefaultCellStyle.ForeColor;
            }
        }

        // ==================== CRUD Registro Clínico (adentro de esta clase) ====================

        // LECTURA
        private DataRow RC_GetByCita(int idCita)
        {
            if (idCita <= 0) return null;
            const string sql = @"
SELECT TOP(1) *
FROM RegistroClinico
WHERE IdCita = @c
ORDER BY FechaRegistro DESC, IdRegistroClinico DESC;";
            return RC_LoadSingle(sql, delegate (SqlCommand cmd) {
                cmd.Parameters.Add("@c", SqlDbType.Int).Value = idCita;
            });
        }

        private DataRow RC_GetByMascotaFecha(int idMascota, DateTime fecha)
        {
            if (idMascota <= 0) return null;
            const string sql = @"
SELECT TOP(1) *
FROM RegistroClinico
WHERE IdMascota = @m AND CONVERT(date, FechaRegistro) = @f
ORDER BY FechaRegistro DESC, IdRegistroClinico DESC;";
            return RC_LoadSingle(sql, delegate (SqlCommand cmd) {
                cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                cmd.Parameters.Add("@f", SqlDbType.Date).Value = fecha.Date;
            });
        }

        private DataRow RC_LoadSingle(string sql, Action<SqlCommand> addParams)
        {
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    addParams(cmd);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
            finally { db.cerrarConexion(); }
        }

        // CREAR
        // Ajusta columnas a tu tabla real (Diagnostico, Tratamiento, Peso, etc.)
        private int RC_Create(int? idCita, int idMascota, DateTime fechaRegistro, string motivo, string observacion)
        {
            const string sql = @"
INSERT INTO RegistroClinico (IdCita, IdMascota, FechaRegistro, Motivo, Observacion)
VALUES (@c, @m, @f, @mot, @obs);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@c", SqlDbType.Int).Value = idCita.HasValue ? (object)idCita.Value : DBNull.Value;
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                    cmd.Parameters.Add("@f", SqlDbType.DateTime).Value = fechaRegistro;
                    cmd.Parameters.Add("@mot", SqlDbType.NVarChar, 200).Value = (motivo ?? "").Trim();
                    cmd.Parameters.Add("@obs", SqlDbType.NVarChar, -1).Value = (observacion ?? "").Trim();
                    return (int)cmd.ExecuteScalar();
                }
            }
            finally { db.cerrarConexion(); }
        }

        // ACTUALIZAR
        private void RC_UpdateById(int idRegistroClinico, string motivo, string observacion)
        {
            const string sql = @"
UPDATE RegistroClinico
SET Motivo = @mot, Observacion = @obs
WHERE IdRegistroClinico = @id;";

            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idRegistroClinico;
                    cmd.Parameters.Add("@mot", SqlDbType.NVarChar, 200).Value = (motivo ?? "").Trim();
                    cmd.Parameters.Add("@obs", SqlDbType.NVarChar, -1).Value = (observacion ?? "").Trim();
                    cmd.ExecuteNonQuery();
                }
            }
            finally { db.cerrarConexion(); }
        }

        // ELIMINAR
        private void RC_DeleteById(int idRegistroClinico)
        {
            const string sql = @"DELETE FROM RegistroClinico WHERE IdRegistroClinico = @id;";
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idRegistroClinico;
                    cmd.ExecuteNonQuery();
                }
            }
            finally { db.cerrarConexion(); }
        }

        private void RC_DeleteByMascotaFechaOCita(int? idCita, int idMascota, DateTime fecha)
        {
            const string sql = @"
;WITH x AS (
  SELECT TOP(1) *
  FROM RegistroClinico
  WHERE (IdCita = @c AND @c IS NOT NULL)
     OR (IdCita IS NULL AND IdMascota = @m AND CONVERT(date, FechaRegistro) = @f)
  ORDER BY FechaRegistro DESC, IdRegistroClinico DESC
)
DELETE FROM x;";

            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@c", SqlDbType.Int).Value = idCita.HasValue ? (object)idCita.Value : DBNull.Value;
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                    cmd.Parameters.Add("@f", SqlDbType.Date).Value = fecha.Date;
                    cmd.ExecuteNonQuery();
                }
            }
            finally { db.cerrarConexion(); }
        }

        // Helper usado por existencia y decorado
        private bool RC_ExistePara(int idMascota, DateTime fecha)
        {
            var row = RC_GetByMascotaFecha(idMascota, fecha);
            return row != null;
        }

        // ==================== Utilidades ====================
        private DateTime ParseDateFromRow(DataGridView grid, int rowIndex)
        {
            DateTime f = DateTime.Today;
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return f;
            DateTime tmp;
            string v = Convert.ToString(grid.Rows[rowIndex].Cells["Fecha"].Value);
            if (DateTime.TryParse(v, out tmp)) f = tmp.Date;
            return f;
        }

        private bool TryParseFecha(DataRow r, out DateTime fecha)
        {
            fecha = DateTime.MinValue;
            DateTime tmp;
            if (r.Table.Columns.Contains("Fecha") && DateTime.TryParse(Convert.ToString(r["Fecha"]), out tmp))
            { fecha = tmp.Date; return true; }
            if (r.Table.Columns.Contains("FechaHora") && DateTime.TryParse(Convert.ToString(r["FechaHora"]), out tmp))
            { fecha = tmp.Date; return true; }
            return false;
        }

        private CitaInfo ObtenerCitaInfoDesdeFila(DataGridView grid, int rowIndex)
        {
            Func<string, object> Get = name => grid.Columns.Contains(name) ? grid.Rows[rowIndex].Cells[name].Value : null;

            int idCita; int.TryParse(Convert.ToString(Get("IdCita")), out idCita);
            int idMascota; int.TryParse(Convert.ToString(Get("IdMascota")), out idMascota);

            string masc = Convert.ToString(Get("Mascota"));
            string esp = Convert.ToString(Get("Especie"));
            string raz = Convert.ToString(Get("Raza"));
            string prop = Convert.ToString(Get("Propietario"));
            string mot = Convert.ToString(Get("Motivo"));

            DateTime fecha = DateTime.Today; DateTime.TryParse(Convert.ToString(Get("Fecha")), out fecha);

            TimeSpan hora = TimeSpan.Zero;
            var vh = Convert.ToString(Get("Hora"));
            if (!string.IsNullOrWhiteSpace(vh))
            {
                DateTime ht;
                if (!TimeSpan.TryParse(vh, out hora) && DateTime.TryParse(vh, out ht)) hora = ht.TimeOfDay;
            }

            if (idCita <= 0 && idMascota <= 0 && string.IsNullOrEmpty(masc)) return null;

            return new CitaInfo
            {
                IdCita = idCita,
                IdMascota = idMascota,
                Mascota = masc,
                Especie = esp,
                Raza = raz,
                Propietario = prop,
                Motivo = mot,
                FechaHora = (fecha == DateTime.MinValue ? DateTime.Today : fecha.Date).Add(hora)
            };
        }

        private int ToIntSafe(DataRow r, string col)
        {
            if (!r.Table.Columns.Contains(col)) return 0;
            int v; int.TryParse(Convert.ToString(r[col]), out v);
            return v;
        }
    }

    // === Modelo simple para pasar datos entre formularios ===
    public class CitaInfo
    {
        public int IdCita { get; set; }
        public int IdMascota { get; set; }
        public string Mascota { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public string Propietario { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
