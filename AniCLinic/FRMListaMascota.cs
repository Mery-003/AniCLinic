using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class FRMListaMascota : Form
    {
        private readonly csCRUD _crud = new csCRUD();

        public int IdMascotaSel { get; private set; }
        public string MascotaSel { get; private set; }
        public string EspecieSel { get; private set; }
        public string RazaSel { get; private set; }
        public string PropietarioSel { get; private set; } 

        public FRMListaMascota()
        {
            InitializeComponent();
            PrepararGrid(dgvListaMascota);

            this.Load += (s, e) => CargarLista();
            txtBuscarMascota.TextChanged += (s, e) => Filtrar();
            btnAceptar.Click += btnAceptar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
        }

        private static void PrepararGrid(DataGridView dgv)
        {
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoGenerateColumns = true;
        }

        private void CargarLista()
        {
            string sql = @"
SELECT 
    m.IdMascota AS ID,
    m.Nombre AS Mascota,
    e.Especie AS Especie,
    r.Raza    AS Raza,
    (p.Nombre + ' ' + p.Apellido) AS Propietario
FROM dbo.Mascota m
JOIN dbo.Persona p ON p.IdPersona = m.IdPersona
LEFT JOIN dbo.Especie e ON e.IdEspecie = m.IdEspecie
LEFT JOIN dbo.Raza    r ON r.IdRaza    = m.IdRaza
ORDER BY m.Nombre, p.Apellido, p.Nombre;";

            dgvListaMascota.DataSource = _crud.cargarBDData(sql);

            if (dgvListaMascota.Columns.Contains("ID")) dgvListaMascota.Columns["ID"].Width = 60;
            if (dgvListaMascota.Columns.Contains("Mascota")) dgvListaMascota.Columns["Mascota"].Width = 160;
            if (dgvListaMascota.Columns.Contains("Especie")) dgvListaMascota.Columns["Especie"].Width = 120;
            if (dgvListaMascota.Columns.Contains("Raza")) dgvListaMascota.Columns["Raza"].Width = 120;
            if (dgvListaMascota.Columns.Contains("Propietario")) dgvListaMascota.Columns["Propietario"].Width = 220;
        }

        private void Filtrar()
        {
            var dt = dgvListaMascota.DataSource as DataTable;
            if (dt == null) return;

            string q = (txtBuscarMascota.Text ?? "").Trim().Replace("'", "''");
            if (q.Length == 0) { dt.DefaultView.RowFilter = ""; return; }

            dt.DefaultView.RowFilter =
                "Mascota LIKE '%" + q + "%' OR " +
                "Especie LIKE '%" + q + "%' OR " +
                "Raza LIKE '%" + q + "%' OR " +
                "Propietario LIKE '%" + q + "%'";
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (dgvListaMascota.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una mascota.");
                return;
            }

            IdMascotaSel = Convert.ToInt32(dgvListaMascota.CurrentRow.Cells["ID"].Value);
            MascotaSel = Convert.ToString(dgvListaMascota.CurrentRow.Cells["Mascota"].Value);
            EspecieSel = Convert.ToString(dgvListaMascota.CurrentRow.Cells["Especie"].Value);
            RazaSel = Convert.ToString(dgvListaMascota.CurrentRow.Cells["Raza"].Value);
            PropietarioSel = Convert.ToString(dgvListaMascota.CurrentRow.Cells["Propietario"].Value);

            this.DialogResult = DialogResult.OK;
        }
    }
}
