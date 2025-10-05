using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Ventas : Form
    {
        csCRUD crud = new csCRUD();
        csProducto producto;
        csPersona persona;
        csFactura factura;   // ya no se usa para EC, lo dejamos para no romper otras partes
        csVenta ventaAgg;
        SqlDataReader reader;
        int idEmpl;
        bool vieneCita;

        // Nueva factura EC
        int idFacturaEC;

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
                reader = crud.EjecutarQuery("Select * from Inventario Where IdProducto = 1");
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
                    dgvVentas.Rows.Add(producto.IdProducto, producto.NombreProducto, producto.Descripcion,
                                       producto.PrecioUnitario, producto.Iva, 1, precioTotal);
                    actualizarPrecio();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.vieneCita = vieneCita;
        }

        // ==================== utilitarios UI ====================

        public void prepararGrid()
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

        public void configurarColumnas()
        {
            if (dgvVentas.Columns.Contains("ID"))
                dgvVentas.Columns["ID"].Width = 60;
            if (dgvVentas.Columns.Contains("Nombre"))
                dgvVentas.Columns["Nombre"].Width = 120;
            if (dgvVentas.Columns.Contains("Descripcion"))
                dgvVentas.Columns["Descripcion"].Width = 200;
            if (dgvVentas.Columns.Contains("Precio"))
                dgvVentas.Columns["Precio"].Width = 80;
            if (dgvVentas.Columns.Contains("Cantidad"))
                dgvVentas.Columns["Cantidad"].Width = 80;
            if (dgvVentas.Columns.Contains("Total"))
                dgvVentas.Columns["Total"].Width = 80;
        }

        public void cargarCmb()
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

            reader = crud.EjecutarQuery(
                "Select IdProducto, NombreProducto from Inventario Where Categoria like '" + categ + "%'");

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

                reader = crud.EjecutarQuery("Select * from Inventario Where IdProducto = " + idP);
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

        private void actualizarPrecio()
        {
            decimal venta = 0, iva = 0;
            foreach (DataGridViewRow fila in dgvVentas.Rows)
            {
                if (fila.Cells["Precio"].Value != null &&
                    fila.Cells["Cantidad"].Value != null &&
                    fila.Cells["IVA"].Value != null)
                {
                    decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
                    int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                    decimal ivaPorcentaje = Convert.ToDecimal(fila.Cells["IVA"].Value);

                    decimal subtotal = precio * cantidad;
                    decimal ivaFila = subtotal * ivaPorcentaje;

                    venta += Math.Round(subtotal, 2);
                    iva += Math.Round(ivaFila, 2);
                }
            }
            decimal totalVenta = Math.Round(venta + iva, 2);
            lblTtlVenta.Text = "$ " + venta.ToString("N2");
            lblIVA.Text = "$ " + iva.ToString("N2");
            lblTotal.Text = "$ " + totalVenta.ToString("N2");
        }

        // ==================== EVENTOS DEL DESIGNER ====================

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

                // Si el producto ya está en la grilla, sumar cantidad
                foreach (DataGridViewRow fila in dgvVentas.Rows)
                {
                    if (Convert.ToInt32(fila.Cells["ID"].Value) == producto.IdProducto)
                    {
                        int sumaRepetido = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                        fila.Cells["Cantidad"].Value = sumaRepetido + Convert.ToInt32(txtCantidad.Text);
                        fila.Cells["Total"].Value = Math.Round(
                            Convert.ToDecimal(fila.Cells["Cantidad"].Value) * producto.PrecioUnitario * (1 + producto.Iva), 2);
                        actualizarPrecio();
                        return;
                    }
                }

                decimal cantidad = Convert.ToDecimal(txtCantidad.Text);
                decimal precioTotal = Math.Round(cantidad * producto.PrecioUnitario * (1 + producto.Iva), 2);
                dgvVentas.Rows.Add(producto.IdProducto, producto.NombreProducto, producto.Descripcion,
                                   producto.PrecioUnitario, producto.Iva, cantidad, precioTotal);
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
                DataGridViewRow fila = dgvVentas.CurrentRow;
                if (Convert.ToInt32(fila.Cells["ID"].Value) == 1)
                {
                    MessageBox.Show("No se puede eliminar la cita", "Advertencia", MessageBoxButtons.OK);
                    return;
                }
            }
            if (dgvVentas.SelectedRows.Count > 0)
            {
                DataGridViewRow fila = dgvVentas.CurrentRow;
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
                reader = crud.EjecutarQuery("Select * from Persona where IdPersona = " + id);
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

        // ==================== FINALIZAR / FACTURA EC ====================

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
                    SqlDataReader rInv = crud.EjecutarQuery(
                        "Select CantidadDisponible, NombreProducto from Inventario where IdProducto = " +
                        Convert.ToInt32(fila.Cells["ID"].Value));
                    if (rInv != null && rInv.Read())
                    {
                        if (rInv.GetInt32(0) < Convert.ToInt32(fila.Cells["Cantidad"].Value))
                        {
                            if (Convert.ToInt32(fila.Cells["ID"].Value) == 1) break;

                            MessageBox.Show("No tiene suficientes " + rInv.GetString(1) +
                                            ". Disponible: " + rInv.GetInt32(0));
                            rInv.Close();
                            return;
                        }
                    }
                }

                // Calcular totales (UI)
                decimal venta = 0, iva = 0;
                foreach (DataGridViewRow fila in dgvVentas.Rows)
                {
                    if (fila.Cells["Precio"].Value != null &&
                        fila.Cells["Cantidad"].Value != null &&
                        fila.Cells["IVA"].Value != null)
                    {
                        decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
                        int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                        decimal ivaPorcentaje = Convert.ToDecimal(fila.Cells["IVA"].Value);

                        decimal subtotal = precio * cantidad;
                        decimal ivaFila = subtotal * ivaPorcentaje;

                        venta += subtotal;
                        iva += ivaFila;
                    }
                }
                decimal totalVenta = Math.Round(venta + iva, 2);
                string metodoPago = cmbMetodoPago.Text;
                lblTtlVenta.Text = "$ " + venta.ToString("N2");
                lblIVA.Text = "$ " + iva.ToString("N2");
                lblTotal.Text = "$ " + totalVenta.ToString("N2");

                // Bloquear controles
                txtCedula.ReadOnly = true;
                txtCantidad.Text = "";
                txtCantidad.ReadOnly = true;
                cmbCategoria.SelectedIndex = -1;
                cmbCategoria.Enabled = false;
                cmbProducto.SelectedIndex = -1;
                cmbProducto.Enabled = false;
                cmbMetodoPago.SelectedIndex = -1;
                cmbMetodoPago.Enabled = false;
                btnAgregar.Enabled = false;
                btnEliminar.Enabled = false;
                btnFinalizar.Enabled = false;

                // 1) Cabecera EC (usar cargarBDData porque EjecutarQuery no admite parámetros)
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

                // 2) Insertar ventas y vincular a FacturaEC
                foreach (DataGridViewRow fila in dgvVentas.Rows)
                {
                    ventaAgg = new csVenta(
                        Convert.ToInt32(fila.Cells["ID"].Value),
                        persona.IdPersona, idEmpl,
                        Convert.ToInt32(fila.Cells["Cantidad"].Value),
                        Convert.ToDecimal(fila.Cells["Precio"].Value)
                    );
                    if (!ventaAgg.agregarVenta())
                    {
                        MessageBox.Show("Error al guardar la venta");
                        return;
                    }

                    // Descontar inventario
                    SqlDataReader r2 = crud.EjecutarQuery("Select CantidadDisponible from Inventario Where IdProducto = " + ventaAgg.IdProducto);
                    if (r2 != null && r2.Read())
                    {
                        int cantidadRestada = r2.GetInt32(0) - ventaAgg.CantidadVendida;
                        crud.editarBD("Update Inventario set CantidadDisponible = @Cant Where IdProducto = @IdP",
                            new SqlParameter("@Cant", cantidadRestada),
                            new SqlParameter("@IdP", ventaAgg.IdProducto));
                        r2.Close();
                    }

                    // Obtener IdVenta insertado
                    ventaAgg.obtenerId();

                    // Vincular venta a FacturaEC
                    crud.editarBD("EXEC dbo.sp_FacturaEC_AgregarVenta @IdF,@IdV",
                        new SqlParameter("@IdF", idFacturaEC),
                        new SqlParameter("@IdV", ventaAgg.IdVenta));
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
            var frm = new frFacturaEC(idFacturaEC);
            frm.ShowDialog();
        }
    }

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
}
