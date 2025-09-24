using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class NuvProducto : Form
    {
        csCRUD crud = new csCRUD();
        csProducto prod;
        SqlDataReader reader;
        Inventario fInventario;
        bool editar = false;
        int idProducto;

        public NuvProducto()
        {
            InitializeComponent();
            cargarProveedores();
            cmbCategoria.Items.AddRange(new object[] 
            {
                "Medicamentos",
                "Equipos Medicos",
                "Alimentos",
                "Accesorios",
                "Higiene"
            });
        }
        public NuvProducto(Inventario fI, int idProd)
        {
            fInventario = fI;
            InitializeComponent();
            cargarProveedores();
            idProducto = idProd;
            cmbCategoria.Items.AddRange(new object[]
            {
                "Medicamentos",
                "Equipos Medicos",
                "Alimentos",
                "Accesorios",
                "Higiene"
            });
            editar = true;

            if (idProd != 0)
            {
                prod = CargarProducto(idProd);
                if (prod == null)
                {
                    MessageBox.Show("No se pudo encontrar el producto");
                    return;
                }

                txtNomProducto.Text = prod.NombreProducto;
                txtDescripcion.Text = prod.Descripcion;
                txtCantidad.Text = prod.Cantidad.ToString();
                txtPrecio.Text = prod.PrecioUnitario.ToString();
                cmbCategoria.Text = prod.Categoria;
                cmbProveedor.Text = prod.IdProveedor.ToString();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cargarProveedores()
        {
            reader = crud.EjecutarQuery("Select IdProveedor, NombreProveedor from Proveedor");
            if (reader != null)
            {
                while (reader.Read())
                {
                    int idP = reader.GetInt32(0);
                    string nomP = reader.GetString(1);
                    cmbProveedor.Items.Add(new ProveedorItem(idP, nomP));
                }
            }
        }
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal iva = 0;
                if (!validar())
                    return;
                if (cmbCategoria.SelectedIndex == 1 || cmbCategoria.SelectedIndex == 3 || cmbCategoria.SelectedIndex == 4)
                    iva = 0.15m;
                if (!editar)
                {
                    if (cmbProveedor.SelectedItem is ProveedorItem proveedorSeleccionado)
                    {
                        int idProveedor = proveedorSeleccionado.idProveedor;
                        prod = new csProducto(
                        idProveedor,
                        txtNomProducto.Text,
                        txtDescripcion.Text,
                        cmbCategoria.Text,
                        Convert.ToDecimal(txtPrecio.Text),
                        iva,
                        Convert.ToInt32(txtCantidad.Text));

                        if (prod.agregarProducto())
                            MessageBox.Show("Producto agregado correctamente.");
                        else
                            MessageBox.Show("Error al guardar un producto");
                    }
                }
                else
                {
                    if (cmbProveedor.SelectedItem is ProveedorItem proveedorSeleccionado)
                    {
                        int idProveedor = proveedorSeleccionado.idProveedor;
                        prod = new csProducto(
                        idProveedor,
                        txtNomProducto.Text,
                        txtDescripcion.Text,
                        cmbCategoria.Text,
                        Convert.ToDecimal(txtPrecio.Text),
                        iva,
                        Convert.ToInt32(txtCantidad.Text));

                        if (prod.editarProducto(idProducto))
                            MessageBox.Show("Producto editado correctamente.");
                        else
                            MessageBox.Show("Error al editar el producto");
                    }
                }
                this.Close();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool validar()
        {
            if (string.IsNullOrWhiteSpace(txtNomProducto.Text))
            {
                MessageBox.Show("Llene todos los campos");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Llene todos los campos");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Llene todos los campos");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Llene todos los campos");
                return false;
            }
            if (cmbCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Llene todos los campos");
                return false;
            }
            if (cmbProveedor.SelectedIndex == -1)
            {
                MessageBox.Show("Llene todos los campos");
                return false;
            }
            return true;
        }

        private csProducto CargarProducto(int id)
        {
            prod = null;
            string sentencia = "Select * from Inventario Where IdProducto = " + id;
            using (SqlDataReader reader = crud.EjecutarQuery(sentencia))
            {
                if (reader.Read() && reader != null)
                {
                    prod = new csProducto(
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
            return prod;
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCantidad.Text.Length >= 5) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((char.IsDigit(e.KeyChar) && txtPrecio.Text.Replace(",", "").Replace(".", "").Length >= 6)
                || (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != ',')
                || (e.KeyChar == ',' && txtPrecio.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }
    }

    public class ProveedorItem
    {
        public int idProveedor { get; set; }
        public string nombreProveedor { get; set; }
        public ProveedorItem(int id, string nombre)
        {
            idProveedor = id;
            nombreProveedor = nombre;
        }
        public override string ToString()
        {
            return $"{idProveedor} - {nombreProveedor}";
        }
    }
}
