using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Inventario : Form
    {
        csCRUD crud = new csCRUD();
        bool botonesAgregados = false;

        public Inventario()
        {
            InitializeComponent();
            cargarDataI();
            prepararGrid();
        }
        public void cargarDataI(string filtro = "")
        {
            string sentencia = "Select * from Inventario " +
                "Where NombreProducto like @filtro + '%'";
            dgvInventario.DataSource = crud.cargarBDData(sentencia, new SqlParameter ("@filtro", filtro));
            configurarColumnas();
        }

        private void btnMasProd_Click(object sender, EventArgs e)
        {
            MasProductos mProductos = new MasProductos();
            mProductos.ShowDialog();
            cargarDataI();
        }

        private void btnAggProd_Click(object sender, EventArgs e)
        {
            NuvProducto nProducto = new NuvProducto();
            nProducto.ShowDialog();
            cargarDataI();
        }

        private void txtBuscarHoy_TextChanged(object sender, EventArgs e)
        {
            cargarDataI(txtBuscar.Text);
        }

        public void configurarColumnas()
        {
            var g = dgvInventario;

            if (g.Columns.Contains("IdProducto"))
            {
                g.Columns["IdProducto"].Width = 60;
                g.Columns["IdProducto"].DisplayIndex = 0;
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
            dgvInventario.ReadOnly = true;
            dgvInventario.MultiSelect = false;
            dgvInventario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInventario.RowHeadersVisible = false;
            dgvInventario.AllowUserToAddRows = false;
            dgvInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventario.AutoGenerateColumns = true;
        }

        private void dgvInventario_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var nombreCol = dgvInventario.Columns[e.ColumnIndex].Name;
            if (nombreCol != "Editar" && nombreCol != "Eliminar")
                return;

            var rowView = dgvInventario.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null)
                return;
            int idProd = Convert.ToInt32(rowView["IdProducto"]);

            if (nombreCol == "Eliminar")
            {
                var ok = MessageBox.Show("¿Desea eliminar el producto?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (ok == DialogResult.Yes)
                {
                    if(crud.eliminarBD("Delete from Inventario Where IdProducto = @Id", idProd))
                    {
                        MessageBox.Show("Producto eliminado correctamente");
                        cargarDataI();
                    }
                }
            }
            else
            {
                NuvProducto nProducto = new NuvProducto(this, idProd);
                nProducto.ShowDialog();
                cargarDataI();
            }
        }
    }
}
