using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class FormularioCarnet : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        public FormularioCarnet()
        {
            InitializeComponent();
        }

        private void FormularioCarnet_Load(object sender, EventArgs e)
        {
            CargarCarnets();

            try { dgvCarnet.CellClick -= dgvCarnet_CellClick; } catch { }
            try { dgvCarnet.CellContentClick -= dgvCarnet_CellContentClick; } catch { }
            dgvCarnet.CellClick += dgvCarnet_CellClick;
            dgvCarnet.CellContentClick += dgvCarnet_CellContentClick;
        }

        private void CargarCarnets()
        {
            using (SqlConnection con = _db.AbrirConexion())
            {
                // ¿existen estas columnas? (NO usaremos PesoKg ni EdadAnios)
                bool hasFN = ColumnExists(con, "Mascota", "FechaNacimiento");
                bool hasETxt = ColumnExists(con, "Mascota", "EdadTexto");

                // Armado robusto del SELECT (sin comas sobrantes)
                var cols = new List<string>
                {
                    "m.IdMascota",
                    "m.Nombre AS Mascota",
                    "m.Especie",
                    "m.Raza",
                    "m.Sexo"
                };
                if (hasETxt) cols.Add("m.EdadTexto");
                if (hasFN) cols.Add("m.FechaNacimiento");
                cols.Add(@"
CASE 
    WHEN LTRIM(RTRIM(p.Apellido)) IN ('', '(s/n)', 's/n', 'S/N') THEN p.Nombre
    ELSE CONCAT(p.Nombre, ' ', p.Apellido)
END AS Propietario");

                // ⚠️ FIX: reemplazado IdPropietarioMascota (inexistente) por IdPropietario
                string sql = $@"
SELECT
    {string.Join(",\n    ", cols)}
FROM dbo.Mascota m
OUTER APPLY (
    SELECT TOP 1 prp.IdPropietario
    FROM dbo.PropietarioMascota pm
    INNER JOIN dbo.Propietario prp ON prp.IdPropietario = pm.IdPropietario
    WHERE pm.IdMascota = m.IdMascota
    ORDER BY pm.EsPrincipal DESC, pm.IdPropietario
) ap
LEFT JOIN dbo.Propietario pr ON pr.IdPropietario = ap.IdPropietario
LEFT JOIN dbo.Persona p      ON p.IdPersona = pr.IdPersona
ORDER BY m.Nombre;";

                var da = new SqlDataAdapter(sql, con);
                var dt = new DataTable();
                da.Fill(dt);

                // Columna "Edad" calculada (sin EdadAnios/PesoKg)
                if (!dt.Columns.Contains("Edad"))
                    dt.Columns.Add("Edad", typeof(string));

                foreach (DataRow r in dt.Rows)
                {
                    string edadTxt = "";

                    // 1) prioriza EdadTexto
                    if (hasETxt && dt.Columns.Contains("EdadTexto") && r["EdadTexto"] != DBNull.Value)
                    {
                        string et = Convert.ToString(r["EdadTexto"]).Trim();
                        if (!string.IsNullOrEmpty(et)) edadTxt = et;
                    }
                    // 2) si no hay, usa FechaNacimiento
                    if (string.IsNullOrEmpty(edadTxt) && hasFN &&
                        dt.Columns.Contains("FechaNacimiento") && r["FechaNacimiento"] != DBNull.Value)
                    {
                        DateTime fn = Convert.ToDateTime(r["FechaNacimiento"]);
                        int months = (DateTime.Today.Year - fn.Year) * 12 + (DateTime.Today.Month - fn.Month);
                        if (DateTime.Today.Day < fn.Day) months--;
                        if (months < 0) months = 0;
                        edadTxt = (months >= 12) ? (months / 12) + " año(s)" : months + " mes(es)";
                    }

                    r["Edad"] = edadTxt;
                }

                // Configuración del grid
                dgvCarnet.Columns.Clear();
                dgvCarnet.AutoGenerateColumns = false;
                dgvCarnet.AllowUserToAddRows = false;
                dgvCarnet.ReadOnly = true;
                dgvCarnet.RowHeadersVisible = false;
                dgvCarnet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCarnet.MultiSelect = false;
                dgvCarnet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvCarnet.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "IdMascota",
                    Name = "IdMascota",
                    Visible = false
                });

                AddCol("Mascota", "Mascota");
                AddCol("Especie", "Especie");
                AddCol("Raza", "Raza");
                AddCol("Sexo", "Sexo");
                AddCol("Edad", "Edad");
                AddCol("Propietario", "Propietario");

                if (!dgvCarnet.Columns.Contains("VerCarnet"))
                {
                    var btn = new DataGridViewButtonColumn
                    {
                        Name = "VerCarnet",
                        HeaderText = "Acciones",
                        Text = "Ver Carnet",
                        UseColumnTextForButtonValue = true,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                    };
                    dgvCarnet.Columns.Add(btn);
                }

                dgvCarnet.DataSource = dt;
            }
        }

        // Abre VerCarnet desde el botón del grid (soporta ambos eventos)
        private void dgvCarnet_CellClick(object sender, DataGridViewCellEventArgs e)
            => AbrirCarnetDesdeGrid(dgvCarnet, e);

        private void dgvCarnet_CellContentClick(object sender, DataGridViewCellEventArgs e)
            => AbrirCarnetDesdeGrid(dgvCarnet, e);

        private void AbrirCarnetDesdeGrid(DataGridView grid, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!(grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn) ||
                grid.Columns[e.ColumnIndex].Name != "VerCarnet") return;

            int idMascota;
            if (grid.Rows[e.RowIndex].DataBoundItem is DataRowView drv &&
                drv.Row.Table.Columns.Contains("IdMascota"))
                idMascota = Convert.ToInt32(drv["IdMascota"]);
            else
                idMascota = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["IdMascota"].Value);

            using (var frm = new VerCarnet(idMascota))
                frm.ShowDialog(this);
        }

        // Botón del diseñador: abre VerCarnet de la fila seleccionada
        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            if (dgvCarnet.CurrentRow == null) return;

            int idMascota;
            if (dgvCarnet.CurrentRow.DataBoundItem is DataRowView drv &&
                drv.Row.Table.Columns.Contains("IdMascota"))
                idMascota = Convert.ToInt32(drv["IdMascota"]);
            else
            {
                if (!dgvCarnet.Columns.Contains("IdMascota")) return;
                idMascota = Convert.ToInt32(dgvCarnet.CurrentRow.Cells["IdMascota"].Value);
            }

            using (var frm = new VerCarnet(idMascota))
                frm.ShowDialog(this);
        }

        // Helpers
        private static bool ColumnExists(SqlConnection con, string table, string column)
        {
            using (var cmd = new SqlCommand(@"
                SELECT 1
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @t AND COLUMN_NAME = @c;", con))
            {
                cmd.Parameters.AddWithValue("@t", table);
                cmd.Parameters.AddWithValue("@c", column);
                var o = cmd.ExecuteScalar();
                return o != null && o != DBNull.Value;
            }
        }

        private void AddCol(string header, string dataProperty)
        {
            dgvCarnet.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = dataProperty,
                ReadOnly = true
            });
        }
    }
}
