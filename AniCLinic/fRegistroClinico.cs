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
        // Columna auxiliar para "forzar" que la fila tenga RC justo después de registrar
        private const string COL_TIENE_REG = "__TieneRegistroClinico";

        private DataTable _dtHoy, _dtProximas, _dtAnteriores;

        public fRegistroClinico()
        {
            InitializeComponent();

            // Solo CellContentClick para evitar doble apertura
            try { dgvHoy.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvProximas.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvAnteriores.CellContentClick -= Grid_ButtonClick; } catch { }

            PrepararGrid_Hoy(dgvHoy);
            PrepararGrid_Proximas(dgvProximas);
            PrepararGrid_Anteriores(dgvAnteriores);

            dgvHoy.CellContentClick += Grid_ButtonClick;
            dgvProximas.CellContentClick += Grid_ButtonClick;
            dgvAnteriores.CellContentClick += Grid_ButtonClick;

            dgvHoy.DataBindingComplete += (s, e) => { QuitarFilaNueva(dgvHoy); };
            dgvProximas.DataBindingComplete += (s, e) => { QuitarFilaNueva(dgvProximas); };
            dgvAnteriores.DataBindingComplete += (s, e) => { QuitarFilaNueva(dgvAnteriores); DecorarAnterioresSegunRegistro(); };

            WireBusquedas();
            RecargarTodo();
        }

        #region Preparación de grids
        private void PrepararBase(DataGridView grid)
        {
            grid.DataSource = null;
            grid.AutoGenerateColumns = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.AllowUserToAddRows = false;
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
            grid.Columns.Add(MkText("Estado", "Estado", 100)); // solo Próximas
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

            // ⬇️ Columna oculta que “marca” que la fila ya tiene RC (override hasta recargar BD)
            grid.Columns.Add(MkHidden(COL_TIENE_REG));

            grid.Columns.Add(MkBtn("colEditar", "Editar", 95));
            grid.Columns.Add(MkBtn("colEliminar", "Eliminar", 95));
        }

        private void QuitarFilaNueva(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
        }
        #endregion

        #region Carga / separación
        private void RecargarTodo()
        {
            RecargarColeccionesDesdeBD();

            // Búsqueda activa
            AplicarBusqueda(dgvHoy, _dtHoy, txtBuscarHoy == null ? null : txtBuscarHoy.Text);
            AplicarBusqueda(dgvProximas, _dtProximas, txtBuscarProximas == null ? null : txtBuscarProximas.Text);
            AplicarBusqueda(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores == null ? null : txtBuscarAnteriores.Text);
        }

        private void RecargarColeccionesDesdeBD()
        {
            var all = CedulaUtils.CitasListado(); // trae IdCita, IdMascota, Mascota, Especie, Raza, Fecha/Hora, Motivo, Propietario...

            if (!all.Columns.Contains("Estado"))
                all.Columns.Add("Estado", typeof(string));

            var hoy = DateTime.Today;

            _dtHoy = all.Clone();
            _dtProximas = all.Clone();
            _dtAnteriores = all.Clone();

            // aseguro columna oculta en tablas (por si movemos filas a mano)
            EnsureTieneRegCol(_dtHoy);
            EnsureTieneRegCol(_dtProximas);
            EnsureTieneRegCol(_dtAnteriores);

            foreach (DataRow r in all.Rows)
            {
                if (!TryParseFecha(r, out DateTime f)) continue;
                var d = f.Date;

                if (d > hoy) r["Estado"] = "Próximo";

                int idMascota = ToInt(r, "IdMascota");
                bool tieneReg = RC_ExistePara(idMascota, d); // consulta real

                if (d == hoy)
                {
                    if (tieneReg) _dtAnteriores.Rows.Add((object[])r.ItemArray.Clone());
                    else _dtHoy.Rows.Add((object[])r.ItemArray.Clone());
                }
                else if (d > hoy)
                {
                    _dtProximas.Rows.Add((object[])r.ItemArray.Clone());
                }
                else
                {
                    _dtAnteriores.Rows.Add((object[])r.ItemArray.Clone());
                }
            }

            // Hoy y Anteriores no muestran Estado → elimino del DataTable (no del grid)
            if (_dtHoy.Columns.Contains("Estado")) _dtHoy.Columns.Remove("Estado");
            if (_dtAnteriores.Columns.Contains("Estado")) _dtAnteriores.Columns.Remove("Estado");

            dgvHoy.DataSource = _dtHoy;
            dgvProximas.DataSource = _dtProximas;
            dgvAnteriores.DataSource = _dtAnteriores;

            DecorarAnterioresSegunRegistro();
        }

        private void EnsureTieneRegCol(DataTable dt)
        {
            if (dt == null) return;
            if (!dt.Columns.Contains(COL_TIENE_REG))
                dt.Columns.Add(COL_TIENE_REG, typeof(bool));
        }
        #endregion

        #region Búsqueda
        private void WireBusquedas()
        {
            if (txtBuscarHoy != null)
            {
                txtBuscarHoy.TextChanged -= (s, e) => AplicarBusqueda(dgvHoy, _dtHoy, txtBuscarHoy.Text);
                txtBuscarHoy.TextChanged += (s, e) => AplicarBusqueda(dgvHoy, _dtHoy, txtBuscarHoy.Text);
            }

            if (txtBuscarProximas != null)
            {
                txtBuscarProximas.TextChanged -= (s, e) => AplicarBusqueda(dgvProximas, _dtProximas, txtBuscarProximas.Text);
                txtBuscarProximas.TextChanged += (s, e) => AplicarBusqueda(dgvProximas, _dtProximas, txtBuscarProximas.Text);
            }

            if (txtBuscarAnteriores != null)
            {
                txtBuscarAnteriores.TextChanged -= (s, e) => AplicarBusqueda(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores.Text);
                txtBuscarAnteriores.TextChanged += (s, e) => AplicarBusqueda(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores.Text);
            }
        }

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
                return num ? $"CONVERT([{c}], 'System.String') LIKE '%{val}%'" : $"([{c}] LIKE '%{val}%')";
            }).ToArray());
            grid.DataSource = new DataView(baseTable) { RowFilter = expr };
        }
        #endregion

        #region Clicks botones (Registrar/Editar/Eliminar)
        private void Grid_ButtonClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = (DataGridView)sender;
            if (!(grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            var info = ObtenerCitaInfoDesdeFila(grid, e.RowIndex);
            if (info == null) return;

            // Registrar (HOY)
            if (grid == dgvHoy && grid.Columns[e.ColumnIndex].Name == "colRegistrar")
            {
                using (var frm = new AggRegistroClinico(info, false))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        // 1) quitar de Hoy
                        var dv = grid.DataSource as DataView;
                        DataRowView drv = null;
                        if (dv != null) drv = dv[e.RowIndex];
                        else if (grid.Rows[e.RowIndex].DataBoundItem is DataRowView d0) drv = d0;

                        if (drv != null)
                        {
                            var nueva = _dtAnteriores.NewRow();
                            nueva.ItemArray = (object[])drv.Row.ItemArray.Clone();
                            // 2) marcar override para mostrar Editar sin esperar recargar BD
                            EnsureTieneRegCol(_dtAnteriores);
                            nueva[COL_TIENE_REG] = true;
                            _dtAnteriores.Rows.Add(nueva);

                            drv.Row.Delete();
                            _dtHoy.AcceptChanges();
                            _dtAnteriores.AcceptChanges();

                            // Rebind para refrescar vistas y decorado
                            dgvHoy.DataSource = _dtHoy;
                            dgvAnteriores.DataSource = _dtAnteriores;
                            DecorarAnterioresSegunRegistro();
                        }
                        else
                        {
                            // fallback: recarga general
                            RecargarTodo();
                        }
                    }
                }
                return;
            }

            // Editar (ANTERIORES)
            if (grid == dgvAnteriores && grid.Columns[e.ColumnIndex].Name == "colEditar")
            {
                using (var frm = new AggRegistroClinico(info, true))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        // ya estaba en anteriores; solo redecorar por si cambió algo
                        DecorarAnterioresSegunRegistro();
                    }
                }
                return;
            }

            // Eliminar (ANTERIORES)
            if (grid == dgvAnteriores && grid.Columns[e.ColumnIndex].Name == "colEliminar")
            {
                if (MessageBox.Show("¿Eliminar el registro clínico de esta cita?",
                    "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        RC_DeleteByMascotaFechaOCita(info.IdCita > 0 ? (int?)info.IdCita : null,
                                                     info.IdMascota,
                                                     info.FechaHora);
                        // quitar override si lo hubiera
                        var dv = grid.DataSource as DataView;
                        DataRowView drv = dv != null ? dv[e.RowIndex] : (grid.Rows[e.RowIndex].DataBoundItem as DataRowView);
                        if (drv != null && drv.Row.Table.Columns.Contains(COL_TIENE_REG))
                            drv.Row[COL_TIENE_REG] = false;

                        DecorarAnterioresSegunRegistro();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo eliminar: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        #endregion

        #region Decorado de ANTERIORES (mostrar Editar/Sin registro)
        private void DecorarAnterioresSegunRegistro()
        {
            if (dgvAnteriores.Rows.Count == 0) return;

            foreach (DataGridViewRow row in dgvAnteriores.Rows)
            {
                if (row.IsNewRow) continue;

                DateTime fecha = ParseDateFromRow(dgvAnteriores, row.Index);
                int idMascota = ToInt(row.Cells["IdMascota"]?.Value);

                // Leer override desde la celda oculta (si existe)
                bool overrideTiene = false;
                if (dgvAnteriores.Columns.Contains(COL_TIENE_REG))
                {
                    var val = row.Cells[COL_TIENE_REG]?.Value;
                    if (val is bool b) overrideTiene = b;
                }

                bool tiene = overrideTiene || RC_ExistePara(idMascota, fecha);

                // ===== Editar =====
                if (!tiene)
                {
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

                // ===== Eliminar (siempre botón) =====
                if (!(row.Cells["colEliminar"] is DataGridViewButtonCell))
                    row.Cells["colEliminar"] = new DataGridViewButtonCell();
                row.Cells["colEliminar"].ReadOnly = false;
                row.Cells["colEliminar"].Value = "Eliminar";
                row.Cells["colEliminar"].Style.BackColor = dgvAnteriores.DefaultCellStyle.BackColor;
                row.Cells["colEliminar"].Style.ForeColor = dgvAnteriores.DefaultCellStyle.ForeColor;
            }
        }
        #endregion

        #region CRUD/DB helpers (registro clínico)
        private bool RC_ExistePara(int idMascota, DateTime fecha)
        {
            if (idMascota <= 0) return false;
            const string sql = @"SELECT TOP(1) 1
                                 FROM RegistroClinico
                                 WHERE IdMascota=@m AND CONVERT(date, FechaRegistro)=@f;";
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                    cmd.Parameters.Add("@f", SqlDbType.Date).Value = fecha.Date;
                    var o = cmd.ExecuteScalar();
                    return o != null && o != DBNull.Value;
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
     OR (IdCita IS NULL AND IdMascota=@m AND CONVERT(date, FechaRegistro)=@f)
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
        #endregion

        #region Utilidades
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

        private static int ToInt(object v)
        {
            int n; return int.TryParse(Convert.ToString(v), out n) ? n : 0;
        }
        private static int ToInt(DataRow r, string col)
        {
            if (r == null || !r.Table.Columns.Contains(col)) return 0;
            int n; return int.TryParse(Convert.ToString(r[col]), out n) ? n : 0;
        }
        #endregion
    }

    // Modelo simple para pasar datos
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
