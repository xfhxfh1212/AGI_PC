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
using System.Runtime.InteropServices;
using DeviceMangerModule;
using System.Threading;
using SendSms;

namespace CellSearchAndMonitor
{
    public partial class OrientationFinding : WeifenLuo.WinFormsUI.Docking.DockContent, COM.ZCTT.AGI.Plugin.IPlugin
    {
        static bool EventLoad = false;
        static bool OrtRunning = false;
        private static Thread RebootThread;
        private static Thread ReSendThread;
        public static bool OrtRunning1
        {
            get { return OrientationFinding.OrtRunning; }
            set { OrientationFinding.OrtRunning = value; }
        }
        public OrientationFinding()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Global.tempClass.CSAMStartEvent += new Class1.CSAMStartHander(OrtFBtnUnable);
            Global.tempClass.CSAMStopEvent +=new Class1.CSAMStopHander(OrtFBtnEnable);
            Global.tempClass.FTSTMSIStartEvent += new Class1.FTSTMSIStartHander(OrtFBtnUnable);
            Global.tempClass.FTSTMSIStopEvent += new Class1.FTSTMSIStopHander(OrtFBtnEnable);
        }

        public static OrientationFinding OrtForm;
        #region 实现IPlugin接口
        public new void Load()
        {
            if (OrtForm == null || OrtForm.IsDisposed)
            {
                OrtForm = new OrientationFinding();
                OrtForm.ShowHint = DockState.Document;
                OrtForm.Show(GloabeControl.oriFindingDockPanel);
                OrtForm.DockState = DockState.Document;

            }
            else
            {
                OrtForm.ShowHint = DockState.Document;
                OrtForm.Show(GloabeControl.oriFindingDockPanel);
                OrtForm.DockState = DockState.Document;
                //debugDisplay.DockAreas = DockAreas.Document;
            }
            //sTmsiGraph.ShowDialog();
            if (EventLoad == false)
            {
                Global.tempClass.SendDataToProTrackEvent += this.DataReceived;
                Global.tempClass.StartEvent += new Class1.StartHandler(StartEvent);
                Global.tempClass.CloseDisplayFormEvent += new Class1.CloseDisplayFormHander(CloseForm);
                Global.tempClass.OrientationFidingStartEvent += new Class1.OrientationFidingStartHandler(InitData);
                Global.tempClass.SendACKToCellScanEvent += new Class1.DeviceSendACKToCellScan(ACKHandler);
                EventLoad = true;
            }
            OrtForm.TriggerComboBox.Items.Add("短信");
            //OrtForm.TriggerComboBox.Items.Add("电话");
            //OrtForm.TriggerComboBox.Items.Add("Ping");
            OrtForm.TriggerComboBox.Items.Add("寻呼");
            OrtForm.TriggerComboBox.SelectedItem = OrtForm.TriggerComboBox.Items[0];
            OrtForm.EarfcnLabel.Text = OrtForm.ECGILabel.Text = OrtForm.PCILabel.Text
                = OrtForm.RSRPLabel.Text = OrtForm.TAILabel.Text = OrtForm.FreqLabel.Text = OrtForm.RsrpMsgLabel.Text = "";
            OrtForm.StartButton.Enabled = OrtForm.StopButton.Enabled = OrtForm.ClearButton.Enabled = false;
            
            startPointY = OrtForm.PUCCHOneGraph.Location.Y + 100 - GraphMinHeight;
            PowerGraphInit();
        }

        /// <summary>
        /// 关闭当前窗口执行的事件
        /// </summary>
        private void CloseForm()
        {
            if (OrtRunning == true)
            {
                foreach (Device d in DeviceManger.deviceList)
                {
                    if (d.ConnectionState == (byte)Global.DeviceStateValue.Connecting)
                    {
                        d.Again = false;
                        d.Reboot = false;
                    }
                    if (d.SendRexAnt)
                        d.SendRexAnt = false;
                }
                OrtForm.StopOrtFinding(true, "");
            }

            OrtForm.Close();
            OrtForm.Dispose(true);

        }


        /// <summary>
        /// “开始测试”执行的事件：
        /// 清空所有数据以及界面上的内容
        /// </summary>
        /// <param name="sender">sender为AGIInterface.Class1</param>
        /// <param name="e">自定义的事件参数</param>
        private void StartEvent(object sender, AGIInterface.CustomDataEvtArg e)
        {
            if (OrtForm != null)
            {
                //读取两个表格的数据
                //sTmsiGraph.dgvDisplay.Rows.Clear();
            }
        }
        #endregion

        #region 用户实时信息与小区实时信息数据
        private struct UEInfoStruct
        {
            public string timeStamp;
            public int ID;//0-PUCCH,1-PUSCH
            public double rsrp;
            public int SINR;
        }
        private List<UEInfoStruct> UEInfoList = new List<UEInfoStruct>();
        private List<double> CellInfoList = new List<double>();
        private Thread InfoRefreshThread;
        private int UEInfoListIndex;
        private int CellInfoListIndex;

        #endregion

        /// <summary>
        /// 程序收到数据的处理方法
        /// </summary>
        /// <param name="sender">AGIInterface.Class1</param>
        /// <param name="e">自定义的事件参数</param>
        public void DataReceived(object sender, AGIInterface.CustomDataEvtArg e)
        {
            //将AG_PC_PROTOCOL_DATA显示在界面上
            if (OrtForm.InvokeRequired)
            {
                try
                {
                    if (OrtForm != null)
                    {
                        //proTracDisplay.Invoke(new Class1.DeviceSendDataToProTrackHandler(DataReceived), sender, e);
                        OrtForm.BeginInvoke(new Class1.DeviceSendDataToProTrackHandler(DataReceived), sender, e);
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
                if (Global.CurrentSender == OrtForm.Name)//&& e.deivceName == Global.GCurrentDevice)
                    try
                    {
                        UInt16 messageType = 0;
                        messageType = BitConverter.ToUInt16(e.data, 6);

                        if (messageType == COM.ZCTT.AGI.Common.AGIMsgDefine.L1_AG_PROTOCOL_DATA_MSG_TYPE)
                        {
                            UEInfoStruct UEInfo = new UEInfoStruct();
                            UEInfo.timeStamp = DateTime.Now.ToLongTimeString();

                            L1_PROTOCOL_DATA l1ProtocolData = new L1_PROTOCOL_DATA();
                            int l1size = Marshal.SizeOf(l1ProtocolData);
                            if (l1size > e.data.Length)
                            {
                                return;
                            }

                            #region 将数组转换成L1_AG_PROTOCOL_DATA结构体数组
                            byte[] data_L1_PROTOCOL_DATA = new byte[e.data.Length - l1size];
                            System.Buffer.BlockCopy(e.data, l1size, data_L1_PROTOCOL_DATA, 0, e.data.Length - l1size);
                            #endregion

                            //uplink,重构
                            if (l1ProtocolData.mstL1ProtocolDataHeader.mu8Direction == 0)
                            {
                                L1_UL_UE_MEAS stu_L1_UL_UE_MEAS = new L1_UL_UE_MEAS();
                                int size_L1_UL_UE_MEAS = Marshal.SizeOf(stu_L1_UL_UE_MEAS);
                                IntPtr intptr_L1_UL_UE_MEAS = Marshal.AllocHGlobal(size_L1_UL_UE_MEAS);
                                if (size_L1_UL_UE_MEAS > data_L1_PROTOCOL_DATA.Length)
                                {
                                    return;
                                }
                                Marshal.Copy(data_L1_PROTOCOL_DATA, 100, intptr_L1_UL_UE_MEAS, size_L1_UL_UE_MEAS);
                                stu_L1_UL_UE_MEAS = (L1_UL_UE_MEAS)Marshal.PtrToStructure(intptr_L1_UL_UE_MEAS, typeof(L1_UL_UE_MEAS));


                                switch (stu_L1_UL_UE_MEAS.muChType)
                                {
                                    case 7: //PUCCH
                                        UEInfo.ID = 0;
                                        break;
                                    case 5: //PUSCH
                                        UEInfo.ID = 1;
                                        break;
                                    default:
                                        break;
                                }
                                UEInfo.rsrp = stu_L1_UL_UE_MEAS.ms16Power * 0.125;
                                UEInfo.SINR = Convert.ToInt32(stu_L1_UL_UE_MEAS.ms8Sinr * 0.5);
                                OrtForm.UEInfoList.Add(UEInfo);
                            }
                            //上行
                            else if (l1ProtocolData.mstL1ProtocolDataHeader.mu8Direction == 1)
                            {
                                //int dstOffset = 0;
                                //L1_DL_UE_MEAS stu_L1_DL_UE_MEAS = new L1_DL_UE_MEAS();
                                //int size_L1_DL_UE_MEAS = Marshal.SizeOf(stu_L1_DL_UE_MEAS);
                                //IntPtr intptr_L1_DL_UE_MEAS = Marshal.AllocHGlobal(size_L1_DL_UE_MEAS);
                                //if (size_L1_DL_UE_MEAS > data_L1_PROTOCOL_DATA.Length)
                                //{
                                //    return;
                                //}
                                //Marshal.Copy(data_L1_PROTOCOL_DATA, dstOffset, intptr_L1_DL_UE_MEAS, size_L1_DL_UE_MEAS);
                                //dstOffset = dstOffset + size_L1_DL_UE_MEAS;
                                //stu_L1_DL_UE_MEAS = (L1_DL_UE_MEAS)Marshal.PtrToStructure(intptr_L1_DL_UE_MEAS, typeof(L1_DL_UE_MEAS));
                                //Marshal.FreeHGlobal(intptr_L1_DL_UE_MEAS);

                                //rsrpGraph.richTextBox2.AppendText("DownLink PDSCH RSRP = " /*+ (stu_L1_DL_UE_MEAS).ToString("f2")*/ + "dBm\r\n");
                            }
                        }

                        else if (OrtForm.CellInfoShow && messageType == COM.ZCTT.AGI.Common.AGIMsgDefine.L1_PHY_COMMEAS_IND_MSG_TYPE)
                        {
                            //RSRP/RSRQ/RSSI
                            UInt32 mu32MeasSelect = BitConverter.ToUInt32(e.data, 24);
                            if ("CRS" == GetChannelType(mu32MeasSelect))
                            {

                                CRS_RSRPQI_INFO stu_CRS_RSRPQI_INFO = new CRS_RSRPQI_INFO();
                                int size_CRS_RSRPQI_INFO = Marshal.SizeOf(stu_CRS_RSRPQI_INFO);
                                L1_PHY_COMMEAS_IND l1PhyComMeasInd = new L1_PHY_COMMEAS_IND();
                                int size_l1PhyComMeasInd = Marshal.SizeOf(l1PhyComMeasInd);
                                IntPtr intptr_CRS_RSRPQI_INFO = Marshal.AllocHGlobal(size_CRS_RSRPQI_INFO);
                                Marshal.Copy(e.data, size_l1PhyComMeasInd, intptr_CRS_RSRPQI_INFO, size_CRS_RSRPQI_INFO);
                                //dstOffset = dstOffset + size_CRS_RSRPQI_INFO;
                                stu_CRS_RSRPQI_INFO = (CRS_RSRPQI_INFO)Marshal.PtrToStructure(intptr_CRS_RSRPQI_INFO, typeof(CRS_RSRPQI_INFO));
                                Marshal.FreeHGlobal(intptr_CRS_RSRPQI_INFO);

                                OrtForm.CellInfoList.Add(stu_CRS_RSRPQI_INFO.mstCrs0RsrpqiInfo.ms16CRS_RP * 0.125);

                            }
                        }
                        else if (messageType == COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_CELL_CAPTURE_IND_MSG_TYPE)
                                {
                                    Dictionary<string, string> result = Decode.dataBackControl_SendMIBToDisplayEvent(e);
                                    OrtForm.EarfcnLabel.Text = result["EARFCN"];
                                    OrtForm.PCILabel.Text = result["PCI"];
                                    OrtForm.FreqLabel.Text = Global.EARFCNToFreq(Convert.ToInt32(result["EARFCN"])).ToString();
                                    OrtForm.TAILabel.Text = result["TAC"];
                                    OrtForm.ECGILabel.Text = result["CellID"];
                                    
                                    
                                    if ((result["TAC"] == "0x0" || result["CellID"] == "0x0" || result["RSRQ"] == "NULL" || result["RSRP"] == "NULL") && DeviceManger.FindDevice(e.deivceName).Again == false)
                                    {
                                        DeviceManger.FindDevice(e.deivceName).Again = true;
                                        OrtForm.ResideColor.BackColor = System.Drawing.Color.Red;
                                        CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                                        {
                                            StopOrtFinding(false, e.deivceName);
                                        };
                                        OrtForm.Invoke(CrossThreadInfoRefresh);
                                        
                                    }
                                    else if ((result["TAC"] == "0x0" || result["CellID"] == "0x0" || result["RSRQ"] == "NULL" || result["RSRP"] == "NULL") && DeviceManger.FindDevice(e.deivceName).Again == true)
                                    {
                                        Device temDevice = DeviceManger.FindDevice(e.deivceName);
                                        temDevice.Again = false;
                                        
                                        OrtForm.ResideColor.BackColor = System.Drawing.Color.Red;
                                        OrtForm.RsrpMsgLabel.Text = result["PCI"] + " 小区驻留失败！";

                                        if (temDevice.Reboot == false)
                                        {
                                            temDevice.Reboot = true;
                                            DeviceManger DM = new DeviceManger();
                                            try
                                            {
                                                if (OrtForm.InfoRefreshThread != null)
                                                    OrtForm.InfoRefreshThread.Abort();
                                                if (SendMessageOrPingThread != null)
                                                    SendMessageOrPingThread.Abort();
                                            }
                                            catch { }
                                            RebootThread = new Thread(() => DM.RebootThreadFunc(temDevice));
                                            RebootThread.Start();
                                            ReSendThread = new Thread(() => ReSendFun(temDevice));
                                            ReSendThread.Start();
                                            OrtForm.StopButton.Enabled = false;
                                            MessageBox.Show(result["PCI"] + " 小区驻留失败，努力搜索中！");
                                        }
                                        else
                                        {
                                            CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                                            {
                                                StopOrtFinding(false, e.deivceName);
                                            };
                                            OrtForm.Invoke(CrossThreadInfoRefresh);
                                            MessageBox.Show(result["PCI"] + " 小区驻留失败！");
                                        }
                                    }
                                    else if (Convert.ToInt16(result["RSRP"]) < -120)
                                    {
                                        OrtForm.RSRPLabel.Text = result["RSRP"] + "dBm";
                                        DeviceManger.FindDevice(e.deivceName).Again = false;
                                        OrtForm.ResideColor.BackColor = System.Drawing.Color.Green;
                                        OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Red;
                                        OrtForm.RsrpMsgLabel.Text = "下行信号强度较差！";
                                    }
                                    else
                                    {
                                        OrtForm.RSRPLabel.Text = result["RSRP"] + "dBm";
                                        OrtForm.RsrpMsgLabel.Text = "";
                                        double crsrp = Convert.ToDouble(result["RSRP"]);
                                        DeviceManger.FindDevice(e.deivceName).Again = false;
                                        OrtForm.ResideColor.BackColor = System.Drawing.Color.Green;
                                        if (crsrp >= -90)
                                            OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Green;
                                        else if (crsrp < -90 && crsrp > -100)
                                            OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Yellow;
                                        else if (crsrp < -100 && crsrp > -110)
                                            OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Orange;
                                        else if (crsrp < -110)
                                            OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Red;
                                    }
                                    

                                }
                        else if (OrtForm.TargetInfoListView.Items.Count!=0 && messageType == COM.ZCTT.AGI.Common.AGIMsgDefine.AG_PC_PROTOCOL_TRACE_REL_ACK_MSG_TYPE)
                        {
                            //保存最后8次
                            //if (OrtForm.TargetInfoListViewLastEight.Count == 8)
                            //{
                            //    OrtForm.TargetInfoListViewLastEight.RemoveAt(0);
                            //}
                            //OrtForm.TargetInfoListViewLastEight.Add(OrtForm.TargetInfoListView);
                            
                            //if(OrtForm.TargetInfoListViewLastEight.Count <= 8)
                            //    OrtForm.DataBackCombox.Items.Add("倒数第" + OrtForm.TargetInfoListViewLastEight.Count.ToString() + "次");
                            ////OrtForm.DataBackCombox.Text = "倒数第" + OrtForm.TargetInfoListViewLastEight.Count.ToString() + "次";
                            //OrtForm.PowerGraphInit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(">>>Message= " + ex.Message + "\r\n StrackTrace: " + ex.StackTrace);
                        return;
                    }
            }
        }
        //private bool sendRexAnt = false;
        public void ACKHandler(object sender, AGIInterface.CustomDataEvtArg e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Class1.DeviceSendACKToCellScan(ACKHandler), sender, e);
                }
                catch
                { }
            }
            else
            {
                if (Global.CurrentSender == OrtForm.Name)
                {
                    if (DeviceManger.FindDevice(e.deivceName).Again == true)
                    {
                        Thread.Sleep(1000);
                        CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                        {
                            OrtFindingStart(sender, e);
                        };
                        OrtForm.Invoke(CrossThreadInfoRefresh);
                    }
                    else if (stop == true)
                    {
                        CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                        {
                            Global.tempClass.OrtFStop();
                            OrtForm.StartButton.Enabled = true;
                            OrtForm.StopButton.Enabled = false;
                            foreach (Device dev in DeviceManger.deviceList)
                            {
                                if (dev.SendRexAnt)
                                    dev.SendRexAnt = false;
                            }
                            OrtRunning = false;
                            stop = false;
                        };
                        OrtForm.Invoke(CrossThreadInfoRefresh);

                    }
                }
            }
        }

        private void ReSendFun(Device temDevice)
        {

            Thread.Sleep(80000);
            while (true)
            {
                if (temDevice.ConnectionState == (byte)Global.DeviceStateValue.Connecting)
                {
                    Thread.Sleep(5000);
                    CustomDataEvtArg e = new CustomDataEvtArg();
                    e.deivceName = temDevice.DeviceName;
                    CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                    {
                        OrtForm.UEInfoList.Clear();
                        OrtForm.CellInfoList.Clear();
                        OrtForm.ResideColor.BackColor = OrtForm.RSRPcolor.BackColor = SystemColors.Control;
                        OrtForm.CellInfoListIndex = OrtForm.UEInfoListIndex = 0;

                        OrtForm.InfoRefreshThread = new Thread(() => InfoRefresh());
                        OrtForm.InfoRefreshThread.Start();
                        SendMessageOrPingThread = new Thread(OrtForm.SendTextMessage);
                        SendMessageOrPingThread.Start();
                        OrtFindingStart(OrtForm, e);
                    };
                    OrtForm.Invoke(CrossThreadInfoRefresh);
                    return;
                }
            }
        }

        private void SendRxAnt(object sender, string dev)
        {
            CustomDataEvtArg EvtArg = CellSearchAndMonitorUnions.RxAntMsg(1);
            EvtArg.deivceName = dev;
            Global.tempClass.SendDataToDevice(sender, EvtArg);
            DeviceManger.FindDevice(EvtArg.deivceName).SendRexAnt = true;
        }
        private string GetChannelType(UInt32 mu32MeasSelect)
        {
            string str = "";
            List<string> Lchannel = new List<string>();
            Dictionary<UInt32, string> Dchannel = new Dictionary<UInt32, string>();
            Dchannel.Add(0x8, "Poweroffset");
            Dchannel.Add(0x100, "PBCH");
            Dchannel.Add(0x200, "PCFICH");
            Dchannel.Add(0x400, "PDCCH");
            Dchannel.Add(0x800, "PHICH");
            Dchannel.Add(0x1000, "PSS/SSS");
            Dchannel.Add(0x2000, "CRS");
            foreach (var item in Dchannel)
            {
                if ((mu32MeasSelect & item.Key) == item.Key)
                {
                    Lchannel.Add(item.Value);
                }
            }
            foreach (string channel in Lchannel)
            {
                if (str == "")
                {
                    str += channel;
                }
                else
                {
                    str += ";" + channel;
                }
            }
            return str;
        }

        private bool IsOrienting = false;
        private bool CellInfoShow = false;
        private void InitData()
        {
            if (OrtForm.IsOrienting)
            {
                MessageBox.Show("请先停止测向！");
                return;
            }

            OrtForm.CellInfoShow = true;

            OrtForm.TriggerComboBox.SelectedIndex = 0;//CellSearchAndMonitorUnions.ComboBoxStatus;
            OrtForm.StartButton.Enabled = true;

            OrtForm.DataBackCombox.Items.Clear();
            //存储最后8次结果
            SaveTargetInfoLastEight();
        }
        private static Thread SendMessageOrPingThread;
        private void StartButton_Click(object sender, EventArgs e)
        {
            
            switch(OrtForm.TriggerComboBox.Text)
            {
                case "短信":
                    {
                        if (DeviceManger.deviceList.Count == 0)
                        {
                            MessageBox.Show("没有可用板卡！");
                            return;
                        }
                        Global.GCurrentDevice = "";
                        foreach (Device device in DeviceManger.deviceList)
                        {
                            if (device.ConnectionState == (byte)Global.DeviceStateValue.Connecting)
                            {
                                device.Again = false;
                                device.Reboot = false;
                                Global.GCurrentDevice = device.DeviceName;
                            }
                        }
                        if (Global.GCurrentDevice == "")
                        {
                            MessageBox.Show("没有可用板卡！");
                            return;
                        }
                        if (CellSearchAndMonitorUnions.CellMonitorLV.Items.Count == 0)
                        {
                            MessageBox.Show("没有监控板卡！");
                            return;
                        }
                        string band = "";
                        string earfcn = "";
                        string pci = "";
                        //string ltemode = Global.EARFCNToLteMode(Convert.ToInt32(CellSearchAndMonitorUnions.TargetEarfcn));
                        if (CellSearchAndMonitorUnions.TargetEarfcn == null)
                        {
                            MessageBox.Show("未设置相应的频点！");
                            return;
                        }
                        int i = 0;
                        foreach (ListViewItem item in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                        {
                            if (CellSearchAndMonitorUnions.TargetEarfcn == item.SubItems[2].Text)
                            {
                                OrtForm.FreqLabel.Text = item.SubItems[1].Text;
                                OrtForm.EarfcnLabel.Text = item.SubItems[2].Text;
                                OrtForm.PCILabel.Text = item.SubItems[3].Text;
                                OrtForm.TAILabel.Text = OrtForm.ECGILabel.Text = OrtForm.RSRPLabel.Text = OrtForm.RsrpMsgLabel.Text = "";
                                earfcn = item.SubItems[2].Text;
                                pci = item.SubItems[3].Text;
                                OrtForm.TargetInfoListView.Items.Clear();
                                OrtForm.SendMsgCount = 0;

                                OrtForm.PowerGraphInit();

                                OrtForm.CellInfoShow = true;
                                CustomDataEvtArg CDEArgMes = CellSearchAndMonitorUnions.GenerateMessage(earfcn, pci, 0);
                                CDEArgMes.deivceName = item.SubItems[4].Text.Trim();
                                
                                SendRxAnt(sender,CDEArgMes.deivceName);
                                //Global.tempClass.Start(CellSearchSender, CDEArgMes);
                                Global.CurrentSender = OrtForm.Name;
                                DeviceManger.FindDevice(CDEArgMes.deivceName).Release = true;
                                Global.tempClass.SendDataToDevice(sender, CDEArgMes);
                                break;
                            }
                            i++;
                        }
                        if (i == CellSearchAndMonitorUnions.CellMonitorLV.Items.Count)
                        {
                            MessageBox.Show("没有可用监控板卡！");
                            return;
                        }
                        OrtForm.StartButton.Enabled = false;
                        OrtForm.StopButton.Enabled = true;
                        OrtForm.ClearButton.Enabled = false;
                        //OrtForm.ClearButton.Enabled = true;

                        OrtForm.UEInfoList.Clear();
                        OrtForm.CellInfoList.Clear();
                        OrtForm.ResideColor.BackColor = OrtForm.RSRPcolor.BackColor = SystemColors.Control;
                        OrtForm.CellInfoListIndex = OrtForm.UEInfoListIndex = 0;
                        OrtForm.SendTimesLabel.Text = "0";

                        OrtForm.InfoRefreshThread = new Thread(() => InfoRefresh());
                        OrtForm.InfoRefreshThread.Start();
                        SendMessageOrPingThread = new Thread(OrtForm.SendTextMessage);
                        SendMessageOrPingThread.Start();
                        Global.tempClass.OrtFStart();
                        OrtRunning = true;
                        break;

                    }
                default:
                    break;
            }
        }
        private void OrtFindingStart(object sender, CustomDataEvtArg e)
        {
            switch (OrtForm.TriggerComboBox.Text)
            {
                case "短信":
                    {
                        if (DeviceManger.deviceList.Count == 0)
                        {
                            MessageBox.Show("没有可用板卡！");
                            return;
                        }
                        Global.GCurrentDevice = "";
                        foreach (Device device in DeviceManger.deviceList)
                        {
                            if (device.ConnectionState == (byte)Global.DeviceStateValue.Connecting)
                                Global.GCurrentDevice = device.DeviceName;
                        }
                        if (Global.GCurrentDevice == "")
                        {
                            MessageBox.Show("没有可用板卡！");
                            return;
                        }
                        if (CellSearchAndMonitorUnions.CellMonitorLV.Items.Count == 0)
                        {
                            MessageBox.Show("没有监控板卡！");
                            return;
                        }
                        string band = "";
                        string earfcn = "";
                        string pci = "";
                        //string ltemode = Global.EARFCNToLteMode(Convert.ToInt32(CellSearchAndMonitorUnions.TargetEarfcn));
                        if (CellSearchAndMonitorUnions.TargetEarfcn == null)
                        {
                            MessageBox.Show("未设置相应的频点！");
                            return;
                        }
                        foreach (ListViewItem item in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                        {
                            if (item.SubItems[4].Text.Trim() == e.deivceName)
                            {
                                OrtForm.FreqLabel.Text = item.SubItems[1].Text;
                                OrtForm.EarfcnLabel.Text = item.SubItems[2].Text;
                                OrtForm.PCILabel.Text = item.SubItems[3].Text;

                                earfcn = item.SubItems[2].Text;
                                pci = item.SubItems[3].Text;
                                OrtForm.PowerGraphInit();

                                OrtForm.CellInfoShow = true;
                                CustomDataEvtArg CDEArgMes = CellSearchAndMonitorUnions.GenerateMessage(earfcn, pci, 0);
                                CDEArgMes.deivceName = item.SubItems[4].Text.Trim();
                                //Global.tempClass.Start(CellSearchSender, CDEArgMes);
                                Global.CurrentSender = OrtForm.Name;
                                DeviceManger.FindDevice(CDEArgMes.deivceName).Release = true;
                                Global.tempClass.SendDataToDevice(sender, CDEArgMes);
                                break;
                            }
                        }

                        OrtForm.StartButton.Enabled = false;
                        OrtForm.StopButton.Enabled = true;
                        OrtForm.ClearButton.Enabled = false;
                        //Global.tempClass.OrtFStart();
                        OrtRunning = true;
                        break;

                    }
                default:
                    break;
            }
        }
        //[STAThread]

        //[DllImport("sms.dll", EntryPoint = "Sms_Connection")]
        //public static extern uint Sms_Connection(string CopyRight, uint Com_Port, uint Com_BaudRate, out string Mobile_Type, out string CopyRightToCOM);

        //[DllImport("sms.dll", EntryPoint = "Sms_Disconnection")]
        //public static extern uint Sms_Disconnection();

        //[DllImport("sms.dll", EntryPoint = "Sms_Send")]
        //public static extern uint Sms_Send(string Sms_TelNum, string Sms_Text);

        //[DllImport("sms.dll", EntryPoint = "Sms_Receive")]
        //public static extern uint Sms_Receive(string Sms_Type, out string Sms_Text);

        //[DllImport("sms.dll", EntryPoint = "Sms_Delete")]
        //public static extern uint Sms_Delete(string Sms_Index);

        //[DllImport("sms.dll", EntryPoint = "Sms_AutoFlag")]
        //public static extern uint Sms_AutoFlag();

        //[DllImport("sms.dll", EntryPoint = "Sms_NewFlag")]
        //public static extern uint Sms_NewFlag();

        private int SendMsgCount = 0;
        private void SendTextMessage()
        {
            try
            {
                //String threshold = "";
                String tel = "";
                tel = CellSearchAndMonitorUnions.TargetTel;
                if (String.IsNullOrEmpty(tel))
                {
                    MessageBox.Show("请停止测向并选择目标号码！");
                    return;
                }
                //if (!CellSearchAndMonitorUnions.IsDigitalString(threshold))
                //{
                //    MessageBox.Show("请输入合法门限值！");
                //    return;
                //}
                //int SendMesThreshold = Convert.ToInt32(threshold);
                int smsModCB = CellSearchAndMonitorUnions.SmsModCB;
                String centerCode = CellSearchAndMonitorUnions.CenterCode;
                int FailSendingCount = 0;
                if (STM_Sms_Connection())
                {
                    //string MesText = CellSearchAndMonitorUnions.TestMessage;
                    Thread.Sleep(5000);
                    //for (int i = 0; i < SendMesThreshold; i++)
                    while (true)
                    {
                        //if (Sms_Send(tel, MesText) == 0)

                        CellSearchAndMonitorUnions.sendSms.send_msg_get(smsModCB, centerCode, tel);
                        
                        Thread.Sleep(4000);

                        int flag = CellSearchAndMonitorUnions.sendSms.Get_Result();
                        //MessageBox.Show("发送状态：" + flag);
                        if (flag != 0)
                        {
                            FailSendingCount++;
                            MessageBox.Show("发送失败！");
                        }
                        else
                        {
                            CrossThreadOperationControl CrossThreadTimeRefresh = delegate()
                            {
                                OrtForm.SendMsgCount++;
                                OrtForm.SendTimesLabel.Text = OrtForm.SendMsgCount.ToString();
                            };
                            OrtForm.Invoke(CrossThreadTimeRefresh);
                        }
                        Thread.Sleep(CellSearchAndMonitorUnions.TestMessageSendTime);
                    }

                    //MessageBox.Show("短信发送失败共计 " + FailSendingCount + "次！");

                    //Sms_Disconnection();
                    //Sms_connection = false;
                    //CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                    //    {
                    //        StopOrtButton();
                    //    };
                    //OrtForm.Invoke(CrossThreadInfoRefresh);

                    //Thread.Sleep(3000);
                    //AGIInterface.CustomDataEvtArg cusArg = new CustomDataEvtArg();
                    //cusArg.data = new byte[] { 0, 0, 0, 0, 4, 2, 0x0c, 0x40, 1, 0, 0, 0 };//加消息头。。
                    //cusArg.deivceName = Global.GCurrentDevice;
                    //Global.tempClass.SendDataToDevice(FTSTMSIForm, cusArg);
                }
            }
            catch (Exception e) { 
                //MessageBox.Show(e.Message); 
            }
        }
        private bool STM_Sms_Connection()
        {
            String TypeStr = "";
            String CopyRightToCOM = "";
            String CopyRightStr = "//上海迅赛信息技术有限公司,网址www.xunsai.com//";
            String Port = CellSearchAndMonitorUnions.SmsModCB.ToString();

            //if (Sms_Connection(CopyRightStr, uint.Parse(Port), 9600, out TypeStr, out CopyRightToCOM) == 1) ///5为串口号，0为红外接口，1,2,3,...为串口
            if (CellSearchAndMonitorUnions.Sms_connection == false)
            {
                bool temBool = CellSearchAndMonitorUnions.sendSms.Init("192.168.7.1", 8899, 8899);
                //MessageBox.Show("连接状态：" + temBool);

                if (temBool)
                {
                    CellSearchAndMonitorUnions.Sms_connection = true;
                    return true;
                }
                else
                {
                    CellSearchAndMonitorUnions.Sms_connection = false;
                    MessageBox.Show("短信猫连接失败，请检查！");
                    return false;
                }
            }
            else
                return true;
        }
        private delegate void CrossThreadOperationControl();
        private void InfoRefresh()
        {
            while (true)
            {
                int UEInfoListLast = OrtForm.UEInfoList.Count;
                int CellInfoListLast = OrtForm.CellInfoList.Count;
                if (UEInfoListLast > UEInfoListIndex)
                {
                    ListViewItem LVItem = new ListViewItem(new string[4]);
                    LVItem.SubItems[0].Text = (OrtForm.TargetInfoListView.Items.Count + 1).ToString();
                    LVItem.SubItems[3].Text = OrtForm.UEInfoList[UEInfoListIndex].timeStamp;
                    double PUSCHRsrp = MinRSRP; //int PUSCHCount = 0;
                    double PUCCHRsrp = MinRSRP; //int PUCCHCount = 0;

                    List<double> PUSCHRsrpList = new List<double>();
                    List<double> PUCCHRsrpList = new List<double>();
                    for (; UEInfoListIndex < UEInfoListLast; UEInfoListIndex++)
                    {
                        //SINR统计门限过滤
                        if (UEInfoList[UEInfoListIndex].SINR <= CellSearchAndMonitorUnions.TestSINRThreshold)
                            continue;
                        //功率值过滤
                        //if (UEInfoList[UEInfoListIndex].rsrp < -120 || UEInfoList[UEInfoListIndex].rsrp > -35)
                        //    continue;
                      
                        //if (UEInfoList[UEInfoListIndex].rsrp > MaxRSRP || UEInfoList[UEInfoListIndex].rsrp < MinRSRP) continue;
                        switch (UEInfoList[UEInfoListIndex].ID)
                        {
                            //PUCCH
                            case 0:
                                if (UEInfoList[UEInfoListIndex].rsrp > -125 && UEInfoList[UEInfoListIndex].rsrp < -35)
                                    PUCCHRsrpList.Add(UEInfoList[UEInfoListIndex].rsrp);
                                //PUCCHRsrp += UEInfoList[UEInfoListIndex].rsrp;
                                //PUCCHCount++;
                                break;
                            //PUSCH
                            case 1:
                                if (UEInfoList[UEInfoListIndex].rsrp > -130 && UEInfoList[UEInfoListIndex].rsrp < -35)
                                    PUSCHRsrpList.Add(UEInfoList[UEInfoListIndex].rsrp);
                                //PUSCHRsrp += UEInfoList[UEInfoListIndex].rsrp;
                                //PUSCHCount++;
                                break;
                            default:
                                break;
                        }
                    }
                    if (PUSCHRsrpList.Count >= 15)
                    {
                        if(PUSCHRsrpList.Count > 15)
                            PUSCHRsrpList.RemoveRange(15, PUSCHRsrpList.Count - 15);
                        PUSCHRsrpList.Sort();
                        PUSCHRsrp = PUSCHRsrpList.Sum() - PUSCHRsrpList[0] - PUSCHRsrpList[1] - PUSCHRsrpList[2] - PUSCHRsrpList[13] - PUSCHRsrpList[14];
                        PUSCHRsrp /= PUSCHRsrpList.Count - 5;
                        LVItem.SubItems[1].Text = PUSCHRsrp.ToString("f2") + "dBm";
                    }
                    else if (PUSCHRsrpList.Count > 5)
                    {
                        PUSCHRsrpList.Sort();
                        PUSCHRsrp = PUSCHRsrpList.Sum() - PUSCHRsrpList[0] - PUSCHRsrpList[PUSCHRsrpList.Count - 1];
                        PUSCHRsrp /= PUSCHRsrpList.Count - 2;
                        LVItem.SubItems[1].Text = PUSCHRsrp.ToString("f2") + "dBm";
                    }
                    else if (PUSCHRsrpList.Count > 0)
                    {
                        PUSCHRsrp = PUSCHRsrpList.Average();
                        LVItem.SubItems[1].Text = PUSCHRsrp.ToString("f2") + "dBm";
                    }
                    else
                    {
                        LVItem.SubItems[1].Text = "N/A";
                    }

                    if (PUCCHRsrpList.Count >= 15)
                    {
                        if (PUCCHRsrpList.Count > 15)
                            PUCCHRsrpList.RemoveRange(15, PUCCHRsrpList.Count - 15);
                        PUCCHRsrpList.Sort();
                        PUCCHRsrp = PUCCHRsrpList.Sum() - PUCCHRsrpList[0] - PUCCHRsrpList[1] - PUCCHRsrpList[2] - PUCCHRsrpList[13] - PUCCHRsrpList[14];
                        PUCCHRsrp /= PUCCHRsrpList.Count - 5;
                        LVItem.SubItems[2].Text = PUCCHRsrp.ToString("f2") + "dBm";
                    }
                    else if (PUCCHRsrpList.Count > 5)
                    {
                        PUCCHRsrpList.Sort();
                        PUCCHRsrp = PUCCHRsrpList.Sum() - PUCCHRsrpList[0] - PUCCHRsrpList[PUCCHRsrpList.Count - 1];
                        PUCCHRsrp /= PUCCHRsrpList.Count - 2;
                        LVItem.SubItems[2].Text = PUCCHRsrp.ToString("f2") + "dBm";
                    }
                    else if (PUCCHRsrpList.Count > 0)
                    {
                        PUCCHRsrp = PUCCHRsrpList.Average();
                        LVItem.SubItems[2].Text = PUCCHRsrp.ToString("f2") + "dBm";
                    }
                    else
                    {
                        LVItem.SubItems[2].Text = "N/A";
                    }
                    
                    CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                    {
                        OrtForm.TargetInfoListView.Items.Add(LVItem);
                        OrtForm.TargetInfoListView.Items[OrtForm.TargetInfoListView.Items.Count - 1].Selected = true;
                        OrtForm.TargetInfoListView.Items[OrtForm.TargetInfoListView.Items.Count - 1].EnsureVisible();

                       
                        OrtForm.PowerGraphRefresh(PUSCHRsrp, PUCCHRsrp);
                    };
                    OrtForm.Invoke(CrossThreadInfoRefresh);
                    //OrtForm.UEInfoList.RemoveRange(0, UEInfoListLast);
                }

                if (CellInfoListLast > CellInfoListIndex)
                {
                    double CellPower = 0; int count = 0;
                    for (; CellInfoListIndex < CellInfoListLast; CellInfoListIndex++, count++)
                    {
                        CellPower += CellInfoList[CellInfoListIndex];
                    }
                    CellPower /= count;
                    CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                    {
                        OrtForm.RSRPLabel.Text = CellPower.ToString("f2") + "dBm";
                        if (CellPower >= -90)
                            OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Green;
                        else if (CellPower < -90 && CellPower > -100)
                            OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Yellow;
                        else if (CellPower < -100 && CellPower > -110)
                            OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Orange;
                        else if (CellPower < -110)
                            OrtForm.RSRPcolor.BackColor = System.Drawing.Color.Red;
                    };
                    OrtForm.Invoke(CrossThreadInfoRefresh);
                    //OrtForm.CellInfoList.RemoveRange(0, CellInfoListLast);
                }


                Thread.Sleep(CellSearchAndMonitorUnions.OriRefreshTime);
            }
        }
            

    

        #region stopbutton
        private List<ListView> TargetInfoListViewLastEight = new List<ListView>();
        
        private void StopButton_Click(object sender, EventArgs e)
        {
            foreach (Device d in DeviceManger.deviceList)
            {
                if (d.ConnectionState == (byte)Global.DeviceStateValue.Connecting)
                {
                    d.Again = false;
                    d.Reboot = false;
                }
            }
            StopOrtFinding(true, "");
            //Global.tempClass.FTSTMSIStop();
            OrtForm.StopButton.Enabled = false;
        }
        private void StopOrtButton()
        {
            //if (Sms_connection == true)
                //Sms_Disconnection();
            OrtForm.CellInfoShow = false;
            foreach (ListViewItem item in CellSearchAndMonitorUnions.CellMonitorLV.Items)
            {
                AGIInterface.CustomDataEvtArg cusArg = new CustomDataEvtArg();
                cusArg.data = new byte[] { 0, 0, 0, 0, 4, 2, 0x0c, 0x40, 1, 0, 0, 0 };//加消息头。。
                cusArg.deivceName = item.SubItems[4].Text.Trim();
                Global.tempClass.SendDataToDevice(OrtForm, cusArg);
            }
            try
            {
                if (OrtForm.InfoRefreshThread != null)
                    OrtForm.InfoRefreshThread.Abort();
            }
            catch { }
            try
            {
                if (SendMessageOrPingThread != null)
                    SendMessageOrPingThread.Abort();
            }
            catch { }

            if (OrtForm.TargetInfoListViewLastEight.Count == 8)
            {
                OrtForm.TargetInfoListViewLastEight.RemoveAt(0);
            }
            OrtForm.TargetInfoListViewLastEight.Add(OrtForm.TargetInfoListView);

            SaveTargetInfoLastEight();
            if (OrtForm.TargetInfoListViewLastEight.Count <= 8)
                OrtForm.DataBackCombox.Items.Add("最后八组第" + OrtForm.TargetInfoListViewLastEight.Count.ToString() + "组");
            //OrtForm.DataBackCombox.Text = "倒数第" + OrtForm.TargetInfoListViewLastEight.Count.ToString() + "次";
            //OrtForm.PowerGraphInit();

            OrtForm.StartButton.Enabled = true;
            OrtForm.StopButton.Enabled = false;
            OrtForm.ClearButton.Enabled = true;
            //OrtRunning = false;
        }
        private static bool stop = false;
        private void StopOrtFinding(bool All, string device)
        {
            if (All)
            {
                int i = 0;
                foreach (ListViewItem LVItem in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                {
                    if (i == CellSearchAndMonitorUnions.CellMonitorLV.Items.Count - 1)
                        stop = true;
                    string temdevice = LVItem.SubItems[4].Text.Trim();
                    Device dev = DeviceManger.FindDevice(temdevice);
                    if (dev.Release == true)
                    {
                        //if (Sms_connection == true)
                        //{
                        //    //Sms_Disconnection();
                        //    Sms_connection = false;
                        //}
                        try
                        {
                            if (OrtForm.InfoRefreshThread != null)
                                OrtForm.InfoRefreshThread.Abort();
                        }
                        catch { }
                        AGIInterface.CustomDataEvtArg cusArg = new CustomDataEvtArg();
                        cusArg.data = new byte[] { 0, 0, 0, 0, 4, 2, 0x0c, 0x40, 1, 0, 0, 0 };//加消息头。。
                        cusArg.deivceName = dev.DeviceName;
                        Global.CurrentSender = OrtForm.Name;
                        Global.tempClass.SendDataToDevice(OrtForm, cusArg);

                        //foreach (ListViewItem LVItem in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                        //{

                        //    Global.GCurrentDevice = LVItem.SubItems[6].Text.Trim();
                        //    cusArg.deivceName = Global.GCurrentDevice;
                        //    Global.CurrentSender = FTSTMSIForm.Name;
                        //    //Global.tempClass.Start(CellSearchSender, CDEArgMes);
                        //    Global.tempClass.SendDataToDevice(FTSTMSIForm, cusArg);
                        //}
                        //FTSTMSIForm.GetSTMSIButton.Enabled = true;
                        try
                        {
                            if (SendMessageOrPingThread != null)
                                SendMessageOrPingThread.Abort();
                            //if (RebootThread != null)
                            //    RebootThread.Abort();
                            //if (ReSendThread != null)
                            //    ReSendThread.Abort();
                        }
                        catch { }

                        DeviceManger.FindDevice(temdevice).Release = false;
                    }
                    i++;
                }
                if (OrtForm.TargetInfoListViewLastEight.Count == 8)
                {
                    OrtForm.TargetInfoListViewLastEight.RemoveAt(0);
                }
                OrtForm.TargetInfoListViewLastEight.Add(OrtForm.TargetInfoListView);
                OrtForm.CellInfoShow = false;
                SaveTargetInfoLastEight();
                if (OrtForm.TargetInfoListViewLastEight.Count <= 8)
                    OrtForm.DataBackCombox.Items.Add("最后八组第" + OrtForm.TargetInfoListViewLastEight.Count.ToString() + "组");
            }
            else
            {
                if (DeviceManger.FindDevice(device).Release == true)
                {
                    DeviceManger.FindDevice(device).Release = false;
                }
                int releaseCount = 0;
                foreach (ListViewItem LVItem in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                {
                    string temdevice = LVItem.SubItems[4].Text.Trim();
                    if (DeviceManger.FindDevice(temdevice).Release == false)
                    {
                        releaseCount++;
                    }
                }
                if (releaseCount == CellSearchAndMonitorUnions.CellMonitorLV.Items.Count)
                {
                    //if (Sms_connection == true)
                    //{
                    //    //Sms_Disconnection();
                    //    Sms_connection = false;
                    //}
                    try
                    {
                        if (OrtForm.InfoRefreshThread != null)
                            OrtForm.InfoRefreshThread.Abort();
                    }
                    catch { }
                    try
                    {
                        if (SendMessageOrPingThread != null)
                            SendMessageOrPingThread.Abort();
                        if (RebootThread != null)
                            RebootThread.Abort();
                        if (ReSendThread != null)
                            ReSendThread.Abort();
                    }
                    catch { }
                    if (OrtForm.TargetInfoListViewLastEight.Count == 8)
                    {
                        OrtForm.TargetInfoListViewLastEight.RemoveAt(0);
                    }
                    OrtForm.TargetInfoListViewLastEight.Add(OrtForm.TargetInfoListView);
                    OrtForm.CellInfoShow = false;
                    SaveTargetInfoLastEight();
                    if (OrtForm.TargetInfoListViewLastEight.Count <= 8)
                        OrtForm.DataBackCombox.Items.Add("最后八组第" + OrtForm.TargetInfoListViewLastEight.Count.ToString() + "组");
                    stop = true;
                }
                AGIInterface.CustomDataEvtArg cusArg = new CustomDataEvtArg();
                cusArg.data = new byte[] { 0, 0, 0, 0, 4, 2, 0x0c, 0x40, 1, 0, 0, 0 };//加消息头。。
                cusArg.deivceName = device;
                Global.CurrentSender = OrtForm.Name;
                Global.tempClass.SendDataToDevice(OrtForm, cusArg);

            }
        }
        #endregion

        private void SaveTargetInfoLastEight()
        {
            for (int i = 0; i < TargetInfoListViewLastEight.Count; i++)
            {
                string FilePath = System.Windows.Forms.Application.StartupPath + "\\测向结果\\最后八组第" + (i + 1).ToString() + "组";
                CellSearchAndMonitorUnions.DataSave(TargetInfoListViewLastEight[i], FilePath);
            }
            TargetInfoListViewLastEight.Clear();
        }

        private void DataBackCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(OrtForm.IsOrienting)
            {
                MessageBox.Show("请先停止测向，才能读取历史数据");
                return;
            }

            string FilePath = System.Windows.Forms.Application.StartupPath + "\\测向结果\\" + OrtForm.DataBackCombox.Text;
            CellSearchAndMonitorUnions.DataLoad(TargetInfoListView, FilePath);
        }

        #region 能量图
        
        private static int startPointY = 0;
        private void PowerGraphInit()
        {
            int smallGap=10;
            int bigGap=20;
            int start = 45;
            OrtForm.PUSCHOneGraph.Left = OrtForm.groupBox13.Left + start;
            OrtForm.PUCCHOneGraph.Left = OrtForm.PUSCHOneGraph.Right + smallGap;
            OrtForm.PUSCHTwoGraph.Left = OrtForm.PUCCHOneGraph.Right + bigGap;
            OrtForm.PUCCHTwoGraph.Left = OrtForm.PUSCHTwoGraph.Right + smallGap;
            OrtForm.PUSCHThreeGraph.Left = OrtForm.PUCCHTwoGraph.Right + bigGap;
            OrtForm.PUCCHThreeGraph.Left = OrtForm.PUSCHThreeGraph.Right + smallGap;
            OrtForm.PUSCHFourGraph.Left = OrtForm.PUCCHThreeGraph.Right + bigGap;
            OrtForm.PUCCHFourGraph.Left = OrtForm.PUSCHFourGraph.Right + smallGap;

            OrtForm.PUCCHOneGraph.Height = OrtForm.PUCCHTwoGraph.Height = OrtForm.PUCCHThreeGraph.Height = OrtForm.PUCCHFourGraph.Height
                = OrtForm.PUSCHOneGraph.Height = OrtForm.PUSCHTwoGraph.Height = OrtForm.PUSCHThreeGraph.Height = OrtForm.PUSCHFourGraph.Height = GraphMinHeight;

            OrtForm.TeamOne.Left = OrtForm.PUSCHOneGraph.Left;
            OrtForm.TeamTwo.Left = OrtForm.PUSCHTwoGraph.Left;
            OrtForm.TeamThree.Left = OrtForm.PUSCHThreeGraph.Left;
            OrtForm.TeamFour.Left = OrtForm.PUSCHFourGraph.Left;

            

            OrtForm.PUSCHOneGraph.Location = new Point(OrtForm.PUSCHOneGraph.Location.X, startPointY);
            OrtForm.PUCCHOneGraph.Location = new Point(OrtForm.PUCCHOneGraph.Location.X, startPointY);
            OrtForm.PUSCHTwoGraph.Location = new Point(OrtForm.PUSCHTwoGraph.Location.X, startPointY);
            OrtForm.PUCCHTwoGraph.Location = new Point(OrtForm.PUCCHTwoGraph.Location.X, startPointY);
            OrtForm.PUSCHThreeGraph.Location = new Point(OrtForm.PUSCHThreeGraph.Location.X, startPointY);
            OrtForm.PUCCHThreeGraph.Location = new Point(OrtForm.PUCCHThreeGraph.Location.X, startPointY);
            OrtForm.PUSCHFourGraph.Location = new Point(OrtForm.PUSCHFourGraph.Location.X, startPointY);
            OrtForm.PUCCHFourGraph.Location = new Point(OrtForm.PUCCHFourGraph.Location.X, startPointY);
            for (int i = 0; i < GraphRSRPList.Count; i++)
            {
                GraphRSRPList[i] = MinRSRP;
            }
        }

        private static int MaxRSRP = -50;
        private static int MinRSRP = -120; 
        private int GraphMinHeight = 10;
        private List<double> GraphRSRPList = new List<double>(new double[8]);
        private void PowerGraphRefresh(double NewPUSCH,double NewPUCCH)
        {
            for(int i=0;i<6;i++)
            {
                GraphRSRPList[i] = GraphRSRPList[i + 2];
            }
            GraphRSRPList[6] = NewPUSCH == 0 ? MinRSRP : NewPUSCH;
            GraphRSRPList[7] = NewPUCCH == 0 ? MinRSRP : NewPUCCH;

            double MaxRSRPGap = MaxRSRP - MinRSRP;
            int standardHeight = 150;

            OrtForm.PUSCHOneGraph.Height = GraphMinHeight + Convert.ToInt32((GraphRSRPList[0] - MinRSRP) / MaxRSRPGap * standardHeight);
            OrtForm.PUCCHOneGraph.Height = GraphMinHeight + Convert.ToInt32((GraphRSRPList[1] - MinRSRP) / MaxRSRPGap * standardHeight);
            OrtForm.PUSCHTwoGraph.Height = GraphMinHeight + Convert.ToInt32((GraphRSRPList[2] - MinRSRP) / MaxRSRPGap * standardHeight);
            OrtForm.PUCCHTwoGraph.Height = GraphMinHeight + Convert.ToInt32((GraphRSRPList[3] - MinRSRP) / MaxRSRPGap * standardHeight);
            OrtForm.PUSCHThreeGraph.Height = GraphMinHeight + Convert.ToInt32((GraphRSRPList[4] - MinRSRP) / MaxRSRPGap * standardHeight);
            OrtForm.PUCCHThreeGraph.Height = GraphMinHeight + Convert.ToInt32((GraphRSRPList[5] - MinRSRP) / MaxRSRPGap * standardHeight);
            OrtForm.PUSCHFourGraph.Height = GraphMinHeight + Convert.ToInt32((GraphRSRPList[6] - MinRSRP) / MaxRSRPGap * standardHeight);
            OrtForm.PUCCHFourGraph.Height = GraphMinHeight + Convert.ToInt32((GraphRSRPList[7] - MinRSRP) / MaxRSRPGap * standardHeight);

            OrtForm.PUSCHOneGraph.Location = new Point(OrtForm.PUSCHOneGraph.Location.X, startPointY - OrtForm.PUSCHOneGraph.Height + GraphMinHeight);
            OrtForm.PUCCHOneGraph.Location = new Point(OrtForm.PUCCHOneGraph.Location.X, startPointY - OrtForm.PUCCHOneGraph.Height + GraphMinHeight);
            OrtForm.PUSCHTwoGraph.Location = new Point(OrtForm.PUSCHTwoGraph.Location.X, startPointY - OrtForm.PUSCHTwoGraph.Height + GraphMinHeight);
            OrtForm.PUCCHTwoGraph.Location = new Point(OrtForm.PUCCHTwoGraph.Location.X, startPointY - OrtForm.PUCCHTwoGraph.Height + GraphMinHeight);
            OrtForm.PUSCHThreeGraph.Location = new Point(OrtForm.PUSCHThreeGraph.Location.X, startPointY - OrtForm.PUSCHThreeGraph.Height + GraphMinHeight);
            OrtForm.PUCCHThreeGraph.Location = new Point(OrtForm.PUCCHThreeGraph.Location.X, startPointY - OrtForm.PUCCHThreeGraph.Height + GraphMinHeight);
            OrtForm.PUSCHFourGraph.Location = new Point(OrtForm.PUSCHFourGraph.Location.X, startPointY - OrtForm.PUSCHFourGraph.Height + GraphMinHeight);
            OrtForm.PUCCHFourGraph.Location = new Point(OrtForm.PUCCHFourGraph.Location.X, startPointY - OrtForm.PUCCHFourGraph.Height + GraphMinHeight);
        }
        #endregion

        private void ClearButton_Click(object sender, EventArgs e)
        {
            OrtForm.TargetInfoListView.Items.Clear();
            OrtForm.PowerGraphInit();
        }
        private static bool OrtFBtnStatus = false;
        private static int unableCount = 0;
        private static int enableCount = 0;
        private void OrtFBtnUnable()
        {
            CrossThreadOperationControl CrossThreadChange = delegate()
            {
                
                if (unableCount == 0)
                {
                    OrtFBtnStatus = OrtForm.StartButton.Enabled;
                    OrtForm.StartButton.Enabled = false;
                }
                unableCount++;
                if (unableCount == 2)
                    unableCount = 0;
            };
            OrtForm.Invoke(CrossThreadChange);
            
            
        }
        private void OrtFBtnEnable()
        {
            CrossThreadOperationControl CrossThreadChange = delegate()
            {
                if (unableCount == 0)
                {
                    if (OrtFBtnStatus == true)
                        OrtForm.StartButton.Enabled = true;
                }
                unableCount++;
                if (unableCount == 2)
                    unableCount = 0;
            };
            OrtForm.Invoke(CrossThreadChange);
            
            
        }
    }
}
