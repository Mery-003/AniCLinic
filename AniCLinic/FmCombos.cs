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

namespace AniCLinic
{
    public partial class FmCombos : Form
    {
        csCRUD crud = new csCRUD();
        private bool marcando = false;
        public FmCombos()
        {
            InitializeComponent();
            cargarDatos();
            cargarcmb();
        }
        private void cargarDatos()
        {
            DataTable dt = crud.cargarBDData(
                @"SELECT 
    P.IdPromocion, 
    P.Nombre, 
    P.Descripcion, 
    P.Tipo, 
    P.Descuento, 
    P.Categoria,
    STRING_AGG(I.NombreProducto, ', ') AS Productos,
    P.FechaInicio, 
    P.FechaFin,
    P.Condicion,
    P.Activa
FROM 
    Promocion P
INNER JOIN 
    PromocionProducto PP ON P.IdPromocion = PP.IdPromocion
INNER JOIN 
    Inventario I ON I.IdProducto = PP.IdProducto
GROUP BY 
    P.IdPromocion, P.Nombre, P.Descripcion, P.Tipo, 
    P.Descuento, P.Categoria, P.FechaInicio, P.FechaFin, P.Condicion, P.Activa;"
            );
            dgvPromo.DataSource = dt;
            configurarGrid();
        }
        private void configurarGrid()
        {
            dgvPromo.ReadOnly = true;
            dgvPromo.AllowUserToAddRows = false;
            dgvPromo.AllowUserToDeleteRows = false;
            dgvPromo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPromo.MultiSelect = false;
            dgvPromo.AutoGenerateColumns = true;
        }
        private void cargarcmb()
        {
            clbCategorias.Items.Clear();
            clbCategorias.Items.AddRange(new object[]
            {
                "Medicamentos",
                "Equipos Medicos",
                "Alimentos",
                "Accesorios",
                "Higiene"
            });
        }
        private void CargarProductosPorCategorias(List<string> categoriasSeleccionadas, List<string> productosSeleccionados = null)
        {
            clbProductos.Items.Clear();
            clbProductos.Items.Add("Seleccionar Todo");

            if (categoriasSeleccionadas.Count == 0) return;

            foreach (var categoria in categoriasSeleccionadas)
            {
                string header = "--- " + categoria + " ---";
                clbProductos.Items.Add(header);

                string sql = @"SELECT NombreProducto FROM Inventario WHERE Categoria = @cat ORDER BY NombreProducto";
                DataTable dt = crud.cargarBDData(sql, new SqlParameter("@cat", categoria));

                foreach (DataRow row in dt.Rows)
                {
                    string prodName = row["NombreProducto"].ToString();
                    clbProductos.Items.Add(prodName);

                    if (productosSeleccionados != null && productosSeleccionados.Contains(prodName))
                        clbProductos.SetItemChecked(clbProductos.Items.Count - 1, true);
                }
            }

            AjustarTamanoCheckedListBox(clbProductos);
        }

        private void AjustarTamanoCheckedListBox(CheckedListBox clb)
        {
            if (clb.Items.Count == 0) return;

            int itemHeight = clb.ItemHeight;
            int totalItems = clb.Items.Count;
            int altura = (itemHeight * totalItems) + 6;
            int alturaMax = 250;
            clb.Height = Math.Min(altura, alturaMax);

            int anchoMax = 0;
            using (Graphics g = clb.CreateGraphics())
            {
                foreach (var item in clb.Items)
                {
                    int ancho = (int)g.MeasureString(item.ToString(), clb.Font).Width;
                    if (ancho > anchoMax) anchoMax = ancho;
                }
            }
            clb.Width = Math.Min(anchoMax + 40, 400);
        }

        private void clbCategorias_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                var categoriasSeleccionadas = new List<string>();
                foreach (var item in clbCategorias.CheckedItems)
                    categoriasSeleccionadas.Add(item.ToString());

                if (e.NewValue == CheckState.Checked && !categoriasSeleccionadas.Contains(clbCategorias.Items[e.Index].ToString()))
                    categoriasSeleccionadas.Add(clbCategorias.Items[e.Index].ToString());
                else if (e.NewValue == CheckState.Unchecked)
                    categoriasSeleccionadas.Remove(clbCategorias.Items[e.Index].ToString());

                CargarProductosPorCategorias(categoriasSeleccionadas);
            });
        }

        private void clbCategorias_ItemCheck_1(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                var categoriasSeleccionadas = new List<string>();
                foreach (var item in clbCategorias.CheckedItems)
                    categoriasSeleccionadas.Add(item.ToString());

                if (e.NewValue == CheckState.Checked && !categoriasSeleccionadas.Contains(clbCategorias.Items[e.Index].ToString()))
                    categoriasSeleccionadas.Add(clbCategorias.Items[e.Index].ToString());
                else if (e.NewValue == CheckState.Unchecked)
                    categoriasSeleccionadas.Remove(clbCategorias.Items[e.Index].ToString());

                CargarProductosPorCategorias(categoriasSeleccionadas);
            });
        }

        private void clbProductos_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (marcando) return;
            marcando = true;

            if (clbProductos.Items[e.Index].ToString().StartsWith("---"))
            {
                e.NewValue = CheckState.Unchecked;
                marcando = false;
                return;
            }

            if (e.Index == 0)
            {
                bool marcarTodo = e.NewValue == CheckState.Checked;
                for (int i = 1; i < clbProductos.Items.Count; i++)
                    if (!clbProductos.Items[i].ToString().StartsWith("---"))
                        clbProductos.SetItemChecked(i, marcarTodo);
            }
            else if (e.Index > 0 && e.NewValue == CheckState.Unchecked)
            {
                clbProductos.SetItemChecked(0, false);
            }

            marcando = false;
        }
    }
}

