namespace CellSearchAndMonitor
{
    partial class SystemConfig
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
            this.SearchListView = new System.Windows.Forms.ListView();
            this.序号 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.主频 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EARFCN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.OPComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.earfcnBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.freqBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BandTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.MessageTextBox = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SendPingTimeTextBox = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.smsModCB = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SendMessageTimeTextBox = new System.Windows.Forms.TextBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SINRThresholdText = new System.Windows.Forms.TextBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.OriRefreshTimeTextBox = new System.Windows.Forms.TextBox();
            this.SaveDataButton = new System.Windows.Forms.Button();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.UeSilenceCheckTimerText = new System.Windows.Forms.TextBox();
            this.RxAntNumText = new System.Windows.Forms.TextBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.CenterCodeText = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SearchListView);
            this.groupBox1.Location = new System.Drawing.Point(18, 174);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(586, 387);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "检测集信息列表";
            // 
            // SearchListView
            // 
            this.SearchListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.序号,
            this.主频,
            this.EARFCN});
            this.SearchListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchListView.FullRowSelect = true;
            this.SearchListView.GridLines = true;
            this.SearchListView.Location = new System.Drawing.Point(4, 25);
            this.SearchListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SearchListView.Name = "SearchListView";
            this.SearchListView.Size = new System.Drawing.Size(578, 358);
            this.SearchListView.TabIndex = 1;
            this.SearchListView.UseCompatibleStateImageBehavior = false;
            this.SearchListView.View = System.Windows.Forms.View.Details;
            this.SearchListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchListView_MouseClick);
            // 
            // 序号
            // 
            this.序号.Text = "Band";
            this.序号.Width = 100;
            // 
            // 主频
            // 
            this.主频.Text = "频率";
            this.主频.Width = 124;
            // 
            // EARFCN
            // 
            this.EARFCN.Text = "EARFCN";
            this.EARFCN.Width = 120;
            // 
            // OPComboBox
            // 
            this.OPComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OPComboBox.FormattingEnabled = true;
            this.OPComboBox.Location = new System.Drawing.Point(9, 52);
            this.OPComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OPComboBox.Name = "OPComboBox";
            this.OPComboBox.Size = new System.Drawing.Size(150, 26);
            this.OPComboBox.TabIndex = 0;
            this.OPComboBox.SelectedIndexChanged += new System.EventHandler(this.OPComboBox_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.earfcnBox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.AddButton);
            this.groupBox2.Controls.Add(this.freqBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.BandTextBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(208, 18);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(392, 144);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "检测集";
            // 
            // earfcnBox
            // 
            this.earfcnBox.Location = new System.Drawing.Point(242, 82);
            this.earfcnBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.earfcnBox.Name = "earfcnBox";
            this.earfcnBox.Size = new System.Drawing.Size(122, 28);
            this.earfcnBox.TabIndex = 6;
            this.earfcnBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.earfcnBox_KeyUp);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(160, 87);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 18);
            this.label8.TabIndex = 5;
            this.label8.Text = "Earfcn：";
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(12, 87);
            this.AddButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(112, 34);
            this.AddButton.TabIndex = 4;
            this.AddButton.Text = "添加";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // freqBox
            // 
            this.freqBox.Location = new System.Drawing.Point(242, 32);
            this.freqBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.freqBox.Name = "freqBox";
            this.freqBox.Size = new System.Drawing.Size(122, 28);
            this.freqBox.TabIndex = 3;
            this.freqBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.freqBox_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(171, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "频率：";
            // 
            // BandTextBox
            // 
            this.BandTextBox.Location = new System.Drawing.Point(68, 32);
            this.BandTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BandTextBox.Name = "BandTextBox";
            this.BandTextBox.Size = new System.Drawing.Size(55, 28);
            this.BandTextBox.TabIndex = 1;
            this.BandTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BandTextBox_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Band：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.OPComboBox);
            this.groupBox3.Location = new System.Drawing.Point(18, 18);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(182, 144);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "运营商";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox14);
            this.groupBox4.Controls.Add(this.groupBox8);
            this.groupBox4.Controls.Add(this.groupBox7);
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Location = new System.Drawing.Point(614, 18);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(387, 300);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "触发配置";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.MessageTextBox);
            this.groupBox8.Location = new System.Drawing.Point(196, 122);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox8.Size = new System.Drawing.Size(186, 162);
            this.groupBox8.TabIndex = 4;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "短消息内容";
            // 
            // MessageTextBox
            // 
            this.MessageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageTextBox.Location = new System.Drawing.Point(4, 25);
            this.MessageTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MessageTextBox.Multiline = true;
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.Size = new System.Drawing.Size(178, 133);
            this.MessageTextBox.TabIndex = 0;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.SendPingTimeTextBox);
            this.groupBox7.Location = new System.Drawing.Point(10, 207);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Size = new System.Drawing.Size(178, 76);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Ping发送间隔";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(102, 34);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 18);
            this.label5.TabIndex = 1;
            this.label5.Text = "秒";
            // 
            // SendPingTimeTextBox
            // 
            this.SendPingTimeTextBox.Location = new System.Drawing.Point(28, 30);
            this.SendPingTimeTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SendPingTimeTextBox.Name = "SendPingTimeTextBox";
            this.SendPingTimeTextBox.Size = new System.Drawing.Size(62, 28);
            this.SendPingTimeTextBox.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.smsModCB);
            this.groupBox6.Location = new System.Drawing.Point(10, 36);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Size = new System.Drawing.Size(178, 76);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "短信类型";
            // 
            // smsModCB
            // 
            this.smsModCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.smsModCB.FormattingEnabled = true;
            this.smsModCB.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "明短信"});
            this.smsModCB.Location = new System.Drawing.Point(28, 32);
            this.smsModCB.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.smsModCB.Name = "smsModCB";
            this.smsModCB.Size = new System.Drawing.Size(126, 26);
            this.smsModCB.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.SendMessageTimeTextBox);
            this.groupBox5.Location = new System.Drawing.Point(10, 122);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Size = new System.Drawing.Size(178, 76);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "短消息发送间隔";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 34);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 18);
            this.label3.TabIndex = 1;
            this.label3.Text = "秒";
            // 
            // SendMessageTimeTextBox
            // 
            this.SendMessageTimeTextBox.Location = new System.Drawing.Point(28, 30);
            this.SendMessageTimeTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SendMessageTimeTextBox.Name = "SendMessageTimeTextBox";
            this.SendMessageTimeTextBox.Size = new System.Drawing.Size(62, 28);
            this.SendMessageTimeTextBox.TabIndex = 0;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.groupBox11);
            this.groupBox9.Controls.Add(this.groupBox10);
            this.groupBox9.Location = new System.Drawing.Point(614, 327);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox9.Size = new System.Drawing.Size(387, 120);
            this.groupBox9.TabIndex = 4;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "测向配置";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.label7);
            this.groupBox11.Controls.Add(this.SINRThresholdText);
            this.groupBox11.Location = new System.Drawing.Point(200, 30);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox11.Size = new System.Drawing.Size(178, 76);
            this.groupBox11.TabIndex = 4;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "信噪比门限";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(106, 34);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 18);
            this.label7.TabIndex = 2;
            this.label7.Text = "dB";
            // 
            // SINRThresholdText
            // 
            this.SINRThresholdText.Location = new System.Drawing.Point(34, 30);
            this.SINRThresholdText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SINRThresholdText.Name = "SINRThresholdText";
            this.SINRThresholdText.Size = new System.Drawing.Size(61, 28);
            this.SINRThresholdText.TabIndex = 0;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.label6);
            this.groupBox10.Controls.Add(this.OriRefreshTimeTextBox);
            this.groupBox10.Location = new System.Drawing.Point(10, 30);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox10.Size = new System.Drawing.Size(178, 76);
            this.groupBox10.TabIndex = 3;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "测向数据刷新间隔";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(102, 34);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 18);
            this.label6.TabIndex = 1;
            this.label6.Text = "秒";
            // 
            // OriRefreshTimeTextBox
            // 
            this.OriRefreshTimeTextBox.Location = new System.Drawing.Point(28, 30);
            this.OriRefreshTimeTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OriRefreshTimeTextBox.Name = "OriRefreshTimeTextBox";
            this.OriRefreshTimeTextBox.Size = new System.Drawing.Size(62, 28);
            this.OriRefreshTimeTextBox.TabIndex = 0;
            // 
            // SaveDataButton
            // 
            this.SaveDataButton.Location = new System.Drawing.Point(753, 534);
            this.SaveDataButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SaveDataButton.Name = "SaveDataButton";
            this.SaveDataButton.Size = new System.Drawing.Size(112, 34);
            this.SaveDataButton.TabIndex = 5;
            this.SaveDataButton.Text = "保存";
            this.SaveDataButton.UseVisualStyleBackColor = true;
            this.SaveDataButton.Click += new System.EventHandler(this.SaveDataButton_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_ItemClicked);
            // 
            // UeSilenceCheckTimerText
            // 
            this.UeSilenceCheckTimerText.Location = new System.Drawing.Point(28, 26);
            this.UeSilenceCheckTimerText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.UeSilenceCheckTimerText.Name = "UeSilenceCheckTimerText";
            this.UeSilenceCheckTimerText.Size = new System.Drawing.Size(62, 28);
            this.UeSilenceCheckTimerText.TabIndex = 7;
            // 
            // RxAntNumText
            // 
            this.RxAntNumText.Location = new System.Drawing.Point(32, 26);
            this.RxAntNumText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RxAntNumText.Name = "RxAntNumText";
            this.RxAntNumText.Size = new System.Drawing.Size(86, 28);
            this.RxAntNumText.TabIndex = 9;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label9);
            this.groupBox12.Controls.Add(this.UeSilenceCheckTimerText);
            this.groupBox12.Location = new System.Drawing.Point(624, 452);
            this.groupBox12.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox12.Size = new System.Drawing.Size(178, 74);
            this.groupBox12.TabIndex = 10;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "静默定时器";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(102, 30);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 18);
            this.label9.TabIndex = 8;
            this.label9.Text = "秒";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.RxAntNumText);
            this.groupBox13.Location = new System.Drawing.Point(816, 452);
            this.groupBox13.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox13.Size = new System.Drawing.Size(180, 74);
            this.groupBox13.TabIndex = 11;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "接收天线数";
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.CenterCodeText);
            this.groupBox14.Location = new System.Drawing.Point(192, 32);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(190, 78);
            this.groupBox14.TabIndex = 5;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "短信中心";
            // 
            // CenterCodeText
            // 
            this.CenterCodeText.Location = new System.Drawing.Point(8, 34);
            this.CenterCodeText.Name = "CenterCodeText";
            this.CenterCodeText.Size = new System.Drawing.Size(176, 28);
            this.CenterCodeText.TabIndex = 0;
            // 
            // SystemConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 582);
            this.Controls.Add(this.groupBox13);
            this.Controls.Add(this.groupBox12);
            this.Controls.Add(this.SaveDataButton);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SystemConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统配置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SystemConfig_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox OPComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView SearchListView;
        private System.Windows.Forms.ColumnHeader 序号;
        private System.Windows.Forms.ColumnHeader 主频;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.TextBox freqBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox BandTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SendMessageTimeTextBox;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox SendPingTimeTextBox;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TextBox MessageTextBox;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox OriRefreshTimeTextBox;
        private System.Windows.Forms.Button SaveDataButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox SINRThresholdText;
        private System.Windows.Forms.TextBox earfcnBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox UeSilenceCheckTimerText;
        private System.Windows.Forms.ColumnHeader EARFCN;
        private System.Windows.Forms.TextBox RxAntNumText;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox smsModCB;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.TextBox CenterCodeText;
    }
}