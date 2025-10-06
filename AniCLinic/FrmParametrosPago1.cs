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
    public partial class FrmParametrosPago1 : Form
    {
        private readonly csCRUD crud = new csCRUD();

        public FrmParametrosPago1()
        {
            InitializeComponent();
            prepararGrid();
            prepararPickers();
            hookEventos();
            cargarHistorial();
            recalcularJornada();
        }

        private void prepararGrid()
        {
            var g = dgvHistorial;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.AutoGenerateColumns = true;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void prepararPickers()
        {
            dtVigenteDesde.Format = DateTimePickerFormat.Short;

            dtHoraInicioJornada.Format = DateTimePickerFormat.Time;
            dtHoraInicioJornada.ShowUpDown = true;

            dtHoraFinJornada.Format = DateTimePickerFormat.Time;
            dtHoraFinJornada.ShowUpDown = true;

            dtHoraInicioAlmuerzo.Format = DateTimePickerFormat.Time;
            dtHoraInicioAlmuerzo.ShowUpDown = true;

            dtHoraFinAlmuerzo.Format = DateTimePickerFormat.Time;
            dtHoraFinAlmuerzo.ShowUpDown = true;

            dtHoraInicioJornada.Value = DateTime.Today.AddHours(7);
            dtHoraFinJornada.Value = DateTime.Today.AddHours(17);
            dtHoraInicioAlmuerzo.Value = DateTime.Today.AddHours(12);
            dtHoraFinAlmuerzo.Value = DateTime.Today.AddHours(13);

            nudToleranciaAtraso.Minimum = 0; nudToleranciaAtraso.Maximum = 120; nudToleranciaAtraso.Value = 5;
            nudUmbralExtra.Minimum = 0; nudUmbralExtra.Maximum = 240; nudUmbralExtra.Value = 10;

            nudPagoHoraNormal.DecimalPlaces = 2; nudPagoHoraNormal.Maximum = 100000; nudPagoHoraNormal.Value = 2;
            nudPagoHoraExtra.DecimalPlaces = 2; nudPagoHoraExtra.Maximum = 100000; nudPagoHoraExtra.Value = 3;
        }

        private void hookEventos()
        {
            dtHoraInicioJornada.ValueChanged += (s, e) => recalcularJornada();
            dtHoraFinJornada.ValueChanged += (s, e) => recalcularJornada();
            dtHoraInicioAlmuerzo.ValueChanged += (s, e) => recalcularJornada();
            dtHoraFinAlmuerzo.ValueChanged += (s, e) => recalcularJornada();

            btnCalcularJornada.Click += (s, e) => recalcularJornada();
            btnGuardarNuevo.Click += (s, e) => guardarNuevo();
            btnActivarSeleccionado.Click += (s, e) => activarSeleccionado();
            btnCerrar.Click += (s, e) => this.Close();
        }

        private void cargarHistorial()
        {
            string sql = @"SELECT IdParametro, VigenteDesde,
                                  HoraInicioJornada, HoraFinJornada,
                                  HoraInicioAlmuerzo, HoraFinAlmuerzo,
                                  JornadaMinutos, ToleranciaAtrasoMin, UmbralExtraMin,
                                  PagoHoraNormal, PagoHoraExtra, Activo
                           FROM ParametrosPago
                           ORDER BY Activo DESC, VigenteDesde DESC, IdParametro DESC;";

            dgvHistorial.DataSource = crud.cargarBDData(sql);
        }

        private int calcularJornadaMinutos(out string error)
        {
            error = null;

            var hIni = dtHoraInicioJornada.Value.TimeOfDay;
            var hFin = dtHoraFinJornada.Value.TimeOfDay;
            var aIni = dtHoraInicioAlmuerzo.Value.TimeOfDay;
            var aFin = dtHoraFinAlmuerzo.Value.TimeOfDay;

            if (hFin <= hIni)
            {
                error = "La hora FIN de jornada debe ser mayor a la hora INICIO.";
                return 0;
            }
            if (!(aIni >= hIni && aFin <= hFin && aFin > aIni))
            {
                error = "El horario de ALMUERZO debe estar dentro de la jornada y fin > inicio.";
                return 0;
            }

            int jornada = (int)(hFin - hIni).TotalMinutes;
            int almuerzo = (int)(aFin - aIni).TotalMinutes;

            int result = jornada - almuerzo;
            if (result <= 0) error = "La JornadaMinutos calculada es <= 0.";
            return result;
        }

        private void recalcularJornada()
        {
            string err;
            int min = calcularJornadaMinutos(out err);
            lblJornadaMinutos.Text = (err == null) ? $"{min} min" : $"Error: {err}";
        }

        private bool validar(out string mensaje, out int jornadaMin)
        {
            mensaje = null;
            jornadaMin = 0;

            string err;
            jornadaMin = calcularJornadaMinutos(out err);
            if (err != null) { mensaje = err; return false; }

            if (nudPagoHoraNormal.Value <= 0 || nudPagoHoraExtra.Value <= 0)
            {
                mensaje = "Los pagos por hora deben ser > 0.";
                return false;
            }
            return true;
        }

        private void guardarNuevo()
        {
            if (!validar(out string mensaje, out int jornadaMin))
            {
                MessageBox.Show(mensaje, "Parámetros de Pago");
                return;
            }

            string sql = @"
BEGIN TRY
  BEGIN TRAN;

  IF @activo = 1
     UPDATE ParametrosPago SET Activo = 0 WHERE Activo = 1;

  INSERT INTO ParametrosPago
    (VigenteDesde, HoraInicioJornada, HoraFinJornada,
     HoraInicioAlmuerzo, HoraFinAlmuerzo,
     JornadaMinutos, ToleranciaAtrasoMin, UmbralExtraMin,
     PagoHoraNormal, PagoHoraExtra, Activo)
  VALUES
    (@vig, @hini, @hfin,
     @almin, @almfin,
     @jornada, @tol, @umbral,
     @pNorm, @pExtra, @activo);

  COMMIT;
END TRY
BEGIN CATCH
  IF @@TRANCOUNT > 0 ROLLBACK;
  THROW;
END CATCH;";

            var ok = crud.ejecutarBD(sql,
                new SqlParameter("@vig", dtVigenteDesde.Value.Date),
                new SqlParameter("@hini", dtHoraInicioJornada.Value.TimeOfDay),
                new SqlParameter("@hfin", dtHoraFinJornada.Value.TimeOfDay),
                new SqlParameter("@almin", dtHoraInicioAlmuerzo.Value.TimeOfDay),
                new SqlParameter("@almfin", dtHoraFinAlmuerzo.Value.TimeOfDay),
                new SqlParameter("@jornada", jornadaMin),
                new SqlParameter("@tol", (int)nudToleranciaAtraso.Value),
                new SqlParameter("@umbral", (int)nudUmbralExtra.Value),
                new SqlParameter("@pNorm", nudPagoHoraNormal.Value),
                new SqlParameter("@pExtra", nudPagoHoraExtra.Value),
                new SqlParameter("@activo", chkActivo.Checked ? 1 : 0)
            );

            if (ok > 0)
            {
                MessageBox.Show("Parámetro guardado correctamente.", "Parámetros de Pago");
                cargarHistorial();
            }
        }

        private void activarSeleccionado()
        {
            if (dgvHistorial.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un registro del historial.");
                return;
            }
            var row = (dgvHistorial.CurrentRow.DataBoundItem as DataRowView);
            if (row == null) return;

            int id = Convert.ToInt32(row["IdParametro"]);

            string sql = @"
BEGIN TRY
  BEGIN TRAN;
  UPDATE ParametrosPago SET Activo = 0 WHERE Activo = 1;
  UPDATE ParametrosPago SET Activo = 1 WHERE IdParametro = @id;
  COMMIT;
END TRY
BEGIN CATCH
  IF @@TRANCOUNT > 0 ROLLBACK;
  THROW;
END CATCH;";

            var res = crud.ejecutarBD(sql, new SqlParameter("@id", id));
            if (res > 0)
            {
                MessageBox.Show("Parámetro activado.");
                cargarHistorial();
            }
        }
    }
}
