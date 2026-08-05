namespace DVLD_PresentationLayer.Dashboard
{
    partial class ucServiceBreakdown
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
            this.lblAppTypeTitle = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.pbAppTypeTitle = new Guna.UI2.WinForms.Guna2ProgressBar();
            this.SuspendLayout();
            // 
            // lblAppTypeTitle
            // 
            this.lblAppTypeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAppTypeTitle.ForeColor = System.Drawing.Color.Black;
            this.lblAppTypeTitle.Location = new System.Drawing.Point(14, 0);
            this.lblAppTypeTitle.Name = "lblAppTypeTitle";
            this.lblAppTypeTitle.Size = new System.Drawing.Size(295, 17);
            this.lblAppTypeTitle.TabIndex = 2;
            this.lblAppTypeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.Black;
            this.lblTotal.Location = new System.Drawing.Point(368, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(48, 17);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbAppTypeTitle
            // 
            this.pbAppTypeTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbAppTypeTitle.BackColor = System.Drawing.Color.Transparent;
            this.pbAppTypeTitle.BorderRadius = 4;
            this.pbAppTypeTitle.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pbAppTypeTitle.Location = new System.Drawing.Point(17, 18);
            this.pbAppTypeTitle.Name = "pbAppTypeTitle";
            this.pbAppTypeTitle.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(95)))));
            this.pbAppTypeTitle.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(95)))));
            this.pbAppTypeTitle.Size = new System.Drawing.Size(401, 10);
            this.pbAppTypeTitle.TabIndex = 4;
            this.pbAppTypeTitle.Text = "guna2ProgressBar1";
            this.pbAppTypeTitle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // ucServiceBreakdown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbAppTypeTitle);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblAppTypeTitle);
            this.Name = "ucServiceBreakdown";
            this.Size = new System.Drawing.Size(445, 28);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAppTypeTitle;
        private System.Windows.Forms.Label lblTotal;
        private Guna.UI2.WinForms.Guna2ProgressBar pbAppTypeTitle;
    }
}
