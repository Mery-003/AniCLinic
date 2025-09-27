namespace AniCLinic
{
    partial class frmHistorialMedicoReport
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
            this.rvwHistorial = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rvwHistorial
            // 
            this.rvwHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rvwHistorial.Location = new System.Drawing.Point(0, 0);
            this.rvwHistorial.Name = "rvwHistorial";
            this.rvwHistorial.ServerReport.BearerToken = null;
            this.rvwHistorial.Size = new System.Drawing.Size(546, 450);
            this.rvwHistorial.TabIndex = 0;
            // 
            // frmHistorialMedicoReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 450);
            this.Controls.Add(this.rvwHistorial);
            this.Name = "frmHistorialMedicoReport";
            this.Text = "frmHistorialMedicoReport";
            this.Load += new System.EventHandler(this.frmHistorialMedicoReport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rvwHistorial;
    }
}