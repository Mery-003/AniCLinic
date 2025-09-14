namespace AniCLinic
{
    partial class frReportes
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
            this.btnMes = new Guna.UI2.WinForms.Guna2Button();
            this.btnProducto = new Guna.UI2.WinForms.Guna2Button();
            this.btnListaProductos = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Separator2 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.lblReportes = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtAño = new Guna.UI2.WinForms.Guna2TextBox();
            this.SuspendLayout();
            // 
            // btnMes
            // 
            this.btnMes.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnMes.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnMes.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnMes.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnMes.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnMes.ForeColor = System.Drawing.Color.White;
            this.btnMes.Location = new System.Drawing.Point(75, 221);
            this.btnMes.Name = "btnMes";
            this.btnMes.Size = new System.Drawing.Size(212, 130);
            this.btnMes.TabIndex = 0;
            this.btnMes.Text = "POR MES";
            this.btnMes.Click += new System.EventHandler(this.btnAño_Click);
            // 
            // btnProducto
            // 
            this.btnProducto.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnProducto.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnProducto.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnProducto.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnProducto.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnProducto.ForeColor = System.Drawing.Color.White;
            this.btnProducto.Location = new System.Drawing.Point(408, 221);
            this.btnProducto.Name = "btnProducto";
            this.btnProducto.Size = new System.Drawing.Size(212, 130);
            this.btnProducto.TabIndex = 1;
            this.btnProducto.Text = "POR PRODUCTO";
            this.btnProducto.Click += new System.EventHandler(this.btnProducto_Click);
            // 
            // btnListaProductos
            // 
            this.btnListaProductos.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnListaProductos.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnListaProductos.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnListaProductos.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnListaProductos.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnListaProductos.ForeColor = System.Drawing.Color.White;
            this.btnListaProductos.Location = new System.Drawing.Point(750, 221);
            this.btnListaProductos.Name = "btnListaProductos";
            this.btnListaProductos.Size = new System.Drawing.Size(212, 130);
            this.btnListaProductos.TabIndex = 2;
            this.btnListaProductos.Text = "LISTA DE PRODUCTOS";
            this.btnListaProductos.Click += new System.EventHandler(this.btnListaProductos_Click);
            // 
            // guna2Separator2
            // 
            this.guna2Separator2.FillColor = System.Drawing.Color.DarkSlateGray;
            this.guna2Separator2.FillThickness = 2;
            this.guna2Separator2.Location = new System.Drawing.Point(-10, 39);
            this.guna2Separator2.Name = "guna2Separator2";
            this.guna2Separator2.Size = new System.Drawing.Size(1068, 10);
            this.guna2Separator2.TabIndex = 115;
            // 
            // guna2Separator1
            // 
            this.guna2Separator1.FillColor = System.Drawing.Color.DarkSlateGray;
            this.guna2Separator1.FillThickness = 2;
            this.guna2Separator1.Location = new System.Drawing.Point(-10, 97);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(1068, 10);
            this.guna2Separator1.TabIndex = 114;
            // 
            // lblReportes
            // 
            this.lblReportes.BackColor = System.Drawing.Color.Transparent;
            this.lblReportes.Font = new System.Drawing.Font("Cooper Black", 21.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReportes.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblReportes.Location = new System.Drawing.Point(451, 55);
            this.lblReportes.Name = "lblReportes";
            this.lblReportes.Size = new System.Drawing.Size(135, 36);
            this.lblReportes.TabIndex = 113;
            this.lblReportes.Text = "Reportes";
            // 
            // guna2HtmlLabel3
            // 
            this.guna2HtmlLabel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.guna2HtmlLabel3.Location = new System.Drawing.Point(434, 125);
            this.guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            this.guna2HtmlLabel3.Size = new System.Drawing.Size(162, 19);
            this.guna2HtmlLabel3.TabIndex = 142;
            this.guna2HtmlLabel3.Text = "Ingrese el año del reporte";
            // 
            // txtAño
            // 
            this.txtAño.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtAño.BorderColor = System.Drawing.Color.Silver;
            this.txtAño.BorderRadius = 20;
            this.txtAño.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAño.DefaultText = "";
            this.txtAño.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtAño.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtAño.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtAño.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtAño.FillColor = System.Drawing.Color.LightGray;
            this.txtAño.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtAño.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAño.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtAño.Location = new System.Drawing.Point(403, 150);
            this.txtAño.Name = "txtAño";
            this.txtAño.PlaceholderText = "";
            this.txtAño.SelectedText = "";
            this.txtAño.Size = new System.Drawing.Size(227, 36);
            this.txtAño.TabIndex = 141;
            this.txtAño.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAño_KeyPress);
            // 
            // frReportes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(1024, 522);
            this.Controls.Add(this.guna2HtmlLabel3);
            this.Controls.Add(this.txtAño);
            this.Controls.Add(this.guna2Separator2);
            this.Controls.Add(this.guna2Separator1);
            this.Controls.Add(this.lblReportes);
            this.Controls.Add(this.btnListaProductos);
            this.Controls.Add(this.btnProducto);
            this.Controls.Add(this.btnMes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frReportes";
            this.Text = "frReportes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btnMes;
        private Guna.UI2.WinForms.Guna2Button btnProducto;
        private Guna.UI2.WinForms.Guna2Button btnListaProductos;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator2;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblReportes;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
        private Guna.UI2.WinForms.Guna2TextBox txtAño;
    }
}