using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggRegistroClinico : Form
    {
        private readonly bool _isEdit;
        private readonly bool _esEmergencia;
        private readonly CitaInfo _info;

        private int _idRegistroClinicoExistente = 0;

        public AggRegistroClinico(CitaInfo info, bool isEdit)
            : this(info, isEdit, false) { }

        public AggRegistroClinico(CitaInfo info, bool isEdit, bool esEmergencia)
        {
            InitializeComponent();

            _info = info ?? new CitaInfo { FechaHora = DateTime.Now };
            if (_info.FechaHora == default) _info.FechaHora = DateTime.Now;

            _isEdit = isEdit;
            _esEmergencia = esEmergencia;

            Text = _isEdit ? "Editar Registro Clínico" : "Registrar Atención";
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;

            if (txtPropietario != null) txtPropietario.ReadOnly = true;
            if (txtMascota != null) txtMascota.ReadOnly = true;
            if (txtMotivo != null) txtMotivo.ReadOnly = false; 

            if (txtReceta != null)
            {
                txtReceta.Multiline = true;
                txtReceta.ScrollBars = ScrollBars.Vertical;
                txtReceta.AcceptsReturn = true;
                txtReceta.WordWrap = true;
            }

            try 
            { 
                btnAceptar.Click -= BtnGuardar_Click; 
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try 
            { 
                btnCancelar.Click -= BtnCancelar_Click; 
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnAceptar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;

            if (btnListaMascota != null)
            {
                btnListaMascota.Visible = _esEmergencia;
                try 
                { 
                    btnListaMascota.Click -= BtnListaMascota_Click; 
                } 
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnListaMascota.Click += BtnListaMascota_Click;
            }

            CargarCabeceraDesdeCita();

            if (_esEmergencia && txtMotivo != null && string.IsNullOrWhiteSpace(txtMotivo.Text))
                txtMotivo.Text = "Emergencia";

            if (_isEdit)
            {
                CargarRegistroClinicoPorCita(
                    _info.IdCita > 0 ? (int?)_info.IdCita : null,
                    ResolverIdMascotaRobusto(_info),
                    _info.FechaHora
                );
            }
            else
            {
                // Si no es edición, mantenemos "Emergencia" en el flujo de emergencia
                if (_esEmergencia && txtMotivo != null && string.IsNullOrWhiteSpace(txtMotivo.Text))
                    txtMotivo.Text = "Emergencia";
                else
                    LimpiarCampos();
            }
        }

        public AggRegistroClinico()
        {
            InitializeComponent();
        }

        private void CargarCabeceraDesdeCita()
        {
            if (txtPropietario != null) txtPropietario.Text = _info.Propietario ?? "";
            if (txtMascota != null) txtMascota.Text = _info.Mascota ?? "";
            if (txtMotivo != null && !_esEmergencia)
                txtMotivo.Text = _info.Motivo ?? txtMotivo.Text;
        }

        private void LimpiarCampos()
        {
            _idRegistroClinicoExistente = 0;
            if (txtDiagnostico != null) txtDiagnostico.Clear();
            if (txtTratamiento != null) txtTratamiento.Clear();
            if (txtReceta != null) txtReceta.Clear();
            // txtMotivo se mantiene (puede venir "Emergencia")
        }

        private void CargarRegistroClinicoPorCita(int? idCita, int idMascota, DateTime fechaHora)
        {
            var crud = new csCRUD();

            DataTable dt = null;

            if (idCita.HasValue)
            {
                const string sql = @"
SELECT TOP(1) rc.IdRegistroClinico, rc.MotivoConsulta, rc.Diagnostico, rc.Tratamiento, rc.AplicacionTratamiento
FROM RegistroClinico rc
JOIN GestionCita c ON c.IdCita = @c
WHERE rc.IdMascota = c.IdMascota
  AND DATEDIFF(MINUTE, rc.FechaRegistro, c.FechaHora) = 0
ORDER BY rc.IdRegistroClinico DESC;";
                dt = crud.cargarBDData(sql, new SqlParameter("@c", idCita.Value));
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                const string sql2 = @"
SELECT TOP(1) IdRegistroClinico, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento
FROM RegistroClinico
WHERE IdMascota = @m
  AND DATEDIFF(MINUTE, FechaRegistro, @fh) = 0
ORDER BY IdRegistroClinico DESC;";
                dt = crud.cargarBDData(sql2,
                        new SqlParameter("@m", idMascota),
                        new SqlParameter("@fh", fechaHora));
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                const string sql3 = @"
SELECT TOP(1) IdRegistroClinico, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento
FROM RegistroClinico
WHERE IdMascota = @m AND CONVERT(date, FechaRegistro) = CONVERT(date, @fh)
ORDER BY FechaRegistro DESC, IdRegistroClinico DESC;";
                dt = crud.cargarBDData(sql3,
                        new SqlParameter("@m", idMascota),
                        new SqlParameter("@fh", fechaHora));
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                _idRegistroClinicoExistente = Convert.ToInt32(row["IdRegistroClinico"]);
                if (txtMotivo != null)      txtMotivo.Text      = Convert.ToString(row["MotivoConsulta"] ?? txtMotivo.Text);
                if (txtDiagnostico != null) txtDiagnostico.Text = Convert.ToString(row["Diagnostico"] ?? "");
                if (txtTratamiento != null) txtTratamiento.Text = Convert.ToString(row["Tratamiento"] ?? "");
                if (txtReceta != null)      txtReceta.Text      = Convert.ToString(row["AplicacionTratamiento"] ?? "");
            }
            else
            {
                // En emergencia, si quedara vacío, lo aseguramos
                if (_esEmergencia && txtMotivo != null && string.IsNullOrWhiteSpace(txtMotivo.Text))
                    txtMotivo.Text = "Emergencia";
                else
                    LimpiarCampos();
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCamposObligatorios()) 
                    return;
                GuardarRegistroClinico();
                DialogResult = DialogResult.OK;
                if (!_isEdit)
                {
                    Menu menu = Application.OpenForms["Menu"] as Menu;
                    menu.AbrirEnPanel(menu.pnlMenu1, new Ventas(menu.idEmpleado, true));
                    this.Close();
                }
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error de base de datos:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el registro clínico:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCamposObligatorios()
        {
            // Motivo: en emergencia lo forzamos a "Emergencia" si viniera vacío, pero igual verificamos
            if (txtMotivo != null && string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                if (_esEmergencia)
                {
                    txtMotivo.Text = "Emergencia";
                }
                else
                {
                    MessageBox.Show("Ingrese el motivo de la consulta.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMotivo?.Focus();
                    return false;
                }
            }

            if (txtDiagnostico == null || string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            {
                MessageBox.Show("Ingrese el diagnóstico.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiagnostico?.Focus();
                return false;
            }

            if (txtTratamiento == null || string.IsNullOrWhiteSpace(txtTratamiento.Text))
            {
                MessageBox.Show("Ingrese el tratamiento.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTratamiento?.Focus();
                return false;
            }

            if (txtReceta == null || string.IsNullOrWhiteSpace(txtReceta.Text))
            {
                MessageBox.Show("Ingrese la receta / aplicación del tratamiento.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReceta?.Focus();
                return false;
            }
            return true;
        }

        private void GuardarRegistroClinico()
        {
            var crud = new csCRUD();

            string mot  = (txtMotivo != null)      ? (txtMotivo.Text ?? "").Trim()      : "";
            string diag = (txtDiagnostico != null) ? (txtDiagnostico.Text ?? "").Trim() : "";
            string trat = (txtTratamiento != null) ? (txtTratamiento.Text ?? "").Trim() : "";
            string rec  = (txtReceta != null)      ? (txtReceta.Text ?? "").Trim()      : "";

            if (_esEmergencia && string.IsNullOrWhiteSpace(mot)) 
                mot = "Emergencia";
            if (string.IsNullOrWhiteSpace(mot)) 
                mot = "Emergencia";

            int idMascota = ResolverIdMascotaRobusto(_info);
            if (idMascota <= 0)
                throw new InvalidOperationException("No se pudo resolver la mascota asociada.");

            DateTime fechaHora = _info.FechaHora != default ? _info.FechaHora : DateTime.Now;

            if (_idRegistroClinicoExistente > 0)
            {
                const string sqlU = @"
UPDATE RegistroClinico
   SET MotivoConsulta         = @mot,
       Diagnostico            = @diag,
       Tratamiento            = @trat,
       AplicacionTratamiento  = @apli
 WHERE IdRegistroClinico = @id;";
                crud.editarBD(sqlU,
                    new SqlParameter("@mot", mot),
                    new SqlParameter("@diag", diag),
                    new SqlParameter("@trat", trat),
                    new SqlParameter("@apli", rec),
                    new SqlParameter("@id", _idRegistroClinicoExistente)
                );
            }
            else
            {
                const string sqlI = @"
INSERT INTO RegistroClinico
    (IdMascota, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento, FechaRegistro)
VALUES
    (@m, @mot, @diag, @trat, @apli, @fh);";
                crud.agregarBD(sqlI,
                    new SqlParameter("@m", idMascota),
                    new SqlParameter("@mot", mot),
                    new SqlParameter("@diag", diag),
                    new SqlParameter("@trat", trat),
                    new SqlParameter("@apli", rec),
                    new SqlParameter("@fh", fechaHora)
                );
            }
            
        }

        private int ResolverIdMascotaRobusto(CitaInfo info)
        {
            if (info.IdMascota > 0) return info.IdMascota;

            int id = ResolverIdMascotaPorCita(info.IdCita);
            if (id > 0) 
                return id;

            id = ResolverIdMascotaPorNombreYFecha(info.Mascota, info.FechaHora.Date);
            return id;
        }

        private int ResolverIdMascotaPorCita(int idCita)
        {
            if (idCita <= 0) return 0;

            const string sql = @"SELECT IdMascota FROM dbo.GestionCita WHERE IdCita=@id;";
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    var obj = cmd.ExecuteScalar();
                    return (obj == null || obj == DBNull.Value) ? 0 : Convert.ToInt32(obj);
                }
            }
            finally { db.cerrarConexion(); }
        }

        private int ResolverIdMascotaPorNombreYFecha(string nombreMascota, DateTime fechaCita)
        {
            if (string.IsNullOrWhiteSpace(nombreMascota)) return 0;

            const string sql = @"
;WITH base AS (
    SELECT m.IdMascota
    FROM dbo.Mascota m
    WHERE m.Nombre = @masc
)
SELECT TOP(1) b.IdMascota
FROM base b
LEFT JOIN dbo.GestionCita c
       ON c.IdMascota = b.IdMascota
      AND CONVERT(date, c.FechaHora) = @f
ORDER BY CASE WHEN c.IdCita IS NULL THEN 1 ELSE 0 END,
         b.IdMascota DESC;";
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@masc", nombreMascota.Trim());
                    cmd.Parameters.AddWithValue("@f", fechaCita);
                    var obj = cmd.ExecuteScalar();
                    return (obj == null || obj == DBNull.Value) ? 0 : Convert.ToInt32(obj);
                }
            }
            finally { db.cerrarConexion(); }
        }

        private void BtnListaMascota_Click(object sender, EventArgs e)
        {
            using (var frm = new FRMListaMascota())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    // Rellenar cabecera
                    _info.IdMascota = frm.IdMascotaSel;
                    _info.Mascota = frm.MascotaSel;
                    _info.Propietario = frm.PropietarioSel;

                    if (txtPropietario != null) txtPropietario.Text = _info.Propietario ?? "";
                    if (txtMascota != null)     txtMascota.Text     = _info.Mascota ?? "";

                    // Si el motivo está vacío, pon "Emergencia"
                    if (txtMotivo != null && string.IsNullOrWhiteSpace(txtMotivo.Text))
                        txtMotivo.Text = "Emergencia";

                    // Fecha/hora por defecto ahora si viniera vacía
                    if (_info.FechaHora == default) _info.FechaHora = DateTime.Now;
                }
            }
        }

        private void AggRegistroClinico_Load(object sender, EventArgs e) { }
    }
}
