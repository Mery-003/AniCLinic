using System;
using System.Data;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class FRMListaProveedores : Form
    {
        private readonly csCRUD _crud = new csCRUD();

        // Valores que devolverá el diálogo al llamador
        public int IdProveedorSel { get; private set; }
        public string NombreSel { get; private set; }
        public string RUCSel { get; private set; }
        public string TelefonoSel { get; private set; }
        public string CorreoSel { get; private set; }
        public string DireccionSel { get; private set; }

        public FRMListaProveedores()
        {
            InitializeComponent();
            PrepararGrid(dgvListaProveedor);

            this.Load += (s, e) => CargarLista();
            txtBuscarProveedor.TextChanged += (s, e) => Filtrar();
            btnAceptar.Click += btnAceptar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Doble clic en una fila = Aceptar
            dgvListaProveedor.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnAceptar_Click(s, e);
            };

            // Al mostrar, enfocar búsqueda
            this.Shown += (s, e) => txtBuscarProveedor.Focus();
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
            string sql = @"
SELECT  IdProveedor      AS ID,
        NombreProveedor  AS Nombre,
        RUC,
        Telefono,
        Correo,
        Direccion
FROM    dbo.Proveedor
ORDER BY NombreProveedor;";

            dgvListaProveedor.DataSource = _crud.cargarBDData(sql);

            if (dgvListaProveedor.Columns.Contains("ID")) dgvListaProveedor.Columns["ID"].Width = 70;
            if (dgvListaProveedor.Columns.Contains("RUC")) dgvListaProveedor.Columns["RUC"].Width = 130;
            if (dgvListaProveedor.Columns.Contains("Telefono")) dgvListaProveedor.Columns["Telefono"].Width = 120;
        }

        private void Filtrar()
        {
            var dt = dgvListaProveedor.DataSource as DataTable;
            if (dt == null) return;

            string q = (txtBuscarProveedor.Text ?? "").Trim().Replace("'", "''");
            if (q.Length == 0) { dt.DefaultView.RowFilter = ""; return; }

            dt.DefaultView.RowFilter =
                "Convert([RUC],'System.String') LIKE '%" + q + "%' OR " +
                "[Nombre] LIKE '%" + q + "%' OR " +
                "Convert([Telefono],'System.String') LIKE '%" + q + "%' OR " +
                "Convert([Correo],'System.String') LIKE '%" + q + "%' OR " +
                "Convert([Direccion],'System.String') LIKE '%" + q + "%'";
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (dgvListaProveedor.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            var row = dgvListaProveedor.CurrentRow;
            IdProveedorSel = Convert.ToInt32(row.Cells["ID"].Value);
            NombreSel = row.Cells["Nombre"].Value + "";
            RUCSel = row.Cells["RUC"].Value + "";
            TelefonoSel = row.Cells["Telefono"].Value + "";
            CorreoSel = row.Cells["Correo"].Value + "";
            DireccionSel = row.Cells["Direccion"].Value + "";

            this.DialogResult = DialogResult.OK;
        }
    }
}
