using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using COM.ZCTT.AGI.Common;
using System.Runtime.InteropServices;
using AGIInterface;
using SendSms;

namespace CellSearchAndMonitor
{
    public class CellSearchAndMonitorUnions
    {
        public struct BandAndEarfcn
        {
            public string Band { get; set; }
            public string Freq { get; set; }
            public string Earfcn { get; set; }
        }
        public static Dictionary<string, List<BandAndEarfcn> > OPCellSearchInfoDic = new Dictionary<string, List<BandAndEarfcn> >();

        #region 目标信息输出struct
        public class TargetListViewInfo
        {
            public string stmsi { get; set; }
            public string freq { get; set; }
            public string earfcn { get; set; }
            public string pci { get; set; }
            public string tai { get; set; }
            public string plmn { get; set; }
            public string ecgi { get; set; }
            public string band { get; set; }
            public UInt32 count { get; set; }
            public string time { get; set; }
        }
        #endregion
        public static Dictionary<String, TargetListViewInfo> AllSTMSICount = new Dictionary<String, TargetListViewInfo>();
        public static Dictionary<String, TargetListViewInfo> TopTenSTMSICount = new Dictionary<String, TargetListViewInfo>();

        public static int SmsModCB;
        public static String CenterCode;
        public static int TestMessageSendTime;
        public static int PingTime;
        public static string TestMessage;
        public static int OriRefreshTime;
        public static int TestSINRThreshold;

        public static String TargetTel;
        public static String MesThresHold;

        public static System.Windows.Forms.ListView CellMonitorLV = null;
        //public static System.Windows.Forms.ListViewItem TargetInfoLVItem = null;
        public static string ComboBoxStatus;
        public static uint UeSilenceCheckTimer;
        public static byte RxAntNum;
        public static string TargetSTMSI;
        public static string TargetEarfcn;

        public static bool Sms_connection = false;
        public static SendSmsClass sendSms = new SendSmsClass();
        /// <summary>
        /// 检测字符是否由整数组成
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsDigitalString(string str)
        {
            return Regex.IsMatch(str, @"^-?\d+$");
        }

        public static bool IsIPAddress(string str)
        {
            return Regex.IsMatch(str, @"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static bool IsPhoneNum(string str)
        {
            return Regex.IsMatch(str, @"(1[0-9])\d{9}$");
        }

        public static bool IsIntOrDoubleString(string str)
        {
            return Regex.IsMatch(str, @"^-?\d+(\.\d)?$");
        }

        #region 保存信息
        public static void DataSave(System.Windows.Forms.ListView SavingListView, string FilePath)
        {
            if (String.IsNullOrEmpty(FilePath)) return;

            if (File.Exists(FilePath)) File.Delete(FilePath);

            try
            {
                FileStream fs = new FileStream(FilePath, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);

                foreach (System.Windows.Forms.ListViewItem LVItem in SavingListView.Items)
                {
                    foreach (System.Windows.Forms.ListViewItem.ListViewSubItem LVSubItem in LVItem.SubItems)
                    {
                        bw.Write((String)MySecurity.EncryptString(LVSubItem.Text.Trim()));
                    }
                    bw.Write((String)MySecurity.EncryptString((String)LVItem.Tag));
                    bw.Flush();
                }

                bw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>>Message=" + ex.Message + "\r\nStacktrace:" + ex.StackTrace);
            }
        }
        #endregion
        #region 读取信息
        public static void DataLoad(System.Windows.Forms.ListView LoadingListView, string FilePath)
        {
            if (String.IsNullOrEmpty(FilePath)) return;

            if (!File.Exists(FilePath)) return;

            try
            {
                FileStream fs = new FileStream(FilePath, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);

                while (br.PeekChar() > 0)
                {
                    int columns = LoadingListView.Columns.Count;
                    System.Windows.Forms.ListViewItem LVItem = new System.Windows.Forms.ListViewItem(new string[columns]);

                    foreach (System.Windows.Forms.ListViewItem.ListViewSubItem LVSubItem in LVItem.SubItems)
                    {
                        LVSubItem.Text = MySecurity.DecryptTextFromMemory(br.ReadString());
                    }
                    LVItem.Tag = MySecurity.DecryptTextFromMemory(br.ReadString());
                    LoadingListView.Items.Add(LVItem);
                }

                br.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>>Message=" + ex.Message + "\r\nStacktrace:" + ex.StackTrace);
            }
        }
        #endregion

        public static CustomDataEvtArg RxAntMsg(byte RxAntNum)
        {
            PC_AG_SET_AGT_INFO_REQ SetAgtInfoReq = new PC_AG_SET_AGT_INFO_REQ();
            SetAgtInfoReq.msgHeader.reserved = 0;
            SetAgtInfoReq.msgHeader.SrcID = 4;
            SetAgtInfoReq.msgHeader.destID = 2;
            SetAgtInfoReq.msgHeader.msgType = 0x4000;/* V1.0 */
            SetAgtInfoReq.msgHeader.transactionID = 0;

            SetAgtInfoReq.u8WorkMode = 2;
            SetAgtInfoReq.u8IterfaceType = 1;
            SetAgtInfoReq.u8OutDataType = 2;
            SetAgtInfoReq.u8SaveMode = 2;
            SetAgtInfoReq.RptInterval = 0;

            SetAgtInfoReq.u8RxAntNum = RxAntNum;
            if (RxAntNum == 1)
                SetAgtInfoReq.u8RxAntStatus = 1;
            else if (RxAntNum == 2)
                SetAgtInfoReq.u8RxAntStatus = 3;
            SetAgtInfoReq.mu8GpsValidFlag = 0;
            int DataLength = Marshal.SizeOf(SetAgtInfoReq);

            //消息头中的消息长度
            SetAgtInfoReq.msgHeader.MsgLen = (UInt16)((DataLength - 12) / 4);

            CustomDataEvtArg testOKArgs = new CustomDataEvtArg();
            testOKArgs.data = new byte[DataLength];

            int reqlength = Marshal.SizeOf(SetAgtInfoReq);
            byte[] reqdata = new byte[reqlength];
            IntPtr intptr = Marshal.AllocHGlobal(reqlength);
            Marshal.StructureToPtr(SetAgtInfoReq, intptr, true);
            Marshal.Copy(intptr, reqdata, 0, reqlength);
            Marshal.FreeHGlobal(intptr);
            System.Buffer.BlockCopy(reqdata, 0, testOKArgs.data, 0, reqlength);

            testOKArgs.deivceName = Global.GCurrentDevice;
            Global.ProTracReq = testOKArgs.data;
            return testOKArgs;
        }

        #region 生成ProtocolTracing消息
        const int MAX_SIZE_IMSI = 21;
        const int MAX_SIZE_GUTI = 10;
        const int MAX_SIZE_IMEI = 16;
        //mode:0-测向,2-STMSI,3-cellsearch
        public static CustomDataEvtArg GenerateMessage(string earfcn, string pci, byte mode)
        {
            PC_AG_PROTOCOL_TRACE_REQ ProTracReq = new PC_AG_PROTOCOL_TRACE_REQ();

            //消息头
            ProTracReq.msgHeader.Reserved = 0;
            ProTracReq.msgHeader.Source = 4;
            ProTracReq.msgHeader.Destination = 2;
            ProTracReq.msgHeader.MessageType = 0x400a;/* V1.0 */
            ProTracReq.msgHeader.TransactionID = 0;
            ProTracReq.msgHeader.MsgLen = 0;//需要最后发送的时候计算


            //PROTOCOL_TRACE_UE_INFO_STRU mstUeInfo
            ProTracReq.mstUeInfo = new PROTOCOL_TRACE_UE_INFO_STRU();
            ProTracReq.mstUeInfo.mastUEInfoList = new UE_INFO_STRU[1];
            for (int i = 0; i < ProTracReq.mstUeInfo.mastUEInfoList.Length; i++)
            {
                UE_INFO_STRU ueinfo = new UE_INFO_STRU();
                ueinfo.mu8UEIDTYPE = 0;
                ueinfo.mu8UEIDLength = 8;
                ueinfo.mu8CellInfoFlag = 0;
                ueinfo.mu8UeCategory = 1;
                ueinfo.mstCellInfo = new CELL_INFO_STRU();
                ueinfo.mstCellInfo.u16EARFCN = 0;
                ueinfo.mstCellInfo.u16PCI = 0;
                ueinfo.muUeIdData = new UEID_DATA_STRU();
                ueinfo.muUeIdData.mau8IMSI = new byte[MAX_SIZE_IMSI + 3];

                ProTracReq.mstUeInfo.mastUEInfoList[i] = ueinfo;
            }
            ProTracReq.mstUeInfo.mastUsimInfoList = new USIM_INFO_STRU[1];
            for (int i = 0; i < ProTracReq.mstUeInfo.mastUsimInfoList.Length; i++)
            {
                USIM_INFO_STRU usiminfo = new USIM_INFO_STRU();
                usiminfo.mau8IMSI = new byte[MAX_SIZE_IMSI + 3];
                usiminfo.mau16SimAC = 0;
                usiminfo.mu8KLengthIndex = 1;
                usiminfo.mu8Pading = 0;
                usiminfo.maU8KDATA = new byte[32];

                ProTracReq.mstUeInfo.mastUsimInfoList[i] = usiminfo;
            }

            //cellsearch mode
            ProTracReq.mstUeInfo.mu8UESelectMode = mode;
            ProTracReq.mstUeInfo.mu8TraceUENum = 1;
            ProTracReq.mstUeInfo.mu8UEIDListCount = 1;
            ProTracReq.mstUeInfo.mu8KeyGetMode = 0;
            ProTracReq.mstUeInfo.mu32UeSilenceCheckTimer = CellSearchAndMonitorUnions.UeSilenceCheckTimer;
            ProTracReq.mstCellInfo.mu8RATType = 0;

            //PROTOCOL_TRACE_CELL_INFO_STRU mstCellInfo
            ProTracReq.mstCellInfo = new PROTOCOL_TRACE_CELL_INFO_STRU();
            ProTracReq.mstCellInfo.mu16CellNumber = 1;
            ProTracReq.mstCellInfo.mu32PlmnNum = 1;
            ProTracReq.mstCellInfo.mu8RATType = 0;
            ProTracReq.mstCellInfo.mu16ProtolLayerSelect = 0x0200;
            ProTracReq.mstCellInfo.mu16AverageFrameNum = 1;
            ProTracReq.mstCellInfo.mu16statisticalInfoReportPeriod = 1;
            ProTracReq.mstCellInfo.mu16CtrlInfoReportPeriod = 1;
            ProTracReq.mstCellInfo.mu16FreqBandNum = 1;

            ProTracReq.mstCellInfo.PlmnIdList = new PLMNID_STRU[4];
            for (int i = 0; i < ProTracReq.mstCellInfo.PlmnIdList.Length; i++)
            {
                PLMNID_STRU plmnid = new PLMNID_STRU();
                plmnid.mau8AucMcc = new byte[3] { 4, 6, 0 };
                plmnid.mu8Pading1 = 0;
                plmnid.mau8AucMnc = new byte[2] { 0, 0 };
                plmnid.mau8Pading2 = new byte[2] { 0, 0 };

                ProTracReq.mstCellInfo.PlmnIdList[i] = plmnid;
            }

            CELL_INFO_STRU cellinfo = new CELL_INFO_STRU();
            //EARFCN_INFO_STRU earfcninfo = new EARFCN_INFO_STRU();

            ProTracReq.mstCellInfo.mu8CellSelectMode = 0;
            
            //int freqIndex = 0;
            //switch(band)
            //{
            //    case "38":
            //        freqIndex = 0;
            //        break;
            //    case "39":
            //        freqIndex = 1;
            //        break;
            //    case "40":
            //        freqIndex = 2;
            //        break;
            //    case "41":
            //        freqIndex = 3;
            //        break;
            //    case "1":
            //        freqIndex = 4;
            //        break;
            //    case "3":
            //        freqIndex = 5;
            //        break;
            //    default:
            //        System.Windows.Forms.MessageBox.Show("错误的band！");
            //        break;
            //}
            try
            {
                
                if (earfcn == "-1")
                {
                    earfcn = "0";
                }
                //MessageBox.Show(freq.ToString());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Message=" + ex.Message + "\r\nStacktrace:" + ex.StackTrace);
            }

            ProTracReq.mstCellInfo.mu16CellNumber = 1;
            ProTracReq.mstCellInfo.mu16FreqBandNum = 0;
            cellinfo.u16EARFCN = Convert.ToUInt16(earfcn);
            cellinfo.u16PCI = Convert.ToUInt16(pci);

            switch (mode)
            {
                case 0:
                    ProTracReq.mstCellInfo.mu16ProtolLayerSelect = 515;
                    ProTracReq.mstCellInfo.mu16L1MeasSelect = 8193;
                    break;
                case 2:
                    ProTracReq.mstCellInfo.mu16ProtolLayerSelect = 515;
                    ProTracReq.mstCellInfo.mu16L1MeasSelect = 8193;
                    break;
                case 3:
                    ProTracReq.mstCellInfo.mu16ProtolLayerSelect = 515;
                    ProTracReq.mstCellInfo.mu16L1MeasSelect = 8193;
                    break;
                default:
                    break;
            }
            
            UInt16 mcc = 460;
            UInt16 mnc = 00;
            if(mode==0)
            {
                byte[] CRNTIorRRID = new byte[2];

                ProTracReq.mstUeInfo.mu8UESelectMode = 0;
                int ii = 0;
                int j = 1;
                ProTracReq.mstUeInfo.mastUEInfoList[0].mu8UEIDTYPE = 1; //GUTI
                ////////////////////////////////////////////////////////////////////////////0806
                ProTracReq.mstUeInfo.mastUEInfoList[0].mu8UEIDLength = 10;

                byte[] TypeGUTI = new byte[16];
                byte[] tempKey = System.Text.Encoding.Default.GetBytes(CellSearchAndMonitorUnions.TargetSTMSI);
                for (int i = 0; i < tempKey.Length; i++)
                {
                    if (tempKey[i] >= '0' && tempKey[i] <= '9')
                    {
                        tempKey[i] = (byte)(tempKey[i] - 48);
                    }
                    else if (tempKey[i] >= 'a' && tempKey[i] <= 'f')
                    {
                        tempKey[i] = (byte)(tempKey[i] - 'a' + 10);
                    }
                    else if (tempKey[i] >= 'A' && tempKey[i] <= 'F')
                    {
                        tempKey[i] = (byte)(tempKey[i] - 'A' + 10);
                    }
                }

                for (int num = 5; num < 10; num++)
                {

                    byte tempValue = (byte)(tempKey[ii] << 4);
                    tempValue &= 0xf0;
                    tempValue |= tempKey[j];

                    TypeGUTI[num] = tempValue;
                    ii += 2;
                    j += 2;
                }

                //      TypeGUTI[0] = 0;
                //      TypeGUTI[1] = 0xf1;
                //      TypeGUTI[2] = 0x10;
                //      TypeGUTI[3] = 0;
                //      TypeGUTI[4] = 0;
                
                TypeGUTI[0] = (byte)(mnc / 10);
                TypeGUTI[1] = (byte)(mnc % 10 | 0xf0);
                TypeGUTI[2] = (byte)(mcc % 10);
                TypeGUTI[3] = (byte)(mcc % 100 / 10);
                TypeGUTI[4] = (byte)(mcc / 100);

                int length;
                if (TypeGUTI.Length >= 10)
                {
                    length = 10;
                }
                else
                {
                    length = TypeGUTI.Length;
                }
                System.Buffer.BlockCopy(TypeGUTI,
                                        0,
                                        ProTracReq.mstUeInfo.mastUEInfoList[0].muUeIdData.mau8GUTI,
                                        0,
                                        TypeGUTI.Length);

                ///////////////////////////////////////////////////////////////////////////0806

            }
            byte[] KDATA = new byte[32];
            ProTracReq.mstUeInfo.mastUsimInfoList[0].mu8KLengthIndex = 2;
            ProTracReq.mstUeInfo.mastUsimInfoList[0].maU8KDATA = KDATA;

            int DataLength = Marshal.SizeOf(ProTracReq);
            DataLength = DataLength + Marshal.SizeOf(cellinfo);

            
            ProTracReq.mstCellInfo.PlmnIdList[0].mau8AucMcc[0] = (byte)(mcc / 100);
            ProTracReq.mstCellInfo.PlmnIdList[1].mau8AucMcc[0] = (byte)(mcc / 100);
            ProTracReq.mstCellInfo.PlmnIdList[2].mau8AucMcc[0] = (byte)(mcc / 100);
            ProTracReq.mstCellInfo.PlmnIdList[3].mau8AucMcc[0] = (byte)(mcc / 100);

            ProTracReq.mstCellInfo.PlmnIdList[0].mau8AucMcc[1] = (byte)(mcc % 100 / 10);
            ProTracReq.mstCellInfo.PlmnIdList[1].mau8AucMcc[1] = (byte)(mcc % 100 / 10);
            ProTracReq.mstCellInfo.PlmnIdList[2].mau8AucMcc[1] = (byte)(mcc % 100 / 10);
            ProTracReq.mstCellInfo.PlmnIdList[3].mau8AucMcc[1] = (byte)(mcc % 100 / 10);

            ProTracReq.mstCellInfo.PlmnIdList[0].mau8AucMcc[2] = (byte)(mcc % 10);
            ProTracReq.mstCellInfo.PlmnIdList[1].mau8AucMcc[2] = (byte)(mcc % 10);
            ProTracReq.mstCellInfo.PlmnIdList[2].mau8AucMcc[2] = (byte)(mcc % 10);
            ProTracReq.mstCellInfo.PlmnIdList[3].mau8AucMcc[2] = (byte)(mcc % 10);

            ProTracReq.mstCellInfo.PlmnIdList[0].mau8AucMnc[0] = (byte)(mnc / 10);
            ProTracReq.mstCellInfo.PlmnIdList[1].mau8AucMnc[0] = (byte)(mnc / 10);
            ProTracReq.mstCellInfo.PlmnIdList[2].mau8AucMnc[0] = (byte)(mnc / 10);
            ProTracReq.mstCellInfo.PlmnIdList[3].mau8AucMnc[0] = (byte)(mnc / 10);

            ProTracReq.mstCellInfo.PlmnIdList[0].mau8AucMnc[1] = (byte)(mnc % 10);
            ProTracReq.mstCellInfo.PlmnIdList[1].mau8AucMnc[1] = (byte)(mnc % 10);
            ProTracReq.mstCellInfo.PlmnIdList[2].mau8AucMnc[1] = (byte)(mnc % 10);
            ProTracReq.mstCellInfo.PlmnIdList[3].mau8AucMnc[1] = (byte)(mnc % 10);

            //消息头中的消息长度
            ProTracReq.msgHeader.MsgLen = (UInt16)((DataLength - 12) / 4);

            CustomDataEvtArg testOKArgs = new CustomDataEvtArg();
            testOKArgs.data = new byte[DataLength];

            int reqlength = Marshal.SizeOf(ProTracReq);
            byte[] reqdata = new byte[reqlength];
            IntPtr intptr = Marshal.AllocHGlobal(reqlength);
            Marshal.StructureToPtr(ProTracReq, intptr, true);
            Marshal.Copy(intptr, reqdata, 0, reqlength);
            Marshal.FreeHGlobal(intptr);
            System.Buffer.BlockCopy(reqdata, 0, testOKArgs.data, 0, reqlength);

            int cellinfolength = Marshal.SizeOf(cellinfo);
            byte[] cellinfodata1 = new byte[cellinfolength];
            IntPtr ptr = Marshal.AllocHGlobal(cellinfolength);
            Marshal.StructureToPtr(cellinfo, ptr, true);
            Marshal.Copy(ptr, cellinfodata1, 0, cellinfolength);
            Marshal.FreeHGlobal(ptr);
            System.Buffer.BlockCopy(cellinfodata1, 0, testOKArgs.data, reqlength, cellinfolength);

            //testOKArgs.deivceName = Global.GCurrentDevice;;
            testOKArgs.moduleName = "ProtocolTrace";
            testOKArgs.readyModule = "ProtocolTrace";
            //Global.tempClass.Ready(sender, testOKArgs);
            Global.ProTracReq = testOKArgs.data;
            Global.ReadyModule = "ProtocolTrace";
            //Global.tempClass.Ready(sender, testOKArgs);

            return testOKArgs;
        }
        #endregion
    }
}
