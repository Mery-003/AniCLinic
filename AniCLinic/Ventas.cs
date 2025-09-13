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
        public Ventas()
        {
            InitializeComponent();
            visible(false);
            prepararGrid();
            cargarProductos();
            cargarCmb();
        }
        public Ventas(int id)
        {
            idEmpl = id;
            InitializeComponent();
            visible(false);
            prepararGrid();
            cargarProductos();
            cargarCmb();
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
        }
        private void visible(bool bvisible)
        {
            lblIVA.Visible = bvisible;
            lblIVAno.Visible = bvisible;
            lblTotalno.Visible = bvisible;
            lblTotal.Visible = bvisible;
            lblTtlVenta.Visible = bvisible;
            lblTtlVno.Visible = bvisible;
            btnImprimir.Visible = bvisible;
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
                        Convert.ToInt32(reader["CantidadDisponible"])
                        );
                }
            }
            else 
                producto = null;
            return producto;
        }
        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCantidad.Text.Length >= 4) && e.KeyChar != 8)
            {
                e.Handled = true;
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
                    decimal venta = 0;
                    decimal iva;
                    decimal totalVenta;
                    btnFinalizar.Location = new Point(873, 407);
                    dgvVentas.Size = new Size(811, 255);
                    visible(true);

                    foreach (DataGridViewRow fila in dgvVentas.Rows)
                    {
                        if (fila.Cells["Total"].Value != null)
                        {
                            venta += Convert.ToDecimal(fila.Cells["Total"].Value);
                        }
                    }
                    iva = Math.Round(venta * 0.15m, 2);
                    totalVenta = Math.Round(venta + iva, 2);
                    string metodoPago = cmbMetodoPago.Text;
                    lblTtlVenta.Text = "$ " + venta.ToString();
                    lblIVA.Text = "$ " + iva.ToString();
                    lblTotal.Text = "$ " + totalVenta.ToString();

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

                    factura = new csFactura(persona.IdPersona, idEmpl, venta, iva, totalVenta, metodoPago);
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
                    }
                    MessageBox.Show("Factura generada correctamente");
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                        fila.Cells["Total"].Value = Convert.ToDecimal(fila.Cells["Cantidad"].Value) * producto.PrecioUnitario;
                        return;
                    }
                }
                decimal cantidad = Convert.ToDecimal(txtCantidad.Text);
                decimal precioTotal = cantidad * producto.PrecioUnitario;
                dgvVentas.Rows.Add(producto.IdProducto, producto.NombreProducto, producto.Descripcion,
                    producto.PrecioUnitario, cantidad, precioTotal);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCedula_TextChanged(object sender, EventArgs e)
        {
            if(txtCedula.Text.Length == 10)
            {
                persona = new csPersona();
                int id = persona.obtenerIdPorCedula(txtCedula.Text);
                reader = crud.EjecutarQuery("Select * from Persona where IdPersona = " + id);
                if(reader != null)
                {
                    if(reader.Read())
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
            if(dgvVentas.SelectedRows.Count > 0)
            {
                DataGridViewRow fila = dgvVentas.Rows[0];
                dgvVentas.Rows.Remove(fila);
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
