using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class FMRListaPropietario : Form
    {
        private readonly csCRUD _crud = new csCRUD();

        public int IdPersonaSel { get; private set; }
        public string NombreSel { get; private set; }
        public string ApellidoSel { get; private set; }
        public string CedulaSel { get; private set; }

        public FMRListaPropietario()
        {
            InitializeComponent();
            PrepararGrid(dgvListaPropietario);

            this.Load += (s, e) => CargarLista();
            txtBuscarPropietario.TextChanged += (s, e) => Filtrar();
            btnAceptar.Click += btnAceptar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
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
Select IdPersona AS ID,
           Nombre,
           Apellido,
           Cedula AS [C.I.],
           Celular 
from Persona
Order by Nombre, Apellido";
            dgvListaPropietario.DataSource = _crud.cargarBDData(sql);
            if (dgvListaPropietario.Columns.Contains("ID")) dgvListaPropietario.Columns["ID"].Width = 60;
            if (dgvListaPropietario.Columns.Contains("C.I.")) dgvListaPropietario.Columns["C.I."].Width = 130;
            if (dgvListaPropietario.Columns.Contains("Celular")) dgvListaPropietario.Columns["Celular"].Width = 140;
        }

        private void Filtrar()
        {
            DataTable dt = dgvListaPropietario.DataSource as DataTable;
            if (dt == null) return;
            string q = (txtBuscarPropietario.Text ?? "").Trim().Replace("'", "''");
            if (q.Length == 0) { dt.DefaultView.RowFilter = ""; return; }

            dt.DefaultView.RowFilter =
                "Convert([C.I.],'System.String') LIKE '%" + q + "%' OR " +
                "Nombre LIKE '%" + q + "%' OR Apellido LIKE '%" + q + "%' OR " +
                "([Nombre] + ' ' + [Apellido]) LIKE '%" + q + "%'";
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (dgvListaPropietario.CurrentRow == null) 
            { 
                MessageBox.Show("Seleccione un propietario."); 
                return; 
            }
            IdPersonaSel = Convert.ToInt32(dgvListaPropietario.CurrentRow.Cells["ID"].Value);
            NombreSel = dgvListaPropietario.CurrentRow.Cells["Nombre"].Value + "";
            ApellidoSel = dgvListaPropietario.CurrentRow.Cells["Apellido"].Value + "";
            CedulaSel = dgvListaPropietario.CurrentRow.Cells["C.I."].Value + "";
            this.DialogResult = DialogResult.OK;
        }
    }
}
