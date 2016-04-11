using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using COM.ZCTT.AGI;
using COM.ZCTT.AGI.Common;
using AGIInterface;

namespace LTESystem
{
    public partial class Login : Form
    {
        private static string LoginFilePath = "";
        private static string LoginSaveFilePath = "";
        private static FileStream fs = null;
        

        public Login()
        {
            InitializeComponent();
            checkBox1.Checked = true;
        }

        
        private void logInButton_Click(object sender, EventArgs e)
        {
            if (Global.userInfoDic.ContainsKey(this.userText.Text) && Global.userInfoDic[this.userText.Text] == pwText.Text)
            {
                frmLTESystem frmLS = new frmLTESystem();
                frmLS.Show();
                this.Hide();
                if (checkBox1.Checked == true)
                {
                    fs = new FileStream(LoginSaveFilePath, FileMode.Create);
                    //Global.userInfo info = new Global.userInfo();
                    String UserName = this.userText.Text;
                    String PassWord = this.pwText.Text;

                    BinaryWriter bw = new BinaryWriter(fs);
                    //bw.Seek(0, SeekOrigin.End);
                    bw.Write((String)MySecurity.EncryptString(UserName));
                    bw.Write((String)MySecurity.EncryptString(PassWord));
                    bw.Flush();

                    bw.Close();
                    fs.Close();
                    fs = null;
                }
                else
                {
                    File.Delete(LoginSaveFilePath);
                }
            }
            else
            {
                MessageBox.Show("用户名或密码错误！");
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.AcceptButton = this.logInButton;
            LoginFilePath = System.Windows.Forms.Application.StartupPath + "\\login.bin";
            LoginSaveFilePath = System.Windows.Forms.Application.StartupPath + "\\loginsave.bin";

            //如果文件不存在,则新建文件
            if(!File.Exists(LoginFilePath))
            {
                this.logInButton.Enabled = false;
                fs = new FileStream(LoginFilePath, FileMode.Create);
                fs.Close();
                fs = null;
            }
            if (File.Exists(LoginSaveFilePath))
            {
                try
                {
                    fs = new FileStream(LoginSaveFilePath, FileMode.Open);
                    BinaryReader br = new BinaryReader(fs);

                    //while (br.PeekChar() > 0)
                    //{
                    //Global.userInfo info = new Global.userInfo();
                    userText.Text = MySecurity.DecryptTextFromMemory(br.ReadString());
                    pwText.Text = MySecurity.DecryptTextFromMemory(br.ReadString());
                    //Global.userInfoDic.Add(info.UserName, info.PassWord);
                    //}

                    br.Close();
                    fs.Close();
                    fs = null;

                }
                catch (Exception ex)
                {
                    fs.Close();
                    fs = null;
                    Console.WriteLine(">>>Message=" + ex.Message + "\r\nStacktrace:" + ex.StackTrace);
                }
            }
            
            ReadUserInfo();
        }

        private void registButton_Click(object sender, EventArgs e)
        {
            Register frmReg = new Register();
            if(frmReg.ShowDialog()==DialogResult.OK)
            {
                this.logInButton.Enabled = true;
            }
        }

        /// <summary>
        /// 读取本地文件中用户信息
        /// </summary>
        private void ReadUserInfo()
        {
            try
            {
                fs = new FileStream(LoginFilePath, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);

                while (br.PeekChar() > 0)
                {
                    Global.userInfo info = new Global.userInfo();
                    info.UserName = MySecurity.DecryptTextFromMemory(br.ReadString());
                    info.PassWord = MySecurity.DecryptTextFromMemory(br.ReadString());
                    Global.userInfoDic.Add(info.UserName, info.PassWord);
                }
               
                br.Close();
                fs.Close();
                fs = null;

            }
            catch (Exception ex)
            {
                fs.Close();
                fs = null;
                Console.WriteLine(">>>Message=" + ex.Message + "\r\nStacktrace:" + ex.StackTrace);
            }
            
        }

       
    }
}
