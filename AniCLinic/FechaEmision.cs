using System;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class FechaEmision : Form
    {
        public DateTime FechaSeleccionada => dtpFecha.Value.Date;

        private DateTime _minPermitida;
        private DateTime _maxPermitida;

        public FechaEmision()
        {
            InitializeComponent();

            var hoy = DateTime.Today;
            _minPermitida = hoy;               // no permite fechas pasadas
            _maxPermitida = hoy.AddMonths(3);  // máximo 3 meses hacia adelante

            dtpFecha.MinDate = _minPermitida;
            dtpFecha.MaxDate = _maxPermitida;
            dtpFecha.Value = _minPermitida;

            btnAceptar.Click += (s, e) => Aceptar();
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) this.DialogResult = DialogResult.Cancel;
                if (e.KeyCode == Keys.Enter) Aceptar();
            };

            // salvaguarda por si cambian por código
            this.FormClosing += FechaEmision_FormClosing;
        }

        private void Aceptar()
        {
            var f = dtpFecha.Value.Date;
            if (f < _minPermitida || f > _maxPermitida)
            {
                MessageBox.Show(
                    $"La fecha debe estar entre {_minPermitida:dd/MM/yyyy} y {_maxPermitida:dd/MM/yyyy}.",
                    "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFecha.Value = _minPermitida;
                dtpFecha.Focus();
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void FechaEmision_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                var f = dtpFecha.Value.Date;
                if (f < _minPermitida || f > _maxPermitida)
                {
                    e.Cancel = true;
                    MessageBox.Show(
                        $"La fecha debe estar entre {_minPermitida:dd/MM/yyyy} y {_maxPermitida:dd/MM/yyyy}.",
                        "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtpFecha.Value = _minPermitida;
                    dtpFecha.Focus();
                }
            }
        }
    }
}
