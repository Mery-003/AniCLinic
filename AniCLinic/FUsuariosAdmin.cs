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
    public partial class FUsuariosAdmin : Form
    {
        csCRUD crud = new csCRUD();
        bool botonesAgregados = false;
        public FUsuariosAdmin()
        {
            InitializeComponent();
            prepararGrid();
            cargarDataU();
        }
        public void cargarDataU(string filtro = "")
        {
            string sentencia = "Select E.IdEmpleado, (P.Nombre + ' ' + P.Apellido) as Nombre, P.Cedula ,E.Usuario, " +
                "P.Correo, P.DireccionDomiciliaria, Case Administrador when 1 then 'Si' else 'no' end as Administrador " +
                "from Empleados E inner join Persona P on E.IdPersona=P.IdPersona " +
                "Where P.Nombre like (@filtro + '%') or P.Cedula like (@filtro + '%')";
            dgvUsuarios.DataSource = crud.cargarBDData(sentencia, new SqlParameter("@filtro", filtro));
            configurarColumnas();
        }
        public void configurarColumnas()
        {
            var g = dgvUsuarios;

            if (g.Columns.Contains("IdEmpleado"))
            {
                g.Columns["IdEmpleado"].Width = 60;
                g.Columns["IdEmpleado"].DisplayIndex = 0;
            }

            if (!botonesAgregados)
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
                    Width = 80
                };
                g.Columns.Add(colEliminar);

                botonesAgregados = true;
            }
        }
        public void prepararGrid()
        {
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.AutoGenerateColumns = true;
        }

        private void dgvUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var nombreCol = dgvUsuarios.Columns[e.ColumnIndex].Name;
            if (nombreCol != "Editar" && nombreCol != "Eliminar")
                return;

            var rowView = dgvUsuarios.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null)
                return;
            int idEmpleado = Convert.ToInt32(rowView["IdEmpleado"]);

            if (nombreCol == "Eliminar")
            {
                var ok = MessageBox.Show("¿Desea eliminar el Empleado?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (ok == DialogResult.Yes)
                {
                    if (crud.eliminarBD("Delete from Empleados Where IdEmpleado = @Id", idEmpleado))
                    {
                        MessageBox.Show("Empleado eliminado correctamente");
                        cargarDataU();
                    }
                }
            }
            else
            {
                DatosUsuario usuario = new DatosUsuario(this, idEmpleado);
                usuario.ShowDialog();
                cargarDataU();
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            cargarDataU(txtBuscar.Text);
        }

        private void btnAggEmpleado_Click(object sender, EventArgs e)
        {
            DatosUsuario usuario = new DatosUsuario();
            usuario.ShowDialog();
            cargarDataU();
        }
    }
}
