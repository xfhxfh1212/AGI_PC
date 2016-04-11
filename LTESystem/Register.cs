using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using COM.ZCTT.AGI.Common;

namespace LTESystem
{
    public partial class Register : Form
    {
        private static string LoginFilePath = "";
        private static FileStream fs = null;
        

        public Register()
        {
            InitializeComponent();
        }

        private void registButton_Click(object sender, EventArgs e)
        {
            if (Global.userInfoDic.ContainsKey(this.userText.Text))
            {
                MessageBox.Show("该用户名已被注册！");
                return;
            }

            if(String.IsNullOrWhiteSpace(this.userText.Text)
                || String.IsNullOrWhiteSpace(this.pwText.Text))
            {
                MessageBox.Show("用户名或密码不可为空！");
                return;
            }

            if(this.pwText.Text==this.pwConText.Text)
            {
                try
                {
                    fs = new FileStream(LoginFilePath, FileMode.Append);
                    Global.userInfo info = new Global.userInfo();
                    info.UserName = this.userText.Text;
                    info.PassWord = this.pwText.Text;

                    BinaryWriter bw = new BinaryWriter(fs);
                    //bw.Seek(0, SeekOrigin.End);
                    bw.Write((String)MySecurity.EncryptString(info.UserName));
                    bw.Write((String)MySecurity.EncryptString(info.PassWord));
                    bw.Flush();

                    bw.Close();
                    fs.Close();
                    fs = null;

                    Global.userInfoDic.Add(info.UserName, info.PassWord);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(">>>Message=" + ex.Message + "\r\nStacktrace:" + ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("两次密码不一致！");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Register_Load(object sender, EventArgs e)
        {
            this.AcceptButton = this.registButton;
            LoginFilePath = System.Windows.Forms.Application.StartupPath + "\\login.bin";
        }

    }
}
