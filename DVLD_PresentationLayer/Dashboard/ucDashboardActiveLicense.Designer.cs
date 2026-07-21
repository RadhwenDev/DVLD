namespace DVLD_PresentationLayer.Dashboard
{
    partial class ucDashboardActiveLicense
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblLicenseID = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDateExpiration = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblLicenseID
            // 
            this.lblLicenseID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicenseID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(136)))), ((int)(((byte)(164)))));
            this.lblLicenseID.Location = new System.Drawing.Point(26, 30);
            this.lblLicenseID.Name = "lblLicenseID";
            this.lblLicenseID.Size = new System.Drawing.Size(159, 20);
            this.lblLicenseID.TabIndex = 2;
            // 
            // lblFullName
            // 
            this.lblFullName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFullName.Location = new System.Drawing.Point(26, 4);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(297, 20);
            this.lblFullName.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(340, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Expires";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDateExpiration
            // 
            this.lblDateExpiration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateExpiration.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateExpiration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(136)))), ((int)(((byte)(164)))));
            this.lblDateExpiration.Location = new System.Drawing.Point(237, 30);
            this.lblDateExpiration.Name = "lblDateExpiration";
            this.lblDateExpiration.Size = new System.Drawing.Size(191, 20);
            this.lblDateExpiration.TabIndex = 2;
            this.lblDateExpiration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ucDashboardActiveLicense
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblDateExpiration);
            this.Controls.Add(this.lblLicenseID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblFullName);
            this.Name = "ucDashboardActiveLicense";
            this.Size = new System.Drawing.Size(440, 50);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblLicenseID;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDateExpiration;
    }
}
