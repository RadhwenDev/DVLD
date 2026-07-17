namespace DVLD_PresentationLayer.Applications
{
    partial class ucSecondStepNewApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucSecondStepNewApp));
            this.label1 = new System.Windows.Forms.Label();
            this.pbIconStep = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.btnContinue = new Guna.UI2.WinForms.Guna2Button();
            this.btnBack = new Guna.UI2.WinForms.Guna2Button();
            this.cbApplicationType = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cbLicenseClass = new Guna.UI2.WinForms.Guna2ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbIconStep)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(97, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(362, 49);
            this.label1.TabIndex = 8;
            this.label1.Text = "Step 2 — Select Service Type";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbIconStep
            // 
            this.pbIconStep.BackColor = System.Drawing.SystemColors.InfoText;
            this.pbIconStep.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbIconStep.BackgroundImage")));
            this.pbIconStep.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbIconStep.FillColor = System.Drawing.Color.Transparent;
            this.pbIconStep.ImageRotate = 0F;
            this.pbIconStep.Location = new System.Drawing.Point(51, 25);
            this.pbIconStep.Name = "pbIconStep";
            this.pbIconStep.Size = new System.Drawing.Size(30, 39);
            this.pbIconStep.TabIndex = 7;
            this.pbIconStep.TabStop = false;
            // 
            // guna2Separator1
            // 
            this.guna2Separator1.Location = new System.Drawing.Point(-1, 75);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(901, 16);
            this.guna2Separator1.TabIndex = 6;
            // 
            // btnContinue
            // 
            this.btnContinue.BorderRadius = 15;
            this.btnContinue.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnContinue.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnContinue.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnContinue.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnContinue.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.btnContinue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnContinue.ForeColor = System.Drawing.Color.White;
            this.btnContinue.Location = new System.Drawing.Point(674, 277);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(180, 52);
            this.btnContinue.TabIndex = 11;
            this.btnContinue.Text = "Continue";
            // 
            // btnBack
            // 
            this.btnBack.BorderColor = System.Drawing.Color.Silver;
            this.btnBack.BorderRadius = 15;
            this.btnBack.BorderThickness = 1;
            this.btnBack.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnBack.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnBack.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnBack.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnBack.FillColor = System.Drawing.Color.White;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.Black;
            this.btnBack.Location = new System.Drawing.Point(51, 277);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(104, 52);
            this.btnBack.TabIndex = 12;
            this.btnBack.Text = "Back";
            // 
            // cbApplicationType
            // 
            this.cbApplicationType.BackColor = System.Drawing.Color.Transparent;
            this.cbApplicationType.BorderRadius = 15;
            this.cbApplicationType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbApplicationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbApplicationType.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbApplicationType.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbApplicationType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbApplicationType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbApplicationType.ItemHeight = 46;
            this.cbApplicationType.Location = new System.Drawing.Point(51, 135);
            this.cbApplicationType.Name = "cbApplicationType";
            this.cbApplicationType.Size = new System.Drawing.Size(803, 52);
            this.cbApplicationType.TabIndex = 13;
            // 
            // cbLicenseClass
            // 
            this.cbLicenseClass.BackColor = System.Drawing.Color.Transparent;
            this.cbLicenseClass.BorderRadius = 15;
            this.cbLicenseClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbLicenseClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLicenseClass.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbLicenseClass.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbLicenseClass.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbLicenseClass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbLicenseClass.ItemHeight = 46;
            this.cbLicenseClass.Location = new System.Drawing.Point(51, 208);
            this.cbLicenseClass.Name = "cbLicenseClass";
            this.cbLicenseClass.Size = new System.Drawing.Size(803, 52);
            this.cbLicenseClass.TabIndex = 14;
            this.cbLicenseClass.Visible = false;
            // 
            // ucSecondStepNewApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cbLicenseClass);
            this.Controls.Add(this.cbApplicationType);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbIconStep);
            this.Controls.Add(this.guna2Separator1);
            this.Name = "ucSecondStepNewApp";
            this.Size = new System.Drawing.Size(900, 362);
            this.Load += new System.EventHandler(this.ucSecondStepNewApp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbIconStep)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2PictureBox pbIconStep;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2Button btnContinue;
        private Guna.UI2.WinForms.Guna2Button btnBack;
        private Guna.UI2.WinForms.Guna2ComboBox cbApplicationType;
        private Guna.UI2.WinForms.Guna2ComboBox cbLicenseClass;
    }
}
