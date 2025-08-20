namespace AniCLinic
{
    partial class Menu
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
            this.components = new System.ComponentModel.Container();
            Guna.UI2.AnimatorNS.Animation animation2 = new Guna.UI2.AnimatorNS.Animation();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.PanelMenu = new Guna.UI2.WinForms.Guna2Panel();
            this.btnSalir = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2Separator2 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.BtnInventario = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnHistorial = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnCitas = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnPacientes = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnPanel = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.guna2Transition1 = new Guna.UI2.WinForms.Guna2Transition();
            this.pnlMenu1 = new System.Windows.Forms.Panel();
            this.btnMinimizarMenu = new Guna.UI2.WinForms.Guna2CircleButton();
            this.btnMaximizarMenu = new Guna.UI2.WinForms.Guna2CircleButton();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.PanelMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this;
            // 
            // PanelMenu
            // 
            this.PanelMenu.BackColor = System.Drawing.Color.CadetBlue;
            this.PanelMenu.Controls.Add(this.btnSalir);
            this.PanelMenu.Controls.Add(this.guna2Separator2);
            this.PanelMenu.Controls.Add(this.guna2HtmlLabel2);
            this.PanelMenu.Controls.Add(this.guna2Separator1);
            this.PanelMenu.Controls.Add(this.BtnInventario);
            this.PanelMenu.Controls.Add(this.btnHistorial);
            this.PanelMenu.Controls.Add(this.btnCitas);
            this.PanelMenu.Controls.Add(this.btnPacientes);
            this.PanelMenu.Controls.Add(this.btnPanel);
            this.PanelMenu.Controls.Add(this.guna2CirclePictureBox1);
            this.guna2Transition1.SetDecoration(this.PanelMenu, Guna.UI2.AnimatorNS.DecorationType.None);
            this.PanelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.PanelMenu.Location = new System.Drawing.Point(0, 0);
            this.PanelMenu.Name = "PanelMenu";
            this.PanelMenu.Size = new System.Drawing.Size(256, 663);
            this.PanelMenu.TabIndex = 0;
            this.PanelMenu.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMenu_Paint);
            // 
            // btnSalir
            // 
            this.btnSalir.CustomImages.Image = global::AniCLinic.Properties.Resources.door_open_fill;
            this.btnSalir.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnSalir.CustomImages.ImageSize = new System.Drawing.Size(30, 30);
            this.guna2Transition1.SetDecoration(this.btnSalir, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnSalir.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSalir.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSalir.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSalir.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSalir.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSalir.FillColor = System.Drawing.Color.Empty;
            this.btnSalir.FillColor2 = System.Drawing.Color.Empty;
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnSalir.ForeColor = System.Drawing.Color.Black;
            this.btnSalir.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnSalir.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btnSalir.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.Location = new System.Drawing.Point(0, 580);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(263, 56);
            this.btnSalir.TabIndex = 9;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // guna2Separator2
            // 
            this.guna2Transition1.SetDecoration(this.guna2Separator2, Guna.UI2.AnimatorNS.DecorationType.None);
            this.guna2Separator2.Location = new System.Drawing.Point(-23, 214);
            this.guna2Separator2.Name = "guna2Separator2";
            this.guna2Separator2.Size = new System.Drawing.Size(286, 10);
            this.guna2Separator2.TabIndex = 8;
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2Transition1.SetDecoration(this.guna2HtmlLabel2, Guna.UI2.AnimatorNS.DecorationType.None);
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Britannic Bold", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(90, 183);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(85, 25);
            this.guna2HtmlLabel2.TabIndex = 7;
            this.guna2HtmlLabel2.Text = "AniClinic";
            // 
            // guna2Separator1
            // 
            this.guna2Transition1.SetDecoration(this.guna2Separator1, Guna.UI2.AnimatorNS.DecorationType.None);
            this.guna2Separator1.Location = new System.Drawing.Point(3, 548);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(286, 10);
            this.guna2Separator1.TabIndex = 2;
            // 
            // BtnInventario
            // 
            this.BtnInventario.CustomImages.Image = global::AniCLinic.Properties.Resources.box_1_fill;
            this.BtnInventario.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.BtnInventario.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.BtnInventario, Guna.UI2.AnimatorNS.DecorationType.None);
            this.BtnInventario.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.BtnInventario.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.BtnInventario.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.BtnInventario.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.BtnInventario.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.BtnInventario.FillColor = System.Drawing.Color.Empty;
            this.BtnInventario.FillColor2 = System.Drawing.Color.Empty;
            this.BtnInventario.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.BtnInventario.ForeColor = System.Drawing.Color.Black;
            this.BtnInventario.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.BtnInventario.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.BtnInventario.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnInventario.Location = new System.Drawing.Point(0, 474);
            this.BtnInventario.Name = "BtnInventario";
            this.BtnInventario.Size = new System.Drawing.Size(263, 56);
            this.BtnInventario.TabIndex = 6;
            this.BtnInventario.Text = "Inventario";
            // 
            // btnHistorial
            // 
            this.btnHistorial.CustomImages.Image = global::AniCLinic.Properties.Resources.dossier_fill;
            this.btnHistorial.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnHistorial.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.btnHistorial, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnHistorial.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHistorial.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHistorial.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHistorial.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHistorial.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHistorial.FillColor = System.Drawing.Color.Empty;
            this.btnHistorial.FillColor2 = System.Drawing.Color.Empty;
            this.btnHistorial.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnHistorial.ForeColor = System.Drawing.Color.Black;
            this.btnHistorial.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnHistorial.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btnHistorial.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistorial.Location = new System.Drawing.Point(0, 416);
            this.btnHistorial.Name = "btnHistorial";
            this.btnHistorial.Size = new System.Drawing.Size(263, 56);
            this.btnHistorial.TabIndex = 5;
            this.btnHistorial.Text = "Historial Médico";
            // 
            // btnCitas
            // 
            this.btnCitas.CustomImages.Image = global::AniCLinic.Properties.Resources.alarm_fill;
            this.btnCitas.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCitas.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.btnCitas, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnCitas.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCitas.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCitas.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCitas.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCitas.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCitas.FillColor = System.Drawing.Color.Empty;
            this.btnCitas.FillColor2 = System.Drawing.Color.Empty;
            this.btnCitas.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnCitas.ForeColor = System.Drawing.Color.Black;
            this.btnCitas.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnCitas.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btnCitas.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCitas.Location = new System.Drawing.Point(0, 358);
            this.btnCitas.Name = "btnCitas";
            this.btnCitas.Size = new System.Drawing.Size(263, 56);
            this.btnCitas.TabIndex = 4;
            this.btnCitas.Text = "Citas";
            this.btnCitas.Click += new System.EventHandler(this.btnCitas_Click);
            // 
            // btnPacientes
            // 
            this.btnPacientes.CustomImages.Image = global::AniCLinic.Properties.Resources._1084899;
            this.btnPacientes.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPacientes.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.btnPacientes, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnPacientes.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPacientes.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPacientes.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPacientes.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPacientes.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPacientes.FillColor = System.Drawing.Color.Empty;
            this.btnPacientes.FillColor2 = System.Drawing.Color.Empty;
            this.btnPacientes.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnPacientes.ForeColor = System.Drawing.Color.Black;
            this.btnPacientes.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnPacientes.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btnPacientes.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPacientes.Location = new System.Drawing.Point(0, 300);
            this.btnPacientes.Name = "btnPacientes";
            this.btnPacientes.Size = new System.Drawing.Size(263, 56);
            this.btnPacientes.TabIndex = 3;
            this.btnPacientes.Text = "Pacientes";
            this.btnPacientes.Click += new System.EventHandler(this.btnPacientes_Click);
            // 
            // btnPanel
            // 
            this.btnPanel.BorderColor = System.Drawing.Color.Transparent;
            this.btnPanel.CustomImages.Image = global::AniCLinic.Properties.Resources.home_heart_fill;
            this.btnPanel.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPanel.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.btnPanel, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnPanel.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPanel.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPanel.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPanel.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPanel.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPanel.FillColor = System.Drawing.Color.Empty;
            this.btnPanel.FillColor2 = System.Drawing.Color.Empty;
            this.btnPanel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnPanel.ForeColor = System.Drawing.Color.Black;
            this.btnPanel.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnPanel.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btnPanel.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPanel.ImageSize = new System.Drawing.Size(10, 10);
            this.btnPanel.Location = new System.Drawing.Point(0, 242);
            this.btnPanel.Name = "btnPanel";
            this.btnPanel.Size = new System.Drawing.Size(263, 56);
            this.btnPanel.TabIndex = 2;
            this.btnPanel.Text = "Panel";
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 20;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2Transition1
            // 
            this.guna2Transition1.AnimationType = Guna.UI2.AnimatorNS.AnimationType.HorizSlide;
            this.guna2Transition1.Cursor = null;
            animation2.AnimateOnlyDifferences = true;
            animation2.BlindCoeff = ((System.Drawing.PointF)(resources.GetObject("animation2.BlindCoeff")));
            animation2.LeafCoeff = 0F;
            animation2.MaxTime = 1F;
            animation2.MinTime = 0F;
            animation2.MosaicCoeff = ((System.Drawing.PointF)(resources.GetObject("animation2.MosaicCoeff")));
            animation2.MosaicShift = ((System.Drawing.PointF)(resources.GetObject("animation2.MosaicShift")));
            animation2.MosaicSize = 0;
            animation2.Padding = new System.Windows.Forms.Padding(0);
            animation2.RotateCoeff = 0F;
            animation2.RotateLimit = 0F;
            animation2.ScaleCoeff = ((System.Drawing.PointF)(resources.GetObject("animation2.ScaleCoeff")));
            animation2.SlideCoeff = ((System.Drawing.PointF)(resources.GetObject("animation2.SlideCoeff")));
            animation2.TimeCoeff = 0F;
            animation2.TransparencyCoeff = 0F;
            this.guna2Transition1.DefaultAnimation = animation2;
            // 
            // pnlMenu1
            // 
            this.pnlMenu1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.guna2Transition1.SetDecoration(this.pnlMenu1, Guna.UI2.AnimatorNS.DecorationType.None);
            this.pnlMenu1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMenu1.Location = new System.Drawing.Point(256, 0);
            this.pnlMenu1.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMenu1.Name = "pnlMenu1";
            this.pnlMenu1.Size = new System.Drawing.Size(803, 663);
            this.pnlMenu1.TabIndex = 12;
            this.pnlMenu1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnMinimizarMenu
            // 
            this.btnMinimizarMenu.Animated = true;
            this.btnMinimizarMenu.BackColor = System.Drawing.Color.Transparent;
            this.guna2Transition1.SetDecoration(this.btnMinimizarMenu, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnMinimizarMenu.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnMinimizarMenu.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnMinimizarMenu.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnMinimizarMenu.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnMinimizarMenu.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnMinimizarMenu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnMinimizarMenu.ForeColor = System.Drawing.Color.White;
            this.btnMinimizarMenu.Image = global::AniCLinic.Properties.Resources.arrow_left_s_line;
            this.btnMinimizarMenu.IndicateFocus = true;
            this.btnMinimizarMenu.Location = new System.Drawing.Point(233, 533);
            this.btnMinimizarMenu.Name = "btnMinimizarMenu";
            this.btnMinimizarMenu.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.btnMinimizarMenu.Size = new System.Drawing.Size(41, 41);
            this.btnMinimizarMenu.TabIndex = 11;
            this.btnMinimizarMenu.UseTransparentBackground = true;
            this.btnMinimizarMenu.Click += new System.EventHandler(this.btnMinimizarMenu_Click);
            // 
            // btnMaximizarMenu
            // 
            this.btnMaximizarMenu.Animated = true;
            this.btnMaximizarMenu.BackColor = System.Drawing.Color.Transparent;
            this.guna2Transition1.SetDecoration(this.btnMaximizarMenu, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnMaximizarMenu.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnMaximizarMenu.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnMaximizarMenu.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnMaximizarMenu.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnMaximizarMenu.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnMaximizarMenu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnMaximizarMenu.ForeColor = System.Drawing.Color.White;
            this.btnMaximizarMenu.Image = global::AniCLinic.Properties.Resources.arrow_right_s_line;
            this.btnMaximizarMenu.IndicateFocus = true;
            this.btnMaximizarMenu.Location = new System.Drawing.Point(26, 533);
            this.btnMaximizarMenu.Name = "btnMaximizarMenu";
            this.btnMaximizarMenu.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.btnMaximizarMenu.Size = new System.Drawing.Size(41, 41);
            this.btnMaximizarMenu.TabIndex = 10;
            this.btnMaximizarMenu.UseTransparentBackground = true;
            this.btnMaximizarMenu.Visible = false;
            this.btnMaximizarMenu.Click += new System.EventHandler(this.btnMaximizarMenu_Click);
            // 
            // guna2CirclePictureBox1
            // 
            this.guna2CirclePictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Transition1.SetDecoration(this.guna2CirclePictureBox1, Guna.UI2.AnimatorNS.DecorationType.None);
            this.guna2CirclePictureBox1.FillColor = System.Drawing.Color.LightSeaGreen;
            this.guna2CirclePictureBox1.Image = global::AniCLinic.Properties.Resources.Imagen_de_WhatsApp_2025_08_07_a_las_00_191;
            this.guna2CirclePictureBox1.ImageRotate = 0F;
            this.guna2CirclePictureBox1.Location = new System.Drawing.Point(65, 36);
            this.guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            this.guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox1.Size = new System.Drawing.Size(141, 132);
            this.guna2CirclePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2CirclePictureBox1.TabIndex = 2;
            this.guna2CirclePictureBox1.TabStop = false;
            this.guna2CirclePictureBox1.UseTransparentBackground = true;
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(1059, 663);
            this.Controls.Add(this.pnlMenu1);
            this.Controls.Add(this.btnMinimizarMenu);
            this.Controls.Add(this.btnMaximizarMenu);
            this.Controls.Add(this.PanelMenu);
            this.guna2Transition1.SetDecoration(this, Guna.UI2.AnimatorNS.DecorationType.None);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu";
            this.Load += new System.EventHandler(this.Menu_Load);
            this.PanelMenu.ResumeLayout(false);
            this.PanelMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Panel PanelMenu;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientButton btnPanel;
        private Guna.UI2.WinForms.Guna2GradientButton BtnInventario;
        private Guna.UI2.WinForms.Guna2GradientButton btnHistorial;
        private Guna.UI2.WinForms.Guna2GradientButton btnCitas;
        private Guna.UI2.WinForms.Guna2GradientButton btnPacientes;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator2;
        private Guna.UI2.WinForms.Guna2GradientButton btnSalir;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2Transition guna2Transition1;
        private Guna.UI2.WinForms.Guna2CircleButton btnMaximizarMenu;
        private Guna.UI2.WinForms.Guna2CircleButton btnMinimizarMenu;
        private System.Windows.Forms.Panel pnlMenu1;
    }
}