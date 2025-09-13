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
    public partial class frFactura : Form
    {
        int idFactura;
        public frFactura(int idFactura)
        {
            InitializeComponent();
            this.idFactura = idFactura;
        }

        private void frFactura_Load(object sender, EventArgs e)
        {
            csCRUD crud = new csCRUD();
            DataTable dt = new DataTable();
            ReportDataSource dataset = new ReportDataSource();
            rvwFactura.LocalReport.DataSources.Clear();
            dt = crud.cargarBDData("Select V.CantidadVendida as Cantidad, I.NombreProducto as Producto, V.PrecioUnitario as Precio, "+
                                    " V.Total as Total, F.NumeroFactura as NumeroF, F.Subtotal as Subtotal, F.IVA as IVA, F.Total as TotalVenta" +
                                    " from[Venta Inventario] V "+
                                    " Inner join DetalleFactura D ON V.IdVenta = D.IdVenta "+
                                    " Inner join Factura F ON F.IdFactura = D.IdFactura"+
                                    " Inner join Inventario I ON V.IdProducto = I.IdProducto"+
                                    " Where F.IdFactura = @IdFactura", new SqlParameter("@IdFactura", idFactura));
            rvwFactura.LocalReport.ReportEmbeddedResource = "AniCLinic.rptFactura.rdlc";
            dataset = new ReportDataSource("dsFactura", dt);
            rvwFactura.LocalReport.DataSources.Add(dataset);
            dataset.Value = dt;
            rvwFactura.LocalReport.Refresh();
            this.rvwFactura.RefreshReport();

            this.rvwFactura.RefreshReport();
        }
    }
}
