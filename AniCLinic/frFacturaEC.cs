using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class frFacturaEC : Form
    {
        private readonly int idFacturaEC;
        private readonly csCRUD crud = new csCRUD();

        public frFacturaEC(int idFacturaEC)
        {
            InitializeComponent();
            this.idFacturaEC = idFacturaEC;
        }

        private void frFacturaEC_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = crud.cargarBDData(
                    "SELECT * FROM dbo.vwFacturaEC_Rdlc WHERE IdFacturaEC = @Id",
                    new SqlParameter("@Id", idFacturaEC)
                );

                reportViewer1.Reset();
                reportViewer1.ProcessingMode = ProcessingMode.Local;

                // Si el RDLC está embebido:
                reportViewer1.LocalReport.ReportEmbeddedResource = "AniCLinic.rptFacturaEC.rdlc";
                // Si NO está embebido, usa:
                // reportViewer1.LocalReport.ReportPath = System.IO.Path.Combine(Application.StartupPath, "rptFacturaEC.rdlc");

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsFacturaEC", dt));
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la factura: " + ex.Message);
            }
        }
    }
}
