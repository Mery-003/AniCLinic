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
    public partial class FrmPermiso : Form
    {
        csCRUD crud = new csCRUD();
        bool botonesAgregados = false;

        public FrmPermiso()
        {
            InitializeComponent();
            prepararGrid();

            txtBuscar.TextChanged += (s, e) => cargarDataP();
            btnNuevo.Click += btnNuevo_Click;
            btnNuevoDias.Click += btnNuevoDias_Click;

            cargarDataP();
        }

        private void prepararGrid()
        {
            var g = dgvPermisos;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AutoGenerateColumns = true;

            g.CellContentClick -= dgvPermisos_CellContentClick;
            g.CellContentClick += dgvPermisos_CellContentClick;
        }

        private void configurarColumnas()
        {
            var g = dgvPermisos;

            if (g.Columns.Contains("IdEmpleado")) g.Columns["IdEmpleado"].Visible = false;
            if (g.Columns.Contains("IdPermiso")) g.Columns["IdPermiso"].Visible = false;
            if (g.Columns.Contains("Motivo")) g.Columns["Motivo"].FillWeight = 180;
            if (g.Columns.Contains("Minutos")) g.Columns["Minutos"].HeaderText = "Min";
            if (g.Columns.Contains("AtrasoMin")) g.Columns["AtrasoMin"].HeaderText = "Atraso (min)";
            if (g.Columns.Contains("ExtraMin")) g.Columns["ExtraMin"].HeaderText = "Extra (min)";
            if (g.Columns.Contains("MinTrabajados")) g.Columns["MinTrabajados"].HeaderText = "Trabajados (min)";


            if (!botonesAgregados)
            {
                g.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Editar",
                    HeaderText = "",
                    Text = "Editar",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                });
                g.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Eliminar",
                    HeaderText = "",
                    Text = "Eliminar",
                    UseColumnTextForButtonValue = true,
                    Width = 90
                });
                botonesAgregados = true;
            }
        }

        private void cargarDataP()
        {
            string filtro = txtBuscar.Text?.Trim() ?? "";

            DateTime hoy = DateTime.Today;
            DateTime desde = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime hasta = new DateTime(hoy.Year, hoy.Month, DateTime.DaysInMonth(hoy.Year, hoy.Month)); // fin del mes

            var ctlDesde = this.Controls.Find("dtDesde", true).FirstOrDefault();
            if (ctlDesde is DateTimePicker dpDesde) desde = dpDesde.Value.Date;

            var ctlHasta = this.Controls.Find("dtHasta", true).FirstOrDefault();
            if (ctlHasta is DateTimePicker dpHasta) hasta = dpHasta.Value.Date;

            string sqlBase = @"
SELECT  pr.IdPermiso,
        e.IdEmpleado,
        p.Cedula,
        (p.Nombre + ' ' + p.Apellido) AS Empleado,
        pr.Fecha,
        pr.HoraInicio,
        pr.HoraFin,
        pr.Motivo,
        pr.Justificado,
        ISNULL(calc.AtrasoMin,0)      AS AtrasoMin,
        ISNULL(calc.ExtraMin,0)       AS ExtraMin,
        ISNULL(calc.MinTrabajados,0)  AS MinTrabajados
FROM Permisos pr
JOIN Empleados e ON e.IdEmpleado = pr.IdEmpleado
JOIN Persona   p ON p.IdPersona  = e.IdPersona
LEFT JOIN vAsistenciaCalculoDia calc
       ON calc.IdEmpleado = pr.IdEmpleado AND calc.Fecha = pr.Fecha
WHERE (@filtro = '' 
       OR p.Cedula LIKE (@filtro + '%') 
       OR (p.Nombre + ' ' + p.Apellido) LIKE (@filtro + '%'))
  AND pr.Fecha BETWEEN @desde AND @hasta
ORDER BY pr.Fecha DESC, Empleado;";

            var dt = crud.cargarBDData(
                sqlBase,
                new SqlParameter("@filtro", filtro),
                new SqlParameter("@desde", desde),
                new SqlParameter("@hasta", hasta)
            );

            if (dt == null || dt.Rows.Count == 0)
            {
                string sqlNoFecha = @"
SELECT  pr.IdPermiso,
        e.IdEmpleado,
        p.Cedula,
        (p.Nombre + ' ' + p.Apellido) AS Empleado,
        pr.Fecha,
        pr.HoraInicio,
        pr.HoraFin,
        pr.Motivo,
        pr.Justificado,
        ISNULL(calc.AtrasoMin,0)      AS AtrasoMin,
        ISNULL(calc.ExtraMin,0)       AS ExtraMin,
        ISNULL(calc.MinTrabajados,0)  AS MinTrabajados
FROM Permisos pr
JOIN Empleados e ON e.IdEmpleado = pr.IdEmpleado
JOIN Persona   p ON p.IdPersona  = e.IdPersona
LEFT JOIN vAsistenciaCalculoDia calc
       ON calc.IdEmpleado = pr.IdEmpleado AND calc.Fecha = pr.Fecha
WHERE (@filtro = '' 
       OR p.Cedula LIKE (@filtro + '%') 
       OR (p.Nombre + ' ' + p.Apellido) LIKE (@filtro + '%'))
ORDER BY pr.Fecha DESC, Empleado;";

                dt = crud.cargarBDData(sqlNoFecha, new SqlParameter("@filtro", filtro));
            }

            dgvPermisos.DataSource = dt;
            configurarColumnas();

            this.Text = $"Permisos — {dt?.Rows.Count ?? 0} registro(s)";
        }



        private void btnNuevo_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmPermisoEd())  
                frm.ShowDialog(this);
            cargarDataP();
        }

        private void dgvPermisos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var col = dgvPermisos.Columns[e.ColumnIndex].Name;
            if (col != "Editar" && col != "Eliminar") return;

            var row = dgvPermisos.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null) return;

            int idPermiso = Convert.ToInt32(row["IdPermiso"]);

            if (col == "Eliminar")
            {
                if (MessageBox.Show("¿Eliminar el permiso seleccionado?", "Permisos",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    int filas = crud.ejecutarBD("DELETE FROM Permisos WHERE IdPermiso=@id",
                        new SqlParameter("@id", idPermiso));
                    if (filas > 0) cargarDataP();
                }
            }
            else
            {
                using (var frm = new FrmPermisoEd(idPermiso)) 
                    frm.ShowDialog(this);
                cargarDataP();
            }
        }

        private void btnNuevoDias_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmPermisoDias())  
                frm.ShowDialog(this);

            cargarDataP();  
        }
    }
}
