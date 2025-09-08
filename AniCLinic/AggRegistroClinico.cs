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

            Text = _isEdit ? "Editar Registro Clínico" : "Registrar Atención";
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;

            txtPropietario.ReadOnly = true;
            txtMascota.ReadOnly = true;
            txtMotivo.ReadOnly = true;

            btnAceptar.Click -= BtnGuardar_Click;
            btnCancelar.Click -= BtnCancelar_Click;
            btnAceptar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;

            try { this.Load -= AggRegistroClinico_Load; } catch { }

            CargarCabeceraDesdeCita();

            if (txtMotivo != null) txtMotivo.Text = _info.Motivo ?? "";

            CargarRegistroClinicoDelDia(_info.IdMascota, _info.FechaHora.Date);
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
                txtDiagnostico.Text = Convert.ToString(row["Diagnostico"] ?? "");
                txtTratamiento.Text = Convert.ToString(row["Tratamiento"] ?? "");
                txtReceta.Text = Convert.ToString(row["AplicacionTratamiento"] ?? "");
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
    Diagnostico = @diag,
    Tratamiento = @trat,
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
