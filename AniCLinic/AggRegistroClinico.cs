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

        private const string SCH = "dbo";
        private const string TBL = "RegistroClinico";

        // PK para editar/eliminar el del día si existe
        private int _idRegistroClinicoExistente = 0;

        public AggRegistroClinico(CitaInfo info, bool isEdit)
        {
            InitializeComponent();

            _info = info ?? throw new ArgumentNullException("info");
            _isEdit = isEdit;

            Text = _isEdit ? "Editar Registro Clínico" : "Registrar Atención";
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;

            // Cabecera
            txtPropietario.ReadOnly = true;
            txtMascota.ReadOnly = true;
            txtMotivo.ReadOnly = true;
            txtVeterinario.ReadOnly = true;

            btnAceptar.Click -= BtnGuardar_Click;
            btnCancelar.Click -= BtnCancelar_Click;
            btnAceptar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;

            try { this.Load -= AggRegistroClinico_Load; } catch { }

            // Mostrar datos base
            CargarCabeceraDesdeCita();

            // Prefill de motivo clínico (si lo usas en otro TextBox, muévelo allí)
            if (txtMotivo != null) txtMotivo.Text = _info.Motivo ?? "";

            // Cargar del día (si ya se registró algo para esa cita)
            CargarRegistroClinicoDelDia(_info.IdMascota, _info.IdVeterinario, _info.FechaHora.Date);
        }

        public AggRegistroClinico()
        {
            InitializeComponent();
        }

        private void CargarCabeceraDesdeCita()
        {
            txtPropietario.Text = _info.Propietario ?? "";
            txtMascota.Text = string.Format("{0} ({1}/{2})", _info.Mascota, _info.Especie, _info.Raza);
            txtMotivo.Text = _info.Motivo ?? "";
            txtVeterinario.Text = _info.Veterinario ?? "";
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

        // ========== Cargar (si existe) por IdMascota + IdVeterinario + fecha ==========
        private void CargarRegistroClinicoDelDia(int idMascota, int idVet, DateTime fecha)
        {
            string sql = string.Format(@"
SELECT TOP(1) IdRegistroClinico, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento
FROM {0}.{1}
WHERE IdMascota = @m AND IdVeterinario = @v AND CONVERT(date, FechaRegistro) = @f
ORDER BY FechaRegistro DESC, IdRegistroClinico DESC;", SCH, TBL);

            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMascota;
                    cmd.Parameters.Add("@v", SqlDbType.Int).Value = idVet;
                    cmd.Parameters.Add("@f", SqlDbType.Date).Value = fecha;

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            _idRegistroClinicoExistente = Convert.ToInt32(rd["IdRegistroClinico"]);
                            txtDiagnostico.Text = Convert.ToString(rd["Diagnostico"] ?? "");
                            txtTratamiento.Text = Convert.ToString(rd["Tratamiento"] ?? "");
                            // txtReceta lo usamos como "Aplicación de Tratamiento"
                            txtReceta.Text = Convert.ToString(rd["AplicacionTratamiento"] ?? "");
                            // Si tienes un TextBox para MotivoConsulta, cárgalo aquí:
                            // txtMotivoConsulta.Text    = Convert.ToString(rd["MotivoConsulta"] ?? "");
                        }
                        else
                        {
                            _idRegistroClinicoExistente = 0; // nuevo
                        }
                    }
                }
            }
            finally { db.cerrarConexion(); }
        }

        // ========== Guardar ==========
        private void GuardarRegistroClinico()
        {
            var db = new csConexionBD();
            db.abrirConexion();
            try
            {
                SqlTransaction tx = db.obtenerConexion().BeginTransaction();
                try
                {
                    if (_idRegistroClinicoExistente > 0)
                    {
                        // UPDATE
                        string sqlU = string.Format(@"
UPDATE {0}.{1}
SET MotivoConsulta = @mot,
    Diagnostico = @diag,
    Tratamiento = @trat,
    AplicacionTratamiento = @apli
WHERE IdRegistroClinico = @id;", SCH, TBL);

                        using (var cmd = new SqlCommand(sqlU, db.obtenerConexion(), tx))
                        {
                            cmd.Parameters.Add("@mot", SqlDbType.NVarChar, 300).Value = (object)(txtMotivo.Text ?? "").ToString();
                            cmd.Parameters.Add("@diag", SqlDbType.NVarChar, 800).Value = (object)(txtDiagnostico.Text ?? "").ToString();
                            cmd.Parameters.Add("@trat", SqlDbType.NVarChar, 800).Value = (object)(txtTratamiento.Text ?? "").ToString();
                            cmd.Parameters.Add("@apli", SqlDbType.NVarChar, 300).Value = (object)(txtReceta.Text ?? "").ToString();
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = _idRegistroClinicoExistente;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // INSERT
                        string sqlI = string.Format(@"
INSERT INTO {0}.{1}
    (IdMascota, IdVeterinario, MotivoConsulta, Diagnostico, Tratamiento, AplicacionTratamiento, FechaRegistro)
VALUES
    (@m, @v, @mot, @diag, @trat, @apli, @f);", SCH, TBL);

                        using (var cmd = new SqlCommand(sqlI, db.obtenerConexion(), tx))
                        {
                            cmd.Parameters.Add("@m", SqlDbType.Int).Value = _info.IdMascota;
                            cmd.Parameters.Add("@v", SqlDbType.Int).Value = _info.IdVeterinario;
                            cmd.Parameters.Add("@mot", SqlDbType.NVarChar, 300).Value = (object)(txtMotivo.Text ?? "").ToString();
                            cmd.Parameters.Add("@diag", SqlDbType.NVarChar, 800).Value = (object)(txtDiagnostico.Text ?? "").ToString();
                            cmd.Parameters.Add("@trat", SqlDbType.NVarChar, 800).Value = (object)(txtTratamiento.Text ?? "").ToString();
                            cmd.Parameters.Add("@apli", SqlDbType.NVarChar, 300).Value = (object)(txtReceta.Text ?? "").ToString();
                            cmd.Parameters.Add("@f", SqlDbType.DateTime2).Value = _info.FechaHora.Date; // guardamos con la fecha de la cita (hora 00:00)
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }
                catch
                {
                    try { tx.Rollback(); } catch { }
                    throw;
                }
            }
            finally { db.cerrarConexion(); }
        }

        private void AggRegistroClinico_Load(object sender, EventArgs e) { }
    }
}
