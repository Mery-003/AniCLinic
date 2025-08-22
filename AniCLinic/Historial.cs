using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Historial : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        public Historial()
        {
            InitializeComponent();
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            if (dgvDatos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una fila.", "Aviso");
                return;
            }

            int idRegistro = Convert.ToInt32(
                dgvDatos.CurrentRow.Cells["IdRegistroClinico"].Value);

            var frm = new VerHistorial(idRegistro);
            frm.Show();   // o ShowDialog();
        }

        private void Historial_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }
        private void CargarDatos()
        {
            using (SqlConnection con = _db.AbrirConexion())
            {
                string query = @"
                    SELECT IdRegistroClinico, IdMascota, IdVeterinario, 
                           MotivoConsulta, Diagnostico, Tratamiento, 
                           AplicacionTratamiento, FechaRegistro
                    FROM RegistroClinico";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvDatos.DataSource = dt;

                // Agregar columna de botón "Ver Informe" si no existe
                if (!dgvDatos.Columns.Contains("VerInforme"))
                {
                    DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                    btn.HeaderText = "Acciones";
                    btn.Name = "VerInforme";
                    btn.Text = "Ver Informe";
                    btn.UseColumnTextForButtonValue = true;
                    dgvDatos.Columns.Add(btn);
                }
            }
        }

        private void dgvDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvDatos.Columns[e.ColumnIndex].Name == "VerInforme")
            {
                int idRegistro = Convert.ToInt32(
                    dgvDatos.Rows[e.RowIndex].Cells["IdRegistroClinico"].Value);

                // Abre el formulario/panel de detalle
                var frm = new VerHistorial(idRegistro);
                frm.ShowDialog();
            }
        }
    }
}
