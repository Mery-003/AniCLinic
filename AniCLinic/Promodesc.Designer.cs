namespace AniCLinic
{
    partial class Promodesc
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
            this.dgvPromo = new System.Windows.Forms.DataGridView();
            this.btnactivar = new System.Windows.Forms.Button();
            this.btndesactivar = new System.Windows.Forms.Button();
            this.dtmFecha = new System.Windows.Forms.DateTimePicker();
            this.btneditar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Btcrear = new System.Windows.Forms.Button();
            this.lbCategoria = new System.Windows.Forms.Label();
            this.lbtipdesc = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbdescripcion = new System.Windows.Forms.Label();
            this.tbDescripcion = new System.Windows.Forms.TextBox();
            this.lbNombres = new System.Windows.Forms.Label();
            this.tbNombres = new System.Windows.Forms.TextBox();
            this.tbvalor = new System.Windows.Forms.TextBox();
            this.cbTipo = new System.Windows.Forms.ComboBox();
            this.lbProductos = new System.Windows.Forms.Label();
            this.btEliminar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lbvalor = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Dtfechafin = new System.Windows.Forms.DateTimePicker();
            this.clbProductos = new System.Windows.Forms.CheckedListBox();
            this.clbCategorias = new System.Windows.Forms.CheckedListBox();
            this.ckbVieneCita = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPromo)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPromo
            // 
            this.dgvPromo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPromo.Location = new System.Drawing.Point(14, 326);
            this.dgvPromo.Margin = new System.Windows.Forms.Padding(2);
            this.dgvPromo.Name = "dgvPromo";
            this.dgvPromo.RowHeadersWidth = 51;
            this.dgvPromo.RowTemplate.Height = 24;
            this.dgvPromo.Size = new System.Drawing.Size(1054, 180);
            this.dgvPromo.TabIndex = 0;
            this.dgvPromo.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPromo_CellClick);
            // 
            // btnactivar
            // 
            this.btnactivar.Location = new System.Drawing.Point(870, 181);
            this.btnactivar.Margin = new System.Windows.Forms.Padding(2);
            this.btnactivar.Name = "btnactivar";
            this.btnactivar.Size = new System.Drawing.Size(97, 35);
            this.btnactivar.TabIndex = 1;
            this.btnactivar.Text = "ACTIVAR";
            this.btnactivar.UseVisualStyleBackColor = true;
            this.btnactivar.Click += new System.EventHandler(this.btnactivar_Click);
            // 
            // btndesactivar
            // 
            this.btndesactivar.Location = new System.Drawing.Point(971, 181);
            this.btndesactivar.Margin = new System.Windows.Forms.Padding(2);
            this.btndesactivar.Name = "btndesactivar";
            this.btndesactivar.Size = new System.Drawing.Size(97, 35);
            this.btndesactivar.TabIndex = 2;
            this.btndesactivar.Text = "DESACTIVAR";
            this.btndesactivar.UseVisualStyleBackColor = true;
            this.btndesactivar.Click += new System.EventHandler(this.btdesactivar_Click);
            // 
            // dtmFecha
            // 
            this.dtmFecha.Location = new System.Drawing.Point(95, 282);
            this.dtmFecha.Margin = new System.Windows.Forms.Padding(2);
            this.dtmFecha.Name = "dtmFecha";
            this.dtmFecha.Size = new System.Drawing.Size(195, 20);
            this.dtmFecha.TabIndex = 3;
            // 
            // btneditar
            // 
            this.btneditar.Location = new System.Drawing.Point(769, 181);
            this.btneditar.Margin = new System.Windows.Forms.Padding(2);
            this.btneditar.Name = "btneditar";
            this.btneditar.Size = new System.Drawing.Size(97, 35);
            this.btneditar.TabIndex = 4;
            this.btneditar.Text = "EDITAR";
            this.btneditar.UseVisualStyleBackColor = true;
            this.btneditar.Click += new System.EventHandler(this.btneditar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 28.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 46);
            this.label1.TabIndex = 5;
            this.label1.Text = "Promociones";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 259);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "FECHAS";
            // 
            // Btcrear
            // 
            this.Btcrear.Location = new System.Drawing.Point(565, 181);
            this.Btcrear.Margin = new System.Windows.Forms.Padding(2);
            this.Btcrear.Name = "Btcrear";
            this.Btcrear.Size = new System.Drawing.Size(97, 35);
            this.Btcrear.TabIndex = 8;
            this.Btcrear.Text = "CREAR";
            this.Btcrear.UseVisualStyleBackColor = true;
            this.Btcrear.Click += new System.EventHandler(this.Btcrear_Click);
            // 
            // lbCategoria
            // 
            this.lbCategoria.AutoSize = true;
            this.lbCategoria.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCategoria.Location = new System.Drawing.Point(27, 71);
            this.lbCategoria.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbCategoria.Name = "lbCategoria";
            this.lbCategoria.Size = new System.Drawing.Size(107, 18);
            this.lbCategoria.TabIndex = 9;
            this.lbCategoria.Text = "CATEGORIAS";
            // 
            // lbtipdesc
            // 
            this.lbtipdesc.AutoSize = true;
            this.lbtipdesc.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbtipdesc.Location = new System.Drawing.Point(401, 73);
            this.lbtipdesc.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbtipdesc.Name = "lbtipdesc";
            this.lbtipdesc.Size = new System.Drawing.Size(90, 18);
            this.lbtipdesc.TabIndex = 12;
            this.lbtipdesc.Text = "TIPO DESC";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(166, 247);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 18);
            this.label6.TabIndex = 15;
            this.label6.Text = "INICIO";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(397, 247);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 18);
            this.label7.TabIndex = 16;
            this.label7.Text = "FIN";
            // 
            // lbdescripcion
            // 
            this.lbdescripcion.AutoSize = true;
            this.lbdescripcion.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbdescripcion.Location = new System.Drawing.Point(903, 73);
            this.lbdescripcion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbdescripcion.Name = "lbdescripcion";
            this.lbdescripcion.Size = new System.Drawing.Size(117, 18);
            this.lbdescripcion.TabIndex = 17;
            this.lbdescripcion.Text = "DESCRIPCION";
            // 
            // tbDescripcion
            // 
            this.tbDescripcion.Location = new System.Drawing.Point(879, 103);
            this.tbDescripcion.Margin = new System.Windows.Forms.Padding(2);
            this.tbDescripcion.Name = "tbDescripcion";
            this.tbDescripcion.Size = new System.Drawing.Size(173, 20);
            this.tbDescripcion.TabIndex = 18;
            // 
            // lbNombres
            // 
            this.lbNombres.AutoSize = true;
            this.lbNombres.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNombres.Location = new System.Drawing.Point(651, 73);
            this.lbNombres.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbNombres.Name = "lbNombres";
            this.lbNombres.Size = new System.Drawing.Size(188, 18);
            this.lbNombres.TabIndex = 19;
            this.lbNombres.Text = "NOMBRE DE LA PROMO ";
            // 
            // tbNombres
            // 
            this.tbNombres.Location = new System.Drawing.Point(642, 103);
            this.tbNombres.Margin = new System.Windows.Forms.Padding(2);
            this.tbNombres.Name = "tbNombres";
            this.tbNombres.Size = new System.Drawing.Size(204, 20);
            this.tbNombres.TabIndex = 20;
            // 
            // tbvalor
            // 
            this.tbvalor.Location = new System.Drawing.Point(536, 103);
            this.tbvalor.Margin = new System.Windows.Forms.Padding(2);
            this.tbvalor.Name = "tbvalor";
            this.tbvalor.Size = new System.Drawing.Size(55, 20);
            this.tbvalor.TabIndex = 21;
            // 
            // cbTipo
            // 
            this.cbTipo.FormattingEnabled = true;
            this.cbTipo.Location = new System.Drawing.Point(399, 102);
            this.cbTipo.Margin = new System.Windows.Forms.Padding(2);
            this.cbTipo.Name = "cbTipo";
            this.cbTipo.Size = new System.Drawing.Size(91, 21);
            this.cbTipo.TabIndex = 22;
            // 
            // lbProductos
            // 
            this.lbProductos.AutoSize = true;
            this.lbProductos.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProductos.Location = new System.Drawing.Point(208, 71);
            this.lbProductos.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbProductos.Name = "lbProductos";
            this.lbProductos.Size = new System.Drawing.Size(104, 18);
            this.lbProductos.TabIndex = 23;
            this.lbProductos.Text = "PRODUCTOS";
            // 
            // btEliminar
            // 
            this.btEliminar.Location = new System.Drawing.Point(668, 181);
            this.btEliminar.Margin = new System.Windows.Forms.Padding(2);
            this.btEliminar.Name = "btEliminar";
            this.btEliminar.Size = new System.Drawing.Size(97, 35);
            this.btEliminar.TabIndex = 24;
            this.btEliminar.Text = "ELIMINAR";
            this.btEliminar.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(397, 73);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 18);
            this.label3.TabIndex = 26;
            // 
            // lbvalor
            // 
            this.lbvalor.AutoSize = true;
            this.lbvalor.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbvalor.Location = new System.Drawing.Point(533, 71);
            this.lbvalor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbvalor.Name = "lbvalor";
            this.lbvalor.Size = new System.Drawing.Size(64, 18);
            this.lbvalor.TabIndex = 27;
            this.lbvalor.Text = "VALOR ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(595, 103);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 18);
            this.label5.TabIndex = 28;
            this.label5.Text = "%";
            // 
            // Dtfechafin
            // 
            this.Dtfechafin.Location = new System.Drawing.Point(321, 282);
            this.Dtfechafin.Margin = new System.Windows.Forms.Padding(2);
            this.Dtfechafin.Name = "Dtfechafin";
            this.Dtfechafin.Size = new System.Drawing.Size(194, 20);
            this.Dtfechafin.TabIndex = 10;
            // 
            // clbProductos
            // 
            this.clbProductos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clbProductos.FormattingEnabled = true;
            this.clbProductos.Location = new System.Drawing.Point(199, 101);
            this.clbProductos.Name = "clbProductos";
            this.clbProductos.Size = new System.Drawing.Size(146, 21);
            this.clbProductos.TabIndex = 44;
            // 
            // clbCategorias
            // 
            this.clbCategorias.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clbCategorias.FormattingEnabled = true;
            this.clbCategorias.Location = new System.Drawing.Point(14, 104);
            this.clbCategorias.Name = "clbCategorias";
            this.clbCategorias.Size = new System.Drawing.Size(133, 89);
            this.clbCategorias.TabIndex = 45;
            // 
            // ckbVieneCita
            // 
            this.ckbVieneCita.AutoSize = true;
            this.ckbVieneCita.Location = new System.Drawing.Point(14, 199);
            this.ckbVieneCita.Name = "ckbVieneCita";
            this.ckbVieneCita.Size = new System.Drawing.Size(152, 17);
            this.ckbVieneCita.TabIndex = 46;
            this.ckbVieneCita.Text = "Promocion junto a una cita";
            this.ckbVieneCita.UseVisualStyleBackColor = true;
            // 
            // Promodesc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 526);
            this.Controls.Add(this.ckbVieneCita);
            this.Controls.Add(this.clbCategorias);
            this.Controls.Add(this.clbProductos);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbvalor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btEliminar);
            this.Controls.Add(this.lbProductos);
            this.Controls.Add(this.cbTipo);
            this.Controls.Add(this.tbvalor);
            this.Controls.Add(this.tbNombres);
            this.Controls.Add(this.lbNombres);
            this.Controls.Add(this.tbDescripcion);
            this.Controls.Add(this.lbdescripcion);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbtipdesc);
            this.Controls.Add(this.Dtfechafin);
            this.Controls.Add(this.lbCategoria);
            this.Controls.Add(this.Btcrear);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btneditar);
            this.Controls.Add(this.dtmFecha);
            this.Controls.Add(this.btndesactivar);
            this.Controls.Add(this.btnactivar);
            this.Controls.Add(this.dgvPromo);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Promodesc";
            this.Text = "Promociones";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPromo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPromo;
        private System.Windows.Forms.Button btnactivar;
        private System.Windows.Forms.Button btndesactivar;
        private System.Windows.Forms.DateTimePicker dtmFecha;
        private System.Windows.Forms.Button btneditar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Btcrear;
        private System.Windows.Forms.Label lbCategoria;
        private System.Windows.Forms.Label lbtipdesc;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbdescripcion;
        private System.Windows.Forms.TextBox tbDescripcion;
        private System.Windows.Forms.Label lbNombres;
        private System.Windows.Forms.TextBox tbNombres;
        private System.Windows.Forms.TextBox tbvalor;
        private System.Windows.Forms.ComboBox cbTipo;
        private System.Windows.Forms.Label lbProductos;
        private System.Windows.Forms.Button btEliminar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbvalor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker Dtfechafin;
        private System.Windows.Forms.CheckedListBox clbProductos;
        private System.Windows.Forms.CheckedListBox clbCategorias;
        private System.Windows.Forms.CheckBox ckbVieneCita;
    }
}