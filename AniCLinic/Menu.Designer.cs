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
            Guna.UI2.AnimatorNS.Animation animation7 = new Guna.UI2.AnimatorNS.Animation();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.PanelMenu = new Guna.UI2.WinForms.Guna2Panel();
            this.panelReporteriaSubmenu = new Guna.UI2.WinForms.Guna2Panel();
            this.btncarnet = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnhisto = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnSalir = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2Separator2 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.btnReporteria = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnRegistro = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnCitas = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnPacientes = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnPanel = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.guna2Transition1 = new Guna.UI2.WinForms.Guna2Transition();
            this.pnlMenu1 = new System.Windows.Forms.Panel();
            this.btnMinimizarMenu = new Guna.UI2.WinForms.Guna2CircleButton();
            this.btnMaximizarMenu = new Guna.UI2.WinForms.Guna2CircleButton();
            this.PanelMenu.SuspendLayout();
            this.panelReporteriaSubmenu.SuspendLayout();
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
            this.PanelMenu.Controls.Add(this.panelReporteriaSubmenu);
            this.PanelMenu.Controls.Add(this.btnSalir);
            this.PanelMenu.Controls.Add(this.guna2Separator2);
            this.PanelMenu.Controls.Add(this.guna2HtmlLabel2);
            this.PanelMenu.Controls.Add(this.guna2Separator1);
            this.PanelMenu.Controls.Add(this.btnReporteria);
            this.PanelMenu.Controls.Add(this.btnRegistro);
            this.PanelMenu.Controls.Add(this.btnCitas);
            this.PanelMenu.Controls.Add(this.btnPacientes);
            this.PanelMenu.Controls.Add(this.btnPanel);
            this.PanelMenu.Controls.Add(this.guna2CirclePictureBox1);
            this.guna2Transition1.SetDecoration(this.PanelMenu, Guna.UI2.AnimatorNS.DecorationType.None);
            this.PanelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.PanelMenu.Location = new System.Drawing.Point(0, 0);
            this.PanelMenu.Name = "PanelMenu";
            this.PanelMenu.Size = new System.Drawing.Size(256, 693);
            this.PanelMenu.TabIndex = 0;
            this.PanelMenu.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMenu_Paint);
            // 
            // panelReporteriaSubmenu
            // 
            this.panelReporteriaSubmenu.Controls.Add(this.btncarnet);
            this.panelReporteriaSubmenu.Controls.Add(this.btnhisto);
            this.guna2Transition1.SetDecoration(this.panelReporteriaSubmenu, Guna.UI2.AnimatorNS.DecorationType.None);
            this.panelReporteriaSubmenu.Location = new System.Drawing.Point(-3, 518);
            this.panelReporteriaSubmenu.Name = "panelReporteriaSubmenu";
            this.panelReporteriaSubmenu.Size = new System.Drawing.Size(263, 97);
            this.panelReporteriaSubmenu.TabIndex = 14;
            this.panelReporteriaSubmenu.Visible = false;
            // 
            // btncarnet
            // 
            this.btncarnet.BackColor = System.Drawing.Color.Teal;
            this.btncarnet.CustomImages.Image = global::AniCLinic.Properties.Resources.info_card_fill;
            this.btncarnet.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btncarnet.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.btncarnet, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btncarnet.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btncarnet.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btncarnet.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btncarnet.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btncarnet.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btncarnet.FillColor = System.Drawing.Color.Empty;
            this.btncarnet.FillColor2 = System.Drawing.Color.Empty;
            this.btncarnet.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btncarnet.ForeColor = System.Drawing.Color.Black;
            this.btncarnet.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btncarnet.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btncarnet.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncarnet.Location = new System.Drawing.Point(0, 49);
            this.btncarnet.Name = "btncarnet";
            this.btncarnet.Size = new System.Drawing.Size(263, 39);
            this.btncarnet.TabIndex = 11;
            this.btncarnet.Text = "Carnet";
            this.btncarnet.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btncarnet.TextOffset = new System.Drawing.Point(40, 0);
            this.btncarnet.Click += new System.EventHandler(this.btncarnet_Click_1);
            // 
            // btnhisto
            // 
            this.btnhisto.BackColor = System.Drawing.Color.Teal;
            this.btnhisto.CustomImages.Image = global::AniCLinic.Properties.Resources.article_fill;
            this.btnhisto.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnhisto.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.btnhisto, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnhisto.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnhisto.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnhisto.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnhisto.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnhisto.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnhisto.FillColor = System.Drawing.Color.Empty;
            this.btnhisto.FillColor2 = System.Drawing.Color.Empty;
            this.btnhisto.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnhisto.ForeColor = System.Drawing.Color.Black;
            this.btnhisto.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnhisto.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btnhisto.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnhisto.Location = new System.Drawing.Point(0, 3);
            this.btnhisto.Name = "btnhisto";
            this.btnhisto.Size = new System.Drawing.Size(263, 40);
            this.btnhisto.TabIndex = 10;
            this.btnhisto.Text = "Historial Veterinario";
            this.btnhisto.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnhisto.TextOffset = new System.Drawing.Point(40, 0);
            this.btnhisto.Click += new System.EventHandler(this.btnhisto_Click_1);
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
            this.btnSalir.Location = new System.Drawing.Point(-3, 612);
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
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(90, 183);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(87, 27);
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
            // btnReporteria
            // 
            this.btnReporteria.CustomImages.Image = global::AniCLinic.Properties.Resources.file_3_fill;
            this.btnReporteria.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnReporteria.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.btnReporteria, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnReporteria.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnReporteria.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnReporteria.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReporteria.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReporteria.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnReporteria.FillColor = System.Drawing.Color.Empty;
            this.btnReporteria.FillColor2 = System.Drawing.Color.Empty;
            this.btnReporteria.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnReporteria.ForeColor = System.Drawing.Color.Black;
            this.btnReporteria.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnReporteria.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btnReporteria.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReporteria.Location = new System.Drawing.Point(0, 471);
            this.btnReporteria.Name = "btnReporteria";
            this.btnReporteria.Size = new System.Drawing.Size(263, 56);
            this.btnReporteria.TabIndex = 6;
            this.btnReporteria.Text = "Reportería";
            this.btnReporteria.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnReporteria.TextOffset = new System.Drawing.Point(40, 0);
            this.btnReporteria.Click += new System.EventHandler(this.btnReporteria_Click);
            // 
            // btnRegistro
            // 
            this.btnRegistro.CustomImages.Image = global::AniCLinic.Properties.Resources.dossier_fill;
            this.btnRegistro.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnRegistro.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Transition1.SetDecoration(this.btnRegistro, Guna.UI2.AnimatorNS.DecorationType.None);
            this.btnRegistro.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRegistro.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRegistro.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRegistro.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRegistro.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnRegistro.FillColor = System.Drawing.Color.Empty;
            this.btnRegistro.FillColor2 = System.Drawing.Color.Empty;
            this.btnRegistro.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnRegistro.ForeColor = System.Drawing.Color.Black;
            this.btnRegistro.HoverState.FillColor = System.Drawing.Color.DarkSlateGray;
            this.btnRegistro.HoverState.FillColor2 = System.Drawing.Color.CadetBlue;
            this.btnRegistro.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistro.Location = new System.Drawing.Point(0, 416);
            this.btnRegistro.Name = "btnRegistro";
            this.btnRegistro.Size = new System.Drawing.Size(263, 56);
            this.btnRegistro.TabIndex = 5;
            this.btnRegistro.Text = "Registro Clínico";
            this.btnRegistro.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnRegistro.TextOffset = new System.Drawing.Point(40, 0);
            this.btnRegistro.Click += new System.EventHandler(this.btnHistorial_Click);
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
            this.btnCitas.Text = "Generar Citas";
            this.btnCitas.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCitas.TextOffset = new System.Drawing.Point(40, 0);
            this.btnCitas.Click += new System.EventHandler(this.btnCitas_Click);
            // 
            // btnPacientes
            // 
            this.btnPacientes.CustomImages.Image = global::AniCLinic.Properties.Resources.shake_hands_fill;
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
            this.btnPacientes.Text = "Gestión de Cuentas";
            this.btnPacientes.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPacientes.TextOffset = new System.Drawing.Point(40, 0);
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
            this.btnPanel.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPanel.TextOffset = new System.Drawing.Point(40, 0);
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
            animation7.AnimateOnlyDifferences = true;
            animation7.BlindCoeff = ((System.Drawing.PointF)(resources.GetObject("animation7.BlindCoeff")));
            animation7.LeafCoeff = 0F;
            animation7.MaxTime = 1F;
            animation7.MinTime = 0F;
            animation7.MosaicCoeff = ((System.Drawing.PointF)(resources.GetObject("animation7.MosaicCoeff")));
            animation7.MosaicShift = ((System.Drawing.PointF)(resources.GetObject("animation7.MosaicShift")));
            animation7.MosaicSize = 0;
            animation7.Padding = new System.Windows.Forms.Padding(0);
            animation7.RotateCoeff = 0F;
            animation7.RotateLimit = 0F;
            animation7.ScaleCoeff = ((System.Drawing.PointF)(resources.GetObject("animation7.ScaleCoeff")));
            animation7.SlideCoeff = ((System.Drawing.PointF)(resources.GetObject("animation7.SlideCoeff")));
            animation7.TimeCoeff = 0F;
            animation7.TransparencyCoeff = 0F;
            this.guna2Transition1.DefaultAnimation = animation7;
            // 
            // pnlMenu1
            // 
            this.pnlMenu1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.guna2Transition1.SetDecoration(this.pnlMenu1, Guna.UI2.AnimatorNS.DecorationType.None);
            this.pnlMenu1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMenu1.Location = new System.Drawing.Point(256, 0);
            this.pnlMenu1.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMenu1.Name = "pnlMenu1";
            this.pnlMenu1.Size = new System.Drawing.Size(1059, 693);
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
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(1315, 693);
            this.Controls.Add(this.pnlMenu1);
            this.Controls.Add(this.PanelMenu);
            this.Controls.Add(this.btnMaximizarMenu);
            this.Controls.Add(this.btnMinimizarMenu);
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
            this.panelReporteriaSubmenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Panel PanelMenu;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientButton btnPanel;
        private Guna.UI2.WinForms.Guna2GradientButton btnReporteria;
        private Guna.UI2.WinForms.Guna2GradientButton btnRegistro;
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
        private Guna.UI2.WinForms.Guna2Panel panelReporteriaSubmenu;
        private Guna.UI2.WinForms.Guna2GradientButton btncarnet;
        private Guna.UI2.WinForms.Guna2GradientButton btnhisto;
    }
}