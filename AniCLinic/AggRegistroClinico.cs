using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggRegistroClinico : Form
    {
        private readonly bool _isEdit;
        private readonly CitaInfo _info;

        // si existe un registro ese día, guardamos su Id para UPDATE
        private int _idRegistroClinicoExistente = 0;

        public AggRegistroClinico(CitaInfo info, bool isEdit)
        {
            InitializeComponent();

            _info = info ?? throw new ArgumentNullException("info");
            _isEdit = isEdit;

            Text = _isEdit ? "Editar Registro Clínico" : "Registrar Atención";
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;

            // Cabecera solo lectura
            if (txtPropietario != null) txtPropietario.ReadOnly = true;
            if (txtMascota != null) txtMascota.ReadOnly = true;
            if (txtMotivo != null) txtMotivo.ReadOnly = true;

            // === Veterinario igual que en AggCita ===
            if (txtVeterinario != null)
            {
                txtVeterinario.ReadOnly = true;
                txtVeterinario.TabStop = false;
                txtVeterinario.Text = CedulaUtils.VeterinarioDeSesion() ?? string.Empty;
            }

            // Wire botones
            try { btnAceptar.Click -= BtnGuardar_Click; } catch { }
            try { btnCancelar.Click -= BtnCancelar_Click; } catch { }
            btnAceptar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;

            try { this.Load -= AggRegistroClinico_Load; } catch { }

            // Cabecera desde la cita (solo nombre de la mascota)
            CargarCabeceraDesdeCita();

            // Pre-llenar motivo por si viene desde la cita
            if (txtMotivo != null) txtMotivo.Text = _info.Motivo ?? "";

            // Si ya hay registro hoy para esa mascota, precargar para editar
            CargarRegistroClinicoDelDia(_info.IdMascota, _info.FechaHora.Date);
        }

        // Constructor vacío (si el diseñador lo requiere)
        public AggRegistroClinico()
        {
            InitializeComponent();
        }

        private void CargarCabeceraDesdeCita()
        {
            if (txtPropietario != null) txtPropietario.Text = _info.Propietario ?? "";
            // SOLO nombre de la mascota
            if (txtMascota != null) txtMascota.Text = _info.Mascota ?? "";
            if (txtMotivo != null) txtMotivo.Text = _info.Motivo ?? "";
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
                // Validación completa
                if (!ValidarCampos()) return;

                GuardarRegistroClinico();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el registro clínico:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== Validación de campos requeridos =====
        private bool ValidarCampos()
        {
            if (txtMotivo == null || string.IsNullOrWhiteSpace(txtMotivo.Text))
            { MessageBox.Show("Ingrese el motivo de la consulta."); if (txtMotivo != null) txtMotivo.Focus(); return false; }

            if (txtDiagnostico == null || string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            { MessageBox.Show("Ingrese el diagnóstico."); if (txtDiagnostico != null) txtDiagnostico.Focus(); return false; }

            if (txtTratamiento == null || string.IsNullOrWhiteSpace(txtTratamiento.Text))
            { MessageBox.Show("Ingrese el tratamiento."); if (txtTratamiento != null) txtTratamiento.Focus(); return false; }

            if (txtReceta == null || string.IsNullOrWhiteSpace(txtReceta.Text))
            { MessageBox.Show("Ingrese la receta / aplicación."); if (txtReceta != null) txtReceta.Focus(); return false; }

            if (txtVeterinario == null || string.IsNullOrWhiteSpace(txtVeterinario.Text))
            { MessageBox.Show("No se detectó el veterinario de la sesión."); return false; }

            return true;
        }

        private void CargarRegistroClinicoDelDia(int idMascota, DateTime fecha)
        {
            string sql = @"
SELECT TOP(1) IdRegistroClinico, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento
FROM RegistroClinico
WHERE IdMascota = @m AND CONVERT(date, FechaRegistro) = @f
ORDER BY FechaRegistro DESC, IdRegistroClinico DESC;";

            csCRUD crud = new csCRUD();
            DataTable dt = crud.cargarBDData(sql,
                new SqlParameter("@m", idMascota),
                new SqlParameter("@f", fecha));

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                _idRegistroClinicoExistente = Convert.ToInt32(row["IdRegistroClinico"]);
                if (txtMotivo != null) txtMotivo.Text = Convert.ToString(row["MotivoConsulta"] ?? "");
                if (txtDiagnostico != null) txtDiagnostico.Text = Convert.ToString(row["Diagnostico"] ?? "");
                if (txtTratamiento != null) txtTratamiento.Text = Convert.ToString(row["Tratamiento"] ?? "");
                if (txtReceta != null) txtReceta.Text = Convert.ToString(row["AplicacionTratamiento"] ?? "");
            }
            else
            {
                _idRegistroClinicoExistente = 0;
            }
        }

        private void GuardarRegistroClinico()
        {
            csCRUD crud = new csCRUD();

            if (_idRegistroClinicoExistente > 0)
            {
                // UPDATE
                string sqlU = @"
UPDATE RegistroClinico
SET MotivoConsulta = @mot,
    Diagnostico    = @diag,
    Tratamiento    = @trat,
    AplicacionTratamiento = @apli
WHERE IdRegistroClinico = @id;";

                crud.editarBD(sqlU,
                    new SqlParameter("@mot", txtMotivo.Text ?? ""),
                    new SqlParameter("@diag", txtDiagnostico.Text ?? ""),
                    new SqlParameter("@trat", txtTratamiento.Text ?? ""),
                    new SqlParameter("@apli", txtReceta.Text ?? ""),
                    new SqlParameter("@id", _idRegistroClinicoExistente)
                );
            }
            else
            {
                // INSERT
                string sqlI = @"
INSERT INTO RegistroClinico
    (IdMascota, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento, FechaRegistro)
VALUES
    (@m, @mot, @diag, @trat, @apli, @f);";

                crud.agregarBD(sqlI,
                    new SqlParameter("@m", _info.IdMascota),
                    new SqlParameter("@mot", txtMotivo.Text ?? ""),
                    new SqlParameter("@diag", txtDiagnostico.Text ?? ""),
                    new SqlParameter("@trat", txtTratamiento.Text ?? ""),
                    new SqlParameter("@apli", txtReceta.Text ?? ""),
                    new SqlParameter("@f", _info.FechaHora.Date)
                );
            }
        }

        private void AggRegistroClinico_Load(object sender, EventArgs e) { }
    }
}
