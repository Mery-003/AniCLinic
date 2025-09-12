using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fPacientes : Form
    {
        private readonly csCRUD _crud = new csCRUD();
        private bool _botonesAgregados = false;

        public fPacientes()
        {
            InitializeComponent();
            PrepararGrid();
            CargarData();  
        }

        private void PrepararGrid()
        {
            var g = dgvPacientes;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;  
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AutoGenerateColumns = true;
        }

        private void CargarData(string filtro = "")
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
WHERE (@Filtro ='' OR P.Cedula LIKE @Filtro + '%' OR P.Nombre LIKE @Filtro + '%' 
OR M.Nombre LIKE @Filtro + '%')
ORDER BY M.IdMascota DESC;";

            dgvPacientes.DataSource = _crud.cargarBDData(sql, new SqlParameter("@Filtro", filtro));

            ConfigurarColumnas();
        }

        private void ConfigurarColumnas()
        {
            var g = dgvPacientes;

            if (g.Columns.Contains("ID"))
            {
                g.Columns["ID"].Width = 60;
                g.Columns["ID"].DisplayIndex = 0;
            }

            if (!_botonesAgregados)
            {
                var colEditar = new DataGridViewButtonColumn
                {
                    Name = "Editar",
                    HeaderText = "",  
                    Text = "Editar",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                };
                g.Columns.Add(colEditar);

                var colEliminar = new DataGridViewButtonColumn
                {
                    Name = "Eliminar",
                    HeaderText = "",
                    Text = "Eliminar",
                    UseColumnTextForButtonValue = true,
                    Width = 90
                };
                g.Columns.Add(colEliminar);

                _botonesAgregados = true;
            }
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            var f = new AgregarPaciente(this);
            f.ShowDialog();
            CargarData((txtMascotaNombre.Text ?? "").Trim());
        }

        private void dgvPacientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) 
                return;

            var nombreCol = dgvPacientes.Columns[e.ColumnIndex].Name;
            if (nombreCol != "Editar" && nombreCol != "Eliminar") 
                return;

            var rowView = dgvPacientes.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) 
                return;

            int idMascota = Convert.ToInt32(rowView["ID"]);

            if (nombreCol == "Eliminar")
            {
                var ok = MessageBox.Show("¿Eliminar la mascota seleccionada?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (ok == DialogResult.Yes)
                {
                    if (_crud.eliminarBD("DELETE FROM Mascota WHERE IdMascota=@id", idMascota))
                    {
                        MessageBox.Show("Mascota eliminada correctamente.");
                        CargarData((txtMascotaNombre.Text ?? "").Trim());
                    }
                }
            }
            else 
            {
                var f = new AgregarPaciente(this, idMascota);
                f.ShowDialog();
                CargarData((txtMascotaNombre.Text).Trim());
            }
        }

        private void txtMascotaNombre_TextChanged_1(object sender, EventArgs e)
        {
            string filtro = txtMascotaNombre.Text;
            CargarData(filtro);
        }
    }
}
