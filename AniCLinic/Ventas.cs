using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Ventas : Form
    {
        csCRUD crud = new csCRUD();
        csProducto producto;
        csPersona persona;
        csFactura factura;
        csVenta ventaAgg;
        SqlDataReader reader;
        int idEmpl;
        bool vieneCita;

        public Ventas()
        {
            InitializeComponent();
            prepararGrid();
            cargarProductos();
            cargarCmb();
            btnImprimir.Enabled = false;
        }
        public Ventas(int id)
        {
            idEmpl = id;
            InitializeComponent();
            prepararGrid();
            cargarProductos();
            cargarCmb();
            btnImprimir.Enabled = false;
        }
        public Ventas(int id, decimal valor, bool vieneCita)
        {
            idEmpl = id;
            InitializeComponent();
            prepararGrid();
            cargarProductos();
            cargarCmb();
            btnImprimir.Enabled = false;
            try
            {
                reader = crud.EjecutarQuery("Select * from Inventario Where IdProducto = 1");
                if (reader.Read() && reader != null)
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
                }
                decimal precioTotal = 1 * producto.PrecioUnitario * (1 + producto.Iva);
                precioTotal = Math.Round(precioTotal, 2);
                dgvVentas.Rows.Add(producto.IdProducto, producto.NombreProducto, producto.Descripcion,
                    producto.PrecioUnitario, producto.Iva, 1, precioTotal, "");
                actualizarPrecio();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.vieneCita = vieneCita;
        }
        public void cargarCmb()
        {
            cmbCategoria.Items.AddRange(new object[]
            {
            "Medicamentos",
            "Equipos Medicos",
            "Alimentos",
            "Accesorios",
            "Higiene"
            });
            cmbMetodoPago.Items.AddRange(new object[]
            {
            "Efectivo",
            "Transferencia",
            "Tarjeta"
            });
        }
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
            dgvVentas.Columns["ID"].Width = 60;
            dgvVentas.Columns["Nombre"].Width = 120;
            dgvVentas.Columns["Descripcion"].Width = 200;
            dgvVentas.Columns["Precio"].Width = 80;
            dgvVentas.Columns["Cantidad"].Width = 80;
            dgvVentas.Columns["Total"].Width = 80;

            if (!dgvVentas.Columns.Contains("Promocion"))
            {
                DataGridViewTextBoxColumn colPromo = new DataGridViewTextBoxColumn();
                colPromo.Name = "Promocion";
                colPromo.HeaderText = "Promoción";
                colPromo.Width = 120;
                dgvVentas.Columns.Add(colPromo);
            }
        }
        private void cargarProductos(string categ = "")
        {
            reader = crud.EjecutarQuery("Select IdProducto, NombreProducto from Inventario Where Categoria like '" + categ + "%'");

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
            if (cmbProducto.SelectedItem is ProductoItem productoSeleccionado)
            {
                int idP = productoSeleccionado.idProducto;

                reader = crud.EjecutarQuery("Select * from Inventario Where IdProducto = " + idP);
                if (reader.Read() && reader != null)
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
                producto = null;
            return producto;
        }

        private decimal aplicarPromocion(csProducto prod, decimal cantidad, out int idPromocion, out string promoDesc)
        {
            idPromocion = 0;
            promoDesc = "";
            decimal precioUnitario = prod.PrecioUnitario;
            decimal subtotal = precioUnitario * cantidad;
            decimal descuento = 0;

            try
            {
                SqlDataReader promoReader = crud.EjecutarQuery($@"
            SELECT TOP 1 IdPromocion, Nombre, Tipo, Valor, Categoria, IdProducto, CondicionExtra 
            FROM Promocion
            WHERE Activa = 1 
              AND GETDATE() BETWEEN FechaInicio AND FechaFin
              AND (Categoria = '{prod.Categoria}' OR IdProducto = {prod.IdProducto} OR Categoria IS NULL)
            ORDER BY IdPromocion DESC");

                if (promoReader != null && promoReader.Read())
                {
                    int promoId = Convert.ToInt32(promoReader["IdPromocion"]);
                    string tipo = promoReader["Tipo"].ToString();
                    decimal valor = promoReader["Valor"] != DBNull.Value ? Convert.ToDecimal(promoReader["Valor"]) : 0;
                    string nombrePromo = promoReader["Nombre"].ToString();

                    if (promoId == 2)
                    {
                        if (vieneCita && DateTime.Now.Month == 10) 
                        {
                            idPromocion = promoId;
                            promoDesc = nombrePromo;

                            if (tipo == "Descuento")
                                descuento = subtotal * (valor / 100);
                            else if (tipo == "Gratis")
                                descuento = subtotal;
                        }
                    }
                    else 
                    {
                        idPromocion = promoId;
                        promoDesc = nombrePromo;

                        if (tipo == "Descuento")
                            descuento = subtotal * (valor / 100);
                        else if (tipo == "Gratis")
                            descuento = subtotal;
                    }
                }
                promoReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar promoción: " + ex.Message);
            }

            return Math.Round(subtotal - descuento, 2);
        }

        private void actualizarPrecio()
        {
            decimal subtotal = 0;
            decimal iva = 0;
            decimal descuento = 0;
            decimal totalVenta;

            foreach (DataGridViewRow fila in dgvVentas.Rows)
            {
                if (fila.Cells["Precio"].Value != null && fila.Cells["Cantidad"].Value != null && fila.Cells["IVA"].Value != null)
                {
                    decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
                    int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                    decimal ivaPorcentaje = Convert.ToDecimal(fila.Cells["IVA"].Value);
                    decimal totalFila = Convert.ToDecimal(fila.Cells["Total"].Value);

                    decimal subtotalFila = precio * cantidad;
                    decimal ivaFila = totalFila - (totalFila / (1 + ivaPorcentaje)); 
                    decimal descuentoFila = subtotalFila - (totalFila / (1 + ivaPorcentaje));

                    subtotal += subtotalFila;
                    iva += Math.Round(ivaFila, 2);
                    descuento += Math.Round(descuentoFila, 2);
                }
            }

            totalVenta = Math.Round((subtotal - descuento) + iva, 2);

            lblTtlVenta.Text = "$ " + subtotal.ToString("N2");
            lblIVA.Text = "$ " + iva.ToString("N2");
            lblDescuento.Text = "$ " + descuento.ToString("N2");
            lblTotal.Text = "$ " + totalVenta.ToString("N2");
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                int sumaRepetido;
                producto = cargarProducto();
                foreach (DataGridViewRow fila in dgvVentas.Rows)
                {
                    if (Convert.ToInt32(fila.Cells["ID"].Value) == producto.IdProducto)
                    {
                        sumaRepetido = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                        fila.Cells["Cantidad"].Value = sumaRepetido + Convert.ToInt32(txtCantidad.Text);
                        fila.Cells["Total"].Value = Math.Round(Convert.ToDecimal(fila.Cells["Cantidad"].Value) * producto.PrecioUnitario * (1 + producto.Iva), 2);
                        actualizarPrecio();
                        return;
                    }
                }

                decimal cantidad = Convert.ToDecimal(txtCantidad.Text);

                int idPromo;
                string promoDesc;
                decimal precioConPromo = aplicarPromocion(producto, cantidad, out idPromo, out promoDesc);

                decimal precioTotal = Math.Round(precioConPromo * (1 + producto.Iva), 2);

                dgvVentas.Rows.Add(producto.IdProducto, producto.NombreProducto, producto.Descripcion,
                    producto.PrecioUnitario, producto.Iva, cantidad, precioTotal, promoDesc);

                actualizarPrecio();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
                    MessageBox.Show("No tiene ningun producto añadido para la venta.");
                    return;
                }
                if (cmbMetodoPago.SelectedIndex == -1)
                {
                    MessageBox.Show("Seleccione un metodo de pago");
                    return;
                }
                var conf = MessageBox.Show("¿Desea finalizar la venta?", "Venta", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (conf == DialogResult.Yes)
                {
                    foreach (DataGridViewRow fila in dgvVentas.Rows)
                    {
                        SqlDataReader reader = crud.EjecutarQuery("Select CantidadDisponible, NombreProducto from Inventario where IdProducto = " +
                            Convert.ToInt32(fila.Cells["ID"].Value));
                        if (reader.Read())
                        {
                            if (reader.GetInt32(0) < Convert.ToInt32(fila.Cells["Cantidad"].Value))
                            {
                                if (Convert.ToInt32(fila.Cells["ID"].Value) == 1)
                                {
                                    break;
                                }
                                MessageBox.Show("Error no tiene suficientes " + reader.GetString(1) + " en el inventario. \n" +
                                    "Actualmente cuenta con " + reader.GetInt32(0) + " Unidades disponibles disponibles.");
                                reader.Close();
                                return;
                            }
                        }
                    }
                    

                    btnImprimir.Enabled = true;

                    decimal subtotal = 0;
                    decimal ivaTotal = 0;
                    decimal totalVenta;
                    decimal descuento = 0;
                    decimal descuentoTotal = 0;

                    foreach (DataGridViewRow fila in dgvVentas.Rows)
                    {
                        if (fila.Cells["Precio"].Value != null && fila.Cells["Cantidad"].Value != null && fila.Cells["IVA"].Value != null && fila.Cells["Total"].Value != null)
                        {
                            decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
                            int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                            decimal ivaPorcentaje = Convert.ToDecimal(fila.Cells["IVA"].Value);
                            decimal totalFila = Convert.ToDecimal(fila.Cells["Total"].Value);

                            decimal subtotalFila = precio * cantidad;
                            decimal ivaFila = totalFila - (totalFila / (1 + ivaPorcentaje));
                            decimal descuentoFila = subtotalFila - (totalFila / (1 + ivaPorcentaje));

                            subtotal += subtotalFila;
                            ivaTotal += Math.Round(ivaFila, 2);
                            descuentoTotal += Math.Round(descuentoFila, 2);
                        }
                    }

                    totalVenta = Math.Round((subtotal - descuentoTotal) + ivaTotal, 2);

                    lblTtlVenta.Text = "$ " + subtotal.ToString("N2");
                    lblIVA.Text = "$ " + ivaTotal.ToString("N2");
                    lblDescuento.Text = "$ " + descuentoTotal.ToString("N2");
                    lblTotal.Text = "$ " + totalVenta.ToString("N2");

                    string metodoPago = cmbMetodoPago.Text;

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

                    factura = new csFactura(persona.IdPersona,
                                        idEmpl,
                                        subtotal,   
                                        ivaTotal,     
                                        totalVenta,   
                                        metodoPago
                                        );
                    factura.obtenerNumFactura();
                    if (!factura.agregarFactura())
                    {
                        MessageBox.Show("Error al guardar la factura.");
                        return;
                    }

                    factura.obtenerId();

                    foreach (DataGridViewRow fila in dgvVentas.Rows)
                    {
                        ventaAgg = new csVenta(Convert.ToInt32(fila.Cells["ID"].Value),
                            persona.IdPersona, idEmpl, Convert.ToInt32(fila.Cells["Cantidad"].Value),
                            Convert.ToDecimal(fila.Cells["Precio"].Value));
                        if (!ventaAgg.agregarVenta())
                        {
                            MessageBox.Show("Error al guardar la venta");
                            return;
                        }
                        SqlDataReader reader = crud.EjecutarQuery("Select CantidadDisponible from Inventario Where IdProducto = " + ventaAgg.IdProducto);
                        if (reader.Read())
                        {
                            int cantidadRestada = reader.GetInt32(0) - ventaAgg.CantidadVendida;
                            crud.editarBD("Update Inventario set CantidadDisponible = @CantidadRestada Where IdProducto = @IdProducto",
                                new SqlParameter("@CantidadRestada", cantidadRestada),
                                new SqlParameter("@IdProducto", ventaAgg.IdProducto));
                            reader.Close();
                        }
                        ventaAgg.obtenerId();

                        crud.agregarBD("Insert into DetalleFactura (IdFactura, IdVenta) values (@IdFactura, @IdVenta)",
                            new SqlParameter("@IdFactura", factura.IdFactura),
                            new SqlParameter("@IdVenta", ventaAgg.IdVenta));

                        if (fila.Cells["Promocion"].Value != null && fila.Cells["Promocion"].Value.ToString() != "")
                        {
                            SqlDataReader promoReader = crud.EjecutarQuery("SELECT TOP 1 IdPromocion FROM Promocion WHERE Nombre = '" + fila.Cells["Promocion"].Value.ToString() + "'");
                            if (promoReader != null && promoReader.Read())
                            {
                                int idPromo = promoReader.GetInt32(0);
                                decimal precioSinPromo = Convert.ToDecimal(fila.Cells["Precio"].Value) * Convert.ToInt32(fila.Cells["Cantidad"].Value);
                                decimal precioConPromo = Convert.ToDecimal(fila.Cells["Total"].Value);
                                descuento = precioSinPromo - precioConPromo;

                                SqlDataReader promoExist = crud.EjecutarQuery("SELECT COUNT(*) FROM FacturaPromocion WHERE IdFactura = " + factura.IdFactura + " AND IdPromocion = " + idPromo);
                                if (promoExist.Read() && promoExist.GetInt32(0) == 0)
                                {
                                    crud.agregarBD("INSERT INTO FacturaPromocion (IdFactura, IdPromocion, Descuento) VALUES (@IdFactura, @IdPromocion, @Descuento)",
                                        new SqlParameter("@IdFactura", factura.IdFactura),
                                        new SqlParameter("@IdPromocion", idPromo),
                                        new SqlParameter("@Descuento", descuento));
                                }
                                promoExist.Close();
                            }
                            promoReader.Close();
                        }
                    }
                    MessageBox.Show("Factura generada correctamente");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCantidad.Text.Length >= 4) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((char.IsDigit(e.KeyChar) && txtPrecio.Text.Replace(",", "").Replace(".", "").Length >= 8)
                || (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != ',')
                || (e.KeyChar == ',' && txtPrecio.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }

        private void txtCedula_TextChanged(object sender, EventArgs e)
        {
            if (txtCedula.Text.Length == 10)
            {
                persona = new csPersona();
                int id = persona.obtenerIdPorCedula(txtCedula.Text);
                reader = crud.EjecutarQuery("Select * from Persona where IdPersona = " + id);
                if (reader != null)
                {
                    if (reader.Read())
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
                    }
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
            {
                e.Handled = true;
            }
        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategoria.SelectedIndex == -1)
                return;
            cmbProducto.Items.Clear();
            cargarProductos(cmbCategoria.Text);
        }

        private void cmbProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedIndex == -1)
                return;
            producto = cargarProducto();
            txtPrecio.Text = producto.PrecioUnitario.ToString();
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
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            frFactura frFac = new frFactura(factura.IdFactura);
            frFac.ShowDialog();
        }

        private void Ventas_Load(object sender, EventArgs e)
        {

        }

        private void btnAbrirListaPropietario_Click(object sender, EventArgs e)
        {
            using (var frm = new FMRListaPropietario())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtCedula.Text = frm.CedulaSel;
                }
            }
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
        public override string ToString()
        {
            return $"{idProducto} - {nombreProducto}";
        }
    }
}
