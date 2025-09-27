using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class frmHistorialMedicoReport : Form
    {
        int idMascota;
        public frmHistorialMedicoReport(int idMasc)
        {
            InitializeComponent();
            idMascota = idMasc;
        }

        private void frmHistorialMedicoReport_Load(object sender, EventArgs e)
        {
            try
            {
                var crud = new csCRUD();

                // IMPORTANTE: incluye IdMascota porque el grupo del tablix lo usa
                string sql = @"
SELECT
    M.IdMascota                   AS IdMascota,
    M.Nombre                      AS Nombre,
    E.Especie                     AS Especie,
    R.Raza                        AS Raza,
    ISNULL(M.Discapacidad,'')     AS Discapacidad,
    (P.Nombre + ' ' + P.Apellido) AS Propietario,
    RC.FechaRegistro              AS Fecha,
    ISNULL(RC.MotivoConsulta,'')  AS Motivo,
    ISNULL(RC.Diagnostico,'')     AS Diagnostico,
    ISNULL(RC.Tratamiento,'')     AS Tratamiento,
    ISNULL(RC.AplicacionTratamiento,'') AS Receta
FROM RegistroClinico RC
JOIN Mascota  M ON RC.IdMascota = M.IdMascota
JOIN Persona  P ON P.IdPersona  = M.IdPersona
JOIN Especie  E ON E.IdEspecie  = M.IdEspecie
JOIN Raza     R ON R.IdRaza     = M.IdRaza
WHERE M.IdMascota = @IdMascota
ORDER BY RC.FechaRegistro DESC, RC.IdRegistroClinico DESC;";

                DataTable dt = crud.cargarBDData(sql, new SqlParameter("@IdMascota", idMascota));
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Esta mascota no tiene fichas registradas.", "Sin datos",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                    return;
                }

                // Configurar ReportViewer
                rvwHistorial.Reset();
                rvwHistorial.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local;
                rvwHistorial.LocalReport.DataSources.Clear();

                // RDLC embebido (ajusta si está en subcarpeta)
                rvwHistorial.LocalReport.ReportEmbeddedResource = "AniCLinic.rptRegistroClinico.rdlc";

                // DataSource: nombre EXACTO como en el RDLC
                rvwHistorial.LocalReport.DataSources.Add(
                    new Microsoft.Reporting.WinForms.ReportDataSource("dsRegistroClinico", dt));

                // *** NO parámetros aquí ***  (los quitaste del RDLC)

                rvwHistorial.RefreshReport();
            }
            catch (Microsoft.Reporting.WinForms.LocalProcessingException ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Error de reporte",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
