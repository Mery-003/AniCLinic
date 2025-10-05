using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class MovimientosDeCompras : Form
    {
        private readonly csCRUD _crud = new csCRUD();
        private DataTable _dt;

        public MovimientosDeCompras()
        {
            InitializeComponent();
            ConfigurarGrid();

            this.Load += (s, e) =>
            {
                CargarGrid();
                AgregarColumnaVerOrden();
                PostFormato();
            };

            dgvmovimiento.CellContentClick += dgvmovimiento_CellContentClick;
            dgvmovimiento.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) AbrirOrdenPorFila(e.RowIndex); };
            dgvmovimiento.DataBindingComplete += (s, e) => PostFormato();
        }

        private void ConfigurarGrid()
        {
            // tamaño fijo (ajústalo en el diseñador)
            dgvmovimiento.Dock = DockStyle.None;
            dgvmovimiento.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            dgvmovimiento.AutoSize = false;

            dgvmovimiento.ReadOnly = true;
            dgvmovimiento.MultiSelect = false;
            dgvmovimiento.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvmovimiento.AllowUserToAddRows = false;
            dgvmovimiento.RowHeadersVisible = false;

            // autoajuste por contenido + scroll si no entra todo
            dgvmovimiento.AutoGenerateColumns = true;
            dgvmovimiento.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvmovimiento.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvmovimiento.ScrollBars = ScrollBars.Both;
        }

        private void CargarGrid()
        {
            // Incluimos IdCompra solo para abrir el reporte (se ocultará).
            const string sql = @"
SELECT
    c.IdCompra,                              -- oculto
    c.NumeroOC            AS [N° Orden],     -- único ID visible
    c.FechaCompra,
    c.FechaEmision,
    p.RUC,
    p.NombreProveedor     AS Proveedor,
    c.MetodoPago,
    c.Subtotal0,
    c.Subtotal12,
    c.IVA12,
    c.Total
FROM dbo.OC_Compra c
JOIN dbo.Proveedor p ON p.IdProveedor = c.IdProveedor
ORDER BY c.IdCompra DESC;";

            _dt = _crud.cargarBDData(sql);
            dgvmovimiento.DataSource = _dt;

            // formatos
            if (dgvmovimiento.Columns.Contains("FechaCompra"))
                dgvmovimiento.Columns["FechaCompra"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            if (dgvmovimiento.Columns.Contains("FechaEmision"))
                dgvmovimiento.Columns["FechaEmision"].DefaultCellStyle.Format = "dd/MM/yyyy";
            foreach (var col in new[] { "Subtotal0", "Subtotal12", "IVA12", "Total" })
                if (dgvmovimiento.Columns.Contains(col))
                    dgvmovimiento.Columns[col].DefaultCellStyle.Format = "N2";
        }

        private void PostFormato()
        {
            // ocultar IdCompra (solo para uso interno)
            if (dgvmovimiento.Columns.Contains("IdCompra"))
                dgvmovimiento.Columns["IdCompra"].Visible = false;

            // crear/ubicar la columna botón al final
            if (!dgvmovimiento.Columns.Contains("colVerOrden"))
                AgregarColumnaVerOrden();
            dgvmovimiento.Columns["colVerOrden"].DisplayIndex = dgvmovimiento.Columns.Count - 1;

            // ordenar columnas visibles en un orden lógico
            string[] orden =
            {
                "N° Orden","FechaCompra","FechaEmision","RUC","Proveedor",
                "MetodoPago","Subtotal0","Subtotal12","IVA12","Total","colVerOrden"
            };

            foreach (var nombre in orden)
                if (dgvmovimiento.Columns.Contains(nombre))
                    dgvmovimiento.Columns[nombre].DisplayIndex = Array.IndexOf(orden, nombre);
        }

        private void AgregarColumnaVerOrden()
        {
            if (dgvmovimiento.Columns.Contains("colVerOrden")) return;

            var col = new DataGridViewButtonColumn
            {
                Name = "colVerOrden",
                HeaderText = "",
                Text = "Ver orden",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            };
            dgvmovimiento.Columns.Add(col);
        }

        private void dgvmovimiento_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvmovimiento.Columns[e.ColumnIndex].Name == "colVerOrden")
                AbrirOrdenPorFila(e.RowIndex);
        }

        private void AbrirOrdenPorFila(int rowIndex)
        {
            var drv = dgvmovimiento.Rows[rowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            // usamos el IdCompra oculto para abrir el reporte
            var val = drv["IdCompra"];
            if (val == null || val == DBNull.Value) return;

            int idCompra = Convert.ToInt32(val);
            using (var frm = new frOrdenCompra(idCompra))
                frm.ShowDialog(this);
        }
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
