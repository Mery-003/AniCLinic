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
    public partial class FrmPagos : Form
    {
        private readonly csCRUD crud = new csCRUD();
        private bool botonesAgregados = false;

        public FrmPagos()
        {
            InitializeComponent();
            PrepararControles();
            PrepararGrid();
            CargarPagosTabla();
            txtBuscar.TextChanged += (s, e) => FiltrarPagosLocal();
        }

        private void FiltrarPagos()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (dgvPagos.DataSource is DataTable dt)
            {
                if (string.IsNullOrEmpty(filtro))
                    (dgvPagos.DataSource as DataTable).DefaultView.RowFilter = string.Empty;
                else
                {
                    (dgvPagos.DataSource as DataTable).DefaultView.RowFilter =
                        $"Cedula LIKE '%{filtro}%' OR Empleado LIKE '%{filtro}%'";
                }
            }
        }

        private void FiltrarPagosLocal()
        {
            if (dgvPagos.DataSource is DataTable dt)
            {
                string f = (txtBuscar.Text ?? "").Trim().Replace("'", "''");

                if (string.IsNullOrEmpty(f))
                {
                    dt.DefaultView.RowFilter = string.Empty;
                }
                else
                {
                    dt.DefaultView.RowFilter =
                        $"[Cedula] LIKE '%{f}%' OR [Empleado] LIKE '%{f}%'";
                }
            }
        }


        private void PrepararControles()
        {
            if (cboMes.Items.Count == 0)
            {
                cboMes.DropDownStyle = ComboBoxStyle.DropDownList;
                cboMes.Items.AddRange(new object[]
                {
                    "1 - Enero","2 - Febrero","3 - Marzo","4 - Abril","5 - Mayo","6 - Junio",
                    "7 - Julio","8 - Agosto","9 - Septiembre","10 - Octubre","11 - Noviembre","12 - Diciembre"
                });
                cboMes.SelectedIndex = DateTime.Today.Month - 1;
            }

            nudAnio.Minimum = 2000;
            nudAnio.Maximum = 2100;
            nudAnio.Value = DateTime.Today.Year;

            txtBuscar.TextChanged += (s, e) => CargarPagosTabla();
            cboMes.SelectedIndexChanged += (s, e) => CargarPagosTabla();
            nudAnio.ValueChanged += (s, e) => CargarPagosTabla();

            btnCalcular.Click += BtnCalcular_Click;
            btnGenerar.Click += BtnGenerar_Click;
        }

        private int MesSeleccionado()
        {
            var txt = cboMes.SelectedItem?.ToString() ?? "";
            if (int.TryParse(new string(txt.TakeWhile(char.IsDigit).ToArray()), out int m) && m >= 1 && m <= 12)
                return m;
            return DateTime.Today.Month;
        }

        private int AnioSeleccionado() => (int)nudAnio.Value;

        private void PrepararGrid()
        {
            var g = dgvPagos;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AutoGenerateColumns = true;

            g.CellContentClick -= DgvPagos_CellContentClick;
            g.CellContentClick += DgvPagos_CellContentClick;
        }

        private void ConfigurarColumnas()
        {
            var g = dgvPagos;
            if (g.Columns.Count == 0) return;

            if (g.Columns.Contains("IdPago")) g.Columns["IdPago"].Visible = false;
            if (g.Columns.Contains("IdEmpleado")) g.Columns["IdEmpleado"].Visible = false;

            if (g.Columns.Contains("SueldoBase")) g.Columns["SueldoBase"].DefaultCellStyle.Format = "N2";
            if (g.Columns.Contains("TotalHorasExtras")) g.Columns["TotalHorasExtras"].HeaderText = "Pago Extras";
            if (g.Columns.Contains("TotalMultas")) g.Columns["TotalMultas"].HeaderText = "Descuento Atrasos";
            if (g.Columns.Contains("TotalPagar")) g.Columns["TotalPagar"].DefaultCellStyle.Format = "N2";

            if (!botonesAgregados)
            {
                var verBtn = new DataGridViewButtonColumn
                {
                    Name = "Recalcular",
                    HeaderText = "",
                    Text = "Recalcular",
                    UseColumnTextForButtonValue = true,
                    Width = 100
                };
                g.Columns.Add(verBtn);
                botonesAgregados = true;
            }
        }

        
        private void CargarPagosTabla()
        {
            int mes = MesSeleccionado();
            int anio = AnioSeleccionado();
            string filtro = txtBuscar.Text?.Trim() ?? "";

            string sql = @"
SELECT  pm.IdPago, e.IdEmpleado, p.Cedula,
        (p.Nombre + ' ' + p.Apellido) AS Empleado,
        pm.Mes, pm.Año, pm.SueldoBase,
        pm.TotalHorasExtras, pm.TotalMultas, pm.TotalPagar
FROM PagosMensuales pm
JOIN Empleados e ON e.IdEmpleado = pm.IdEmpleado
JOIN Persona   p ON p.IdPersona  = e.IdPersona
WHERE pm.Año = @anio AND pm.Mes = @mes
  AND (
        @filtro = '' OR
        p.Cedula LIKE '%' + @filtro + '%' OR
        (p.Nombre + ' ' + p.Apellido) LIKE '%' + @filtro + '%'
      )
ORDER BY Empleado;";

            dgvPagos.DataSource = crud.cargarBDData(
                sql,
                new SqlParameter("@anio", AnioSeleccionado()),
                new SqlParameter("@mes", MesSeleccionado()),
                new SqlParameter("@filtro", txtBuscar.Text?.Trim() ?? "")
            );

            ConfigurarColumnas();
            this.Text = $"Pagos — {anio}/{mes:D2}  ({(dgvPagos.Rows?.Count ?? 0)} registro(s))";
        }

        
        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            int mes = MesSeleccionado();
            int anio = AnioSeleccionado();
            string filtro = txtBuscar.Text?.Trim() ?? "";

            string sql = @"
SELECT  v.IdEmpleado, p.Cedula,
        (p.Nombre + ' ' + p.Apellido) AS Empleado,
        v.Mes, v.Anio AS Año,
        v.SueldoBase, v.TotalHorasExtras, v.TotalMultas, v.TotalPagar
FROM vCalculoPagoMes v
JOIN Empleados e ON e.IdEmpleado = v.IdEmpleado
JOIN Persona   p ON p.IdPersona  = e.IdPersona
WHERE v.Anio = @anio AND v.Mes = @mes
  AND (
        @filtro = '' OR
        p.Cedula LIKE '%' + @filtro + '%' OR
        (p.Nombre + ' ' + p.Apellido) LIKE '%' + @filtro + '%'
      )
ORDER BY Empleado;";

            dgvPagos.DataSource = crud.cargarBDData(
                sql,
                new SqlParameter("@anio", AnioSeleccionado()),
                new SqlParameter("@mes", MesSeleccionado()),
                new SqlParameter("@filtro", txtBuscar.Text?.Trim() ?? "")
            );


            ConfigurarColumnas();
            MessageBox.Show("Previsualización calculada (sin guardar).", "Pagos");
        }

       
        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            int mes = MesSeleccionado();
            int anio = AnioSeleccionado();

            var ok = MessageBox.Show(
                $"Se recalcularán y GUARDARÁN los pagos de {anio}/{mes:D2}.\n" +
                $"Esto reemplazará lo existente para ese mes.\n\n¿Continuar?",
                "Pagos", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (ok != DialogResult.Yes) return;

            string sql = "EXEC dbo.sp_GenerarPagosMensuales @Anio, @Mes;";
            var filas = crud.ejecutarBD(sql,
                new SqlParameter("@Anio", anio),
                new SqlParameter("@Mes", mes));

            CargarPagosTabla();
            MessageBox.Show("Pagos generados y guardados correctamente.", "Pagos");
        }

        private void DgvPagos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var col = dgvPagos.Columns[e.ColumnIndex].Name;
            if (col != "Recalcular") return;

            var row = dgvPagos.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;

            int idEmp = Convert.ToInt32(row["IdEmpleado"]);
            int mes = MesSeleccionado();
            int anio = AnioSeleccionado();

            string sql = @"
SELECT TOP 1 v.*
FROM vCalculoPagoMes v
WHERE v.IdEmpleado = @id AND v.Anio=@anio AND v.Mes=@mes;";

            var dt = crud.cargarBDData(sql,
                new SqlParameter("@id", idEmp),
                new SqlParameter("@anio", anio),
                new SqlParameter("@mes", mes));

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos de asistencia/permiso para este empleado en el mes.", "Pagos");
                return;
            }

            var dr = dt.Rows[0];
            MessageBox.Show(
                $"Sueldo base: {dr["SueldoBase"]:N2}\n" +
                $"Pago extras:  {dr["TotalHorasExtras"]:N2}\n" +
                $"Desc. atraso: {dr["TotalMultas"]:N2}\n" +
                $"TOTAL PAGAR:  {dr["TotalPagar"]:N2}",
                "Detalle (pre-cálculo)");
        }

        private void btnParametrosPagos_Click(object sender, EventArgs e)
        {
            using (var f = new FrmParametrosPago1())
            {
                f.StartPosition = FormStartPosition.CenterParent; 
                f.ShowInTaskbar = false;
                f.TopMost = true;           
                f.ShowDialog(this);       
            }
        }
    }
}
