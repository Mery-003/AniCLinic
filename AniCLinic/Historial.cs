using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Historial : Form
    {
        private readonly csCRUD _crud = new csCRUD();

        public Historial()
        {
            InitializeComponent();
            PrepararGrid();
            WireEvents();
            CargarPacientes();
        }

        private void WireEvents()
        {
            try { btnVer.Click -= btnVer_Click; } catch { }
            try { txtMascotaNombre.TextChanged -= txtMascotaNombre_TextChanged; } catch { }

            btnVer.Click += btnVer_Click;
            txtMascotaNombre.TextChanged += txtMascotaNombre_TextChanged;
        }

        private void PrepararGrid()
        {
            var g = dgvHistorial;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.AutoGenerateColumns = true;                       
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            g.DataBindingComplete += (s, e) =>
            {
                if (g.Columns.Contains("ID"))
                {
                    g.Columns["ID"].Width = 60;
                    g.Columns["ID"].DisplayIndex = 0;
                }
            };
        }

        private void CargarPacientes(string filtro = "")
        {
            string sql = @"
SELECT 
    M.IdMascota AS ID,
    M.Nombre,
    M.Especie,
    M.Raza,
    M.Sexo,
    M.Edad,
    M.PesoKg,
    M.Discapacidad,
    (P.Nombre + ' ' + P.Apellido) AS Propietario
FROM Mascota M
INNER JOIN Persona P ON P.IdPersona = M.IdPersona
WHERE (@Filtro = '' 
       OR P.Cedula LIKE @Filtro + '%'
       OR P.Nombre LIKE @Filtro + '%'
       OR M.Nombre LIKE @Filtro + '%')
ORDER BY M.IdMascota DESC;";

            dgvHistorial.DataSource = _crud.cargarBDData(sql, new SqlParameter("@Filtro", filtro ?? ""));
        }

        private void txtMascotaNombre_TextChanged(object sender, EventArgs e)
            => CargarPacientes((txtMascotaNombre.Text ?? "").Trim());

        private void btnVer_Click(object sender, EventArgs e)
        {
            if (dgvHistorial.CurrentRow == null ||
                dgvHistorial.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Seleccione un paciente de la lista.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var drv = (DataRowView)dgvHistorial.CurrentRow.DataBoundItem;

            int idMascota = Convert.ToInt32(drv["ID"]);
            string nombreMascota = Convert.ToString(drv["Nombre"]);
            string propietario = Convert.ToString(drv["Propietario"]);

            string sql = @"
SELECT 
    rc.IdRegistroClinico,
    rc.FechaRegistro,
    ISNULL(rc.MotivoConsulta,'') AS MotivoConsulta
FROM RegistroClinico rc
WHERE rc.IdMascota = @m
ORDER BY rc.FechaRegistro DESC, rc.IdRegistroClinico DESC;";

            var dt = _crud.cargarBDData(sql, new SqlParameter("@m", idMascota));
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Esta mascota no tiene fichas registradas.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var picker = new HistorialFecha(dt, $"{nombreMascota} — {propietario}"))
            {
                if (picker.ShowDialog(this) == DialogResult.OK && picker.SelectedIdRegistroClinico > 0)
                {
                    using (var ver = new VerHistorial(picker.SelectedIdRegistroClinico))
                        ver.ShowDialog(this);
                }
            }
        }
    }
}
