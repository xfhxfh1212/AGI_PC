namespace LTESystem
{
    partial class Login
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.userLabel = new System.Windows.Forms.Label();
            this.userText = new System.Windows.Forms.TextBox();
            this.pwText = new System.Windows.Forms.TextBox();
            this.pwLabel = new System.Windows.Forms.Label();
            this.logInButton = new System.Windows.Forms.Button();
            this.registButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // userLabel
            // 
            this.userLabel.AutoSize = true;
            this.userLabel.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userLabel.Location = new System.Drawing.Point(23, 77);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(66, 19);
            this.userLabel.TabIndex = 0;
            this.userLabel.Text = "用户名";
            // 
            // userText
            // 
            this.userText.Location = new System.Drawing.Point(109, 74);
            this.userText.Name = "userText";
            this.userText.Size = new System.Drawing.Size(120, 21);
            this.userText.TabIndex = 1;
            // 
            // pwText
            // 
            this.pwText.Location = new System.Drawing.Point(109, 128);
            this.pwText.Name = "pwText";
            this.pwText.PasswordChar = '*';
            this.pwText.Size = new System.Drawing.Size(120, 21);
            this.pwText.TabIndex = 3;
            // 
            // pwLabel
            // 
            this.pwLabel.AutoSize = true;
            this.pwLabel.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pwLabel.Location = new System.Drawing.Point(23, 130);
            this.pwLabel.Name = "pwLabel";
            this.pwLabel.Size = new System.Drawing.Size(67, 19);
            this.pwLabel.TabIndex = 2;
            this.pwLabel.Text = "密  码";
            // 
            // logInButton
            // 
            this.logInButton.Location = new System.Drawing.Point(37, 214);
            this.logInButton.Name = "logInButton";
            this.logInButton.Size = new System.Drawing.Size(75, 23);
            this.logInButton.TabIndex = 4;
            this.logInButton.Text = "登录";
            this.logInButton.UseVisualStyleBackColor = true;
            this.logInButton.Click += new System.EventHandler(this.logInButton_Click);
            // 
            // registButton
            // 
            this.registButton.Location = new System.Drawing.Point(154, 214);
            this.registButton.Name = "registButton";
            this.registButton.Size = new System.Drawing.Size(75, 23);
            this.registButton.TabIndex = 5;
            this.registButton.Text = "注册";
            this.registButton.UseVisualStyleBackColor = true;
            this.registButton.Click += new System.EventHandler(this.registButton_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(154, 175);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "保存用户";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.registButton);
            this.Controls.Add(this.logInButton);
            this.Controls.Add(this.pwText);
            this.Controls.Add(this.pwLabel);
            this.Controls.Add(this.userText);
            this.Controls.Add(this.userLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.TextBox userText;
        private System.Windows.Forms.TextBox pwText;
        private System.Windows.Forms.Label pwLabel;
        private System.Windows.Forms.Button logInButton;
        private System.Windows.Forms.Button registButton;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;
    }
}

