using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fCitas : Form
    {
        // Usamos el mismo helper que en fPacientes
        private readonly csCRUD _crud = new csCRUD();

        public fCitas()
        {
            InitializeComponent();

            PrepararGrid();
            WireEvents();


            CargarData();
        }

        private void WireEvents()
        {
            btnNuvCita.Click -= btnNuvCita_Click;
            btnNuvCita.Click += btnNuvCita_Click;

            dgvCitas.CellContentClick -= DgvCitas_CellContentClick;
            dgvCitas.CellContentClick += DgvCitas_CellContentClick;

            dgvCitas.DataBindingComplete -= DgvCitas_DataBindingComplete;
            dgvCitas.DataBindingComplete += DgvCitas_DataBindingComplete;

            // === Búsqueda en vivo como fPacientes ===
            txtBuscar.TextChanged -= txtBuscar_TextChanged;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
        }

        private void PrepararGrid()
        {
            dgvCitas.AutoGenerateColumns = false;
            dgvCitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCitas.MultiSelect = false;
            dgvCitas.AllowUserToAddRows = false;
            dgvCitas.ReadOnly = true; // solo lectura
            dgvCitas.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvCitas.Columns.Clear();

            // Id (clave)
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colIdCita",
                HeaderText = "Id",
                DataPropertyName = "IdCita",
                Width = 60,
                ReadOnly = true
            });

            // Campos de vista
            dgvCitas.Columns.Add(MkText("Mascota", "Mascota", 120));
            dgvCitas.Columns.Add(MkText("Especie", "Especie", 100));
            dgvCitas.Columns.Add(MkText("Raza", "Raza", 120));

            // FECHA y HORA (calculadas)
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                HeaderText = "Fecha",
                DataPropertyName = "Fecha",
                Width = 90,
                ReadOnly = true
            });

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHora",
                HeaderText = "Hora",
                DataPropertyName = "Hora",
                Width = 70,
                ReadOnly = true
            });

            dgvCitas.Columns.Add(MkText("Motivo", "Motivo", 220));
            dgvCitas.Columns.Add(MkText("Propietario", "Propietario", 160));

            // Botones
            dgvCitas.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colEditar",
                HeaderText = "",
                Text = "Editar",
                UseColumnTextForButtonValue = true,
                Width = 70
            });

            dgvCitas.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colEliminar",
                HeaderText = "",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true,
                Width = 80
            });

            // Ocultas para lógica/filtro
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFechaHoraOculta",
                HeaderText = "FechaHora",
                DataPropertyName = "FechaHora",
                Visible = false,
                ReadOnly = true
            });

            // Cedula del propietario (para filtrar) si viene desde SQL
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCedulaOculta",
                HeaderText = "CedulaPropietario",
                DataPropertyName = "CedulaPropietario",
                Visible = false,
                ReadOnly = true
            });
        }

        private DataGridViewTextBoxColumn MkText(string header, string prop, int width) =>
            new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = true
            };

        // Búsqueda 
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            var filtro = (txtBuscar.Text ?? string.Empty).Trim();
            CargarData(filtro);
        }

        // Cargar/recargar datos DESDE SQL 
        private void CargarData(string filtro = "")
        {
            string sql = @"
SELECT
    C.IdCita,
    C.FechaHora,
    C.Motivo,
    M.Nombre AS Mascota,
    M.Especie,
    M.Raza,
    (P.Nombre + ' ' + P.Apellido) AS Propietario,
    P.Cedula AS CedulaPropietario
FROM GestionCita C
INNER JOIN Mascota   M ON M.IdMascota = C.IdMascota
INNER JOIN Persona   P ON P.IdPersona = M.IdPersona
WHERE (@Filtro = '' 
       OR P.Cedula LIKE @Filtro + '%'
       OR P.Nombre LIKE @Filtro + '%'
       OR M.Nombre LIKE @Filtro + '%')
ORDER BY C.FechaHora DESC;";

            // Cargamos DataTable como en fPacientes
            var dt = _crud.cargarBDData(sql, new SqlParameter("@Filtro", filtro));

            // Asegurar columnas Fecha/Hora visibles y reglas de edición
            PrepararFechasHorasYBind(dt);
        }

        private void PrepararFechasHorasYBind(DataTable dt)
        {
            if (dt == null)
            {
                dgvCitas.DataSource = null;
                return;
            }

            // Asegurar columnas visibles "Fecha" y "Hora"
            if (!dt.Columns.Contains("Fecha")) dt.Columns.Add("Fecha", typeof(string));
            if (!dt.Columns.Contains("Hora")) dt.Columns.Add("Hora", typeof(string));

            foreach (DataRow r in dt.Rows)
            {
                if (dt.Columns.Contains("FechaHora") && r["FechaHora"] != DBNull.Value)
                {
                    var fh = (DateTime)r["FechaHora"];
                    r["Fecha"] = fh.ToString("dd/MM/yyyy");
                    r["Hora"] = fh.ToString("HH:mm");
                }
                else
                {
                    if (TryBuildFechaHoraFromCols(r,
                            dt.Columns.Contains("Fecha") ? r["Fecha"] : null,
                            dt.Columns.Contains("Hora") ? r["Hora"] : null,
                            out DateTime fh2))
                    {
                        r["Fecha"] = fh2.ToString("dd/MM/yyyy");
                        r["Hora"] = fh2.ToString("HH:mm");
                        if (!dt.Columns.Contains("FechaHora"))
                            dt.Columns.Add("FechaHora", typeof(DateTime));
                        r["FechaHora"] = fh2;
                    }
                    else
                    {
                        if (r["Fecha"] == DBNull.Value) r["Fecha"] = "";
                        if (r["Hora"] == DBNull.Value) r["Hora"] = "";
                    }
                }
            }

            dgvCitas.DataSource = dt;
        }

        private bool TryBuildFechaHoraFromCols(DataRow r, object fechaObj, object horaObj, out DateTime fh)
        {
            fh = default;

            // FECHA
            DateTime fecha;
            if (fechaObj is DateTime fd) fecha = fd.Date;
            else if (!DateTime.TryParse(Convert.ToString(fechaObj), out fecha)) return false;
            else fecha = fecha.Date;

            // HORA
            TimeSpan hora;
            if (horaObj is TimeSpan hs) hora = hs;
            else
            {
                var hsTxt = Convert.ToString(horaObj)?.Trim() ?? "";
                if (!TimeSpan.TryParse(hsTxt, out hora))
                {
                    if (!TimeSpan.TryParseExact(hsTxt, new[] { @"hh\:mm", @"h\:mm" },
                        System.Globalization.CultureInfo.InvariantCulture, out hora))
                        return false;
                }
            }

            fh = fecha.Add(hora);
            return true;
        }

        // Estética y reglas de botones
        private void DgvCitas_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            AplicarReglasEditarPorFecha();
        }

        private void AplicarReglasEditarPorFecha()
        {
            var ahora = DateTime.Now;

            foreach (DataGridViewRow row in dgvCitas.Rows)
            {
                DateTime? fh = null;
                var val = row.Cells["colFechaHoraOculta"]?.Value;

                if (val != null && val != DBNull.Value)
                {
                    try { fh = Convert.ToDateTime(val); } catch { fh = null; }
                }

                bool esActualOFutura = fh.HasValue && fh.Value >= ahora;

                if (!esActualOFutura)
                {
                    // Reemplazar botón por texto "Cita Antigua" 
                    var celda = new DataGridViewTextBoxCell { Value = "Cita Antigua" };
                    row.Cells["colEditar"] = celda;             // 1) asignar a la fila
                    row.Cells["colEditar"].ReadOnly = true;      // 2) ahora sí, marcar ReadOnly
                    row.Cells["colEditar"].Style.BackColor = Color.FromArgb(255, 235, 238);         // rojo claro
                    row.Cells["colEditar"].Style.SelectionBackColor = Color.FromArgb(255, 205, 210); // rojo claro selección
                    row.Cells["colEditar"].Style.ForeColor = Color.FromArgb(183, 28, 28);           // rojo oscuro
                }
                else
                {
                    if (!(row.Cells["colEditar"] is DataGridViewButtonCell))
                        row.Cells["colEditar"] = new DataGridViewButtonCell { Value = "Editar" };

                    // Restablecer estilos
                    row.Cells["colEditar"].Style.BackColor = dgvCitas.DefaultCellStyle.BackColor;
                    row.Cells["colEditar"].Style.SelectionBackColor = dgvCitas.DefaultCellStyle.SelectionBackColor;
                    row.Cells["colEditar"].Style.ForeColor = dgvCitas.DefaultCellStyle.ForeColor;
                }
            }
        }

        // Acciones de botones
        private void btnNuvCita_Click(object sender, EventArgs e)
        {
            using (var frm = new AggCita())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    CargarData((txtBuscar.Text ?? string.Empty).Trim());
                }
            }
        }

        private void DgvCitas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var colName = dgvCitas.Columns[e.ColumnIndex].Name;

            if (colName == "colEditar" && dgvCitas.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell)
            {
                var id = Convert.ToInt32(dgvCitas.Rows[e.RowIndex].Cells["colIdCita"].Value);

                // Seguridad: solo permitir si es actual/futura
                var raw = dgvCitas.Rows[e.RowIndex].Cells["colFechaHoraOculta"]?.Value;
                DateTime? fh = null;
                if (raw != null && raw != DBNull.Value)
                {
                    try { fh = Convert.ToDateTime(raw); } catch { fh = null; }
                }
                if (!fh.HasValue || fh.Value < DateTime.Now) return;

                using (var frm = new AggCita(id))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        CargarData((txtBuscar.Text ?? string.Empty).Trim());
                    }
                }
            }
            else if (colName == "colEliminar")
            {
                var id = Convert.ToInt32(dgvCitas.Rows[e.RowIndex].Cells["colIdCita"].Value);
                if (MessageBox.Show("¿Eliminar esta cita?", "Confirmar",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    EliminarCita(id);
                    CargarData((txtBuscar.Text ?? string.Empty).Trim());
                }
            }
        }

        private void EliminarCita(int id)
        {
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var cmd = new SqlCommand(
                    "DELETE FROM dbo.GestionCita WHERE IdCita=@id;",
                    db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                db.cerrarConexion();
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e) { }
    }
}
