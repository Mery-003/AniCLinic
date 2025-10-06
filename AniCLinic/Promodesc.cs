using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class Promodesc : Form
    {
        csCRUD crud = new csCRUD();
        private bool marcando = false;

        public Promodesc()
        {
            InitializeComponent();
            cargarDatos();
            cargarcmb();

            // Eventos
            clbCategorias.ItemCheck += clbCategorias_ItemCheck;
            cbTipo.SelectedIndexChanged += cbTipo_SelectedIndexChanged;
            tbvalor.KeyPress += tbvalor_KeyPress;
            clbProductos.ItemCheck += clbProductos_ItemCheck;

            // Validación automática de fechas
            dtmFecha.MinDate = DateTime.Today;
            Dtfechafin.MinDate = DateTime.Today.AddDays(1);

            // Configuración del CheckedListBox
            clbProductos.CheckOnClick = true;
            clbProductos.HorizontalScrollbar = true;
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

            cbTipo.Items.Clear();
            cbTipo.Items.AddRange(new object[]
            {
                "Descuento",
                "Gratis"
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

        private void cbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTipo.SelectedItem == null) return;

            string tipoSeleccionado = cbTipo.SelectedItem.ToString();

            if (tipoSeleccionado == "Gratis")
            {
                tbvalor.Text = "0";
                tbvalor.ReadOnly = true;
                tbvalor.BackColor = Color.LightGray;
            }
            else if (tipoSeleccionado == "Descuento")
            {
                tbvalor.Text = "";
                tbvalor.ReadOnly = false;
                tbvalor.BackColor = Color.White;
            }
        }

        private void tbvalor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;

            if (!char.IsControl(e.KeyChar))
            {
                string texto = tbvalor.Text + e.KeyChar;
                if (texto.Length > 2) e.Handled = true;

                if (int.TryParse(texto, out int valor))
                    if (valor > 98) e.Handled = true;
            }
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

        private void limpiarCampos()
        {
            tbNombres.Clear();
            tbDescripcion.Clear();
            tbvalor.Clear();
            clbCategorias.SelectedIndex = -1;
            clbProductos.Items.Clear();
            cbTipo.SelectedIndex = -1;
            dtmFecha.Value = DateTime.Today;
            Dtfechafin.Value = DateTime.Today.AddDays(1);
        }

        private void dgvPromo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvPromo.Rows[e.RowIndex];

                // 📌 Datos básicos
                tbNombres.Text = fila.Cells["Nombre"].Value?.ToString() ?? "";
                tbDescripcion.Text = fila.Cells["Descripcion"].Value?.ToString() ?? "";
                tbvalor.Text = fila.Cells["Descuento"].Value?.ToString() ?? "";

                // 📌 Tipo de promoción (Descuento o Gratis)
                string tipo = fila.Cells["Tipo"].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(tipo) && cbTipo.Items.Contains(tipo))
                    cbTipo.SelectedItem = tipo;
                else
                    cbTipo.SelectedIndex = -1;

                // 📌 Fechas
                if (DateTime.TryParse(fila.Cells["FechaInicio"].Value?.ToString(), out DateTime fechaInicio))
                    dtmFecha.Value = fechaInicio;

                if (DateTime.TryParse(fila.Cells["FechaFin"].Value?.ToString(), out DateTime fechaFin))
                    Dtfechafin.Value = fechaFin;

                // 📌 Condición
                string condicion = fila.Cells["Condicion"].Value?.ToString() ?? "";
                ckbVieneCita.Checked = (condicion == "cita");

                // 📌 Categoría y productos
                string categoria = fila.Cells["Categoria"].Value?.ToString() ?? "";
                string productos = fila.Cells["Productos"].Value?.ToString() ?? "";

                // 🔹 Desactivar evento para no borrar productos al marcar categorías
                clbCategorias.ItemCheck -= clbCategorias_ItemCheck;

                // Limpiar checks previos
                for (int i = 0; i < clbCategorias.Items.Count; i++)
                    clbCategorias.SetItemChecked(i, false);

                // Marcar la categoría guardada en BD
                if (!string.IsNullOrEmpty(categoria))
                {
                    int index = clbCategorias.Items.IndexOf(categoria);
                    if (index >= 0)
                        clbCategorias.SetItemChecked(index, true);
                }

                // 🔹 Reactivar evento
                clbCategorias.ItemCheck += clbCategorias_ItemCheck;

                // 📌 Productos seleccionados
                List<string> productosSeleccionados = string.IsNullOrEmpty(productos)
                    ? new List<string>()
                    : productos.Split(new[] { ", " }, StringSplitOptions.None).ToList();

                var categoriasSeleccionadas = new List<string>();
                if (!string.IsNullOrEmpty(categoria))
                    categoriasSeleccionadas.Add(categoria);

                // Cargar productos de esa categoría y marcar los que correspondan
                CargarProductosPorCategorias(categoriasSeleccionadas, productosSeleccionados);
            }
        }

        private void Btcrear_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbNombres.Text) ||
                    string.IsNullOrWhiteSpace(tbDescripcion.Text) ||
                    string.IsNullOrWhiteSpace(tbvalor.Text) ||
                    clbCategorias.SelectedIndex == -1 ||
                    cbTipo.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe llenar todos los campos antes de crear la promoción.");
                    return;
                }

                var productosSeleccionados = new List<string>();
                for (int i = 1; i < clbProductos.Items.Count; i++)
                    if (!clbProductos.Items[i].ToString().StartsWith("---") && clbProductos.GetItemChecked(i))
                        productosSeleccionados.Add(clbProductos.Items[i].ToString());

                if (productosSeleccionados.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar al menos un producto.");
                    return;
                }

                DateTime fechaInicio = dtmFecha.Value.Date;
                DateTime fechaFin = Dtfechafin.Value.Date;

                if (fechaInicio < DateTime.Today)
                {
                    MessageBox.Show("La fecha de inicio no puede ser anterior a hoy.");
                    return;
                }
                if (fechaFin <= fechaInicio)
                {
                    MessageBox.Show("La fecha de fin debe ser mayor a la fecha de inicio.");
                    return;
                }

                if (!decimal.TryParse(tbvalor.Text.Trim(), out decimal valor))
                {
                    MessageBox.Show("Formato de valor incorrecto.");
                    return;
                }

                string tipo = cbTipo.SelectedItem.ToString();
                if (tipo == "Descuento" && valor > 98)
                {
                    MessageBox.Show("El descuento máximo permitido es 98%.");
                    return;
                }

                string sqlPromocion = @"INSERT INTO Promocion
    (Nombre, Descripcion, Tipo, Descuento, Categoria, FechaInicio, FechaFin, Activa, Condicion)
    VALUES (@nombre, @descripcion, @tipo, @valor, @categoria, @fechaInicio, @fechaFin, @activa, @condicion);
    SELECT CAST(scope_identity() AS int)";

                crud.conexion.abrirConexion();
                SqlCommand cmd = new SqlCommand(sqlPromocion, crud.conexion.obtenerConexion());
                cmd.Parameters.AddWithValue("@nombre", tbNombres.Text.Trim());
                cmd.Parameters.AddWithValue("@descripcion", tbDescripcion.Text.Trim());
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.Parameters.AddWithValue("@categoria", clbCategorias.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@condicion", ckbVieneCita.Checked ? "Cita" : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@activa", true);

                int idPromocion = (int)cmd.ExecuteScalar();

                foreach (var producto in productosSeleccionados)
                {
                    DataTable dtProd = crud.cargarBDData(
                        "SELECT IdProducto FROM Inventario WHERE NombreProducto = @producto",
                        new SqlParameter("@producto", producto)
                    );
                    int idProducto = Convert.ToInt32(dtProd.Rows[0]["IdProducto"]);

                    crud.editarBD("INSERT INTO PromocionProducto (IdPromocion, IdProducto) VALUES (@idPromo, @idProd)",
                        new SqlParameter("@idPromo", idPromocion),
                        new SqlParameter("@idProd", idProducto));
                }

                crud.conexion.cerrarConexion();
                MessageBox.Show("Promoción creada correctamente.");
                cargarDatos();
                limpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la promoción: " + ex.Message);
                crud.conexion.cerrarConexion();
            }
        }

        private void btneditar_Click(object sender, EventArgs e)
        {
            if (dgvPromo.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar una promoción.");
                return;
            }

            try
            {
                int idPromo = Convert.ToInt32(dgvPromo.CurrentRow.Cells["IdPromocion"].Value);

                if (string.IsNullOrWhiteSpace(tbNombres.Text) ||
                    string.IsNullOrWhiteSpace(tbDescripcion.Text) ||
                    string.IsNullOrWhiteSpace(tbvalor.Text) ||
                    clbCategorias.SelectedIndex == -1 ||
                    cbTipo.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe llenar todos los campos antes de editar la promoción.");
                    return;
                }

                var productosSeleccionados = new List<string>();
                for (int i = 1; i < clbProductos.Items.Count; i++)
                    if (!clbProductos.Items[i].ToString().StartsWith("---") && clbProductos.GetItemChecked(i))
                        productosSeleccionados.Add(clbProductos.Items[i].ToString());

                if (productosSeleccionados.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar al menos un producto.");
                    return;
                }

                DateTime fechaInicio = dtmFecha.Value.Date;
                DateTime fechaFin = Dtfechafin.Value.Date;

                if (fechaInicio < DateTime.Today)
                {
                    MessageBox.Show("La fecha de inicio no puede ser anterior a hoy.");
                    return;
                }
                if (fechaFin <= fechaInicio)
                {
                    MessageBox.Show("La fecha de fin debe ser mayor a la fecha de inicio.");
                    return;
                }

                if (!decimal.TryParse(tbvalor.Text.Trim(), out decimal valor))
                {
                    MessageBox.Show("Formato de valor incorrecto.");
                    return;
                }

                if (cbTipo.SelectedItem.ToString() == "Descuento" && valor > 98)
                {
                    MessageBox.Show("El descuento máximo permitido es 98%.");
                    return;
                }

                crud.conexion.abrirConexion();

                string sqlActualizar = @"UPDATE Promocion
    SET Nombre = @nombre,
        Descripcion = @descripcion,
        Tipo = @tipo,
        Descuento = @valor,
        Categoria = @categoria,
        FechaInicio = @fechaInicio,
        FechaFin = @fechaFin,
        Condicion = @condicion
    WHERE IdPromocion = @idPromo";

                crud.editarBD(sqlActualizar,
                    new SqlParameter("@nombre", tbNombres.Text.Trim()),
                    new SqlParameter("@descripcion", tbDescripcion.Text.Trim()),
                    new SqlParameter("@tipo", cbTipo.SelectedItem.ToString()),
                    new SqlParameter("@valor", valor),
                    new SqlParameter("@categoria", clbCategorias.SelectedItem.ToString()),
                    new SqlParameter("@fechaInicio", fechaInicio),
                    new SqlParameter("@fechaFin", fechaFin),
                    new SqlParameter("@condicion", ckbVieneCita.Checked ? "Cita" : (object)DBNull.Value),
                    new SqlParameter("@idPromo", idPromo)
                );

                crud.editarBD("DELETE FROM PromocionProducto WHERE IdPromocion = @idPromo",
                    new SqlParameter("@idPromo", idPromo));

                foreach (var producto in productosSeleccionados)
                {
                    DataTable dtProd = crud.cargarBDData(
                        "SELECT IdProducto FROM Inventario WHERE NombreProducto = @producto",
                        new SqlParameter("@producto", producto)
                    );
                    int idProducto = Convert.ToInt32(dtProd.Rows[0]["IdProducto"]);

                    crud.editarBD("INSERT INTO PromocionProducto (IdPromocion, IdProducto) VALUES (@idPromo, @idProd)",
                        new SqlParameter("@idPromo", idPromo),
                        new SqlParameter("@idProd", idProducto));
                }

                crud.conexion.cerrarConexion();
                MessageBox.Show("Promoción editada correctamente.");
                cargarDatos();
                limpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar la promoción: " + ex.Message);
                crud.conexion.cerrarConexion();
            }
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            if (dgvPromo.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar una promoción para eliminar.");
                return;
            }

            int idPromo = Convert.ToInt32(dgvPromo.CurrentRow.Cells["IdPromocion"].Value);

            var confirm = MessageBox.Show("¿Está seguro de eliminar la promoción seleccionada?", "Confirmar eliminación", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            crud.conexion.abrirConexion();
            crud.editarBD("DELETE FROM PromocionProducto WHERE IdPromocion = @idPromo",
                new SqlParameter("@idPromo", idPromo));
            crud.editarBD("DELETE FROM Promocion WHERE IdPromocion = @idPromo",
                new SqlParameter("@idPromo", idPromo));
            crud.conexion.cerrarConexion();

            MessageBox.Show("Promoción eliminada correctamente.");
            cargarDatos();
            limpiarCampos();
        }

        private void btnactivar_Click(object sender, EventArgs e)
        {
            if (dgvPromo.CurrentRow != null)
            {
                int idPromo = Convert.ToInt32(dgvPromo.CurrentRow.Cells["IdPromocion"].Value);
                bool exito = crud.editarBD(
                    "UPDATE Promocion SET Activa = @activa WHERE IdPromocion = @id",
                    new SqlParameter("@activa", true),
                    new SqlParameter("@id", idPromo)
                );

                if (exito)
                {
                    MessageBox.Show("Promoción activada correctamente.");
                    cargarDatos();
                }
            }
        }

        private void btdesactivar_Click(object sender, EventArgs e)
        {
            if (dgvPromo.CurrentRow != null)
            {
                int idPromo = Convert.ToInt32(dgvPromo.CurrentRow.Cells["IdPromocion"].Value);
                bool exito = crud.editarBD(
                    "UPDATE Promocion SET Activa = @activa WHERE IdPromocion = @id",
                    new SqlParameter("@activa", false),
                    new SqlParameter("@id", idPromo)
                );

                if (exito)
                {
                    MessageBox.Show("Promoción desactivada correctamente.");
                    cargarDatos();
                }
            }
        }
    }
}