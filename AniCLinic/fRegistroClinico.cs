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

            // Quitar subscripciones previas y usar SOLO CellContentClick (evita doble apertura)
            try { dgvHoy.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvProximas.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvAnteriores.CellContentClick -= Grid_ButtonClick; } catch { }

            PrepararGrid_Hoy(dgvHoy);
            PrepararGrid_Proximas(dgvProximas);
            PrepararGrid_Anteriores(dgvAnteriores);

            dgvHoy.CellContentClick += Grid_ButtonClick;
            dgvProximas.CellContentClick += Grid_ButtonClick;
            dgvAnteriores.CellContentClick += Grid_ButtonClick;

            // Refrescos visuales post-bindeo
            dgvHoy.DataBindingComplete += (s, e) => {
                QuitarColEstadoEnGrid(dgvHoy); QuitarFilaNueva(dgvHoy);
            };
            dgvProximas.DataBindingComplete += (s, e) => {
                QuitarFilaNueva(dgvProximas);
            };
            dgvAnteriores.DataBindingComplete += (s, e) => {
                QuitarColEstadoEnGrid(dgvAnteriores); QuitarFilaNueva(dgvAnteriores);
                DecorarAnterioresSegunRegistro();
            };

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
            grid.AllowUserToAddRows = false; // <- elimina la fila en blanco
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
            // SIN Estado en HOY
            grid.Columns.Add(MkBtn("colRegistrar", "Registrar", 110));
            QuitarColEstadoEnGrid(grid);
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
            // Próximas puede mostrar Estado si lo traes
            if (!grid.Columns.Contains("Estado"))
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
            // SIN Estado en ANTERIORES
            QuitarColEstadoEnGrid(grid);
            grid.Columns.Add(MkBtn("colEditar", "Editar", 95));
            grid.Columns.Add(MkBtn("colEliminar", "Eliminar", 95));
        }

        // Elimina cualquier columna 'Estado' que haya puesto el diseñador
        private void QuitarColEstadoEnGrid(DataGridView grid)
        {
            // por Name
            var porNombre = grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Name.Equals("Estado", StringComparison.OrdinalIgnoreCase)
                         || c.Name.IndexOf("estado", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            foreach (var c in porNombre) grid.Columns.Remove(c);

            // por DataPropertyName
            var porProp = grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => (c.DataPropertyName ?? "").Equals("Estado", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var c in porProp) grid.Columns.Remove(c);
        }

        // Quita la fila de "nuevo" si el diseñador la vuelve a activar
        private void QuitarFilaNueva(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
        }

        // ==================== Carga y separación ====================
        private void RecargarTodo()
        {
            RecargarColeccionesDesdeBD();

            AplicarBusqueda(dgvHoy, _dtHoy, txtBuscarHoy?.Text);
            AplicarBusqueda(dgvProximas, _dtProximas, txtBuscarProximas?.Text);
            AplicarBusqueda(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores?.Text);
        }

        private void RecargarColeccionesDesdeBD()
        {
            var all = CedulaUtils.CitasListado();

            if (!all.Columns.Contains("Estado"))
                all.Columns.Add("Estado", typeof(string));

            var hoyDate = DateTime.Today;

            _dtHoy = all.Clone();
            _dtProximas = all.Clone();
            _dtAnteriores = all.Clone();

            foreach (DataRow r in all.Rows)
            {
                if (!TryParseFecha(r, out DateTime f)) continue;
                var d = f.Date;

                if (d > hoyDate) r["Estado"] = "Próximo";

                int idMascota = ToIntSafe(r, "IdMascota");
                bool tieneReg = TieneRegistroClinico(idMascota, d);

                if (d == hoyDate)
                {
                    if (tieneReg) _dtAnteriores.Rows.Add((object[])r.ItemArray.Clone());
                    else _dtHoy.Rows.Add((object[])r.ItemArray.Clone());
                }
                else if (d > hoyDate) _dtProximas.Rows.Add((object[])r.ItemArray.Clone());
                else _dtAnteriores.Rows.Add((object[])r.ItemArray.Clone());
            }

            // Eliminar filas vacías (id nulo/0 y sin mascota)
            LimpiarFilasVacias(_dtHoy);
            LimpiarFilasVacias(_dtProximas);
            LimpiarFilasVacias(_dtAnteriores);

            dgvHoy.DataSource = _dtHoy;
            dgvProximas.DataSource = _dtProximas;
            dgvAnteriores.DataSource = _dtAnteriores;

            // Asegurar que Estado no quede en Hoy/Anteriores por diseñador
            QuitarColEstadoEnGrid(dgvHoy);
            QuitarColEstadoEnGrid(dgvAnteriores);

            // Decorar Anteriores
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

        private int ToIntSafe(DataRow r, string col)
        {
            if (!r.Table.Columns.Contains(col)) return 0;
            int v; int.TryParse(Convert.ToString(r[col]), out v);
            return v;
        }

        // ==================== Búsquedas ====================
        private void WireBusquedas()
        {
            if (txtBuscarHoy != null)
            {
                txtBuscarHoy.TextChanged -= (s, e) => AplicarBusqueda(dgvHoy, _dtHoy, txtBuscarHoy?.Text);
                txtBuscarHoy.TextChanged += (s, e) => AplicarBusqueda(dgvHoy, _dtHoy, txtBuscarHoy?.Text);
            }
            if (txtBuscarProximas != null)
            {
                txtBuscarProximas.TextChanged -= (s, e) => AplicarBusqueda(dgvProximas, _dtProximas, txtBuscarProximas?.Text);
                txtBuscarProximas.TextChanged += (s, e) => AplicarBusqueda(dgvProximas, _dtProximas, txtBuscarProximas?.Text);
            }
            if (txtBuscarAnteriores != null)
            {
                txtBuscarAnteriores.TextChanged -= (s, e) => AplicarBusqueda(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores?.Text);
                txtBuscarAnteriores.TextChanged += (s, e) => AplicarBusqueda(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores?.Text);
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
            var dv = new DataView(baseTable) { RowFilter = expr };
            grid.DataSource = dv;
        }

        // ==================== Clicks de botones ====================
        private void Grid_ButtonClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = (DataGridView)sender;

            // Solo si la celda es botón
            bool isBtn = grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn
                      || grid.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell;
            if (!isBtn) return;

            // Ignorar "Sin registro" o botón vacío
            var txt = Convert.ToString(grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            if (string.IsNullOrWhiteSpace(txt) || txt.Equals("Sin registro", StringComparison.OrdinalIgnoreCase))
                return;

            var info = ObtenerCitaInfoDesdeFila(grid, e.RowIndex);
            if (info == null)
            {
                MessageBox.Show("No se pudo leer la información de la fila.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (grid == dgvHoy && grid.Columns[e.ColumnIndex].Name == "colRegistrar")
            {
                using (var frm = new AggRegistroClinico(info, false))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK) RecargarTodo();
                }
                return;
            }

            if (grid == dgvAnteriores && grid.Columns[e.ColumnIndex].Name == "colEditar")
            {
                using (var frm = new AggRegistroClinico(info, true))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK) RecargarTodo();
                }
                return;
            }

            if (grid == dgvAnteriores && grid.Columns[e.ColumnIndex].Name == "colEliminar")
            {
                var ok = MessageBox.Show("¿Eliminar el registro clínico de esta cita?",
                    "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ok == DialogResult.Yes)
                {
                    try { EliminarRegistroClinico(info); RecargarTodo(); }
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

                var fecha = ParseDateFromRow(dgvAnteriores, row.Index);
                int idMascota = 0;
                int.TryParse(Convert.ToString(row.Cells["IdMascota"].Value), out idMascota);

                bool tiene = TieneRegistroClinico(idMascota, fecha);

                // Editar
                if (row.Cells["colEditar"] is DataGridViewButtonCell btnEdit)
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
                        btnEdit.Value = "Editar";
                        btnEdit.ReadOnly = false;
                        btnEdit.Style.ForeColor = dgvAnteriores.DefaultCellStyle.ForeColor;
                    }
                }

                // Eliminar
                if (row.Cells["colEliminar"] is DataGridViewButtonCell btnDel)
                {
                    if (!tiene)
                    {
                        var txtCell = new DataGridViewTextBoxCell { Value = "" };
                        row.Cells["colEliminar"] = txtCell;
                        row.Cells["colEliminar"].ReadOnly = true;
                        row.Cells["colEliminar"].Style.ForeColor = Color.Gainsboro;
                        row.Cells["colEliminar"].Style.BackColor = Color.Gainsboro;
                    }
                    else
                    {
                        btnDel.Value = "Eliminar";
                        btnDel.ReadOnly = false;
                        btnDel.Style.ForeColor = dgvAnteriores.DefaultCellStyle.ForeColor;
                    }
                }
            }
        }

        // ==================== DB helpers ====================
        private void EliminarRegistroClinico(CitaInfo info)
        {
            DateTime fechaCita = info.FechaHora.Date;

            const string sql = @"
WITH x AS (
  SELECT TOP(1) *
  FROM RegistroClinico
  WHERE IdMascota = @m AND CONVERT(date, FechaRegistro) = @f
  ORDER BY FechaRegistro DESC, IdRegistroClinico DESC
)
DELETE FROM x;";

            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = info.IdMascota;
                    cmd.Parameters.Add("@f", SqlDbType.Date).Value = fechaCita;
                    cmd.ExecuteNonQuery();
                }
            }
            finally { db.cerrarConexion(); }
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
            {
                fecha = tmp.Date; return true;
            }
            if (r.Table.Columns.Contains("FechaHora") && DateTime.TryParse(Convert.ToString(r["FechaHora"]), out tmp))
            {
                fecha = tmp.Date; return true;
            }
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

        private bool TieneRegistroClinico(int idMascota, DateTime fecha)
        {
            if (idMascota <= 0) return false;

            const string sql = @"
SELECT TOP(1) 1
FROM RegistroClinico
WHERE IdMascota = @m AND CONVERT(date, FechaRegistro) = @f;";

            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                    cmd.Parameters.Add("@f", SqlDbType.Date).Value = fecha.Date;
                    var obj = cmd.ExecuteScalar();
                    return obj != null && obj != DBNull.Value;
                }
            }
            finally { db.cerrarConexion(); }
        }
    }

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
