using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Historial : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        private bool _yaCargue = false;

        public Historial()
        {
            InitializeComponent();

            // Config del grid
            dgvDatos.AllowUserToAddRows = false;
            dgvDatos.MultiSelect = false;
            dgvDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDatos.ReadOnly = true;
            dgvDatos.RowHeadersVisible = false;
            dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Eventos
            this.Load += Historial_Load;
            this.Shown += (s, e) => { if (!_yaCargue) { _yaCargue = true; CargarMascotas(); } };
            btnAggPaciente.Click += btnAggPaciente_Click;

            dgvDatos.CellContentClick += (s, e) => { /* sin acciones */ };
        }

        private void Historial_Load(object sender, EventArgs e)
        {
            if (!_yaCargue)
            {
                _yaCargue = true;
                CargarMascotas();
            }
        }

        /// Llena el grid como FPacientes (sin botones Editar/Eliminar)
        private void CargarMascotas()
        {
            var cn = _db.AbrirConexion();
            try
            {
                var dt = new DataTable();

                string sql = @"
SELECT
    m.IdMascota,
    pr.IdPropietario,
    m.Nombre AS Mascota,
    m.Especie,
    m.Sexo,
    m.EdadTexto AS Edad,
    CASE 
        WHEN LTRIM(RTRIM(pe.Apellido)) IN ('', '(s/n)', 's/n', 'S/N') 
             THEN pe.Nombre
        ELSE CONCAT(pe.Nombre, ' ', pe.Apellido)
    END AS Dueno,
    pe.Celular
FROM dbo.Mascota m
INNER JOIN dbo.PropietarioMascota pm
        ON pm.IdMascota = m.IdMascota AND pm.EsPrincipal = 1
INNER JOIN dbo.Propietario pr
        ON pr.IdPropietario = pm.IdPropietario
INNER JOIN dbo.Persona pe
        ON pe.IdPersona = pr.IdPersona
ORDER BY m.Nombre;";

                using (var da = new SqlDataAdapter(sql, cn))
                {
                    da.Fill(dt);
                }

                dgvDatos.Columns.Clear();
                dgvDatos.DataSource = dt;

                Renombrar("IdMascota", "ID Mascota");
                Renombrar("Mascota", "Mascota");
                Renombrar("Especie", "Especie");
                Renombrar("Sexo", "Sexo");
                Renombrar("Edad", "Edad");
                Renombrar("Dueno", "Dueño");
                Renombrar("Celular", "Celular");

                if (dgvDatos.Columns.Contains("IdPropietario"))
                    dgvDatos.Columns["IdPropietario"].Visible = false;

                foreach (DataGridViewColumn c in dgvDatos.Columns)
                    c.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar mascotas: " + ex.Message, "Historial",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _db.CerrarConexion();
            }
        }

        private void Renombrar(string name, string header)
        {
            if (dgvDatos.Columns.Contains(name))
                dgvDatos.Columns[name].HeaderText = header;
        }

        /// Botón “Ver Historial”: abrir VerHistorial en modo navegable por mascota
        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            if (dgvDatos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una mascota de la lista.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idMascota = Convert.ToInt32(dgvDatos.CurrentRow.Cells["IdMascota"].Value);

            using (var frm = new VerHistorial(idMascota, listarPorMascota: true))
            {
                frm.ShowDialog();
            }
        }

        // Requerido por el diseñador (sin uso)
        private void dgvDatos_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}
