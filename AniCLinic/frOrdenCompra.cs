using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace AniCLinic
{
    public partial class frOrdenCompra : Form
    {
        private readonly csCRUD _crud = new csCRUD();
        private readonly int _idOrden;

        // === Ajusta SOLO si en tu RDLC los datasets tienen otros nombres ===
        private const string DS_CAB = "CabeceraOC";
        private const string DS_DET = "DetallesOC";

        // Opción A: RDLC incrustado como Embedded Resource (Build Action = Embedded Resource)
        // Verifica el namespace + carpeta exactos en tu proyecto
        private const string RDLC_EMBED = "AniCLinic.rptOrdenCompra.rdlc";

        // Opción B: RDLC por archivo (Build Action = Content, Copy to Output = Copy if newer)
        private const string RDLC_FILE_RELATIVE = @"Reportes\rptOrdenCompra.rdlc";

        public frOrdenCompra(int idOrden)
        {
            InitializeComponent();
            _idOrden = idOrden;
            this.Load += frOrdenCompra_Load;
        }

        private void frOrdenCompra_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable cab = _crud.cargarBDData("EXEC dbo.usp_OC_Cab @p0",
                    new SqlParameter("@p0", _idOrden));
                DataTable det = _crud.cargarBDData("EXEC dbo.usp_OC_Det @p0",
                    new SqlParameter("@p0", _idOrden));

                reportViewer1.Reset();
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                reportViewer1.LocalReport.DataSources.Clear();

                // 🔹 Nombre EXACTO del recurso incrustado en tu ensamblado
                reportViewer1.LocalReport.ReportEmbeddedResource = "AniCLinic.rptOrdenCompra.rdlc";

                // 🔹 Los nombres de dataset deben coincidir con el RDLC
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("CabeceraOC", cab));
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DetallesOC", det));

                reportViewer1.ZoomMode = ZoomMode.PageWidth;
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el reporte:\n" + ex.Message,
                    "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private static bool EmbeddedExists(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetManifestResourceNames()
                      .Any(n => n.Equals(resourceName, StringComparison.OrdinalIgnoreCase));
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
