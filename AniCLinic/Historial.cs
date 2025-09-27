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
    E.Especie,
    R.Raza,
    M.Sexo,
    FLOOR(DATEDIFF(DAY, M.FechaNacimiento, GETDATE()) / 365.25) AS Edad,
    M.PesoKg,
    M.Discapacidad,
    (P.Nombre + ' ' + P.Apellido) AS Propietario
FROM Mascota M
INNER JOIN Persona P ON P.IdPersona = M.IdPersona
INNER JOIN Especie   E ON E.IdEspecie = M.IdEspecie
INNER JOIN Raza      R ON R.IdRaza = M.IdRaza
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
            if (dgvHistorial.CurrentRow == null || dgvHistorial.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Seleccione un paciente de la lista.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Toma el Id de la mascota seleccionada en la grilla
            var drv = (DataRowView)dgvHistorial.CurrentRow.DataBoundItem;
            int idMascota = Convert.ToInt32(drv["ID"]);   // viene de M.IdMascota AS ID

            // Abre el formulario que contiene el ReportViewer
            using (var frm = new frmHistorialMedicoReport(idMascota))
            {
                frm.ShowDialog(this);
            }
        }


        private void txtMascotaNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
        }
    }
}
