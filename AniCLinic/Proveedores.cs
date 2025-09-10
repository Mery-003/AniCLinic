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
    public partial class Proveedores : Form
    {
        csCRUD crud = new csCRUD();
        bool botonesAgregados = false;
        public Proveedores()
        {
            InitializeComponent();
            CargarDataP();
            prepararGrid();
        }
        public void CargarDataP(string filtro = "")
        {
            string sentencia = "Select * from Proveedor " +
                "Where NombreProveedor like (@Filtro + '%') or RUC like (@Filtro + '%')";
            dgvProveedores.DataSource = crud.cargarBDData(sentencia, new SqlParameter("@Filtro", filtro) );
            configurarColumnas();
        }
        public void configurarColumnas()
        {
            var g = dgvProveedores;

            if (g.Columns.Contains("IdProveedor"))
            {
                g.Columns["IdProveedor"].Width = 60;
                g.Columns["IdProveedor"].DisplayIndex = 0;
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
            dgvProveedores.ReadOnly = true;
            dgvProveedores.MultiSelect = false;
            dgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.RowHeadersVisible = false;
            dgvProveedores.AllowUserToAddRows = false;
            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedores.AutoGenerateColumns = true;

        }
        private void btnNvProveedor_Click(object sender, EventArgs e)
        {
            AggProveedor nProveedor = new AggProveedor();
            nProveedor.ShowDialog();
            CargarDataP();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarDataP(txtBuscar.Text);
        }

        private void dgvProveedores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var colNombre = dgvProveedores.Columns[e.ColumnIndex].Name;
            if (colNombre != "Editar" && colNombre != "Eliminar")
                return;

            var rowView = dgvProveedores.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null)
                return;
                
            int idProveedor = Convert.ToInt32(rowView["IdProveedor"]);

            if (colNombre == "Eliminar")
            {
                var ok = MessageBox.Show("¿Desea eliminar el proveedor?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (ok == DialogResult.Yes)
                {
                    if(crud.eliminarBD("Delete from Proveedor Where IdProveedor = @Id", idProveedor))
                    {
                        MessageBox.Show("Proveedor eliminado correctamente");
                        CargarDataP();
                    }
                }
            }
            else
            {
                AggProveedor eProveedor = new AggProveedor(this, idProveedor);
                eProveedor.ShowDialog();
                CargarDataP();
            }
        }
    }
}
