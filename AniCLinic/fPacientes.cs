using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fPacientes : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        public fPacientes()
        {
            InitializeComponent();
            //csConexionBD conexionBD = new csConexionBD();
            //dgvPacientes.DataSource = conexionBD.retornaRegistro("Select * from tblMascotas");
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            AgregarPaciente frm = new AgregarPaciente();
            frm.Show();
        }

        private void fPacientes_Load(object sender, EventArgs e)
        {
            CargarMascotasConDueno();
        }

        private bool _accionesAgregadas = false;

        private void CargarMascotasConDueno()
        {
            string sql = @"
            SELECT
                m.IdMascota,
                pr.IdPropietario,
                m.Nombre      AS Mascota,
                m.Especie,
                m.Sexo,
                m.EdadAnios   AS Edad,
                CONCAT(pe.Nombre, ' ', pe.Apellido) AS Dueno,
                pe.Celular
            FROM dbo.Mascota m
            INNER JOIN dbo.PropietarioMascota pm
                    ON pm.IdMascota = m.IdMascota AND pm.EsPrincipal = 1
            INNER JOIN dbo.Propietario pr
                    ON pr.IdPropietario = pm.IdPropietario
            INNER JOIN dbo.Persona pe
                    ON pe.IdPersona = pr.IdPersona
            ORDER BY m.Nombre;";

            try
            {
                var cn = _db.AbrirConexion();
                using (var da = new SqlDataAdapter(sql, cn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    dgvDatos.AutoGenerateColumns = true;
                    dgvDatos.DataSource = dt;

                    // Oculta IDs
                    if (dgvDatos.Columns.Contains("IdMascota")) dgvDatos.Columns["IdMascota"].Visible = false;
                    if (dgvDatos.Columns.Contains("IdPropietario")) dgvDatos.Columns["IdPropietario"].Visible = false;

                    dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvDatos.ReadOnly = true;
                    dgvDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvDatos.MultiSelect = false;

                    // Agregar columnas de acción solo una vez
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
                        dgvDatos.Columns.Add(colEditar);
                        dgvDatos.Columns.Add(colEliminar);
                        _accionesAgregadas = true;

                        dgvDatos.CellContentClick -= dgvDatos_CellContentClick;
                        dgvDatos.CellContentClick += dgvDatos_CellContentClick;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar mascotas: " + ex.Message);
            }
            finally
            {
                _db.CerrarConexion();
            }
        

        }


        private void guna2GradientButton1_Click(object sender, EventArgs e) { }
        private void eliminarpaciente_Click(object sender, EventArgs e) { }

        private void dgvDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // encabezado
            var col = dgvDatos.Columns[e.ColumnIndex].Name;

            var row = dgvDatos.Rows[e.RowIndex];
            int idMascota = Convert.ToInt32(row.Cells["IdMascota"].Value);
            int idPropietario = Convert.ToInt32(row.Cells["IdPropietario"].Value);

            if (col == "Editar")
            {
                using (var f = new AgregarPaciente(idPropietario, idMascota)) // <- nuevo ctor
                {
                    if (f.ShowDialog() == DialogResult.OK)
                        CargarMascotasConDueno();
                }
            }
            else if (col == "Eliminar")
            {
                if (MessageBox.Show("¿Eliminar esta mascota?\n\nSi es la única mascota del dueño, también se eliminará el propietario.",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (EliminarMascotaYPropietarioSiCorresponde(idPropietario, idMascota))
                        CargarMascotasConDueno();
                }
            }
        }

        private bool EliminarMascotaYPropietarioSiCorresponde(int idPropietario, int idMascota)
        {
            var cn = _db.AbrirConexion();
            SqlTransaction tx = cn.BeginTransaction();
            try
            {
                // ¿Cuántas mascotas tiene el propietario?
                int totalMascotasProp;
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM PropietarioMascota WHERE IdPropietario=@p", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@p", idPropietario);
                    totalMascotasProp = (int)cmd.ExecuteScalar();
                }

                // IdPersona del propietario (para potencial borrado al final)
                int idPersona;
                using (var cmd = new SqlCommand(
                    "SELECT IdPersona FROM Propietario WHERE IdPropietario=@p", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@p", idPropietario);
                    idPersona = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 1) Registros clínicos de la mascota
                using (var cmd = new SqlCommand(
                    "DELETE FROM RegistroClinico WHERE IdMascota=@m", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@m", idMascota);
                    cmd.ExecuteNonQuery();
                }

                // 2) Citas del par (propietario, mascota)
                using (var cmd = new SqlCommand(
                    "DELETE FROM GestionCita WHERE IdPropietario=@p AND IdMascota=@m", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@p", idPropietario);
                    cmd.Parameters.AddWithValue("@m", idMascota);
                    cmd.ExecuteNonQuery();
                }

                // 3) Quitar vínculo del propietario con esa mascota
                using (var cmd = new SqlCommand(
                    "DELETE FROM PropietarioMascota WHERE IdPropietario=@p AND IdMascota=@m", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@p", idPropietario);
                    cmd.Parameters.AddWithValue("@m", idMascota);
                    cmd.ExecuteNonQuery();
                }

                // 4) ¿La mascota quedó sin dueños? Si sí, eliminar mascota.
                int dueñosRestantes;
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM PropietarioMascota WHERE IdMascota=@m", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@m", idMascota);
                    dueñosRestantes = (int)cmd.ExecuteScalar();
                }
                if (dueñosRestantes == 0)
                {
                    using (var cmd = new SqlCommand(
                        "DELETE FROM Mascota WHERE IdMascota=@m", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@m", idMascota);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 5) Si el propietario no tenía más mascotas, eliminar Propietario
                if (totalMascotasProp == 1)
                {
                    using (var cmd = new SqlCommand(
                        "DELETE FROM Propietario WHERE IdPropietario=@p", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@p", idPropietario);
                        cmd.ExecuteNonQuery();
                    }

                    // Si la Persona NO es empleado, eliminar Persona
                    int esEmpleado;
                    using (var cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Empleado WHERE IdPersona=@idPer", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@idPer", idPersona);
                        esEmpleado = (int)cmd.ExecuteScalar();
                    }
                    if (esEmpleado == 0)
                    {
                        using (var cmd = new SqlCommand(
                            "DELETE FROM Persona WHERE IdPersona=@idPer", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@idPer", idPersona);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                tx.Commit();
                MessageBox.Show("Eliminado correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }
                MessageBox.Show("No se pudo eliminar: " + ex.Message);
                return false;
            }
            finally
            {
                _db.CerrarConexion();
            }
        }

    }
}
