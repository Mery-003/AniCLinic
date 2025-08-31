using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggRegistroClinico : Form
    {
        private readonly bool _isEdit;
        private readonly CitaInfo _info;

        public AggRegistroClinico(CitaInfo info, bool isEdit)
        {
            InitializeComponent();

            _info = info ?? throw new ArgumentNullException(nameof(info));
            _isEdit = isEdit;

            Text = _isEdit ? "Editar Registro Clínico" : "Registrar Atención";
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;

            // No permitir edición de cabecera
            txtPropietario.ReadOnly = true;
            txtMascota.ReadOnly = true;
            txtMotivo.ReadOnly = true;
            txtVeterinario.ReadOnly = true;

            // Por si el Designer conectó eventos
            btnAceptar.Click -= BtnGuardar_Click;
            btnCancelar.Click -= BtnCancelar_Click;
            btnAceptar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;

            // MUY IMPORTANTE: si el Designer dejó un Load con Fill a "Propietario", lo desconectamos
            try { this.Load -= AggRegistroClinico_Load; } catch { /* si no existe, no pasa nada */ }

            // Cabecera desde la cita
            CargarCabeceraDesdeCita();

            // Si es edición, traer último registro clínico
            CargarRegistroClinicoSiExiste(_info.IdCita);
        }

        // Constructor vacío SOLO para diseñador. No usar en runtime.
        public AggRegistroClinico()
        {
            InitializeComponent();
        }

        private void CargarCabeceraDesdeCita()
        {
            txtPropietario.Text = _info.Propietario ?? "";
            txtMascota.Text = $"{_info.Mascota} ({_info.Especie}/{_info.Raza})";
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

        private void CargarRegistroClinicoSiExiste(int idCita)
        {
            const string sql = @"
SELECT TOP 1 Diagnostico, Tratamiento, Receta
FROM dbo.RegistroClinico
WHERE IdCita = @id
ORDER BY FechaUltimaEdicion DESC;";

            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            txtDiagnostico.Text = rd["Diagnostico"]?.ToString() ?? "";
                            txtTratamiento.Text = rd["Tratamiento"]?.ToString() ?? "";
                            txtReceta.Text = rd["Receta"]?.ToString() ?? "";
                        }
                    }
                }
            }
            finally { db.cerrarConexion(); }
        }

        private void GuardarRegistroClinico()
        {
            const string sqlExiste = "SELECT COUNT(1) FROM dbo.RegistroClinico WHERE IdCita = @id;";
            const string sqlInsert = @"
INSERT INTO dbo.RegistroClinico (IdCita, Diagnostico, Tratamiento, Receta, FechaUltimaEdicion)
VALUES (@id, @diag, @trat, @rec, GETDATE());";
            const string sqlUpdate = @"
UPDATE dbo.RegistroClinico
SET Diagnostico = @diag,
    Tratamiento = @trat,
    Receta      = @rec,
    FechaUltimaEdicion = GETDATE()
WHERE IdCita = @id;";
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var tx = db.obtenerConexion().BeginTransaction())
                {
                    int existe;
                    using (var cmd = new SqlCommand(sqlExiste, db.obtenerConexion(), tx))
                    {
                        cmd.Parameters.AddWithValue("@id", _info.IdCita);
                        existe = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    var sql = (existe > 0) ? sqlUpdate : sqlInsert;
                    using (var cmd = new SqlCommand(sql, db.obtenerConexion(), tx))
                    {
                        cmd.Parameters.AddWithValue("@id", _info.IdCita);
                        cmd.Parameters.AddWithValue("@diag", (object)(txtDiagnostico.Text?.Trim() ?? "") ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@trat", (object)(txtTratamiento.Text?.Trim() ?? "") ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@rec", (object)(txtReceta.Text?.Trim() ?? "") ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    // Marca la cita como registrada
                    using (var cmd = new SqlCommand(
                        "UPDATE dbo.GestionCita SET Registrada = 1 WHERE IdCita = @id;", db.obtenerConexion(), tx))
                    {
                        cmd.Parameters.AddWithValue("@id", _info.IdCita);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
            }
            finally { db.cerrarConexion(); }
        }

        // Si el Designer generó esto, queda vacío (o elimínalo con seguridad)
        private void AggRegistroClinico_Load(object sender, EventArgs e) { }
    }
}
