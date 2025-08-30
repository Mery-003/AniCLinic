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
            CargarData();               // carga inicial (sin filtro)
            WireEvents();               // engancha eventos una sola vez
        }

        private void WireEvents()
        {
            // evita doble enganche
            dgvPacientes.CellContentClick -= dgvPacientes_CellContentClick;
            dgvPacientes.CellContentClick += dgvPacientes_CellContentClick;

            txtMascotaNombre.TextChanged -= txtMascotaNombre_TextChanged;
            txtMascotaNombre.TextChanged += txtMascotaNombre_TextChanged;

            btnAggPaciente.Click -= btnAggPaciente_Click;
            btnAggPaciente.Click += btnAggPaciente_Click;
        }

        private void PrepararGrid()
        {
            var g = dgvPacientes;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;           // <- quita la fila invisible
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AutoGenerateColumns = true;           // usamos alias en el SQL
        }

        private void CargarData(string cedulaFiltro = "")
        {
            // JOIN + alias "ID" y columna Propietario
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
WHERE (@Cedula='' OR P.Cedula LIKE @Cedula + '%')
ORDER BY M.IdMascota DESC;";

            dgvPacientes.DataSource = _crud.cargarBDData(sql, new SqlParameter("@Cedula", cedulaFiltro ?? ""));

            ConfigurarColumnas(); // agregar botones y pequeños ajustes
        }

        private void ConfigurarColumnas()
        {
            var g = dgvPacientes;

            // asegura ancho de ID y orden
            if (g.Columns.Contains("ID"))
            {
                g.Columns["ID"].Width = 60;
                g.Columns["ID"].DisplayIndex = 0;
            }

            // agrega botones solo una vez
            if (!_botonesAgregados)
            {
                var colEditar = new DataGridViewButtonColumn
                {
                    Name = "Editar",
                    HeaderText = "",                 // sin letras
                    Text = "Editar",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                };
                g.Columns.Add(colEditar);

                var colEliminar = new DataGridViewButtonColumn
                {
                    Name = "Eliminar",
                    HeaderText = "",                 // sin letras
                    Text = "Eliminar",
                    UseColumnTextForButtonValue = true,
                    Width = 90
                };
                g.Columns.Add(colEliminar);

                _botonesAgregados = true;
            }
        }

        private void txtMascotaNombre_TextChanged(object sender, EventArgs e)
        {
            // filtra por CÉDULA del propietario (prefijo)
            var filtro = (txtMascotaNombre.Text ?? string.Empty).Trim();
            CargarData(filtro);
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            var f = new AgregarPaciente(this);
            f.ShowDialog();
            // refresca al cerrar
            CargarData((txtMascotaNombre.Text ?? "").Trim());
        }

        private void dgvPacientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var nombreCol = dgvPacientes.Columns[e.ColumnIndex].Name;
            if (nombreCol != "Editar" && nombreCol != "Eliminar") return;

            // fila actual -> DataRow
            var rowView = dgvPacientes.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;

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
            else // Editar
            {
                // Abrir formulario de edición con TODOS los campos cargados
                var f = new AgregarPaciente(this, idMascota);
                f.ShowDialog();
                CargarData((txtMascotaNombre.Text ?? "").Trim());
            }
        }
    }
}
