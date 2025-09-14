using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AniCLinic
{
    public partial class fRegistroClinico : Form
    {
        private const int VENTANA_HOY_MIN = 20;   // +20 min para permanecer en HOY

        private DataTable _dtHoy, _dtProximas, _dtAnteriores;

        public fRegistroClinico()
        {
            InitializeComponent();

            // Un solo manejador de clics por grid
            try { dgvHoy.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvProximas.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvAnteriores.CellContentClick -= Grid_ButtonClick; } catch { }

            PrepararGrid_Hoy(dgvHoy);
            PrepararGrid_Proximas(dgvProximas);
            PrepararGrid_Anteriores(dgvAnteriores);

            dgvHoy.CellContentClick += Grid_ButtonClick;
            dgvProximas.CellContentClick += Grid_ButtonClick;
            dgvAnteriores.CellContentClick += Grid_ButtonClick;

            dgvHoy.DataBindingComplete += (s, e) => QuitarFilaNueva(dgvHoy);
            dgvProximas.DataBindingComplete += (s, e) => QuitarFilaNueva(dgvProximas);
            dgvAnteriores.DataBindingComplete += (s, e) => { QuitarFilaNueva(dgvAnteriores); DecorarAnterioresSegunRegistro(); };

            // Cursor mano solo sobre botones en Anteriores
            dgvAnteriores.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    dgvAnteriores.Cursor = dgvAnteriores.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell ? Cursors.Hand : Cursors.Default;
            };
            dgvAnteriores.CellMouseLeave += (s, e) => dgvAnteriores.Cursor = Cursors.Default;

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
            => new DataGridViewTextBoxColumn { Name = prop, HeaderText = header, DataPropertyName = prop, Width = width, ReadOnly = true };

        private DataGridViewTextBoxColumn MkHidden(string prop)
        {
            var c = MkText("", prop, 2);
            c.Visible = false;
            return c;
        }

        private static DataGridViewButtonColumn MkBtn(string name, string text, int width = 100)
            => new DataGridViewButtonColumn { Name = name, HeaderText = "", Text = text, UseColumnTextForButtonValue = true, Width = width };

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
            grid.Columns.Add(MkBtn("colRegistrar", "Registrar"));
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

        #region Carga / separación (+20 minutos)
        private void RecargarTodo()
        {
            RecargarColeccionesDesdeBD();

            AplicarBusqueda(dgvHoy, _dtHoy, txtBuscarHoy == null ? null : txtBuscarHoy.Text);
            AplicarBusqueda(dgvProximas, _dtProximas, txtBuscarProximas == null ? null : txtBuscarProximas.Text);
            AplicarBusqueda(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores == null ? null : txtBuscarAnteriores.Text);
        }

        private void RecargarColeccionesDesdeBD()
        {
            // Debe traer: IdCita, IdMascota, Mascota, Especie, Raza, Fecha, Hora, Motivo, Propietario, (opcional CedulaPropietario), y/o FechaHora
            var all = CedulaUtils.CitasListado();

            if (!all.Columns.Contains("Estado"))
                all.Columns.Add("Estado", typeof(string));

            var hoy = DateTime.Today;
            var ahora = DateTime.Now;

            _dtHoy = all.Clone();
            _dtProximas = all.Clone();
            _dtAnteriores = all.Clone();

            foreach (DataRow r in all.Rows)
            {
                if (!TryParseFechaHora(r, out DateTime fh)) continue;

                var d = fh.Date;
                int idMascota = ToInt(r, "IdMascota");
                int idCita = ToInt(r, "IdCita");

                if (d > hoy)
                {
                    r["Estado"] = "Próximo";
                    _dtProximas.Rows.Add((object[])r.ItemArray.Clone());
                    continue;
                }
                if (d < hoy)
                {
                    _dtAnteriores.Rows.Add((object[])r.ItemArray.Clone());
                    continue;
                }

                // d == hoy
                bool tieneReg = RC_ExisteParaCita(idCita > 0 ? (int?)idCita : null, idMascota, fh);

                if (!tieneReg && fh.AddMinutes(VENTANA_HOY_MIN) >= ahora)
                    _dtHoy.Rows.Add((object[])r.ItemArray.Clone());
                else
                    _dtAnteriores.Rows.Add((object[])r.ItemArray.Clone());
            }

            if (_dtHoy.Columns.Contains("Estado")) _dtHoy.Columns.Remove("Estado");
            if (_dtAnteriores.Columns.Contains("Estado")) _dtAnteriores.Columns.Remove("Estado");

            dgvHoy.DataSource = _dtHoy;
            dgvProximas.DataSource = _dtProximas;
            dgvAnteriores.DataSource = _dtAnteriores;

            DecorarAnterioresSegunRegistro();
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

            var exprParts = cols.Select(c =>
            {
                var col = baseTable.Columns[c];
                bool needsConvert = col.DataType != typeof(string);
                return needsConvert
                    ? $"Convert([{c}], 'System.String') LIKE '%{val}%'"
                    : $"([{c}] LIKE '%{val}%')";
            });

            var expr = string.Join(" OR ", exprParts);

            grid.DataSource = new DataView(baseTable) { RowFilter = expr };
        }
        #endregion

        #region Clicks botones
        private void Grid_ButtonClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = (DataGridView)sender;

            // Si no es botón (ej. "Sin registro"), salir
            if (!(grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn) ||
                !(grid.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell))
                return;

            var info = ObtenerCitaInfoDesdeFila(grid, e.RowIndex);
            if (info == null) return;

            // Registrar (HOY) → abrir en blanco (AggRegistroClinico ya no precarga nada)
            if (grid == dgvHoy && grid.Columns[e.ColumnIndex].Name == "colRegistrar")
            {
                using (var frm = new AggRegistroClinico(info, false))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        // Recolocar datasets
                        RecargarTodo();
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
                        DecorarAnterioresSegunRegistro();
                }
                return;
            }

            // Eliminar (ANTERIORES) → borra RC de ESA cita y la cita
            if (grid == dgvAnteriores && grid.Columns[e.ColumnIndex].Name == "colEliminar")
            {
                if (MessageBox.Show("¿Eliminar el registro clínico y su cita vinculada?",
                    "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        int rcBorrados = RC_DeletePorCita(
                            info.IdCita > 0 ? (int?)info.IdCita : null,
                            info.IdMascota,
                            info.FechaHora);

                        int citasBorradas = Cita_DeleteByIdOCriterios(
                            info.IdCita > 0 ? (int?)info.IdCita : null,
                            info.IdMascota,
                            info.FechaHora);

                        QuitarFilaDelData(grid, e.RowIndex);
                        RefrescarFormularioCitasSiAbierto();

                        MessageBox.Show("Eliminación completada.", "OK",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #region Decorado de ANTERIORES (Editar/Sin registro)
        private void DecorarAnterioresSegunRegistro()
        {
            if (dgvAnteriores.Rows.Count == 0) return;

            foreach (DataGridViewRow row in dgvAnteriores.Rows)
            {
                if (row.IsNewRow) continue;

                if (!TryGetFechaHoraFromGridRow(dgvAnteriores, row.Index, out DateTime fechaHora))
                    fechaHora = ParseDateFromRow(dgvAnteriores, row.Index);

                int idMascota = ToInt(row.Cells["IdMascota"]?.Value);
                int idCita = ToInt(row.Cells["IdCita"]?.Value);

                bool tiene = RC_ExisteParaCita(idCita > 0 ? (int?)idCita : null, idMascota, fechaHora);

                // Editar
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

                // Eliminar (siempre botón)
                if (!(row.Cells["colEliminar"] is DataGridViewButtonCell))
                    row.Cells["colEliminar"] = new DataGridViewButtonCell();
                row.Cells["colEliminar"].ReadOnly = false;
                row.Cells["colEliminar"].Value = "Eliminar";
                row.Cells["colEliminar"].Style.BackColor = dgvAnteriores.DefaultCellStyle.BackColor;
                row.Cells["colEliminar"].Style.ForeColor = dgvAnteriores.DefaultCellStyle.ForeColor;
            }
        }
        #endregion

        #region DB helpers (usando IdCita solo para consultar)
        // ¿Existe RC para esa CITA?
        private bool RC_ExisteParaCita(int? idCita, int idMascota, DateTime fechaHora)
        {
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                if (idCita.HasValue)
                {
                    const string sqlJoin = @"
SELECT TOP(1) 1
FROM RegistroClinico rc
JOIN GestionCita c ON c.IdCita = @c
WHERE rc.IdMascota = c.IdMascota
  AND DATEDIFF(MINUTE, rc.FechaRegistro, c.FechaHora) = 0;";
                    using (var cmd = new SqlCommand(sqlJoin, db.obtenerConexion()))
                    {
                        cmd.Parameters.Add("@c", SqlDbType.Int).Value = idCita.Value;
                        var o = cmd.ExecuteScalar();
                        return o != null && o != DBNull.Value;
                    }
                }
                else
                {
                    const string sql = @"
SELECT TOP(1) 1
FROM RegistroClinico
WHERE IdMascota = @m
  AND DATEDIFF(MINUTE, FechaRegistro, @fh) = 0;";
                    using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                    {
                        cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                        cmd.Parameters.Add("@fh", SqlDbType.DateTime).Value = fechaHora;
                        var o = cmd.ExecuteScalar();
                        return o != null && o != DBNull.Value;
                    }
                }
            }
            finally { db.cerrarConexion(); }
        }

        // Borrar RC de ESA cita
        // Borra el RC que corresponde EXACTAMENTE a esa cita.
        // Si viene IdCita, usamos un JOIN para localizar el IdRegistroClinico y lo borramos por Id.
        // Si no hay IdCita, igualamos por IdMascota + FechaHora al minuto.
        private int RC_DeletePorCita(int? idCita, int idMascota, DateTime fechaHora)
        {
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                if (idCita.HasValue)
                {
                    const string sql = @"
DECLARE @rid int;
SELECT TOP(1) @rid = rc.IdRegistroClinico
FROM RegistroClinico rc
JOIN GestionCita c ON c.IdCita = @c
WHERE rc.IdMascota = c.IdMascota
  AND DATEDIFF(MINUTE, rc.FechaRegistro, c.FechaHora) = 0
ORDER BY rc.IdRegistroClinico DESC;

IF @rid IS NOT NULL
BEGIN
    DELETE FROM RegistroClinico WHERE IdRegistroClinico = @rid;
    SELECT @@ROWCOUNT;
END
ELSE
BEGIN
    SELECT 0;
END";
                    using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                    {
                        cmd.Parameters.Add("@c", SqlDbType.Int).Value = idCita.Value;
                        var o = cmd.ExecuteScalar();
                        return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
                    }
                }
                else
                {
                    const string sql = @"
DELETE TOP(1) FROM RegistroClinico
WHERE IdMascota = @m
  AND DATEDIFF(MINUTE, FechaRegistro, @fh) = 0;
SELECT @@ROWCOUNT;";
                    using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                    {
                        cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                        cmd.Parameters.Add("@fh", SqlDbType.DateTime).Value = fechaHora;
                        var o = cmd.ExecuteScalar();
                        return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
                    }
                }
            }
            finally { db.cerrarConexion(); }
        }


        // Eliminar cita por IdCita si existe; si no, resolver por mascota+fecha (más cercano)
        private int Cita_DeleteByIdOCriterios(int? idCita, int idMascota, DateTime fechaHora)
        {
            const string sql = @"
DECLARE @id int = @c;
IF @id IS NULL
BEGIN
    SELECT TOP(1) @id = IdCita
    FROM dbo.GestionCita
    WHERE IdMascota=@m AND CONVERT(date, FechaHora)=CONVERT(date, @fh)
    ORDER BY ABS(DATEDIFF(MINUTE, FechaHora, @fh)) ASC, FechaHora DESC, IdCita DESC;
END

IF @id IS NOT NULL
BEGIN
    DELETE FROM dbo.GestionCita WHERE IdCita=@id;
    SELECT 1;
END
ELSE
BEGIN
    SELECT 0;
END";
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@c", SqlDbType.Int).Value = idCita.HasValue ? (object)idCita.Value : DBNull.Value;
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                    cmd.Parameters.Add("@fh", SqlDbType.DateTime).Value = fechaHora;
                    var o = cmd.ExecuteScalar();
                    return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
                }
            }
            finally { db.cerrarConexion(); }
        }
        #endregion

        #region Utilidades
        private void QuitarFilaDelData(DataGridView grid, int rowIndex)
        {
            try
            {
                var dv = grid.DataSource as DataView;
                var drv = dv != null ? dv[rowIndex] : (grid.Rows[rowIndex].DataBoundItem as DataRowView);

                if (drv != null)
                {
                    drv.Delete();
                    drv.Row.Table.AcceptChanges();
                }
                else if (rowIndex >= 0 && rowIndex < grid.Rows.Count)
                {
                    grid.Rows.RemoveAt(rowIndex);
                }
            }
            catch { }

            if (grid == dgvAnteriores) DecorarAnterioresSegunRegistro();
        }

        private void RefrescarFormularioCitasSiAbierto()
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f is fCitas citas)
                {
                    citas.RefrescarListado();
                    break;
                }
            }
        }

        private DateTime ParseDateFromRow(DataGridView grid, int rowIndex)
        {
            DateTime f = DateTime.Today;
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return f;
            if (DateTime.TryParse(Convert.ToString(grid.Rows[rowIndex].Cells["Fecha"].Value), out DateTime tmp))
                f = tmp.Date;
            return f;
        }

        private bool TryParseFechaHora(DataRow r, out DateTime fh)
        {
            fh = DateTime.MinValue;

            if (r.Table.Columns.Contains("FechaHora") &&
                DateTime.TryParse(Convert.ToString(r["FechaHora"]), out DateTime fh1))
            { fh = fh1; return true; }

            if (!(r.Table.Columns.Contains("Fecha") &&
                  DateTime.TryParse(Convert.ToString(r["Fecha"]), out DateTime fx)))
                return false;

            TimeSpan hora = TimeSpan.Zero;
            if (r.Table.Columns.Contains("Hora"))
            {
                var vh = Convert.ToString(r["Hora"]);
                if (!string.IsNullOrWhiteSpace(vh))
                {
                    if (!TimeSpan.TryParse(vh, out hora) && DateTime.TryParse(vh, out DateTime ht))
                        hora = ht.TimeOfDay;
                }
            }

            fh = fx.Date.Add(hora);
            return true;
        }

        private bool TryGetFechaHoraFromGridRow(DataGridView grid, int rowIndex, out DateTime fechaHora)
        {
            fechaHora = DateTime.MinValue;
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return false;

            if (grid.Rows[rowIndex].DataBoundItem is DataRowView drv)
            {
                var t = drv.Row.Table;
                if (t.Columns.Contains("FechaHora") &&
                    DateTime.TryParse(Convert.ToString(drv["FechaHora"]), out DateTime fh))
                { fechaHora = fh; return true; }
            }

            DateTime f;
            var vf = Convert.ToString(grid.Rows[rowIndex].Cells["Fecha"]?.Value);
            if (!DateTime.TryParse(vf, out f)) return false;
            f = f.Date;

            TimeSpan h = TimeSpan.Zero;
            var vh = Convert.ToString(grid.Rows[rowIndex].Cells["Hora"]?.Value);
            if (!string.IsNullOrWhiteSpace(vh))
            {
                if (!TimeSpan.TryParse(vh, out h) && DateTime.TryParse(vh, out DateTime ht))
                    h = ht.TimeOfDay;
            }

            fechaHora = f.Add(h);
            return true;
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

            if (!TryGetFechaHoraFromGridRow(grid, rowIndex, out DateTime fechaHora))
                fechaHora = DateTime.Today;

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
                FechaHora = fechaHora
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
