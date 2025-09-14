using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class VerHistorial : Form
    {
        private readonly int _idRC;

        public VerHistorial(int idRegistroClinico)
        {
            _idRC = idRegistroClinico;
            InitializeComponent();

            this.Load -= VerHistorial_Load;
            this.Load += VerHistorial_Load;
        }

        private void VerHistorial_Load(object sender, EventArgs e)
        {
            try
            {
                var crud = new csCRUD();
                string sql = @"
SELECT
    rc.FechaRegistro,
    ISNULL(rc.MotivoConsulta,'')        AS MotivoConsulta,
    ISNULL(rc.Diagnostico,'')           AS Diagnostico,
    ISNULL(rc.Tratamiento,'')           AS Tratamiento,
    ISNULL(rc.AplicacionTratamiento,'') AS Receta,
    m.Nombre                            AS Mascota,
    (p.Nombre + ' ' + p.Apellido)       AS Propietario
FROM RegistroClinico rc
JOIN Mascota  m ON m.IdMascota  = rc.IdMascota
JOIN Persona  p ON p.IdPersona  = m.IdPersona
WHERE rc.IdRegistroClinico = @id;";

                var dt = crud.cargarBDData(sql, new SqlParameter("@id", _idRC));
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontró la ficha.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var r = dt.Rows[0];

                SetTextSafe("lblFechas", ToFecha(r, "FechaRegistro"));
                SetTextSafe("lblPacientes", Convert.ToString(r["Mascota"] ?? ""));
                SetTextSafe("lblPropietarios", Convert.ToString(r["Propietario"] ?? ""));
                SetTextSafe("lblMotivos", Convert.ToString(r["MotivoConsulta"] ?? ""));
                SetTextSafe("lblDiagnosticos", Convert.ToString(r["Diagnostico"] ?? ""));
                SetTextSafe("lblTratamientos", Convert.ToString(r["Tratamiento"] ?? ""));
                SetTextSafe("lblRecetas", Convert.ToString(r["Receta"] ?? ""));
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la ficha:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ToFecha(DataRow r, string col)
        {
            if (r[col] == DBNull.Value) return "";
            if (DateTime.TryParse(r[col].ToString(), out var d))
                return d.ToString("dd/MM/yyyy HH:mm");
            return Convert.ToString(r[col]);
        }

        private void SetTextSafe(string controlName, string value)
        {
            var ctrl = FindControlRec(this, controlName);
            if (ctrl != null) ctrl.Text = value ?? "";
        }

        private Control FindControlRec(Control root, string name)
        {
            if (root == null) return null;
            if (root.Name == name) return root;
            foreach (Control c in root.Controls)
            {
                var f = FindControlRec(c, name);
                if (f != null) return f;
            }
            return null;
        }

        private void btnCerrar_Click(object sender, EventArgs e) => Close();

       


        private void btnCerrar_Click_2(object sender, EventArgs e)
        {
            this.Close();

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblTratamientos_Click(object sender, EventArgs e)
        {

        }
    }
}
