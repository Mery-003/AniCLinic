using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fCitas : Form
    {
        public fCitas()
        {
            InitializeComponent();

            PrepararGrid();
            WireEvents();

            UxBuscarCedulaHelper.Wire(txtBuscar, ced => RecargarGrid(ced));

            RecargarGrid();
        }

        private void WireEvents()
        {
            btnNuvCita.Click -= btnNuvCita_Click;
            btnNuvCita.Click += btnNuvCita_Click;

            dgvCitas.CellContentClick -= DgvCitas_CellContentClick;
            dgvCitas.CellContentClick += DgvCitas_CellContentClick;
        }

        private void PrepararGrid()
        {
            dgvCitas.AutoGenerateColumns = false;
            dgvCitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCitas.MultiSelect = false;
            dgvCitas.AllowUserToAddRows = false;
            dgvCitas.Columns.Clear();

            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colIdCita",
                HeaderText = "Id",
                DataPropertyName = "IdCita",
                Width = 60,
                ReadOnly = true
            });

            dgvCitas.Columns.Add(MkText("Mascota", "Mascota", 120));
            dgvCitas.Columns.Add(MkText("Especie", "Especie", 100));
            dgvCitas.Columns.Add(MkText("Raza", "Raza", 120));
            dgvCitas.Columns.Add(MkText("Fecha", "Fecha", 90));
            dgvCitas.Columns.Add(MkText("Hora", "Hora", 70));
            dgvCitas.Columns.Add(MkText("Motivo", "Motivo", 220));
            dgvCitas.Columns.Add(MkText("Propietario", "Propietario", 160));
            dgvCitas.Columns.Add(MkText("Veterinario", "Veterinario", 140));

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
        }

        private DataGridViewTextBoxColumn MkText(string header, string prop, int width)
            => new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = true
            };

        private void RecargarGrid(string cedulaFiltro = null)
        {
            dgvCitas.DataSource = CedulaUtils.CitasListado(cedulaFiltro);
        }

        private void btnNuvCita_Click(object sender, EventArgs e)
        {
            using (var frm = new AggCita())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    var filtro = (txtBuscar.Text?.Trim().Length == 10) ? txtBuscar.Text.Trim() : null;
                    RecargarGrid(filtro);
                }
            }
        }

        private void DgvCitas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var col = dgvCitas.Columns[e.ColumnIndex].Name;
            var id = Convert.ToInt32(dgvCitas.Rows[e.RowIndex].Cells["colIdCita"].Value);

            if (col == "colEditar")
            {
                using (var frm = new AggCita(id))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        var filtro = (txtBuscar.Text?.Trim().Length == 10) ? txtBuscar.Text.Trim() : null;
                        RecargarGrid(filtro);
                    }
                }
            }
            else if (col == "colEliminar")
            {
                if (MessageBox.Show("¿Eliminar esta cita?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    EliminarCita(id);
                    var filtro = (txtBuscar.Text?.Trim().Length == 10) ? txtBuscar.Text.Trim() : null;
                    RecargarGrid(filtro);
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
                    "DELETE FROM dbo.GestionCita WHERE IdCita=@id;", db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            finally { db.cerrarConexion(); }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }
    }
}
