using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class MasProductos : Form
    {
        csCRUD crud = new csCRUD();
        public MasProductos()
        {
            InitializeComponent();
            cargarcmbProductos();
        }
        public void cargarcmbProductos()
        {
            SqlDataReader reader = crud.EjecutarQuery("Select IdProducto, NombreProducto from Inventario");
            if(reader != null)
            {
                while(reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string nombre = reader.GetString(1);
                    cmbIdProducto.Items.Add(new Productos(id, nombre));
                }
            }
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            int cantInventario;
            int cant = Convert.ToInt32(txtMasProd.Text);
            if (cmbIdProducto.SelectedItem is Productos productoSeleccionado)
            {
                SqlDataReader reader = crud.EjecutarQuery("Select CantidadDisponible from Inventario " +
                    "Where IdProducto = " + productoSeleccionado.idProducto);
                if (reader != null)
                {
                    reader.Read();
                    cantInventario = reader.GetInt32(0);
                    int suma = cantInventario + cant;
                    if (crud.editarBD("Update Inventario set CantidadDisponible = @Suma " +
                        "where IdProducto = @Id",
                        new SqlParameter("@Suma", suma),
                        new SqlParameter("@Id", productoSeleccionado.idProducto)))
                        MessageBox.Show("Productos añadidos");
                    else
                        MessageBox.Show("Error no se pudo añadir los productos");
                    reader.Close();
                }
            }
            this.Close();
        }
    }

    public class Productos
    {
        public int idProducto { get; set; }
        public string nombrePro { get; set; }
        public Productos(int idProducto, string nombre)
        {
            this.idProducto = idProducto;
            this.nombrePro = nombre;
        }
        public override string ToString()
        {
            return $"{idProducto} - {nombrePro}";
        }
    }
}
