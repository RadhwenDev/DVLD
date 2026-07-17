namespace DVLD_PresentationLayer.Applications
{
    partial class ucApplications
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucApplications));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbTypes = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cbStatuses = new Guna.UI2.WinForms.Guna2ComboBox();
            this.dgvApplications = new Guna.UI2.WinForms.Guna2DataGridView();
            this.tbFilterNameAppID = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnNewApplication = new Guna.UI2.WinForms.Guna2Button();
            this.lblCountTotalAndPending = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplications)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Panel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Panel1.BorderRadius = 20;
            this.guna2Panel1.Controls.Add(this.panel1);
            this.guna2Panel1.Controls.Add(this.cbTypes);
            this.guna2Panel1.Controls.Add(this.cbStatuses);
            this.guna2Panel1.Controls.Add(this.dgvApplications);
            this.guna2Panel1.Controls.Add(this.tbFilterNameAppID);
            this.guna2Panel1.FillColor = System.Drawing.Color.White;
            this.guna2Panel1.Location = new System.Drawing.Point(24, 203);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Padding = new System.Windows.Forms.Padding(20);
            this.guna2Panel1.Size = new System.Drawing.Size(1317, 460);
            this.guna2Panel1.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(610, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(35, 35);
            this.panel1.TabIndex = 14;
            // 
            // cbTypes
            // 
            this.cbTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTypes.BackColor = System.Drawing.Color.Transparent;
            this.cbTypes.BorderRadius = 10;
            this.cbTypes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTypes.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbTypes.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbTypes.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.cbTypes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cbTypes.ItemHeight = 47;
            this.cbTypes.Location = new System.Drawing.Point(943, 27);
            this.cbTypes.MaxDropDownItems = 4;
            this.cbTypes.Name = "cbTypes";
            this.cbTypes.Size = new System.Drawing.Size(351, 53);
            this.cbTypes.TabIndex = 13;
            // 
            // cbStatuses
            // 
            this.cbStatuses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbStatuses.BackColor = System.Drawing.Color.Transparent;
            this.cbStatuses.BorderRadius = 10;
            this.cbStatuses.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbStatuses.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStatuses.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbStatuses.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbStatuses.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.cbStatuses.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cbStatuses.ItemHeight = 47;
            this.cbStatuses.Location = new System.Drawing.Point(658, 27);
            this.cbStatuses.MaxDropDownItems = 4;
            this.cbStatuses.Name = "cbStatuses";
            this.cbStatuses.Size = new System.Drawing.Size(257, 53);
            this.cbStatuses.TabIndex = 12;
            // 
            // dgvApplications
            // 
            this.dgvApplications.AllowUserToAddRows = false;
            this.dgvApplications.AllowUserToResizeColumns = false;
            this.dgvApplications.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            this.dgvApplications.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvApplications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvApplications.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvApplications.ColumnHeadersHeight = 40;
            this.dgvApplications.Cursor = System.Windows.Forms.Cursors.IBeam;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvApplications.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvApplications.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvApplications.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvApplications.Location = new System.Drawing.Point(0, 117);
            this.dgvApplications.Name = "dgvApplications";
            this.dgvApplications.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvApplications.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvApplications.RowHeadersVisible = false;
            this.dgvApplications.RowHeadersWidth = 51;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Arial Narrow", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvApplications.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvApplications.RowTemplate.Height = 60;
            this.dgvApplications.RowTemplate.ReadOnly = true;
            this.dgvApplications.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvApplications.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvApplications.Size = new System.Drawing.Size(1317, 328);
            this.dgvApplications.TabIndex = 2;
            this.dgvApplications.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvApplications.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvApplications.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvApplications.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(201)))), ((int)(((byte)(244)))));
            this.dgvApplications.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvApplications.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvApplications.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvApplications.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvApplications.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvApplications.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvApplications.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvApplications.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvApplications.ThemeStyle.HeaderStyle.Height = 40;
            this.dgvApplications.ThemeStyle.ReadOnly = true;
            this.dgvApplications.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvApplications.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvApplications.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvApplications.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvApplications.ThemeStyle.RowsStyle.Height = 60;
            this.dgvApplications.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvApplications.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvApplications.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvApplications_CellFormatting);
            this.dgvApplications.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvApplications_CellPainting);
            this.dgvApplications.Paint += new System.Windows.Forms.PaintEventHandler(this.dgvApplications_Paint);
            // 
            // tbFilterNameAppID
            // 
            this.tbFilterNameAppID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilterNameAppID.BackColor = System.Drawing.Color.White;
            this.tbFilterNameAppID.BorderRadius = 15;
            this.tbFilterNameAppID.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbFilterNameAppID.DefaultText = "";
            this.tbFilterNameAppID.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbFilterNameAppID.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbFilterNameAppID.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbFilterNameAppID.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbFilterNameAppID.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbFilterNameAppID.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbFilterNameAppID.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.tbFilterNameAppID.ForeColor = System.Drawing.Color.Gray;
            this.tbFilterNameAppID.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbFilterNameAppID.Location = new System.Drawing.Point(20, 27);
            this.tbFilterNameAppID.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.tbFilterNameAppID.Name = "tbFilterNameAppID";
            this.tbFilterNameAppID.PlaceholderText = "🔎 Search by name or application ID...";
            this.tbFilterNameAppID.SelectedText = "";
            this.tbFilterNameAppID.Size = new System.Drawing.Size(558, 63);
            this.tbFilterNameAppID.TabIndex = 3;
            this.tbFilterNameAppID.TextChanged += new System.EventHandler(this.tbFilterNameAppID_TextChanged);
            // 
            // btnNewApplication
            // 
            this.btnNewApplication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewApplication.BorderRadius = 10;
            this.btnNewApplication.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewApplication.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnNewApplication.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnNewApplication.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnNewApplication.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnNewApplication.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(52)))), ((int)(((byte)(90)))));
            this.btnNewApplication.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnNewApplication.ForeColor = System.Drawing.Color.White;
            this.btnNewApplication.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(76)))), ((int)(((byte)(110)))));
            this.btnNewApplication.Location = new System.Drawing.Point(1038, 95);
            this.btnNewApplication.Name = "btnNewApplication";
            this.btnNewApplication.Size = new System.Drawing.Size(269, 52);
            this.btnNewApplication.TabIndex = 10;
            this.btnNewApplication.Text = " ➕       New Application";
            this.btnNewApplication.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnNewApplication.Click += new System.EventHandler(this.btnNewApplication_Click);
            // 
            // lblCountTotalAndPending
            // 
            this.lblCountTotalAndPending.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCountTotalAndPending.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(136)))), ((int)(((byte)(164)))));
            this.lblCountTotalAndPending.Location = new System.Drawing.Point(40, 95);
            this.lblCountTotalAndPending.Name = "lblCountTotalAndPending";
            this.lblCountTotalAndPending.Size = new System.Drawing.Size(274, 26);
            this.lblCountTotalAndPending.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(243, 40);
            this.label1.TabIndex = 8;
            this.label1.Text = "Applications";
            // 
            // ucApplications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.btnNewApplication);
            this.Controls.Add(this.lblCountTotalAndPending);
            this.Controls.Add(this.label1);
            this.Name = "ucApplications";
            this.Size = new System.Drawing.Size(1362, 739);
            this.Load += new System.EventHandler(this.ucApplications_Load);
            this.guna2Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvApplications)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2DataGridView dgvApplications;
        private Guna.UI2.WinForms.Guna2TextBox tbFilterNameAppID;
        private Guna.UI2.WinForms.Guna2Button btnNewApplication;
        private System.Windows.Forms.Label lblCountTotalAndPending;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2ComboBox cbStatuses;
        private Guna.UI2.WinForms.Guna2ComboBox cbTypes;
        private System.Windows.Forms.Panel panel1;
    }
}
