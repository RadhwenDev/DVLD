namespace DVLD_PresentationLayer.Dashboard
{
    partial class ucApplicationPersonInfos
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
            this.pbImagePerson = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblAppType_AppID = new System.Windows.Forms.Label();
            this.lblDateApp = new System.Windows.Forms.Label();
            this.statusBadge = new Guna.UI2.WinForms.Guna2Chip();
            ((System.ComponentModel.ISupportInitialize)(this.pbImagePerson)).BeginInit();
            this.SuspendLayout();
            // 
            // pbImagePerson
            // 
            this.pbImagePerson.FillColor = System.Drawing.Color.Transparent;
            this.pbImagePerson.ImageRotate = 0F;
            this.pbImagePerson.Location = new System.Drawing.Point(29, 10);
            this.pbImagePerson.Name = "pbImagePerson";
            this.pbImagePerson.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.pbImagePerson.Size = new System.Drawing.Size(70, 55);
            this.pbImagePerson.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbImagePerson.TabIndex = 0;
            this.pbImagePerson.TabStop = false;
            // 
            // lblFullName
            // 
            this.lblFullName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFullName.Location = new System.Drawing.Point(120, 10);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(297, 25);
            this.lblFullName.TabIndex = 1;
            // 
            // lblAppType_AppID
            // 
            this.lblAppType_AppID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAppType_AppID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(136)))), ((int)(((byte)(164)))));
            this.lblAppType_AppID.Location = new System.Drawing.Point(120, 40);
            this.lblAppType_AppID.Name = "lblAppType_AppID";
            this.lblAppType_AppID.Size = new System.Drawing.Size(312, 25);
            this.lblAppType_AppID.TabIndex = 1;
            // 
            // lblDateApp
            // 
            this.lblDateApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateApp.Location = new System.Drawing.Point(585, 10);
            this.lblDateApp.Name = "lblDateApp";
            this.lblDateApp.Size = new System.Drawing.Size(181, 32);
            this.lblDateApp.TabIndex = 1;
            this.lblDateApp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusBadge
            // 
            this.statusBadge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statusBadge.BorderRadius = 15;
            this.statusBadge.FillColor = System.Drawing.Color.Transparent;
            this.statusBadge.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.statusBadge.ForeColor = System.Drawing.Color.Transparent;
            this.statusBadge.IsClosable = false;
            this.statusBadge.Location = new System.Drawing.Point(777, 10);
            this.statusBadge.Name = "statusBadge";
            this.statusBadge.Size = new System.Drawing.Size(133, 47);
            this.statusBadge.TabIndex = 2;
            this.statusBadge.Text = "guna2Chip1";
            // 
            // ucApplicationPersonInfos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.statusBadge);
            this.Controls.Add(this.lblAppType_AppID);
            this.Controls.Add(this.lblDateApp);
            this.Controls.Add(this.lblFullName);
            this.Controls.Add(this.pbImagePerson);
            this.Name = "ucApplicationPersonInfos";
            this.Size = new System.Drawing.Size(934, 67);
            ((System.ComponentModel.ISupportInitialize)(this.pbImagePerson)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2CirclePictureBox pbImagePerson;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblAppType_AppID;
        private System.Windows.Forms.Label lblDateApp;
        private Guna.UI2.WinForms.Guna2Chip statusBadge;
    }
}
