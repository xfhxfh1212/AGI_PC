using COM.ZCTT.AGI.Plugin;
using Plugin;
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
using DeviceMangerModule;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Net.NetworkInformation;

namespace CellSearchAndMonitor
{
    public partial class CellSearchAndMonitor : WeifenLuo.WinFormsUI.Docking.DockContent, COM.ZCTT.AGI.Plugin.IPlugin
    {

        
        static bool EventLoad = false;
        private static bool isStop = false;
        static bool CSAMRunning = false;
        public CellSearchAndMonitor()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Global.tempClass.FTSTMSIStartEvent += new Class1.FTSTMSIStartHander(CellSearchBtnUnable);
            Global.tempClass.FTSTMSIStopEvent += new Class1.FTSTMSIStopHander(CellSearchBtnEnable);
            Global.tempClass.OrtFStartEvent += new Class1.OrtFStartHander(CellSearchBtnUnable);
            Global.tempClass.OrtFStopEvent += new Class1.OrtFStopHander(CellSearchBtnEnable);
        }

        private static CellSearchAndMonitor CSAMForm;
        public DeviceManger deviceManger;
        public SystemConfig systemConfig;
        #region 实现IPlugin接口
        public new void Load()
        {
            if (deviceManger == null || deviceManger.IsDisposed)
            {
                deviceManger = new DeviceManger();
            }
            deviceManger.newLoad();
            if (systemConfig == null || systemConfig.IsDisposed)
                systemConfig = new SystemConfig();
            systemConfig.newLoad();
            if (CSAMForm == null || CSAMForm.IsDisposed)
            {
                CSAMForm = new CellSearchAndMonitor();
                CSAMForm.ShowHint = DockState.Document;
                CSAMForm.Show(GloabeControl.cellSearchDockPanel);
                CSAMForm.DockState = DockState.Document;

            }
            else
            {
                CSAMForm.ShowHint = DockState.Document;
                CSAMForm.Show(GloabeControl.cellSearchDockPanel);
                CSAMForm.DockState = DockState.Document;
                //debugDisplay.DockAreas = DockAreas.Document;
            }
            //sTmsiGraph.ShowDialog();
            if (EventLoad == false)
            {
                Global.tempClass.SendDataToProTrackEvent += this.DataReceived;
                Global.tempClass.SendACKToCellScanEvent += new Class1.DeviceSendACKToCellScan(ACKHandler);
                Global.tempClass.StartEvent += new Class1.StartHandler(StartEvent);
                Global.tempClass.CloseDisplayFormEvent += new Class1.CloseDisplayFormHander(CloseForm);

                EventLoad = true;
            }

            //Control.CheckForIllegalCrossThreadCalls = false;

            LoadData();
            if (CSAMForm.CellSearchListView.Items.Count == 0) CSAMForm.CellMonitorButton.Enabled = false;
            CSAMForm.CellStopBtn.Enabled = false;
            CellSearchAndMonitorUnions.CellMonitorLV = CSAMForm.CellMonitorListView;
            //CSAMForm.StopMonitorButton.Enabled = false;
            CSAMForm.OPComboBox.Items.Add("中国移动");
            CSAMForm.OPComboBox.Items.Add("中国联通");
            CSAMForm.OPComboBox.Items.Add("中国电信");
            CSAMForm.OPComboBox.SelectedIndex = 0;
            PingDevice();
        }

        /// <summary>
        /// 关闭当前窗口执行的事件
        /// </summary>
        private void CloseForm()
        {
            isStop = true;
            
            if (mutexRelease == 1)
            {
                ReleaseProtocolTracing();
                CSAMRunning = false;
            }
            try
            {
                if (PingThread != null)
                    PingThread.Abort();
            }
            catch { }
            foreach (Thread td in pingList)
            {
                try
                {
                    if (td != null)
                        td.Abort();
                }
                catch { }
            }
            foreach (Device dev in DeviceManger.deviceList)
            {
                if(dev.SendRexAnt)
                    dev.SendRexAnt = false;
            }
            SaveData();
            CSAMForm.Close();
            CSAMForm.Dispose(true);
        }


        /// <summary>
        /// “开始测试”执行的事件：
        /// 清空所有数据以及界面上的内容
        /// </summary>
        /// <param name="sender">sender为AGIInterface.Class1</param>
        /// <param name="e">自定义的事件参数</param>
        private void StartEvent(object sender, AGIInterface.CustomDataEvtArg e)
        {
            if (CSAMForm != null)
            {

                //读取两个表格的数据
                //sTmsiGraph.dgvDisplay.Rows.Clear();
            }
        }
        #endregion

        private static int mutexRrace = 0;
        private static int mutexRelease = 0;
        private delegate void CrossThreadOperationControl();
        public void ACKHandler(object sender, AGIInterface.CustomDataEvtArg e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Class1.DeviceSendACKToCellScan(ACKHandler), sender, e);
                }
                catch
                {

                }
            }
            else
            {
                if (Global.CurrentSender == CSAMForm.Name && e.deivceName == Global.GCurrentDevice)
                {
                    //
                    if (mutexRrace == 0 && CellSearchItemIndex != CSAMForm.CellSearchListView.Items.Count) //CellSearchItemIndex = 0;
                    {
                        Thread.Sleep(1000);
                        CellSearchItemIndex++;
                        System.Diagnostics.Debug.WriteLine("CellSearchItemIndex: " + CellSearchItemIndex + ">>>>>>>>>>>>>>>>");
                        mutexRrace = 1;
                        CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                        {
                            CSAMForm.CellIndexLabel.Text = CellSearchItemIndex.ToString();
                            if (CellSearchItemIndex != CSAMForm.CellSearchListView.Items.Count && isStop == false)
                                CSAMForm.CellSearchStart();
                        };
                        CSAMForm.Invoke(CrossThreadInfoRefresh);
                    }
                    if (CellSearchItemIndex == CSAMForm.CellSearchListView.Items.Count || isStop == true)
                    {
                        CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                        {
                            //Device temDevice = null;
                            //foreach (Device d in DeviceManger.deviceList)
                            //{
                            //    if (d.ConnectionState == 0)
                            //        temDevice = d;
                            //}
                            //DeviceManger DM = new DeviceManger();
                            //Thread RebootThread = new Thread(() => DM.RebootThreadFun(temDevice));
                            //RebootThread.Start();
                            CSAMForm.CellIndexLabel.Text = CellSearchItemIndex.ToString();
                            foreach (Device dev in DeviceManger.deviceList)
                            {
                                if (dev.SendRexAnt)
                                    dev.SendRexAnt = false;
                            }
                            isStop = false;
                            CSAMForm.CellMonitorButton.Enabled = true;
                            CSAMForm.CellStopBtn.Enabled = false;
                            Global.tempClass.CSAMStop();
                            CSAMRunning = false;
                            //Global.tempClass.FTSTMSIStop();
                        };
                        CSAMForm.Invoke(CrossThreadInfoRefresh);

                    }
                }
            }
        }
        /// <summary>
        /// 程序收到数据的处理方法
        /// </summary>
        /// <param name="sender">AGIInterface.Class1</param>
        /// <param name="e">自定义的事件参数</param>
        private bool isSecond = false;
        public void DataReceived(object sender, AGIInterface.CustomDataEvtArg e)
        {

            if (CSAMForm.InvokeRequired)
            {
                try
                {
                    if (CSAMForm != null)
                    {
                        //proTracDisplay.Invoke(new Class1.DeviceSendDataToProTrackHandler(DataReceived), sender, e);
                        CSAMForm.BeginInvoke(new Class1.DeviceSendDataToProTrackHandler(DataReceived), sender, e);
                    }
                    else
                    {                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(">>>Message= " + ex.Message + "\r\n StrackTrace: " + ex.StackTrace);
                }
            }
            else
            {
                if (Global.CurrentSender == CSAMForm.Name && e.deivceName == Global.GCurrentDevice)
                    try
                    {
                        UInt16 messageType = 0;
                        messageType = BitConverter.ToUInt16(e.data, 6);
                        UInt16 messageLength = 0;
                        messageLength = BitConverter.ToUInt16(e.data, 10);
                        //判断消息长度
                        if (messageLength * 4 != e.data.Length - 12)
                        {
                            return;
                        }

                        switch (messageType)
                        {
                            case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_CELL_CAPTURE_IND_MSG_TYPE:
                                {
                                    Dictionary<string, string> result = Decode.dataBackControl_SendMIBToDisplayEvent(e);
                                    //string tai = (BitConverter.ToUInt16(e.data, 26)).ToString();
                                    //string ecgi = (BitConverter.ToUInt32(e.data, 28)).ToString();
                                    //string rsrp = (BitConverter.ToUInt16(e.data, 32) * 0.125).ToString("f2") + "dBm";
                                    short MinRSRQ = -15;
                                    if ((result["RSRQ"] == "NULL" || Convert.ToInt16(result["RSRQ"]) < MinRSRQ) && isSecond ==false)
                                    {
                                        CellSearchItemIndex--;
                                        isSecond = true;
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>CellSearchItemIndex: " + CellSearchItemIndex);
                                        if (result["RSRP"] != "NULL")
                                            CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[4].Text = result["RSRP"] + "dBm";
                                        else
                                            CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[4].Text = "N/A";
                                        if (result["RSRP"] != "NULL")
                                            CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[5].Text = result["RSRQ"] + "dB";
                                        else
                                            CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[5].Text = "N/A";
                                        switch(result["DLBand"])
                                        {
                                            case "NULL": CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[6].Text = "N/A";break;
                                            case "0": CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[6].Text = "1.4M";break;
                                            case "1": CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[6].Text = "3M";break;
                                            case "2": CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[6].Text = "5M";break;
                                            case "3": CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[6].Text = "10M";break;
                                            case "4": CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[6].Text = "15M";break;
                                            case "5": CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[6].Text = "20M";break;
                                        }
                                        CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[7].Text = result["TAC"];
                                        CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[8].Text = result["CellID"];
                                        isSecond = false;
                                    }
                                    Thread.Sleep(2000);
                                    mutexRelease = 1;
                                    ReleaseProtocolTracing();
                                    break;

                                }

                            case COM.ZCTT.AGI.Common.AGIMsgDefine.L1_AG_PROTOCOL_TRACE_REL_ACK_MSG_TYPE:
                            case COM.ZCTT.AGI.Common.AGIMsgDefine.AG_PC_PROTOCOL_TRACE_REL_ACK_MSG_TYPE:
                                {
                                    CellSearchItemIndex++;
                                    if (CellSearchItemIndex == CSAMForm.CellSearchListView.Items.Count) CellSearchItemIndex = 0;
                                    CSAMForm.CellSearchStart();                    
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(">>>Message= " + ex.Message + "\r\n StrackTrace: " + ex.StackTrace);
                        return;
                    }
            }
        }

        private void AddToSearchListButton_Click(object sender, EventArgs e)
        {
            foreach(var info in CellSearchAndMonitorUnions.OPCellSearchInfoDic[CSAMForm.OPComboBox.Text])
            {
                string freq = info.Freq;
                string earfcn = info.Earfcn;
                string pci = CSAMForm.PCIText.Text.Trim();

                if (String.IsNullOrWhiteSpace(freq)
                    || String.IsNullOrWhiteSpace(pci)) return;
                if (!CellSearchAndMonitorUnions.IsIntOrDoubleString(freq)
                    || !CellSearchAndMonitorUnions.IsDigitalString(pci)) return;

                ListViewItem LVItem = new ListViewItem(new string[10]);

                int index = CSAMForm.CellSearchListView.Items.Count;

                LVItem.SubItems[0].Text = (index + 1).ToString();
                LVItem.SubItems[1].Text = freq;
                LVItem.SubItems[2].Text = earfcn;
                LVItem.SubItems[3].Text = pci;
                LVItem.SubItems[9].Text = CSAMForm.OPComboBox.Text;
                LVItem.Tag = info.Band;

                CSAMForm.CellSearchListView.Items.Add(LVItem);
            }
            CSAMForm.CellMonitorButton.Enabled = true;
        }
        public static List<Device> tddDevice = new List<Device>();
        public static List<Device> fddDevice = new List<Device>();
        static int tdd = 0;
        static int fdd = 0;
        private void CellSearchListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
            //联机时用
            if (DeviceManger.deviceList.Count == 0)
            {
                MessageBox.Show("无可用监控板卡！");
                return;
            }
            DeviceManger.flashDeviceConnect();

            if (DeviceManger.deviceConnect.Count == 0)
            {
                MessageBox.Show("监控板卡未连接！");
                return;
            }
            tddDevice.Clear();
            fddDevice.Clear();
            foreach (Device d in DeviceManger.deviceConnect)
            {
                if (d.LteMode == "TDD")
                    tddDevice.Add(d);
                else if (d.LteMode == "FDD")
                    fddDevice.Add(d);
            }
            tdd = 0;
            fdd = 0; 
            foreach(ListViewItem item in CSAMForm.CellMonitorListView.Items)
            {
                string earfcn = item.SubItems[2].Text;
                string lteMode = Global.EARFCNToLteMode(Convert.ToInt32(earfcn));
                if (lteMode == "TDD")
                    tdd++;
                else if (lteMode == "FDD")
                    fdd++;
            }
            if (e.Button == MouseButtons.Left && CSAMForm.CellSearchListView.FocusedItem != null)
            {
                ListViewItem LVItem = new ListViewItem(new string[8]);

                int index = CSAMForm.CellMonitorListView.Items.Count;
                LVItem.SubItems[0].Text = (index + 1).ToString();
                int i = 1;
                for (; i < 4; i++)
                {
                    LVItem.SubItems[i].Text = CSAMForm.CellSearchListView.FocusedItem.SubItems[i].Text;
                }
                
                LVItem.SubItems[5].Text = CSAMForm.CellSearchListView.FocusedItem.SubItems[7].Text;
                LVItem.SubItems[6].Text = CSAMForm.CellSearchListView.FocusedItem.SubItems[8].Text;
                LVItem.SubItems[7].Text = CSAMForm.CellSearchListView.FocusedItem.SubItems[9].Text;
                
                string earfcn = CSAMForm.CellSearchListView.FocusedItem.SubItems[2].Text;
                string lteMode = Global.EARFCNToLteMode(Convert.ToInt32(earfcn));
                if (lteMode == "TDD")
                {
                    if (tdd >= tddDevice.Count)
                    {
                        MessageBox.Show("TDD监控板卡不足，请检查配置！");
                        return;
                    }
                    else
                    {
                        LVItem.SubItems[4].Text = tddDevice[tdd].DeviceName;
                    }
                }
                else if (lteMode == "FDD")
                {
                    if (fdd >= fddDevice.Count)
                    {
                        MessageBox.Show("FDD监控板卡不足，请检查配置！");
                        return;
                    }
                    else
                    {
                        LVItem.SubItems[4].Text = fddDevice[fdd].DeviceName;
                    }
                }
                
                
                LVItem.Tag = CSAMForm.CellSearchListView.FocusedItem.Tag;
                CSAMForm.CellMonitorListView.Items.Add(LVItem);
                //CSAMForm.CellMonitorButton.Enabled = true;
                isStop = true;
                CellSearchAndMonitorUnions.CellMonitorLV = CSAMForm.CellMonitorListView;
            }
        }

        private ListView currentFocusedListView;
        private void CellSearchListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CSAMForm.contextMenuStrip.Items.Clear();
                if ((sender as ListView).Name == "CellSearchListView")
                {
                    currentFocusedListView = CSAMForm.CellSearchListView;
                    CSAMForm.contextMenuStrip.Items.Add("删除");
                    CSAMForm.contextMenuStrip.Items.Add("全部删除");
                }

                if ((sender as ListView).Name == "CellMonitorListView")
                {
                    currentFocusedListView = CSAMForm.CellMonitorListView;
                    CSAMForm.contextMenuStrip.Items.Add("修改监控板卡");
                    CSAMForm.contextMenuStrip.Items.Add(new ToolStripSeparator());
                    CSAMForm.contextMenuStrip.Items.Add("删除");
                    CSAMForm.contextMenuStrip.Items.Add("全部删除");
                }
                CSAMForm.contextMenuStrip.Show(MousePosition.X, MousePosition.Y);
            }

            if (e.Button == MouseButtons.Left)
            {
                if (!CSAMForm.CellStopBtn.Enabled)
                    CSAMForm.CellMonitorButton.Enabled = true;
            }

        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if ((e.ClickedItem as ToolStripMenuItem).Text == "删除")
            {
                ListView.SelectedListViewItemCollection selectedItems = currentFocusedListView.SelectedItems;
                int j = selectedItems.Count;
                int index = Convert.ToInt32(selectedItems[0].Text.Trim()) - 1;

                for (int i = 0; i < j; i++)
                {
                    currentFocusedListView.Items.Remove(selectedItems[0]);
                }

                //int index = Convert.ToInt32(currentFocusedListView.FocusedItem.Text.Trim()) - 1;

                //currentFocusedListView.Items.RemoveAt(index);

                for (int i = index; i < currentFocusedListView.Items.Count; i++)
                {
                    currentFocusedListView.Items[i].Text = (i + 1).ToString();
                }
                if (CSAMForm.CellSearchListView.Items.Count == 0) CSAMForm.CellMonitorButton.Enabled = false;
                CellSearchAndMonitorUnions.CellMonitorLV = CSAMForm.CellMonitorListView;
            }
            if ((e.ClickedItem as ToolStripMenuItem).Text == "全部删除")
            {
                currentFocusedListView.Items.Clear();
                if (CSAMForm.CellSearchListView.Items.Count == 0) CSAMForm.CellMonitorButton.Enabled = false;
            }
        }

        #region 保存信息
        private void SaveData()
        {
            string FilePath = System.Windows.Forms.Application.StartupPath + "\\CellSearchData.bin";
            CellSearchAndMonitorUnions.DataSave(CSAMForm.CellSearchListView, FilePath);
            FilePath = System.Windows.Forms.Application.StartupPath + "\\CellMonitorData.bin";
            CellSearchAndMonitorUnions.DataSave(CSAMForm.CellMonitorListView, FilePath);
        }
        #endregion

        #region 读取信息
        private void LoadData()
        {
            string FilePath = System.Windows.Forms.Application.StartupPath + "\\CellSearchData.bin";
            CellSearchAndMonitorUnions.DataLoad(CSAMForm.CellSearchListView, FilePath);
            FilePath = System.Windows.Forms.Application.StartupPath + "\\CellMonitorData.bin";
            CellSearchAndMonitorUnions.DataLoad(CSAMForm.CellMonitorListView, FilePath);
        }
        #endregion

        //第一次小区搜索的启动
        private static int CellSearchItemIndex;
        private static object CellSearchSender; 
        private void CellMonitorButton_Click(object sender, EventArgs e)
        {
            isStop = false;
            isSecond = false;
            CellSearchItemIndex = 0;
            CellSearchSender = sender;
            if (DeviceManger.deviceList.Count == 0)
            {
                MessageBox.Show("没有可用板卡！");
                return;
            }
            
            Global.GCurrentDevice = "";
            DeviceManger.flashDeviceConnect();
            if (DeviceManger.deviceConnect.Count == 0)
            {
                MessageBox.Show("没有可用板卡！");
                return;
            }
                //else if (deviceConnect.Count == 1)
                //{
                //    Global.GCurrentDevice = deviceConnect[0].DeviceName;
                //}
                //else if (deviceConnect.Count == 2)
                //{
                //    string device0 = deviceConnect[0].IpAddress.Replace(".", "");
                //    string device1 = deviceConnect[1].IpAddress.Replace(".", "");
                //    if (Convert.ToInt32(device0) < Convert.ToInt32(device1))
                //        Global.GCurrentDevice = deviceConnect[0].DeviceName;
                //    else
                //        Global.GCurrentDevice = deviceConnect[1].DeviceName;
                //}


            CSAMForm.CellSearchStart();
            foreach (ListViewItem item in CSAMForm.CellMonitorListView.Items)
            {
                CSAMForm.CellMonitorListView.Items.Remove(item);
            }
            
        }
        public static bool CSAMBtnStatus;
        private static int unableCount = 0;
        private static int enableCount = 0;
        private void CellSearchBtnUnable()
        {
            CrossThreadOperationControl CrossThreadChange = delegate()
            {
                if (unableCount == 0)
                {
                    CSAMBtnStatus = CSAMForm.CellMonitorButton.Enabled;
                    CSAMForm.CellMonitorButton.Enabled = false;
                    CSAMForm.AddToSearchListButton.Enabled = false;
                    CSAMForm.CellMonitorListView.Enabled = false;
                }
                unableCount++;
                if (unableCount == 2)
                    unableCount = 0;
            };
            CSAMForm.Invoke(CrossThreadChange);
            
        }
        private void CellSearchBtnEnable()
        {
            CrossThreadOperationControl CrossThreadChange = delegate()
            {
                if (enableCount == 0)
                {
                    if (CSAMBtnStatus == true)
                        CSAMForm.CellMonitorButton.Enabled = true;
                    CSAMForm.AddToSearchListButton.Enabled = true;
                    CSAMForm.CellMonitorListView.Enabled = true;
                }
                enableCount++;
                if (enableCount == 2)
                    enableCount = 0;
            };
            CSAMForm.Invoke(CrossThreadChange);
            
        }
        //private bool sendRexAnt = false;
        private void CellSearchStart()
        {
            string freq = CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[1].Text.Trim();
            string earfcn = CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[2].Text.Trim();
            string pci = CSAMForm.CellSearchListView.Items[CellSearchItemIndex].SubItems[3].Text.Trim();

            CustomDataEvtArg CDEArgMes = CellSearchAndMonitorUnions.GenerateMessage(earfcn, pci, 2);
            //Global.GCurrentDevice = DeviceManger.deviceList[0].DeviceName;
            string lteMode = Global.EARFCNToLteMode(Convert.ToInt32(earfcn));
            DeviceManger.flashDeviceConnect();
            foreach (Device dev in DeviceManger.deviceConnect)
            {
                if (dev.LteMode == lteMode)
                {
                    Global.GCurrentDevice = dev.DeviceName;
                    CDEArgMes.deivceName = dev.DeviceName;
                    if (dev.SendRexAnt == false)
                        SendRxAnt(CellSearchSender,dev.DeviceName);
                    break;
                }
            }
            if (String.IsNullOrEmpty(CDEArgMes.deivceName))
            {
                MessageBox.Show("没有"+ lteMode +"板卡！");
                foreach (Device dev in DeviceManger.deviceList)
                {
                    if (dev.SendRexAnt)
                        dev.SendRexAnt = false;
                }
                CSAMForm.CellMonitorButton.Enabled = true;
                CSAMForm.CellStopBtn.Enabled = false;
                isStop = true;
                Global.tempClass.CSAMStop();
                return;
            }
            Global.CurrentSender = CSAMForm.Name;
            //Global.tempClass.Start(CellSearchSender, CDEArgMes);
            if (CellSearchSender != null)
            {
                
                Global.tempClass.SendDataToDevice(CellSearchSender, CDEArgMes);
                CSAMRunning = true;
                //IsWitingForRelAck = false;
                CSAMForm.CellMonitorButton.Enabled = false;
                CSAMForm.CellStopBtn.Enabled = true;
                Global.tempClass.CSAMStart();
                //CSAMForm.StopMonitorButton.Enabled = true;
                mutexRrace = 0;
                //CustomDataEvtArg getAGTStatus = new CustomDataEvtArg();
                //getAGTStatus.deivceName = Global.GCurrentDevice;
                //getAGTStatus.data = new byte[] { 0, 0, 0, 0, 0, 0, 0x01, 0x40, 0, 0, 0, 0 };
                //Global.tempClass.SendDataToDevice(CellSearchSender, getAGTStatus);
            }            
        }
        private void SendRxAnt(object sender, string dev)
        {
            CustomDataEvtArg EvtArg = CellSearchAndMonitorUnions.RxAntMsg(1);
            EvtArg.deivceName = dev;
            Global.tempClass.SendDataToDevice(sender, EvtArg);
            DeviceManger.FindDevice(EvtArg.deivceName).SendRexAnt = true;
        }

        private void StopMonitorButton_Click(object sender, EventArgs e)
        {
            //ReleaseProtocolTracing();
            //CSAMForm.StopMonitorButton.Enabled = false;
            CSAMForm.CellMonitorButton.Enabled = true;
        }

        private void ReleaseProtocolTracing()
        {
            if (mutexRelease == 1)
            {
                Global.CurrentSender = CSAMForm.Name;
                AGIInterface.CustomDataEvtArg cusArg = new CustomDataEvtArg();
                cusArg.data = new byte[] { 0, 0, 0, 0, 4, 2, 0x0c, 0x40, 1, 0, 0, 0 };//加消息头。。
                cusArg.deivceName = Global.GCurrentDevice;
                //IsWitingForRelAck = true;
                Global.tempClass.SendDataToDevice(CellSearchSender, cusArg);
                mutexRelease = 0;
            }
            
        }

        private void CellStopBtn_Click(object sender, EventArgs e)
        {
            CSAMForm.CellStopBtn.Enabled = false;
            isStop = true;
        }
        private static Thread[]pingList;
        private static Thread PingThread;
        private void PingDevice()
        {
            pingList = new Thread[DeviceManger.deviceList.Count];
            for (int i = 0; i < DeviceManger.deviceList.Count;i++ )
            {
                pingList[i] = new Thread(new ParameterizedThreadStart(PingTarget));
                pingList[i].Start(DeviceManger.deviceList[i]);
            }
            PingThread = new Thread(PingCheck);
            PingThread.Start();
        }
        private void PingCheck()
        {
            while (true)
            {
                int prepare = 0;
                int ready = 0;
                int connect = 0;
                foreach (Device dev in DeviceManger.deviceList)
                {
                    if (dev.PingOK == 1)
                        prepare++;
                    else if (dev.PingOK == 2)
                        ready++;
                    if (dev.ConnectionState == 0)
                        connect++;
                }
                CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                {
                    if (connect > 0)
                        CSAMForm.PingText.Text = "已连接";
                    else if(ready > 0)
                        CSAMForm.PingText.Text = "待连接";
                    else if (prepare > 0)
                        CSAMForm.PingText.Text = "准备中";
                    else if (prepare == 0 && ready == 0)
                        CSAMForm.PingText.Text = "未就绪";
                };
                CSAMForm.Invoke(CrossThreadInfoRefresh);
                
            }
        }

        private void PingTarget(object dev)
        {
            try
            {
                string devName = "";
                string tempName = "";
                Device device = null;
                device = (Device)dev;
                string ip = "";
                Ping p = new Ping();
                PingOptions options = new PingOptions();
                string data = "TestData";
                byte[] buffer = Encoding.ASCII.GetBytes(data); 
                int timeout = 1000;
                PingReply reply;
                DateTime startTime = System.DateTime.Now;
                DateTime lastTime = System.DateTime.Now;
                TimeSpan spanTime;
                CrossThreadOperationControl CrossThreadGetInfo = delegate()
                {
                    device.PingOK = 0;
                };
                CSAMForm.Invoke(CrossThreadGetInfo);
                
                if (device == null)
                        return;
                while (true)
                {
                    CrossThreadOperationControl CrossThreadGetName = delegate()
                    {
                        
                    };
                    CSAMForm.Invoke(CrossThreadGetName);
                    ip = device.IpAddress;
                    options.DontFragment = true;
                    //if (IsDisposed || !this.IsHandleCreated) return;
                    reply = p.Send(ip, timeout, buffer, options);
                    if (reply.Status != IPStatus.Success)
                    {
                        if (device.PingOK == 0)
                        {
                            CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                            {
                            };
                            CSAMForm.Invoke(CrossThreadInfoRefresh);
                        }
                        else if (device.PingOK == 3)
                        {
                            lastTime = System.DateTime.Now;
                            spanTime = lastTime - startTime;
                            if (spanTime.Seconds >= 10)
                            {
                                CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                                {
                                };
                                CSAMForm.Invoke(CrossThreadInfoRefresh);
                                device.PingOK = 0;
                            }
                        }
                        else if (device.PingOK == 1 || device.PingOK == 2)
                        {
                            startTime = System.DateTime.Now;
                            device.PingOK = 3;
                        }
                    }
                    else if (reply.Status == IPStatus.Success)
                    {

                        if (device.PingOK == 0 || device.PingOK == 3)
                        {
                            startTime = System.DateTime.Now;
                            device.PingOK = 1;
                            CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                            {
                            };
                            CSAMForm.Invoke(CrossThreadInfoRefresh);
                        }
                        else if (device.PingOK == 1)
                        {   
                            lastTime = System.DateTime.Now;
                            spanTime = lastTime - startTime;
                            if (spanTime.Seconds >= 30)
                            {
                                CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                                {
                                };
                                CSAMForm.Invoke(CrossThreadInfoRefresh);
                                device.PingOK = 2;
                            }
                        }
                        else if (device.PingOK == 2)
                        {
                            CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                            {
                            };
                            CSAMForm.Invoke(CrossThreadInfoRefresh);
                        }
                    }
                    Thread.Sleep(CellSearchAndMonitorUnions.PingTime);
                }
            }
            catch { }
        }

        private void ConBtn_Click(object sender, EventArgs e)
        {
            foreach (Device connDevice in DeviceManger.deviceList)
            {
                if (connDevice.ConnectionState == (byte)Global.DeviceStateValue.Disconnection)
                {
                    connDevice.ConnectionState = (byte)Global.DeviceStateValue.connState;
                    connDevice.NewMessageConn();
                }
                else
                {
                    //MessageBox.Show("This instrument is connected!");
                }
            }
            DeviceManger.flashDeviceConnect();
        }

        private void DisconBtn_Click(object sender, EventArgs e)
        {
            if (CSAMRunning||FindTargetSTMSI.FTSTMSIRunning1||OrientationFinding.OrtRunning1)
            {
                MessageBox.Show("断开连接前请先停止当前工作！");
                return;
            }
            foreach (Device connDevice in DeviceManger.deviceList)
            {
                if (connDevice.ConnectionState == (byte)Global.DeviceStateValue.Connecting)
                {
                    connDevice.CloseDeviceConn();
                    connDevice.timer.Enabled = false;
                    connDevice.HeartCheckStart = false;
                }
                else
                {
                    //MessageBox.Show("This instrument is disconnected!");
                } 
            }
            DeviceManger.flashDeviceConnect();
        }

       
    }
}
