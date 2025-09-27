using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggCita : Form
    {
        private int? _idCitaEdit;
        private int? _idMascotaSel;

        private const string ESTADO_NUEVO = "Pendiente";
        private static readonly TimeSpan APERTURA = new TimeSpan(7, 0, 0);
        private static readonly TimeSpan CIERRE = new TimeSpan(17, 0, 0);

        // Sugerencias opcionales
        public string MotivoSugerido { get; set; }
        public DateTime? FechaSugerida { get; set; }

        public AggCita() : this(null) { }

        public AggCita(int? idCita)
        {
            InitializeComponent();
            _idCitaEdit = idCita;
            InicializarUI();

            if (_idCitaEdit.HasValue)
                CargarCita(_idCitaEdit.Value);
        }

        // Constructor para abrir precargado desde Registro Clínico
        public AggCita(int? idCita, int idMascotaSel, string mascotaSel, string propietarioSel, string especieSel, string razaSel)
        {
            InitializeComponent();
            _idCitaEdit = idCita;
            InicializarUI();

            _idMascotaSel = idMascotaSel;
            txtMascotaCita.Text = mascotaSel ?? "";
            txtPropietarioCita.Text = propietarioSel ?? "";
            txtEspecieCita.Text = especieSel ?? "";
            txtRazaCita.Text = razaSel ?? "";

            if (string.IsNullOrWhiteSpace(txtEspecieCita.Text) || string.IsNullOrWhiteSpace(txtRazaCita.Text))
                CargarEspecieYRazaPorIdMascota(_idMascotaSel.Value);

            AplicarSugerencias();
        }

        private void InicializarUI()
        {
            txtMascotaCita.ReadOnly = true;
            txtPropietarioCita.ReadOnly = true;
            txtEspecieCita.ReadOnly = true;
            txtRazaCita.ReadOnly = true;
            txtHora.ReadOnly = true;

            btnListaMascota.Click += (s, e) => AbrirListaMascota();

            dtpFecha.ValueChanged += (s, e) => txtHora.Clear();
            dtpFecha.CloseUp += (s, e) => { txtHora.Clear(); AbrirSelectorHora(); };
            txtHora.Click += (s, e) => AbrirSelectorHora();

            btnAceptar.Click += (s, e) => Guardar();
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            AplicarRestriccionFechaMinima();
        }

        private void AplicarSugerencias()
        {
            if (!string.IsNullOrWhiteSpace(MotivoSugerido) && string.IsNullOrWhiteSpace(txtMotivo.Text))
                txtMotivo.Text = MotivoSugerido;

            if (FechaSugerida.HasValue)
            {
                var f = FechaSugerida.Value.Date;
                if (f < dtpFecha.MinDate) f = dtpFecha.MinDate;
                dtpFecha.Value = f;
            }
        }

        private void AbrirListaMascota()
        {
            using (var frm = new FRMListaMascota())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _idMascotaSel = frm.IdMascotaSel;
                    txtMascotaCita.Text = frm.MascotaSel;
                    txtPropietarioCita.Text = frm.PropietarioSel;
                    txtEspecieCita.Text = frm.EspecieSel;
                    txtRazaCita.Text = frm.RazaSel;

                    if (string.IsNullOrWhiteSpace(txtEspecieCita.Text) || string.IsNullOrWhiteSpace(txtRazaCita.Text))
                        CargarEspecieYRazaPorIdMascota(_idMascotaSel.Value);
                }
            }
        }

        private void AplicarRestriccionFechaMinima()
        {
            var ahora = DateTime.Now;
            var min = DateTime.Today;
            if (ahora.TimeOfDay >= CIERRE)
                min = DateTime.Today.AddDays(1);

            dtpFecha.MinDate = min;

            if (dtpFecha.Value < min)
                dtpFecha.Value = min;
        }

        private static TimeSpan SiguienteMediaHora(DateTime referencia)
        {
            var m = referencia.Minute;
            if (m == 0 || m == 30) return new TimeSpan(referencia.Hour, m, 0);
            var add = (m < 30) ? (30 - m) : (60 - m);
            var t = referencia.AddMinutes(add);
            return new TimeSpan(t.Hour, t.Minute, 0);
        }

        private void AbrirSelectorHora()
        {
            AplicarRestriccionFechaMinima();
            var hhmm = ElegirHora(dtpFecha.Value.Date);
            if (!string.IsNullOrEmpty(hhmm))
                txtHora.Text = hhmm;
        }

        private string ElegirHora(DateTime dia)
        {
            var ocupadasDT = CedulaUtils.HorasOcupadas(dia);
            var ocupadas = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            foreach (DataRow r in ocupadasDT.Rows)
                ocupadas.Add(Convert.ToString(r["Hora"]));

            TimeSpan inicio = APERTURA;
            if (dia == DateTime.Today)
            {
                var s = SiguienteMediaHora(DateTime.Now);
                if (s > inicio) inicio = s;
            }

            if (dia == DateTime.Today && inicio >= CIERRE)
            {
                MessageBox.Show("La jornada de hoy ya finalizó. Programando para el día siguiente.");
                dtpFecha.Value = DateTime.Today.AddDays(1);
                return ElegirHora(dtpFecha.Value.Date);
            }

            var frm = new Form
            {
                Text = "Seleccionar hora",
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Size = new Size(340, 280),
                TopMost = true
            };
            frm.Deactivate += (s, e) => frm.Close();

            var p = txtHora.PointToScreen(new Point(0, txtHora.Height));
            var wa = Screen.FromControl(this).WorkingArea;
            int x = Math.Min(Math.Max(wa.Left, p.X), wa.Right - frm.Width);
            int y = Math.Min(Math.Max(wa.Top, p.Y), wa.Bottom - frm.Height);
            frm.Location = new Point(x, y);

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                WrapContents = true
            };
            frm.Controls.Add(panel);

            string seleccionado = null;

            var t = inicio;
            int total = 0;
            while (t <= CIERRE)
            {
                var hhmm = $"{t.Hours:D2}:{t.Minutes:D2}";
                var btn = new Button
                {
                    Text = hhmm,
                    Width = 80,
                    Height = 36,
                    Margin = new Padding(6),
                    FlatStyle = FlatStyle.Flat
                };

                bool fueraHorario = t < APERTURA || t > CIERRE;
                bool ocupada = ocupadas.Contains(hhmm);

                if (fueraHorario || ocupada)
                {
                    btn.Enabled = false;
                    btn.BackColor = System.Drawing.Color.MistyRose;
                    btn.ForeColor = System.Drawing.Color.Maroon;
                }
                else
                {
                    btn.BackColor = System.Drawing.Color.Honeydew;
                    btn.ForeColor = System.Drawing.Color.DarkGreen;
                    btn.Click += (s, e) => { seleccionado = hhmm; frm.Close(); };
                }

                panel.Controls.Add(btn);
                total++;
                t = t.Add(TimeSpan.FromMinutes(30));
                if (t.Hours == 17 && t.Minutes == 30) break;
            }

            int cols = 3, btnH = 36, margen = 12;
            int filas = (int)Math.Ceiling(total / (double)cols);
            panel.AutoScrollMinSize = new Size(0, filas * (btnH + margen) + 20);

            frm.ShowDialog(this);
            return seleccionado;
        }

        private void Guardar()
        {
            if (!_idMascotaSel.HasValue)
            {
                MessageBox.Show("Seleccione una mascota (botón Lista).");
                btnListaMascota.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHora.Text))
            {
                MessageBox.Show("Seleccione una hora.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show("Ingrese el motivo.");
                txtMotivo.Focus();
                return;
            }

            var ts = TimeSpan.ParseExact(txtHora.Text, @"hh\:mm", CultureInfo.InvariantCulture);
            if (!(ts >= APERTURA && ts <= CIERRE && (ts.Minutes == 0 || ts.Minutes == 30)))
            {
                MessageBox.Show("Hora fuera de horario (07:00–17:00) o no es múltiplo de 30 min.");
                return;
            }

            var fechaSel = dtpFecha.Value.Date;
            var ahora = DateTime.Now;
            if (fechaSel < DateTime.Today)
            {
                MessageBox.Show("No se permiten días pasados.");
                return;
            }

            if (fechaSel == DateTime.Today && ts < SiguienteMediaHora(ahora))
            {
                MessageBox.Show("La hora seleccionada ya pasó.");
                return;
            }

            var fechaHora = fechaSel.Add(ts);
            int idMascota = _idMascotaSel.Value;

            if (CedulaUtils.ExisteChoqueHorario(fechaSel, txtHora.Text, _idCitaEdit))
            {
                MessageBox.Show("Esa hora ya está ocupada.");
                return;
            }

            var db = new csConexionBD();
            try
            {
                db.abrirConexion();

                if (_idCitaEdit.HasValue)
                {
                    using (var cmd = new SqlCommand(@"
                        UPDATE dbo.GestionCita
                           SET IdMascota=@m, FechaHora=@fh, Motivo=@mo, Estado=@es
                         WHERE IdCita=@id;", db.obtenerConexion()))
                    {
                        cmd.Parameters.AddWithValue("@m", idMascota);
                        cmd.Parameters.AddWithValue("@fh", fechaHora);
                        cmd.Parameters.AddWithValue("@mo", txtMotivo.Text.Trim());
                        cmd.Parameters.AddWithValue("@es", ESTADO_NUEVO);
                        cmd.Parameters.AddWithValue("@id", _idCitaEdit.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO dbo.GestionCita (IdMascota, FechaHora, Motivo, Estado)
                        VALUES (@m, @fh, @mo, @es);", db.obtenerConexion()))
                    {
                        cmd.Parameters.AddWithValue("@m", idMascota);
                        cmd.Parameters.AddWithValue("@fh", fechaHora);
                        cmd.Parameters.AddWithValue("@mo", txtMotivo.Text.Trim());
                        cmd.Parameters.AddWithValue("@es", ESTADO_NUEVO);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cita guardada.");
                DialogResult = DialogResult.OK; // el caller (Registro Clínico) abrirá Ventas si aplica
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar la cita.\n" + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { db.cerrarConexion(); }
        }

        private void CargarCita(int idCita)
        {
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var da = new SqlDataAdapter(@"
    SELECT g.IdCita,
           m.IdMascota,
           m.Nombre AS Mascota,
           e.Especie AS Especie,
           r.Raza    AS Raza,
           (p.Nombre + ' ' + p.Apellido) AS Propietario,
           g.FechaHora,
           g.Motivo
    FROM dbo.GestionCita g
    JOIN dbo.Mascota m  ON m.IdMascota = g.IdMascota
    JOIN dbo.Persona p  ON p.IdPersona = m.IdPersona
    LEFT JOIN dbo.Especie e ON e.IdEspecie = m.IdEspecie
    LEFT JOIN dbo.Raza    r ON r.IdRaza    = m.IdRaza
    WHERE g.IdCita = @id;", db.obtenerConexion()))
                {
                    da.SelectCommand.Parameters.AddWithValue("@id", idCita);
                    var dt = new DataTable(); da.Fill(dt);
                    if (dt.Rows.Count == 0) return;

                    var r = dt.Rows[0];

                    _idMascotaSel = Convert.ToInt32(r["IdMascota"]);
                    txtMascotaCita.Text = Convert.ToString(r["Mascota"]);
                    txtPropietarioCita.Text = Convert.ToString(r["Propietario"]);
                    txtEspecieCita.Text = Convert.ToString(r["Especie"]);
                    txtRazaCita.Text = Convert.ToString(r["Raza"]);

                    DateTime fh = Convert.ToDateTime(r["FechaHora"]);
                    dtpFecha.Value = fh.Date;
                    txtHora.Text = fh.ToString("HH:mm");

                    txtMotivo.Text = Convert.ToString(r["Motivo"]);

                    AplicarSugerencias();
                }
            }
            finally { db.cerrarConexion(); }
        }

        private void CargarEspecieYRazaPorIdMascota(int idMascota)
        {
            var db = new csCRUD();
            var dt = db.cargarBDData(@"
        SELECT e.Especie, r.Raza
        FROM Mascota m
        LEFT JOIN Especie e ON e.IdEspecie = m.IdEspecie
        LEFT JOIN Raza    r ON r.IdRaza    = m.IdRaza
        WHERE m.IdMascota = @id;",
                new SqlParameter("@id", idMascota));

            if (dt != null && dt.Rows.Count > 0)
            {
                var r = dt.Rows[0];
                if (txtEspecieCita != null && string.IsNullOrWhiteSpace(txtEspecieCita.Text))
                    txtEspecieCita.Text = Convert.ToString(r["Especie"]);
                if (txtRazaCita != null && string.IsNullOrWhiteSpace(txtRazaCita.Text))
                    txtRazaCita.Text = Convert.ToString(r["Raza"]);
            }
        }
    }
}
