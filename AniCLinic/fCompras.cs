using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;   // <-- agrega esta línea

namespace AniCLinic
{
    public partial class fCompras : Form
    {
        private readonly csCRUD _crud = new csCRUD();
        private readonly DataTable _dtDetalle = new DataTable();

        private readonly int _idEmpleado;

        private int? _prodSelId = null;
        private string _prodSelNombre = null;
        private decimal _prodSelIva = 0m;

        public fCompras(int idEmpleado = 1)
        {
            InitializeComponent();
            _idEmpleado = idEmpleado;

            btnBuscarProveedor.Click += btnBuscarProveedor_Click;     
            btnElegirProducto.Click += btnElegirProducto_Click;      
            btnAgregarLineaCompra.Click += btnAgregarLineaCompra_Click;
            btnEliminarLineaCompra.Click += btnEliminarLineaCompra_Click;
            btnFinalizarCompra.Click += btnFinalizarCompra_Click;

            
            txtPrecioCompraUnit.KeyPress += txtPrecioCompraUnit_KeyPressSoloDecimal;
            txtPrecioCompraUnit.Leave += (s, e) =>
            {
                if (!decimal.TryParse((txtPrecioCompraUnit.Text ?? "0").Trim(), out var v)) v = 0m;
                txtPrecioCompraUnit.Text = v.ToString("N2");
            };

            // Cantidad
            nudCantidadCompra.Leave += (s, e) =>
            {
                if (!int.TryParse((nudCantidadCompra.Text ?? "0").Trim(), out var c)) c = 0;
                nudCantidadCompra.Text = c.ToString();
            };

            PrepUi();
            CargarMetodosPago();
            PrepDetalle();
        }

        private void PrepUi()
        {
            txtProvRazonSocial.ReadOnly = true;
            txtProvCorreo.ReadOnly = true;
            txtProvTelefono.ReadOnly = true;
            txtProvDireccion.ReadOnly = true;

            txtPrecioCompraUnit.ReadOnly = false; 
            txtPrecioCompraUnit.Text = "0,00";
        }

        private void CargarMetodosPago()
        {
            cboMetodoPagoCompra.Items.Clear();
            cboMetodoPagoCompra.Items.AddRange(new object[] { "Contado", "Transferencia", "Tarjeta", "Crédito" });
            cboMetodoPagoCompra.SelectedIndex = 0;
        }

        private void PrepDetalle()
        {
            _dtDetalle.Columns.Add("IdProducto", typeof(int));
            _dtDetalle.Columns.Add("Nombre", typeof(string));
            _dtDetalle.Columns.Add("Cantidad", typeof(int));
            _dtDetalle.Columns.Add("PrecioUnit", typeof(decimal));
            _dtDetalle.Columns.Add("IVA", typeof(decimal)); 
            _dtDetalle.Columns.Add("Base", typeof(decimal), "Cantidad * PrecioUnit");
            _dtDetalle.Columns.Add("Total", typeof(decimal), "Cantidad * PrecioUnit * (1 + IVA)");

            dgvDetalleCompra.AutoGenerateColumns = true;
            dgvDetalleCompra.DataSource = _dtDetalle;
            dgvDetalleCompra.ReadOnly = true;
            dgvDetalleCompra.AllowUserToAddRows = false;
            dgvDetalleCompra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (dgvDetalleCompra.Columns.Contains("PrecioUnit"))
                dgvDetalleCompra.Columns["PrecioUnit"].DefaultCellStyle.Format = "N2";
            if (dgvDetalleCompra.Columns.Contains("Base"))
                dgvDetalleCompra.Columns["Base"].DefaultCellStyle.Format = "N2";
            if (dgvDetalleCompra.Columns.Contains("Total"))
                dgvDetalleCompra.Columns["Total"].DefaultCellStyle.Format = "N2";
        }

        private void RecalcularTotales()
        {
            decimal sub = 0, iva = 0, tot = 0;
            foreach (DataRow r in _dtDetalle.Rows)
            {
                var c = Convert.ToInt32(r["Cantidad"]);
                var p = Convert.ToDecimal(r["PrecioUnit"]);
                var tv = Convert.ToDecimal(r["IVA"]);
                sub += c * p;
                iva += c * p * tv;
            }
            tot = sub + iva;
            lblSubtotalCompra.Text = sub.ToString("N2");
            lblIVACompra.Text = iva.ToString("N2");
            lblTotalCompra.Text = tot.ToString("N2");
        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            using (var dlg = new FRMListaProveedores())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    txtProvDocumento.Tag = dlg.IdProveedorSel;  // ID
                    txtProvDocumento.Text = dlg.RUCSel;
                    txtProvRazonSocial.Text = dlg.NombreSel;
                    txtProvTelefono.Text = dlg.TelefonoSel ?? "";
                    txtProvCorreo.Text = dlg.CorreoSel ?? "";
                    txtProvDireccion.Text = dlg.DireccionSel ?? "";
                }
            }
        }

        private void btnElegirProducto_Click(object sender, EventArgs e)
        {
            using (var dlg = new FRMListaProductos())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if ((dlg.NombreSel ?? "").IndexOf("cita", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        MessageBox.Show("Ese ítem no es un producto válido para compras.");
                        return;
                    }

                    _prodSelId = dlg.IdProductoSel;
                    _prodSelNombre = dlg.NombreSel;
                    _prodSelIva = dlg.IvaSel;

                    txtProductoSeleccionado.Text = _prodSelNombre;
                    txtPrecioCompraUnit.Text = dlg.PrecioSugeridoSel.ToString("N2");
                }
            }
        }

        private void btnAgregarLineaCompra_Click(object sender, EventArgs e)
        {
            if (txtProvDocumento.Tag == null) { MessageBox.Show("Seleccione un proveedor."); return; }
            if (_prodSelId == null) { MessageBox.Show("Seleccione un producto."); return; }

            if (!int.TryParse((nudCantidadCompra.Text ?? "0").Trim(), out int cant) || cant <= 0)
            { MessageBox.Show("Cantidad inválida."); return; }

            if (!decimal.TryParse((txtPrecioCompraUnit.Text ?? "0").Trim(), out decimal pu) || pu <= 0)
            { MessageBox.Show("Precio de compra inválido."); return; }

            int idProd = _prodSelId.Value;
            string nombre = _prodSelNombre ?? "";
            decimal iva = Math.Max(0, _prodSelIva);

            var rowExist = _dtDetalle.AsEnumerable().FirstOrDefault(r => r.Field<int>("IdProducto") == idProd);
            if (rowExist != null)
            {
                rowExist["Cantidad"] = rowExist.Field<int>("Cantidad") + cant;
                rowExist["PrecioUnit"] = pu;
                rowExist["IVA"] = iva;
            }
            else
            {
                var r = _dtDetalle.NewRow();
                r["IdProducto"] = idProd;
                r["Nombre"] = nombre;
                r["Cantidad"] = cant;
                r["PrecioUnit"] = pu;
                r["IVA"] = iva;
                _dtDetalle.Rows.Add(r);
            }
            RecalcularTotales();
        }

        private void btnEliminarLineaCompra_Click(object sender, EventArgs e)
        {
            if (dgvDetalleCompra.SelectedRows.Count == 0) return;
            foreach (DataGridViewRow gr in dgvDetalleCompra.SelectedRows)
            {
                var drv = gr.DataBoundItem as DataRowView;
                drv?.Row.Delete();
            }
            RecalcularTotales();
        }

        private void btnFinalizarCompra_Click(object sender, EventArgs e)
        {
            if (_dtDetalle.Rows.Count == 0) { MessageBox.Show("No hay ítems en la compra."); return; }
            if (txtProvDocumento.Tag == null) { MessageBox.Show("Seleccione un proveedor."); return; }
            if (cboMetodoPagoCompra.SelectedItem == null) { MessageBox.Show("Seleccione método de pago."); return; }

            DateTime fechaEmision;
            using (var dlg = new FechaEmision())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                fechaEmision = dlg.FechaSeleccionada;
            }

            int idProv = Convert.ToInt32(txtProvDocumento.Tag);
            int idEmp = _idEmpleado;
            string metodo = cboMetodoPagoCompra.SelectedItem.ToString();

            decimal sub0 = 0m, sub12 = 0m, iva12 = 0m, total = 0m;
            foreach (DataRow r in _dtDetalle.Rows)
            {
                int cant = r.Field<int>("Cantidad");
                decimal precio = r.Field<decimal>("PrecioUnit");
                decimal iva = r.Field<decimal>("IVA");

                decimal baseLinea = cant * precio;
                decimal ivaLinea = baseLinea * iva;
                decimal totLinea = baseLinea + ivaLinea;

                if (iva == 0m) sub0 += baseLinea; else sub12 += baseLinea;
                if (iva > 0m) iva12 += ivaLinea;
                total += totLinea;
            }

            try
            {
                var db = new csConexionBD();
                db.abrirConexion();
                var cn = db.obtenerConexion();

                int idCabGenerado = 0;

                using (var tx = cn.BeginTransaction())
                {
                    var (numero, numeroOC) = ObtenerSiguientesNumeros(cn, tx);

                    string sqlCab = $@"
INSERT INTO {TBL_CAB}
(
    {COL_NUM}, {COL_NUMOC}, IdProveedor, IdEmpleado, MetodoPago,
    FechaCompra, FechaEmision,
    Subtotal0, Subtotal12, IVA12, Total
)
VALUES
(
    @Numero, @NumeroOC, @IdProv, @IdEmp, @MetodoPago,
    CAST(GETDATE() AS datetime), @FechaEmision,
    @Sub0, @Sub12, @Iva12, @Total
);
SELECT CAST(SCOPE_IDENTITY() AS int);";

                    using (var cmd = new SqlCommand(sqlCab, cn, tx))
                    {
                        cmd.Parameters.Add("@Numero", SqlDbType.Int).Value = numero;
                        cmd.Parameters.Add("@NumeroOC", SqlDbType.Int).Value = numeroOC;
                        cmd.Parameters.Add("@IdProv", SqlDbType.Int).Value = idProv;
                        cmd.Parameters.Add("@IdEmp", SqlDbType.Int).Value = idEmp;
                        cmd.Parameters.Add("@MetodoPago", SqlDbType.NVarChar, 20).Value = metodo;
                        cmd.Parameters.Add("@FechaEmision", SqlDbType.Date).Value = fechaEmision;

                        var p0 = cmd.Parameters.Add("@Sub0", SqlDbType.Decimal); p0.Precision = 12; p0.Scale = 2; p0.Value = sub0;
                        var p12 = cmd.Parameters.Add("@Sub12", SqlDbType.Decimal); p12.Precision = 12; p12.Scale = 2; p12.Value = sub12;
                        var pI = cmd.Parameters.Add("@Iva12", SqlDbType.Decimal); pI.Precision = 12; pI.Scale = 2; pI.Value = iva12;
                        var pT = cmd.Parameters.Add("@Total", SqlDbType.Decimal); pT.Precision = 12; pT.Scale = 2; pT.Value = total;

                        idCabGenerado = Convert.ToInt32(cmd.ExecuteScalar());
                    }


                    string sqlDet = $@"
INSERT INTO {TBL_DET}
(
    IdCompra, IdProducto, Cantidad, PrecioUnit, IvaItem
)
VALUES
(
    @IdCompra, @IdProd, @Cant, @PU, @IvaItem
);";

                    using (var cmdD = new SqlCommand(sqlDet, cn, tx))
                    {
                        cmdD.Parameters.Add("@IdCompra", SqlDbType.Int);
                        cmdD.Parameters.Add("@IdProd", SqlDbType.Int);

                        var pCant = cmdD.Parameters.Add("@Cant", SqlDbType.Int);
                        var pPU = cmdD.Parameters.Add("@PU", SqlDbType.Decimal); pPU.Precision = 10; pPU.Scale = 2;
                        var pIvaItm = cmdD.Parameters.Add("@IvaItem", SqlDbType.Decimal); pIvaItm.Precision = 4; pIvaItm.Scale = 2;

                        foreach (DataRow r in _dtDetalle.Rows)
                        {
                            int cant = r.Field<int>("Cantidad");
                            decimal pu = r.Field<decimal>("PrecioUnit");
                            decimal iva = r.Field<decimal>("IVA"); // 0.00 / 0.12 / 0.15

                            cmdD.Parameters["@IdCompra"].Value = idCabGenerado;
                            cmdD.Parameters["@IdProd"].Value = r.Field<int>("IdProducto");
                            pCant.Value = cant;
                            pPU.Value = pu;
                            pIvaItm.Value = iva;

                            cmdD.ExecuteNonQuery();
                        }
                    }


                    tx.Commit();
                }

                _dtDetalle.Clear();
                RecalcularTotales();

                using (var frm = new frOrdenCompra(idCabGenerado))
                    frm.ShowDialog(this);

                db.cerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar la orden: " + ex.Message,
                                "Compras", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void txtPrecioCompraUnit_KeyPressSoloDecimal(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            char sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != sep && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && sep != e.KeyChar)
            {
                e.KeyChar = sep;
            }

            var tb = sender as TextBox;
            if ((e.KeyChar == sep) && tb != null && tb.Text.Contains(sep))
            {
                e.Handled = true;
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private const string TBL_CAB = "[dbo].[OC_Compra]";
        private const string TBL_DET = "[dbo].[OC_CompraDetalle]";
        private const string COL_NUM = "Numero";    
        private const string COL_NUMOC = "NumeroOC";  

        private int ObtenerSiguienteNumero(SqlConnection cn, SqlTransaction tx)
        {
            string sql = $@"SELECT ISNULL(MAX({COL_NUM}), 0) + 1 
                    FROM {TBL_CAB} WITH (UPDLOCK, HOLDLOCK)";
            using (var cmd = new SqlCommand(sql, cn, tx))
                return Convert.ToInt32(cmd.ExecuteScalar());
        }
        private (int numero, int numeroOC) ObtenerSiguientesNumeros(SqlConnection cn, SqlTransaction tx)
        {
            string sql = $@"
        SELECT 
            ISNULL(MAX([{COL_NUM}]),   0) + 1 AS NextNumero,
            ISNULL(MAX([{COL_NUMOC}]), 0) + 1 AS NextNumeroOC
        FROM {TBL_CAB} WITH (UPDLOCK, HOLDLOCK);";
            using (var cmd = new SqlCommand(sql, cn, tx))
            using (var rd = cmd.ExecuteReader())
            {
                rd.Read();
                return (rd.GetInt32(0), rd.GetInt32(1));
            }
        }

        private void fCompras_Load(object sender, EventArgs e)
        {

        }
    }
}
