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
            csCRUD crud = new csCRUD();
            DataTable dt = new DataTable();
            ReportDataSource dataset = new ReportDataSource();
            rvwHistorial.LocalReport.DataSources.Clear();
            dt = crud.cargarBDData(@"Select M.Nombre, R.Raza, E.Especie, M.Discapacidad, (P.Nombre + ' ' + P.Apellido) AS Propietario,
RC.MotivoConsulta as Motivo, RC.Diagnostico, RC.Tratamiento, RC.AplicacionTratamiento as Receta,
RC.FechaRegistro as Fecha from RegistroClinico RC
Inner Join Mascota M ON RC.IdMascota = M.IdMascota
Inner Join Especie E ON E.IdEspecie = M.IdEspecie
Inner Join Raza R ON R.IdRaza = M.IdRaza
Inner Join Persona P ON P.IdPersona = M.IdPersona 
Where M.IdMascota = @IdMascota", new SqlParameter("@IdMascota", idMascota));
            rvwHistorial.LocalReport.ReportEmbeddedResource = "AniCLinic.rptRegistroClinico.rdlc";
            dataset = new ReportDataSource("dsRegistroClinico", dt);
            rvwHistorial.LocalReport.DataSources.Add(dataset);
            dataset.Value = dt;
            rvwHistorial.LocalReport.Refresh();

            this.rvwHistorial.RefreshReport();
        }
    }
}
