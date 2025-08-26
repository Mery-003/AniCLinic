using System;
using System.Data;
using System.Data.SqlClient;
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

        private void btnNuvCita_Click(object sender, EventArgs e)
        {
            using (var frm = new AggCita(SesionActual.IdEmpleado, SesionActual.NombreEmpleado))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    CargarCitas();
            }
        }

        private void fCitas_Load(object sender, EventArgs e)
        {
            CargarCitas();
        }

        private void CargarCitas()
        {
            string sql = @"
                SELECT
                    c.IdCita,
                    c.IdPropietario,
                    c.IdMascota,
                    c.IdEmpleado,
                    m.Nombre AS Paciente,
                    m.Especie,
                    m.Raza,
                    CONVERT(date, c.FechaHora)            AS Fecha,
                    CONVERT(varchar(5), c.FechaHora, 108) AS Hora,
                    c.Motivo,
                    c.Estado,
                    CONCAT(pe.Nombre, ' ', pe.Apellido)   AS Propietario,
                    CONCAT(pv.Nombre, ' ', pv.Apellido)   AS Veterinario
                FROM dbo.GestionCita c
                JOIN dbo.Mascota      m   ON m.IdMascota       = c.IdMascota
                JOIN dbo.Propietario  pr  ON pr.IdPropietario  = c.IdPropietario
                JOIN dbo.Persona      pe  ON pe.IdPersona      = pr.IdPersona
                JOIN dbo.Empleado     e   ON e.IdEmpleado      = c.IdEmpleado
                JOIN dbo.Persona      pv  ON pv.IdPersona      = e.IdPersona
                ORDER BY c.FechaHora DESC;";

            try
            {
                var cn = _db.AbrirConexion();
                using (var da = new SqlDataAdapter(sql, cn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    dgvCita.AutoGenerateColumns = true;
                    dgvCita.Columns.Clear();
                    dgvCita.DataSource = dt;

                    // Ocultas
                    OcultarCol("IdCita");
                    OcultarCol("IdEmpleado");
                    OcultarCol("IdPropietario");

                    // Encabezados
                    Renombrar("IdMascota", "Id Mascota");
                    Renombrar("Paciente", "Paciente");
                    Renombrar("Especie", "Especie");
                    Renombrar("Raza", "Raza");
                    Renombrar("Fecha", "Fecha");
                    Renombrar("Hora", "Hora");
                    Renombrar("Motivo", "Motivo");
                    Renombrar("Estado", "Estado");
                    Renombrar("Propietario", "Propietario");
                    Renombrar("Veterinario", "Veterinario");

                    // Orden
                    if (dgvCita.Columns.Contains("Paciente"))
                    {
                        dgvCita.Columns["IdMascota"].DisplayIndex = 0;
                        dgvCita.Columns["Paciente"].DisplayIndex = 1;
                        dgvCita.Columns["Especie"].DisplayIndex = 2;
                        dgvCita.Columns["Raza"].DisplayIndex = 3;
                        dgvCita.Columns["Fecha"].DisplayIndex = 4;
                        dgvCita.Columns["Hora"].DisplayIndex = 5;
                        dgvCita.Columns["Motivo"].DisplayIndex = 6;
                        dgvCita.Columns["Estado"].DisplayIndex = 7;
                        dgvCita.Columns["Propietario"].DisplayIndex = 8;
                        dgvCita.Columns["Veterinario"].DisplayIndex = 9;
                    }

                    dgvCita.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvCita.ReadOnly = true;
                    dgvCita.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvCita.MultiSelect = false;

                    // Botones
                    if (!dgvCita.Columns.Contains("Editar"))
                    {
                        var colEditar = new DataGridViewButtonColumn
                        {
                            Name = "Editar",
                            HeaderText = "",
                            Text = "Editar",
                            UseColumnTextForButtonValue = true,
                            Width = 70
                        };
                        dgvCita.Columns.Add(colEditar);
                    }

                    if (!dgvCita.Columns.Contains("Eliminar"))
                    {
                        var colEliminar = new DataGridViewButtonColumn
                        {
                            Name = "Eliminar",
                            HeaderText = "",
                            Text = "Eliminar",
                            UseColumnTextForButtonValue = true,
                            Width = 80
                        };
                        dgvCita.Columns.Add(colEliminar);
                    }

                    dgvCita.Columns["Editar"].DisplayIndex = dgvCita.Columns.Count - 2;
                    dgvCita.Columns["Eliminar"].DisplayIndex = dgvCita.Columns.Count - 1;

                    dgvCita.CellContentClick -= dgvCita_CellContentClick;
                    dgvCita.CellContentClick += dgvCita_CellContentClick;
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

            int idCita;
            if (dgvCita.Columns.Contains("IdCita"))
                idCita = Convert.ToInt32(row.Cells["IdCita"].Value);
            else if (row.DataBoundItem is DataRowView drv)
                idCita = Convert.ToInt32(drv["IdCita"]);
            else
                return;

            if (col == "Editar")
            {
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
