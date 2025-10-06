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
    public partial class FrmPermisoEd : Form
    {
        csCRUD crud = new csCRUD();
        private readonly int? _idPermiso;

        public FrmPermisoEd()
        {
            InitializeComponent();
            _idPermiso = null;
            PrepararControles();
            CargarEmpleados();
            Text = "Nuevo permiso";
        }
        public FrmPermisoEd(int idPermiso)
        {
            InitializeComponent();
            _idPermiso = idPermiso;
            PrepararControles();
            CargarEmpleados();
            CargarPermiso(_idPermiso.Value);
            Text = "Editar permiso";
        }

        private void PrepararControles()
        {
            dtFecha.Value = DateTime.Today;

            dtHoraInicio.Format = DateTimePickerFormat.Time;
            dtHoraInicio.ShowUpDown = true;
            dtHoraInicio.Value = DateTime.Today.AddHours(9);

            dtHoraFin.Format = DateTimePickerFormat.Time;
            dtHoraFin.ShowUpDown = true;
            dtHoraFin.Value = DateTime.Today.AddHours(11);

            chkJustificado.Checked = true;

            btnGuardar.Click += btnGuardar_Click;
            btnCancelar.Click += (s, e) => this.Close();
        }

        private void CargarEmpleados()
        {
            string sql = @"
SELECT e.IdEmpleado,
       (p.Nombre + ' ' + p.Apellido) AS NombreCompleto
FROM Empleados e
JOIN Persona p ON p.IdPersona = e.IdPersona
WHERE e.Activo = 1
ORDER BY p.Apellido, p.Nombre;";
            var dt = crud.cargarBDData(sql);
            cboEmpleado.DataSource = dt;
            cboEmpleado.ValueMember = "IdEmpleado";
            cboEmpleado.DisplayMember = "NombreCompleto";

            if (dt.Rows.Count == 0)
                cboEmpleado.SelectedIndex = -1;
        }

        private void CargarPermiso(int idPermiso)
        {
            string sql = @"
SELECT IdPermiso, IdEmpleado, Fecha, HoraInicio, HoraFin, Motivo, Justificado
FROM Permisos
WHERE IdPermiso = @id;";

            var dt = crud.cargarBDData(sql, new SqlParameter("@id", idPermiso));
            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            cboEmpleado.SelectedValue = Convert.ToInt32(r["IdEmpleado"]);
            dtFecha.Value = Convert.ToDateTime(r["Fecha"]);
            dtHoraInicio.Value = DateTime.Today.Add((TimeSpan)r["HoraInicio"]);
            dtHoraFin.Value = DateTime.Today.Add((TimeSpan)r["HoraFin"]);
            txtMotivo.Text = r["Motivo"].ToString();
            chkJustificado.Checked = Convert.ToBoolean(r["Justificado"]);
        }

        private bool ValidarFormulario(out string error)
        {
            error = "";

            if (cboEmpleado.SelectedIndex < 0)
            {
                error = "Seleccione un empleado.";
                return false;
            }

            var hIni = dtHoraInicio.Value.TimeOfDay;
            var hFin = dtHoraFin.Value.TimeOfDay;

            if (hIni >= hFin)
            {
                error = "La hora de inicio debe ser menor a la hora fin.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMotivo.Text) || txtMotivo.Text.Trim().Length < 3)
            {
                error = "Ingrese un motivo válido (mínimo 3 caracteres).";
                return false;
            }

            string sqlOverlap = @"
SELECT TOP 1 1
FROM Permisos
WHERE IdEmpleado = @id
  AND Fecha = @fecha
  AND @hIni < HoraFin
  AND @hFin > HoraInicio
  AND (@idPermiso IS NULL OR IdPermiso <> @idPermiso);";

            var dt = crud.cargarBDData(sqlOverlap,
                new SqlParameter("@id", (int)cboEmpleado.SelectedValue),
                new SqlParameter("@fecha", dtFecha.Value.Date),
                new SqlParameter("@hIni", hIni),
                new SqlParameter("@hFin", hFin),
                new SqlParameter("@idPermiso", (object)_idPermiso ?? DBNull.Value)
            );

            if (dt.Rows.Count > 0)
            {
                error = "El rango horario se solapa con otro permiso de este empleado.";
                return false;
            }

            if ((hFin - hIni).TotalHours > 12)
            {
                error = "La duración no puede exceder 12 horas.";
                return false;
            }

            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario(out string msg))
            {
                MessageBox.Show(msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idEmp = (int)cboEmpleado.SelectedValue;
            DateTime fecha = dtFecha.Value.Date;
            TimeSpan hi = dtHoraInicio.Value.TimeOfDay;
            TimeSpan hf = dtHoraFin.Value.TimeOfDay;
            string motivo = txtMotivo.Text.Trim();
            bool just = chkJustificado.Checked;

            if (_idPermiso == null)
            {
                string insert = @"
INSERT INTO Permisos(IdEmpleado, Fecha, HoraInicio, HoraFin, Motivo, Justificado)
VALUES (@id, @fecha, @hIni, @hFin, @motivo, @just);";

                int filas = crud.ejecutarBD(insert,
                    new SqlParameter("@id", idEmp),
                    new SqlParameter("@fecha", fecha),
                    new SqlParameter("@hIni", SqlDbType.Time) { Value = hi },
                    new SqlParameter("@hFin", SqlDbType.Time) { Value = hf },
                    new SqlParameter("@motivo", motivo),
                    new SqlParameter("@just", just)
                );

                if (filas > 0)
                {
                    MessageBox.Show("Permiso registrado.", "AniClinic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el permiso.", "AniClinic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                string update = @"
UPDATE Permisos
SET IdEmpleado=@id, Fecha=@fecha, HoraInicio=@hIni, HoraFin=@hFin, Motivo=@motivo, Justificado=@just
WHERE IdPermiso=@pid;";

                int filas = crud.ejecutarBD(update,
                    new SqlParameter("@id", idEmp),
                    new SqlParameter("@fecha", fecha),
                    new SqlParameter("@hIni", SqlDbType.Time) { Value = hi },
                    new SqlParameter("@hFin", SqlDbType.Time) { Value = hf },
                    new SqlParameter("@motivo", motivo),
                    new SqlParameter("@just", just),
                    new SqlParameter("@pid", _idPermiso.Value)
                );

                if (filas > 0)
                {
                    MessageBox.Show("Permiso actualizado.", "AniClinic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el permiso.", "AniClinic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

