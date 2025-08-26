using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggRegistroClinico : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        // Contexto / claves
        private int? _idCita;              // abrir desde una cita
        private int? _idPropietario;       // dueño
        private int? _idMascota;           // mascota
        private int? _idRegistroClinico;   // edición (opcional)

        // Veterinario (sesión)
        private int? _idEmpleado;
        private string _nombreEmpleado = "";

        private bool _loadedOnce = false;
        private bool _cargando = false;

        public AggRegistroClinico()
        {
            InitializeComponent();
            this.Load += AggRegistroClinico_Load;
            this.Shown += AggRegistroClinico_Shown;
        }

        // Nuevo registro con sesión del vet
        public AggRegistroClinico(int idEmpleadoSesion, string nombreEmpleadoSesion) : this()
        {
            _idEmpleado = idEmpleadoSesion;
            _nombreEmpleado = nombreEmpleadoSesion ?? "";
        }

        // Abrir desde una CITA (recomendado)
        public AggRegistroClinico(int idCita, int idEmpleadoSesion, string nombreEmpleadoSesion) : this()
        {
            _idCita = idCita;
            _idEmpleado = idEmpleadoSesion;
            _nombreEmpleado = nombreEmpleadoSesion ?? "";
        }

        // Abrir con Propietario + Mascota + sesión
        public AggRegistroClinico(int idPropietario, int idMascota, int idEmpleadoSesion, string nombreEmpleadoSesion) : this()
        {
            _idPropietario = idPropietario;
            _idMascota = idMascota;
            _idEmpleado = idEmpleadoSesion;
            _nombreEmpleado = nombreEmpleadoSesion ?? "";
        }

        // Abrir para editar un registro clínico existente
        public AggRegistroClinico(int idRegistroClinico) : this()
        {
            _idRegistroClinico = idRegistroClinico;
        }

        // Setear vet luego (opcional)
        public void SetVeterinario(int idEmpleadoSesion, string nombreEmpleadoSesion)
        {
            _idEmpleado = idEmpleadoSesion;
            _nombreEmpleado = nombreEmpleadoSesion ?? "";
            PintarVeterinario();
        }

        // ===================== LOAD/SHOWN =====================
        private void AggRegistroClinico_Load(object sender, EventArgs e)
        {
            if (_loadedOnce) return;
            _loadedOnce = true;

            try { if (fechahistorial != null) fechahistorial.Text = DateTime.Now.ToString("dd/MM/yyyy"); } catch { }

            // Completa vet desde la sesión si no vino por ctor
            AsegurarVetDesdeSesion();
            PintarVeterinario();

            // Receta multilínea + Enter = salto de línea
            HabilitarRecetaMultilinea();

            // Cabecera: Mascota / Propietario
            LlenarCabeceraAuto();

            // Si vinimos a editar, carga campos del registro y asegura cabecera
            if (_idRegistroClinico.HasValue)
                CargarRegistroClinicoExistente(_idRegistroClinico.Value);
        }

        private void AggRegistroClinico_Shown(object sender, EventArgs e)
        {
            // Por si llamaron SetVeterinario después del Load
            PintarVeterinario();
        }

        private void PintarVeterinario()
        {
            if (string.IsNullOrWhiteSpace(_nombreEmpleado)) return;
            TrySetText(_nombreEmpleado, "txtVeterinario");
            TryBloquear("txtVeterinario");
        }

        // Toma el vet de SesionActual si no fue provisto
        private void AsegurarVetDesdeSesion()
        {
            try
            {
                if ((!_idEmpleado.HasValue || _idEmpleado.Value <= 0) || string.IsNullOrWhiteSpace(_nombreEmpleado))
                {
                    _idEmpleado = SesionActual.IdEmpleado;
                    _nombreEmpleado = SesionActual.NombreEmpleado ?? "";
                }
            }
            catch { /* si no existe SesionActual, no romper */ }
        }

        // ===================== CABECERA (AUTO) =====================
        private void LlenarCabeceraAuto()
        {
            if (_cargando) return;
            _cargando = true;

            try
            {
                if (_idCita.HasValue && CargarDesdeCita(_idCita.Value)) return;
                if (_idPropietario.HasValue && _idMascota.HasValue &&
                    CargarDesdePropietarioMascota(_idPropietario.Value, _idMascota.Value)) return;
                if (_idMascota.HasValue && CargarDesdeMascota(_idMascota.Value)) return;
            }
            finally { _cargando = false; }
        }

        private bool CargarDesdeCita(int idCita)
        {
            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1
                           gc.IdPropietario,
                           gc.IdMascota,
                           gc.Motivo,                          -- motivo de la cita
                           m.Nombre AS Mascota,
                           m.Especie,
                           m.Raza,
                           (pepr.Nombre + ' ' + pepr.Apellido) AS Propietario
                    FROM GestionCita gc
                    JOIN Mascota m       ON m.IdMascota      = gc.IdMascota
                    JOIN Propietario pr  ON pr.IdPropietario = gc.IdPropietario
                    JOIN Persona pepr    ON pepr.IdPersona   = pr.IdPersona
                    WHERE gc.IdCita = @id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    using (var rd = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!rd.Read()) return false;

                        _idPropietario = Convert.ToInt32(rd["IdPropietario"]);
                        _idMascota = Convert.ToInt32(rd["IdMascota"]);

                        // Cabecera
                        PintarCabecera(
                            propietario: rd["Propietario"]?.ToString(),
                            mascota: rd["Mascota"]?.ToString(),
                            especie: rd["Especie"]?.ToString(),
                            raza: rd["Raza"]?.ToString()
                        );

                        // Motivo desde la cita (si hay)
                        var motivo = rd["Motivo"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(motivo))
                            TrySetText(motivo, "txtMotivo");

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar desde la cita: " + ex.Message);
                return false;
            }
            finally { _db.CerrarConexion(); }
        }

        private bool CargarDesdePropietarioMascota(int idPropietario, int idMascota)
        {
            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1
                           m.IdMascota,
                           m.Nombre AS Mascota,
                           m.Especie,
                           m.Raza,
                           (pe.Nombre + ' ' + pe.Apellido) AS Propietario
                    FROM PropietarioMascota pm
                    JOIN Mascota   m ON m.IdMascota   = pm.IdMascota
                    JOIN Propietario pr ON pr.IdPropietario = pm.IdPropietario
                    JOIN Persona     pe ON pe.IdPersona     = pr.IdPersona
                    WHERE pm.IdPropietario = @p AND pm.IdMascota = @m;", cn))
                {
                    cmd.Parameters.AddWithValue("@p", idPropietario);
                    cmd.Parameters.AddWithValue("@m", idMascota);

                    using (var rd = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!rd.Read()) return false;

                        _idMascota = Convert.ToInt32(rd["IdMascota"]);
                        PintarCabecera(
                            propietario: rd["Propietario"]?.ToString(),
                            mascota: rd["Mascota"]?.ToString(),
                            especie: rd["Especie"]?.ToString(),
                            raza: rd["Raza"]?.ToString()
                        );
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar mascota del propietario: " + ex.Message);
                return false;
            }
            finally { _db.CerrarConexion(); }
        }

        private bool CargarDesdeMascota(int idMascota)
        {
            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1
                           m.IdMascota,
                           m.Nombre AS Mascota,
                           m.Especie,
                           m.Raza,
                           (pe.Nombre + ' ' + pe.Apellido) AS Propietario
                    FROM Mascota m
                    LEFT JOIN PropietarioMascota pm ON pm.IdMascota    = m.IdMascota
                    LEFT JOIN Propietario pr       ON pr.IdPropietario = pm.IdPropietario
                    LEFT JOIN Persona     pe       ON pe.IdPersona     = pr.IdPersona
                    WHERE m.IdMascota = @m;", cn))
                {
                    cmd.Parameters.AddWithValue("@m", idMascota);
                    using (var rd = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!rd.Read()) return false;

                        _idMascota = Convert.ToInt32(rd["IdMascota"]);
                        PintarCabecera(
                            propietario: rd["Propietario"]?.ToString(),
                            mascota: rd["Mascota"]?.ToString(),
                            especie: rd["Especie"]?.ToString(),
                            raza: rd["Raza"]?.ToString()
                        );
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar por mascota: " + ex.Message);
                return false;
            }
            finally { _db.CerrarConexion(); }
        }

        private void PintarCabecera(string propietario, string mascota, string especie, string raza)
        {
            // Propietario puede ser txtPaciente o txtPropietario (según diseñador)
            TrySetText(propietario, "txtPaciente", "txtPropietario");
            TrySetText(mascota, "txtMascota");
            TrySetText(especie, "txtEspecie");
            TrySetText(raza, "txtRaza");

            TryBloquear("txtPaciente", "txtPropietario", "txtMascota", "txtEspecie", "txtRaza");
        }

        // ===================== EDICIÓN (REGISTRO CLÍNICO) =====================
        private void CargarRegistroClinicoExistente(int idRegistro)
        {
            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand(@"
                    SELECT rc.IdMascota,
                           rc.MotivoConsulta,
                           rc.Diagnostico,
                           rc.Tratamiento,
                           rc.AplicacionTratamiento
                    FROM RegistroClinico rc
                    WHERE rc.IdRegistroClinico = @id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", idRegistro);
                    using (var rd = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (rd.Read())
                        {
                            if (rd["IdMascota"] != DBNull.Value)
                                _idMascota = Convert.ToInt32(rd["IdMascota"]);

                            TrySetText(rd["MotivoConsulta"]?.ToString(), "txtMotivo");
                            TrySetText(rd["Diagnostico"]?.ToString(), "txtDiagnostico");
                            TrySetText(rd["Tratamiento"]?.ToString(), "txtTratamiento");
                            TrySetText(rd["AplicacionTratamiento"]?.ToString(), "txtReceta");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el registro clínico: " + ex.Message);
            }
            finally { _db.CerrarConexion(); }

            if (_idMascota.HasValue) CargarDesdeMascota(_idMascota.Value);
        }

        // ===================== GUARDAR =====================
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            AsegurarVetDesdeSesion();

            if (!ValidarCamposObligatorios(out string msg))
            {
                MessageBox.Show(msg, "Campos obligatorios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string motivo = GetText("txtMotivo");
            string diagnostico = GetText("txtDiagnostico");
            string tratamiento = GetText("txtTratamiento");
            string receta = GetText("txtReceta");

            var cn = _db.AbrirConexion();
            try
            {
                if (_idRegistroClinico.HasValue)
                {
                    using (var cmd = new SqlCommand(@"
                        UPDATE RegistroClinico
                           SET MotivoConsulta=@motivo,
                               Diagnostico=@diag,
                               Tratamiento=@trat,
                               AplicacionTratamiento=@receta,
                               IdVeterinario=@vet
                         WHERE IdRegistroClinico=@id;", cn))
                    {
                        cmd.Parameters.AddWithValue("@motivo", motivo);
                        cmd.Parameters.AddWithValue("@diag", diagnostico);
                        cmd.Parameters.AddWithValue("@trat", tratamiento);
                        cmd.Parameters.AddWithValue("@receta", receta);
                        cmd.Parameters.AddWithValue("@vet", _idEmpleado.Value);
                        cmd.Parameters.AddWithValue("@id", _idRegistroClinico.Value);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Registro actualizado ✅");
                }
                else
                {
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO RegistroClinico
                            (IdMascota, IdVeterinario, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento, FechaRegistro)
                        VALUES (@m, @vet, @motivo, @diag, @trat, @receta, GETDATE());", cn))
                    {
                        cmd.Parameters.AddWithValue("@m", _idMascota.Value);
                        cmd.Parameters.AddWithValue("@vet", _idEmpleado.Value);
                        cmd.Parameters.AddWithValue("@motivo", motivo);
                        cmd.Parameters.AddWithValue("@diag", diagnostico);
                        cmd.Parameters.AddWithValue("@trat", tratamiento);
                        cmd.Parameters.AddWithValue("@receta", receta);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Registro clínico guardado ✅");
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException ex) { MessageBox.Show("Error SQL: " + ex.Message); }
            catch (Exception ex) { MessageBox.Show("No se pudo guardar: " + ex.Message); }
            finally { _db.CerrarConexion(); }
        }

        // ===================== VALIDACIÓN =====================
        private bool ValidarCamposObligatorios(out string mensaje)
        {
            var faltan = new System.Collections.Generic.List<string>();

            // Propietario puede estar en txtPaciente o txtPropietario
            string propietario = GetText("txtPaciente");
            if (string.IsNullOrWhiteSpace(propietario))
                propietario = GetText("txtPropietario");
            if (string.IsNullOrWhiteSpace(propietario)) faltan.Add("Propietario/Paciente");

            if (string.IsNullOrWhiteSpace(GetText("txtMascota"))) faltan.Add("Mascota");
            if (string.IsNullOrWhiteSpace(GetText("txtMotivo"))) faltan.Add("Motivo");
            if (string.IsNullOrWhiteSpace(GetText("txtDiagnostico"))) faltan.Add("Diagnóstico");
            if (string.IsNullOrWhiteSpace(GetText("txtTratamiento"))) faltan.Add("Tratamiento");
            if (string.IsNullOrWhiteSpace(GetText("txtReceta"))) faltan.Add("Receta");
            if (string.IsNullOrWhiteSpace(GetText("txtVeterinario"))) faltan.Add("Veterinario");

            if (!_idMascota.HasValue) faltan.Add("IdMascota (no se identificó la mascota)");
            if (!_idEmpleado.HasValue || _idEmpleado.Value <= 0) faltan.Add("Veterinario de sesión (IdEmpleado)");

            if (faltan.Count > 0)
            {
                mensaje = "Completa los siguientes campos:\n• " + string.Join("\n• ", faltan);
                return false;
            }

            mensaje = null;
            return true;
        }

        // ===================== HELPERS UI (compatibles con Guna2) =====================
        private void TrySetText(string value, params string[] names)
        {
            foreach (var n in names)
            {
                var c = Controls.Find(n, true).FirstOrDefault();
                if (c == null) continue;

                if (c is ComboBox cb)
                {
                    cb.Items.Clear();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        cb.Items.Add(value);
                        cb.SelectedIndex = 0;
                    }
                    continue;
                }

                try { c.Text = value ?? ""; } catch { }
            }
        }

        private void TryBloquear(params string[] names)
        {
            foreach (var n in names)
            {
                var c = Controls.Find(n, true).FirstOrDefault();
                if (c == null) continue;

                try
                {
                    var ro = c.GetType().GetProperty("ReadOnly");
                    if (ro != null) ro.SetValue(c, true, null);
                    else c.Enabled = false;

                    var ts = c.GetType().GetProperty("TabStop");
                    if (ts != null) ts.SetValue(c, false, null);
                }
                catch { c.Enabled = false; }
            }
        }

        private string GetText(string name)
        {
            var c = Controls.Find(name, true).FirstOrDefault();
            try { return c?.Text?.Trim() ?? string.Empty; } catch { return string.Empty; }
        }

        // Receta multilínea + Enter = salto de línea (sin disparar AcceptButton)
        private IButtonControl _prevAcceptButton = null;
        private void HabilitarRecetaMultilinea()
        {
            var c = Controls.Find("txtReceta", true).FirstOrDefault();
            if (c == null) return;

            try
            {
                var ml = c.GetType().GetProperty("Multiline");
                if (ml != null) ml.SetValue(c, true, null);

                var ar = c.GetType().GetProperty("AcceptsReturn");
                if (ar != null) ar.SetValue(c, true, null);

                c.Enter += (s, e) => { _prevAcceptButton = this.AcceptButton; this.AcceptButton = null; };
                c.Leave += (s, e) => { this.AcceptButton = _prevAcceptButton; };
            }
            catch { }
        }

        // ===================== HANDLERS (del diseñador) =====================
        private void btnCancelar_Click(object sender, EventArgs e) => Close();
        private void txtPropietario_TextChanged(object sender, EventArgs e) { }
        private void txtMascota_TextChanged(object sender, EventArgs e) { }
        private void txtDiagnostico_TextChanged(object sender, EventArgs e) { }
        private void txtMotivo_TextChanged(object sender, EventArgs e) { }
        private void txtTratamiento_TextChanged(object sender, EventArgs e) { }
        private void txtReceta_TextChanged(object sender, EventArgs e) { }
        private void txtVeterinario_TextChanged(object sender, EventArgs e) { }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try { if (fechahistorial != null) fechahistorial.Text = DateTime.Now.ToString("dd/MM/yyyy"); } catch { }
        }
    }
}
