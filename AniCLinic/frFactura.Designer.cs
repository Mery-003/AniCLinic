namespace AniCLinic
{
    partial class frFactura
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
            this.rvwFactura = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rvwFactura
            // 
            this.rvwFactura.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rvwFactura.Location = new System.Drawing.Point(0, 0);
            this.rvwFactura.Name = "rvwFactura";
            this.rvwFactura.ServerReport.BearerToken = null;
            this.rvwFactura.Size = new System.Drawing.Size(553, 450);
            this.rvwFactura.TabIndex = 0;
            // 
            // frFactura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 450);
            this.Controls.Add(this.rvwFactura);
            this.Name = "frFactura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frFactura";
            this.Load += new System.EventHandler(this.frFactura_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rvwFactura;
    }
}