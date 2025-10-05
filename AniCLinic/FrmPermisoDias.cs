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
    public partial class FrmPermisoDias : Form
    {
        private readonly csCRUD crud = new csCRUD();

        public FrmPermisoDias()
        {
            InitializeComponent();
            CargarEmpleados();
            dtDesde.Value = DateTime.Today;
            dtHasta.Value = DateTime.Today;
        }

        private void CargarEmpleados()
        {
            string sql = @"
SELECT E.IdEmpleado,
       (P.Nombre + ' ' + P.Apellido) AS NombreCompleto,
       P.Cedula
FROM Empleados E 
JOIN Persona P ON P.IdPersona = E.IdPersona
WHERE E.Activo = 1
ORDER BY P.Apellido, P.Nombre;";

            var dt = crud.cargarBDData(sql);
            cboEmpleado.DisplayMember = "NombreCompleto";
            cboEmpleado.ValueMember = "IdEmpleado";
            cboEmpleado.DataSource = dt;
            cboEmpleado.SelectedIndex = dt.Rows.Count > 0 ? 0 : -1;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cboEmpleado.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un empleado.");
                return;
            }

            var idEmpleado = Convert.ToInt32(cboEmpleado.SelectedValue);
            var desde = dtDesde.Value.Date;
            var hasta = dtHasta.Value.Date;
            var justificado = chkJustificado.Checked ? 1 : 0;
            var motivo = (txtMotivo.Text ?? "").Trim();

            if (desde > hasta)
            {
                MessageBox.Show("La fecha 'Desde' no puede ser mayor que 'Hasta'.");
                return;
            }

            if (string.IsNullOrWhiteSpace(motivo))
            {
                MessageBox.Show("Ingrese un motivo.");
                return;
            }

            var param = crud.cargarBDData("SELECT TOP 1 HoraInicioJornada, HoraFinJornada FROM vParametroActivo;");
            if (param == null || param.Rows.Count == 0)
            {
                MessageBox.Show("No hay parámetros de pago activos.");
                return;
            }

            var hIni = (TimeSpan)param.Rows[0]["HoraInicioJornada"];
            var hFin = (TimeSpan)param.Rows[0]["HoraFinJornada"];

            try
            {
                int total = 0;

                for (DateTime f = desde; f <= hasta; f = f.AddDays(1))
                {
                    string sqlDel = @"
DELETE FROM Permisos 
 WHERE IdEmpleado = @id AND Fecha = @fecha;";

                    crud.ejecutarBD(sqlDel,
                        new SqlParameter("@id", idEmpleado),
                        new SqlParameter("@fecha", f));

                    string sqlIns = @"
INSERT INTO Permisos(IdEmpleado, Fecha, HoraInicio, HoraFin, Motivo, Justificado)
VALUES(@id, @fecha, @hIni, @hFin, @motivo, @just);";

                    total += crud.ejecutarBD(sqlIns,
                        new SqlParameter("@id", idEmpleado),
                        new SqlParameter("@fecha", f),
                        new SqlParameter("@hIni", hIni),
                        new SqlParameter("@hFin", hFin),
                        new SqlParameter("@motivo", motivo),
                        new SqlParameter("@just", justificado));
                }

                MessageBox.Show($"Permiso por días registrado.\nFilas afectadas: {total}");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
