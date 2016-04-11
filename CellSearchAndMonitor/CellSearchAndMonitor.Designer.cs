namespace CellSearchAndMonitor
{
    partial class CellSearchAndMonitor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CellSearchListView = new System.Windows.Forms.ListView();
            this.序号 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.主频 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EARFCN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PCI = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.强度 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.质量 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TAI = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ECGI = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PCIText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.AddToSearchListButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CellStopBtn = new System.Windows.Forms.Button();
            this.CellIndexLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CellMonitorButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.CellMonitorListView = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.OPComboBox = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.DisconBtn = new System.Windows.Forms.Button();
            this.PingText = new System.Windows.Forms.Label();
            this.ConBtn = new System.Windows.Forms.Button();
            this.带宽 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.CellSearchListView);
            this.groupBox1.Location = new System.Drawing.Point(12, 150);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 168);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "搜索小区列表";
            // 
            // CellSearchListView
            // 
            this.CellSearchListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.序号,
            this.主频,
            this.EARFCN,
            this.PCI,
            this.强度,
            this.质量,
            this.带宽,
            this.TAI,
            this.ECGI,
            this.columnHeader1});
            this.CellSearchListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CellSearchListView.FullRowSelect = true;
            this.CellSearchListView.GridLines = true;
            this.CellSearchListView.Location = new System.Drawing.Point(3, 17);
            this.CellSearchListView.Name = "CellSearchListView";
            this.CellSearchListView.Size = new System.Drawing.Size(391, 148);
            this.CellSearchListView.TabIndex = 0;
            this.CellSearchListView.UseCompatibleStateImageBehavior = false;
            this.CellSearchListView.View = System.Windows.Forms.View.Details;
            this.CellSearchListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CellSearchListView_MouseClick);
            this.CellSearchListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CellSearchListView_MouseDoubleClick);
            // 
            // 序号
            // 
            this.序号.Text = "序号";
            this.序号.Width = 40;
            // 
            // 主频
            // 
            this.主频.Text = "主频";
            this.主频.Width = 50;
            // 
            // EARFCN
            // 
            this.EARFCN.Text = "EARFCN";
            // 
            // PCI
            // 
            this.PCI.Text = "PCI";
            this.PCI.Width = 40;
            // 
            // 强度
            // 
            this.强度.Text = "强度";
            // 
            // 质量
            // 
            this.质量.Text = "质量";
            this.质量.Width = 50;
            // 
            // TAI
            // 
            this.TAI.Text = "TAI";
            // 
            // ECGI
            // 
            this.ECGI.Text = "E-CGI";
            this.ECGI.Width = 70;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "运营商";
            // 
            // PCIText
            // 
            this.PCIText.Location = new System.Drawing.Point(201, 16);
            this.PCIText.Name = "PCIText";
            this.PCIText.Size = new System.Drawing.Size(107, 21);
            this.PCIText.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "PCI：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "运营商：";
            // 
            // AddToSearchListButton
            // 
            this.AddToSearchListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddToSearchListButton.Location = new System.Drawing.Point(314, 14);
            this.AddToSearchListButton.Name = "AddToSearchListButton";
            this.AddToSearchListButton.Size = new System.Drawing.Size(75, 23);
            this.AddToSearchListButton.TabIndex = 8;
            this.AddToSearchListButton.Text = "手动添加";
            this.AddToSearchListButton.UseVisualStyleBackColor = true;
            this.AddToSearchListButton.Click += new System.EventHandler(this.AddToSearchListButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.CellStopBtn);
            this.groupBox2.Controls.Add(this.CellIndexLabel);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.CellMonitorButton);
            this.groupBox2.Location = new System.Drawing.Point(15, 324);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(394, 62);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "监控";
            // 
            // CellStopBtn
            // 
            this.CellStopBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CellStopBtn.Location = new System.Drawing.Point(262, 20);
            this.CellStopBtn.Name = "CellStopBtn";
            this.CellStopBtn.Size = new System.Drawing.Size(75, 23);
            this.CellStopBtn.TabIndex = 9;
            this.CellStopBtn.Text = "停止监控";
            this.CellStopBtn.UseVisualStyleBackColor = true;
            this.CellStopBtn.Click += new System.EventHandler(this.CellStopBtn_Click);
            // 
            // CellIndexLabel
            // 
            this.CellIndexLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CellIndexLabel.AutoSize = true;
            this.CellIndexLabel.Location = new System.Drawing.Point(198, 25);
            this.CellIndexLabel.Name = "CellIndexLabel";
            this.CellIndexLabel.Size = new System.Drawing.Size(11, 12);
            this.CellIndexLabel.TabIndex = 8;
            this.CellIndexLabel.Text = " ";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(215, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "个小区";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(151, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "已搜索";
            // 
            // CellMonitorButton
            // 
            this.CellMonitorButton.Location = new System.Drawing.Point(58, 20);
            this.CellMonitorButton.Name = "CellMonitorButton";
            this.CellMonitorButton.Size = new System.Drawing.Size(75, 23);
            this.CellMonitorButton.TabIndex = 5;
            this.CellMonitorButton.Text = "小区监控";
            this.CellMonitorButton.UseVisualStyleBackColor = true;
            this.CellMonitorButton.Click += new System.EventHandler(this.CellMonitorButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.CellMonitorListView);
            this.groupBox3.Location = new System.Drawing.Point(12, 392);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(397, 128);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "守控小区列表";
            // 
            // CellMonitorListView
            // 
            this.CellMonitorListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader2,
            this.columnHeader9,
            this.columnHeader12,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader3});
            this.CellMonitorListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CellMonitorListView.FullRowSelect = true;
            this.CellMonitorListView.GridLines = true;
            this.CellMonitorListView.Location = new System.Drawing.Point(3, 17);
            this.CellMonitorListView.Name = "CellMonitorListView";
            this.CellMonitorListView.Size = new System.Drawing.Size(391, 108);
            this.CellMonitorListView.TabIndex = 1;
            this.CellMonitorListView.UseCompatibleStateImageBehavior = false;
            this.CellMonitorListView.View = System.Windows.Forms.View.Details;
            this.CellMonitorListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CellSearchListView_MouseClick);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "序号";
            this.columnHeader7.Width = 46;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "主频";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "EARFCN";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "PCI";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "监控板卡";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "TAI";
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "E-CGI";
            this.columnHeader11.Width = 72;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "运营商";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.OPComboBox);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.AddToSearchListButton);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.PCIText);
            this.groupBox4.Location = new System.Drawing.Point(12, 99);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(397, 45);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            // 
            // OPComboBox
            // 
            this.OPComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPComboBox.FormattingEnabled = true;
            this.OPComboBox.Location = new System.Drawing.Point(66, 16);
            this.OPComboBox.Name = "OPComboBox";
            this.OPComboBox.Size = new System.Drawing.Size(88, 20);
            this.OPComboBox.TabIndex = 9;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_ItemClicked);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.DisconBtn);
            this.groupBox5.Controls.Add(this.PingText);
            this.groupBox5.Controls.Add(this.ConBtn);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(397, 81);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "快速连接";
            // 
            // DisconBtn
            // 
            this.DisconBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DisconBtn.Location = new System.Drawing.Point(302, 31);
            this.DisconBtn.Name = "DisconBtn";
            this.DisconBtn.Size = new System.Drawing.Size(75, 23);
            this.DisconBtn.TabIndex = 4;
            this.DisconBtn.Text = "断开";
            this.DisconBtn.UseVisualStyleBackColor = true;
            this.DisconBtn.Click += new System.EventHandler(this.DisconBtn_Click);
            // 
            // PingText
            // 
            this.PingText.AutoSize = true;
            this.PingText.Location = new System.Drawing.Point(42, 36);
            this.PingText.Name = "PingText";
            this.PingText.Size = new System.Drawing.Size(41, 12);
            this.PingText.TabIndex = 3;
            this.PingText.Text = "      ";
            // 
            // ConBtn
            // 
            this.ConBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConBtn.Location = new System.Drawing.Point(184, 31);
            this.ConBtn.Name = "ConBtn";
            this.ConBtn.Size = new System.Drawing.Size(75, 23);
            this.ConBtn.TabIndex = 2;
            this.ConBtn.Text = "连接";
            this.ConBtn.UseVisualStyleBackColor = true;
            this.ConBtn.Click += new System.EventHandler(this.ConBtn_Click);
            // 
            // 带宽
            // 
            this.带宽.Text = "带宽";
            // 
            // CellSearchAndMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 532);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "CellSearchAndMonitor";
            this.Text = "CellSearchAndMonitor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView CellSearchListView;
        private System.Windows.Forms.ColumnHeader 序号;
        private System.Windows.Forms.ColumnHeader 主频;
        private System.Windows.Forms.ColumnHeader PCI;
        private System.Windows.Forms.ColumnHeader TAI;
        private System.Windows.Forms.ColumnHeader ECGI;
        private System.Windows.Forms.ColumnHeader 强度;
        private System.Windows.Forms.TextBox PCIText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AddToSearchListButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button CellMonitorButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView CellMonitorListView;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ComboBox OPComboBox;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader 质量;
        private System.Windows.Forms.Label CellIndexLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button CellStopBtn;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button ConBtn;
        private System.Windows.Forms.Button DisconBtn;
        private System.Windows.Forms.Label PingText;
        private System.Windows.Forms.ColumnHeader EARFCN;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader 带宽;
    }
}