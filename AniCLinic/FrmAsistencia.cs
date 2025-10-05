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
    public partial class FrmAsistencia : Form
    {

        csCRUD crud = new csCRUD();
        bool botonesAgregados = false;

        private DateTime _diaActual = DateTime.Today;
        private Timer _relojCambioDia;

        public FrmAsistencia()
        {
            InitializeComponent();
            prepararGrid();
            configurarEventosAutoReset();
            cargarDataA();
        }


        public void cargarDataA(string filtro = "")
        {
            var hoy = _diaActual; 

            string sentencia = @"
SELECT  E.IdEmpleado,
        P.Cedula,
        P.Nombre,
        P.Apellido,
        A.HoraEntrada,
        ISNULL(calc.AtrasoMin,0)      AS AtrasoMin,
        A.HoraSalida,
        ISNULL(calc.ExtraMin,0)       AS ExtraMin,
        ISNULL(calc.MinTrabajados,0)  AS MinTrabajados
FROM Empleados E
JOIN Persona P              ON P.IdPersona = E.IdPersona
LEFT JOIN Asistencias A     ON A.IdEmpleado = E.IdEmpleado AND A.Fecha = @hoy
LEFT JOIN vAsistenciaCalculoDia calc
                            ON calc.IdEmpleado = E.IdEmpleado AND calc.Fecha = @hoy
WHERE E.Activo = 1
  AND (
        @filtro = '' OR
        (P.Nombre + ' ' + P.Apellido) LIKE (@filtro + '%') OR
        P.Cedula LIKE (@filtro + '%')
      )
ORDER BY P.Apellido, P.Nombre;";

            dgvAsistencias.DataSource = crud.cargarBDData(
                sentencia,
                new SqlParameter("@hoy", hoy),
                new SqlParameter("@filtro", filtro ?? string.Empty)
            );

            configurarColumnas();
        }


        public void prepararGrid()
        {
            var g = dgvAsistencias;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AutoGenerateColumns = true;

            g.CellContentClick -= dgvAsistencias_CellContentClick;
            g.CellContentClick += dgvAsistencias_CellContentClick;

            g.CellFormatting -= dgvAsistencias_CellFormatting;
            g.CellFormatting += dgvAsistencias_CellFormatting;
        }

        public void configurarColumnas()
        {
            var g = dgvAsistencias;
            if (g.Columns.Count == 0) return;

            if (g.Columns.Contains("IdEmpleado"))
            {
                g.Columns["IdEmpleado"].Width = 60;
                g.Columns["IdEmpleado"].DisplayIndex = 0;
                g.Columns["IdEmpleado"].Visible = false; 
            }

            if (!botonesAgregados)
            {
                var colEntrada = new DataGridViewButtonColumn
                {
                    Name = "Entrada",
                    HeaderText = "Entrada",
                    Text = "Marcar",
                    UseColumnTextForButtonValue = true,
                    Width = 90
                };
                g.Columns.Add(colEntrada);

                var colSalida = new DataGridViewButtonColumn
                {
                    Name = "Salida",
                    HeaderText = "Salida",
                    Text = "Marcar",
                    UseColumnTextForButtonValue = true,
                    Width = 90
                };
                g.Columns.Add(colSalida);

                botonesAgregados = true;
            }
            if (g.Columns.Contains("HoraEntrada")) g.Columns["HoraEntrada"].HeaderText = "Hora Entrada";
            if (g.Columns.Contains("HoraSalida")) g.Columns["HoraSalida"].HeaderText = "Hora Salida";
            if (g.Columns.Contains("AtrasoMin")) g.Columns["AtrasoMin"].HeaderText = "Atraso (min)";
            if (g.Columns.Contains("ExtraMin")) g.Columns["ExtraMin"].HeaderText = "Extra (min)";
            if (g.Columns.Contains("MinTrabajados")) g.Columns["MinTrabajados"].HeaderText = "Trabajados (min)";

            if (g.Columns.Contains("Cedula")) g.Columns["Cedula"].DisplayIndex = 1;
            if (g.Columns.Contains("Nombre")) g.Columns["Nombre"].DisplayIndex = 2;
            if (g.Columns.Contains("Apellido")) g.Columns["Apellido"].DisplayIndex = 3;
            if (g.Columns.Contains("Entrada")) g.Columns["Entrada"].DisplayIndex = 4;
            if (g.Columns.Contains("HoraEntrada")) g.Columns["HoraEntrada"].DisplayIndex = 5;
            if (g.Columns.Contains("AtrasoMin")) g.Columns["AtrasoMin"].DisplayIndex = 6;
            if (g.Columns.Contains("Salida")) g.Columns["Salida"].DisplayIndex = 7;
            if (g.Columns.Contains("HoraSalida")) g.Columns["HoraSalida"].DisplayIndex = 8;
            if (g.Columns.Contains("ExtraMin")) g.Columns["ExtraMin"].DisplayIndex = 9;
            if (g.Columns.Contains("MinTrabajados")) g.Columns["MinTrabajados"].DisplayIndex = 10;

        }

        private void dgvAsistencias_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var nombreCol = dgvAsistencias.Columns[e.ColumnIndex].Name;
            if (nombreCol != "Entrada" && nombreCol != "Salida") return;

            var rowView = dgvAsistencias.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;

            int idEmpleado = Convert.ToInt32(rowView["IdEmpleado"]);
            string horaEnt = rowView["HoraEntrada"] == DBNull.Value ? "" : rowView["HoraEntrada"].ToString();
            string horaSal = rowView["HoraSalida"] == DBNull.Value ? "" : rowView["HoraSalida"].ToString();

            if (nombreCol == "Entrada")
            {
                if (!string.IsNullOrWhiteSpace(horaEnt))
                {
                    MessageBox.Show("La ENTRADA ya fue marcada.", "AniClinic");
                    return;
                }
                MarcarEntrada(idEmpleado);
            }
            else 
            {
                if (string.IsNullOrWhiteSpace(horaEnt))
                {
                    MessageBox.Show("Primero marca la ENTRADA.", "AniClinic");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(horaSal))
                {
                    MessageBox.Show("La SALIDA ya fue marcada.", "AniClinic");
                    return;
                }
                MarcarSalida(idEmpleado);
            }

            cargarDataA(txtBuscar.Text.Trim());
        }

        private void dgvAsistencias_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var g = dgvAsistencias;
            var colName = g.Columns[e.ColumnIndex].Name;
            if (colName != "Entrada" && colName != "Salida") return;

            var horaEnt = Convert.ToString(((DataRowView)g.Rows[e.RowIndex].DataBoundItem)["HoraEntrada"] ?? "");
            var horaSal = Convert.ToString(((DataRowView)g.Rows[e.RowIndex].DataBoundItem)["HoraSalida"] ?? "");

            if (colName == "Entrada")
            {
                bool habilitar = string.IsNullOrWhiteSpace(horaEnt);
                g.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = !habilitar;
                g.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor =
                    habilitar ? System.Drawing.Color.Black : System.Drawing.Color.Gray;
            }
            else 
            {
                bool habilitar = !string.IsNullOrWhiteSpace(horaEnt) && string.IsNullOrWhiteSpace(horaSal);
                g.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = !habilitar;
                g.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor =
                    habilitar ? System.Drawing.Color.Black : System.Drawing.Color.Gray;
            }
        }

        private void MarcarEntrada(int idEmpleado)
        {
            string sql = @"
BEGIN TRY
    INSERT INTO Asistencias(IdEmpleado, Fecha, HoraEntrada, Estado)
    VALUES (@id, CAST(GETDATE() AS DATE), CAST(GETDATE() AS TIME(0)), 'ACTIVO');
END TRY
BEGIN CATCH
    IF ERROR_NUMBER() IN (2627, 2601)
    BEGIN
        UPDATE Asistencias
           SET HoraEntrada = ISNULL(HoraEntrada, CAST(GETDATE() AS TIME(0))),
               Estado = 'ACTIVO'
         WHERE IdEmpleado = @id
           AND Fecha = CAST(GETDATE() AS DATE);
    END
    ELSE
        THROW;
END CATCH;";

            crud.ejecutarBD(sql, new SqlParameter("@id", idEmpleado));
        }

        private void MarcarSalida(int idEmpleado)
        {
            string sql = @"
UPDATE Asistencias
   SET HoraSalida = CAST(GETDATE() AS TIME(0)),
       Estado = 'CERRADO'
 WHERE IdEmpleado = @id
   AND Fecha = CAST(GETDATE() AS DATE)
   AND HoraEntrada IS NOT NULL
   AND HoraSalida IS NULL;";

            int filas = crud.ejecutarBD(sql, new SqlParameter("@id", idEmpleado));
            if (filas == 0)
                MessageBox.Show("No se pudo marcar la SALIDA (¿ya estaba marcada?).", "AniClinic");
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            cargarDataA(txtBuscar.Text);
        }

        private void configurarEventosAutoReset()
        {
            _relojCambioDia = new Timer();
            _relojCambioDia.Interval = 30_000; 
            _relojCambioDia.Tick += (s, e) =>
            {
                var hoy = DateTime.Today;
                if (hoy != _diaActual)
                {
                    _diaActual = hoy;
                    txtBuscar.Clear();
                    cargarDataA();
                }
            };
            _relojCambioDia.Start();
        }

        private void txtBuscar_TextChanged_1(object sender, EventArgs e)
        {
            cargarDataA( txtBuscar.Text);
        }
    }
}
