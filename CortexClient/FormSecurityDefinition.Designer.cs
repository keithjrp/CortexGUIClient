namespace CortexClient
{
    partial class FormSecurityDefinition
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSecurityDefinition));
            this.tabSecurityDef = new System.Windows.Forms.TabPage();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tabCtrlDeal = new System.Windows.Forms.TabControl();
            this.tabSecurityBasic = new System.Windows.Forms.TabPage();
            this.lblSecTypeName = new System.Windows.Forms.Label();
            this.lblSecTypeDescr = new System.Windows.Forms.Label();
            this.lblCurrencyID = new System.Windows.Forms.Label();
            this.lblSecurityType = new System.Windows.Forms.Label();
            this.cbxCurrencyID = new System.Windows.Forms.ComboBox();
            this.cbxSecurityTypeID = new System.Windows.Forms.ComboBox();
            this.txtSecName = new System.Windows.Forms.TextBox();
            this.lblSecName = new System.Windows.Forms.Label();
            this.lblSecDescr = new System.Windows.Forms.Label();
            this.txtSecDescription = new System.Windows.Forms.TextBox();
            this.txtSecCode = new System.Windows.Forms.TextBox();
            this.lblSecCode = new System.Windows.Forms.Label();
            this.tabCtrlSecurityDef = new System.Windows.Forms.TabControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblIsinValue = new System.Windows.Forms.Label();
            this.lblSedolValue = new System.Windows.Forms.Label();
            this.lblCusipValue = new System.Windows.Forms.Label();
            this.lblTickerValue = new System.Windows.Forms.Label();
            this.tabSecurityDef.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabCtrlDeal.SuspendLayout();
            this.tabSecurityBasic.SuspendLayout();
            this.tabCtrlSecurityDef.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSecurityDef
            // 
            this.tabSecurityDef.BackColor = System.Drawing.Color.SkyBlue;
            this.tabSecurityDef.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabSecurityDef.Controls.Add(this.toolStripContainer1);
            this.tabSecurityDef.Controls.Add(this.tabCtrlDeal);
            this.tabSecurityDef.Location = new System.Drawing.Point(4, 22);
            this.tabSecurityDef.Name = "tabSecurityDef";
            this.tabSecurityDef.Padding = new System.Windows.Forms.Padding(3);
            this.tabSecurityDef.Size = new System.Drawing.Size(650, 179);
            this.tabSecurityDef.TabIndex = 0;
            this.tabSecurityDef.Text = "Security Definition";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(33, 118);
            this.toolStripContainer1.Location = new System.Drawing.Point(621, 27);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(33, 143);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Padding = new System.Windows.Forms.Padding(0, 0, 25, 25);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripButton,
            this.toolStripSeparator});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(24, 118);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.saveToolStripButton.Text = "&Save";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(21, 6);
            // 
            // tabCtrlDeal
            // 
            this.tabCtrlDeal.Controls.Add(this.tabSecurityBasic);
            this.tabCtrlDeal.Location = new System.Drawing.Point(14, 16);
            this.tabCtrlDeal.Name = "tabCtrlDeal";
            this.tabCtrlDeal.SelectedIndex = 0;
            this.tabCtrlDeal.Size = new System.Drawing.Size(601, 154);
            this.tabCtrlDeal.TabIndex = 0;
            // 
            // tabSecurityBasic
            // 
            this.tabSecurityBasic.BackColor = System.Drawing.Color.SkyBlue;
            this.tabSecurityBasic.Controls.Add(this.lblIsinValue);
            this.tabSecurityBasic.Controls.Add(this.lblSedolValue);
            this.tabSecurityBasic.Controls.Add(this.lblCusipValue);
            this.tabSecurityBasic.Controls.Add(this.lblTickerValue);
            this.tabSecurityBasic.Controls.Add(this.label3);
            this.tabSecurityBasic.Controls.Add(this.label4);
            this.tabSecurityBasic.Controls.Add(this.label1);
            this.tabSecurityBasic.Controls.Add(this.label2);
            this.tabSecurityBasic.Controls.Add(this.lblSecTypeName);
            this.tabSecurityBasic.Controls.Add(this.lblSecTypeDescr);
            this.tabSecurityBasic.Controls.Add(this.lblCurrencyID);
            this.tabSecurityBasic.Controls.Add(this.lblSecurityType);
            this.tabSecurityBasic.Controls.Add(this.cbxCurrencyID);
            this.tabSecurityBasic.Controls.Add(this.cbxSecurityTypeID);
            this.tabSecurityBasic.Controls.Add(this.txtSecName);
            this.tabSecurityBasic.Controls.Add(this.lblSecName);
            this.tabSecurityBasic.Controls.Add(this.lblSecDescr);
            this.tabSecurityBasic.Controls.Add(this.txtSecDescription);
            this.tabSecurityBasic.Controls.Add(this.txtSecCode);
            this.tabSecurityBasic.Controls.Add(this.lblSecCode);
            this.tabSecurityBasic.Location = new System.Drawing.Point(4, 22);
            this.tabSecurityBasic.Name = "tabSecurityBasic";
            this.tabSecurityBasic.Padding = new System.Windows.Forms.Padding(3);
            this.tabSecurityBasic.Size = new System.Drawing.Size(593, 128);
            this.tabSecurityBasic.TabIndex = 0;
            this.tabSecurityBasic.Text = "Basic Info";
            // 
            // lblSecTypeName
            // 
            this.lblSecTypeName.AutoSize = true;
            this.lblSecTypeName.Location = new System.Drawing.Point(128, 106);
            this.lblSecTypeName.Name = "lblSecTypeName";
            this.lblSecTypeName.Size = new System.Drawing.Size(35, 13);
            this.lblSecTypeName.TabIndex = 32;
            this.lblSecTypeName.Text = "Name";
            // 
            // lblSecTypeDescr
            // 
            this.lblSecTypeDescr.AutoSize = true;
            this.lblSecTypeDescr.Location = new System.Drawing.Point(9, 106);
            this.lblSecTypeDescr.Name = "lblSecTypeDescr";
            this.lblSecTypeDescr.Size = new System.Drawing.Size(60, 13);
            this.lblSecTypeDescr.TabIndex = 31;
            this.lblSecTypeDescr.Text = "Description";
            // 
            // lblCurrencyID
            // 
            this.lblCurrencyID.AutoSize = true;
            this.lblCurrencyID.Location = new System.Drawing.Point(196, 66);
            this.lblCurrencyID.Name = "lblCurrencyID";
            this.lblCurrencyID.Size = new System.Drawing.Size(49, 13);
            this.lblCurrencyID.TabIndex = 30;
            this.lblCurrencyID.Text = "Currency";
            // 
            // lblSecurityType
            // 
            this.lblSecurityType.AutoSize = true;
            this.lblSecurityType.Location = new System.Drawing.Point(11, 66);
            this.lblSecurityType.Name = "lblSecurityType";
            this.lblSecurityType.Size = new System.Drawing.Size(72, 13);
            this.lblSecurityType.TabIndex = 29;
            this.lblSecurityType.Text = "Security Type";
            // 
            // cbxCurrencyID
            // 
            this.cbxCurrencyID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbxCurrencyID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbxCurrencyID.FormattingEnabled = true;
            this.cbxCurrencyID.Location = new System.Drawing.Point(199, 82);
            this.cbxCurrencyID.Name = "cbxCurrencyID";
            this.cbxCurrencyID.Size = new System.Drawing.Size(140, 21);
            this.cbxCurrencyID.TabIndex = 28;
            this.cbxCurrencyID.SelectedIndexChanged += new System.EventHandler(this.cbxCurrencyID_SelectedIndexChanged);
            // 
            // cbxSecurityTypeID
            // 
            this.cbxSecurityTypeID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbxSecurityTypeID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbxSecurityTypeID.FormattingEnabled = true;
            this.cbxSecurityTypeID.Location = new System.Drawing.Point(11, 82);
            this.cbxSecurityTypeID.Name = "cbxSecurityTypeID";
            this.cbxSecurityTypeID.Size = new System.Drawing.Size(182, 21);
            this.cbxSecurityTypeID.TabIndex = 27;
            this.cbxSecurityTypeID.SelectedIndexChanged += new System.EventHandler(this.cbxSecurityGroupID_SelectedIndexChanged);
            // 
            // txtSecName
            // 
            this.txtSecName.Location = new System.Drawing.Point(480, 31);
            this.txtSecName.Name = "txtSecName";
            this.txtSecName.Size = new System.Drawing.Size(100, 20);
            this.txtSecName.TabIndex = 12;
            // 
            // lblSecName
            // 
            this.lblSecName.AutoSize = true;
            this.lblSecName.Location = new System.Drawing.Point(480, 15);
            this.lblSecName.Name = "lblSecName";
            this.lblSecName.Size = new System.Drawing.Size(35, 13);
            this.lblSecName.TabIndex = 13;
            this.lblSecName.Text = "Name";
            // 
            // lblSecDescr
            // 
            this.lblSecDescr.AutoSize = true;
            this.lblSecDescr.Location = new System.Drawing.Point(9, 15);
            this.lblSecDescr.Name = "lblSecDescr";
            this.lblSecDescr.Size = new System.Drawing.Size(60, 13);
            this.lblSecDescr.TabIndex = 6;
            this.lblSecDescr.Text = "Description";
            // 
            // txtSecDescription
            // 
            this.txtSecDescription.Location = new System.Drawing.Point(11, 31);
            this.txtSecDescription.Name = "txtSecDescription";
            this.txtSecDescription.Size = new System.Drawing.Size(328, 20);
            this.txtSecDescription.TabIndex = 5;
            // 
            // txtSecCode
            // 
            this.txtSecCode.Location = new System.Drawing.Point(361, 31);
            this.txtSecCode.Name = "txtSecCode";
            this.txtSecCode.Size = new System.Drawing.Size(100, 20);
            this.txtSecCode.TabIndex = 10;
            // 
            // lblSecCode
            // 
            this.lblSecCode.AutoSize = true;
            this.lblSecCode.Location = new System.Drawing.Point(361, 15);
            this.lblSecCode.Name = "lblSecCode";
            this.lblSecCode.Size = new System.Drawing.Size(32, 13);
            this.lblSecCode.TabIndex = 11;
            this.lblSecCode.Text = "Code";
            // 
            // tabCtrlSecurityDef
            // 
            this.tabCtrlSecurityDef.Controls.Add(this.tabSecurityDef);
            this.tabCtrlSecurityDef.Location = new System.Drawing.Point(-1, 2);
            this.tabCtrlSecurityDef.Name = "tabCtrlSecurityDef";
            this.tabCtrlSecurityDef.SelectedIndex = 0;
            this.tabCtrlSecurityDef.Size = new System.Drawing.Size(658, 205);
            this.tabCtrlSecurityDef.TabIndex = 36;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(478, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Cusip";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(358, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Ticker";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(478, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 36;
            this.label3.Text = "Isin";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(358, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Sedol";
            // 
            // lblIsinValue
            // 
            this.lblIsinValue.AutoSize = true;
            this.lblIsinValue.Location = new System.Drawing.Point(513, 90);
            this.lblIsinValue.Name = "lblIsinValue";
            this.lblIsinValue.Size = new System.Drawing.Size(53, 13);
            this.lblIsinValue.TabIndex = 40;
            this.lblIsinValue.Text = "Isin Value";
            // 
            // lblSedolValue
            // 
            this.lblSedolValue.AutoSize = true;
            this.lblSedolValue.Location = new System.Drawing.Point(394, 90);
            this.lblSedolValue.Name = "lblSedolValue";
            this.lblSedolValue.Size = new System.Drawing.Size(64, 13);
            this.lblSedolValue.TabIndex = 39;
            this.lblSedolValue.Text = "Sedol Value";
            // 
            // lblCusipValue
            // 
            this.lblCusipValue.AutoSize = true;
            this.lblCusipValue.Location = new System.Drawing.Point(513, 66);
            this.lblCusipValue.Name = "lblCusipValue";
            this.lblCusipValue.Size = new System.Drawing.Size(63, 13);
            this.lblCusipValue.TabIndex = 38;
            this.lblCusipValue.Text = "Cusip Value";
            // 
            // lblTickerValue
            // 
            this.lblTickerValue.AutoSize = true;
            this.lblTickerValue.Location = new System.Drawing.Point(394, 66);
            this.lblTickerValue.Name = "lblTickerValue";
            this.lblTickerValue.Size = new System.Drawing.Size(67, 13);
            this.lblTickerValue.TabIndex = 37;
            this.lblTickerValue.Text = "Ticker Value";
            // 
            // FormSecurityDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SkyBlue;
            this.ClientSize = new System.Drawing.Size(658, 206);
            this.Controls.Add(this.tabCtrlSecurityDef);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormSecurityDefinition";
            this.Text = "Security Definition";
            this.Load += new System.EventHandler(this.FormSecurityDefinition_Load);
            this.tabSecurityDef.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabCtrlDeal.ResumeLayout(false);
            this.tabSecurityBasic.ResumeLayout(false);
            this.tabSecurityBasic.PerformLayout();
            this.tabCtrlSecurityDef.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabSecurityDef;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.TabControl tabCtrlDeal;
        private System.Windows.Forms.TabPage tabSecurityBasic;
        private System.Windows.Forms.Label lblSecTypeName;
        private System.Windows.Forms.Label lblSecTypeDescr;
        private System.Windows.Forms.Label lblCurrencyID;
        private System.Windows.Forms.Label lblSecurityType;
        private System.Windows.Forms.ComboBox cbxCurrencyID;
        private System.Windows.Forms.ComboBox cbxSecurityTypeID;
        private System.Windows.Forms.TextBox txtSecName;
        private System.Windows.Forms.Label lblSecName;
        private System.Windows.Forms.Label lblSecDescr;
        private System.Windows.Forms.TextBox txtSecDescription;
        private System.Windows.Forms.TextBox txtSecCode;
        private System.Windows.Forms.Label lblSecCode;
        private System.Windows.Forms.TabControl tabCtrlSecurityDef;
        private System.Windows.Forms.Label lblIsinValue;
        private System.Windows.Forms.Label lblSedolValue;
        private System.Windows.Forms.Label lblCusipValue;
        private System.Windows.Forms.Label lblTickerValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}