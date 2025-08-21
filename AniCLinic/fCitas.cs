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
    public partial class fCitas : Form
    {
        public fCitas()
        {
            InitializeComponent();
        }
        private readonly ConexionBD _db = new ConexionBD();
        private bool _accionesAgregadas = false;
        private void btnNuvCita_Click(object sender, EventArgs e)
        {
            AggCita frm = new AggCita();
            frm.Show();
        }

        private void fCitas_Load(object sender, EventArgs e)
        {
            CargarCitas();
        }

        private void CargarCitas()
        {
            // Trae lo esencial y deja IDs ocultos para usarlos después
            string sql = @"
                SELECT
                    gc.IdCita,
                    gc.IdPropietario,
                    gc.IdMascota,
                    gc.IdEmpleado,
                    m.Nombre AS Paciente,
                    m.Especie,
                    m.Raza,
                    CAST(gc.FechaHora AS date) AS Fecha,
                    CONVERT(varchar(5), gc.FechaHora, 108) AS Hora,
                    gc.Motivo,
                    gc.Estado
                FROM dbo.GestionCita gc
                INNER JOIN dbo.Mascota m      ON m.IdMascota = gc.IdMascota
                -- (Opcional para mostrar el nombre del vet):
                -- INNER JOIN dbo.Empleado e     ON e.IdEmpleado = gc.IdEmpleado
                -- INNER JOIN dbo.Persona pv     ON pv.IdPersona = e.IdPersona
                ORDER BY gc.FechaHora DESC;";

            try
            {
                var cn = _db.AbrirConexion();
                using (var da = new SqlDataAdapter(sql, cn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    dgvCita.AutoGenerateColumns = true;
                    dgvCita.Columns.Clear();           // por si definiste columnas en el diseñador
                    dgvCita.DataSource = dt;

                    // Ocultar IDs
                    OcultarCol("IdCita");
                    OcultarCol("IdPropietario");
                    OcultarCol("IdMascota");
                    OcultarCol("IdEmpleado");

                    // Encabezados amigables
                    Renombrar("Paciente", "Paciente");
                    Renombrar("Especie", "Especie");
                    Renombrar("Raza", "Raza");
                    Renombrar("Fecha", "Fecha");
                    Renombrar("Hora", "Hora");
                    Renombrar("Motivo", "Motivo");
                    Renombrar("Estado", "Estado");

                    // Presentación
                    dgvCita.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvCita.ReadOnly = true;
                    dgvCita.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvCita.MultiSelect = false;

                    // Columnas botón (una sola vez)
                    if (!_accionesAgregadas)
                    {
                        var colEditar = new DataGridViewButtonColumn
                        {
                            Name = "Editar",
                            HeaderText = "",
                            Text = "Editar",
                            UseColumnTextForButtonValue = true,
                            Width = 70
                        };
                        var colEliminar = new DataGridViewButtonColumn
                        {
                            Name = "Eliminar",
                            HeaderText = "",
                            Text = "Eliminar",
                            UseColumnTextForButtonValue = true,
                            Width = 80
                        };
                        dgvCita.Columns.Add(colEditar);
                        dgvCita.Columns.Add(colEliminar);
                        _accionesAgregadas = true;

                        // ✋ AÚN SIN LÓGICA: no enganchamos ningún evento aquí.
                        // Cuando quieras darle funcionalidad:
                        // dgvCita.CellContentClick += dgvCita_CellContentClick;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar citas: " + ex.Message);
            }
            finally
            {
                _db.CerrarConexion();
            }
        }

        private void OcultarCol(string name)
        {
            if (dgvCita.Columns.Contains(name))
                dgvCita.Columns[name].Visible = false;
        }

        private void Renombrar(string name, string header)
        {
            if (dgvCita.Columns.Contains(name))
                dgvCita.Columns[name].HeaderText = header;
        }

        private void dgvCita_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var col = dgvCita.Columns[e.ColumnIndex].Name;
            var row = dgvCita.Rows[e.RowIndex];
            int idCita = Convert.ToInt32(row.Cells["IdCita"].Value);

            if (col == "Editar")
            {
                // Reusar el mismo formulario, en modo edición
                using (var f = new AggCita(idCita))
                {
                    if (f.ShowDialog() == DialogResult.OK)
                        CargarCitas();
                }
            }
            else if (col == "Eliminar")
            {
                if (MessageBox.Show("¿Eliminar la cita seleccionada?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    EliminarCita(idCita);
                    CargarCitas();
                }
            }
        }

        private void EliminarCita(int idCita)
        {
            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand("DELETE FROM GestionCita WHERE IdCita=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Cita eliminada.");
            }
            catch (Exception ex) { MessageBox.Show("No se pudo eliminar: " + ex.Message); }
            finally { _db.CerrarConexion(); }
        }
        
    }
}
