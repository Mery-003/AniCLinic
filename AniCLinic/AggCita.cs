using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggCita : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        private int? _idCitaEdit;
        private int? _idMascota;
        private int? _idPropietario;        // dueño
        private int? _idEmpleado;           // veterinario (sesión en nueva cita)
        private string _nombreEmpleado = "";
        private bool _llenandoCombo = false;
        private bool _cargandoEdicion = false;

        // ======= NUEVO: separación mínima (min) entre citas =======
        private const int BUFFER_MIN = 30;

        public AggCita()
        {
            InitializeComponent();

            cmbMascotaCita.SelectedIndexChanged += CmbMascotaCita_Changed;
            cmbMascotaCita.SelectionChangeCommitted += CmbMascotaCita_Changed;

            this.Load -= AggCita_Load;
            this.Load += AggCita_Load;
            this.Shown -= AggCita_Shown;
            this.Shown += AggCita_Shown;
        }

        // NUEVA CITA (desde sesión)
        public AggCita(int idEmpleadoSesion, string nombreEmpleadoSesion) : this()
        {
            _idEmpleado = idEmpleadoSesion;
            _nombreEmpleado = nombreEmpleadoSesion ?? "";
        }

        // EDITAR CITA
        public AggCita(int idCita) : this()
        {
            _idCitaEdit = idCita;
        }

        private sealed class MascotaItem
        {
            public int IdMascota { get; set; }
            public string Nombre { get; set; }
            public string Especie { get; set; }
            public string Raza { get; set; }
            public override string ToString() => Nombre;
        }

        // ===== Helpers =====
        private static string SoloDigitos(string s) => new string((s ?? "").Where(char.IsDigit).ToArray());
        private static bool CedulaValidaEC(string ci)
        {
            var d = SoloDigitos(ci);
            if (!Regex.IsMatch(d, @"^\d{10}$")) return false;
            int prov = int.Parse(d.Substring(0, 2));
            int t = d[2] - '0';
            if (prov < 1 || prov > 24 || t >= 6) return false;
            int[] coef = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int v = (d[i] - '0') * coef[i];
                if (v >= 10) v -= 9;
                suma += v;
            }
            int ver = (10 - (suma % 10)) % 10;
            return ver == (d[9] - '0');
        }

        private void CargarDatosMascotaPorId(int idMascota)
        {
            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand("SELECT Especie, Raza FROM Mascota WHERE IdMascota=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", idMascota);
                    using (var rd = cmd.ExecuteReader())
                        if (rd.Read())
                        {
                            txtEspecie.Text = rd["Especie"]?.ToString() ?? "";
                            txtRaza.Text = rd["Raza"]?.ToString() ?? "";
                        }
                }
            }
            finally { _db.CerrarConexion(); }
        }

        private void ActualizarCamposDesdeCombo()
        {
            if (_llenandoCombo || _cargandoEdicion) return;

            if (cmbMascotaCita.SelectedItem is MascotaItem it)
            {
                _idMascota = it.IdMascota;
                txtEspecie.Text = it.Especie ?? "";
                txtRaza.Text = it.Raza ?? "";
                return;
            }

            if (cmbMascotaCita.SelectedItem is DataRowView drv)
            {
                _idMascota = Convert.ToInt32(drv["IdMascota"]);
                txtEspecie.Text = drv["Especie"]?.ToString() ?? "";
                txtRaza.Text = drv["Raza"]?.ToString() ?? "";
                return;
            }

            if (cmbMascotaCita.SelectedValue != null &&
                int.TryParse(cmbMascotaCita.SelectedValue.ToString(), out int id))
            {
                _idMascota = id;
                CargarDatosMascotaPorId(id);
                return;
            }

            _idMascota = null;
            txtEspecie.Clear();
            txtRaza.Clear();
        }

        private void CargarMascotasDePropietario(int idPropietario, int? seleccionarIdMascota = null)
        {
            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand(@"
                    SELECT m.IdMascota, m.Nombre, m.Especie, m.Raza
                    FROM PropietarioMascota pm
                    JOIN Mascota m ON m.IdMascota = pm.IdMascota
                    WHERE pm.IdPropietario = @p
                    ORDER BY m.Nombre;", cn))
                {
                    cmd.Parameters.AddWithValue("@p", idPropietario);
                    using (var rd = cmd.ExecuteReader())
                    {
                        var lista = new List<MascotaItem>();
                        while (rd.Read())
                            lista.Add(new MascotaItem
                            {
                                IdMascota = Convert.ToInt32(rd["IdMascota"]),
                                Nombre = rd["Nombre"]?.ToString(),
                                Especie = rd["Especie"]?.ToString(),
                                Raza = rd["Raza"]?.ToString()
                            });

                        _llenandoCombo = true;

                        cmbMascotaCita.DataSource = null;
                        cmbMascotaCita.DisplayMember = nameof(MascotaItem.Nombre);
                        cmbMascotaCita.ValueMember = nameof(MascotaItem.IdMascota);
                        cmbMascotaCita.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbMascotaCita.DataSource = lista;
                        cmbMascotaCita.Enabled = lista.Count > 0;

                        if (lista.Count == 0)
                        {
                            _idMascota = null;
                            txtEspecie.Clear();
                            txtRaza.Clear();
                            MessageBox.Show("Este propietario no tiene mascotas registradas.");
                        }
                        else
                        {
                            if (seleccionarIdMascota.HasValue && lista.Any(x => x.IdMascota == seleccionarIdMascota.Value))
                                cmbMascotaCita.SelectedValue = seleccionarIdMascota.Value;
                            else
                                cmbMascotaCita.SelectedIndex = 0;
                        }
                    }
                }
            }
            finally
            {
                _llenandoCombo = false;
                _db.CerrarConexion();
                ActualizarCamposDesdeCombo();
            }
        }

        // ===== Eventos =====
        private void AggCita_Load(object sender, EventArgs e)
        {
            dtpFecha.Format = DateTimePickerFormat.Custom;
            dtpFecha.CustomFormat = "dddd, dd 'de' MMMM 'de' yyyy";

            // No permitir fechas pasadas
            dtpFecha.MinDate = DateTime.Today;

            cmbMascotaCita.Enabled = false;

            // Nueva cita: fija el vet desde sesión
            if (!_idCitaEdit.HasValue && _idEmpleado.HasValue)
            {
                txtVeterinario.Text = _nombreEmpleado;
                txtVeterinario.ReadOnly = true;
                txtVeterinario.TabStop = false;
            }

            if (_idCitaEdit.HasValue)
                CargarCitaParaEditar(_idCitaEdit.Value);
        }

        private void AggCita_Shown(object sender, EventArgs e)
        {
            if (_idCitaEdit.HasValue && string.IsNullOrWhiteSpace(txtCedulaCita.Text))
                CargarCitaParaEditar(_idCitaEdit.Value);
        }

        private void CmbMascotaCita_Changed(object sender, EventArgs e) => ActualizarCamposDesdeCombo();
        private void btnCancelar_Click(object sender, EventArgs e) => Close();

        // lupa: buscar por C.I.
        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            var ci = (txtCedulaCita.Text ?? "").Trim();

            if (!CedulaValidaEC(ci))
            {
                MessageBox.Show("C.I. inválida. Debe ser un número válido de 10 dígitos.");
                return;
            }

            var cn = _db.AbrirConexion();
            try
            {
                int? idProp = null;
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 pr.IdPropietario
                    FROM Propietario pr
                    JOIN Persona pe ON pe.IdPersona = pr.IdPersona
                    WHERE pe.Cedula = @ci;", cn))
                {
                    cmd.Parameters.AddWithValue("@ci", ci);
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value) idProp = Convert.ToInt32(obj);
                }

                if (!idProp.HasValue)
                {
                    _idPropietario = null;
                    cmbMascotaCita.DataSource = null;
                    cmbMascotaCita.Enabled = false;
                    txtEspecie.Clear();
                    txtRaza.Clear();
                    MessageBox.Show("No existe un propietario con esa C.I.");
                    return;
                }

                _idPropietario = idProp.Value;
                CargarMascotasDePropietario(_idPropietario.Value);
                cmbMascotaCita.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar por C.I.: " + ex.Message);
            }
            finally { _db.CerrarConexion(); }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!_idPropietario.HasValue) { MessageBox.Show("Primero ingrese la C.I. y seleccione una mascota."); return; }
            if (!_idMascota.HasValue) { MessageBox.Show("Seleccione una mascota."); return; }
            if (!_idEmpleado.HasValue) { MessageBox.Show("No hay un veterinario de sesión. Vuelva a iniciar sesión."); return; }

            // Motivo obligatorio
            var motivoTexto = (txtMotivo.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(motivoTexto))
            {
                MessageBox.Show("El motivo es obligatorio.");
                txtMotivo.Focus();
                return;
            }

            // Parse de hora
            if (!TimeSpan.TryParseExact(mtbHora.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var hora) &&
                !TimeSpan.TryParseExact(mtbHora.Text, "HH\\:mm", CultureInfo.InvariantCulture, out hora))
            {
                MessageBox.Show("Hora inválida. Usa HH:mm (24h).");
                return;
            }

            // Validación: solo cada 30 minutos
            if (hora.Minutes != 0 && hora.Minutes != 30)
            {
                MessageBox.Show("Las citas deben ser cada 30 minutos (minutos 00 o 30).");
                return;
            }

            // Ventana horaria 07:00 – 17:00 (17:00 solo en punto)
            var min = new TimeSpan(7, 0, 0);
            var max = new TimeSpan(17, 0, 0);
            if (hora < min || hora > max || (hora == max && hora.Minutes != 0))
            {
                MessageBox.Show("Solo se pueden agendar citas entre 07:00 y 17:00.");
                return;
            }

            // Fecha seleccionada
            var fecha = dtpFecha.Value.Date;
            if (fecha < DateTime.Today)
            {
                MessageBox.Show("La fecha no puede ser anterior a hoy.");
                return;
            }

            var fechaHora = fecha + hora;

            // ======= NUEVO: no permitir hoy en pasado (o mismo instante) =======
            if (fecha.Date == DateTime.Today && fechaHora <= DateTime.Now)
            {
                MessageBox.Show("No puedes agendar en una hora que ya pasó hoy.");
                return;
            }

            // Conflictos con buffer de 30 min: mismo veterinario / misma mascota
            if (HayConflictoCitaPorVeterinario(_idEmpleado.Value, fechaHora, _idCitaEdit))
            {
                MessageBox.Show("Ya existe una cita para ese veterinario en un horario muy cercano (±30 min).");
                return;
            }
            if (HayConflictoCitaPorMascota(_idMascota.Value, fechaHora, _idCitaEdit))
            {
                MessageBox.Show("Esa mascota tiene una cita en un horario muy cercano (±30 min).");
                return;
            }

            // Guardar / Actualizar
            var cn = _db.AbrirConexion();
            try
            {
                if (_idCitaEdit.HasValue)
                {
                    using (var cmd = new SqlCommand(@"
                        UPDATE GestionCita
                           SET IdPropietario = @p,
                               IdMascota     = @m,
                               IdEmpleado    = @e,
                               FechaHora     = @fh,
                               Motivo        = @motivo
                         WHERE IdCita       = @id;", cn))
                    {
                        cmd.Parameters.AddWithValue("@p", _idPropietario.Value);
                        cmd.Parameters.AddWithValue("@m", _idMascota.Value);
                        cmd.Parameters.AddWithValue("@e", _idEmpleado.Value);
                        cmd.Parameters.AddWithValue("@fh", fechaHora);
                        cmd.Parameters.AddWithValue("@motivo", motivoTexto);
                        cmd.Parameters.AddWithValue("@id", _idCitaEdit.Value);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Cita actualizada ✅");
                }
                else
                {
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO GestionCita
                            (IdPropietario, IdMascota, IdEmpleado, FechaHora, Motivo, Estado)
                        VALUES
                            (@p, @m, @e, @fh, @motivo, N'Programada');", cn))
                    {
                        cmd.Parameters.AddWithValue("@p", _idPropietario.Value);
                        cmd.Parameters.AddWithValue("@m", _idMascota.Value);
                        cmd.Parameters.AddWithValue("@e", _idEmpleado.Value);
                        cmd.Parameters.AddWithValue("@fh", fechaHora);
                        cmd.Parameters.AddWithValue("@motivo", motivoTexto);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Cita guardada correctamente ✅");
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException ex) { MessageBox.Show("Error SQL: " + ex.Message); }
            catch (Exception ex) { MessageBox.Show("No se pudo guardar la cita: " + ex.Message); }
            finally { _db.CerrarConexion(); }
        }

        // ====== Validaciones de conflicto (±30 min en la MISMA FECHA) ======
        private bool HayConflictoCitaPorVeterinario(int idEmpleado, DateTime fechaHora, int? idCitaExcluir)
        {
            var cn = _db.AbrirConexion();
            try
            {
                string sql = @"
SELECT COUNT(*)
FROM GestionCita
WHERE IdEmpleado = @e
  AND CAST(FechaHora AS date) = CAST(@fh AS date)
  AND Estado IN (N'Programada', N'Reprogramada') " +
  (idCitaExcluir.HasValue ? "AND IdCita <> @id " : "") + @"
  AND ABS(DATEDIFF(MINUTE, FechaHora, @fh)) < @buffer;";

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@e", idEmpleado);
                    cmd.Parameters.AddWithValue("@fh", fechaHora);
                    cmd.Parameters.AddWithValue("@buffer", BUFFER_MIN);
                    if (idCitaExcluir.HasValue) cmd.Parameters.AddWithValue("@id", idCitaExcluir.Value);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            finally { _db.CerrarConexion(); }
        }

        private bool HayConflictoCitaPorMascota(int idMascota, DateTime fechaHora, int? idCitaExcluir)
        {
            var cn = _db.AbrirConexion();
            try
            {
                string sql = @"
SELECT COUNT(*)
FROM GestionCita
WHERE IdMascota = @m
  AND CAST(FechaHora AS date) = CAST(@fh AS date)
  AND Estado IN (N'Programada', N'Reprogramada') " +
  (idCitaExcluir.HasValue ? "AND IdCita <> @id " : "") + @"
  AND ABS(DATEDIFF(MINUTE, FechaHora, @fh)) < @buffer;";

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@m", idMascota);
                    cmd.Parameters.AddWithValue("@fh", fechaHora);
                    cmd.Parameters.AddWithValue("@buffer", BUFFER_MIN);
                    if (idCitaExcluir.HasValue) cmd.Parameters.AddWithValue("@id", idCitaExcluir.Value);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            finally { _db.CerrarConexion(); }
        }

        private void CargarCitaParaEditar(int idCita)
        {
            if (_cargandoEdicion) return;
            _cargandoEdicion = true;

            // 1) Leer TODO a variables (sin tocar UI)
            int? idProp = null, idMasc = null, idEmp = null;
            string cedulaProp = null, especie = null, raza = null, motivo = null, nombreVet = null;
            DateTime fechaHora = DateTime.Now;

            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand(@"
                    SELECT gc.IdPropietario, gc.IdMascota, gc.IdEmpleado, gc.FechaHora, gc.Motivo,
                           m.Especie, m.Raza,
                           ISNULL(pv.Nombre + ' ' + pv.Apellido, N'(sin asignar)') AS Veterinario,
                           pepr.Cedula AS CedulaPropietario
                    FROM GestionCita gc
                    JOIN Mascota  m       ON m.IdMascota  = gc.IdMascota
                    LEFT JOIN Empleado e  ON e.IdEmpleado = gc.IdEmpleado
                    LEFT JOIN Persona  pv ON pv.IdPersona = e.IdPersona
                    JOIN Propietario pr   ON pr.IdPropietario = gc.IdPropietario
                    JOIN Persona  pepr    ON pepr.IdPersona = pr.IdPersona
                    WHERE gc.IdCita = @id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    using (var rd = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!rd.Read())
                        {
                            MessageBox.Show("No se encontró la cita.");
                            return;
                        }

                        idProp = Convert.ToInt32(rd["IdPropietario"]);
                        idMasc = Convert.ToInt32(rd["IdMascota"]);
                        idEmp = rd["IdEmpleado"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["IdEmpleado"]);
                        fechaHora = Convert.ToDateTime(rd["FechaHora"]);
                        motivo = rd["Motivo"]?.ToString();
                        especie = rd["Especie"]?.ToString();
                        raza = rd["Raza"]?.ToString();
                        nombreVet = rd["Veterinario"]?.ToString();
                        cedulaProp = rd["CedulaPropietario"]?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la cita: " + ex.Message);
                return;
            }
            finally
            {
                _db.CerrarConexion();
            }

            // 2) Con el reader cerrado, ahora sí tocamos UI / hacemos más consultas
            _idPropietario = idProp;
            _idMascota = idMasc;
            _idEmpleado = idEmp;

            txtCedulaCita.Text = cedulaProp ?? "";
            CargarMascotasDePropietario(_idPropietario.Value, _idMascota);
            cmbMascotaCita.Enabled = true;

            // Mantener la restricción de fecha mínima en edición también
            dtpFecha.MinDate = DateTime.Today;
            dtpFecha.Value = fechaHora.Date;

            mtbHora.Text = fechaHora.ToString("HH:mm");

            txtEspecie.Text = especie ?? "";
            txtRaza.Text = raza ?? "";
            txtMotivo.Text = motivo ?? "";

            txtVeterinario.Text = nombreVet ?? "";
            txtVeterinario.ReadOnly = true;
            txtVeterinario.TabStop = false;

            _cargandoEdicion = false;
        }

        private void btnBuscarVet_Click(object sender, EventArgs e) { }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { }
    }
}
