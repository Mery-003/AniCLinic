using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Ventas : Form
    {
        // ======== Servicios / estado ========
        private readonly csCRUD crud = new csCRUD();
        private csProducto producto;
        private csPersona persona;
        private csVenta ventaAgg;
        private SqlDataReader reader;

        private int idEmpl;
        private bool vieneCita;

        // FacturaEC generada (válida)
        private int idFacturaEC = 0;

        // ======== CTORs ========
        public Ventas()
        {
            InitializeComponent();
            prepararGrid();
            cargarProductos();
            cargarCmb();
            btnImprimir.Enabled = false;
        }

        public Ventas(int id) : this()
        {
            idEmpl = id;
        }

        public Ventas(int id, bool vieneCita) : this(id)
        {
            try
            {
                // Agrega automáticamente la "cita" como producto Id=1 (si existe)
                reader = crud.EjecutarQuery("SELECT * FROM Inventario WHERE IdProducto = 1");
                if (reader != null && reader.Read())
                {
                    producto = new csProducto(
                        Convert.ToInt32(reader["IdProducto"]),
                        Convert.ToInt32(reader["IdProveedor"]),
                        reader["NombreProducto"].ToString(),
                        reader["Descripcion"].ToString(),
                        reader["Categoria"].ToString(),
                        Convert.ToDecimal(reader["PrecioUnitario"]),
                        Convert.ToDecimal(reader["Iva"]),
                        Convert.ToInt32(reader["CantidadDisponible"])
                    );
                    decimal precioTotal = Math.Round(1 * producto.PrecioUnitario * (1 + producto.Iva), 2);

                    // Promoción y columnas ya se crean en prepararGrid()
                    dgvVentas.Rows.Add(
                        producto.IdProducto,
                        producto.NombreProducto,
                        producto.Descripcion,
                        producto.PrecioUnitario,
                        producto.Iva,
                        1,                      // Cantidad
                        precioTotal,            // Total (con IVA)
                        "",                     // Promoción
                        0                       // IdPromocion
                    );
                    actualizarPrecio();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.vieneCita = vieneCita;
        }

        // ======== UI helpers ========
        private void prepararGrid()
        {
            dgvVentas.ReadOnly = true;
            dgvVentas.MultiSelect = false;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.RowHeadersVisible = false;
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.AutoGenerateColumns = true;

            configurarColumnas();
        }

        private void configurarColumnas()
        {
            // Ajusta a tus nombres reales de columnas (si difieren).
            if (dgvVentas.Columns.Contains("ID"))
                dgvVentas.Columns["ID"].Width = 60;
            if (dgvVentas.Columns.Contains("Nombre"))
                dgvVentas.Columns["Nombre"].Width = 120;
            if (dgvVentas.Columns.Contains("Descripcion"))
                dgvVentas.Columns["Descripcion"].Width = 200;
            if (dgvVentas.Columns.Contains("Precio"))
                dgvVentas.Columns["Precio"].Width = 80;
            if (dgvVentas.Columns.Contains("IVA"))
                dgvVentas.Columns["IVA"].Width = 60;
            if (dgvVentas.Columns.Contains("Cantidad"))
                dgvVentas.Columns["Cantidad"].Width = 80;
            if (dgvVentas.Columns.Contains("Total"))
                dgvVentas.Columns["Total"].Width = 100;

            // Columna visible para nombre de promoción
            if (!dgvVentas.Columns.Contains("Promocion"))
            {
                var colPromo = new DataGridViewTextBoxColumn
                {
                    Name = "Promocion",
                    HeaderText = "Promoción",
                    Width = 140
                };
                dgvVentas.Columns.Add(colPromo);
            }

            // Columna oculta para IdPromocion
            if (!dgvVentas.Columns.Contains("IdPromocion"))
            {
                var colIdPromo = new DataGridViewTextBoxColumn
                {
                    Name = "IdPromocion",
                    HeaderText = "IdPromocion",
                    Visible = false
                };
                dgvVentas.Columns.Add(colIdPromo);
            }
        }

        private void cargarCmb()
        {
            cmbCategoria.Items.Clear();
            cmbCategoria.Items.AddRange(new object[]
            {
                "Medicamentos",
                "Equipos Medicos",
                "Alimentos",
                "Accesorios",
                "Higiene"
            });

            cmbMetodoPago.Items.Clear();
            cmbMetodoPago.Items.AddRange(new object[]
            {
                "Efectivo",
                "Transferencia",
                "Tarjeta"
            });
        }

        private void cargarProductos(string categ = "")
        {
            cmbProducto.Items.Clear();

            // EjecutarQuery SIN parámetros
            string sql = "SELECT IdProducto, NombreProducto FROM Inventario WHERE Categoria LIKE '"
                         + (categ ?? "").Replace("'", "''") + "%'";
            reader = crud.EjecutarQuery(sql);

            if (reader != null)
            {
                while (reader.Read())
                {
                    int idProducto = reader.GetInt32(0);
                    string nombreProducto = reader.GetString(1);
                    cmbProducto.Items.Add(new ProductoItem(idProducto, nombreProducto));
                }
            }
        }

        private csProducto cargarProducto()
        {
            if (cmbProducto.SelectedItem is ProductoItem prodSel)
            {
                int idP = prodSel.idProducto;

                // EjecutarQuery SIN parámetros
                reader = crud.EjecutarQuery("SELECT * FROM Inventario WHERE IdProducto = " + idP);

                if (reader != null && reader.Read())
                {
                    producto = new csProducto(
                        idP,
                        Convert.ToInt32(reader["IdProveedor"]),
                        reader["NombreProducto"].ToString(),
                        reader["Descripcion"].ToString(),
                        reader["Categoria"].ToString(),
                        Convert.ToDecimal(reader["PrecioUnitario"]),
                        Convert.ToDecimal(reader["Iva"]),
                        Convert.ToInt32(reader["CantidadDisponible"])
                    );
                }
            }
            else
            {
                producto = null;
            }
            return producto;
        }

        // Aplica la mejor promoción disponible al subtotal de "cantidad" unidades del producto.
        // Devuelve el precio con promoción (SIN IVA). Retorna datos de la promo por out.
        private decimal aplicarPromocion(csProducto prod, decimal cantidad,
                                         out int idPromocion, out string promoDesc, out string condicion)
        {
            idPromocion = 0;
            promoDesc = "";
            condicion = "";

            decimal subtotal = prod.PrecioUnitario * cantidad;
            decimal bestDiscount = 0m;

            try
            {
                // EjecutarQuery SIN parámetros
                string sql = @"
SELECT p.IdPromocion, p.Nombre, p.Tipo, p.Descuento, p.Condicion
FROM Promocion p
INNER JOIN PromocionProducto pp ON p.IdPromocion = pp.IdPromocion
WHERE p.Activa = 1
  AND GETDATE() BETWEEN p.FechaInicio AND p.FechaFin
  AND pp.IdProducto = " + prod.IdProducto;

                SqlDataReader promoReader = crud.EjecutarQuery(sql);

                if (promoReader != null)
                {
                    while (promoReader.Read())
                    {
                        int promoId = Convert.ToInt32(promoReader["IdPromocion"]);
                        string tipo = promoReader["Tipo"]?.ToString() ?? "";
                        decimal valor = promoReader["Descuento"] != DBNull.Value ? Convert.ToDecimal(promoReader["Descuento"]) : 0m;
                        string nombrePromo = promoReader["Nombre"]?.ToString() ?? "";
                        string cond = promoReader["Condicion"]?.ToString() ?? "";

                        decimal candidateDiscount = 0m;
                        if (tipo.Equals("Gratis", StringComparison.OrdinalIgnoreCase))
                            candidateDiscount = subtotal;
                        else if (tipo.Equals("Descuento", StringComparison.OrdinalIgnoreCase))
                            candidateDiscount = subtotal * (valor / 100m);

                        if (candidateDiscount > bestDiscount)
                        {
                            bestDiscount = candidateDiscount;
                            idPromocion = promoId;
                            promoDesc = nombrePromo;
                            condicion = cond;
                        }
                    }
                    if (!promoReader.IsClosed) promoReader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar promoción: " + ex.Message);
            }

            decimal precioConProm = Math.Round(subtotal - bestDiscount, 2);
            if (precioConProm < 0) precioConProm = 0;
            return precioConProm; // sin IVA
        }

        // Helper para evitar error si lblDescuento no existe en el diseñador
        private void SetTextIfExists(string controlName, string text)
        {
            var ctrl = this.Controls[controlName];
            if (ctrl != null) ctrl.Text = text;
        }

        // Actualiza labels: Subtotal, IVA, Descuento, Total
        private void actualizarPrecio()
        {
            decimal subtotal = 0m;
            decimal iva = 0m;
            decimal descuento = 0m;

            if (dgvVentas.Rows.Count <= 0)
            {
                lblTtlVenta.Text = "$ 0.00";
                lblIVA.Text = "$ 0.00";
                SetTextIfExists("lblDescuento", "$ 0.00");
                lblTotal.Text = "$ 0.00";
                return;
            }

            foreach (DataGridViewRow fila in dgvVentas.Rows)
            {
                if (fila.Cells["Precio"].Value == null ||
                    fila.Cells["Cantidad"].Value == null ||
                    fila.Cells["IVA"].Value == null ||
                    fila.Cells["Total"].Value == null) continue;

                decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
                int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                decimal ivaPorcentaje = Convert.ToDecimal(fila.Cells["IVA"].Value);
                decimal totalFilaConIVA = Convert.ToDecimal(fila.Cells["Total"].Value);

                decimal subtotalFila = precio * cantidad;                          // sin IVA ni descuento
                decimal baseConDescuento = totalFilaConIVA / (1 + ivaPorcentaje);  // sin IVA, ya con descuento aplicado
                decimal ivaFila = totalFilaConIVA - baseConDescuento;
                decimal descuentoFila = subtotalFila - baseConDescuento;

                subtotal += subtotalFila;
                iva += Math.Round(ivaFila, 2);
                descuento += Math.Round(descuentoFila, 2);
            }

            decimal totalVenta = Math.Round((subtotal - descuento) + iva, 2);

            lblTtlVenta.Text = "$ " + subtotal.ToString("N2");
            lblIVA.Text = "$ " + iva.ToString("N2");
            SetTextIfExists("lblDescuento", "$ " + descuento.ToString("N2"));
            lblTotal.Text = "$ " + totalVenta.ToString("N2");
        }

        // ======== EVENTOS ========
        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCantidad.Text.Length >= 4) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((char.IsDigit(e.KeyChar) && txtPrecio.Text.Replace(",", "").Replace(".", "").Length >= 8)
                || (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != ',')
                || (e.KeyChar == ',' && txtPrecio.Text.Contains(",")))
                e.Handled = true;
        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategoria.SelectedIndex == -1) return;
            cargarProductos(cmbCategoria.Text);
        }

        private void cmbProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedIndex == -1) return;
            producto = cargarProducto();
            if (producto != null)
                txtPrecio.Text = producto.PrecioUnitario.ToString("N2");
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbProducto.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtCantidad.Text))
                {
                    MessageBox.Show("Seleccione un producto y cantidad.");
                    return;
                }

                producto = cargarProducto();
                if (producto == null) return;

                int cantidadNueva = Convert.ToInt32(txtCantidad.Text);

                // Buscar si ya está en el grid
                DataGridViewRow filaExistente = null;
                foreach (DataGridViewRow fila in dgvVentas.Rows)
                {
                    if (Convert.ToInt32(fila.Cells["ID"].Value) == producto.IdProducto)
                    {
                        filaExistente = fila;
                        break;
                    }
                }

                int cantidadTotal = cantidadNueva;
                if (filaExistente != null)
                    cantidadTotal += Convert.ToInt32(filaExistente.Cells["Cantidad"].Value);

                // Política de promoción: aplicar a la primera unidad (como venías haciendo)
                int idPromo = 0;
                string promoDesc = "";
                string condicionPromo = "";

                decimal precioConPromoSinIVA = 0m;
                if (cantidadTotal > 0)
                {
                    // 1ra unidad con promoción (si existe)
                    decimal precioPrimeraSinIVA = aplicarPromocion(producto, 1, out int idPromoFila, out string promoDescFila, out string condFila);
                    idPromo = idPromoFila;
                    promoDesc = promoDescFila;
                    condicionPromo = condFila;

                    // Unidades restantes sin promoción
                    int cantidadRestante = cantidadTotal - 1;
                    decimal restoSinIVA = cantidadRestante * producto.PrecioUnitario;

                    precioConPromoSinIVA = precioPrimeraSinIVA + restoSinIVA;
                }

                // Convertir a CON IVA
                decimal totalConIVA = Math.Round(precioConPromoSinIVA * (1 + producto.Iva), 2);

                if (filaExistente != null)
                {
                    filaExistente.Cells["Cantidad"].Value = cantidadTotal;
                    filaExistente.Cells["Total"].Value = totalConIVA;
                    filaExistente.Cells["Promocion"].Value = promoDesc;
                    filaExistente.Cells["IdPromocion"].Value = idPromo;
                }
                else
                {
                    dgvVentas.Rows.Add(
                        producto.IdProducto,
                        producto.NombreProducto,
                        producto.Descripcion,
                        producto.PrecioUnitario,
                        producto.Iva,
                        cantidadTotal,
                        totalConIVA,
                        promoDesc,
                        idPromo
                    );
                }

                actualizarPrecio();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (vieneCita && dgvVentas.SelectedRows.Count > 0)
            {
                var fila = dgvVentas.CurrentRow;
                if (Convert.ToInt32(fila.Cells["ID"].Value) == 1)
                {
                    MessageBox.Show("No se puede eliminar la cita", "Advertencia", MessageBoxButtons.OK);
                    return;
                }
            }
            if (dgvVentas.SelectedRows.Count > 0)
            {
                var fila = dgvVentas.CurrentRow;
                dgvVentas.Rows.Remove(fila);
                actualizarPrecio();
            }
        }

        private void txtCedula_TextChanged(object sender, EventArgs e)
        {
            if (txtCedula.Text.Length == 10)
            {
                persona = new csPersona();
                int id = persona.obtenerIdPorCedula(txtCedula.Text);

                reader = crud.EjecutarQuery("SELECT * FROM Persona WHERE IdPersona = " + id);

                if (reader != null && reader.Read())
                {
                    byte[] foto = null;
                    if (reader["Imagen"] != DBNull.Value)
                        foto = (byte[])reader["Imagen"];

                    persona = new csPersona(
                        Convert.ToInt32(reader["IdPersona"]),
                        reader["Nombre"].ToString(),
                        reader["Apellido"].ToString(),
                        reader["Celular"].ToString(),
                        reader["Cedula"].ToString(),
                        reader["Correo"].ToString(),
                        reader["DireccionDomiciliaria"].ToString(),
                        foto
                    );

                    txtNombre.Text = persona.Nombre + " " + persona.Apellido;
                    txtCelular.Text = persona.Celular;
                    txtCorreo.Text = persona.Correo;
                }
            }
            else
            {
                txtNombre.Text = "";
                txtCelular.Text = "";
                txtCorreo.Text = "";
            }
        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCedula.Text.Length >= 10) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void btnAbrirListaPropietario_Click(object sender, EventArgs e)
        {
            using (var frm = new FMRListaPropietario())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    txtCedula.Text = frm.CedulaSel;
            }
        }

        // ======== FINALIZAR: FacturaEC ÚNICA ========
        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCedula.Text.Length != 10 || string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("Por favor ingrese un cliente para continuar");
                    return;
                }
                if (dgvVentas.Rows.Count == 0)
                {
                    MessageBox.Show("No tiene ningún producto añadido para la venta.");
                    return;
                }
                if (cmbMetodoPago.SelectedIndex == -1)
                {
                    MessageBox.Show("Seleccione un método de pago");
                    return;
                }

                var conf = MessageBox.Show("¿Desea finalizar la venta?", "Venta", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (conf != DialogResult.Yes) return;

                // Validar stock
                foreach (DataGridViewRow fila in dgvVentas.Rows)
                {
                    int idP = Convert.ToInt32(fila.Cells["ID"].Value);
                    SqlDataReader rInv = crud.EjecutarQuery(
                        "SELECT CantidadDisponible, NombreProducto FROM Inventario WHERE IdProducto = " + idP);

                    if (rInv != null && rInv.Read())
                    {
                        int cantDisp = rInv.GetInt32(0);
                        int cantReq = Convert.ToInt32(fila.Cells["Cantidad"].Value);

                        if (cantDisp < cantReq)
                        {
                            if (idP == 1) { rInv.Close(); break; } // cita
                            MessageBox.Show($"No tiene suficientes {rInv.GetString(1)}. Disponible: {cantDisp}");
                            rInv.Close();
                            return;
                        }
                    }
                    if (rInv != null && !rInv.IsClosed) rInv.Close();
                }

                // Recalcular totales para labels (UI)
                // (Usa el total ya calculado fila a fila)
                decimal subtotal = 0, ivaTotal = 0, descuentoTotal = 0;
                foreach (DataGridViewRow fila in dgvVentas.Rows)
                {
                    if (fila.Cells["Precio"].Value == null ||
                        fila.Cells["Cantidad"].Value == null ||
                        fila.Cells["IVA"].Value == null ||
                        fila.Cells["Total"].Value == null) continue;

                    decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
                    int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                    decimal ivaPorcentaje = Convert.ToDecimal(fila.Cells["IVA"].Value);
                    decimal totalFila = Convert.ToDecimal(fila.Cells["Total"].Value);

                    decimal subtotalFila = precio * cantidad;
                    decimal baseConDesc = totalFila / (1 + ivaPorcentaje);
                    decimal ivaFila = totalFila - baseConDesc;
                    decimal descFila = subtotalFila - baseConDesc;

                    subtotal += subtotalFila;
                    ivaTotal += Math.Round(ivaFila, 2);
                    descuentoTotal += Math.Round(descFila, 2);
                }
                decimal totalVenta = Math.Round((subtotal - descuentoTotal) + ivaTotal, 2);

                lblTtlVenta.Text = "$ " + subtotal.ToString("N2");
                lblIVA.Text = "$ " + ivaTotal.ToString("N2");
                SetTextIfExists("lblDescuento", "$ " + descuentoTotal.ToString("N2"));
                lblTotal.Text = "$ " + totalVenta.ToString("N2");
                string metodoPago = cmbMetodoPago.Text;

                // Bloquear controles
                txtCedula.ReadOnly = true;
                txtCantidad.Text = "";
                txtCantidad.ReadOnly = true;
                cmbCategoria.SelectedIndex = -1; cmbCategoria.Enabled = false;
                cmbProducto.SelectedIndex = -1; cmbProducto.Enabled = false;
                cmbMetodoPago.SelectedIndex = -1; cmbMetodoPago.Enabled = false;
                btnAgregar.Enabled = false;
                btnEliminar.Enabled = false;
                btnFinalizar.Enabled = false;

                // 1) Crear cabecera FacturaEC (usa cargarBDData con parámetros)
                DataTable dtId = crud.cargarBDData(
                    "DECLARE @Id INT; " +
                    "EXEC dbo.sp_FacturaEC_Crear @IdPersona,@IdEmpleado,@FormaPago,'001','001',@Id OUTPUT; " +
                    "SELECT @Id AS Id;",
                    new SqlParameter("@IdPersona", persona.IdPersona),
                    new SqlParameter("@IdEmpleado", idEmpl),
                    new SqlParameter("@FormaPago", metodoPago)
                );
                if (dtId.Rows.Count == 0)
                    throw new Exception("No se pudo crear la cabecera de la Factura EC.");
                idFacturaEC = Convert.ToInt32(dtId.Rows[0]["Id"]);

                // 2) Insertar ventas, descontar stock y vincular a FacturaEC
                foreach (DataGridViewRow fila in dgvVentas.Rows)
                {
                    int idP = Convert.ToInt32(fila.Cells["ID"].Value);
                    int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                    decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);

                    ventaAgg = new csVenta(idP, persona.IdPersona, idEmpl, cantidad, precio);
                    if (!ventaAgg.agregarVenta())
                    {
                        MessageBox.Show("Error al guardar la venta");
                        return;
                    }

                    // Descontar inventario
                    SqlDataReader r2 = crud.EjecutarQuery("SELECT CantidadDisponible FROM Inventario WHERE IdProducto = " + idP);
                    if (r2 != null && r2.Read())
                    {
                        int cantidadRestada = r2.GetInt32(0) - cantidad;
                        crud.editarBD("UPDATE Inventario SET CantidadDisponible = @cant WHERE IdProducto = @idp",
                            new SqlParameter("@cant", cantidadRestada),
                            new SqlParameter("@idp", idP));
                        r2.Close();
                    }

                    // Obtener IdVenta insertada y vincular
                    ventaAgg.obtenerId();
                    crud.editarBD("EXEC dbo.sp_FacturaEC_AgregarVenta @IdF,@IdV",
                        new SqlParameter("@IdF", idFacturaEC),
                        new SqlParameter("@IdV", ventaAgg.IdVenta));

                    // (Opcional) aquí podrías acumular promos por factura, si manejas tabla extra
                }

                // 3) Recalcular totales de cabecera EC
                crud.editarBD("EXEC dbo.sp_FacturaEC_Recalcular @Id",
                    new SqlParameter("@Id", idFacturaEC));

                btnImprimir.Enabled = true;
                MessageBox.Show("Factura generada correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            // ÚNICO flujo de impresión válido
            var frm = new frFacturaEC(idFacturaEC);
            frm.ShowDialog();
        }

        // ======== Util ========
        public class ProductoItem
        {
            public int idProducto { get; set; }
            public string nombreProducto { get; set; }
            public ProductoItem(int id, string nombre)
            {
                idProducto = id;
                nombreProducto = nombre;
            }
            public override string ToString() => $"{idProducto} - {nombreProducto}";
        }

        private void lblVerPromo_Click(object sender, EventArgs e)
        {
            Promodesc promo = new Promodesc();
            promo.ShowDialog();
            dgvVentas.Rows.Clear();
            actualizarPrecio();
        }
    }
}
