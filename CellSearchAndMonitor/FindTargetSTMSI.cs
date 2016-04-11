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
using System.IO;
using System.Runtime.InteropServices;
using DeviceMangerModule;
using System.Threading;
using System.Net.NetworkInformation;
using System.Xml;
using SendSms;

namespace CellSearchAndMonitor
{
    public partial class FindTargetSTMSI : WeifenLuo.WinFormsUI.Docking.DockContent, COM.ZCTT.AGI.Plugin.IPlugin
    {
        static bool EventLoad = false;
        static bool FTSTMSIRunning = false;
        static bool Sms_Connection = false;
        public static bool FTSTMSIRunning1
        {
            get { return FindTargetSTMSI.FTSTMSIRunning; }
            set { FindTargetSTMSI.FTSTMSIRunning = value; }
        }
        static bool Again = false;
        static bool Release = false;
        public FindTargetSTMSI()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Global.tempClass.OrtFStartEvent += new Class1.OrtFStartHander(FTSTMSIBtnUnable);
            Global.tempClass.OrtFStopEvent += new Class1.OrtFStopHander(FTSTMSIBtnEnable);
            Global.tempClass.CSAMStartEvent += new Class1.CSAMStartHander(FTSTMSIBtnUnable);
            Global.tempClass.CSAMStopEvent += new Class1.CSAMStopHander(FTSTMSIBtnEnable);
        }

        public static FindTargetSTMSI FTSTMSIForm;
        #region 实现IPlugin接口
        public new void Load()
        {
            if (FTSTMSIForm == null || FTSTMSIForm.IsDisposed)
            {
                FTSTMSIForm = new FindTargetSTMSI();
                FTSTMSIForm.ShowHint = DockState.Document;
                FTSTMSIForm.Show(GloabeControl.findSTMSIDockPanel);
                FTSTMSIForm.DockState = DockState.Document;

            }
            else
            {
                FTSTMSIForm.ShowHint = DockState.Document;
                FTSTMSIForm.Show(GloabeControl.findSTMSIDockPanel);
                FTSTMSIForm.DockState = DockState.Document;
                //debugDisplay.DockAreas = DockAreas.Document;
            }

            //sTmsiGraph.ShowDialog();
            if (EventLoad == false)
            {
                Global.tempClass.SendDataToProTrackEvent += this.DataReceived;
                Global.tempClass.StartEvent += new Class1.StartHandler(StartEvent);
                Global.tempClass.CloseDisplayFormEvent += new Class1.CloseDisplayFormHander(CloseForm);
                Global.tempClass.SendACKToCellScanEvent += new Class1.DeviceSendACKToCellScan(ACKHandler);
                EventLoad = true;
            }

            FTSTMSIForm.groupBox5.Height = FTSTMSIForm.groupBox1.Height = (FTSTMSIForm.groupBox2.Top - FTSTMSIForm.groupBox5.Top)/2;
            FTSTMSIForm.groupBox1.Top = FTSTMSIForm.groupBox5.Bottom;
            //FTSTMSIForm.ModifyTargetButton.Enabled = false;
            //FTSTMSIForm.TriggerComboBox.Items.Add("短信");
            //FTSTMSIForm.TriggerComboBox.Items.Add("电话");
            //FTSTMSIForm.TriggerComboBox.Items.Add("Ping");
            //FTSTMSIForm.TriggerComboBox.Items.Add("寻呼");
            //FTSTMSIForm.TriggerComboBox.SelectedItem = FTSTMSIForm.TriggerComboBox.Items[0];
            FTSTMSIForm.SendTimesLabel.Text = FTSTMSIForm.NullLabel.Text = FTSTMSIForm.label6.Text = FTSTMSIForm.RsrpLabel.Text = FTSTMSIForm.RsrpLabel2.Text 
                = FTSTMSIForm.RsrpMsgLabel.Text = FTSTMSIForm.RsrpMsgLabel2.Text =  "";
            FTSTMSIForm.ResideColor1.Text = FTSTMSIForm.ResideColor2.Text = FTSTMSIForm.RsrpColor1.Text = FTSTMSIForm.RsrpColor2.Text = "    ";
            FTSTMSIForm.MesThresholdText.Text = "50";
            FTSTMSIForm.EffectiveTimeText.Text = "6";
            FTSTMSIForm.GetSTMSIButton.Enabled = false;
            FTSTMSIForm.StopFindingSTMSIButton.Enabled = false;
            FTSTMSIForm.checkBox2.Checked = true;
            FTSTMSIForm.checkBox3.Checked = true;
            //CellSearchAndMonitorUnions.ComboBoxStatus = FTSTMSIForm.TriggerComboBox.Text;
            LoadData();
        }

        /// <summary>
        /// 关闭当前窗口执行的事件
        /// </summary>
        private void CloseForm()
        {

            if (FTSTMSIRunning == true)
            {
                foreach (Device d in DeviceManger.deviceList)
                {
                        d.SendRexAnt = false;
                }
                FTSTMSIForm.StopFindingSTMSI(true,"");
            }
            SaveData();
            SaveSTMSI();
            FTSTMSIForm.Close();
            FTSTMSIForm.Dispose(true);            
        }


        /// <summary>
        /// “开始测试”执行的事件：
        /// 清空所有数据以及界面上的内容
        /// </summary>
        /// <param name="sender">sender为AGIInterface.Class1</param>
        /// <param name="e">自定义的事件参数</param>
        private void StartEvent(object sender, AGIInterface.CustomDataEvtArg e)
        {
            if (FTSTMSIForm != null)
            {
                //读取两个表格的数据
                //sTmsiGraph.dgvDisplay.Rows.Clear();
            }
        }
        #endregion

        
        /// <summary>
        /// 程序收到数据的处理方法
        /// </summary>
        /// <param name="sender">AGIInterface.Class1</param>
        /// <param name="e">自定义的事件参数</param>
        private DateTime SendMessageTime;
        private double EffectiveTime;
        private TimeSpan TimeDif;
        bool MsgUseless = false;
        private int SendMsgCount = 0;
        static int stmsiCount = 0;
        static int nullCount = 0;

        private static Thread RebootThread;
        private static Thread RebootThread2;
        private static Thread ReSendThread;
        private static Thread ReSendThread2;
        public void DataReceived(object sender, AGIInterface.CustomDataEvtArg e)
        {
            //将AG_PC_PROTOCOL_DATA显示在界面上
            if (FTSTMSIForm.InvokeRequired)
            {
                try
                {
                    if (FTSTMSIForm != null)
                    {
                        //proTracDisplay.Invoke(new Class1.DeviceSendDataToProTrackHandler(DataReceived), sender, e);
                        FTSTMSIForm.BeginInvoke(new Class1.DeviceSendDataToProTrackHandler(DataReceived), sender, e);
                    }
                    else
                    {}
                }
                catch (Exception ex)
                {
                    Console.WriteLine(">>>Message= " + ex.Message + "\r\n StrackTrace: " + ex.StackTrace);
                }
            }
            else
            {
                if (Global.CurrentSender == FTSTMSIForm.Name)//e.deivceName == Global.GCurrentDevice)
                    try
                    {
                        //TimeDif = SendMessageTime - DateTime.Now;
                        
                        
                            UInt16 messageType = 0;
                            messageType = BitConverter.ToUInt16(e.data, 6);
                            UInt16 messageLength = 0;
                            messageLength = BitConverter.ToUInt16(e.data, 10);
                            //判断消息长度
                            if (messageLength * 4 != e.data.Length - 12)
                            {
                                return;
                            }

                            if (messageType == COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_UE_CAPTURE_IND_MSG_TYPE)
                            {   
                                FTSTMSIForm.TimeDif = DateTime.Now - FTSTMSIForm.SendMessageTime;
                                if (FTSTMSIForm.MsgUseless == true || (FTSTMSIForm.MsgUseless == false && ((FTSTMSIForm.checkBox3.Checked == false) || (FTSTMSIForm.checkBox3.Checked == true && FTSTMSIForm.SendMsgCount > 0 && FTSTMSIForm.TimeDif.Seconds <= FTSTMSIForm.EffectiveTime))))
                                {
                                    STMSIStruct stru_STMSIStruct = new STMSIStruct();

                                    L2P_AG_UE_CAPTURE_IND L2pAgUeCaptureInd = new L2P_AG_UE_CAPTURE_IND();
                                    //得到结构体大小
                                    int isize = Marshal.SizeOf(L2pAgUeCaptureInd);
                                    //比较结构体大小
                                    if (isize > e.data.Length)
                                    {
                                        return;
                                    }
                                    //分配结构体大小的内存空间
                                    IntPtr Intptr = Marshal.AllocHGlobal(isize);
                                    //将byte数组拷贝到内存空间
                                    Marshal.Copy(e.data, 0, Intptr, isize);
                                    //将内存空间转换为目标结构体
                                    //Marshal.PtrToStructure(intptr, ProtocolData);
                                    L2pAgUeCaptureInd = (L2P_AG_UE_CAPTURE_IND)Marshal.PtrToStructure(Intptr, typeof(L2P_AG_UE_CAPTURE_IND));
                                    //释放内存空间
                                    Marshal.FreeHGlobal(Intptr);
                                    string stmsi = "";
                                    for (int i = 1; i <= 32; i = i * 2)
                                    {
                                        if ((L2pAgUeCaptureInd.mstUECaptureInfo.mu8UEIDTypeFlg & i) == i)
                                        {
                                            switch (i)
                                            {
                                                case 32:
                                                    stru_STMSIStruct.mMec = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[5];
                                                    Byte[] sTMSI = new Byte[] { 0, 0, 0, 0 };
                                                    sTMSI[3] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[6];
                                                    sTMSI[2] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[7];
                                                    sTMSI[1] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[8];
                                                    sTMSI[0] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[9];
                                                    stru_STMSIStruct.sTMSI = sTMSI;
                                                    stru_STMSIStruct.mu8EstCause = L2pAgUeCaptureInd.mstUECaptureInfo.mu8Pading1;
                                                    //Global.GStmsiList.Add(stru_STMSIStruct);
                                                    stmsi = stru_STMSIStruct.mMec.ToString("X2") + stru_STMSIStruct.sTMSI[0].ToString("X2") + stru_STMSIStruct.sTMSI[1].ToString("X2") + stru_STMSIStruct.sTMSI[2].ToString("X2") + stru_STMSIStruct.sTMSI[3].ToString("X2");
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    //count
                                    if (stmsi == "")
                                    {
                                        nullCount++;
                                        stmsiCount++;
                                        FTSTMSIForm.NullLabel.Text = nullCount.ToString();
                                        double rate = (double)nullCount / (double)stmsiCount;
                                        if (stmsiCount > 50 && rate > 0.2)
                                        {
                                           FTSTMSIForm.label6.Text = "该处重叠覆盖，不适合工作！";
                                        }
                                        return;
                                    }
                                    else
                                        stmsiCount++;

                                    System.Diagnostics.Debug.WriteLine(" >>>>>>>>>>>>>receive a stmsi:  " + stmsi);

                                    if ((stru_STMSIStruct.mu8EstCause == 0x02 && FTSTMSIForm.checkBox1.Checked == false && FTSTMSIForm.checkBox2.Checked == true) 
                                        || (FTSTMSIForm.checkBox1.Checked == true && FTSTMSIForm.checkBox2.Checked == false && stru_STMSIStruct.mu8EstCause == 0x04) 
                                        || (FTSTMSIForm.checkBox1.Checked == true && FTSTMSIForm.checkBox2.Checked == false && stru_STMSIStruct.mu8EstCause == 0x03) 
                                        || (FTSTMSIForm.checkBox1.Checked == true && FTSTMSIForm.checkBox2.Checked == true && (stru_STMSIStruct.mu8EstCause == 0x02 
                                        || stru_STMSIStruct.mu8EstCause == 0x03 || stru_STMSIStruct.mu8EstCause == 0x04)))
                                    {

                                        CellSearchAndMonitorUnions.TargetListViewInfo info = new CellSearchAndMonitorUnions.TargetListViewInfo();
                                        info.stmsi = stmsi;
                                        foreach (ListViewItem item in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                                        {
                                            if (e.deivceName == item.SubItems[4].Text.Trim())
                                            {
                                                info.freq = item.SubItems[1].Text.Trim();
                                                info.earfcn = item.SubItems[2].Text.Trim();
                                                info.pci = item.SubItems[3].Text.Trim();
                                                info.tai = item.SubItems[5].Text.Trim();
                                                info.ecgi = item.SubItems[6].Text.Trim();
                                                info.plmn = item.SubItems[7].Text.Trim();
                                                info.band = (String)item.Tag;
                                            }
                                        }
                                        info.count = 1;
                                        info.time = System.DateTime.Now.ToLongTimeString();
                                        if (CellSearchAndMonitorUnions.AllSTMSICount.ContainsKey(stmsi))
                                        {
                                            CellSearchAndMonitorUnions.AllSTMSICount[stmsi].count++;
                                            CellSearchAndMonitorUnions.AllSTMSICount[stmsi].time = System.DateTime.Now.ToLongTimeString();
                                        }
                                        else
                                        {
                                            CellSearchAndMonitorUnions.AllSTMSICount.Add(stmsi, info);
                                        }

                                        //if (FTSTMSIForm.TopTenSTMSICount.ContainsKey(stmsi))
                                        //{
                                        //    //FTSTMSIForm.TopTenSTMSICount[stmsi].count++;
                                        //}
                                        //else
                                        //{
                                        //    if (FTSTMSIForm.TopTenSTMSICount.Count < 10)
                                        //    {
                                        //        FTSTMSIForm.TopTenSTMSICount.Add(stmsi, info);
                                        //    }
                                        //    else
                                        //    {
                                        //        if (FTSTMSIForm.AllSTMSICount[stmsi].count > FTSTMSIForm.TopTenSTMSICount.Last().Value.count)
                                        //        {
                                        //            FTSTMSIForm.TopTenSTMSICount.Remove(FTSTMSIForm.TopTenSTMSICount.Last().Key);
                                        //            FTSTMSIForm.TopTenSTMSICount.Add(stmsi, AllSTMSICount[stmsi]);
                                        //        }
                                        //    }
                                        //}

                                        var temp = CellSearchAndMonitorUnions.AllSTMSICount.OrderByDescending(
                                            item => item.Value.count
                                            );
                                        Dictionary<String, CellSearchAndMonitorUnions.TargetListViewInfo> t = new Dictionary<String, CellSearchAndMonitorUnions.TargetListViewInfo>();
                                        temp.ToList().ForEach(item => t.Add(item.Key, item.Value));
                                        CellSearchAndMonitorUnions.AllSTMSICount = t;

                                        FTSTMSIForm.TargetInfoListView.Items.Clear();
                                        foreach (var item in CellSearchAndMonitorUnions.AllSTMSICount)
                                        {
                                            ListViewItem LVItem = new ListViewItem(new string[3]);
                                            LVItem.SubItems[0].Text = (FTSTMSIForm.TargetInfoListView.Items.Count + 1).ToString();
                                            LVItem.SubItems[1].Text = item.Key.ToString();
                                            LVItem.SubItems[2].Text = item.Value.count.ToString();
                                            //LVItem.SubItems[3].Text = item.Value.time.ToString();
                                            //LVItem.SubItems[4].Text = item.Value.freq.ToString();
                                            //LVItem.SubItems[5].Text = item.Value.earfcn.ToString();
                                            //LVItem.SubItems[6].Text = item.Value.pci.ToString();
                                            
                                            //LVItem.Tag = item.Value.band;

                                            FTSTMSIForm.TargetInfoListView.Items.Add(LVItem);
                                        }
                                        ListViewItem Item = new ListViewItem(new string[6]);
                                        Item.SubItems[0].Text = (FTSTMSIForm.LivedInfoListView.Items.Count + 1).ToString();
                                        Item.SubItems[1].Text = info.stmsi;
                                        Item.SubItems[2].Text = info.time;
                                        Item.SubItems[3].Text = info.freq;
                                        Item.SubItems[4].Text = info.earfcn;
                                        Item.SubItems[5].Text = info.pci;
                                        FTSTMSIForm.LivedInfoListView.Items.Add(Item);
                                        //FTSTMSIForm.LivedInfoListView.Items[FTSTMSIForm.LivedInfoListView.Items.Count - 1].Selected = true;
                                        FTSTMSIForm.LivedInfoListView.Items[FTSTMSIForm.LivedInfoListView.Items.Count - 1].EnsureVisible();
                                    }
                                }
                                //FTSTMSIForm.TargetInfoListViewRefresh();
                            }
                            else if (messageType == COM.ZCTT.AGI.Common.AGIMsgDefine.L1_PHY_COMMEAS_IND_MSG_TYPE)
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
                                    double crsrp = stu_CRS_RSRPQI_INFO.mstCrs0RsrpqiInfo.ms16CRS_RP * 0.125;

                                    if (e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[0].SubItems[4].Text)
                                    {
                                        FTSTMSIForm.RsrpLabel.Text = crsrp.ToString() + "dBm";
                                        if (crsrp >= -90)
                                            FTSTMSIForm.RsrpColor1.BackColor = System.Drawing.Color.Green;
                                        else if (crsrp < -90 && crsrp > -100)
                                            FTSTMSIForm.RsrpColor1.BackColor = System.Drawing.Color.Yellow;
                                        else if (crsrp < -100 && crsrp > -110)
                                            FTSTMSIForm.RsrpColor1.BackColor = System.Drawing.Color.Orange;
                                        else if (crsrp < -110)
                                            FTSTMSIForm.RsrpColor1.BackColor = System.Drawing.Color.Red;
                                    }
                                    else if (CellSearchAndMonitorUnions.CellMonitorLV.Items.Count > 1 && e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[1].SubItems[4].Text)
                                    {
                                        FTSTMSIForm.RsrpLabel2.Text = crsrp.ToString() + "dBm";
                                        if (crsrp >= -90)
                                            FTSTMSIForm.RsrpColor2.BackColor = System.Drawing.Color.Green;
                                        else if (crsrp < -90 && crsrp > -100)
                                            FTSTMSIForm.RsrpColor2.BackColor = System.Drawing.Color.Yellow;
                                        else if (crsrp < -100 && crsrp > -110)
                                            FTSTMSIForm.RsrpColor2.BackColor = System.Drawing.Color.Orange;
                                        else if (crsrp < -110)
                                            FTSTMSIForm.RsrpColor2.BackColor = System.Drawing.Color.Red;
                                    }
                                    
                                    //FTSTMSIForm.RSRPLabel.Text = crsrp.ToString() + "dBm";
                                }
                            }
                            else if (messageType == COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_CELL_CAPTURE_IND_MSG_TYPE)
                            {
                                Dictionary<string, string> result = Decode.dataBackControl_SendMIBToDisplayEvent(e);
                                if ((result["TAC"] == "0x0" || result["CellID"] == "0x0" || result["RSRQ"] == "NULL" || result["RSRP"] == "NULL"))
                                {
                                    Device temDevice = DeviceManger.FindDevice(e.deivceName);
                                    if (e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[0].SubItems[4].Text)
                                        FTSTMSIForm.ResideColor1.BackColor = System.Drawing.Color.Red;
                                    else if (CellSearchAndMonitorUnions.CellMonitorLV.Items.Count > 1 && e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[1].SubItems[4].Text)
                                        FTSTMSIForm.ResideColor2.BackColor = System.Drawing.Color.Red;
                                    if (temDevice.Again == false)
                                    {
                                        temDevice.Again = true;
                                       
                                    }
                                    else if (temDevice.Again == true)
                                    {
                                        temDevice.Again = false;
                                        if (e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[0].SubItems[4].Text)
                                            FTSTMSIForm.RsrpLabel.Text = result["PCI"] + " 小区驻留失败！";
                                        else if (CellSearchAndMonitorUnions.CellMonitorLV.Items.Count > 1 && e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[1].SubItems[4].Text)
                                            FTSTMSIForm.RsrpLabel2.Text = result["PCI"] + " 小区驻留失败！";

                                        if (temDevice.Reboot == false)
                                        {
                                            temDevice.Reboot = true;

                                            if (e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[0].SubItems[4].Text)
                                            {
                                                DeviceManger DM = new DeviceManger();
                                                CrossThreadOperationControl CrossThread = delegate()
                                                {
                                                    try
                                                    {
                                                        RebootThread = new Thread(() => DM.RebootThreadFunc(temDevice));
                                                        RebootThread.Start();
                                                        ReSendThread = new Thread(() => ReSendFun(temDevice));
                                                        ReSendThread.Start();
                                                    }
                                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                                };
                                                FTSTMSIForm.Invoke(CrossThread);

                                            }
                                            else if (CellSearchAndMonitorUnions.CellMonitorLV.Items.Count > 1 && e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[1].SubItems[4].Text)
                                            {
                                                DeviceManger DM = new DeviceManger();
                                                CrossThreadOperationControl CrossThread = delegate()
                                                {
                                                    try
                                                    {
                                                        RebootThread2 = new Thread(() => DM.RebootThreadFunc(temDevice));
                                                        RebootThread2.Start();
                                                        ReSendThread2 = new Thread(() => ReSendFun(temDevice));
                                                        ReSendThread2.Start();
                                                    }
                                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                                };
                                                FTSTMSIForm.Invoke(CrossThread);
                                                
                                            }
                                            int releaseCount = 0;
                                            temDevice.Release = false;
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
                                                FTSTMSIForm.StopFindingSTMSIButton.Enabled = false;
                                            }
                                            MessageBox.Show(result["PCI"] + " 小区驻留失败，努力搜索中！");
                                            return;
                                        }
                                        else
                                        {
                                            MessageBox.Show(result["PCI"] + " 小区驻留失败！");
                                        }
                                    }
                                    CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                                    {
                                        StopFindingSTMSI(false, e.deivceName);
                                    };
                                    FTSTMSIForm.Invoke(CrossThreadInfoRefresh);
                                }
                                else
                                {
                                    
                                    if (e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[0].SubItems[4].Text)
                                    {
                                        FTSTMSIForm.RsrpLabel.Text = result["RSRP"] + "dBm";
                                        FTSTMSIForm.ResideColor1.BackColor = System.Drawing.Color.Green;
                                        if (Convert.ToInt16(result["RSRP"]) < -120)
                                        {
                                            FTSTMSIForm.RsrpMsgLabel.Text = result["PCI"] + " 小区下行信号强度较差！";
                                        }
                                    }
                                    else if (CellSearchAndMonitorUnions.CellMonitorLV.Items.Count > 1 && e.deivceName == CellSearchAndMonitorUnions.CellMonitorLV.Items[1].SubItems[4].Text)
                                    {
                                        FTSTMSIForm.RsrpLabel2.Text = result["RSRP"] + "dBm";
                                        FTSTMSIForm.ResideColor2.BackColor = System.Drawing.Color.Green;
                                        if (Convert.ToInt16(result["RSRP"]) < -120)
                                        {
                                            FTSTMSIForm.RsrpMsgLabel2.Text = result["PCI"] + " 小区下行信号强度较差！";
                                        }
                                    }
                                    DeviceManger.FindDevice(e.deivceName).Again = false;
                                    DeviceManger.FindDevice(e.deivceName).Reboot = false;
                                    //Thread.Sleep(2000);
                                }
                            }
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(">>>Message= " + ex.Message + "\r\n StrackTrace: " + ex.StackTrace);
                        return;
                    }
            }
        }
        public void ACKHandler(object sender, AGIInterface.CustomDataEvtArg e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Class1.DeviceSendACKToCellScan(ACKHandler), sender, e);
                }
                catch
                {}
            }
            else
            {
                if (Global.CurrentSender == FTSTMSIForm.Name)
                {
                    
                    if (DeviceManger.FindDevice(e.deivceName).Again == true)
                    {
                        Thread.Sleep(1000);
                        CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                        {
                            FindSTMSIStart(sender, e);
                        };
                        FTSTMSIForm.Invoke(CrossThreadInfoRefresh);
                    }
                    else if(stop == true)
                    {
                        CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                        {
                            Global.tempClass.FTSTMSIStop();
                            FTSTMSIForm.StopFindingSTMSIButton.Enabled = false;
                            foreach (Device dev in DeviceManger.deviceList)
                            {
                                dev.SendRexAnt = false;
                            }
                            FTSTMSIRunning = false;
                            stop = false;
                        };
                        FTSTMSIForm.Invoke(CrossThreadInfoRefresh);
                        
                    }
                }
            }
        }

        private void ReSendFun(Device temDevice)
        {
            try
            {
                Thread.Sleep(70000);
                while (true)
                {
                    if (temDevice.ConnectionState == (byte)Global.DeviceStateValue.Connecting)
                    {
                        Thread.Sleep(5000);
                        CustomDataEvtArg e = new CustomDataEvtArg();
                        e.deivceName = temDevice.DeviceName;
                        CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                        {
                            FindSTMSIStart(FTSTMSIForm, e);
                            FTSTMSIForm.StopFindingSTMSIButton.Enabled = true;
                        };
                        FTSTMSIForm.Invoke(CrossThreadInfoRefresh);
                        return;
                    }
                }
            }
            catch (Exception ex) { }
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
        #region 目标号码信息
        private struct TargetInfo
        {
            public string name { get; set; }
            public string tel { get; set; }
            public string ip { get; set; }
            public string imsi { get; set; }
        }
        #endregion

        private void AddTargetButton_Click(object sender, EventArgs e)
        {
            ListViewItem item = ListViewItemDataModify();
            if (item != null)
                MonitorListView.Items.Add(item);
        }

        private void MonitorListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TargetTelText.Text = MonitorListView.FocusedItem.SubItems[1].Text;
                CellSearchAndMonitorUnions.TargetTel = FTSTMSIForm.TargetTelText.Text;
                //TargetIPText.Text = MonitorListView.FocusedItem.SubItems[2].Text;
                if(!FTSTMSIForm.StopFindingSTMSIButton.Enabled)
                    FTSTMSIForm.GetSTMSIButton.Enabled = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                FTSTMSIForm.contextMenuStrip.Items.Clear();
                currentFocusedListView = FTSTMSIForm.MonitorListView;
                contextMenuStrip.Items.Add("删除");
                contextMenuStrip.Items.Add("全部删除");
                FTSTMSIForm.contextMenuStrip.Show(MousePosition.X, MousePosition.Y);
            }

        }
        private ListView currentFocusedListView;
        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch ((e.ClickedItem as ToolStripMenuItem).Text)
            {
                case "删除":
                ListView.SelectedListViewItemCollection selectedItems = currentFocusedListView.SelectedItems;
                int j = selectedItems.Count;
                for (int i = 0; i < j; i++)
                {
                    currentFocusedListView.Items.Remove(selectedItems[0]);
                }
                if (MonitorListView.Items.Count == 0) MonitorListView.Enabled = false;
                //CellSearchAndMonitorUnions.CellMonitorLV = MonitorListView;
                    break;
                case "全部删除":
                    MonitorListView.Items.Clear();
                    break;

                default:
                    break;
            }
            
        }

        private void ModifyTargetButton_Click(object sender, EventArgs e)
        {
            ListViewItem LVItem = ListViewItemDataModify();
            if (LVItem == null) return;

            for (int i = 0; i < LVItem.SubItems.Count; i++)
            {
                MonitorListView.SelectedItems[0].SubItems[i].Text = LVItem.SubItems[i].Text;
            }

            TargetTelText.Text = MonitorListView.FocusedItem.SubItems[1].Text;
            //TargetIPText.Text = MonitorListView.FocusedItem.SubItems[2].Text;
        }

        private static ListViewItem ListViewItemDataModify()
        {
            TargetInfo info = new TargetInfo();
            info.name = FTSTMSIForm.NameText.Text.Trim();
            info.tel = FTSTMSIForm.TelText.Text.Trim();
            //info.ip = FTSTMSIForm.IPText.Text.Trim();
            //info.imsi = FTSTMSIForm.IMSIText.Text.Trim();

            bool CanModify = false;

            //switch (FTSTMSIForm.TriggerComboBox.Text)
            //{
            //    case "短信":
                //case "电话":
                    CanModify = !String.IsNullOrWhiteSpace(info.name) && CellSearchAndMonitorUnions.IsPhoneNum(info.tel);
                    //break;
                //case "Ping":
                //    CanModify = !String.IsNullOrWhiteSpace(info.name) && CellSearchAndMonitorUnions.IsIPAddress(info.ip);
                //    break;
                //case "寻呼":
                    //CanModify = !String.IsNullOrWhiteSpace(info.name) && !String.IsNullOrWhiteSpace(info.imsi);
                    //break;
                //default:
                    //break;
            //}
            ListViewItem LVItem = null;
            if (CanModify)
            {
                LVItem = new ListViewItem(new string[2]);
                LVItem.SubItems[0].Text = info.name;
                LVItem.SubItems[1].Text = info.tel;
                //LVItem.SubItems[2].Text = info.ip;
                //LVItem.SubItems[3].Text = info.imsi;
            }

            return LVItem;
        }

        private void MonitorListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (MonitorListView.SelectedItems.Count > 0)
            {
                if (!FTSTMSIForm.StopFindingSTMSIButton.Enabled)
                    FTSTMSIForm.GetSTMSIButton.Enabled = true;
                //ModifyTargetButton.Enabled = true;
            }
            else
            {
                GetSTMSIButton.Enabled = false;
                //ModifyTargetButton.Enabled = false;
            }
        }

        #region 保存信息

        private void SaveData()
        {
            string FilePath = System.Windows.Forms.Application.StartupPath + "\\MonitorListData.bin";
            CellSearchAndMonitorUnions.DataSave(FTSTMSIForm.MonitorListView, FilePath);
            FilePath = System.Windows.Forms.Application.StartupPath + "\\TargetInfoListData.bin";
            CellSearchAndMonitorUnions.DataSave(FTSTMSIForm.TargetInfoListView, FilePath);
            FilePath = System.Windows.Forms.Application.StartupPath + "\\LivedInfoListData.bin";
            CellSearchAndMonitorUnions.DataSave(FTSTMSIForm.LivedInfoListView, FilePath);
        }
        #endregion
        #region 读取信息
        private void LoadData()
        {
            string FilePath = System.Windows.Forms.Application.StartupPath + "\\MonitorListData.bin";
            CellSearchAndMonitorUnions.DataLoad(FTSTMSIForm.MonitorListView, FilePath);
            FilePath = System.Windows.Forms.Application.StartupPath + "\\TargetInfoListData.bin";
            CellSearchAndMonitorUnions.DataLoad(FTSTMSIForm.TargetInfoListView, FilePath);
            FilePath = System.Windows.Forms.Application.StartupPath + "\\LivedInfoListData.bin";
            CellSearchAndMonitorUnions.DataLoad(FTSTMSIForm.LivedInfoListView, FilePath);
            FTSTMSIForm.LoadSTMSI();
        }
        #endregion

        private void TriggerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //switch (FTSTMSIForm.TriggerComboBox.Text)
            //{
                //case "短信":
                //case "电话":
                    FTSTMSIForm.TelText.Enabled = true;
                    //FTSTMSIForm.IPText.Enabled = false;
                    //FTSTMSIForm.IMSIText.Enabled = false;
                    //break;
                //case "Ping":
                //    FTSTMSIForm.IPText.Enabled = true;
                //    FTSTMSIForm.TelText.Enabled = false;
                //    FTSTMSIForm.IMSIText.Enabled = false;
                //    break;
                //case "寻呼":
                    //FTSTMSIForm.IMSIText.Enabled = true;
                    //FTSTMSIForm.TelText.Enabled = false;
                    //FTSTMSIForm.IPText.Enabled = false;
                    //break;
                //default:
                    //break;

            //}
        }

        private static Thread SendMessageOrPingThread;

        private void GetSTMSIButton_Click(object sender, EventArgs e)
        {
            FTSTMSIForm.GetSTMSIButton.Enabled = false;
           
            //switch (FTSTMSIForm.TriggerComboBox.Text)
            //{
                //case "短信":
                    //{
                        if (DeviceManger.deviceList.Count == 0)
                        {
                            MessageBox.Show("没有可用板卡！");
                            return;
                        }
                        //DeviceManger.flashDeviceConnect();
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
                        CellSearchAndMonitorUnions.AllSTMSICount.Clear();
                        CellSearchAndMonitorUnions.TopTenSTMSICount.Clear();
                        FTSTMSIForm.SendMsgCount = 0;
                        FTSTMSIForm.SendTimesLabel.Text = "0";
                        FTSTMSIForm.MsgUseless = false;
                        FTSTMSIForm.NullLabel.Text = FTSTMSIForm.label6.Text = FTSTMSIForm.RsrpLabel.Text 
                            = FTSTMSIForm.RsrpLabel2.Text = FTSTMSIForm.RsrpMsgLabel.Text = FTSTMSIForm.RsrpMsgLabel2.Text = "";
                        FTSTMSIForm.ResideColor1.BackColor = FTSTMSIForm.ResideColor2.BackColor  = FTSTMSIForm.RsrpColor1.BackColor = FTSTMSIForm.RsrpColor2.BackColor  = SystemColors.Control;
                        nullCount = stmsiCount = 0;
                        foreach (ListViewItem LVItem in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                        {
                            string earfcn = LVItem.SubItems[2].Text.Trim();
                            string pci = LVItem.SubItems[3].Text.Trim();
                            if (string.IsNullOrEmpty(earfcn) || string.IsNullOrEmpty(pci))
                            {
                                MessageBox.Show("监控板卡配置错误，请重新添加！");
                                return;
                            }
                                
                            CustomDataEvtArg CDEArgMes = CellSearchAndMonitorUnions.GenerateMessage(earfcn, pci, 2);
                            CDEArgMes.deivceName = LVItem.SubItems[4].Text.Trim();
                            SendRxAnt(sender,CDEArgMes.deivceName);
                            Global.CurrentSender = FTSTMSIForm.Name;
                            DeviceManger.FindDevice(CDEArgMes.deivceName).Release = true;
                            //Global.tempClass.Start(CellSearchSender, CDEArgMes);
                            Global.tempClass.SendDataToDevice(sender, CDEArgMes);
                        }
                       
                        CellSearchAndMonitorUnions.TargetTel = FTSTMSIForm.TargetTelText.Text;
                        CellSearchAndMonitorUnions.MesThresHold = FTSTMSIForm.MesThresholdText.Text;
                        if (CellSearchAndMonitorUnions.IsDigitalString(FTSTMSIForm.EffectiveTimeText.Text))
                            FTSTMSIForm.EffectiveTime = Convert.ToDouble(FTSTMSIForm.EffectiveTimeText.Text);
                        else
                            FTSTMSIForm.EffectiveTime = Convert.ToDouble(CellSearchAndMonitorUnions.MesThresHold);
                        //Thread.Sleep(3000);

                        SendMessageOrPingThread = new Thread(FTSTMSIForm.SendTextMessage);
                        SendMessageOrPingThread.Start();
                        Global.tempClass.FTSTMISIStart();
                        //stop = false;
                        FTSTMSIRunning = true;

                        FTSTMSIForm.LivedInfoListView.Items.Clear();
                        //break;
            //        }
            //    default:
            //        break;
            //}
            FTSTMSIForm.GetSTMSIButton.Enabled = false;
            FTSTMSIForm.StopFindingSTMSIButton.Enabled = true;
        }
        //private bool sendRexAnt = false;
        private void FindSTMSIStart(object sender, CustomDataEvtArg e)
        {
            //switch (FTSTMSIForm.TriggerComboBox.Text)
            //{
            //    case "短信":
            //        {
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
                        CellSearchAndMonitorUnions.AllSTMSICount.Clear();
                        CellSearchAndMonitorUnions.TopTenSTMSICount.Clear();
                        FTSTMSIForm.SendMsgCount = 0;
                        FTSTMSIForm.SendTimesLabel.Text = "0";
                        //FTSTMSIForm.MsgUseless = false;
                        foreach (ListViewItem LVItem in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                        {
                            if (LVItem.SubItems[4].Text.Trim() == e.deivceName)
                            {
                                string earfcn = LVItem.SubItems[2].Text.Trim();
                                string pci = LVItem.SubItems[3].Text.Trim();
                                if (string.IsNullOrEmpty(earfcn) || string.IsNullOrEmpty(pci))
                                {
                                    MessageBox.Show("监控板卡配置错误，请重新添加！");
                                    return;
                                }
                                CustomDataEvtArg CDEArgMes = CellSearchAndMonitorUnions.GenerateMessage(earfcn, pci, 2);
                                CDEArgMes.deivceName = e.deivceName;
                                Global.CurrentSender = FTSTMSIForm.Name;
                                DeviceManger.FindDevice(e.deivceName).Release = true;
                                
                                //Global.tempClass.Start(CellSearchSender, CDEArgMes);
                                Global.tempClass.SendDataToDevice(sender, CDEArgMes);
                                break;
                            }
                        }
                        //CellSearchAndMonitorUnions.TargetTel = FTSTMSIForm.TargetTelText.Text;
                        //CellSearchAndMonitorUnions.MesThresHold = FTSTMSIForm.MesThresholdText.Text;
                        if (CellSearchAndMonitorUnions.IsDigitalString(FTSTMSIForm.EffectiveTimeText.Text))
                            FTSTMSIForm.EffectiveTime = Convert.ToDouble(FTSTMSIForm.EffectiveTimeText.Text);
                        else
                            FTSTMSIForm.EffectiveTime = Convert.ToDouble(CellSearchAndMonitorUnions.MesThresHold);
                        //Thread.Sleep(3000);

                        if (DeviceManger.FindDevice(e.deivceName).Again == false)
                        {
                            //SendMessageOrPingThread.Abort();
                            //SendMessageOrPingThread = new Thread(FTSTMSIForm.SendTextMessage);
                            //SendMessageOrPingThread.Start();
                            //Global.tempClass.FTSTMISIStart();
                        }
                        //stop = false;
                        FTSTMSIRunning = true;
            //            break;
            //        }
            //    default:
            //        break;
            //}
            FTSTMSIForm.GetSTMSIButton.Enabled = false;
            FTSTMSIForm.StopFindingSTMSIButton.Enabled = true;
            
        }
        private void SendRxAnt(object sender,string dev)
        {
            CustomDataEvtArg EvtArg = CellSearchAndMonitorUnions.RxAntMsg(1);
            EvtArg.deivceName = dev;
            Global.tempClass.SendDataToDevice(sender, EvtArg);
            DeviceManger.FindDevice(EvtArg.deivceName).SendRexAnt = true;
        }
        //private void PingTarget()
        //{
        //    if (!CellSearchAndMonitorUnions.IsDigitalString(FTSTMSIForm.MesThresholdText.Text))
        //    {
        //        MessageBox.Show("请输入合法门限值！");
        //        return;
        //    }
        //    int SendMesThreshold = Convert.ToInt32(FTSTMSIForm.MesThresholdText.Text.Trim());

        //    int FailSendingCount = 0;

        //    string ip = FTSTMSIForm.TargetIPText.Text;
        //    Ping p = new Ping();
        //    PingOptions options = new PingOptions();
        //    options.DontFragment = true;
        //    string data = "TestData";
        //    byte[] buffer = Encoding.ASCII.GetBytes(data);
        //    int timeout = 1000;
        //    PingReply reply = p.Send(ip, timeout, buffer, options);
        //    if (reply.Status == IPStatus.Success)
        //    {				
        //        for (int i = 1; i < SendMesThreshold; i++)
        //        {
        //            Thread.Sleep(CellSearchAndMonitorUnions.PingTime);
        //            reply = p.Send(ip, timeout, buffer, options);
        //            if (reply.Status != IPStatus.Success)
        //                FailSendingCount++;                
        //        }
        //        MessageBox.Show("Ping发送失败共计 " + FailSendingCount + "次！");
        //    }
        //    else
        //        MessageBox.Show(ip + "： ping失败！！");

        //    CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
        //    {
        //        StopFindingSTMSI();
        //    };
        //    FTSTMSIForm.Invoke(CrossThreadInfoRefresh);
        //}

        #region 发送短消息

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

        //[DllImport("SendSms.dll", EntryPoint = "Init")]
        //public bool Init(string bs_ip, int bsport, int recvport);

        //[DllImport("SendSms.dll", EntryPoint = "send_msg_get")]
        //public void send_msg_get(int sms_mode, string centercode, string isdnstr);

        //[DllImport("SendSms.dll", EntryPoint = "SendCall")]
        //public void SendCall(string isdn, int time);

        //[DllImport("SendSms.dll", EntryPoint = "Get_Result")]
        //public int Get_Result();

        private delegate void CrossThreadOperationControl();
        
        private void SendTextMessage()
        {
            
            try
            {
                string threshold = "";
                string tel = "";
                CrossThreadOperationControl CrossThreadInfoGet = delegate()
                {
                    threshold = CellSearchAndMonitorUnions.MesThresHold;
                    tel = CellSearchAndMonitorUnions.TargetTel;

                };
                FTSTMSIForm.Invoke(CrossThreadInfoGet);
                if (!CellSearchAndMonitorUnions.IsDigitalString(threshold))
                {
                    MessageBox.Show("请输入合法门限值！");
                    return;
                }
                int SendMesThreshold = Convert.ToInt32(threshold);
                int smsModCB = CellSearchAndMonitorUnions.SmsModCB;
                String centerCode = CellSearchAndMonitorUnions.CenterCode;
                int FailSendingCount = 0;
                if (STM_Sms_Connection())
                {
                    //string MesText = CellSearchAndMonitorUnions.TestMessage;
                    Thread.Sleep(5000);
                    for (int i = 0; i < SendMesThreshold; i++)
                    {
                        //if (Sms_Send(tel, MesText) == 0)
                        //sendSms.SendCall(tel,20000);
                        CellSearchAndMonitorUnions.sendSms.send_msg_get(smsModCB, centerCode, tel);//13800100500
                        Thread.Sleep(4000);
                        int flg = CellSearchAndMonitorUnions.sendSms.Get_Result();
                        //MessageBox.Show("发送结果：" + flg);
                        if(flg != 0)
                        {
                            FailSendingCount++;
                            MessageBox.Show("发送失败！");
                        }
                        else
                        {
                            //MessageBox.Show("发送成功！");
                            //System.Console.WriteLine("发送成功");
                            CrossThreadOperationControl CrossThreadTimeRefresh = delegate()
                            {
                                FTSTMSIForm.SendMessageTime = DateTime.Now;
                                FTSTMSIForm.SendMsgCount++;
                                FTSTMSIForm.SendTimesLabel.Text = FTSTMSIForm.SendMsgCount.ToString();
                            };
                            FTSTMSIForm.Invoke(CrossThreadTimeRefresh);
                        }
                        Thread.Sleep(CellSearchAndMonitorUnions.TestMessageSendTime);
                    }

                    MessageBox.Show("短信发送失败共计 " + FailSendingCount + "次！");

                    //Sms_Disconnection();
                    //Sms_connection = false;
                    CrossThreadOperationControl CrossThreadInfoRefresh = delegate()
                    {
                        StopFindingSTMSI(true,"");
                    };
                    FTSTMSIForm.Invoke(CrossThreadInfoRefresh);

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
                    CrossThreadOperationControl CrossThreadInfoChange = delegate()
                    {
                        FTSTMSIForm.MsgUseless = true;
                    };
                    FTSTMSIForm.Invoke(CrossThreadInfoChange);

                    return false;
                }
            }
            else
                return true;
        }
        #endregion

        private void TargetInfoListViewRefresh()
        {
            
        }

        private void TargetInfoListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && FTSTMSIForm.TargetInfoListView.FocusedItem != null)
            {
                if (StopFindingSTMSIButton.Enabled == true)
                {
                    StopFindingSTMSI(true,"");
                    FTSTMSIForm.StopFindingSTMSIButton.Enabled = false;
                }
                
                FTSTMSIForm.TargetSTMSIText.Enabled = true;
                //CellSearchAndMonitorUnions.TargetInfoLVItem=FTSTMSIForm.TargetInfoListView.FocusedItem;
                //CellSearchAndMonitorUnions.ComboBoxStatus=FTSTMSIForm.TriggerComboBox.Text;
                FTSTMSIForm.TargetSTMSIText.Text = FTSTMSIForm.TargetInfoListView.FocusedItem.SubItems[1].Text;
                String temEarfcn = "";
                foreach (ListViewItem item in FTSTMSIForm.LivedInfoListView.Items)
                {
                    if (FTSTMSIForm.TargetInfoListView.FocusedItem.SubItems[1].Text == item.SubItems[1].Text)
                    {
                        temEarfcn = item.SubItems[4].Text;
                        break;
                    }
                }
                if (temEarfcn != "")
                    CellSearchAndMonitorUnions.TargetEarfcn = temEarfcn;
                Global.tempClass.OrientationFidingStart();
            }
        }

        private void StopFindingSTMSIButton_Click(object sender, EventArgs e)
        {
            StopFindingSTMSI(true,"");
            //Global.tempClass.FTSTMSIStop();
            FTSTMSIForm.StopFindingSTMSIButton.Enabled = false;
        }
        private static bool stop = false;
        private void StopFindingSTMSI(bool All, string device)
        {
            if (All)
            {
                int i = 0;
                foreach (Device d in DeviceManger.deviceList)
                {
                    if (d.ConnectionState == (byte)Global.DeviceStateValue.Connecting)
                    {
                        d.Again = false;
                        d.Reboot = false;
                    }
                }
                foreach (ListViewItem LVItem in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                {
                    if (i == CellSearchAndMonitorUnions.CellMonitorLV.Items.Count - 1)
                        stop = true;
                    string temdevice = LVItem.SubItems[4].Text.Trim();
                    Device dev = DeviceManger.FindDevice(temdevice);
                    if (dev.Release == true)
                    {
                        
                        AGIInterface.CustomDataEvtArg cusArg = new CustomDataEvtArg();
                        cusArg.data = new byte[] { 0, 0, 0, 0, 4, 2, 0x0c, 0x40, 1, 0, 0, 0 };//加消息头。。
                        cusArg.deivceName = dev.DeviceName;
                        Global.CurrentSender = FTSTMSIForm.Name;
                        Global.tempClass.SendDataToDevice(FTSTMSIForm, cusArg);

                        //foreach (ListViewItem LVItem in CellSearchAndMonitorUnions.CellMonitorLV.Items)
                        //{

                        //    Global.GCurrentDevice = LVItem.SubItems[6].Text.Trim();
                        //    cusArg.deivceName = Global.GCurrentDevice;
                        //    Global.CurrentSender = FTSTMSIForm.Name;
                        //    //Global.tempClass.Start(CellSearchSender, CDEArgMes);
                        //    Global.tempClass.SendDataToDevice(FTSTMSIForm, cusArg);
                        //}
                        //FTSTMSIForm.GetSTMSIButton.Enabled = true;


                        DeviceManger.FindDevice(temdevice).Release = false;
                    }
                    i++;
                }
                //if (Sms_connection == true)
                //{
                //    //Sms_Disconnection();
                //    Sms_connection = false;
                //}
                try
                {
                    if (SendMessageOrPingThread != null)
                        SendMessageOrPingThread.Abort();
                    if (RebootThread != null)
                        RebootThread.Abort();
                    if (RebootThread2 != null)
                        RebootThread2.Abort();
                    if (ReSendThread != null)
                        ReSendThread.Abort();
                    if (ReSendThread2 != null)
                        ReSendThread2.Abort();
                }
                catch (Exception e) {}
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
                            if (SendMessageOrPingThread != null)
                                SendMessageOrPingThread.Abort();
                            //if (RebootThread != null)
                            //    RebootThread.Abort();
                            //if (RebootThread2 != null)
                            //    RebootThread2.Abort();
                            //if (ReSendThread != null)
                            //    ReSendThread.Abort();
                            //if (ReSendThread2 != null)
                            //    ReSendThread2.Abort();
                        }
                        catch { }
                        stop = true;
                    }
                
                AGIInterface.CustomDataEvtArg cusArg = new CustomDataEvtArg();
                cusArg.data = new byte[] { 0, 0, 0, 0, 4, 2, 0x0c, 0x40, 1, 0, 0, 0 };//加消息头。。
                cusArg.deivceName = device;
                Global.CurrentSender = FTSTMSIForm.Name;
                Global.tempClass.SendDataToDevice(FTSTMSIForm, cusArg);

            }
        }

        private void TargetSTMSIText_TextChanged(object sender, EventArgs e)
        {
            CellSearchAndMonitorUnions.TargetSTMSI = FTSTMSIForm.TargetSTMSIText.Text;
            //CellSearchAndMonitorUnions.TargetInfoLVItem.SubItems[1].Text = TargetSTMSIText.Text;
        }

        private static bool FTSTMSIBtnStatus = false;
        private static int unableCount = 0;
        private static int enableCount = 0;
        private void FTSTMSIBtnUnable()
        {
            CrossThreadOperationControl CrossThreadChange = delegate()
            {
                if (unableCount == 0)
                {
                    FTSTMSIForm.MonitorListView.Enabled = false;
                    FTSTMSIBtnStatus = FTSTMSIForm.GetSTMSIButton.Enabled;
                    FTSTMSIForm.GetSTMSIButton.Enabled = false;
                }
                unableCount++;
                if (unableCount == 2)
                    unableCount = 0;
            };
            FTSTMSIForm.Invoke(CrossThreadChange);
        }
        private void FTSTMSIBtnEnable()
        {
            CrossThreadOperationControl CrossThreadChange = delegate()
            {
                
                if (unableCount == 0)
                {
                    if (FTSTMSIBtnStatus == true)
                        FTSTMSIForm.GetSTMSIButton.Enabled = true;
                    FTSTMSIForm.MonitorListView.Enabled = true;
                }
                unableCount++;
                if (unableCount == 2)
                    unableCount = 0;
            };
            FTSTMSIForm.Invoke(CrossThreadChange);
        }
        private void LoadSTMSI()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                String filePath = System.Windows.Forms.Application.StartupPath + "\\FTSTMSI.xml";
                xml.Load(filePath);
                XmlNode FTStmsi = xml.SelectSingleNode("FTSTMSI");
                XmlNode stmsi = FTStmsi.SelectSingleNode("STMSI");
                FTSTMSIForm.TargetSTMSIText.Text = stmsi.Attributes["Value"].InnerText;
                XmlNode Earfcn = FTStmsi.SelectSingleNode("Earfcn");
                CellSearchAndMonitorUnions.TargetEarfcn = Earfcn.Attributes["Value"].InnerText;

                XmlNode MesThreshold = FTStmsi.SelectSingleNode("MesThresholdText");
                FTSTMSIForm.MesThresholdText.Text = MesThreshold.Attributes["Value"].InnerText;
                XmlNode CheckBox1 = FTStmsi.SelectSingleNode("CheckBox1");
                if (CheckBox1.Attributes["Value"].InnerText == "true")
                    FTSTMSIForm.checkBox1.Checked = true;
                else FTSTMSIForm.checkBox1.Checked = false;
                XmlNode CheckBox2 = FTStmsi.SelectSingleNode("CheckBox2");
                if (CheckBox2.Attributes["Value"].InnerText == "true")
                    FTSTMSIForm.checkBox2.Checked = true;
                else FTSTMSIForm.checkBox2.Checked = false;
                XmlNode CheckBox3 = FTStmsi.SelectSingleNode("CheckBox3");
                if (CheckBox3.Attributes["Value"].InnerText == "true")
                    FTSTMSIForm.checkBox3.Checked = true;
                else FTSTMSIForm.checkBox3.Checked = false;

                XmlNode EffectiveTime = FTStmsi.SelectSingleNode("EffectiveTimeText");
                FTSTMSIForm.EffectiveTimeText.Text = EffectiveTime.Attributes["Value"].InnerText;

                if (string.IsNullOrEmpty(FTSTMSIForm.TargetSTMSIText.Text) == false)
                {
                    FTSTMSIForm.TargetSTMSIText.Enabled = true;
                    Global.tempClass.OrientationFidingStart();
                }
            }
            catch
            {

            }
        }
        private void SaveSTMSI()
        {
            try
            {
                String filePath = System.Windows.Forms.Application.StartupPath + "\\FTSTMSI.xml";
                //File.Create(filePath);
                XmlDocument xml = new XmlDocument();
                XmlDeclaration xmldecl = xml.CreateXmlDeclaration("1.0", "gb2312", null);
                xml.AppendChild(xmldecl);

                //加入一个根元素
                XmlElement ftstmsi = xml.CreateElement("", "FTSTMSI", "");
                xml.AppendChild(ftstmsi);
                //XmlNode node = xml.SelectSingleNode("SystemConfig");//查找<PositionAndSize> 

                //创建一个<Position>节点 
                XmlElement stmsi = xml.CreateElement("STMSI");
                stmsi.SetAttribute("Value", FTSTMSIForm.TargetSTMSIText.Text);
                ftstmsi.AppendChild(stmsi);

                XmlElement Earfcn = xml.CreateElement("Earfcn");
                Earfcn.SetAttribute("Value", CellSearchAndMonitorUnions.TargetEarfcn);
                ftstmsi.AppendChild(Earfcn);

                XmlElement MesThreshold = xml.CreateElement("MesThresholdText");
                MesThreshold.SetAttribute("Value", FTSTMSIForm.MesThresholdText.Text);
                ftstmsi.AppendChild(MesThreshold);

                XmlElement CheckBox1 = xml.CreateElement("CheckBox1");
                if(FTSTMSIForm.checkBox1.Checked)
                    CheckBox1.SetAttribute("Value", "true");
                else CheckBox1.SetAttribute("Value", "false");
                ftstmsi.AppendChild(CheckBox1);

                XmlElement CheckBox2 = xml.CreateElement("CheckBox2");
                if (FTSTMSIForm.checkBox2.Checked)
                    CheckBox2.SetAttribute("Value", "true");
                else CheckBox2.SetAttribute("Value", "false");
                ftstmsi.AppendChild(CheckBox2);

                XmlElement CheckBox3 = xml.CreateElement("CheckBox3");
                if (FTSTMSIForm.checkBox3.Checked)
                    CheckBox3.SetAttribute("Value", "true");
                else CheckBox3.SetAttribute("Value", "false");
                ftstmsi.AppendChild(CheckBox3);

                XmlElement EffectiveTime = xml.CreateElement("EffectiveTimeText");
                EffectiveTime.SetAttribute("Value", FTSTMSIForm.EffectiveTimeText.Text);
                ftstmsi.AppendChild(EffectiveTime);

                xml.Save(filePath);
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
