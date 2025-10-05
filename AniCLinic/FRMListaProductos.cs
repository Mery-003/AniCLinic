using System;
using System.Data;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class FRMListaProductos : Form
    {
        private readonly csCRUD _crud = new csCRUD();

        // Lo que devuelve al llamador
        public int IdProductoSel { get; private set; }
        public string NombreSel { get; private set; }
        public decimal IvaSel { get; private set; }   // 0.00, 0.12, 0.15, etc.
        public decimal PrecioSugeridoSel { get; private set; }

        public FRMListaProductos()
        {
            InitializeComponent();
            PrepararGrid(dgvListaProductos);

            this.Load += (s, e) => CargarLista();
            txtBuscarProducto.TextChanged += (s, e) => Filtrar();
            btnAceptar.Click += btnAceptar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            dgvListaProductos.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) btnAceptar_Click(s, e); };
            this.Shown += (s, e) => txtBuscarProducto.Focus();
        }

        private static void PrepararGrid(DataGridView dgv)
        {
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoGenerateColumns = true;
        }

        private void CargarLista()
        {
            // TODOS los productos, excluyendo lo que no es producto (Cita / Servicio)
            // Si algún día agregas una columna EsProducto (bit), cambia el WHERE por: WHERE EsProducto = 1
            string sql = @"
SELECT  i.IdProducto     AS ID,
        i.NombreProducto AS Nombre,
        i.PrecioUnitario AS Precio,
        i.Iva
FROM    dbo.Inventario i
WHERE   (i.Categoria <> 'Cita' AND i.Categoria <> 'Servicio')  -- ⬅️ excluye no-productos
ORDER BY i.NombreProducto;";

            dgvListaProductos.DataSource = _crud.cargarBDData(sql);

            if (dgvListaProductos.Columns.Contains("ID")) dgvListaProductos.Columns["ID"].Width = 70;
            if (dgvListaProductos.Columns.Contains("Precio")) dgvListaProductos.Columns["Precio"].DefaultCellStyle.Format = "N2";
            if (dgvListaProductos.Columns.Contains("Iva")) dgvListaProductos.Columns["Iva"].DefaultCellStyle.Format = "P0";
        }

        private void Filtrar()
        {
            var dt = dgvListaProductos.DataSource as DataTable;
            if (dt == null) return;
            string q = (txtBuscarProducto.Text ?? "").Trim().Replace("'", "''");
            dt.DefaultView.RowFilter = q.Length == 0
                ? ""
                : "[Nombre] LIKE '%" + q + "%' OR Convert([ID],'System.String') LIKE '%" + q + "%'";
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (dgvListaProductos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            var row = dgvListaProductos.CurrentRow;
            IdProductoSel = Convert.ToInt32(row.Cells["ID"].Value);
            NombreSel = row.Cells["Nombre"].Value + "";
            IvaSel = row.Cells["Iva"].Value == DBNull.Value ? 0m : Convert.ToDecimal(row.Cells["Iva"].Value);
            PrecioSugeridoSel = row.Cells["Precio"].Value == DBNull.Value ? 0m : Convert.ToDecimal(row.Cells["Precio"].Value);

            this.DialogResult = DialogResult.OK;
        }
    }
}
