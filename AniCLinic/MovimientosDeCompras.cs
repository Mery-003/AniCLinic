using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class MovimientosDeCompras : Form
    {
        private readonly csCRUD _crud = new csCRUD();
        private DataTable _dt;

        // 🔧 Ajusta si tu columna de stock en Inventario se llama distinto
        // antes decía "Stock"
        private const string COL_STOCK = "CantidadDisponible";

        public MovimientosDeCompras()
        {
            InitializeComponent();
            ConfigurarGrid();

            this.Load += (s, e) =>
            {
                CargarGrid();
                AgregarColumnaVerOrden();
                AgregarColumnaEntregar();
                PostFormato();
                RefrescarBotonesEntregar();
            };

            dgvmovimiento.CellContentClick += dgvmovimiento_CellContentClick;
            dgvmovimiento.CellDoubleClick += (s, e2) => { if (e2.RowIndex >= 0) AbrirOrdenPorFila(e2.RowIndex); };
            dgvmovimiento.DataBindingComplete += (s, e3) => { PostFormato(); RefrescarBotonesEntregar(); };

            // 🔎 filtro en vivo
            if (txtBuscarCompra != null)
                txtBuscarCompra.TextChanged += (s, e4) => FiltrarCompras();
        }

        private void ConfigurarGrid()
        {
            dgvmovimiento.Dock = DockStyle.None;
            dgvmovimiento.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            dgvmovimiento.AutoSize = false;

            dgvmovimiento.ReadOnly = true;
            dgvmovimiento.MultiSelect = false;
            dgvmovimiento.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvmovimiento.AllowUserToAddRows = false;
            dgvmovimiento.RowHeadersVisible = false;

            dgvmovimiento.AutoGenerateColumns = true;
            dgvmovimiento.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvmovimiento.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvmovimiento.ScrollBars = ScrollBars.Both;
        }

        private void CargarGrid()
        {
            // Incluye estado lógico (bit) y texto.
            const string sql = @"
SELECT
    c.IdCompra,                               -- oculto
    c.NumeroOC              AS [N° Orden],
    c.FechaCompra,
    c.FechaEmision,
    p.RUC,
    p.NombreProveedor       AS Proveedor,
    c.MetodoPago,
    c.Subtotal0,
    c.Subtotal12,
    c.IVA12,
    c.Total,
    c.Entregado             AS EntregadoBit,  -- oculto para lógica
    CASE WHEN c.Entregado = 1 THEN 'Entregado' ELSE 'Pendiente' END AS Estado
FROM dbo.OC_Compra c
JOIN dbo.Proveedor p ON p.IdProveedor = c.IdProveedor
ORDER BY c.IdCompra DESC;";

            _dt = _crud.cargarBDData(sql);
            dgvmovimiento.DataSource = _dt;

            if (dgvmovimiento.Columns.Contains("FechaCompra"))
                dgvmovimiento.Columns["FechaCompra"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            if (dgvmovimiento.Columns.Contains("FechaEmision"))
                dgvmovimiento.Columns["FechaEmision"].DefaultCellStyle.Format = "dd/MM/yyyy";
            foreach (var col in new[] { "Subtotal0", "Subtotal12", "IVA12", "Total" })
                if (dgvmovimiento.Columns.Contains(col))
                    dgvmovimiento.Columns[col].DefaultCellStyle.Format = "N2";
        }

        private void PostFormato()
        {
            if (dgvmovimiento.Columns.Contains("IdCompra"))
                dgvmovimiento.Columns["IdCompra"].Visible = false;
            if (dgvmovimiento.Columns.Contains("EntregadoBit"))
                dgvmovimiento.Columns["EntregadoBit"].Visible = false;

            if (!dgvmovimiento.Columns.Contains("colVerOrden"))
                AgregarColumnaVerOrden();
            if (!dgvmovimiento.Columns.Contains("colEntregar"))
                AgregarColumnaEntregar();

            // Orden: ... Estado (antes de Ver), Ver orden, Entregar
            string[] orden =
            {
                "N° Orden","FechaCompra","FechaEmision","RUC","Proveedor",
                "MetodoPago","Subtotal0","Subtotal12","IVA12","Total",
                "Estado","colVerOrden","colEntregar"
            };

            foreach (var nombre in orden)
                if (dgvmovimiento.Columns.Contains(nombre))
                    dgvmovimiento.Columns[nombre].DisplayIndex = Array.IndexOf(orden, nombre);
        }

        private void AgregarColumnaVerOrden()
        {
            if (dgvmovimiento.Columns.Contains("colVerOrden")) return;

            var col = new DataGridViewButtonColumn
            {
                Name = "colVerOrden",
                HeaderText = "",
                Text = "Ver orden",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            };
            dgvmovimiento.Columns.Add(col);
        }

        private void AgregarColumnaEntregar()
        {
            if (dgvmovimiento.Columns.Contains("colEntregar")) return;

            var col = new DataGridViewButtonColumn
            {
                Name = "colEntregar",
                HeaderText = "",
                Text = "Entregar",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            };
            dgvmovimiento.Columns.Add(col);
        }

        private void RefrescarBotonesEntregar()
        {
            if (!dgvmovimiento.Columns.Contains("colEntregar") || _dt == null) return;

            foreach (DataGridViewRow row in dgvmovimiento.Rows)
            {
                if (row.DataBoundItem is DataRowView drv)
                {
                    bool entregado = drv.Row.Field<bool>("EntregadoBit");
                    var btn = row.Cells["colEntregar"] as DataGridViewButtonCell;

                    if (entregado)
                    {
                        btn.Value = "—";
                        btn.Style.ForeColor = Color.Gray;
                        btn.Style.BackColor = Color.Gainsboro;
                        btn.FlatStyle = FlatStyle.Popup;
                    }
                    else
                    {
                        btn.Value = "Entregar";
                        btn.Style.ForeColor = Color.Black;
                        btn.Style.BackColor = Color.Empty;
                        btn.FlatStyle = FlatStyle.Standard;
                    }
                }
            }
        }

        private void dgvmovimiento_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var colName = dgvmovimiento.Columns[e.ColumnIndex].Name;

            if (colName == "colVerOrden")
            {
                AbrirOrdenPorFila(e.RowIndex);
                return;
            }

            if (colName == "colEntregar")
            {
                var drv = dgvmovimiento.Rows[e.RowIndex].DataBoundItem as DataRowView;
                if (drv == null) return;

                bool entregado = drv.Row.Field<bool>("EntregadoBit");
                if (entregado) return; // bloqueado

                int idCompra = Convert.ToInt32(drv.Row["IdCompra"]);

                try
                {
                    if (MarcarEntregaYActualizarStock(idCompra))
                    {
                        // reflejar cambios en la fila
                        drv["EntregadoBit"] = true;
                        drv["Estado"] = "Entregado";
                        RefrescarBotonesEntregar();
                    }
                    else
                    {
                        MessageBox.Show("La orden ya estaba marcada como entregada.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // refresco por si otro proceso cambió el estado
                        CargarGrid();
                        PostFormato();
                        RefrescarBotonesEntregar();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo registrar la entrega: " + ex.Message,
                                    "Compras", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AbrirOrdenPorFila(int rowIndex)
        {
            var drv = dgvmovimiento.Rows[rowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            var val = drv["IdCompra"];
            if (val == null || val == DBNull.Value) return;

            int idCompra = Convert.ToInt32(val);
            using (var frm = new frOrdenCompra(idCompra))
                frm.ShowDialog(this);
        }

        // 🔒 Lógica atómica: si ya está entregada, no hace nada; si no, suma stock y marca entregada.
        private bool MarcarEntregaYActualizarStock(int idCompra)
        {
            var db = new csConexionBD();
            SqlTransaction tx = null;

            try
            {
                db.abrirConexion();
                var cn = db.obtenerConexion();
                tx = cn.BeginTransaction(System.Data.IsolationLevel.Serializable);

                // 1) ¿ya entregada?
                bool yaEntregado;
                using (var cmdChk = new SqlCommand(
                    "SELECT Entregado FROM dbo.OC_Compra WITH (UPDLOCK, HOLDLOCK) WHERE IdCompra=@id",
                    cn, tx))
                {
                    cmdChk.Parameters.Add("@id", SqlDbType.Int).Value = idCompra;
                    yaEntregado = Convert.ToBoolean(cmdChk.ExecuteScalar() ?? false);
                }
                if (yaEntregado)
                {
                    tx.Rollback();
                    return false; // ya estaba entregada
                }

                // 2) cuántos productos debería actualizar esta orden
                int expected;
                using (var cmdExp = new SqlCommand(
                    @"SELECT COUNT(DISTINCT IdProducto)
              FROM dbo.OC_CompraDetalle WHERE IdCompra=@id;", cn, tx))
                {
                    cmdExp.Parameters.Add("@id", SqlDbType.Int).Value = idCompra;
                    expected = Convert.ToInt32(cmdExp.ExecuteScalar() ?? 0);
                }

                // 3) sumar existencias SOLO para productos existentes en Inventario
                var sqlSumar = $@"
;WITH d AS(
    SELECT cd.IdProducto, SUM(cd.Cantidad) AS CantTotal
    FROM dbo.OC_CompraDetalle cd
    WHERE cd.IdCompra = @id
    GROUP BY cd.IdProducto
)
UPDATE i
   SET i.[{COL_STOCK}] = ISNULL(i.[{COL_STOCK}],0) + d.CantTotal
  FROM dbo.Inventario i
  JOIN d ON d.IdProducto = i.IdProducto;";

                int rowsUpd;
                using (var cmdUpd = new SqlCommand(sqlSumar, cn, tx))
                {
                    cmdUpd.Parameters.Add("@id", SqlDbType.Int).Value = idCompra;
                    rowsUpd = cmdUpd.ExecuteNonQuery();  // filas de Inventario afectadas
                }

                // 4) si no se actualizaron todos, abortar y decir cuáles faltan
                if (rowsUpd != expected)
                {
                    using (var cmdMiss = new SqlCommand(@"
                ;WITH d AS(
                    SELECT cd.IdProducto
                    FROM dbo.OC_CompraDetalle cd
                    WHERE cd.IdCompra = @id
                    GROUP BY cd.IdProducto
                )
                SELECT d.IdProducto
                FROM d
                LEFT JOIN dbo.Inventario i ON i.IdProducto = d.IdProducto
                WHERE i.IdProducto IS NULL;", cn, tx))
                    {
                        cmdMiss.Parameters.Add("@id", SqlDbType.Int).Value = idCompra;
                        using (var rd = cmdMiss.ExecuteReader())
                        {
                            var faltan = new System.Collections.Generic.List<int>();
                            while (rd.Read()) faltan.Add(rd.GetInt32(0));
                            tx.Rollback();
                            throw new InvalidOperationException(
                                "Hay productos de la orden que no existen en Inventario: " +
                                (faltan.Count == 0 ? "(desconocidos)" : string.Join(", ", faltan)));
                        }
                    }
                }

                // 5) marcar como entregada (no cambia el esquema)
                using (var cmdFlag = new SqlCommand(
                    "UPDATE dbo.OC_Compra SET Entregado=1, FechaEntrega=GETDATE() WHERE IdCompra=@id;",
                    cn, tx))
                {
                    cmdFlag.Parameters.Add("@id", SqlDbType.Int).Value = idCompra;
                    cmdFlag.ExecuteNonQuery();
                }

                tx.Commit();
                return true;
            }
            catch
            {
                try { tx?.Rollback(); } catch { }
                throw;
            }
            finally
            {
                db.cerrarConexion();
            }
        }



        // 🔎 Filtro en vivo por N° Orden / Proveedor / RUC / Método de pago
        private void FiltrarCompras()
        {
            if (_dt == null) return;
            string q = (txtBuscarCompra?.Text ?? "").Trim().Replace("'", "''");

            if (string.IsNullOrEmpty(q))
            {
                _dt.DefaultView.RowFilter = "";
                return;
            }

            _dt.DefaultView.RowFilter =
                $"Convert([N° Orden], 'System.String') LIKE '%{q}%' " +
                $"OR Proveedor LIKE '%{q}%' " +
                $"OR RUC LIKE '%{q}%' " +
                $"OR MetodoPago LIKE '%{q}%'";
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
