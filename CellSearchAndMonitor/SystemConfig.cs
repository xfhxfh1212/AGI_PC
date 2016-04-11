using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using AGIInterface;
using COM.ZCTT.AGI.Common;
using System.Xml.Serialization;
using System.Xml;
namespace CellSearchAndMonitor
{
    public partial class SystemConfig : DockContent, COM.ZCTT.AGI.Plugin.IPlugin
    {
        private static string FileName = "";
        public SystemConfig()
        {
            InitializeComponent();
        }

        static bool EventLoad = false;
        #region 实现IPlugin接口
        private static SystemConfig SysConfig;
        
        /// <summary>
        /// 初始化该模块，实现了IPlugin接口的load方法
        /// </summary>
        public new void Load()
        {
            if (SysConfig == null || SysConfig.IsDisposed)
            {
                SysConfig = new SystemConfig();
            }

            FileName = System.Windows.Forms.Application.StartupPath + "\\SystemConfig.xml";

            if (EventLoad == false)
            {
                Global.tempClass.StartEvent += new Class1.StartHandler(StartEvent);
                Global.tempClass.StopEvent += new Class1.StopHandler(StopEvent);
                EventLoad = true;
                //IsConfiged = false;
            }
            if (CellSearchAndMonitorUnions.OPCellSearchInfoDic.Count == 0)
            {
                List<CellSearchAndMonitorUnions.BandAndEarfcn> CMCCList = new List<CellSearchAndMonitorUnions.BandAndEarfcn>();
                CellSearchAndMonitorUnions.BandAndEarfcn info = new CellSearchAndMonitorUnions.BandAndEarfcn();
                info.Band = "38"; info.Freq = "2585"; info.Earfcn = "37900"; CMCCList.Add(info);
                info.Band = "38"; info.Freq = "2605"; info.Earfcn = "38100"; CMCCList.Add(info);
                info.Band = "39"; info.Freq = "1890"; info.Earfcn = "38350"; CMCCList.Add(info);
                info.Band = "40"; info.Freq = "2330"; info.Earfcn = "38950"; CMCCList.Add(info);
                CellSearchAndMonitorUnions.OPCellSearchInfoDic.Add("中国移动", CMCCList);
                CellSearchAndMonitorUnions.OPCellSearchInfoDic.Add("中国联通", new List<CellSearchAndMonitorUnions.BandAndEarfcn>());
                CellSearchAndMonitorUnions.OPCellSearchInfoDic.Add("中国电信", new List<CellSearchAndMonitorUnions.BandAndEarfcn>());
            }
            if (LoadFile(FileName) != true)
            {
                SysConfig.smsModCB.Text = "1";
                SysConfig.SendMessageTimeTextBox.Text = "2";
                SysConfig.SendPingTimeTextBox.Text = "3";
                SysConfig.MessageTextBox.Text = "test";
                SysConfig.OriRefreshTimeTextBox.Text = "6";
                SysConfig.SINRThresholdText.Text = "-5";
                SysConfig.UeSilenceCheckTimerText.Text = "18";
                SysConfig.RxAntNumText.Text = "2";
            }
            if (SysConfig.smsModCB.Text == "明短信")
                CellSearchAndMonitorUnions.SmsModCB = 3;
            else
                CellSearchAndMonitorUnions.SmsModCB = Convert.ToInt32(SysConfig.smsModCB.Text);
            CellSearchAndMonitorUnions.CenterCode = SysConfig.CenterCodeText.Text;
            CellSearchAndMonitorUnions.TestMessageSendTime = 1000 * Convert.ToInt32(SysConfig.SendMessageTimeTextBox.Text);
            CellSearchAndMonitorUnions.PingTime = 1000 * Convert.ToInt32(SysConfig.SendPingTimeTextBox.Text);
            CellSearchAndMonitorUnions.TestMessage = SysConfig.MessageTextBox.Text;
            CellSearchAndMonitorUnions.OriRefreshTime = 1000 * Convert.ToInt32(SysConfig.OriRefreshTimeTextBox.Text);
            CellSearchAndMonitorUnions.TestSINRThreshold = Convert.ToInt32(SysConfig.SINRThresholdText.Text);
            CellSearchAndMonitorUnions.UeSilenceCheckTimer = Convert.ToUInt32(SysConfig.UeSilenceCheckTimerText.Text);
            CellSearchAndMonitorUnions.RxAntNum = Convert.ToByte(SysConfig.RxAntNumText.Text);
            if(SysConfig.OPComboBox.Items.Count==0)
            {
                SysConfig.OPComboBox.Items.Add("中国移动");
                OPComboBoxLast = "中国移动";
                SysConfig.OPComboBox.Items.Add("中国联通");
                SysConfig.OPComboBox.Items.Add("中国电信");
            }
            SysConfig.OPComboBox.SelectedItem = SysConfig.OPComboBox.Items[0];
            SysConfig.ShowDialog();
            return;
        }
        public void newLoad()
        {
            if (SysConfig == null || SysConfig.IsDisposed)
            {
                SysConfig = new SystemConfig();
            }
            FileName = System.Windows.Forms.Application.StartupPath + "\\SystemConfig.xml";

            if (EventLoad == false)
            {
                Global.tempClass.StartEvent += new Class1.StartHandler(StartEvent);
                Global.tempClass.StopEvent += new Class1.StopHandler(StopEvent);
                EventLoad = true;
                //IsConfiged = false;
            }
            if (CellSearchAndMonitorUnions.OPCellSearchInfoDic.Count == 0)
            {
                List<CellSearchAndMonitorUnions.BandAndEarfcn> CMCCList = new List<CellSearchAndMonitorUnions.BandAndEarfcn>();
                CellSearchAndMonitorUnions.BandAndEarfcn info = new CellSearchAndMonitorUnions.BandAndEarfcn();
                info.Band = "38"; info.Freq = "2585"; info.Earfcn = "37900"; CMCCList.Add(info);
                info.Band = "38"; info.Freq = "2605"; info.Earfcn = "38100"; CMCCList.Add(info);
                info.Band = "39"; info.Freq = "1895"; info.Earfcn = "38400"; CMCCList.Add(info);
                info.Band = "40"; info.Freq = "2330"; info.Earfcn = "38950"; CMCCList.Add(info);
                CellSearchAndMonitorUnions.OPCellSearchInfoDic.Add("中国移动", CMCCList);
                CellSearchAndMonitorUnions.OPCellSearchInfoDic.Add("中国联通", new List<CellSearchAndMonitorUnions.BandAndEarfcn>());
                CellSearchAndMonitorUnions.OPCellSearchInfoDic.Add("中国电信", new List<CellSearchAndMonitorUnions.BandAndEarfcn>());
            }
            if (LoadFile(FileName) != true)
            {
                SysConfig.smsModCB.Text = "1";
                SysConfig.CenterCodeText.Text = "13800100500";
                SysConfig.SendMessageTimeTextBox.Text = "2";
                SysConfig.SendPingTimeTextBox.Text = "3";
                SysConfig.MessageTextBox.Text = "test";
                SysConfig.OriRefreshTimeTextBox.Text = "6";
                SysConfig.SINRThresholdText.Text = "-5";
                SysConfig.UeSilenceCheckTimerText.Text = "18";
                SysConfig.RxAntNumText.Text = "2";
            }
            if (SysConfig.smsModCB.Text == "明短信")
                CellSearchAndMonitorUnions.SmsModCB = 3;
            else
                CellSearchAndMonitorUnions.SmsModCB = Convert.ToInt32(SysConfig.smsModCB.Text);
            CellSearchAndMonitorUnions.CenterCode = SysConfig.CenterCodeText.Text;
            CellSearchAndMonitorUnions.TestMessageSendTime = 1000 * Convert.ToInt32(SysConfig.SendMessageTimeTextBox.Text);
            CellSearchAndMonitorUnions.PingTime = 1000 * Convert.ToInt32(SysConfig.SendPingTimeTextBox.Text);
            CellSearchAndMonitorUnions.TestMessage = SysConfig.MessageTextBox.Text;
            CellSearchAndMonitorUnions.OriRefreshTime = 1000 * Convert.ToInt32(SysConfig.OriRefreshTimeTextBox.Text);
            CellSearchAndMonitorUnions.TestSINRThreshold = Convert.ToInt32(SysConfig.SINRThresholdText.Text);
            CellSearchAndMonitorUnions.UeSilenceCheckTimer = Convert.ToUInt32(SysConfig.UeSilenceCheckTimerText.Text);
            CellSearchAndMonitorUnions.RxAntNum = Convert.ToByte(SysConfig.RxAntNumText.Text);
            if (SysConfig.OPComboBox.Items.Count == 0)
            {
                SysConfig.OPComboBox.Items.Add("中国移动");
                OPComboBoxLast = "中国移动";
                SysConfig.OPComboBox.Items.Add("中国联通");
                SysConfig.OPComboBox.Items.Add("中国电信");
            }
            SysConfig.OPComboBox.SelectedItem = SysConfig.OPComboBox.Items[0];
        }
        /// <summary>
        /// 开始“协议跟踪”事件绑定的方法
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="ex">自定义的事件参数</param>
        public void StartEvent(object sender, CustomDataEvtArg ex)
        {
            if (ex.moduleName == "ProtocolTrace"
                &&
                ex.isRealTime == true)
            {
                //CustomDataEvtArg cusArg = new CustomDataEvtArg();
                //cusArg.data = Global.ProTracReq;
                //cusArg.deivceName = Global.GCurrentDevice;
                //Global.tempClass.SendDataToDevice(sender, cusArg);
                //Global.FirstTime = true;
            }
        }

        /// <summary>
        /// 停止“协议跟踪”事件绑定的方法
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="ex">自定义的事件参数</param>
        public void StopEvent(object sender, CustomDataEvtArg ex)
        {
            if (ex.moduleName == "ProtocolTrace")
            {
                //AGIInterface.CustomDataEvtArg cusArg = new CustomDataEvtArg();
                //cusArg.data = new byte[] { 0, 0, 0, 0, 4, 2, 0x0c, 0x40, 1, 0, 0, 0 };//加消息头。。
                //cusArg.deivceName = Global.GCurrentDevice;
                //Global.tempClass.SendDataToDevice(sender, cusArg);
            }
        }
        #endregion

        

        private void AddButton_Click(object sender, EventArgs e)
        {
            ListViewItem LVItem = new ListViewItem(new string[3]);

            LVItem.SubItems[0].Text = SysConfig.BandTextBox.Text;
            LVItem.SubItems[1].Text = SysConfig.freqBox.Text;
            LVItem.SubItems[2].Text = SysConfig.earfcnBox.Text;
            SysConfig.SearchListView.Items.Add(LVItem);
        }

        private static string OPComboBoxLast;
        private void OPComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OPComboBoxLast != SysConfig.OPComboBox.Text) 
            {
                CellSearchAndMonitorUnions.OPCellSearchInfoDic[OPComboBoxLast].Clear();
                foreach (ListViewItem LVItem in SysConfig.SearchListView.Items)
                {
                    CellSearchAndMonitorUnions.BandAndEarfcn info = new CellSearchAndMonitorUnions.BandAndEarfcn();
                    info.Band = LVItem.SubItems[0].Text;
                    info.Freq = LVItem.SubItems[1].Text;
                    info.Earfcn = LVItem.SubItems[2].Text;
                    CellSearchAndMonitorUnions.OPCellSearchInfoDic[OPComboBoxLast].Add(info);
                }

                OPComboBoxLast = SysConfig.OPComboBox.Text;
            }
            
            SysConfig.SearchListView.Items.Clear();
            foreach (var info in CellSearchAndMonitorUnions.OPCellSearchInfoDic[SysConfig.OPComboBox.Text])
            {
                ListViewItem LVItem = new ListViewItem(new string[3]);

                LVItem.SubItems[0].Text = info.Band;
                LVItem.SubItems[1].Text = info.Freq;
                LVItem.SubItems[2].Text = info.Earfcn;

                SysConfig.SearchListView.Items.Add(LVItem);
            }
        }

        XmlDocument xmldoc;
        XmlElement xmlelem;
        /// <summary>
        /// 导入文件时需要调用的函数
        /// </summary>
        /// <param name="filepath">需要导入的文件路径</param>
        /// <returns>如果导入成功返回true；导入文件错误返回false</returns>
        private bool LoadFile(string filepath)
        {
            try
            {
                xmldoc = new XmlDocument();
                xmldoc.Load(filepath);
                XmlNode root1 = xmldoc.SelectSingleNode("//Config");

                foreach (XmlNode nd in root1.ChildNodes)
                {
                    switch (nd.Name)
                    {
                        case "SmsModCB":
                            SysConfig.smsModCB.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "CenterCode":
                            SysConfig.CenterCodeText.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "TestMessageSendTime":
                            SysConfig.SendMessageTimeTextBox.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "PingTime":
                            SysConfig.SendPingTimeTextBox.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "TestMessage":
                            SysConfig.MessageTextBox.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "OriRefreshTime":
                            SysConfig.OriRefreshTimeTextBox.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "SINRThreshold":
                            SysConfig.SINRThresholdText.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "UeSilenceCheckTimer":
                            SysConfig.UeSilenceCheckTimerText.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "RxAntNum":
                            SysConfig.RxAntNumText.Text = nd.Attributes["Value"].InnerText;
                            break;
                        case "OPInformation":
                            foreach(XmlNode child in nd.ChildNodes)
                            {
                                string op = "中国移动";
                                switch(child.Name)
                                {
                                    case "CMCC":
                                        op = "中国移动";
                                        break;
                                    case "CUCC":
                                        op = "中国联通";
                                        break;
                                    case "CTCC":
                                        op = "中国电信";
                                        break;
                                    default:
                                        break;
                                }
                                
                                CellSearchAndMonitorUnions.OPCellSearchInfoDic[op].Clear();
                                foreach (XmlNode cchild in child.ChildNodes)
                                {
                                    CellSearchAndMonitorUnions.BandAndEarfcn info = new CellSearchAndMonitorUnions.BandAndEarfcn();
                                    foreach (XmlNode ccchild in cchild.ChildNodes)
                                    {
                                        switch (ccchild.Name)
                                        {
                                            case "Band":
                                                info.Band = ccchild.Attributes["Value"].InnerText;
                                                break;
                                            case "Freq":
                                                info.Freq = ccchild.Attributes["Value"].InnerText;
                                                break;
                                            case "Earfcn":
                                                info.Earfcn = ccchild.Attributes["Value"].InnerText;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    CellSearchAndMonitorUnions.OPCellSearchInfoDic[op].Add(info);

                                }
                                    
                            }
                            break;
                        default:
                            break;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            SysConfig.SearchListView.Items.Remove(SysConfig.SearchListView.FocusedItem);
        }

        private void SearchListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SysConfig.contextMenuStrip.Items.Clear();
                contextMenuStrip.Items.Add("删除");
                SysConfig.contextMenuStrip.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void SaveDataButton_Click(object sender, EventArgs e)
        {
            if (!CellSearchAndMonitorUnions.IsPhoneNum(SysConfig.CenterCodeText.Text))
            {
                MessageBox.Show("请填入正确的短信中心号码！");
                return;
            }
            SysConfig.Close();
            SysConfig.Dispose();
        }

        private void SaveData()
        {
            OPComboBoxLast = SysConfig.OPComboBox.Text;
            CellSearchAndMonitorUnions.OPCellSearchInfoDic[OPComboBoxLast].Clear();
            foreach (ListViewItem LVItem in SysConfig.SearchListView.Items)
            {
                CellSearchAndMonitorUnions.BandAndEarfcn info = new CellSearchAndMonitorUnions.BandAndEarfcn();
                info.Band = LVItem.SubItems[0].Text;
                info.Freq = LVItem.SubItems[1].Text;
                info.Earfcn = LVItem.SubItems[2].Text;
                CellSearchAndMonitorUnions.OPCellSearchInfoDic[OPComboBoxLast].Add(info);
            }
            if (SysConfig.smsModCB.Text == "明短信")
                CellSearchAndMonitorUnions.SmsModCB = 3;
            else
                CellSearchAndMonitorUnions.SmsModCB = Convert.ToInt32(SysConfig.smsModCB.Text);
            CellSearchAndMonitorUnions.CenterCode = SysConfig.CenterCodeText.Text;
            CellSearchAndMonitorUnions.TestMessageSendTime = 1000 * Convert.ToInt32(SysConfig.SendMessageTimeTextBox.Text);
            CellSearchAndMonitorUnions.PingTime = 1000 * Convert.ToInt32(SysConfig.SendPingTimeTextBox.Text);
            CellSearchAndMonitorUnions.TestMessage = SysConfig.MessageTextBox.Text;
            CellSearchAndMonitorUnions.OriRefreshTime = 1000 * Convert.ToInt32(SysConfig.OriRefreshTimeTextBox.Text);
            CellSearchAndMonitorUnions.TestSINRThreshold = Convert.ToInt32(SysConfig.SINRThresholdText.Text);
            CellSearchAndMonitorUnions.UeSilenceCheckTimer = Convert.ToUInt32(SysConfig.UeSilenceCheckTimerText.Text);
            CellSearchAndMonitorUnions.RxAntNum = Convert.ToByte(SysConfig.RxAntNumText.Text);
            try
            {
                xmldoc = new XmlDocument();
                XmlDeclaration xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmldoc.AppendChild(xmldecl);
                
                //加入一个根元素
                xmlelem = xmldoc.CreateElement("", "SystemConfig", "");
                xmldoc.AppendChild(xmlelem);
                XmlNode node = xmldoc.SelectSingleNode("SystemConfig");//查找<PositionAndSize> 

                //创建一个<Position>节点 
                XmlElement xe1 = xmldoc.CreateElement("Config");

                XmlElement xeSub = xmldoc.CreateElement("SmsModCB");
                xeSub.SetAttribute("Value", SysConfig.smsModCB.Text);
                xe1.AppendChild(xeSub);

                xeSub = xmldoc.CreateElement("CenterCode");
                xeSub.SetAttribute("Value", SysConfig.CenterCodeText.Text);
                xe1.AppendChild(xeSub);

                xeSub = xmldoc.CreateElement("TestMessageSendTime");
                xeSub.SetAttribute("Value", SysConfig.SendMessageTimeTextBox.Text);
                xe1.AppendChild(xeSub);

                xeSub = xmldoc.CreateElement("PingTime");
                xeSub.SetAttribute("Value", SysConfig.SendPingTimeTextBox.Text);
                xe1.AppendChild(xeSub);

                xeSub = xmldoc.CreateElement("TestMessage");
                xeSub.SetAttribute("Value", SysConfig.MessageTextBox.Text);
                xe1.AppendChild(xeSub);

                xeSub = xmldoc.CreateElement("OriRefreshTime");
                xeSub.SetAttribute("Value", SysConfig.OriRefreshTimeTextBox.Text);
                xe1.AppendChild(xeSub);

                xeSub = xmldoc.CreateElement("SINRThreshold");
                xeSub.SetAttribute("Value", SysConfig.SINRThresholdText.Text);
                xe1.AppendChild(xeSub);

                xeSub = xmldoc.CreateElement("UeSilenceCheckTimer");
                xeSub.SetAttribute("Value", SysConfig.UeSilenceCheckTimerText.Text);
                xe1.AppendChild(xeSub);

                xeSub = xmldoc.CreateElement("RxAntNum");
                xeSub.SetAttribute("Value", SysConfig.RxAntNumText.Text);
                xe1.AppendChild(xeSub);
                #region 运营商信息存储
                xeSub = xmldoc.CreateElement("OPInformation");

                foreach (var op in CellSearchAndMonitorUnions.OPCellSearchInfoDic.Keys)
                {
                    XmlElement xeSubSub = xmldoc.CreateElement("CMCC");
                    switch (op)
                    {
                        case "中国移动":
                            xeSubSub = xmldoc.CreateElement("CMCC");
                            break;
                        case "中国联通":
                            xeSubSub = xmldoc.CreateElement("CUCC");
                            break;
                        case "中国电信":
                            xeSubSub = xmldoc.CreateElement("CTCC");
                            break;
                        default:
                            break;

                    }
                    foreach (var info in CellSearchAndMonitorUnions.OPCellSearchInfoDic[op])
                    {
                        XmlElement xeSubSubSub = xmldoc.CreateElement("Item");

                        XmlElement xeSubSubSubSub = xmldoc.CreateElement("Band");
                        xeSubSubSubSub.SetAttribute("Value", info.Band);
                        xeSubSubSub.AppendChild(xeSubSubSubSub);

                        xeSubSubSubSub = xmldoc.CreateElement("Freq");
                        xeSubSubSubSub.SetAttribute("Value", info.Freq);
                        xeSubSubSub.AppendChild(xeSubSubSubSub);

                        xeSubSubSubSub = xmldoc.CreateElement("Earfcn");
                        xeSubSubSubSub.SetAttribute("Value", info.Earfcn);
                        xeSubSubSub.AppendChild(xeSubSubSubSub);

                        xeSubSub.AppendChild(xeSubSubSub);
                    }
                    xeSub.AppendChild(xeSubSub);
                }
                xe1.AppendChild(xeSub);
                #endregion

                node.AppendChild(xe1);
                
                xmldoc.Save(FileName);
            }
            catch
            {

            }
        }

        private void SystemConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveData();
        }

        private void BandTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            String Freq = freqBox.Text.Trim();
            String Band = BandTextBox.Text.Trim();
            if (CellSearchAndMonitorUnions.IsIntOrDoubleString(Freq) && CellSearchAndMonitorUnions.IsDigitalString(Band))
            {
                int band = Convert.ToUInt16(Band);
                double freq = Convert.ToDouble(Freq);
                int earfcn = Global.FreqToEARFCN(band, freq);
                earfcnBox.Text = earfcn.ToString();
            }
        }

        private void freqBox_KeyUp(object sender, KeyEventArgs e)
        {
            String Freq = freqBox.Text.Trim();
            String Band = BandTextBox.Text.Trim();
            if (CellSearchAndMonitorUnions.IsIntOrDoubleString(Freq) && CellSearchAndMonitorUnions.IsDigitalString(Band))
            {
                int band = Convert.ToUInt16(Band);
                double freq = Convert.ToDouble(Freq);
                int earfcn = Global.FreqToEARFCN(band, freq);
                earfcnBox.Text = earfcn.ToString();
            }
        }

        private void earfcnBox_KeyUp(object sender, KeyEventArgs e)
        {
            String Earfcn = earfcnBox.Text.Trim();
            if (CellSearchAndMonitorUnions.IsDigitalString(Earfcn))
            {
                int earfcn = Convert.ToInt32(Earfcn);
                double freq = Global.EARFCNToFreq(earfcn);
                int band = Global.EARFCNToBand(earfcn);
                freqBox.Text = freq.ToString();
                BandTextBox.Text = band.ToString();
            }
        }
    }
}
