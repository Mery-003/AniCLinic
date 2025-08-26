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
        private bool _accionesAgregadas = false;

        public fPacientes()
        {
            InitializeComponent();
        }

        private void fPacientes_Load(object sender, EventArgs e)
        {
            CargarMascotasConDueno();
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e) { }
        private void btnAgregar_Click(object sender, EventArgs e) { }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            var frm = new AgregarPaciente();
            frm.Show();
        }

        private void CargarMascotasConDueno()
        {
            string sql = @"
                SELECT
                    m.IdMascota,
                    pr.IdPropietario,
                    m.Nombre AS Mascota,
                    m.Especie,
                    m.Sexo,
                    m.EdadTexto AS Edad,  -- 👈 usa la columna calculada

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


            try
            {
                var cn = _db.AbrirConexion();
                using (var da = new SqlDataAdapter(sql, cn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    dgvDatos.AutoGenerateColumns = true;
                    dgvDatos.DataSource = dt;

                    if (dgvDatos.Columns.Contains("Dueno"))
                        dgvDatos.Columns["Dueno"].HeaderText = "Dueño";

                    if (dgvDatos.Columns.Contains("Edad"))
                        dgvDatos.Columns["Edad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


                    // 🔒 Oculta IDs
                    if (dgvDatos.Columns.Contains("IdMascota"))
                        dgvDatos.Columns["IdMascota"].Visible = true;   // <- corregido (antes estaba en true)
                    if (dgvDatos.Columns.Contains("IdPropietario"))
                        dgvDatos.Columns["IdPropietario"].Visible = false;

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

        private void dgvDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // encabezado
            var col = dgvDatos.Columns[e.ColumnIndex].Name;

            var row = dgvDatos.Rows[e.RowIndex];
            int idMascota = Convert.ToInt32(row.Cells["IdMascota"].Value);
            int idPropietario = Convert.ToInt32(row.Cells["IdPropietario"].Value);

            if (col == "Editar")
            {
                using (var f = new AgregarPaciente(idPropietario, idMascota)) // ctor que recibe IDs
                {
                    if (f.ShowDialog() == DialogResult.OK)
                        CargarMascotasConDueno();
                }
            }
            else if (col == "Eliminar")
            {
                // 1) Verificar si la mascota tiene citas
                bool tieneCitas = MascotaTieneCitas(idMascota);

                if (tieneCitas)
                {
                    // Advertencia adicional por citas
                    var rCitas = MessageBox.Show(
                        "⚠ Esta mascota tiene citas registradas.\n\n" +
                        "Al eliminarla, también se eliminarán sus citas.\n\n" +
                        "¿Desea continuar?",
                        "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (rCitas == DialogResult.No) return;
                }

                // 2) Advertencia general (mascota + posible propietario)
                var rGeneral = MessageBox.Show(
                    "¿Eliminar esta mascota?\n\n" +
                    "Si es la única mascota del dueño, también se eliminará el propietario.",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (rGeneral == DialogResult.Yes)
                {
                    if (EliminarMascotaYPropietarioSiCorresponde(idPropietario, idMascota))
                        CargarMascotasConDueno();
                }
            }
        }

        /// <summary>
        /// Valida si la mascota tiene citas. 
        /// Nota: Usamos la tabla GestionCita porque tu eliminación borra ahí.
        /// </summary>
        private bool MascotaTieneCitas(int idMascota)
        {
            using (var conn = new SqlConnection(_db.CadenaConexion))
            {
                conn.Open();
                const string query = "SELECT COUNT(*) FROM GestionCita WHERE IdMascota = @idMascota";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idMascota", idMascota);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Elimina en transacción: RegistroClinico → GestionCita → PropietarioMascota → Mascota (si queda sin dueños)
        /// y Propietario (si queda sin mascotas) → Persona (si no es Empleado).
        /// </summary>
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

                // 4) ¿La mascota quedó sin dueños? Si sí, eliminar Mascota
                int duenosRestantes;
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM PropietarioMascota WHERE IdMascota=@m", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@m", idMascota);
                    duenosRestantes = (int)cmd.ExecuteScalar();
                }
                if (duenosRestantes == 0)
                {
                    using (var cmd = new SqlCommand(
                        "DELETE FROM Mascota WHERE IdMascota=@m", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@m", idMascota);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 5) Si el propietario no tenía más mascotas, eliminar Propietario y quizá Persona
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

        private void guna2GradientButton1_Click(object sender, EventArgs e) { }
        private void eliminarpaciente_Click(object sender, EventArgs e) { }
    }
}
