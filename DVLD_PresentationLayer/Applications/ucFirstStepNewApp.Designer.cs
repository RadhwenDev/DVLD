namespace DVLD_PresentationLayer.Applications
{
    partial class ucFirstStepNewApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucFirstStepNewApp));
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.cbPerson = new Guna.UI2.WinForms.Guna2ComboBox();
            this.btnContinue = new Guna.UI2.WinForms.Guna2Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pbIconStep = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIconStep)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.White;
            this.guna2Panel1.BorderRadius = 15;
            this.guna2Panel1.Controls.Add(this.cbPerson);
            this.guna2Panel1.Controls.Add(this.btnContinue);
            this.guna2Panel1.Controls.Add(this.label1);
            this.guna2Panel1.Controls.Add(this.pbIconStep);
            this.guna2Panel1.Controls.Add(this.guna2Separator1);
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(900, 234);
            this.guna2Panel1.TabIndex = 9;
            // 
            // cbPerson
            // 
            this.cbPerson.BackColor = System.Drawing.Color.Transparent;
            this.cbPerson.BorderRadius = 15;
            this.cbPerson.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPerson.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbPerson.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbPerson.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbPerson.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbPerson.ItemHeight = 37;
            this.cbPerson.Location = new System.Drawing.Point(23, 144);
            this.cbPerson.Name = "cbPerson";
            this.cbPerson.Size = new System.Drawing.Size(658, 43);
            this.cbPerson.TabIndex = 6;
            this.cbPerson.SelectedIndexChanged += new System.EventHandler(this.cbPerson_SelectedIndexChanged);
            // 
            // btnContinue
            // 
            this.btnContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContinue.BorderRadius = 15;
            this.btnContinue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnContinue.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnContinue.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnContinue.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnContinue.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnContinue.Enabled = false;
            this.btnContinue.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(77)))), ((int)(((byte)(111)))));
            this.btnContinue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnContinue.ForeColor = System.Drawing.Color.White;
            this.btnContinue.Location = new System.Drawing.Point(708, 144);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(180, 45);
            this.btnContinue.TabIndex = 3;
            this.btnContinue.Text = "Continue";
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(98, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(362, 49);
            this.label1.TabIndex = 5;
            this.label1.Text = "Step 1 — Select Applicant";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbIconStep
            // 
            this.pbIconStep.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbIconStep.BackgroundImage")));
            this.pbIconStep.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbIconStep.FillColor = System.Drawing.Color.Transparent;
            this.pbIconStep.ImageRotate = 0F;
            this.pbIconStep.Location = new System.Drawing.Point(41, 27);
            this.pbIconStep.Name = "pbIconStep";
            this.pbIconStep.Size = new System.Drawing.Size(39, 49);
            this.pbIconStep.TabIndex = 4;
            this.pbIconStep.TabStop = false;
            // 
            // guna2Separator1
            // 
            this.guna2Separator1.Location = new System.Drawing.Point(0, 82);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(900, 19);
            this.guna2Separator1.TabIndex = 0;
            // 
            // ucFirstStepNewApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.guna2Panel1);
            this.Name = "ucFirstStepNewApp";
            this.Size = new System.Drawing.Size(900, 237);
            this.Load += new System.EventHandler(this.ucFirstStepNewApp_Load);
            this.guna2Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbIconStep)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Button btnContinue;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2PictureBox pbIconStep;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2ComboBox cbPerson;
    }
}
