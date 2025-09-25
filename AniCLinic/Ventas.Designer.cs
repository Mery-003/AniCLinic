namespace AniCLinic
{
    partial class Ventas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ventas));
            this.dgvVentas = new Guna.UI2.WinForms.Guna2DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Nombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Precio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Iva = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmbProducto = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cmbCategoria = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txtCantidad = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2HtmlLabel10 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtNombre = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtCelular = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtCedula = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel4 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel5 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel6 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtCorreo = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2HtmlLabel7 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtPrecio = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnAgregar = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnEliminar = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnFinalizar = new Guna.UI2.WinForms.Guna2GradientButton();
            this.lblTtlVno = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTtlVenta = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblIVAno = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblIVA = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTotalno = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTotal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnImprimir = new Guna.UI2.WinForms.Guna2GradientButton();
            this.cmbMetodoPago = new Guna.UI2.WinForms.Guna2ComboBox();
            this.guna2HtmlLabel8 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Button9 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2HtmlLabel9 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2HtmlLabel11 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Separator2 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2HtmlLabel12 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Separator3 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2Button2 = new Guna.UI2.WinForms.Guna2Button();
            this.btnAbrirListaPropietario = new Guna.UI2.WinForms.Guna2GradientButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvVentas
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvVentas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvVentas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvVentas.ColumnHeadersHeight = 50;
            this.dgvVentas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvVentas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Nombre,
            this.Descripcion,
            this.Precio,
            this.Iva,
            this.Cantidad,
            this.Total});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvVentas.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvVentas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvVentas.Location = new System.Drawing.Point(28, 393);
            this.dgvVentas.Name = "dgvVentas";
            this.dgvVentas.RowHeadersVisible = false;
            this.dgvVentas.RowHeadersWidth = 50;
            this.dgvVentas.Size = new System.Drawing.Size(998, 209);
            this.dgvVentas.TabIndex = 57;
            this.dgvVentas.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvVentas.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvVentas.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvVentas.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvVentas.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvVentas.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvVentas.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvVentas.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.dgvVentas.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvVentas.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvVentas.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.DarkCyan;
            this.dgvVentas.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvVentas.ThemeStyle.HeaderStyle.Height = 50;
            this.dgvVentas.ThemeStyle.ReadOnly = false;
            this.dgvVentas.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvVentas.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvVentas.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvVentas.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvVentas.ThemeStyle.RowsStyle.Height = 22;
            this.dgvVentas.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvVentas.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            // 
            // Nombre
            // 
            this.Nombre.HeaderText = "Nombre";
            this.Nombre.Name = "Nombre";
            // 
            // Descripcion
            // 
            this.Descripcion.HeaderText = "Descripcion";
            this.Descripcion.Name = "Descripcion";
            // 
            // Precio
            // 
            this.Precio.HeaderText = "Precio";
            this.Precio.Name = "Precio";
            // 
            // Iva
            // 
            this.Iva.HeaderText = "IVA";
            this.Iva.Name = "Iva";
            // 
            // Cantidad
            // 
            this.Cantidad.HeaderText = "Cantidad";
            this.Cantidad.Name = "Cantidad";
            // 
            // Total
            // 
            this.Total.HeaderText = "Total";
            this.Total.Name = "Total";
            // 
            // cmbProducto
            // 
            this.cmbProducto.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbProducto.BorderColor = System.Drawing.Color.Silver;
            this.cmbProducto.BorderRadius = 19;
            this.cmbProducto.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProducto.FillColor = System.Drawing.Color.LightGray;
            this.cmbProducto.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbProducto.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbProducto.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.cmbProducto.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.cmbProducto.ItemHeight = 30;
            this.cmbProducto.Location = new System.Drawing.Point(72, 237);
            this.cmbProducto.Name = "cmbProducto";
            this.cmbProducto.Size = new System.Drawing.Size(225, 36);
            this.cmbProducto.TabIndex = 129;
            this.cmbProducto.SelectedIndexChanged += new System.EventHandler(this.cmbProducto_SelectedIndexChanged);
            // 
            // cmbCategoria
            // 
            this.cmbCategoria.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbCategoria.BorderColor = System.Drawing.Color.Silver;
            this.cmbCategoria.BorderRadius = 19;
            this.cmbCategoria.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.FillColor = System.Drawing.Color.LightGray;
            this.cmbCategoria.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbCategoria.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbCategoria.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.cmbCategoria.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.cmbCategoria.ItemHeight = 30;
            this.cmbCategoria.Location = new System.Drawing.Point(72, 181);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new System.Drawing.Size(225, 36);
            this.cmbCategoria.TabIndex = 130;
            this.cmbCategoria.SelectedIndexChanged += new System.EventHandler(this.cmbCategoria_SelectedIndexChanged);
            // 
            // txtCantidad
            // 
            this.txtCantidad.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCantidad.BorderColor = System.Drawing.Color.Silver;
            this.txtCantidad.BorderRadius = 20;
            this.txtCantidad.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCantidad.DefaultText = "";
            this.txtCantidad.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCantidad.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCantidad.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCantidad.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCantidad.FillColor = System.Drawing.Color.LightGray;
            this.txtCantidad.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCantidad.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCantidad.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCantidad.Location = new System.Drawing.Point(72, 294);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.PlaceholderText = "";
            this.txtCantidad.SelectedText = "";
            this.txtCantidad.Size = new System.Drawing.Size(225, 32);
            this.txtCantidad.TabIndex = 133;
            this.txtCantidad.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCantidad_KeyPress);
            // 
            // guna2HtmlLabel10
            // 
            this.guna2HtmlLabel10.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel10.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel10.Location = new System.Drawing.Point(81, 220);
            this.guna2HtmlLabel10.Name = "guna2HtmlLabel10";
            this.guna2HtmlLabel10.Size = new System.Drawing.Size(57, 17);
            this.guna2HtmlLabel10.TabIndex = 134;
            this.guna2HtmlLabel10.Text = "Producto:";
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(82, 163);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(59, 17);
            this.guna2HtmlLabel1.TabIndex = 135;
            this.guna2HtmlLabel1.Text = "Categoria:";
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(49, 49);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(104, 17);
            this.guna2HtmlLabel2.TabIndex = 136;
            this.guna2HtmlLabel2.Text = "Cedula del cliente:";
            // 
            // txtNombre
            // 
            this.txtNombre.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtNombre.BorderColor = System.Drawing.Color.Silver;
            this.txtNombre.BorderRadius = 16;
            this.txtNombre.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNombre.DefaultText = "";
            this.txtNombre.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtNombre.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtNombre.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtNombre.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtNombre.FillColor = System.Drawing.Color.LightGray;
            this.txtNombre.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtNombre.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNombre.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtNombre.Location = new System.Drawing.Point(566, 66);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.PlaceholderText = "";
            this.txtNombre.ReadOnly = true;
            this.txtNombre.SelectedText = "";
            this.txtNombre.Size = new System.Drawing.Size(200, 32);
            this.txtNombre.TabIndex = 137;
            // 
            // txtCelular
            // 
            this.txtCelular.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCelular.BorderColor = System.Drawing.Color.Silver;
            this.txtCelular.BorderRadius = 16;
            this.txtCelular.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCelular.DefaultText = "";
            this.txtCelular.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCelular.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCelular.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCelular.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCelular.FillColor = System.Drawing.Color.LightGray;
            this.txtCelular.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCelular.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCelular.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCelular.Location = new System.Drawing.Point(330, 66);
            this.txtCelular.Name = "txtCelular";
            this.txtCelular.PlaceholderText = "";
            this.txtCelular.ReadOnly = true;
            this.txtCelular.SelectedText = "";
            this.txtCelular.Size = new System.Drawing.Size(200, 32);
            this.txtCelular.TabIndex = 138;
            // 
            // txtCedula
            // 
            this.txtCedula.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCedula.BorderColor = System.Drawing.Color.Silver;
            this.txtCedula.BorderRadius = 16;
            this.txtCedula.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCedula.DefaultText = "";
            this.txtCedula.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCedula.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCedula.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCedula.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCedula.FillColor = System.Drawing.Color.LightGray;
            this.txtCedula.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCedula.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCedula.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCedula.Location = new System.Drawing.Point(42, 66);
            this.txtCedula.Name = "txtCedula";
            this.txtCedula.PlaceholderText = "";
            this.txtCedula.SelectedText = "";
            this.txtCedula.Size = new System.Drawing.Size(200, 32);
            this.txtCedula.TabIndex = 139;
            this.txtCedula.TextChanged += new System.EventHandler(this.txtCedula_TextChanged);
            this.txtCedula.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCedula_KeyPress);
            // 
            // guna2HtmlLabel3
            // 
            this.guna2HtmlLabel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel3.Location = new System.Drawing.Point(572, 49);
            this.guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            this.guna2HtmlLabel3.Size = new System.Drawing.Size(113, 17);
            this.guna2HtmlLabel3.TabIndex = 140;
            this.guna2HtmlLabel3.Text = "Nombre del cliente:";
            // 
            // guna2HtmlLabel4
            // 
            this.guna2HtmlLabel4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel4.Location = new System.Drawing.Point(330, 49);
            this.guna2HtmlLabel4.Name = "guna2HtmlLabel4";
            this.guna2HtmlLabel4.Size = new System.Drawing.Size(44, 17);
            this.guna2HtmlLabel4.TabIndex = 141;
            this.guna2HtmlLabel4.Text = "Celular:";
            // 
            // guna2HtmlLabel5
            // 
            this.guna2HtmlLabel5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel5.Location = new System.Drawing.Point(83, 277);
            this.guna2HtmlLabel5.Name = "guna2HtmlLabel5";
            this.guna2HtmlLabel5.Size = new System.Drawing.Size(54, 17);
            this.guna2HtmlLabel5.TabIndex = 142;
            this.guna2HtmlLabel5.Text = "Cantidad:";
            // 
            // guna2HtmlLabel6
            // 
            this.guna2HtmlLabel6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel6.Location = new System.Drawing.Point(821, 49);
            this.guna2HtmlLabel6.Name = "guna2HtmlLabel6";
            this.guna2HtmlLabel6.Size = new System.Drawing.Size(44, 17);
            this.guna2HtmlLabel6.TabIndex = 144;
            this.guna2HtmlLabel6.Text = "Correo:";
            // 
            // txtCorreo
            // 
            this.txtCorreo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCorreo.BorderColor = System.Drawing.Color.Silver;
            this.txtCorreo.BorderRadius = 16;
            this.txtCorreo.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCorreo.DefaultText = "";
            this.txtCorreo.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCorreo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCorreo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCorreo.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCorreo.FillColor = System.Drawing.Color.LightGray;
            this.txtCorreo.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCorreo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCorreo.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCorreo.Location = new System.Drawing.Point(817, 66);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.PlaceholderText = "";
            this.txtCorreo.ReadOnly = true;
            this.txtCorreo.SelectedText = "";
            this.txtCorreo.Size = new System.Drawing.Size(200, 32);
            this.txtCorreo.TabIndex = 143;
            // 
            // guna2HtmlLabel7
            // 
            this.guna2HtmlLabel7.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel7.Location = new System.Drawing.Point(82, 341);
            this.guna2HtmlLabel7.Name = "guna2HtmlLabel7";
            this.guna2HtmlLabel7.Size = new System.Drawing.Size(41, 17);
            this.guna2HtmlLabel7.TabIndex = 146;
            this.guna2HtmlLabel7.Text = "Precio:";
            // 
            // txtPrecio
            // 
            this.txtPrecio.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtPrecio.BorderColor = System.Drawing.Color.Silver;
            this.txtPrecio.BorderRadius = 20;
            this.txtPrecio.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPrecio.DefaultText = "";
            this.txtPrecio.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtPrecio.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtPrecio.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPrecio.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPrecio.FillColor = System.Drawing.Color.LightGray;
            this.txtPrecio.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPrecio.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPrecio.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPrecio.Location = new System.Drawing.Point(137, 336);
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.PlaceholderText = "";
            this.txtPrecio.ReadOnly = true;
            this.txtPrecio.SelectedText = "";
            this.txtPrecio.Size = new System.Drawing.Size(59, 32);
            this.txtPrecio.TabIndex = 145;
            this.txtPrecio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPrecio_KeyPress);
            // 
            // btnAgregar
            // 
            this.btnAgregar.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnAgregar.BorderRadius = 19;
            this.btnAgregar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnAgregar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnAgregar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnAgregar.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnAgregar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnAgregar.FillColor2 = System.Drawing.Color.DarkOliveGreen;
            this.btnAgregar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregar.ForeColor = System.Drawing.Color.White;
            this.btnAgregar.Location = new System.Drawing.Point(371, 208);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(97, 41);
            this.btnAgregar.TabIndex = 147;
            this.btnAgregar.Text = "AGREGAR";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnEliminar.BorderRadius = 19;
            this.btnEliminar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnEliminar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnEliminar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnEliminar.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnEliminar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnEliminar.FillColor2 = System.Drawing.Color.DarkOliveGreen;
            this.btnEliminar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(371, 268);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(97, 41);
            this.btnEliminar.TabIndex = 148;
            this.btnEliminar.Text = "ELIMINAR";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnFinalizar
            // 
            this.btnFinalizar.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnFinalizar.BorderRadius = 19;
            this.btnFinalizar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnFinalizar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnFinalizar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnFinalizar.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnFinalizar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnFinalizar.FillColor2 = System.Drawing.Color.DarkOliveGreen;
            this.btnFinalizar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFinalizar.ForeColor = System.Drawing.Color.White;
            this.btnFinalizar.Location = new System.Drawing.Point(889, 268);
            this.btnFinalizar.Name = "btnFinalizar";
            this.btnFinalizar.Size = new System.Drawing.Size(97, 41);
            this.btnFinalizar.TabIndex = 149;
            this.btnFinalizar.Text = "FINALIZAR";
            this.btnFinalizar.Click += new System.EventHandler(this.btnFinalizar_Click);
            // 
            // lblTtlVno
            // 
            this.lblTtlVno.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblTtlVno.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTtlVno.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTtlVno.Location = new System.Drawing.Point(594, 268);
            this.lblTtlVno.Name = "lblTtlVno";
            this.lblTtlVno.Size = new System.Drawing.Size(76, 19);
            this.lblTtlVno.TabIndex = 150;
            this.lblTtlVno.Text = "SUBTOTAL:";
            // 
            // lblTtlVenta
            // 
            this.lblTtlVenta.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblTtlVenta.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTtlVenta.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTtlVenta.Location = new System.Drawing.Point(691, 268);
            this.lblTtlVenta.Name = "lblTtlVenta";
            this.lblTtlVenta.Size = new System.Drawing.Size(21, 19);
            this.lblTtlVenta.TabIndex = 151;
            this.lblTtlVenta.Text = "$ 0";
            // 
            // lblIVAno
            // 
            this.lblIVAno.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblIVAno.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIVAno.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblIVAno.Location = new System.Drawing.Point(595, 296);
            this.lblIVAno.Name = "lblIVAno";
            this.lblIVAno.Size = new System.Drawing.Size(31, 19);
            this.lblIVAno.TabIndex = 152;
            this.lblIVAno.Text = "IVA:";
            // 
            // lblIVA
            // 
            this.lblIVA.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblIVA.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblIVA.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblIVA.Location = new System.Drawing.Point(691, 296);
            this.lblIVA.Name = "lblIVA";
            this.lblIVA.Size = new System.Drawing.Size(21, 19);
            this.lblIVA.TabIndex = 153;
            this.lblIVA.Text = "$ 0";
            // 
            // lblTotalno
            // 
            this.lblTotalno.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblTotalno.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalno.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTotalno.Location = new System.Drawing.Point(597, 321);
            this.lblTotalno.Name = "lblTotalno";
            this.lblTotalno.Size = new System.Drawing.Size(50, 19);
            this.lblTotalno.TabIndex = 154;
            this.lblTotalno.Text = "TOTAL:";
            // 
            // lblTotal
            // 
            this.lblTotal.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTotal.Location = new System.Drawing.Point(691, 321);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(21, 19);
            this.lblTotal.TabIndex = 155;
            this.lblTotal.Text = "$ 0";
            // 
            // btnImprimir
            // 
            this.btnImprimir.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnImprimir.BorderRadius = 19;
            this.btnImprimir.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnImprimir.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnImprimir.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnImprimir.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnImprimir.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnImprimir.FillColor2 = System.Drawing.Color.DarkOliveGreen;
            this.btnImprimir.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnImprimir.ForeColor = System.Drawing.Color.White;
            this.btnImprimir.Location = new System.Drawing.Point(889, 208);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(97, 41);
            this.btnImprimir.TabIndex = 156;
            this.btnImprimir.Text = "IMPRIMIR";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // cmbMetodoPago
            // 
            this.cmbMetodoPago.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbMetodoPago.BorderColor = System.Drawing.Color.Silver;
            this.cmbMetodoPago.BorderRadius = 19;
            this.cmbMetodoPago.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetodoPago.FillColor = System.Drawing.Color.LightGray;
            this.cmbMetodoPago.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbMetodoPago.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbMetodoPago.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.cmbMetodoPago.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.cmbMetodoPago.ItemHeight = 30;
            this.cmbMetodoPago.Location = new System.Drawing.Point(586, 195);
            this.cmbMetodoPago.Name = "cmbMetodoPago";
            this.cmbMetodoPago.Size = new System.Drawing.Size(225, 36);
            this.cmbMetodoPago.TabIndex = 157;
            // 
            // guna2HtmlLabel8
            // 
            this.guna2HtmlLabel8.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel8.Location = new System.Drawing.Point(591, 178);
            this.guna2HtmlLabel8.Name = "guna2HtmlLabel8";
            this.guna2HtmlLabel8.Size = new System.Drawing.Size(97, 17);
            this.guna2HtmlLabel8.TabIndex = 158;
            this.guna2HtmlLabel8.Text = "Metodo de Pago:";
            // 
            // guna2Button9
            // 
            this.guna2Button9.BackColor = System.Drawing.Color.Transparent;
            this.guna2Button9.BorderRadius = 24;
            this.guna2Button9.CausesValidation = false;
            this.guna2Button9.CheckedState.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Button9.DisabledState.BorderColor = System.Drawing.Color.LightGray;
            this.guna2Button9.DisabledState.CustomBorderColor = System.Drawing.Color.LightGray;
            this.guna2Button9.DisabledState.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Button9.DisabledState.ForeColor = System.Drawing.Color.LightGray;
            this.guna2Button9.Enabled = false;
            this.guna2Button9.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Button9.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button9.ForeColor = System.Drawing.Color.White;
            this.guna2Button9.Location = new System.Drawing.Point(25, 12);
            this.guna2Button9.Name = "guna2Button9";
            this.guna2Button9.Size = new System.Drawing.Size(1012, 100);
            this.guna2Button9.TabIndex = 145;
            // 
            // guna2Separator1
            // 
            this.guna2Separator1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Separator1.FillColor = System.Drawing.Color.DarkSlateGray;
            this.guna2Separator1.Location = new System.Drawing.Point(26, 36);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(1011, 10);
            this.guna2Separator1.TabIndex = 159;
            // 
            // guna2HtmlLabel9
            // 
            this.guna2HtmlLabel9.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel9.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel9.Location = new System.Drawing.Point(51, 19);
            this.guna2HtmlLabel9.Name = "guna2HtmlLabel9";
            this.guna2HtmlLabel9.Size = new System.Drawing.Size(133, 19);
            this.guna2HtmlLabel9.TabIndex = 160;
            this.guna2HtmlLabel9.Text = "DATOS DEL CLIENTE";
            // 
            // guna2Button1
            // 
            this.guna2Button1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Button1.BorderRadius = 24;
            this.guna2Button1.CausesValidation = false;
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.LightGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.LightGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.LightGray;
            this.guna2Button1.Enabled = false;
            this.guna2Button1.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Location = new System.Drawing.Point(25, 130);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(488, 249);
            this.guna2Button1.TabIndex = 161;
            this.guna2Button1.UseTransparentBackground = true;
            // 
            // guna2HtmlLabel11
            // 
            this.guna2HtmlLabel11.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel11.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel11.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel11.Location = new System.Drawing.Point(50, 134);
            this.guna2HtmlLabel11.Name = "guna2HtmlLabel11";
            this.guna2HtmlLabel11.Size = new System.Drawing.Size(77, 19);
            this.guna2HtmlLabel11.TabIndex = 164;
            this.guna2HtmlLabel11.Text = "PRODUCTO";
            // 
            // guna2Separator2
            // 
            this.guna2Separator2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Separator2.FillColor = System.Drawing.Color.DarkSlateGray;
            this.guna2Separator2.Location = new System.Drawing.Point(25, 151);
            this.guna2Separator2.Name = "guna2Separator2";
            this.guna2Separator2.Size = new System.Drawing.Size(489, 10);
            this.guna2Separator2.TabIndex = 163;
            // 
            // guna2HtmlLabel12
            // 
            this.guna2HtmlLabel12.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2HtmlLabel12.Font = new System.Drawing.Font("Segoe UI Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel12.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel12.Location = new System.Drawing.Point(566, 134);
            this.guna2HtmlLabel12.Name = "guna2HtmlLabel12";
            this.guna2HtmlLabel12.Size = new System.Drawing.Size(114, 19);
            this.guna2HtmlLabel12.TabIndex = 167;
            this.guna2HtmlLabel12.Text = "PAGO Y TOTALES";
            // 
            // guna2Separator3
            // 
            this.guna2Separator3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Separator3.FillColor = System.Drawing.Color.DarkSlateGray;
            this.guna2Separator3.Location = new System.Drawing.Point(541, 151);
            this.guna2Separator3.Name = "guna2Separator3";
            this.guna2Separator3.Size = new System.Drawing.Size(489, 10);
            this.guna2Separator3.TabIndex = 166;
            // 
            // guna2Button2
            // 
            this.guna2Button2.BackColor = System.Drawing.Color.Transparent;
            this.guna2Button2.BorderRadius = 24;
            this.guna2Button2.CausesValidation = false;
            this.guna2Button2.DisabledState.BorderColor = System.Drawing.Color.LightGray;
            this.guna2Button2.DisabledState.CustomBorderColor = System.Drawing.Color.LightGray;
            this.guna2Button2.DisabledState.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Button2.DisabledState.ForeColor = System.Drawing.Color.LightGray;
            this.guna2Button2.Enabled = false;
            this.guna2Button2.FillColor = System.Drawing.Color.WhiteSmoke;
            this.guna2Button2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button2.ForeColor = System.Drawing.Color.White;
            this.guna2Button2.Location = new System.Drawing.Point(541, 128);
            this.guna2Button2.Name = "guna2Button2";
            this.guna2Button2.Size = new System.Drawing.Size(488, 249);
            this.guna2Button2.TabIndex = 165;
            this.guna2Button2.UseTransparentBackground = true;
            // 
            // btnAbrirListaPropietario
            // 
            this.btnAbrirListaPropietario.BackColor = System.Drawing.Color.Transparent;
            this.btnAbrirListaPropietario.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAbrirListaPropietario.BackgroundImage")));
            this.btnAbrirListaPropietario.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAbrirListaPropietario.BorderRadius = 14;
            this.btnAbrirListaPropietario.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnAbrirListaPropietario.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnAbrirListaPropietario.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnAbrirListaPropietario.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnAbrirListaPropietario.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnAbrirListaPropietario.FillColor = System.Drawing.Color.LightSkyBlue;
            this.btnAbrirListaPropietario.FillColor2 = System.Drawing.Color.RoyalBlue;
            this.btnAbrirListaPropietario.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnAbrirListaPropietario.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.btnAbrirListaPropietario.Image = global::AniCLinic.Properties.Resources._3003596;
            this.btnAbrirListaPropietario.ImageSize = new System.Drawing.Size(25, 25);
            this.btnAbrirListaPropietario.Location = new System.Drawing.Point(253, 61);
            this.btnAbrirListaPropietario.Name = "btnAbrirListaPropietario";
            this.btnAbrirListaPropietario.Size = new System.Drawing.Size(44, 37);
            this.btnAbrirListaPropietario.TabIndex = 169;
            this.btnAbrirListaPropietario.UseTransparentBackground = true;
            // 
            // Ventas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(1059, 614);
            this.Controls.Add(this.btnAbrirListaPropietario);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblTotalno);
            this.Controls.Add(this.lblIVA);
            this.Controls.Add(this.lblIVAno);
            this.Controls.Add(this.lblTtlVenta);
            this.Controls.Add(this.lblTtlVno);
            this.Controls.Add(this.cmbMetodoPago);
            this.Controls.Add(this.cmbCategoria);
            this.Controls.Add(this.cmbProducto);
            this.Controls.Add(this.guna2HtmlLabel12);
            this.Controls.Add(this.guna2Separator3);
            this.Controls.Add(this.guna2HtmlLabel11);
            this.Controls.Add(this.guna2Separator2);
            this.Controls.Add(this.guna2HtmlLabel9);
            this.Controls.Add(this.guna2Separator1);
            this.Controls.Add(this.guna2HtmlLabel6);
            this.Controls.Add(this.txtCorreo);
            this.Controls.Add(this.guna2HtmlLabel8);
            this.Controls.Add(this.guna2HtmlLabel4);
            this.Controls.Add(this.txtCelular);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.guna2HtmlLabel3);
            this.Controls.Add(this.txtCedula);
            this.Controls.Add(this.guna2HtmlLabel2);
            this.Controls.Add(this.btnFinalizar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.guna2HtmlLabel7);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.guna2HtmlLabel5);
            this.Controls.Add(this.guna2HtmlLabel1);
            this.Controls.Add(this.guna2HtmlLabel10);
            this.Controls.Add(this.txtCantidad);
            this.Controls.Add(this.dgvVentas);
            this.Controls.Add(this.guna2Button9);
            this.Controls.Add(this.guna2Button1);
            this.Controls.Add(this.guna2Button2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Ventas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ventas";
            this.Load += new System.EventHandler(this.Ventas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView dgvVentas;
        private Guna.UI2.WinForms.Guna2ComboBox cmbProducto;
        private Guna.UI2.WinForms.Guna2ComboBox cmbCategoria;
        private Guna.UI2.WinForms.Guna2TextBox txtCantidad;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel10;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2TextBox txtNombre;
        private Guna.UI2.WinForms.Guna2TextBox txtCelular;
        private Guna.UI2.WinForms.Guna2TextBox txtCedula;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel4;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel5;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel6;
        private Guna.UI2.WinForms.Guna2TextBox txtCorreo;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel7;
        private Guna.UI2.WinForms.Guna2TextBox txtPrecio;
        private Guna.UI2.WinForms.Guna2GradientButton btnAgregar;
        private Guna.UI2.WinForms.Guna2GradientButton btnEliminar;
        private Guna.UI2.WinForms.Guna2GradientButton btnFinalizar;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTtlVno;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTtlVenta;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblIVAno;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblIVA;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalno;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotal;
        private Guna.UI2.WinForms.Guna2GradientButton btnImprimir;
        private Guna.UI2.WinForms.Guna2ComboBox cmbMetodoPago;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel8;
        private Guna.UI2.WinForms.Guna2Button guna2Button9;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel9;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel11;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel12;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator3;
        private Guna.UI2.WinForms.Guna2Button guna2Button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Precio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Iva;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn Total;
        private Guna.UI2.WinForms.Guna2GradientButton btnAbrirListaPropietario;
    }
}