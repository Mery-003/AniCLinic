using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fPacientes : Form
    {
        private readonly csCRUD _crud = new csCRUD();

        public fPacientes()
        {
            InitializeComponent();

            PrepararGrid(dgvPropietarios);
            PrepararGrid(dgvMascotas);

            this.Load += (s, e) =>
            {
                RefrescarPropietarios();
                RefrescarMascotas();
            };

            btnPropietarios.Click += (s, e) =>
            {
                using (var f = new AggPropietario(this, 0))
                    f.ShowDialog();                    // sin Owner -> evita referencia circular
            };
            btnMascotas.Click += (s, e) =>
            {
                using (var f = new AggMascota(this, 0))
                    f.ShowDialog();                    // sin Owner
            };

            // Click en botones de grillas
            dgvPropietarios.CellContentClick += dgvPropietarios_CellContentClick;
            dgvMascotas.CellContentClick += dgvMascotas_CellContentClick;
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

        // ====================== PROPIETARIOS ======================
        public void RefrescarPropietarios()
        {
            string sql = @"
Select P.IdPersona as ID, (P.Nombre + ' ' + P.Apellido) as Propietario,
P.Cedula, P.Celular, P.Correo from Mascota M
Inner Join Persona P ON M.IdPersona=P.IdPersona";
            dgvPropietarios.Columns.Clear();
            dgvPropietarios.DataSource = _crud.cargarBDData(sql);
            AgregarColumnasAccion(dgvPropietarios);
            if (dgvPropietarios.Columns.Contains("ID")) dgvPropietarios.Columns["ID"].Width = 60;
            if (dgvPropietarios.Columns.Contains("C.I.")) dgvPropietarios.Columns["C.I."].Width = 130;
            if (dgvPropietarios.Columns.Contains("Celular")) dgvPropietarios.Columns["Celular"].Width = 140;
        }

        // ======================== MASCOTAS ========================
        public void RefrescarMascotas()
        {
            string sql = @"
SELECT m.IdMascota AS ID,
       m.Nombre AS Mascota,
       m.IdEspecie AS Especie,
       m.IdRaza AS Raza,
       m.Sexo AS Sexo,
       DATEDIFF(YEAR, m.FechaNacimiento, GETDATE()) AS [Edad (años)],
       (p.Nombre + ' ' + p.Apellido) AS Propietario
FROM Mascota m
INNER JOIN Persona p ON p.IdPersona = m.IdPersona
ORDER BY m.IdMascota DESC;";
            dgvMascotas.Columns.Clear();
            dgvMascotas.DataSource = _crud.cargarBDData(sql);
            AgregarColumnasAccion(dgvMascotas);
            if (dgvMascotas.Columns.Contains("ID")) dgvMascotas.Columns["ID"].Width = 60;
        }

        private static void AgregarColumnasAccion(DataGridView dgv)
        {
            bool tieneEditar = false, tieneEliminar = false;
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                if (c.HeaderText == "Editar") tieneEditar = true;
                if (c.HeaderText == "Eliminar") tieneEliminar = true;
            }
            if (!tieneEditar)
            {
                var c1 = new DataGridViewButtonColumn();
                c1.HeaderText = "Editar";
                c1.Text = "Editar";
                c1.UseColumnTextForButtonValue = true;
                c1.Width = 90;
                dgv.Columns.Add(c1);
            }
            if (!tieneEliminar)
            {
                var c2 = new DataGridViewButtonColumn();
                c2.HeaderText = "Eliminar";
                c2.Text = "Eliminar";
                c2.UseColumnTextForButtonValue = true;
                c2.Width = 90;
                dgv.Columns.Add(c2);
            }
        }

        // Clicks
        private void dgvPropietarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colBtn = dgvPropietarios.Columns[e.ColumnIndex] as DataGridViewButtonColumn;
            if (colBtn == null) return;

            int id = Convert.ToInt32(dgvPropietarios.Rows[e.RowIndex].Cells["ID"].Value);

            if (colBtn.HeaderText == "Editar")
            {
                using (var f = new AggPropietario(this, id))
                    f.ShowDialog(); // sin Owner
            }
            else if (colBtn.HeaderText == "Eliminar")
            {
                // ¿tiene mascotas?
                DataTable dt = _crud.cargarBDData("SELECT COUNT(1) FROM Mascota WHERE IdPersona=@id;", new SqlParameter("@id", id));
                int n = (dt != null && dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0][0]) : 0;

                string msg = n > 0
                    ? "Este propietario tiene " + n + " mascota(s).\nSe eliminarán también.\n\n¿Eliminar?"
                    : "¿Eliminar propietario?";

                if (MessageBox.Show(msg, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _crud.editarBD("DELETE FROM Mascota WHERE IdPersona=@id;", new SqlParameter("@id", id));
                    _crud.editarBD("DELETE FROM Persona WHERE IdPersona=@id;", new SqlParameter("@id", id));
                    RefrescarPropietarios();
                    RefrescarMascotas();
                }
            }
        }

        private void dgvMascotas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colBtn = dgvMascotas.Columns[e.ColumnIndex] as DataGridViewButtonColumn;
            if (colBtn == null) return;

            int id = Convert.ToInt32(dgvMascotas.Rows[e.RowIndex].Cells["ID"].Value);

            if (colBtn.HeaderText == "Editar")
            {
                using (var f = new AggMascota(this, id))
                    f.ShowDialog(); // sin Owner
            }
            else if (colBtn.HeaderText == "Eliminar")
            {
                if (MessageBox.Show("¿Eliminar mascota?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _crud.editarBD("DELETE FROM Mascota WHERE IdMascota=@id;", new SqlParameter("@id", id));
                    RefrescarMascotas();
                }
            }
        }
    }
}
