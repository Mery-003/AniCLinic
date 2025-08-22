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
    public partial class VerHistorial : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        private readonly int _idRegistro;
        public VerHistorial(int idRegistro)
        {
            InitializeComponent();
            _idRegistro = idRegistro;
        }


        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VerHistorial_Load(object sender, EventArgs e)
        {
            CargarDetalle();
        }
        private void CargarDetalle()
        {
            const string sql = @"
                SELECT 
                    rc.FechaRegistro,
                    m.Nombre AS Paciente,
                    rc.MotivoConsulta,
                    rc.Diagnostico,
                    rc.Tratamiento,
                    rc.AplicacionTratamiento,
                    CONCAT(p.Nombre, ' ', p.Apellido) AS Veterinario
                FROM dbo.RegistroClinico rc
                INNER JOIN dbo.Mascota m       ON m.IdMascota = rc.IdMascota
                INNER JOIN dbo.Empleado emp    ON emp.IdEmpleado = rc.IdVeterinario
                INNER JOIN dbo.Persona  p      ON p.IdPersona  = emp.IdPersona
                WHERE rc.IdRegistroClinico = @id;
            ";

            using (SqlConnection con = _db.AbrirConexion())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", _idRegistro);

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        MessageBox.Show("No se encontró el registro clínico.", "Aviso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                        return;
                    }

                    // Asignaciones (manejo de posibles NULL)
                    lblFecha.Text = dr["FechaRegistro"] is DBNull
                                          ? "-"
                                          : Convert.ToDateTime(dr["FechaRegistro"]).ToString("dd/MM/yyyy HH:mm");

                    lblPaciente.Text = dr["Paciente"]?.ToString() ?? "-";
                    txtMotivo.Text = dr["MotivoConsulta"]?.ToString() ?? "";
                    txtDiagnostico.Text = dr["Diagnostico"]?.ToString() ?? "";
                    txtTratamiento.Text = dr["Tratamiento"]?.ToString() ?? "";
                    txtReceta.Text = dr["AplicacionTratamiento"]?.ToString() ?? "";
                    lblVeterinario.Text = dr["Veterinario"]?.ToString() ?? "-";
                }
            }
        }
    }
}
