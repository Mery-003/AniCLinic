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

        private int _idRegistroClinicoExistente = 0;

        public AggRegistroClinico(CitaInfo info, bool isEdit)
        {
            InitializeComponent();

            _info = info ?? throw new ArgumentNullException("info");
            _isEdit = isEdit;

            // Cabecera
            Text = _isEdit ? "Editar Registro Clínico" : "Registrar Atención";
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;

            // ReadOnly para cabecera
            if (txtPropietario != null) txtPropietario.ReadOnly = true;
            if (txtMascota != null) txtMascota.ReadOnly = true;
            if (txtMotivo != null) txtMotivo.ReadOnly = true;

            // Receta multilínea con scroll y saltos de línea
            if (txtReceta != null)
            {
                txtReceta.Multiline = true;
                txtReceta.ScrollBars = ScrollBars.Vertical;
                txtReceta.AcceptsReturn = true;
                txtReceta.WordWrap = true;
            }

            // Eventos
            try { btnAceptar.Click -= BtnGuardar_Click; } catch { }
            try { btnCancelar.Click -= BtnCancelar_Click; } catch { }
            btnAceptar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;

            // Carga de datos en pantalla
            CargarCabeceraDesdeCita();
            CargarRegistroClinicoDelDia(_info.IdMascota, _info.FechaHora.Date);
        }

        public AggRegistroClinico()
        {
            InitializeComponent();
        }

        // --------- Cabecera (solo nombre de la mascota) ----------
        private void CargarCabeceraDesdeCita()
        {
            if (txtPropietario != null) txtPropietario.Text = _info.Propietario ?? "";
            if (txtMascota != null) txtMascota.Text = _info.Mascota ?? "";
            if (txtMotivo != null) txtMotivo.Text = _info.Motivo ?? "";
        }

        // --------- Carga previa del registro clínico del día (si existe) ----------
        private void CargarRegistroClinicoDelDia(int idMascota, DateTime fecha)
        {
            if (idMascota <= 0) idMascota = ResolverIdMascotaRobusto(_info);

            string sql = @"
SELECT TOP(1) IdRegistroClinico, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento
FROM RegistroClinico
WHERE IdMascota = @m AND CONVERT(date, FechaRegistro) = @f
ORDER BY FechaRegistro DESC, IdRegistroClinico DESC;";

            var crud = new csCRUD();
            var dt = crud.cargarBDData(sql,
                new SqlParameter("@m", idMascota),
                new SqlParameter("@f", fecha));

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
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

        // --------- Botones ----------
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCamposObligatorios()) return;

                GuardarRegistroClinico();
                DialogResult = DialogResult.OK;
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
            // Motivo puede venir de la cita; si lo vacías, también lo exigimos
            if (txtMotivo != null && string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show("Ingrese el motivo de la consulta.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMotivo.Focus();
                return false;
            }

            if (txtDiagnostico == null || string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            {
                MessageBox.Show("Ingrese el diagnóstico.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (txtDiagnostico != null) txtDiagnostico.Focus();
                return false;
            }

            if (txtTratamiento == null || string.IsNullOrWhiteSpace(txtTratamiento.Text))
            {
                MessageBox.Show("Ingrese el tratamiento.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (txtTratamiento != null) txtTratamiento.Focus();
                return false;
            }

            if (txtReceta == null || string.IsNullOrWhiteSpace(txtReceta.Text))
            {
                MessageBox.Show("Ingrese la receta / aplicación del tratamiento.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (txtReceta != null) txtReceta.Focus();
                return false;
            }

            // ✅ Veterinario eliminado: no validamos ni mostramos avisos
            return true;
        }

        // --------- Guardar (INSERT/UPDATE) ----------
        private void GuardarRegistroClinico()
        {
            var crud = new csCRUD();

            string mot = (txtMotivo != null) ? (txtMotivo.Text ?? "").Trim() : "";
            string diag = (txtDiagnostico != null) ? (txtDiagnostico.Text ?? "").Trim() : "";
            string trat = (txtTratamiento != null) ? (txtTratamiento.Text ?? "").Trim() : "";
            string rec = (txtReceta != null) ? (txtReceta.Text ?? "").Trim() : "";

            int idMascota = ResolverIdMascotaRobusto(_info);
            if (idMascota <= 0)
                throw new InvalidOperationException("No se pudo resolver la mascota asociada.");

            DateTime fecha = _info.FechaHora.Date;

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
    (@m, @mot, @diag, @trat, @apli, @f);";

                crud.agregarBD(sqlI,
                    new SqlParameter("@m", idMascota),
                    new SqlParameter("@mot", mot),
                    new SqlParameter("@diag", diag),
                    new SqlParameter("@trat", trat),
                    new SqlParameter("@apli", rec),
                    new SqlParameter("@f", fecha)
                );
            }
        }

        // --------- Resolver IdMascota (robusto, sin joins a Persona) ----------
        private int ResolverIdMascotaRobusto(CitaInfo info)
        {
            if (info.IdMascota > 0) return info.IdMascota;

            int id = ResolverIdMascotaPorCita(info.IdCita);
            if (id > 0) return id;

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

        // Prioriza la mascota que tenga cita en esa fecha; si no hay, toma cualquiera con ese nombre.
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
ORDER BY CASE WHEN c.IdCita IS NULL THEN 1 ELSE 0 END,  -- prefiero con cita ese día
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

        // Diseñador
        private void AggRegistroClinico_Load(object sender, EventArgs e) { }
    }
}
