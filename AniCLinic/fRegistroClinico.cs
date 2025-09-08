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

        // ===== Colores (texto gris oscuro y resaltado claro) =====
        private static readonly Color TEXT_GRAY = Color.FromArgb(0x37, 0x41, 0x51); // #374151
        private static readonly Color SEL_GRAY = Color.FromArgb(0xE5, 0xE7, 0xEB); // #E5E7EB

        // Tabla de Registro Clínico
        private const string SCH_REG = "dbo";
        private const string TBL_REG = "RegistroClinico";

        public fRegistroClinico()
        {
            InitializeComponent();

            // por si el diseñador dejó algo enganchado
            try { dgvHoy.CellClick -= Grid_ButtonClick; dgvHoy.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvProximas.CellClick -= Grid_ButtonClick; dgvProximas.CellContentClick -= Grid_ButtonClick; } catch { }
            try { dgvAnteriores.CellClick -= Grid_ButtonClick; dgvAnteriores.CellContentClick -= Grid_ButtonClick; } catch { }

            ConfigurarGridsBase();

            PrepararGrid_Hoy(dgvHoy);
            PrepararGrid_Proximas(dgvProximas);
            PrepararGrid_Anteriores(dgvAnteriores);

            AsegurarBotones();
            EstiloBotones();

            // conectar ambos para asegurar el click
            dgvHoy.CellClick += Grid_ButtonClick; dgvHoy.CellContentClick += Grid_ButtonClick;
            dgvProximas.CellClick += Grid_ButtonClick; dgvProximas.CellContentClick += Grid_ButtonClick;
            dgvAnteriores.CellClick += Grid_ButtonClick; dgvAnteriores.CellContentClick += Grid_ButtonClick;

            // Re-decorar "Anteriores" cuando cambian/ordenan los datos
            dgvAnteriores.DataBindingComplete -= DgvAnteriores_DataBindingComplete;
            dgvAnteriores.DataBindingComplete += DgvAnteriores_DataBindingComplete;
            dgvAnteriores.Sorted -= DgvAnteriores_Sorted;
            dgvAnteriores.Sorted += DgvAnteriores_Sorted;

            WireBusquedas();
            RecargarTodo();
        }

        private void DgvAnteriores_DataBindingComplete(object s, DataGridViewBindingCompleteEventArgs e)
            => DecorarGridAnteriores();
        private void DgvAnteriores_Sorted(object s, EventArgs e)
            => DecorarGridAnteriores();

        // ==================== Estilo base ====================
        private void ConfigurarGridsBase()
        {
            foreach (var g in new DataGridView[] { dgvHoy, dgvProximas, dgvAnteriores })
            {
                if (g == null) continue;

                g.AutoGenerateColumns = false;
                g.AllowUserToAddRows = false;
                g.MultiSelect = false;
                g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                g.ReadOnly = true;
                g.Columns.Clear();

                g.BackgroundColor = SystemColors.Window;
                g.DefaultCellStyle.BackColor = SystemColors.Window;
                g.RowsDefaultCellStyle.BackColor = SystemColors.Window;
                g.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Window;

                // Texto gris más oscuro
                g.DefaultCellStyle.ForeColor = TEXT_GRAY;
                g.RowsDefaultCellStyle.ForeColor = TEXT_GRAY;
                g.AlternatingRowsDefaultCellStyle.ForeColor = TEXT_GRAY;

                // Resaltado claro
                g.DefaultCellStyle.SelectionBackColor = SEL_GRAY;
                g.DefaultCellStyle.SelectionForeColor = TEXT_GRAY;
                g.RowHeadersDefaultCellStyle.SelectionBackColor = SEL_GRAY;
                g.RowHeadersDefaultCellStyle.SelectionForeColor = TEXT_GRAY;

                g.GridColor = SystemColors.ControlLight;
            }
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
            grid.Columns.Add(MkHidden("IdMascota"));
            grid.Columns.Add(MkHidden("IdVeterinario"));

            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            grid.Columns.Add(MkText("Veterinario", "Veterinario", 140));
            grid.Columns.Add(MkText("Estado", "Estado", 100));
        }
        private void PrepararGrid_Proximas(DataGridView grid)
        {
            grid.Columns.Add(MkHidden("IdMascota"));
            grid.Columns.Add(MkHidden("IdVeterinario"));

            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            grid.Columns.Add(MkText("Veterinario", "Veterinario", 140));
            grid.Columns.Add(MkText("Estado", "Estado", 100));
        }
        private void PrepararGrid_Anteriores(DataGridView grid)
        {
            grid.Columns.Add(MkHidden("IdMascota"));
            grid.Columns.Add(MkHidden("IdVeterinario"));

            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            grid.Columns.Add(MkText("Veterinario", "Veterinario", 140));
            // >>> Sin columna "Estado" en Anteriores
        }

        private void AsegurarBotones()
        {
            if (dgvHoy.Columns["colRegistrar"] == null) dgvHoy.Columns.Add(MkBtn("colRegistrar", "Registrar", 110));
            if (dgvAnteriores.Columns["colEditar"] == null) dgvAnteriores.Columns.Add(MkBtn("colEditar", "Editar", 95));
            if (dgvAnteriores.Columns["colEliminar"] == null) dgvAnteriores.Columns.Add(MkBtn("colEliminar", "Eliminar", 95));
        }
        private void EstiloBotones()
        {
            var c = TEXT_GRAY;
            var b0 = dgvHoy.Columns["colRegistrar"] as DataGridViewButtonColumn;
            var b1 = dgvAnteriores.Columns["colEditar"] as DataGridViewButtonColumn;
            var b2 = dgvAnteriores.Columns["colEliminar"] as DataGridViewButtonColumn;
            if (b0 != null) b0.DefaultCellStyle.ForeColor = c;
            if (b1 != null) b1.DefaultCellStyle.ForeColor = c;
            if (b2 != null) b2.DefaultCellStyle.ForeColor = c;
        }

        // ==================== Carga ====================
        private void RecargarTodo()
        {
            RecargarHoy(); RecargarProximas(); RecargarAnteriores();
            AplicarBusquedaHoy(); AplicarBusquedaProximas(); AplicarBusquedaAnteriores();
        }

        private void RecargarHoy()
        {
            var dt = CedulaUtils.CitasListado(null); // debe traer IdMascota e IdVeterinario si es posible
            if (!dt.Columns.Contains("Estado")) dt.Columns.Add("Estado", typeof(string));
            foreach (DataRow r in dt.Rows) r["Estado"] = "Pendiente";
            _dtHoy = FiltrarPorFecha(dt, TipoSeccion.Hoy);
            dgvHoy.DataSource = _dtHoy;
        }
        private void RecargarProximas()
        {
            var dt = CedulaUtils.CitasListado(null);
            if (!dt.Columns.Contains("Estado")) dt.Columns.Add("Estado", typeof(string));
            foreach (DataRow r in dt.Rows) r["Estado"] = "Próximo";
            _dtProximas = FiltrarPorFecha(dt, TipoSeccion.Proximas);
            dgvProximas.DataSource = _dtProximas;
        }
        private void RecargarAnteriores()
        {
            var dt = CedulaUtils.CitasListado(null);
            if (!dt.Columns.Contains("Estado")) dt.Columns.Add("Estado", typeof(string));
            foreach (DataRow r in dt.Rows) r["Estado"] = "No registrado";
            _dtAnteriores = FiltrarPorFecha(dt, TipoSeccion.Anteriores);
            dgvAnteriores.DataSource = _dtAnteriores;

            // Decora filas de "Anteriores" según tengan o no registro clínico
            DecorarGridAnteriores();
        }

        private enum TipoSeccion { Hoy, Proximas, Anteriores }
        private DataTable FiltrarPorFecha(DataTable dt, TipoSeccion seccion)
        {
            if (dt == null) return null;
            var hoy = DateTime.Today;
            var clone = dt.Clone();

            foreach (DataRow r in dt.Rows)
            {
                DateTime f;
                if (!TryParseFecha(r, out f)) continue;
                var d = f.Date;
                bool ok = (seccion == TipoSeccion.Hoy && d == hoy) ||
                          (seccion == TipoSeccion.Proximas && d > hoy) ||
                          (seccion == TipoSeccion.Anteriores && d < hoy);
                if (ok) clone.Rows.Add((object[])r.ItemArray.Clone());
            }
            return clone;
        }
        private bool TryParseFecha(DataRow r, out DateTime fecha)
        {
            fecha = DateTime.MinValue;
            DateTime tmp;
            if (r.Table.Columns.Contains("Fecha") && DateTime.TryParse(Convert.ToString(r["Fecha"]), out tmp)) { fecha = tmp.Date; return true; }
            if (r.Table.Columns.Contains("FechaHora") && DateTime.TryParse(Convert.ToString(r["FechaHora"]), out tmp)) { fecha = tmp.Date; return true; }
            return false;
        }

        // ==================== Búsquedas ====================
        private void WireBusquedas()
        {
            if (txtBuscarHoy != null) { txtBuscarHoy.TextChanged -= TxtBuscarHoy_TextChanged; txtBuscarHoy.TextChanged += TxtBuscarHoy_TextChanged; }
            if (txtBuscarProximas != null) { txtBuscarProximas.TextChanged -= TxtBuscarProximas_TextChanged; txtBuscarProximas.TextChanged += TxtBuscarProximas_TextChanged; }
            if (txtBuscarAnteriores != null) { txtBuscarAnteriores.TextChanged -= TxtBuscarAnteriores_TextChanged; txtBuscarAnteriores.TextChanged += TxtBuscarAnteriores_TextChanged; }
        }
        private void TxtBuscarHoy_TextChanged(object s, EventArgs e) { AplicarBusquedaHoy(); }
        private void TxtBuscarProximas_TextChanged(object s, EventArgs e) { AplicarBusquedaProximas(); }
        private void TxtBuscarAnteriores_TextChanged(object s, EventArgs e)
        {
            AplicarBusquedaAnteriores();
            DecorarGridAnteriores(); // aplicar de nuevo según el filtro
        }

        private void AplicarBusquedaHoy() { AplicarBusqueda(dgvHoy, _dtHoy, txtBuscarHoy == null ? null : txtBuscarHoy.Text); }
        private void AplicarBusquedaProximas() { AplicarBusqueda(dgvProximas, _dtProximas, txtBuscarProximas == null ? null : txtBuscarProximas.Text); }
        private void AplicarBusquedaAnteriores() { AplicarBusqueda(dgvAnteriores, _dtAnteriores, txtBuscarAnteriores == null ? null : txtBuscarAnteriores.Text); }

        private void AplicarBusqueda(DataGridView grid, DataTable baseTable, string term)
        {
            if (grid == null || baseTable == null) return;
            var t = (term ?? "").Trim();
            if (t.Length == 0) { grid.DataSource = baseTable; return; }

            string[] campos = { "IdCita", "Mascota", "Especie", "Raza", "Fecha", "Hora", "Motivo", "Propietario", "Veterinario", "Estado", "CedulaPropietario" };
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

        // ==================== Botones ====================
        private void Grid_ButtonClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = (DataGridView)sender;

            // Si la celda no es botón, no hacemos nada
            var btnCol = grid.Columns[e.ColumnIndex] as DataGridViewButtonColumn;
            if (btnCol == null) return;

            // 1) intenta leer desde la fila
            var info = ObtenerCitaInfoDesdeFila(grid, e.RowIndex);

            // 2) si faltan IDs, intenta completar por DB usando IdCita
            if ((info == null) || info.IdMascota <= 0 || info.IdVeterinario <= 0)
            {
                int idCita = 0;
                try { idCita = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["IdCita"].Value); } catch { }
                if (idCita > 0)
                {
                    var full = ObtenerCitaInfo_DB(idCita);
                    if (full != null) info = full;
                }
            }

            if (info == null)
            {
                MessageBox.Show("No se pudo leer la información de la fila.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // En "Anteriores": permitir Eliminar siempre; bloquear Editar si no hay registro
            if (grid == dgvAnteriores)
            {
                bool tiene = ExisteRegistroClinico(info);
                if (btnCol.Name == "colEditar" && !tiene) return;
                // si es Eliminar, continúa (aunque no exista registro no pasa nada)
            }

            if (btnCol.Name == "colRegistrar")
            {
                using (var frm = new AggRegistroClinico(info, false))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK) RecargarSegunGrid(grid);
                }
            }
            else if (btnCol.Name == "colEditar")
            {
                using (var frm = new AggRegistroClinico(info, true))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK) RecargarSegunGrid(grid);
                }
            }
            else if (btnCol.Name == "colEliminar")
            {
                // asegurar IDs para eliminar
                if (info.IdMascota <= 0 || info.IdVeterinario <= 0)
                {
                    var full = ObtenerCitaInfo_DB(info.IdCita);
                    if (full != null) info = full;
                }

                var ok = MessageBox.Show("¿Eliminar el registro clínico de esta cita?",
                                         "Confirmar eliminación",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ok == DialogResult.Yes)
                {
                    try { EliminarRegistroClinico(info); RecargarSegunGrid(grid); }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo eliminar: " + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RecargarSegunGrid(DataGridView grid)
        {
            if (grid == dgvHoy) RecargarHoy();
            else if (grid == dgvProximas) RecargarProximas();
            else if (grid == dgvAnteriores) RecargarAnteriores();
        }

        // ====== Lee desde la FILA (permite IDs vacíos, se completan luego por DB) ======
        private CitaInfo ObtenerCitaInfoDesdeFila(DataGridView grid, int rowIndex)
        {
            Func<string, object> Get = name => grid.Columns.Contains(name) ? grid.Rows[rowIndex].Cells[name].Value : null;

            int idCita; int.TryParse(Convert.ToString(Get("IdCita")), out idCita);
            int idMascota; int.TryParse(Convert.ToString(Get("IdMascota")), out idMascota);
            int idVet; int.TryParse(Convert.ToString(Get("IdVeterinario")), out idVet);

            string masc = Convert.ToString(Get("Mascota"));
            string esp = Convert.ToString(Get("Especie"));
            string raz = Convert.ToString(Get("Raza"));
            string prop = Convert.ToString(Get("Propietario"));
            string vet = Convert.ToString(Get("Veterinario"));
            string mot = Convert.ToString(Get("Motivo"));

            DateTime fecha = DateTime.Today; DateTime.TryParse(Convert.ToString(Get("Fecha")), out fecha);

            TimeSpan hora = TimeSpan.Zero;
            var vh = Convert.ToString(Get("Hora"));
            if (!string.IsNullOrWhiteSpace(vh))
            {
                DateTime ht;
                if (!TimeSpan.TryParse(vh, out hora) && DateTime.TryParse(vh, out ht)) hora = ht.TimeOfDay;
            }

            // Si no hay nada útil, devuelve null
            if (idCita <= 0 && idMascota <= 0 && string.IsNullOrEmpty(masc)) return null;

            return new CitaInfo
            {
                IdCita = idCita,
                IdMascota = idMascota,
                IdVeterinario = idVet,
                Mascota = masc,
                Especie = esp,
                Raza = raz,
                Propietario = prop,
                Veterinario = vet,
                Motivo = mot,
                FechaHora = (fecha == DateTime.MinValue ? DateTime.Today : fecha.Date).Add(hora)
            };
        }

        // ====== Completa por DB usando IdCita (2 variantes de FK de Personas) ======
        private CitaInfo ObtenerCitaInfo_DB(int idCita)
        {
            // Incluimos IdVeterinario para poder grabar/eliminar en RegistroClinico
            const string SQL_A = @"
SELECT c.IdCita, c.Fecha, c.Hora,
       m.IdMascota, m.Nombre AS Mascota, m.Especie, m.Raza,
       (p.Nombres + ' ' + p.Apellidos) AS Propietario,
       v.IdVeterinario, (v.Nombres + ' ' + v.Apellidos) AS Veterinario,
       c.Motivo
FROM dbo.GestionCita AS c
JOIN dbo.Mascota     AS m ON m.IdMascota = c.IdMascota
JOIN dbo.Personas    AS p ON p.IdPersona = c.IdPropietario
JOIN dbo.Veterinario AS v ON v.IdVeterinario = c.IdVeterinario
WHERE c.IdCita = @id;";

            const string SQL_B = @"
SELECT c.IdCita, c.Fecha, c.Hora,
       m.IdMascota, m.Nombre AS Mascota, m.Especie, m.Raza,
       (p.Nombres + ' ' + p.Apellidos) AS Propietario,
       v.IdVeterinario, (v.Nombres + ' ' + v.Apellidos) AS Veterinario,
       c.Motivo
FROM dbo.GestionCita AS c
JOIN dbo.Mascota     AS m ON m.IdMascota = c.IdMascota
JOIN dbo.Personas    AS p ON p.IdPersona = c.IdPersona
JOIN dbo.Veterinario AS v ON v.IdVeterinario = c.IdVeterinario
WHERE c.IdCita = @id;";

            CitaInfo info;
            if (TryLeerCita(SQL_A, idCita, out info)) return info;
            if (TryLeerCita(SQL_B, idCita, out info)) return info;

            return null;
        }

        private bool TryLeerCita(string sql, int idCita, out CitaInfo info)
        {
            info = null;
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) return false;

                        DateTime fecha = DateTime.Today;
                        if (rd["Fecha"] is DateTime) fecha = ((DateTime)rd["Fecha"]).Date;
                        else DateTime.TryParse(Convert.ToString(rd["Fecha"]), out fecha);

                        TimeSpan hora = TimeSpan.Zero;
                        string hs = rd["Hora"] == null ? null : rd["Hora"].ToString();
                        if (!string.IsNullOrWhiteSpace(hs))
                        {
                            DateTime ht;
                            if (!TimeSpan.TryParse(hs, out hora) && DateTime.TryParse(hs, out ht)) hora = ht.TimeOfDay;
                        }

                        info = new CitaInfo
                        {
                            IdCita = idCita,
                            IdMascota = Convert.ToInt32(rd["IdMascota"]),
                            IdVeterinario = Convert.ToInt32(rd["IdVeterinario"]),
                            Mascota = Convert.ToString(rd["Mascota"]),
                            Especie = Convert.ToString(rd["Especie"]),
                            Raza = Convert.ToString(rd["Raza"]),
                            Propietario = Convert.ToString(rd["Propietario"]),
                            Veterinario = Convert.ToString(rd["Veterinario"]),
                            Motivo = Convert.ToString(rd["Motivo"]),
                            FechaHora = fecha.Add(hora)
                        };
                        return true;
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }
            finally { db.cerrarConexion(); }
        }

        // ====== Helpers de Registro Clínico ======

        private bool ExisteRegistroClinico(CitaInfo info)
        {
            // Si faltan IDs, intenta completarlos por DB
            if ((info.IdMascota <= 0 || info.IdVeterinario <= 0) && info.IdCita > 0)
            {
                var full = ObtenerCitaInfo_DB(info.IdCita);
                if (full != null) info = full;
            }

            var fecha = info.FechaHora.Date;

            string sql = $@"
SELECT TOP(1) 1
FROM {SCH_REG}.{TBL_REG}
WHERE IdMascota = @m AND IdVeterinario = @v AND CONVERT(date, FechaRegistro) = @f;";

            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = info.IdMascota;
                    cmd.Parameters.Add("@v", SqlDbType.Int).Value = info.IdVeterinario;
                    cmd.Parameters.Add("@f", SqlDbType.Date).Value = fecha;
                    var o = cmd.ExecuteScalar();
                    return o != null && o != DBNull.Value;
                }
            }
            finally { db.cerrarConexion(); }
        }

        /// <summary>
        /// En "Anteriores":
        /// - Si NO tiene registro clínico => en la celda de Editar se muestra "No registro"
        ///   (texto, no botón) y en Eliminar se deja como botón.
        /// - Si SÍ tiene => botones Editar y Eliminar activos.
        /// </summary>
        private void DecorarGridAnteriores()
        {
            if (dgvAnteriores == null || dgvAnteriores.Rows.Count == 0) return;
            if (dgvAnteriores.Columns["colEditar"] == null || dgvAnteriores.Columns["colEliminar"] == null) return;

            for (int i = 0; i < dgvAnteriores.Rows.Count; i++)
            {
                var row = dgvAnteriores.Rows[i];
                if (row.IsNewRow) continue;

                var info = ObtenerCitaInfoDesdeFila(dgvAnteriores, i);
                if (info == null) continue;

                bool tieneRegistro = ExisteRegistroClinico(info);

                if (!tieneRegistro)
                {
                    // EDITAR -> texto "No registro"
                    var txt = new DataGridViewTextBoxCell { Value = "No registro" };
                    txt.Style.ForeColor = TEXT_GRAY;
                    txt.Style.Font = new Font(dgvAnteriores.Font, FontStyle.Italic);
                    row.Cells["colEditar"] = txt;

                    // ELIMINAR -> debe quedar como botón siempre
                    if (!(row.Cells["colEliminar"] is DataGridViewButtonCell))
                        row.Cells["colEliminar"] = new DataGridViewButtonCell();
                    row.Cells["colEliminar"].Value = "Eliminar";
                }
                else
                {
                    // EDITAR -> botón
                    if (!(row.Cells["colEditar"] is DataGridViewButtonCell))
                        row.Cells["colEditar"] = new DataGridViewButtonCell();
                    row.Cells["colEditar"].Value = "Editar";

                    // ELIMINAR -> botón
                    if (!(row.Cells["colEliminar"] is DataGridViewButtonCell))
                        row.Cells["colEliminar"] = new DataGridViewButtonCell();
                    row.Cells["colEliminar"].Value = "Eliminar";
                }
            }
        }

        // Borrar registro clínico del día (IdMascota + IdVeterinario + Fecha)
        private void EliminarRegistroClinico(CitaInfo info)
        {
            // completar si vinieron vacíos
            if ((info.IdMascota <= 0 || info.IdVeterinario <= 0) && info.IdCita > 0)
            {
                var full = ObtenerCitaInfo_DB(info.IdCita);
                if (full != null) info = full;
            }

            DateTime fechaCita = info.FechaHora.Date;

            string sql = string.Format(@"
WITH x AS (
  SELECT TOP(1) *
  FROM {0}.{1}
  WHERE IdMascota = @m AND IdVeterinario = @v AND CONVERT(date, FechaRegistro) = @f
  ORDER BY FechaRegistro DESC, IdRegistroClinico DESC
)
DELETE FROM x;", SCH_REG, TBL_REG);

            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = info.IdMascota;
                    cmd.Parameters.Add("@v", SqlDbType.Int).Value = info.IdVeterinario;
                    cmd.Parameters.Add("@f", SqlDbType.Date).Value = fechaCita;
                    cmd.ExecuteNonQuery();
                }
            }
            finally { db.cerrarConexion(); }
        }
    }

    public class CitaInfo
    {
        public int IdCita { get; set; }
        public int IdMascota { get; set; }
        public int IdVeterinario { get; set; }
        public string Mascota { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public string Propietario { get; set; }
        public string Veterinario { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
