using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class frReportes : Form
    {
        private readonly csCRUD crud = new csCRUD();

        public frReportes()
        {
            InitializeComponent();
        }

        private void txtAño_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtAño.Text.Length >= 4) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void btnAño_Click(object sender, EventArgs e)
        {
            var reporte = new frReporteFinanciero(txtAño.Text, "Mes");
            reporte.ShowDialog();
        }

        private void btnProducto_Click(object sender, EventArgs e)
        {
            var reporte = new frReporteFinanciero(txtAño.Text, "Producto");
            reporte.ShowDialog();
        }

        private void btnListaProductos_Click(object sender, EventArgs e)
        {
            var reporte = new frReporteFinanciero(txtAño.Text, "ListaProducto");
            reporte.ShowDialog();
        }

        // KeyPress del cuadro donde escribes el ID de la factura
        private void guna2TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si quieres permitir más de 4 dígitos, quita la condición de Length.
            if ((!char.IsDigit(e.KeyChar) || txtFactura.Text.Length >= 9) && e.KeyChar != 8)
                e.Handled = true;
        }

        // Botón "Buscar/Imprimir factura"
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFactura.Text))
            {
                MessageBox.Show("Ingrese un Id o el NúmeroEC de la factura.");
                return;
            }

            string entrada = txtFactura.Text.Trim();

            try
            {
                // 1) Si escriben el NúmeroEC (tiene guiones: 001-001-000000123)
                if (entrada.Contains("-"))
                {
                    var dt = crud.cargarBDData(
                        "SELECT TOP 1 IdFacturaEC FROM dbo.FacturaEC WHERE NumeroEC = @Num",
                        new SqlParameter("@Num", entrada)
                    );

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No existe Factura EC con ese NúmeroEC.");
                        return;
                    }

                    int idEC = Convert.ToInt32(dt.Rows[0]["IdFacturaEC"]);
                    new frFacturaEC(idEC).ShowDialog();
                    return;
                }

                // 2) Si escriben un Id numérico → buscamos SOLO en FacturaEC
                if (!int.TryParse(entrada, out int id))
                {
                    MessageBox.Show("El Id debe ser numérico o ingrese un NúmeroEC con guiones.");
                    return;
                }

                var dt2 = crud.cargarBDData(
                    "SELECT 1 FROM dbo.FacturaEC WHERE IdFacturaEC = @Id",
                    new SqlParameter("@Id", id)
                );

                if (dt2.Rows.Count == 0)
                {
                    MessageBox.Show("No existe Factura con ese Id.");
                    return;
                }

                new frFacturaEC(id).ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar la factura: " + ex.Message);
            }
        }


        private void frReportes_Load(object sender, EventArgs e)
        {
            // opcional: estado inicial de controles
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {
            // opcional
        }
    }
}
