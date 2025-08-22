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
    public partial class Carnet : Form
    {
        private readonly ConexionBD _db = new ConexionBD();
        public Carnet()
        {
            InitializeComponent();
        }

        private void Carnet_Load(object sender, EventArgs e)
        {
            CargarCarnets();
            dgvCarnet.CellClick += dgvCarnet_CellClick; // engancha el evento
        }

        private void CargarCarnets()
        {
            using (SqlConnection con = _db.AbrirConexion())
            {
                // Mascota + Propietario principal (si hay)
                string sql = @"
                    SELECT 
                        m.IdMascota,
                        m.Nombre       AS Mascota,
                        m.Especie,
                        m.Raza,
                        m.Sexo,
                        m.PesoKg,
                        m.EdadAnios,
                        CONCAT(p.Nombre, ' ', p.Apellido) AS Propietario
                    FROM dbo.Mascota m
                    OUTER APPLY (
                        SELECT TOP 1 prp.IdPropietario
                        FROM dbo.PropietarioMascota pm
                        INNER JOIN dbo.Propietario prp ON prp.IdPropietario = pm.IdPropietario
                        WHERE pm.IdMascota = m.IdMascota
                        ORDER BY pm.EsPrincipal DESC
                    ) ap
                    LEFT JOIN dbo.Propietario pr ON pr.IdPropietario = ap.IdPropietario
                    LEFT JOIN dbo.Persona p      ON p.IdPersona = pr.IdPersona
                    ORDER BY m.Nombre;
                ";

                var da = new SqlDataAdapter(sql, con);
                var dt = new DataTable();
                da.Fill(dt);

                dgvCarnet.AutoGenerateColumns = true;
                dgvCarnet.DataSource = dt;

                // Botón Ver Carnet (solo una vez)
                if (!dgvCarnet.Columns.Contains("VerCarnet"))
                {
                    var btn = new DataGridViewButtonColumn
                    {
                        Name = "VerCarnet",
                        HeaderText = "Acciones",
                        Text = "Ver Carnet",
                        UseColumnTextForButtonValue = true
                    };
                    dgvCarnet.Columns.Add(btn);
                }

                // Opcional: ocultar Id
                if (dgvCarnet.Columns.Contains("IdMascota"))
                    dgvCarnet.Columns["IdMascota"].Visible = false;
            }
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {

        }

        private void dgvPacientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCarnet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvCarnet.Columns[e.ColumnIndex].Name == "VerCarnet")
            {
                int idMascota = Convert.ToInt32(
                    dgvCarnet.Rows[e.RowIndex].Cells["IdMascota"].Value);

                var frm = new VerCarnet(idMascota);   // detalle
                frm.ShowDialog();
            }
        }
    }
}
