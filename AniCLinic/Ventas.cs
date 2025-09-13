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
        SqlDataReader reader;
        public Ventas()
        {
            InitializeComponent();
            visible(false);
            prepararGrid();
            cargarProductos();
            cmbCategoria.Items.AddRange(new object[]
            {
                "Medicamentos",
                "Equipos Medicos",
                "Alimentos",
                "Accesorios",
                "Higiene"
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
            lblTtlVenta.Text = venta.ToString();
            lblIVA.Text = iva.ToString();
            lblTotal.Text = totalVenta.ToString();
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
            producto = cargarProducto();
            decimal cantidad = Convert.ToDecimal(txtCantidad.Text);
            decimal precioTotal = cantidad * producto.PrecioUnitario;
            dgvVentas.Rows.Add(producto.IdProducto, producto.NombreProducto, producto.Descripcion,
                producto.PrecioUnitario, cantidad, precioTotal);
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
            cmbProducto.Items.Clear();
            cargarProductos(cmbCategoria.Text);
        }

        private void cmbProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
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
