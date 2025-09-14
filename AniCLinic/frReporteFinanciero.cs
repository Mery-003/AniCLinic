using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace AniCLinic
{
    public partial class frReporteFinanciero : Form
    {
        string Año;
        string Condicion;
        public frReporteFinanciero(string año, string condicion)
        {
            InitializeComponent();
            Año = año;
            Condicion = condicion;
        }

        private void frReporteFinanciero_Load(object sender, EventArgs e)
        {
            csCRUD crud = new csCRUD();
            DataTable dt = new DataTable();
            ReportDataSource dataset = new ReportDataSource();
            rvwReporteAño.LocalReport.DataSources.Clear();
            if(Condicion == "Mes")
            {
                if (string.IsNullOrWhiteSpace(Año))
                    dt = crud.cargarBDData("Select DATEPART(MONTH, FechaFactura) as Mes, sum(Total) as Total from Factura " +
                    "group by DATEPART(MONTH, FechaFactura)", new SqlParameter("@Año", Año));
                else 
                    dt = crud.cargarBDData("Select DATEPART(MONTH, FechaFactura) as Mes, sum(Total) as Total from Factura " +
                    "Where YEAR(FechaFactura) = @Año " +
                    "group by DATEPART(MONTH, FechaFactura)", new SqlParameter("@Año", Año));
                rvwReporteAño.LocalReport.ReportEmbeddedResource = "AniCLinic.rptReporteAño.rdlc";
                dataset = new ReportDataSource("dsReporteFinanciero", dt);
                rvwReporteAño.LocalReport.DataSources.Add(dataset);
                dataset.Value = dt;
                rvwReporteAño.LocalReport.Refresh();
            }
            if (Condicion == "Producto")
            {
                if (string.IsNullOrWhiteSpace(Año))
                    dt = crud.cargarBDData("Select top 5 I.IdProducto as ID, I.NombreProducto as Nombre, sum(V.Total) as Total from Factura F " +
    "inner join DetalleFactura D ON D.IdFactura=F.IdFactura " +
    "inner join [Venta Inventario] V ON D.IdVenta = V.IdVenta " +
    "inner join Inventario I ON I.IdProducto = V.IdProducto " +
    "Group by I.IdProducto, I.NombreProducto Order by Total desc",
    new SqlParameter("@Año", Año));
                else
                    dt = crud.cargarBDData("Select top 5 I.IdProducto as ID, I.NombreProducto as Nombre, sum(V.Total) as Total from Factura F " +
    "inner join DetalleFactura D ON D.IdFactura=F.IdFactura " +
    "inner join [Venta Inventario] V ON D.IdVenta = V.IdVenta " +
    "inner join Inventario I ON I.IdProducto = V.IdProducto " +
    "Where year(F.FechaFactura) = @Año " +
    "Group by I.IdProducto, I.NombreProducto Order by Total desc",
    new SqlParameter("@Año", Año));
                rvwReporteAño.LocalReport.ReportEmbeddedResource = "AniCLinic.rptReporteProducto.rdlc";
                dataset = new ReportDataSource("dsReporteProducto", dt);
                rvwReporteAño.LocalReport.DataSources.Add(dataset);
                dataset.Value = dt;
                rvwReporteAño.LocalReport.Refresh();
            }
            if (Condicion == "ListaProducto")
            {
                dt = crud.cargarBDData("Select IdProducto as ID, NombreProducto as Nombre, PrecioUnitario as Precio, " +
                        "CantidadDisponible as Cantidad from Inventario");
                rvwReporteAño.LocalReport.ReportEmbeddedResource = "AniCLinic.rptListaProductos.rdlc";
                dataset = new ReportDataSource("dsListaProductos", dt);
                rvwReporteAño.LocalReport.DataSources.Add(dataset);
                dataset.Value = dt;
                rvwReporteAño.LocalReport.Refresh();
            }

            this.rvwReporteAño.RefreshReport();
        }
    }
}
