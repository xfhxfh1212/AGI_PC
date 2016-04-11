namespace LTESystem
{
    partial class Register
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
            this.pwText = new System.Windows.Forms.TextBox();
            this.pwLabel = new System.Windows.Forms.Label();
            this.userText = new System.Windows.Forms.TextBox();
            this.userLabel = new System.Windows.Forms.Label();
            this.pwConText = new System.Windows.Forms.TextBox();
            this.pwConLabel = new System.Windows.Forms.Label();
            this.registButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pwText
            // 
            this.pwText.Location = new System.Drawing.Point(123, 95);
            this.pwText.Name = "pwText";
            this.pwText.PasswordChar = '*';
            this.pwText.Size = new System.Drawing.Size(120, 21);
            this.pwText.TabIndex = 7;
            // 
            // pwLabel
            // 
            this.pwLabel.AutoSize = true;
            this.pwLabel.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pwLabel.Location = new System.Drawing.Point(16, 97);
            this.pwLabel.Name = "pwLabel";
            this.pwLabel.Size = new System.Drawing.Size(87, 19);
            this.pwLabel.TabIndex = 6;
            this.pwLabel.Text = "密    码";
            // 
            // userText
            // 
            this.userText.Location = new System.Drawing.Point(123, 41);
            this.userText.Name = "userText";
            this.userText.Size = new System.Drawing.Size(120, 21);
            this.userText.TabIndex = 5;
            // 
            // userLabel
            // 
            this.userLabel.AutoSize = true;
            this.userLabel.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userLabel.Location = new System.Drawing.Point(18, 43);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(86, 19);
            this.userLabel.TabIndex = 4;
            this.userLabel.Text = "用 户 名";
            // 
            // pwConText
            // 
            this.pwConText.Location = new System.Drawing.Point(122, 146);
            this.pwConText.Name = "pwConText";
            this.pwConText.PasswordChar = '*';
            this.pwConText.Size = new System.Drawing.Size(120, 21);
            this.pwConText.TabIndex = 9;
            // 
            // pwConLabel
            // 
            this.pwConLabel.AutoSize = true;
            this.pwConLabel.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pwConLabel.Location = new System.Drawing.Point(18, 148);
            this.pwConLabel.Name = "pwConLabel";
            this.pwConLabel.Size = new System.Drawing.Size(85, 19);
            this.pwConLabel.TabIndex = 8;
            this.pwConLabel.Text = "确认密码";
            // 
            // registButton
            // 
            this.registButton.Location = new System.Drawing.Point(100, 206);
            this.registButton.Name = "registButton";
            this.registButton.Size = new System.Drawing.Size(75, 23);
            this.registButton.TabIndex = 10;
            this.registButton.Text = "注册";
            this.registButton.UseVisualStyleBackColor = true;
            this.registButton.Click += new System.EventHandler(this.registButton_Click);
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.registButton);
            this.Controls.Add(this.pwConText);
            this.Controls.Add(this.pwConLabel);
            this.Controls.Add(this.pwText);
            this.Controls.Add(this.pwLabel);
            this.Controls.Add(this.userText);
            this.Controls.Add(this.userLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Register";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "注册";
            this.Load += new System.EventHandler(this.Register_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pwText;
        private System.Windows.Forms.Label pwLabel;
        private System.Windows.Forms.TextBox userText;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.TextBox pwConText;
        private System.Windows.Forms.Label pwConLabel;
        private System.Windows.Forms.Button registButton;
    }
}