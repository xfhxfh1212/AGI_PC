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
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using COM.ZCTT.AGI.Common;
using U8 = System.Byte;
using S8 = System.SByte;
using U16 = System.UInt16;
using S16 = System.Int16;
using U32 = System.UInt32;
using S32 = System.Int32;
using F32 = System.Single;
using B8 = System.Byte;
using System.IO;

namespace CellSearchAndMonitor
{
    class Decode
    {
        ////RRC解码函数
        //[DllImport("HKASN1.dll", EntryPoint = "DecodeDataStreamWithPosInfo", CallingConvention = CallingConvention.Cdecl)]
        //static extern uint DecodeDataStreamWithPosInfo(ushort msgId,
        //                                    byte[] msgData,
        //                                    byte[] decDataAdr,
        //                                    int dataSize,
        //                                    byte[] msgName);

        //[DllImport("HKASN1.dll", EntryPoint = "DecodeDataStream", CallingConvention = CallingConvention.Cdecl)]
        //static extern uint DecodeDataStream(ushort msgId,
        //                                    byte[] msgData,
        //                                    byte[] decDataAdr,
        //                                    int dataSize,
        //                                    byte[] msgName);
        ////NAS解码函数
        //[DllImport("HKNASDecoder.dll", EntryPoint = "dissect_nas_msg", CallingConvention = CallingConvention.Cdecl)]
        //static extern long dissect_nas_msg(byte[] pOut,         //解码后的消息地址，用于输出显示
        //                                          int ulNasMsgSize, //待解码消息的长度
        //                                          byte[] pMsg,         //待解码消息的首地址
        //                                          byte[] chMsgType);   //返回消息名字，是字符串，可以用于显示，也可以不使用

        //[DllImport("protocal.dll", EntryPoint = "macCeAnalyze", CallingConvention = CallingConvention.Cdecl)]
        //static extern void macCeAnalyze(U8 macCeType, U8[] pData, IntPtr intptr);

        //[DllImport("protocal.dll", EntryPoint = "macPduSubHeaderAnalyze", CallingConvention = CallingConvention.Cdecl)]
        //static extern void macPduSubHeaderAnalyze(U8[] pHeader, IntPtr intptr);
        ////static extern void macPduSubHeaderAnalyze(U8[] pHeader,ref L2P_MAC_PDU_SUBHEADER_STRU stru);

        //[DllImport("protocal.dll", EntryPoint = "macRarSubHeaderAnalyze", CallingConvention = CallingConvention.Cdecl)]
        //static extern void macRarSubHeaderAnalyze(U8[] pHeader, IntPtr intptr);

        ///* UM模式调用 */
        //[DllImport("protocal.dll", EntryPoint = "rlcUmdHeaderAnalyze", CallingConvention = CallingConvention.Cdecl)]
        //static extern void rlcUmdHeaderAnalyze(U8 u8snLen, U8[] pData, IntPtr intptr); /* 取值5或10 */

        ///* AM模式调用 */
        //[DllImport("protocal.dll", EntryPoint = "rlcAmdHeaderAnalyze", CallingConvention = CallingConvention.Cdecl)]
        //static extern void rlcAmdHeaderAnalyze(U32 u32pduByteNum, U8[] pData, IntPtr intptr);/*pdu长度，单位byte */

        ///* SRB data pdu 调用*/
        //[DllImport("protocal.dll", EntryPoint = "pdcpDataPduForSrbAnalyze", CallingConvention = CallingConvention.Cdecl)]
        //static extern void pdcpDataPduForSrbAnalyze(U8[] pData, ref U8 pPdcpsn);

        ///* DRB pdu调用 */
        //[DllImport("protocal.dll", EntryPoint = "pdcpPduForDrbAnalyze", CallingConvention = CallingConvention.Cdecl)]
        //static extern void pdcpPduForDrbAnalyze(U8[] pData,
        //                                        U8 U8SnBitLen, /* 取值7或12 */
        //                                        U32 u16PduLen,  /*pdu长度，单位byte */
        //                                        ref L2P_PDCP_PDU_DRB_STRU stru);

        //[DllImport("protocal.dll", EntryPoint = "macRarAnalyze", CallingConvention = CallingConvention.Cdecl)]
        //static extern void macRarAnalyze(U8 CeLength, U8[] pData, IntPtr intptr);

        public static byte[] decodeData = new byte[] { 0 };
        public static byte Level = 0;
        private static Dictionary<string, string> result = new Dictionary<string, string>();
        /// <summary>
        /// 窗体接收到MIB消息时执行的方法
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">自定义的事件参数，其中包含了MIB信息</param>
        public static Dictionary<string, string> dataBackControl_SendMIBToDisplayEvent(CustomDataEvtArg e)
        {
            result.Clear();
            try
            {
                #region decode
                decodeData = new byte[e.data.Length];
                

                decodeData = e.data;
                Level = 0;
                
                
                UInt16 messageType = BitConverter.ToUInt16(e.data, 6);
                switch (messageType)
                {
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_CELL_STATE_IND_MSG_TYPE:
                        {

                            XX_L2P_AG_CELL_STATE_IND XXCellStateData = new XX_L2P_AG_CELL_STATE_IND();
                            //得到结构体大小
                            int isize = Marshal.SizeOf(XXCellStateData);
                            //比较结构体大小
                            if (isize > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr Intptr = Marshal.AllocHGlobal(isize);
                            //将byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, Intptr, isize);
                            //将内存空间转换为目标结构体
                            //Marshal.PtrToStructure(intptr, ProtocolData);
                            XXCellStateData = (XX_L2P_AG_CELL_STATE_IND)Marshal.PtrToStructure(Intptr, typeof(XX_L2P_AG_CELL_STATE_IND));
                            //释放内存空间
                            Marshal.FreeHGlobal(Intptr);
                            string str = null;
                            str += "+- CellStatus:".PadRight(30, ' ');
                            str += (CELL_STATECellStatus)XXCellStateData.mstCellStateHeader.mu16CellStatus + "\r\n";
                            str += "+- PCI:".PadRight(30, ' ');
                            str += XXCellStateData.mstCellStateHeader.mu16PCI.ToString() + "\r\n";
                            str += "+- EARFCN:".PadRight(30, ' ');
                            str += XXCellStateData.mstCellStateHeader.mu16EARFCN.ToString() + "\r\n";
                            str += "+- Padding:".PadRight(30, ' ');
                            str += XXCellStateData.mstCellStateHeader.mu16Pading.ToString() + "\r\n";
                            
                            return null;
                        }
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_UE_RELEASE_IND_MSG_TYPE:
                        {
                            XX_L2P_AG_UE_RELEASE_IND XXUeRealseData = new XX_L2P_AG_UE_RELEASE_IND();
                            int isize = Marshal.SizeOf(XXUeRealseData);
                            int offset = 0;
                            //比较结构体大小
                            if (isize > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr Intptr = Marshal.AllocHGlobal(isize);
                            //将byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, Intptr, isize);
                            offset += isize;
                            //将内存空间转换为目标结构体
                            XXUeRealseData = (XX_L2P_AG_UE_RELEASE_IND)Marshal.PtrToStructure(Intptr, typeof(XX_L2P_AG_UE_RELEASE_IND));
                            //释放内存空间
                            Marshal.FreeHGlobal(Intptr);
                            string str = null;

                            str += "+- Release UE number:".PadRight(30, ' ');
                            str += XXUeRealseData.mstUEReleaseHeader.mu8UeNUM + "\r\n";
                            // AG_UE_CAPTURE_INFO_STRU
                            for (int ueCount = 0; ueCount < XXUeRealseData.mstUEReleaseHeader.mu8UeNUM; ueCount++)
                            {
                                AG_UE_CAPTURE_INFO_STRU stru_AG_UE_CAPTURE_INFO_STRU = new AG_UE_CAPTURE_INFO_STRU();
                                int size_AG_UE_CAPTURE_INFO_STRU = Marshal.SizeOf(stru_AG_UE_CAPTURE_INFO_STRU);
                                if (size_AG_UE_CAPTURE_INFO_STRU > e.data.Length - isize)
                                {
                                    return null;
                                }
                                IntPtr intptr_AG_UE_CAPTURE_INFO_STRU = Marshal.AllocHGlobal(size_AG_UE_CAPTURE_INFO_STRU);
                                Marshal.Copy(e.data, offset, intptr_AG_UE_CAPTURE_INFO_STRU, size_AG_UE_CAPTURE_INFO_STRU);
                                offset += size_AG_UE_CAPTURE_INFO_STRU;
                                stru_AG_UE_CAPTURE_INFO_STRU = (AG_UE_CAPTURE_INFO_STRU)Marshal.PtrToStructure(intptr_AG_UE_CAPTURE_INFO_STRU, typeof(AG_UE_CAPTURE_INFO_STRU));
                                Marshal.FreeHGlobal(intptr_AG_UE_CAPTURE_INFO_STRU);

                                #region 显示
                                str += "+- UE [" + ueCount + "]:\r\n";
                                str += "   +- EARFCN:".PadRight(30, ' ');
                                str += stru_AG_UE_CAPTURE_INFO_STRU.mu16EARFCN.ToString() + "\r\n";

                                str += "   +- PCI:".PadRight(30, ' ');
                                str += stru_AG_UE_CAPTURE_INFO_STRU.mu16PCI.ToString() + "\r\n";

                                str += "   +- UEIDType:".PadRight(30, ' ');
                                string UEIDType = "";
                                string UEIDData = "";
                                for (int i = 1; i <= 32; i = i * 2)
                                {
                                    if ((stru_AG_UE_CAPTURE_INFO_STRU.mu8UEIDTypeFlg & i) == i)
                                    {
                                        switch (i)
                                        {
                                            case 1:
                                                UEIDType += "IMSI, ";

                                                UEIDData += "   +- ImsiDigitCnt:".PadRight(30, ' ');
                                                UEIDData += stru_AG_UE_CAPTURE_INFO_STRU.mu8ImsiDigitCnt.ToString() + "\r\n";

                                                UEIDData += "   +- IMSI:".PadRight(30, ' ');
                                                UEIDData += BitConverter.ToString(stru_AG_UE_CAPTURE_INFO_STRU.mau8IMSI) + "\r\n";
                                                break;
                                            case 2:
                                                UEIDType += "GUTI, ";
                                                UEIDData += "   +- GUTIDATA\r\n";
                                                UEIDData += "     | +- mmec:".PadRight(30, ' ');
                                                B8[] mmmec = new B8[] { 0 };
                                                mmmec[0] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[5];
                                                UEIDData += BitConverter.ToString(mmmec) + "\r\n";
                                                UEIDData += "     | +- m-TMSI:".PadRight(30, ' ');
                                                B8[] mTMSI = new B8[] { 0, 0, 0, 0 };
                                                mTMSI[3] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[6];
                                                mTMSI[2] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[7];
                                                mTMSI[1] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[8];
                                                mTMSI[0] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[9];
                                                UEIDData += BitConverter.ToString(mTMSI) + "\r\n";
                                                break;
                                            case 4:
                                                UEIDType += "IMEI, ";

                                                UEIDData += "   +- IMEI:".PadRight(30, ' ');
                                                UEIDData += BitConverter.ToString(stru_AG_UE_CAPTURE_INFO_STRU.mau8IMEI) + "\r\n";
                                                break;
                                            case 32:
                                                UEIDType += "S-TMSI, ";
                                                UEIDData += "   +- S-TMSIDATA\r\n";
                                                UEIDData += "     | +- mmec:".PadRight(30, ' ');
                                                B8[] smmec = new B8[] { 0 };
                                                smmec[0] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[5];
                                                UEIDData += BitConverter.ToString(smmec) + "\r\n";
                                                UEIDData += "     | +- m-TMSI:".PadRight(30, ' ');
                                                B8[] sTMSI = new B8[] { 0, 0, 0, 0 };
                                                sTMSI[3] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[6];
                                                sTMSI[2] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[7];
                                                sTMSI[1] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[8];
                                                sTMSI[0] = stru_AG_UE_CAPTURE_INFO_STRU.mau8GUTIDATA[9];
                                                UEIDData += BitConverter.ToString(sTMSI) + "\r\n";
                                                break;
                                            case 8:
                                                UEIDType += "CRNTI, ";

                                                UEIDData += "   +- CRNTIDATA:".PadRight(30, ' ');
                                                UEIDData += stru_AG_UE_CAPTURE_INFO_STRU.mu16CRNTIDATA.ToString() + "(Ox" + stru_AG_UE_CAPTURE_INFO_STRU.mu16CRNTIDATA.ToString("X") + ")" + "\r\n";
                                                break;
                                            case 16:
                                                UEIDType += "PRID, ";

                                                UEIDData += "   +- PRIDDATA:".PadRight(30, ' ');
                                                UEIDData += stru_AG_UE_CAPTURE_INFO_STRU.mu8PRIDDATA.ToString() + "\r\n";
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }

                                if (UEIDType.Length > 2)
                                {
                                    UEIDType = UEIDType.Remove(UEIDType.Length - 2);
                                }

                                str += UEIDType + "\r\n";
                                str += UEIDData + "\r\n";
                                #endregion
                            }

                            return null;
                        }
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_CELL_RELEASE_IND_MSG_TYPE:
                        {
                            XX_L2P_AG_CELL_RELEASE_IND XXCellRealseData = new XX_L2P_AG_CELL_RELEASE_IND();
                            int isize = Marshal.SizeOf(XXCellRealseData);
                            //比较结构体大小
                            if (isize > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr Intptr = Marshal.AllocHGlobal(isize);
                            //将byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, Intptr, isize);
                            //将内存空间转换为目标结构体
                            XXCellRealseData = (XX_L2P_AG_CELL_RELEASE_IND)Marshal.PtrToStructure(Intptr, typeof(XX_L2P_AG_CELL_RELEASE_IND));
                            //释放内存空间
                            Marshal.FreeHGlobal(Intptr);
                            string str = null;

                            str += "Release Cell number:".PadRight(30, ' ');
                            str += XXCellRealseData.mstCellReleaseHeader.mu8CellNUM + "\r\n";


                            return null;
                        }
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_CELL_CAPTURE_IND_MSG_TYPE:
                        {
                            XX_L2P_AG_CELL_CAPTURE_IND XXCellCaptureData = new XX_L2P_AG_CELL_CAPTURE_IND();
                            //得到结构体大小
                            int isize = Marshal.SizeOf(XXCellCaptureData);
                            //比较结构体大小
                            if (isize > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr Intptr = Marshal.AllocHGlobal(isize);
                            //将byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, Intptr, isize);
                            //将内存空间转换为目标结构体
                            //Marshal.PtrToStructure(intptr, ProtocolData);
                            XXCellCaptureData = (XX_L2P_AG_CELL_CAPTURE_IND)Marshal.PtrToStructure(Intptr, typeof(XX_L2P_AG_CELL_CAPTURE_IND));
                            //释放内存空间
                            Marshal.FreeHGlobal(Intptr);
                            string str = null;

                            str += "+- CellStatus:".PadRight(30, ' ');
                            str += (CELL_CAPTURECellStatus)XXCellCaptureData.mstCellCaptureHeader.mu16CellStatus + "\r\n";
                            str += "+- PCI:".PadRight(30, ' ');
                            str += XXCellCaptureData.mstCellCaptureHeader.mu16PCI.ToString() + "\r\n";
                            result.Add("PCI", XXCellCaptureData.mstCellCaptureHeader.mu16PCI.ToString());
                            str += "+- EARFCN:".PadRight(30, ' ');
                            str += XXCellCaptureData.mstCellCaptureHeader.mu16EARFCN.ToString() + "\r\n";
                            result.Add("EARFCN",XXCellCaptureData.mstCellCaptureHeader.mu16EARFCN.ToString());
                            str += "+- TAC:".PadRight(30, ' ');
                            str += "0x" + XXCellCaptureData.mstCellCaptureHeader.mu16TAC.ToString("X") + "\r\n";
                            result.Add("TAC", "0x" + XXCellCaptureData.mstCellCaptureHeader.mu16TAC.ToString("X"));
                            str += "+- CellID:".PadRight(30, ' ');
                            str += "0x" + XXCellCaptureData.mstCellCaptureHeader.mu32CellID.ToString("X") + "\r\n";
                            result.Add("CellID", "0x" + XXCellCaptureData.mstCellCaptureHeader.mu32CellID.ToString("X"));
                            str += "+- RSRP:".PadRight(30, ' ');
                            str += XXCellCaptureData.mstCellCaptureHeader.mu16Rsrp.ToString() + "\r\n";
                            if (XXCellCaptureData.mstCellCaptureHeader.mu16Rsrp == 0)
                                result.Add("RSRP", "NULL");
                            else
                                result.Add("RSRP", XXCellCaptureData.mstCellCaptureHeader.mu16Rsrp.ToString());
                            str += "+- RSRQ:".PadRight(30, ' ');
                            str += (XXCellCaptureData.mstCellCaptureHeader.mu16Rsrq / 10).ToString() + "\r\n";
                            if (XXCellCaptureData.mstCellCaptureHeader.mu16Rsrq == 0)
                                result.Add("RSRQ", "NULL");
                            else
                                result.Add("RSRQ", (XXCellCaptureData.mstCellCaptureHeader.mu16Rsrq/10).ToString());
                            str += "+- DownlinkBandwidth:".PadRight(30, ' ');
                            str += (Dlbandwidth)XXCellCaptureData.mstCellCaptureHeader.mu8Dlbandwidth + "\r\n";
                            if (XXCellCaptureData.mstCellCaptureHeader.mu8Dlbandwidth == 0)
                                result.Add("DLBand", "NULL");
                            else
                                result.Add("DLBand", XXCellCaptureData.mstCellCaptureHeader.mu8Dlbandwidth.ToString());

                            str += "+- PhichDuration:".PadRight(30, ' ');
                            str += (PhichDuration)XXCellCaptureData.mstCellCaptureHeader.mu8PhichDuration + "\r\n";
                            str += "+- PhichResource:".PadRight(30, ' ');
                            str += (PhichResource)XXCellCaptureData.mstCellCaptureHeader.mu8PhichResource + "\r\n";
                            str += "+- SpecialSubframePatterns:".PadRight(30, ' ');
                            str += (SpecialSubframePatterns)XXCellCaptureData.mstCellCaptureHeader.mu8SpecialSubframePatterns + "\r\n";
                            str += "+- UplinkDownlinkConfiguration:".PadRight(30, ' ');
                            str += (UplinkDownlinkConfiguration)XXCellCaptureData.mstCellCaptureHeader.mu8UplinkDownlinkConfiguration + "\r\n";

                            Console.WriteLine(str);
                            return result;
                        }
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_SPECIFIED_CELL_SCAN_DATA_MSG_TYPE:
                        {
                            #region
                            //2015.01.08 modifi by zhouyt
                            //===========================================================
                            U32 mu32SibPresentFlg = BitConverter.ToUInt32(e.data, 28);
                            //===========================================================

                            byte[] sbMsgData = new byte[30000];
                            byte[] sbMsgName = new byte[2000];
                            byte[] data;

                            //该变量表示MIB或SIB数据区的起始位置。
                            //2014.5.29 modifi by yangchun
                            //=============================================
                            int dataFlag = 32;
                            //=============================================
                            //该变量表示SIB数据区的长度，MIB为3。
                            ushort dataLength;
                            //MIB时为46，SIB时为47
                            ushort msgID;

                            for (int i = 0; i <= 14; i++)
                            {
                                sbMsgData = new byte[30000];
                                sbMsgName = new byte[2000];

                                if ((mu32SibPresentFlg & (int)Math.Pow(2, i)) == (int)Math.Pow(2, i))
                                {
                                    //MIB
                                    if (i == 0)
                                    {
                                        msgID = 46;
                                        dataLength = 3;
                                        data = new byte[dataLength];
                                        dataFlag = 32;
                                        System.Buffer.BlockCopy(e.data, dataFlag, data, 0, 3);
                                        //UInt32 xx = DecodeDataStream(msgID, sbMsgData, data, data.Length, sbMsgName);
                                        //-----------------------------------------------------------------
                                        //decodeDataView = new byte[data.Length];
                                        //System.Buffer.BlockCopy(data, 0, decodeDataView, 0, data.Length);
                                        ////-------------------------------------------------------------------

                                        ////decodeView.richTextBox1.Text += "MIB:" + "\r\n";
                                        //decodeView.richTextBox1.Text += System.Text.Encoding.ASCII.GetString(sbMsgData);

                                        dataFlag += 4;
                                    }
                                    //SIB
                                    else if (i >= 1 && i <= 13)
                                    {
                                        try
                                        {
                                            msgID = 47;
                                            if (dataFlag > e.data.Length)
                                            {
                                                continue;
                                            }
                                            dataLength = BitConverter.ToUInt16(e.data, dataFlag);
                                            dataFlag += 4;

                                            data = new byte[dataLength];
                                            System.Buffer.BlockCopy(e.data, dataFlag, data, 0, dataLength);
                                            //UInt32 xx = DecodeDataStreamWithPosInfo(msgID, sbMsgData, data, data.Length, sbMsgName);
                                            //UInt32 xx = DecodeDataStream(msgID, sbMsgData, data, data.Length, sbMsgName);
                                            //-----------------------------------------------------------------
                                            //decodeDataView = new byte[data.Length];
                                            //System.Buffer.BlockCopy(data, 0, decodeDataView, 0, data.Length);
                                            ////-------------------------------------------------------------------

                                            ////decodeView.richTextBox1.Text += "SIB" + i.ToString() + ":" + "\r\n";
                                            //decodeView.richTextBox1.Text += System.Text.Encoding.ASCII.GetString(sbMsgData);

                                            dataFlag += (((dataLength + 3) / 4) * 4);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(">>>Message= " + ex.Message + "\r\n StrackTrace: " + ex.StackTrace);
                                        }
                                    }
                                    //paging
                                    else if (i == 14)
                                    {
                                        //decodeView.richTextBox1.Text += "Paging\r\n";
                                    }
                                }
                            }

                            //decodeText = decodeView.richTextBox1.Text;
                            break;
                            #endregion
                        }
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_CELL_SYSINFO_IND_MSG_TYPE:
                        {
                            //2014.5.29 modifi by yangchun
                            //===========================================================
                            U32 mu32SibPresentFlg = BitConverter.ToUInt32(e.data, 28);
                            //===========================================================

                            byte[] sbMsgData = new byte[30000];
                            byte[] sbMsgName = new byte[2000];
                            byte[] data;

                            //该变量表示MIB或SIB数据区的起始位置。
                            //2014.5.29 modifi by yangchun
                            //=============================================
                            int dataFlag = 32;
                            //=============================================
                            //该变量表示SIB数据区的长度，MIB为3。
                            ushort dataLength;
                            //MIB时为46，SIB时为47
                            ushort msgID;

                            for (int i = 0; i <= 14; i++)
                            {
                                sbMsgData = new byte[30000];
                                sbMsgName = new byte[2000];

                                if ((mu32SibPresentFlg & (int)Math.Pow(2, i)) == (int)Math.Pow(2, i))
                                {
                                    //MIB
                                    if (i == 0)
                                    {
                                        msgID = 46;
                                        dataLength = 3;
                                        data = new byte[dataLength];
                                        dataFlag = 32;
                                        System.Buffer.BlockCopy(e.data, dataFlag, data, 0, 3);
                                        //UInt32 xx = DecodeDataStreamWithPosInfo(msgID, sbMsgData, data, data.Length, sbMsgName);
                                        //UInt32 xx = DecodeDataStream(msgID, sbMsgData, data, data.Length, sbMsgName);
                                        //-----------------------------------------------------------------
                                        //decodeDataView = new byte[data.Length];
                                        //System.Buffer.BlockCopy(data, 0, decodeDataView, 0, data.Length);
                                        //-------------------------------------------------------------------

                                        //decodeView.richTextBox1.Text += "MIB:" + "\r\n";
                                        //decodeView.richTextBox1.Text += System.Text.Encoding.ASCII.GetString(sbMsgData);

                                        dataFlag += 4;
                                    }
                                    //SIB
                                    else if (i >= 1 && i <= 13)
                                    {
                                        try
                                        {
                                            msgID = 47;
                                            if (dataFlag > e.data.Length)
                                            {
                                                continue;
                                            }
                                            dataLength = BitConverter.ToUInt16(e.data, dataFlag);
                                            dataFlag += 4;

                                            data = new byte[dataLength];
                                            System.Buffer.BlockCopy(e.data, dataFlag, data, 0, dataLength);
                                            //UInt32 xx = DecodeDataStreamWithPosInfo(msgID, sbMsgData, data, data.Length, sbMsgName);
                                            //UInt32 xx = DecodeDataStream(msgID, sbMsgData, data, data.Length, sbMsgName);
                                            //-----------------------------------------------------------------
                                            //decodeDataView = new byte[data.Length];
                                            //System.Buffer.BlockCopy(data, 0, decodeDataView, 0, data.Length);
                                            //-------------------------------------------------------------------

                                            //decodeView.richTextBox1.Text += "SIB" + i.ToString() + ":" + "\r\n";
                                            //decodeView.richTextBox1.Text += System.Text.Encoding.ASCII.GetString(sbMsgData);

                                            dataFlag += (((dataLength + 3) / 4) * 4);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(">>>Message= " + ex.Message + "\r\n StrackTrace: " + ex.StackTrace);
                                        }
                                    }
                                    //paging
                                    else if (i == 14)
                                    {
                                        //decodeView.richTextBox1.Text += "Paging\r\n";
                                    }
                                }
                            }

                            //decodeText = decodeView.richTextBox1.Text;
                            return null;
                        }
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_UE_SILENCE_RPT_IND_MSG_TYPE:
                        {
                            L2P_AG_UE_SILENCE_RPT_IND L2pAgUeSilenceInd = new L2P_AG_UE_SILENCE_RPT_IND();
                            //得到结构体大小
                            int isize = Marshal.SizeOf(L2pAgUeSilenceInd);
                            //比较结构体大小
                            if (isize > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr Intptr = Marshal.AllocHGlobal(isize);
                            //将byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, Intptr, isize);
                            //将内存空间转换为目标结构体
                            //Marshal.PtrToStructure(intptr, ProtocolData);
                            L2pAgUeSilenceInd = (L2P_AG_UE_SILENCE_RPT_IND)Marshal.PtrToStructure(Intptr, typeof(L2P_AG_UE_SILENCE_RPT_IND));
                            //释放内存空间
                            Marshal.FreeHGlobal(Intptr);
                            string str = null;

                            str += "+- EARFCN:".PadRight(30, ' ');
                            str += L2pAgUeSilenceInd.mstUESilenceInfo.mu16EARFCN.ToString() + "\r\n";

                            str += "+- PCI:".PadRight(30, ' ');
                            str += L2pAgUeSilenceInd.mstUESilenceInfo.mu16PCI.ToString() + "\r\n";

                            //edit at 2014.11.28
                            if (L2pAgUeSilenceInd.mstUESilenceInfo.mu8Pading1 == 1)//表示为原SILENCE time超时上报
                            {

                            }

                            ///////////////////////////////////////////////////
                            str += "+- UEIDType:".PadRight(30, ' ');
                            string UEIDType = "";
                            string UEIDData = "";
                            for (int i = 1; i <= 32; i = i * 2)
                            {
                                if ((L2pAgUeSilenceInd.mstUESilenceInfo.mu8UEIDTypeFlg & i) == i)
                                {
                                    switch (i)
                                    {
                                        case 1:
                                            UEIDType += "IMSI, ";

                                            UEIDData += "+- ImsiDigitCnt:".PadRight(30, ' ');
                                            UEIDData += L2pAgUeSilenceInd.mstUESilenceInfo.mu8ImsiDigitCnt.ToString() + "\r\n";

                                            UEIDData += "+- IMSI:".PadRight(30, ' ');
                                            UEIDData += BitConverter.ToString(L2pAgUeSilenceInd.mstUESilenceInfo.mau8IMSI) + "\r\n";
                                            break;
                                        case 2:
                                            UEIDType += "GUTI, ";
                                            UEIDData += "+- GUTIDATA\r\n";
                                            UEIDData += "   | +- mmec:".PadRight(30, ' ');
                                            B8[] mmmec = new B8[] { 0 };
                                            mmmec[0] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[5];
                                            UEIDData += BitConverter.ToString(mmmec) + "\r\n";
                                            UEIDData += "   | +- m-TMSI:".PadRight(30, ' ');
                                            B8[] mTMSI = new B8[] { 0, 0, 0, 0 };
                                            mTMSI[3] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[6];
                                            mTMSI[2] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[7];
                                            mTMSI[1] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[8];
                                            mTMSI[0] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[9];
                                            UEIDData += BitConverter.ToString(mTMSI) + "\r\n";
                                            break;
                                        case 4:
                                            UEIDType += "IMEI, ";

                                            UEIDData += "+- IMEI:".PadRight(30, ' ');
                                            UEIDData += BitConverter.ToString(L2pAgUeSilenceInd.mstUESilenceInfo.mau8IMEI) + "\r\n";
                                            break;
                                        case 32:
                                            UEIDType += "S-TMSI, ";
                                            UEIDData += "+- S-TMSIDATA\r\n";
                                            UEIDData += "   | +- mmec:".PadRight(30, ' ');
                                            B8[] smmec = new B8[] { 0 };
                                            smmec[0] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[5];
                                            UEIDData += BitConverter.ToString(smmec) + "\r\n";
                                            UEIDData += "   | +- m-TMSI:".PadRight(30, ' ');
                                            B8[] sTMSI = new B8[] { 0, 0, 0, 0 };
                                            sTMSI[3] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[6];
                                            sTMSI[2] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[7];
                                            sTMSI[1] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[8];
                                            sTMSI[0] = L2pAgUeSilenceInd.mstUESilenceInfo.mau8GUTIDATA[9];
                                            UEIDData += BitConverter.ToString(sTMSI) + "\r\n";
                                            break;
                                        case 8:
                                            UEIDType += "CRNTI, ";

                                            UEIDData += "+- CRNTIDATA:".PadRight(30, ' ');
                                            UEIDData += L2pAgUeSilenceInd.mstUESilenceInfo.mu16CRNTIDATA.ToString() + "(Ox" + L2pAgUeSilenceInd.mstUESilenceInfo.mu16CRNTIDATA.ToString("X") + ")" + "\r\n";
                                            break;
                                        case 16:
                                            UEIDType += "PRID, ";

                                            UEIDData += "+- PRIDDATA:".PadRight(30, ' ');
                                            UEIDData += L2pAgUeSilenceInd.mstUESilenceInfo.mu8PRIDDATA.ToString() + "\r\n";
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            if (UEIDType.Length > 2)
                            {
                                UEIDType = UEIDType.Remove(UEIDType.Length - 2);
                            }

                            str += UEIDType + "\r\n";
                            str += UEIDData + "\r\n";

                            //decodeView.richTextBox1.Text += str;
                            //decodeText = decodeView.richTextBox1.Text;
                            return null;
                        } 
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_UE_CAPTURE_IND_MSG_TYPE:
                        {
                            L2P_AG_UE_CAPTURE_IND L2pAgUeCaptureInd = new L2P_AG_UE_CAPTURE_IND();
                            //得到结构体大小
                            int isize = Marshal.SizeOf(L2pAgUeCaptureInd);
                            //比较结构体大小
                            if (isize > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr Intptr = Marshal.AllocHGlobal(isize);
                            //将byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, Intptr, isize);
                            //decodeDataView = new byte[e.data.Length];
                            //System.Buffer.BlockCopy(e.data, 0, decodeDataView, 0, e.data.Length);
                            //将内存空间转换为目标结构体
                            //Marshal.PtrToStructure(intptr, ProtocolData);
                            L2pAgUeCaptureInd = (L2P_AG_UE_CAPTURE_IND)Marshal.PtrToStructure(Intptr, typeof(L2P_AG_UE_CAPTURE_IND));
                            //释放内存空间
                            Marshal.FreeHGlobal(Intptr);
                            string str = null;

                            str += "+- EARFCN:".PadRight(30, ' ');
                            str += L2pAgUeCaptureInd.mstUECaptureInfo.mu16EARFCN.ToString() + "\r\n";

                            str += "+- PCI:".PadRight(30, ' ');
                            str += L2pAgUeCaptureInd.mstUECaptureInfo.mu16PCI.ToString() + "\r\n";

                            str += "+- UEIDType:".PadRight(30, ' ');
                            string UEIDType = "";
                            string UEIDData = "";
                            for (int i = 1; i <= 32; i = i * 2)
                            {
                                if ((L2pAgUeCaptureInd.mstUECaptureInfo.mu8UEIDTypeFlg & i) == i)
                                {
                                    switch (i)
                                    {
                                        case 1:
                                            UEIDType += "IMSI, ";

                                            UEIDData += "+- ImsiDigitCnt:".PadRight(30, ' ');
                                            UEIDData += L2pAgUeCaptureInd.mstUECaptureInfo.mu8ImsiDigitCnt.ToString() + "\r\n";

                                            UEIDData += "+- IMSI:".PadRight(30, ' ');
                                            UEIDData += BitConverter.ToString(L2pAgUeCaptureInd.mstUECaptureInfo.mau8IMSI) + "\r\n";
                                            break;
                                        case 2:
                                            UEIDType += "GUTI, ";

                                            UEIDData += "+- GUTIDATA\r\n";
                                            UEIDData += "   | +- mmec:".PadRight(30, ' ');
                                            B8[] mmmec = new B8[] { 0 };
                                            mmmec[0] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[5];
                                            UEIDData += BitConverter.ToString(mmmec) + "\r\n";
                                            UEIDData += "   | +- m-TMSI:".PadRight(30, ' ');
                                            B8[] mTMSI = new B8[] { 0, 0, 0, 0 };
                                            mTMSI[3] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[6];
                                            mTMSI[2] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[7];
                                            mTMSI[1] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[8];
                                            mTMSI[0] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[9];
                                            UEIDData += BitConverter.ToString(mTMSI) + "\r\n";
                                            break;
                                        case 4:
                                            UEIDType += "IMEI, ";

                                            UEIDData += "+- IMEI:".PadRight(30, ' ');
                                            UEIDData += BitConverter.ToString(L2pAgUeCaptureInd.mstUECaptureInfo.mau8IMEI) + "\r\n";
                                            break;
                                        case 8:
                                            UEIDType += "CRNTI, ";

                                            UEIDData += "+- CRNTIDATA:".PadRight(30, ' ');
                                            UEIDData += L2pAgUeCaptureInd.mstUECaptureInfo.mu16CRNTIDATA.ToString() + "(Ox" + L2pAgUeCaptureInd.mstUECaptureInfo.mu16CRNTIDATA.ToString("X") + ")" + "\r\n";
                                            break;
                                        case 16:
                                            UEIDType += "PRID, ";

                                            UEIDData += "+- PRIDDATA:".PadRight(30, ' ');
                                            UEIDData += L2pAgUeCaptureInd.mstUECaptureInfo.mu8PRIDDATA.ToString() + "\r\n";
                                            break;
                                        case 32:
                                            UEIDType += "S-TMSI, ";

                                            UEIDData += "+- S-TMSIDATA\r\n";
                                            UEIDData += "   | +- mmec:".PadRight(30, ' ');
                                            B8[] smmec = new B8[] { 0 };
                                            smmec[0] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[5];
                                            UEIDData += BitConverter.ToString(smmec) + "\r\n";
                                            UEIDData += "   | +- m-TMSI:".PadRight(30, ' ');
                                            B8[] sTMSI = new B8[] { 0, 0, 0, 0 };
                                            sTMSI[3] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[6];
                                            sTMSI[2] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[7];
                                            sTMSI[1] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[8];
                                            sTMSI[0] = L2pAgUeCaptureInd.mstUECaptureInfo.mau8GUTIDATA[9];
                                            UEIDData += BitConverter.ToString(sTMSI) + "\r\n";
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            if (UEIDType.Length > 2)
                            {
                                UEIDType = UEIDType.Remove(UEIDType.Length - 2);
                            }

                            str += UEIDType + "\r\n";
                            str += UEIDData + "\r\n";

                            //decodeView.richTextBox1.Text += str;
                            //decodeText = decodeView.richTextBox1.Text;
                            return null;
                        }
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_PROTOCOL_DATA_MSG_TYPE:
                        {
                            #region L2P_PROTOCOL_DATA解析

                            #region 将数据转换成结构体 L2P_PROTOCOL_DATA
                            L2P_PROTOCOL_DATA stru_L2P_Protocol_Data = new L2P_PROTOCOL_DATA();
                            //得到结构体的大小
                            int size_L2P_Protocal_Data = Marshal.SizeOf(stru_L2P_Protocol_Data);
                            //比较结构体大小，看数据长度是否正确
                            if (size_L2P_Protocal_Data > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr intptr_L2p_Protocal_Data = Marshal.AllocHGlobal(size_L2P_Protocal_Data);
                            //讲byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, intptr_L2p_Protocal_Data, size_L2P_Protocal_Data);
                            //讲内存空间转换为目标结构体
                            stru_L2P_Protocol_Data = (L2P_PROTOCOL_DATA)Marshal.PtrToStructure(intptr_L2p_Protocal_Data, typeof(L2P_PROTOCOL_DATA));
                            //释放内存空间
                            Marshal.FreeHGlobal(intptr_L2p_Protocal_Data);
                            #endregion

                            #region 将数组转换成ProtocolData结构体数组
                            byte[] data_L2P_Protocal_Data = new byte[e.data.Length - size_L2P_Protocal_Data];
                            System.Buffer.BlockCopy(e.data, size_L2P_Protocal_Data, data_L2P_Protocal_Data, 0, e.data.Length - size_L2P_Protocal_Data);
                            //decodeDataView = data_L2P_Protocal_Data;
                            #endregion
                            int stru_size = 0;
                            string str = null;
                            IntPtr stru_intptr;
                            int count = 0;//PDU个数
                            int Location = 0;
                            int temp = 0;
                            byte[] tempdata = new byte[512];
                            #region L2P_PROTOCOL_DATA
                            str += "+- Rnti Value ::= ".PadRight(5, ' ');
                            str += stru_L2P_Protocol_Data.mstProtocolDataHeader.mu32RntiValue.ToString() + "\r\n";
                            switch (stru_L2P_Protocol_Data.mu16DataType)
                            {
                                case 1://L2P_MAC_PRACH
                                    #region L2P_MAC_PRACH
                                    Location = 0;
                                    L2P_MAC_PRACH stru = new L2P_MAC_PRACH();
                                    stru_size = Marshal.SizeOf(stru);
                                    stru_intptr = Marshal.AllocHGlobal(stru_size);
                                    Marshal.Copy(e.data, size_L2P_Protocal_Data, stru_intptr, stru_size);
                                    stru = (L2P_MAC_PRACH)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_PRACH));
                                    Marshal.FreeHGlobal(stru_intptr);
                                    Location = size_L2P_Protocal_Data + stru_size;//当前数据位置
                                    //解析结构体 L2P_MAC_PRACH
                                    //==============================================================
                                    str += "+- L2P MAC PRACH\r\n";
                                    str += "    +- RadioFrameNum ::= " + stru.mu16RadioFrameNum.ToString() + "\r\n";
                                    str += "    +- Prach ::= " + stru.mu8Prach.ToString() + "\r\n";
                                    str += "    +- Tld ::= " + stru.mu8Tld.ToString() + "\r\n";
                                    str += "    +- DlCarrierFreq ::= " + stru.mu16DlCarrierFreq.ToString() + "\r\n";
                                    str += "    +- Pci ::= " + stru.mu16Pci.ToString() + "\r\n";
                                    str += "    +- CellIndex ::= " + stru.mu8CellIndex.ToString() + "\r\n";
                                    str += "    +- PRACH \r\n";
                                    //==============================================================

                                    //解析结构体 PRACH_INFO_STRU
                                    //============================================================
                                    for (int i = 0; i < stru.mu8Prach; i++)
                                    {
                                        PRACH_INFO_STRU prach_stru = new PRACH_INFO_STRU();
                                        stru_size = Marshal.SizeOf(prach_stru);
                                        stru_intptr = Marshal.AllocHGlobal(stru_size);
                                        Marshal.Copy(e.data, Location, stru_intptr, stru_size);
                                        prach_stru = (PRACH_INFO_STRU)Marshal.PtrToStructure(stru_intptr, typeof(PRACH_INFO_STRU));
                                        Marshal.FreeHGlobal(stru_intptr);
                                        Location += stru_size;

                                        if (i + 1 < stru.mu8Prach)
                                        {
                                            str += "        +- PRACH[" + i + "]\r\n";
                                            str += "            | +- fld ::= " + prach_stru.mu8fld.ToString() + "\r\n";
                                            str += "            | +- NPrid ::= " + prach_stru.mu8NPrid.ToString() + "\r\n";
                                            str += "            | +- Preambled ::= " + prach_stru.mu8Preambled.ToString() + "\r\n";
                                            str += "            | +- Ta ::= " + prach_stru.mu16Ta.ToString() + "\r\n";
                                            str += "            | +- Power ::= " + prach_stru.ms16Power.ToString() + "\r\n";
                                        }
                                        else
                                        {
                                            str += "            +- PRACH[" + i + "]\r\n";
                                            str += "              +- fld ::= " + prach_stru.mu8fld.ToString() + "\r\n";
                                            str += "              +- NPrid ::= " + prach_stru.mu8NPrid.ToString() + "\r\n";
                                            str += "              +- Preambled ::= " + prach_stru.mu8Preambled.ToString() + "\r\n";
                                            str += "              +- Ta ::= " + prach_stru.mu16Ta.ToString() + "\r\n";
                                            str += "              +- Power ::= " + prach_stru.ms16Power.ToString() + "\r\n";
                                        }
                                    }
                                    //============================================================
                                    #endregion
                                    break;
                                case 2://L2P_MAC_DCIINFO
                                    #region L2P_MAC_DCIINFO
                                    Location = 0;
                                    L2P_MAC_DCI_STRU stru2 = new L2P_MAC_DCI_STRU();
                                    stru_size = Marshal.SizeOf(stru2);
                                    stru_intptr = Marshal.AllocHGlobal(stru_size);
                                    Marshal.Copy(e.data, size_L2P_Protocal_Data, stru_intptr, stru_size);
                                    stru2 = (L2P_MAC_DCI_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_DCI_STRU));
                                    Marshal.FreeHGlobal(stru_intptr);
                                    Location = size_L2P_Protocal_Data + stru_size;
                                    //解析结构体 L2P_MAC_DCI_STRU
                                    //===========================================
                                    str += "+- L2P MAC DCIINFO\r\n";
                                    str += "  +- RadioFrameNumber ::= ".PadRight(5, ' ');
                                    str += stru2.mu16RadioFrameNumber.ToString() + "\r\n";
                                    str += "  +- SubFramNumber ::= ".PadRight(5, ' ');
                                    str += stru2.mu8SubFramNumber.ToString() + "\r\n";
                                    str += "  +- DciNumber ::= ".PadRight(5, ' ');
                                    str += stru2.mu8nDci.ToString() + "\r\n";
                                    str += "  +- DlCarrierFreq ::= ".PadRight(5, ' ');
                                    str += stru2.mu16DlCarrierFreq.ToString() + "\r\n";
                                    str += "  +- Pci ::= ".PadRight(5, ' ');
                                    str += stru2.mu16Pci.ToString() + "\r\n";
                                    str += "  +- CellIndex ::= ".PadRight(5, ' ');
                                    str += stru2.mu8CellIndex.ToString() + "\r\n";
                                    str += "  +- DCI \r\n";
                                    //===========================================
                                    PDCCH_DCI_STRU stru_PDCCH_DCI_STRU = new PDCCH_DCI_STRU();
                                    int size_PDCCH_DCI_STRU = Marshal.SizeOf(stru_PDCCH_DCI_STRU);
                                    IntPtr intptr_PDCCH_DCI_STRU = Marshal.AllocHGlobal(size_PDCCH_DCI_STRU);

                                    for (int i = 0; i < stru2.mu8nDci; i++)
                                    {
                                        Marshal.Copy(e.data, Location, intptr_PDCCH_DCI_STRU, size_PDCCH_DCI_STRU);
                                        Location = Location + size_PDCCH_DCI_STRU;
                                        stru_PDCCH_DCI_STRU = (PDCCH_DCI_STRU)Marshal.PtrToStructure(intptr_PDCCH_DCI_STRU, typeof(PDCCH_DCI_STRU));
                                        str += "    +- DCI[" + (i + 1).ToString() + "]" + "\r\n";
                                        str += "    | +- Rnti ::= ".PadRight(5, ' ');
                                        str += stru_PDCCH_DCI_STRU.mu16Rnti.ToString() + "\r\n";
                                        str += "    | +- CceIndex ::= ".PadRight(5, ' ');
                                        str += stru_PDCCH_DCI_STRU.mu16CceIndex.ToString() + "\r\n";
                                        str += "    | +- AggregationLvl ::= ".PadRight(5, ' ');
                                        str += stru_PDCCH_DCI_STRU.mu8AggregationLvl.ToString() + "\r\n";
                                        str += "    | +- RntiType ::= ".PadRight(5, ' ');
                                        str += stru_PDCCH_DCI_STRU.mu8RntiType.ToString() + "\r\n";
                                        str += "    | +- DciFormat ::= ".PadRight(5, ' ');
                                        int size_DciBody = 24;
                                        byte[] DiBody = new byte[size_DciBody];
                                        System.Buffer.BlockCopy(e.data, Location, DiBody, 0, size_DciBody);
                                        Location = Location + size_DciBody;
                                        if (i + 1 < stru2.mu8nDci)
                                        {
                                            #region DIC 原始码流解析
                                            switch (stru_PDCCH_DCI_STRU.mu32DciFormat)
                                            {
                                                case 0:
                                                    #region DCI_Format0
                                                    {
                                                        str += "DCI_Format0\r\n";/* DCI_Format0,DCI_Format1,DCI_Format1A,DCI_Format1B,DCI_Format1C,DCI_Format1D,
                                   DCI_Format2,DCI_Format2A,DCI_Format2B,DCI_Format3,DCI_Format3A */
                                                        str += "    | +- DciBody" + "\r\n";
                                                        DCI_FORMAT0_Type dciForamt0 = new DCI_FORMAT0_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt0 = Marshal.SizeOf(dciForamt0);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt0 > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt0 = Marshal.AllocHGlobal(size_dciForamt0);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt0, size_dciForamt0);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt0 = (DCI_FORMAT0_Type)Marshal.PtrToStructure(intptr_dciForamt0, typeof(DCI_FORMAT0_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt0);

                                                        str += "    |   +- HoppingFlag ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.hoppingFlag + "  /* Frequency hopping flag */" + "\r\n";
                                                        str += "    |   +- MCSIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.mcsIndex + "  /* Modulation and coding scheme and redundancy version */" + "\r\n";
                                                        str += "    |   +- NDI ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.ndi + "  /* New data indicator */" + "\r\n";
                                                        str += "    |   +- Tpc ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.tpc + "  /* TPC command for scheduled PUSCH */" + "\r\n";
                                                        str += "    |   +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.resourceAlloc + "  /* Resource block assignment and hopping resource allocation */" + "\r\n";
                                                        str += "    |   +- ShiftDMRS ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.shiftDMRS + "  /* Cyclic shift for DM RS */" + "\r\n";
                                                        str += "    |   +- UlIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.ulIndex + "  /* UL index */" + "\r\n";
                                                        str += "    |   +- Dai ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.dai + "  /* Downlink Assignment Index */" + "\r\n";
                                                        str += "    |   +- CQIReq ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.cqiReq + "  /* CQI request */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 1:
                                                    #region DCI_Format1
                                                    {
                                                        str += "DCI_Format1\r\n";/* DCI_Format0,DCI_Format1,DCI_Format1A,DCI_Format1B,DCI_Format1C,DCI_Format1D,
                                   DCI_Format2,DCI_Format2A,DCI_Format2B,DCI_Format3,DCI_Format3A */
                                                        str += "    | +- DciBody" + "\r\n";
                                                        DCI_FORMAT1_Type dciForamt1 = new DCI_FORMAT1_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt1 = Marshal.SizeOf(dciForamt1);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt1 > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt1 = Marshal.AllocHGlobal(size_dciForamt1);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt1, size_dciForamt1);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt1 = (DCI_FORMAT1_Type)Marshal.PtrToStructure(intptr_dciForamt1, typeof(DCI_FORMAT1_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt1);

                                                        str += "    |   +- MCSIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.mcsIndex + "  /* Modulation and coding scheme */" + "\r\n";
                                                        str += "    |   +- HARQProcId ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.harqProcId + "  /* HARQ process number */" + "\r\n";
                                                        str += "    |   +- NDI ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.ndi + "  /* New data indicator */" + "\r\n";
                                                        str += "    |   +- RV ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.rv + "  /* Redundancy version */" + "\r\n";
                                                        str += "    |   +- TPCPucch ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.tpcPucch + "  /* TPC command for PUCCH */" + "\r\n";
                                                        str += "    |   +- DAI ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.dai + "  /* Downlink Assignment Index */" + "\r\n";
                                                        str += "    |   +- ResAllocatType ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.resAllocatType + "  /* Resource allocation header */" + "\r\n";
                                                        str += "    |   +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                        str += "    |   +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 2:
                                                    #region DCI_Format1A
                                                    {
                                                        str += "DCI_Format1A\r\n";/* DCI_Format0,DCI_Format1,DCI_Format1A,DCI_Format1B,DCI_Format1C,DCI_Format1D,
                                   DCI_Format2,DCI_Format2A,DCI_Format2B,DCI_Format3,DCI_Format3A */
                                                        str += "    | +- DciBody" + "\r\n";
                                                        DCI_FORMAT1A_Type dciForamt1a = new DCI_FORMAT1A_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt1a = Marshal.SizeOf(dciForamt1a);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt1a > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt1a = Marshal.AllocHGlobal(size_dciForamt1a);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt1a, size_dciForamt1a);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt1a = (DCI_FORMAT1A_Type)Marshal.PtrToStructure(intptr_dciForamt1a, typeof(DCI_FORMAT1A_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt1a);

                                                        str += "    |   +- Usage ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.usage + "  /* 0-used for random access procedure,1-otherwise */" + "\r\n";
                                                        if (dciForamt1a.usage == 0)
                                                        {
                                                            str += "    |   +- PreambleIndex ::= ".PadRight(5, ' ');
                                                            str += dciForamt1a.preambleIndex + "\r\n";
                                                            str += "    |   +- PrachMaskIndex ::= ".PadRight(5, ' ');
                                                            str += dciForamt1a.prachMaskIndex + "\r\n";
                                                        }
                                                        str += "    |   +- ResAllocatType ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.resAllocatType + "  /* Localized/Distributed VRB assignment flag:0-Localized,1-Distributed */" + "\r\n";
                                                        str += "    |   +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                        str += "    |   +- MCSIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.mcsIndex + "  /* Modulation and coding scheme */" + "\r\n";
                                                        str += "    |   +- HarqProcId ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.harqProcId + "  /* HARQ process number */" + "\r\n";
                                                        str += "    |   +- NDI ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.ndi + "  /* New data indicator */" + "\r\n";
                                                        str += "    |   +- RV ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.rv + "  /* Redundancy version */" + "\r\n";
                                                        str += "    |   +- TPCPucch ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.tpcPucch + "  /* TPC command for PUCCH */" + "\r\n";
                                                        str += "    |   +- DAI ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.dai + "  /* Downlink Assignment Index */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 3:
                                                    #region DCI_Format1B

                                                    #endregion
                                                    break;
                                                case 4:
                                                    #region DCI_Format1C
                                                    {
                                                        str += "DCI_Format1C\r\n";
                                                        str += "    | +- DciBody" + "\r\n";
                                                        DCI_FORMAT1C_Type dciForamt1c = new DCI_FORMAT1C_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt1c = Marshal.SizeOf(dciForamt1c);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt1c > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt1c = Marshal.AllocHGlobal(size_dciForamt1c);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt1c, size_dciForamt1c);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt1c = (DCI_FORMAT1C_Type)Marshal.PtrToStructure(intptr_dciForamt1c, typeof(DCI_FORMAT1C_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt1c);

                                                        str += "    |   +- Gap ::= ".PadRight(5, ' ');
                                                        str += dciForamt1c.Gap + "\r\n";
                                                        str += "    |   +- TBSizeIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt1c.tbSizeIndex + "\r\n";
                                                        str += "    |   +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt1c.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 5:
                                                    #region DCI_Format1D
                                                    #endregion
                                                    break;
                                                case 6:
                                                    #region DCI_Format2
                                                    #endregion
                                                    break;
                                                case 7:
                                                    #region DCI_Format2A
                                                    {
                                                        str += "DCI_Format2A\r\n";
                                                        str += "    | +- DciBody" + "\r\n";
                                                        DCI_FORMAT2A_Type dciForamt2a = new DCI_FORMAT2A_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt2a = Marshal.SizeOf(dciForamt2a);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt2a > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt2a = Marshal.AllocHGlobal(size_dciForamt2a);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt2a, size_dciForamt2a);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt2a = (DCI_FORMAT2A_Type)Marshal.PtrToStructure(intptr_dciForamt2a, typeof(DCI_FORMAT2A_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt2a);


                                                        str += "    |   +- TPCPucch ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.tpcPucch + "  /* TPC command for PUCCH */" + "\r\n";
                                                        str += "    |   +- DAI ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.dai + "  /* Downlink Assignment Index */" + "\r\n";
                                                        str += "    |   +- HARQProcId ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.harqProcId + "  /* HARQ process number */" + "\r\n";
                                                        str += "    |   +- SWAPFlag ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.swapFlag + "  /* Transport block to codeword swap flag */" + "\r\n";
                                                        str += "    |   +- MCSIndex1 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.mcsIndex1 + "  /* Modulation and coding scheme for Transport Block 1 */" + "\r\n";
                                                        str += "    |   +- NDI1 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.ndi1 + "  /* New data indicator for Transport Block 1 */" + "\r\n";
                                                        str += "    |   +- RV1 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.rv1 + "  /* Redundancy version for Transport Block 1 */" + "\r\n";
                                                        str += "    |   +- MCSIndex2 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.mcsIndex2 + "  /* Modulation and coding scheme for Transport Block 2 */" + "\r\n";
                                                        str += "    |   +- NDI2 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.ndi2 + "  /* New data indicator for Transport Block 2 */" + "\r\n";
                                                        str += "    |   +- RV2 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.rv2 + "  /* Redundancy version for Transport Block 2 */" + "\r\n";
                                                        str += "    |   +- PrecodingInfo ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.precodingInfo + "  /* Precoding information */" + "\r\n";
                                                        str += "    |   +- ResAllocatType ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.resAllocatType + "  /* Resource allocation header:0-type 0,1-type 1 */" + "\r\n";
                                                        str += "    |   +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 8:
                                                    #region DCI_Format3
                                                    #endregion
                                                    break;
                                                case 9:
                                                    #region DCI_Format3A
                                                    #endregion
                                                    break;
                                                default:
                                                    break;
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            #region DIC 原始码流解析
                                            switch (stru_PDCCH_DCI_STRU.mu32DciFormat)
                                            {
                                                case 0:
                                                    #region DCI_Format0
                                                    {
                                                        str += "DCI_Format0\r\n";/* DCI_Format0,DCI_Format1,DCI_Format1A,DCI_Format1B,DCI_Format1C,DCI_Format1D,
                                   DCI_Format2,DCI_Format2A,DCI_Format2B,DCI_Format3,DCI_Format3A */
                                                        str += "      +- DciBody" + "\r\n";
                                                        DCI_FORMAT0_Type dciForamt0 = new DCI_FORMAT0_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt0 = Marshal.SizeOf(dciForamt0);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt0 > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt0 = Marshal.AllocHGlobal(size_dciForamt0);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt0, size_dciForamt0);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt0 = (DCI_FORMAT0_Type)Marshal.PtrToStructure(intptr_dciForamt0, typeof(DCI_FORMAT0_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt0);

                                                        str += "        +- HoppingFlag ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.hoppingFlag + "  /* Frequency hopping flag */" + "\r\n";
                                                        str += "        +- MCSIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.mcsIndex + "  /* Modulation and coding scheme and redundancy version */" + "\r\n";
                                                        str += "        +- NDI ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.ndi + "  /* New data indicator */" + "\r\n";
                                                        str += "        +- Tpc ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.tpc + "  /* TPC command for scheduled PUSCH */" + "\r\n";
                                                        str += "        +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.resourceAlloc + "  /* Resource block assignment and hopping resource allocation */" + "\r\n";
                                                        str += "        +- ShiftDMRS ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.shiftDMRS + "  /* Cyclic shift for DM RS */" + "\r\n";
                                                        str += "        +- UlIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.ulIndex + "  /* UL index */" + "\r\n";
                                                        str += "        +- Dai ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.dai + "  /* Downlink Assignment Index */" + "\r\n";
                                                        str += "        +- CQIReq ::= ".PadRight(5, ' ');
                                                        str += dciForamt0.cqiReq + "  /* CQI request */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 1:
                                                    #region DCI_Format1
                                                    {
                                                        str += "DCI_Format1\r\n";/* DCI_Format0,DCI_Format1,DCI_Format1A,DCI_Format1B,DCI_Format1C,DCI_Format1D,
                                   DCI_Format2,DCI_Format2A,DCI_Format2B,DCI_Format3,DCI_Format3A */
                                                        str += "      +- DciBody" + "\r\n";
                                                        DCI_FORMAT1_Type dciForamt1 = new DCI_FORMAT1_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt1 = Marshal.SizeOf(dciForamt1);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt1 > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt1 = Marshal.AllocHGlobal(size_dciForamt1);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt1, size_dciForamt1);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt1 = (DCI_FORMAT1_Type)Marshal.PtrToStructure(intptr_dciForamt1, typeof(DCI_FORMAT1_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt1);

                                                        str += "        +- MCSIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.mcsIndex + "  /* Modulation and coding scheme */" + "\r\n";
                                                        str += "        +- HARQProcId ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.harqProcId + "  /* HARQ process number */" + "\r\n";
                                                        str += "        +- NDI ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.ndi + "  /* New data indicator */" + "\r\n";
                                                        str += "        +- RV ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.rv + "  /* Redundancy version */" + "\r\n";
                                                        str += "        +- TPCPucch ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.tpcPucch + "  /* TPC command for PUCCH */" + "\r\n";
                                                        str += "        +- DAI ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.dai + "  /* Downlink Assignment Index */" + "\r\n";
                                                        str += "        +- ResAllocatType ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.resAllocatType + "  /* Resource allocation header */" + "\r\n";
                                                        str += "        +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                        str += "        +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt1.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 2:
                                                    #region DCI_Format1A
                                                    {
                                                        str += "DCI_Format1A\r\n";/* DCI_Format0,DCI_Format1,DCI_Format1A,DCI_Format1B,DCI_Format1C,DCI_Format1D,
                                   DCI_Format2,DCI_Format2A,DCI_Format2B,DCI_Format3,DCI_Format3A */
                                                        str += "      +- DciBody" + "\r\n";
                                                        DCI_FORMAT1A_Type dciForamt1a = new DCI_FORMAT1A_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt1a = Marshal.SizeOf(dciForamt1a);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt1a > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt1a = Marshal.AllocHGlobal(size_dciForamt1a);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt1a, size_dciForamt1a);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt1a = (DCI_FORMAT1A_Type)Marshal.PtrToStructure(intptr_dciForamt1a, typeof(DCI_FORMAT1A_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt1a);

                                                        str += "        +- Usage ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.usage + "  /* 0-used for random access procedure,1-otherwise */" + "\r\n";
                                                        if (dciForamt1a.usage == 0)
                                                        {
                                                            str += "        +- PreambleIndex ::= ".PadRight(5, ' ');
                                                            str += dciForamt1a.preambleIndex + "\r\n";
                                                            str += "        +- PrachMaskIndex ::= ".PadRight(5, ' ');
                                                            str += dciForamt1a.prachMaskIndex + "\r\n";
                                                        }
                                                        str += "        +- ResAllocatType ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.resAllocatType + "  /* Localized/Distributed VRB assignment flag:0- Localized,1-Distributed */" + "\r\n";
                                                        str += "        +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                        str += "        +- MCSIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.mcsIndex + "  /* Modulation and coding scheme */" + "\r\n";
                                                        str += "        +- HarqProcId ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.harqProcId + "  /* HARQ process number */" + "\r\n";
                                                        str += "        +- NDI ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.ndi + "  /* New data indicator */" + "\r\n";
                                                        str += "        +- RV ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.rv + "  /* Redundancy version */" + "\r\n";
                                                        str += "        +- TPCPucch ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.tpcPucch + "  /* TPC command for PUCCH */" + "\r\n";
                                                        str += "        +- DAI ::= ".PadRight(5, ' ');
                                                        str += dciForamt1a.dai + "  /* Downlink Assignment Index */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 3:
                                                    #region DCI_Format1B 待完成

                                                    #endregion
                                                    break;
                                                case 4:
                                                    #region DCI_Format1C
                                                    {
                                                        str += "DCI_Format1C\r\n";
                                                        str += "      +- DciBody" + "\r\n";
                                                        DCI_FORMAT1C_Type dciForamt1c = new DCI_FORMAT1C_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt1c = Marshal.SizeOf(dciForamt1c);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt1c > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt1c = Marshal.AllocHGlobal(size_dciForamt1c);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt1c, size_dciForamt1c);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt1c = (DCI_FORMAT1C_Type)Marshal.PtrToStructure(intptr_dciForamt1c, typeof(DCI_FORMAT1C_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt1c);

                                                        str += "        +- Gap ::= ".PadRight(5, ' ');
                                                        str += dciForamt1c.Gap + "\r\n";
                                                        str += "        +- TBSizeIndex ::= ".PadRight(5, ' ');
                                                        str += dciForamt1c.tbSizeIndex + "\r\n";
                                                        str += "        +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt1c.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                case 7:
                                                    #region DCI_Format2A
                                                    {
                                                        str += "DCI_Format2A\r\n";
                                                        str += "      +- DciBody" + "\r\n";
                                                        DCI_FORMAT2A_Type dciForamt2a = new DCI_FORMAT2A_Type();
                                                        //得到结构体的大小
                                                        int size_dciForamt2a = Marshal.SizeOf(dciForamt2a);
                                                        //比较结构体大小，看数据长度是否正确
                                                        if (size_dciForamt2a > size_DciBody)
                                                        {
                                                            return null;
                                                        }
                                                        //分配结构体大小的内存空间
                                                        IntPtr intptr_dciForamt2a = Marshal.AllocHGlobal(size_dciForamt2a);
                                                        //讲byte数组拷贝到内存空间
                                                        Marshal.Copy(DiBody, 0, intptr_dciForamt2a, size_dciForamt2a);
                                                        //讲内存空间转换为目标结构体
                                                        dciForamt2a = (DCI_FORMAT2A_Type)Marshal.PtrToStructure(intptr_dciForamt2a, typeof(DCI_FORMAT2A_Type));
                                                        //释放内存空间
                                                        Marshal.FreeHGlobal(intptr_dciForamt2a);


                                                        str += "        +- TPCPucch ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.tpcPucch + "  /* TPC command for PUCCH */" + "\r\n";
                                                        str += "        +- DAI ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.dai + "  /* Downlink Assignment Index */" + "\r\n";
                                                        str += "        +- HARQProcId ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.harqProcId + "  /* HARQ process number */" + "\r\n";
                                                        str += "        +- SWAPFlag ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.swapFlag + "  /* Transport block to codeword swap flag */" + "\r\n";
                                                        str += "        +- MCSIndex1 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.mcsIndex1 + "  /* Modulation and coding scheme for Transport Block 1 */" + "\r\n";
                                                        str += "        +- NDI1 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.ndi1 + "  /* New data indicator for Transport Block 1 */" + "\r\n";
                                                        str += "        +- RV1 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.rv1 + "  /* Redundancy version for Transport Block 1 */" + "\r\n";
                                                        str += "        +- MCSIndex2 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.mcsIndex2 + "  /* Modulation and coding scheme for Transport Block 2 */" + "\r\n";
                                                        str += "        +- NDI2 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.ndi2 + "  /* New data indicator for Transport Block 2 */" + "\r\n";
                                                        str += "        +- RV2 ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.rv2 + "  /* Redundancy version for Transport Block 2 */" + "\r\n";
                                                        str += "        +- PrecodingInfo ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.precodingInfo + "  /* Precoding information */" + "\r\n";
                                                        str += "        +- ResAllocatType ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.resAllocatType + "  /* Resource allocation header:0-type 0,1-type 1 */" + "\r\n";
                                                        str += "        +- ResourceAlloc ::= ".PadRight(5, ' ');
                                                        str += dciForamt2a.resourceAlloc + "  /* Resource block assignment */" + "\r\n";
                                                    }
                                                    #endregion
                                                    break;
                                                default:
                                                    break;
                                            }
                                            #endregion
                                        }

                                    }
                                    Marshal.FreeHGlobal(intptr_PDCCH_DCI_STRU);
                                    #endregion
                                    break;
                                case 3://L2P_MAC_HICHINFO
                                    #region L2P_MAC_HICHINFO
                                    str += "+- L2P MAC HICHINFO\r\n";
                                    L2P_MAC_HICH_STRU stru3 = new L2P_MAC_HICH_STRU();
                                    stru_size = Marshal.SizeOf(stru3);
                                    stru_intptr = Marshal.AllocHGlobal(stru_size);
                                    Marshal.Copy(e.data, size_L2P_Protocal_Data, stru_intptr, stru_size);
                                    stru3 = (L2P_MAC_HICH_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_HICH_STRU));
                                    Marshal.FreeHGlobal(stru_intptr);
                                    #endregion
                                    break;
                                case 4://L2P_MAC_CE
                                    #region L2P_MAC_CE
                                    str += "+- L2P MAC CE\r\n";
                                    //byte[] tempdata = new byte[6*8*2];
                                    Location = 0;
                                    L2P_MAC_DATA_STRU stru1 = new L2P_MAC_DATA_STRU();
                                    stru_size = Marshal.SizeOf(stru1);
                                    stru_intptr = Marshal.AllocHGlobal(stru_size);
                                    Marshal.Copy(e.data, size_L2P_Protocal_Data, stru_intptr, stru_size);
                                    stru1 = (L2P_MAC_DATA_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_DATA_STRU));
                                    Marshal.FreeHGlobal(stru_intptr);
                                    Location = size_L2P_Protocal_Data + stru_size;
                                    temp = 0;
                                    for (int j = 0; j < stru1.mu8TbNum; j++)//TB个数
                                    {
                                        for (int i = 0; i < stru1.mastMacTbInfo[j].mu16MacCeNum; i++)//TB中MAC CE的个数
                                        {
                                            L2P_MAC_CE_STRU ce_stru = new L2P_MAC_CE_STRU();
                                            stru_size = Marshal.SizeOf(ce_stru);
                                            stru_intptr = Marshal.AllocHGlobal(stru_size);
                                            Marshal.Copy(e.data, Location, stru_intptr, stru_size);
                                            //Marshal.Copy(e.data, Location + stru_size * i * j, stru_intptr, stru_size);
                                            ce_stru = (L2P_MAC_CE_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_CE_STRU));
                                            Marshal.FreeHGlobal(stru_intptr);

                                            //decodeDataView = new byte[];
                                            System.Buffer.BlockCopy(ce_stru.mau8CeData, 0, tempdata, temp, 6);
                                            temp += 6;
                                            //decodeDataView = ce_stru.mau8CeData;//获取MAC CE码流

                                            Location += stru_size;
                                            //Console.WriteLine(ce_stru.mu8MacCeType.ToString());
                                            switch (ce_stru.mu8MacCeType)
                                            {
                                                case 0://SHORT_BSR                                                         
                                                    stru_intptr = Marshal.AllocHGlobal(8);//申请结构体指针
                                                    MAC_CE_SHORT_BSR_STRU shortBsr = new MAC_CE_SHORT_BSR_STRU();
                                                    //macCeAnalyze(0, ce_stru.mau8CeData, stru_intptr);//调用库函数解析
                                                    shortBsr = (MAC_CE_SHORT_BSR_STRU)Marshal.PtrToStructure(stru_intptr, typeof(MAC_CE_SHORT_BSR_STRU));
                                                    Marshal.FreeHGlobal(stru_intptr);
                                                    str += "   +- SHORT BSR\r\n";
                                                    str += "      +- LcgId ::= ".PadRight(5, ' ');
                                                    str += "0x" + shortBsr.mu8LCG_ID.ToString("X") + "\r\n";
                                                    str += "      +- BufferSize ::= ".PadRight(5, ' ');
                                                    str += "0x" + shortBsr.mu8BufferSize.ToString("X") + "\r\n";
                                                    //Console.WriteLine("Hello\n");
                                                    break;
                                                case 1://TRUNCATED_BSR
                                                    stru_intptr = Marshal.AllocHGlobal(8);//申请结构体指针
                                                    MAC_CE_TRUNCATED_BSR_STRU truncatedBsr = new MAC_CE_TRUNCATED_BSR_STRU();
                                                    //macCeAnalyze(1, ce_stru.mau8CeData, stru_intptr);
                                                    truncatedBsr = (MAC_CE_TRUNCATED_BSR_STRU)Marshal.PtrToStructure(stru_intptr, typeof(MAC_CE_TRUNCATED_BSR_STRU)); ;
                                                    Marshal.FreeHGlobal(stru_intptr);
                                                    str += "   +- TRUNCATED BSR\r\n";
                                                    str += "      +- LcgId ::= ".PadRight(5, ' ');
                                                    str += "0x" + truncatedBsr.mu8LCG_ID.ToString("X") + "\r\n";
                                                    str += "      +- BufferSize ::= ".PadRight(5, ' ');
                                                    str += "0x" + truncatedBsr.mu8BufferSize.ToString("X") + "\r\n";
                                                    break;
                                                case 2://LONG BSR
                                                    stru_intptr = Marshal.AllocHGlobal(8);//申请结构体指针
                                                    MAC_CE_LONG_BSR_STRU longBsr = new MAC_CE_LONG_BSR_STRU();
                                                    //macCeAnalyze(2, ce_stru.mau8CeData, stru_intptr);
                                                    longBsr = (MAC_CE_LONG_BSR_STRU)Marshal.PtrToStructure(stru_intptr, typeof(MAC_CE_LONG_BSR_STRU)); ;
                                                    Marshal.FreeHGlobal(stru_intptr);
                                                    str += "   +- LONG BSR\r\n";
                                                    str += "      +- BufferSize0 ::= ".PadRight(5, ' ');
                                                    str += "0x" + longBsr.mu8Lcg0BufferSize.ToString("X") + "\r\n";
                                                    str += "      +- BufferSize1 ::= ".PadRight(5, ' ');
                                                    str += "0x" + longBsr.mu8Lcg1BufferSize.ToString("X") + "\r\n";
                                                    str += "      +- BufferSize2 ::= ".PadRight(5, ' ');
                                                    str += "0x" + longBsr.mu8Lcg2BufferSize.ToString("X") + "\r\n";
                                                    str += "      +- BufferSize3 ::= ".PadRight(5, ' ');
                                                    str += "0x" + longBsr.mu8Lcg3BufferSize.ToString("X") + "\r\n";
                                                    break;
                                                case 3://C_RNTI
                                                    stru_intptr = Marshal.AllocHGlobal(8);//申请结构体指针
                                                    MAC_CE_C_RNTI_STRU cRnti = new MAC_CE_C_RNTI_STRU();
                                                    //macCeAnalyze(3, ce_stru.mau8CeData, stru_intptr);
                                                    cRnti = (MAC_CE_C_RNTI_STRU)Marshal.PtrToStructure(stru_intptr, typeof(MAC_CE_C_RNTI_STRU)); ;
                                                    Marshal.FreeHGlobal(stru_intptr);
                                                    str += "   +- C RNTI\r\n";
                                                    str += "      +- CRnti ::= ".PadRight(5, ' ');
                                                    str += "0x" + cRnti.u16CRnti.ToString("X") + "\r\n";
                                                    break;
                                                case 4://DRX_COMMAND

                                                    break;
                                                case 5://UE Contention Resolution Identity
                                                    stru_intptr = Marshal.AllocHGlobal(8);//申请结构体指针
                                                    MAC_CE_UE_CR_ID_STRU CRId = new MAC_CE_UE_CR_ID_STRU();
                                                    //macCeAnalyze(5, ce_stru.mau8CeData, stru_intptr);
                                                    CRId = (MAC_CE_UE_CR_ID_STRU)Marshal.PtrToStructure(stru_intptr, typeof(MAC_CE_UE_CR_ID_STRU)); ;
                                                    Marshal.FreeHGlobal(stru_intptr);
                                                    str += "   +- UE Contention Resolution Identity\r\n";
                                                    for (int n = 0; n < 6; n++)
                                                    {
                                                        str += "      +- UeCRId[" + (n + 1).ToString() + "]::= " + "0x" + CRId.mau8UeCrID[n].ToString("X") + "\r\n";
                                                    }
                                                    break;
                                                case 6://TIMMING_ADVANCE_COMMAND
                                                    stru_intptr = Marshal.AllocHGlobal(8);//申请结构体指针
                                                    MAC_CE_TIMING_ADVANCE_COMMAND_STRU TACommand = new MAC_CE_TIMING_ADVANCE_COMMAND_STRU();
                                                    //macCeAnalyze(6, ce_stru.mau8CeData, stru_intptr);
                                                    TACommand = (MAC_CE_TIMING_ADVANCE_COMMAND_STRU)Marshal.PtrToStructure(stru_intptr, typeof(MAC_CE_TIMING_ADVANCE_COMMAND_STRU)); ;
                                                    Marshal.FreeHGlobal(stru_intptr);
                                                    str += "   +- TIMMING ADVANCE COMMAND\r\n";
                                                    str += "      +- Command ::= " + "0x" + TACommand.mu8TAC.ToString("X") + "\r\n";
                                                    break;
                                                case 7://PHR
                                                    stru_intptr = Marshal.AllocHGlobal(8);//申请结构体指针
                                                    MAC_CE_PHR_STRU PHR = new MAC_CE_PHR_STRU();
                                                    //macCeAnalyze(7, ce_stru.mau8CeData, stru_intptr);
                                                    PHR = (MAC_CE_PHR_STRU)Marshal.PtrToStructure(stru_intptr, typeof(MAC_CE_PHR_STRU)); ;
                                                    Marshal.FreeHGlobal(stru_intptr);
                                                    str += "   +- PHR\r\n";
                                                    str += "      +- PH ::= " + "0x" + PHR.mu8PH.ToString("X") + "\r\n";
                                                    break;
                                                case 8://RAR
                                                    stru_intptr = Marshal.AllocHGlobal(12);//申请结构体指针
                                                    L2P_MAC_RAR_STRU rar_stru = new L2P_MAC_RAR_STRU();

                                                    IntPtr myptr;
                                                    L2P_MAC_RAR_SUBHEADER_STRU subheader_stru = new L2P_MAC_RAR_SUBHEADER_STRU();
                                                    myptr = Marshal.AllocHGlobal(136);
                                                    //macRarSubHeaderAnalyze(ce_stru.mau8CeData, myptr);
                                                    subheader_stru = (L2P_MAC_RAR_SUBHEADER_STRU)Marshal.PtrToStructure(myptr, typeof(L2P_MAC_RAR_SUBHEADER_STRU)); ;
                                                    Marshal.FreeHGlobal(myptr);
                                                    //2014/5/19修改
                                                    //macRarAnalyze((byte)ce_stru.mu8CeLength, ce_stru.mau8CeData, stru_intptr);
                                                    rar_stru = (L2P_MAC_RAR_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_RAR_STRU));
                                                    Marshal.FreeHGlobal(stru_intptr);
                                                    str += "   +- RAR\r\n";
                                                    str += "      +- RAR ::= 0x" + rar_stru.mu8nRAR.ToString("X") + "\r\n";
                                                    str += "      +- TaCommand ::= 0x" + rar_stru.RarInfo.mu16TAC.ToString("X") + "\r\n";
                                                    str += "      +- TcRnti ::= 0x" + rar_stru.RarInfo.mu16TcRnti.ToString("X") + "(" + rar_stru.RarInfo.mu16TcRnti.ToString() + ")" + "\r\n";
                                                    str += "      +- UIGrant ::= 0x" + rar_stru.RarInfo.mu32UIGrant.ToString("X") + "\r\n";
                                                    str += "*******************************************\r\n";
                                                    break;
                                            }

                                        }
                                    }
                                    Level = 1;
                                    //decodeDataView = new byte[temp];
                                    //System.Buffer.BlockCopy(tempdata, 0, decodeDataView, 0, temp);
                                    #endregion
                                    break;
                                case 5://L2P_MAC_SUBHEAD
                                    #region MAC_SUBHEAD
                                    str += "+- L2P MAC SUBHEAD\r\n";
                                    // byte tempdata = new byte[512];
                                    Location = 0;

                                    L2P_MAC_DATA_STRU data_stru = new L2P_MAC_DATA_STRU();
                                    stru_size = Marshal.SizeOf(data_stru);
                                    stru_intptr = Marshal.AllocHGlobal(stru_size);
                                    Marshal.Copy(e.data, size_L2P_Protocal_Data, stru_intptr, stru_size);
                                    data_stru = (L2P_MAC_DATA_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_DATA_STRU));
                                    Marshal.FreeHGlobal(stru_intptr);
                                    //传输参数，进行数据解析
                                    Location = size_L2P_Protocal_Data + stru_size;
                                    temp = 0;
                                    for (int i = 0; i < data_stru.mu8TbNum; i++)//TB个数
                                    {
                                        //获取MAC字头码流
                                        byte[] data = new byte[data_stru.mastMacTbInfo[i].mu16SubHeaderLen * 4];
                                        System.Buffer.BlockCopy(e.data, Location, data, 0, data.Length);

                                        System.Buffer.BlockCopy(data, 0, tempdata, temp, data.Length);
                                        temp += data.Length;
                                        //decodeDataView = new byte[data.Length];
                                        //decodeDataView = data;

                                        //判断mu8RntiType的类型
                                        str += "    +- TB[" + i.ToString() + "]\r\n";
                                        if (stru_L2P_Protocol_Data.mstProtocolDataHeader.mu8RntiType == 2)//RAR子头
                                        {
                                            L2P_MAC_RAR_SUBHEADER_STRU rar_subheader_stru = new L2P_MAC_RAR_SUBHEADER_STRU();
                                            stru_intptr = Marshal.AllocHGlobal(136);
                                            //macRarSubHeaderAnalyze(data, stru_intptr);
                                            rar_subheader_stru = (L2P_MAC_RAR_SUBHEADER_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_RAR_SUBHEADER_STRU));
                                            Marshal.FreeHGlobal(stru_intptr);
                                            str += "    +- SubHeaderNum ::= " + "0x" + rar_subheader_stru.u32SubHeaderNum.ToString("X") + "\r\n";
                                            str += "    +- HeadSize ::= " + "0x" + rar_subheader_stru.u32HeadSize.ToString("X") + "\r\n";
                                            for (int j = 0; j < 32; j++)
                                            {
                                                str += "        +- BI[" + (j + 1).ToString() + "] ::= ".PadRight(10, ' ') + "0x" + rar_subheader_stru.rarSubHeaderInfo[j].u8BI.ToString("X") + "\r\n";
                                                str += "        +- RAPID[" + (j + 1).ToString() + "] ::= ".PadRight(7, ' ') + "0x" + rar_subheader_stru.rarSubHeaderInfo[j].u8RAPID.ToString("X") + "\r\n";
                                                str += "********************************************\r\n";
                                            }
                                            //decodeView.richTextBox1.Text += str;
                                            //decodeText = decodeView.richTextBox1.Text;
                                        }
                                        else//PDU子头
                                        {
                                            L2P_MAC_PDU_SUBHEADER_STRU pdu_subheader_stru = new L2P_MAC_PDU_SUBHEADER_STRU();
                                            stru_intptr = Marshal.AllocHGlobal(136);
                                            //macPduSubHeaderAnalyze(data, stru_intptr);
                                            pdu_subheader_stru = (L2P_MAC_PDU_SUBHEADER_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_MAC_PDU_SUBHEADER_STRU));
                                            Marshal.FreeHGlobal(stru_intptr);

                                            str += "    +- SubHeaderNum ::= " + pdu_subheader_stru.u32SubHeaderNum.ToString() + "\r\n";
                                            str += "    +- HeadSize ::= " + pdu_subheader_stru.u32HeadSize.ToString() + "\r\n";
                                            for (int j = 0; j < 32; j++)
                                            {
                                                str += "        +- Lcid[" + (j + 1).ToString() + "] ::= ".PadRight(7, ' ') + "0x" + pdu_subheader_stru.pduSubHeaderInfo[j].u8Lcid.ToString("X") + "\r\n";
                                                str += "        +- F[" + (j + 1).ToString() + "] ::= ".PadRight(10, ' ') + "0x" + pdu_subheader_stru.pduSubHeaderInfo[j].u8F.ToString("X") + "\r\n";
                                                str += "        +- L[" + (j + 1).ToString() + "] ::= ".PadRight(10, ' ') + "0x" + pdu_subheader_stru.pduSubHeaderInfo[j].u16L.ToString("X") + "\r\n";
                                                str += "**************************************************\n";
                                            }
                                            //decodeView.richTextBox1.Text += str;
                                            //decodeText = decodeView.richTextBox1.Text;
                                        }
                                        Location += data.Length;
                                    }
                                    //decodeDataView = new byte[temp];
                                    //System.Buffer.BlockCopy(tempdata, 0, decodeDataView, 0, temp);
                                    #endregion
                                    break;
                                case 6://L2P_PLC_TYPE
                                    #region L2P_PLC_TYPE
                                    str += "+- L2P PLC TYPE\r\n";
                                    count = 0;//PDU个数
                                    Location = 0;
                                    temp = 0;
                                    count = e.data[size_L2P_Protocal_Data];//获取PDU个数
                                    Location = 4 + size_L2P_Protocal_Data;//获取L2P_Protocal_Data + L2P_RLC_DATA_STRU长度
                                    for (int i = 0; i < count; i++)
                                    {
                                        L2P_RLC_PDCP_PDU_STRU stru4 = new L2P_RLC_PDCP_PDU_STRU();
                                        stru_size = Marshal.SizeOf(stru4);
                                        stru_intptr = Marshal.AllocHGlobal(stru_size);
                                        Marshal.Copy(e.data, Location, stru_intptr, stru_size);
                                        Location += stru_size;
                                        stru4 = (L2P_RLC_PDCP_PDU_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_RLC_PDCP_PDU_STRU));
                                        Marshal.FreeHGlobal(stru_intptr);

                                        byte[] data = new byte[4 * stru4.mu32BodyHeadLen];//获取码流长度
                                        System.Buffer.BlockCopy(e.data, Location, data, 0, data.Length);//获取码流 

                                        Location += data.Length;

                                        System.Buffer.BlockCopy(data, 0, tempdata, temp, data.Length);
                                        temp += data.Length;
                                        //decodeDataView = new byte[data.Length];
                                        //decodeDataView = data;//获取MAC CE码流

                                        switch (stru4.mu8RlcPduType)
                                        {
                                            case 0://DATAMODE_TM_PDU
                                                break;
                                            case 1://DATAMODE_UM_PDU
                                                //---------------------------------------------------
                                                L2P_RLC_UMD_PDU_HEADER_STRU umd_stru = new L2P_RLC_UMD_PDU_HEADER_STRU();
                                                stru_intptr = Marshal.AllocHGlobal(136);
                                                //rlcUmdHeaderAnalyze(5,//SN长度，高层配置，现默认设置为5
                                                                    //data,//码流
                                                                    //stru_intptr);
                                                umd_stru = (L2P_RLC_UMD_PDU_HEADER_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_RLC_UMD_PDU_HEADER_STRU));
                                                Marshal.FreeHGlobal(stru_intptr);
                                                str += "    +- UM PDU \r\n"; //WL
                                                str += "    | +- FI ::= " + "0x" + umd_stru.u8FI.ToString("X") + "\r\n";
                                                str += "    | +- E ::= " + "0x" + umd_stru.u8E.ToString("X") + "\r\n";
                                                str += "    | +- SN ::= " + "0x" + umd_stru.u16SN.ToString("X") + "\r\n";
                                                str += "    | +- HeadSize ::= " + "0x" + umd_stru.u32HeadSize.ToString("X") + "\r\n";
                                                str += "    | +- LiNum ::= " + "0x" + umd_stru.u16LiNum.ToString("X") + "\r\n";
                                                for (int j = 0; j < 32; j++)
                                                {
                                                    str += "    | | +- LI[" + (j + 1).ToString() + "] ::= " + "0x" + umd_stru.extensionInfo[j].u16LI.ToString("X") + "\r\n";
                                                }
                                                //---------------------------------------------------
                                                break;
                                            case 2://DATAMODE_AM_PDU

                                                //---------------------------------------------------
                                                // rlcAmdHeaderAnalyze(0,data);
                                                //---------------------------------------------------
                                                L2P_RLC_AMD_PDU_HEADER_STRU amd_stru = new L2P_RLC_AMD_PDU_HEADER_STRU();
                                                stru_size = Marshal.SizeOf(amd_stru);
                                                stru_intptr = Marshal.AllocHGlobal(404);
                                                //rlcAmdHeaderAnalyze((U32)(data.Length),//SN长度，高层配置，现默认设置为5
                                                                    //data,//码流
                                                                    //stru_intptr);
                                                amd_stru = (L2P_RLC_AMD_PDU_HEADER_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_RLC_AMD_PDU_HEADER_STRU));
                                                Marshal.FreeHGlobal(stru_intptr);
                                                str += "    +- AM PDU \r\n";
                                                str += "    | +- DC ::= " + "0x" + amd_stru.u8DC.ToString("X") + "\r\n";
                                                if (amd_stru.u8DC == 1)
                                                {
                                                    str += "    | +- RF ::= " + "0x" + amd_stru.u8RF.ToString("X") + "  /* 0--AMD PDU; 1--AMD PDU segment */" + "\r\n";
                                                    str += "    | +- P ::= " + "0x" + amd_stru.u8P.ToString("X") + "    /* 0--Status report not requested;1--Status report is requested */" + "\r\n";
                                                    str += "    | +- FI ::= " + "0x" + amd_stru.u8FI.ToString("X") + "   /* Framing Info */" + "\r\n";
                                                    str += "    | +- LSF ::= " + "0x" + amd_stru.u8LSF.ToString("X") + "\r\n";
                                                    str += "    | +- E ::= " + "0x" + amd_stru.u8E.ToString("X") + "\r\n";
                                                    str += "    | +- SN ::= " + "0x" + amd_stru.u16SN.ToString("X") + "\r\n";
                                                    str += "    | +- SO ::= " + "0x" + amd_stru.u16SN.ToString("X") + "\r\n";
                                                    str += "    | +- HeadSize ::= " + "0x" + amd_stru.u16HeadSize.ToString("X") + "\r\n";
                                                    str += "    | +- LiNum ::= " + "0x" + amd_stru.u16LiNum.ToString("X") + "\r\n";
                                                    if (amd_stru.u8E == 1)
                                                    {
                                                        for (int n = 0; n < 32; n++)
                                                        {
                                                            str += "    | | +- LI[" + (n + 1).ToString() + "] ::= " + "0x" + amd_stru.dataPduExten[n].u16LI.ToString("X") + "\r\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    str += "    | +- CPT ::= " + "0x" + amd_stru.u8CPT.ToString("X") + "\r\n";
                                                    str += "    | +- ExtenNum ::= " + "0x" + amd_stru.u8ExtenNum.ToString("X") + "\r\n";
                                                    str += "    | +- AckSN ::= " + "0x" + amd_stru.u8ExtenNum.ToString("X") + "\r\n";
                                                    if (amd_stru.u8ExtenNum != 0xff)
                                                    {
                                                        for (int n = 0; n < 32; n++)
                                                        {
                                                            str += "    | +- ************************************\r\n";
                                                            str += "    | +- NackSn[" + (n + 1).ToString() + "] ::= " + "0x" + amd_stru.statusPduExten[n].u16NackSn.ToString("X") + "\r\n";
                                                            str += "    | +- SOstart[" + (n + 1).ToString() + "] ::= " + "0x" + amd_stru.statusPduExten[n].u16SOstart.ToString("X") + "\r\n";
                                                            str += "    | +- SOend[" + (n + 1).ToString() + "] ::= " + "0x" + amd_stru.statusPduExten[n].u16SOend.ToString("X") + "\r\n";
                                                            str += "    | +- E1[" + (n + 1).ToString() + "] ::= " + "0x" + amd_stru.statusPduExten[n].u8E1.ToString("X") + "\r\n";
                                                            str += "    | +- E2[" + (n + 1).ToString() + "] ::= " + "0x" + amd_stru.statusPduExten[n].u8E2.ToString("X") + "\r\n";
                                                        }
                                                    }
                                                }
                                                break;
                                            case 3://DATAMODE_AM_SEGMENT
                                                break;
                                        }
                                    }

                                    //decodeDataView = new byte[temp];
                                    //System.Buffer.BlockCopy(tempdata, 0, decodeDataView, 0, temp);

                                    #endregion
                                    break;
                                case 7://L2P_PDCP_TYPE
                                    #region L2P_PDCP_TYPE
                                    str += "+- L2P PDCP TYPE\r\n";
                                    count = 0;//PDU个数
                                    Location = 0;
                                    temp = 0;

                                    count = e.data[size_L2P_Protocal_Data];//获取PDU个数

                                    Location = 4 + size_L2P_Protocal_Data;//获取L2P_Protocal_Data + L2P_RLC_DATA_STRU长度

                                    for (int i = 0; i < count; i++)
                                    {
                                        L2P_RLC_PDCP_PDU_STRU stru5 = new L2P_RLC_PDCP_PDU_STRU();
                                        stru_size = Marshal.SizeOf(stru5);
                                        stru_intptr = Marshal.AllocHGlobal(stru_size);

                                        //偏移量L2P_Protocal_Data长度 + 4byte(L2P_RLC_DATA_STRU长度) + i * stru_size
                                        Marshal.Copy(e.data, Location, stru_intptr, stru_size);
                                        stru5 = (L2P_RLC_PDCP_PDU_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_RLC_PDCP_PDU_STRU));
                                        Marshal.FreeHGlobal(stru_intptr);
                                        Location += stru_size;//加上L2P_RLC_PDCP_PDU_STRU长度

                                        byte[] data = new byte[4 * stru5.mu32BodyHeadLen];//获取码流长度
                                        System.Buffer.BlockCopy(e.data, Location, data, 0, data.Length);//获取码流 

                                        System.Buffer.BlockCopy(data, 0, tempdata, temp, data.Length);
                                        temp += data.Length;
                                        //decodeDataView = new byte[data.Length];
                                        //decodeDataView = data;//获取MAC CE码流

                                        if (stru5.mu8RbType == 0)//SRB
                                        {
                                            U8 PdcpSn = 0;
                                            //pdcpDataPduForSrbAnalyze(data, ref PdcpSn);//SRB解析
                                            str += "    +- RbType ::= SRB \n";//WL
                                            str += "    +- PdcpSn ::= " + "0x" + PdcpSn.ToString("X") + "\r\n";
                                        }
                                        else if (stru5.mu8RbType == 1)//DRB
                                        {
                                            L2P_PDCP_PDU_DRB_STRU drb_stru = new L2P_PDCP_PDU_DRB_STRU();
                                            //pdcpPduForDrbAnalyze(data,
                                                                 //7,//SN长度，由高层配置
                                                                 //(U32)(data.Length),
                                                                 //ref drb_stru);
                                            str += "    +- RbType::= DRB \n";//WL
                                            str += "    +- DC ::= " + "0x" + drb_stru.u32DC.ToString("X") + "/* 0--Control PDU; 1--Data PDU */" + "\r\n";
                                            str += "    +- PdcpSn ::= " + "0x" + drb_stru.u16PdcpSn.ToString("X") + "\r\n";
                                            str += "    +- Fms ::= " + "0x" + drb_stru.u16Fms.ToString("X") + "\r\n";
                                            str += "    +- PduType ::= " + "0x" + drb_stru.u32PduType.ToString("X") + "\r\n";
                                            for (int n = 0; n < 32; n++)
                                            {
                                                str += "    | +- rohcFeedbackRacket[" + (n + 1).ToString() + "] ::= " + "0x" + drb_stru.rohcFeedbackPacket[n].ToString("X") + "\r\n";
                                                str += "    | +- Bitmap[" + (n + 1).ToString() + "] ::= " + "0x" + drb_stru.Bitmap[n].ToString("X") + "\r\n";
                                            }
                                        }
                                        Location += data.Length;//加上码流长度                                  
                                    }
                                    //decodeDataView = new byte[temp];
                                    //System.Buffer.BlockCopy(tempdata, 0, decodeDataView, 0, temp);
                                    #endregion
                                    break;
                                case 8://L2P_NASORRRC
                                    #region NASORRRC
                                    #region 将数组转换成   结构体   L2P_PROTOCOL_DATA
                                    byte[] RRCData;//需要详细解码的码流
                                    L2P_RRCNAS_MSG_STRU L2PRRCNASMSGSTRU = new L2P_RRCNAS_MSG_STRU();
                                    //得到结构体大小
                                    stru_size = Marshal.SizeOf(L2PRRCNASMSGSTRU);
                                    //分配结构体大小的内存空间
                                    stru_intptr = Marshal.AllocHGlobal(stru_size);
                                    //将byte数组拷贝到内存空间
                                    Marshal.Copy(e.data, size_L2P_Protocal_Data, stru_intptr, stru_size);
                                    //将内存空间转换为目标结构体
                                    L2PRRCNASMSGSTRU = (L2P_RRCNAS_MSG_STRU)Marshal.PtrToStructure(stru_intptr, typeof(L2P_RRCNAS_MSG_STRU));
                                    //释放内存空间
                                    Marshal.FreeHGlobal(stru_intptr);

                                    try
                                    {
                                        //byte[] decodes = new byte[e.data.Length - size - sizeSTRU];
                                        //UInt16 RrcOrNasLength = BitConverter.ToUInt16(e.data, size + 8);
                                        byte[] decodes = new byte[e.data.Length - size_L2P_Protocal_Data - stru_size];
                                        UInt16 RrcOrNasLength = BitConverter.ToUInt16(e.data, size_L2P_Protocal_Data + 8);
                                        if (L2PRRCNASMSGSTRU.mu16PduLength != 0)//ProtocolDataLength 不为0时
                                        {
                                            //RRC Data
                                            if (L2PRRCNASMSGSTRU.mu16RrcOrNas == 0)//（RRC）根据最新版本接口修改RrcOrNas
                                            {
                                                UInt16 msgID = e.data[size_L2P_Protocal_Data + 4];
                                                byte[] sbMsgData = new byte[30000];
                                                byte[] sbMsgName = new byte[20000];
                                                //byte[] RRCData = new byte[decodes.Length];
                                                RRCData = new byte[decodes.Length];
                                                System.Buffer.BlockCopy(e.data, size_L2P_Protocal_Data + stru_size, RRCData, 0, RRCData.Length);

                                                try
                                                {
                                                    // UInt32 xx = DecodeDataStreamWithPosInfo((ushort)msgID, sbMsgData, RRCData, RRCData.Length, sbMsgName);
                                                    //UInt32 xx = DecodeDataStream((ushort)msgID, sbMsgData, RRCData, RRCData.Length, sbMsgName);
                                                    //-----------------------------------------------------------------
                                                    //decodeDataView = new byte[RrcOrNasLength];
                                                    //System.Buffer.BlockCopy(RRCData, 0, decodeDataView, 0, RrcOrNasLength);
                                                    //-------------------------------------------------------------------
                                                }
                                                catch
                                                {

                                                }
                                                //decodeView.richTextBox1.Text += System.Text.Encoding.ASCII.GetString(sbMsgData);
                                                decodeData = RRCData;
                                            }
                                            //NAS Data
                                            else if (L2PRRCNASMSGSTRU.mu16RrcOrNas == 1)//（NAS）根据最新版本接口修改RrcOrNas
                                            {
                                                //返回消息名字，是字符串，可以用于显示，也可以不使用
                                                byte[] pOut = new byte[30000];
                                                byte[] chMsgType = new byte[2000];
                                                System.Buffer.BlockCopy(e.data, size_L2P_Protocal_Data + stru_size, decodes, 0, decodes.Length);

                                                //long xx = dissect_nas_msg(pOut, RrcOrNasLength, decodes, chMsgType);
                                                //long xx = dissect_nas_msg(pOut, decodes.Length, decodes, chMsgType);
                                                //-----------------------------------------------------------------
                                                //decodeDataView = new byte[RrcOrNasLength];
                                                //System.Buffer.BlockCopy(decodes, 0, decodeDataView, 0, RrcOrNasLength);
                                                //-------------------------------------------------------------------

                                                string stemp = System.Text.Encoding.ASCII.GetString(pOut);
                                                //decodeView.richTextBox1.Text += stemp.Replace("\n", "\r\n");
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }

                                    //decodeData = e.data;
                                    //decodeText = decodeView.richTextBox1.Text;

                                    return null;

                                //if (e.data[26] == 10)
                                //if (e.data[40 + 8] == 0)//（RRC）根据最新版本接口修改RrcOrNas
                                //{
                                //    byte[] sbMsgData = new byte[30000];
                                //    byte[] sbMsgName = new byte[2000];
                                //    ////[StructLayout(LayoutKind.Sequential)]
                                //    byte[] data = new byte[e.data.Length - (45 + 8)];//ProtocolData的长度
                                //    System.Buffer.BlockCopy(e.data, 45 + 8, data, 0, e.data.Length - (45 + 8));//从ProtocolData中第二个byte进行拷贝。
                                //    UInt16 dataLength = BitConverter.ToUInt16(e.data, 42 + 8);

                                //    UInt32 xx = DecodeDataStreamWithPosInfo((ushort)e.data[44 + 8], sbMsgData, data, dataLength - 1, sbMsgName);

                                //    //-----------------------------------------------------------------
                                //    decodeDataView = new byte[data.Length];
                                //    System.Buffer.BlockCopy(data, 0, decodeDataView, 0, data.Length);
                                //    //-------------------------------------------------------------------

                                //    decodeView.richTextBox1.Text += System.Text.Encoding.ASCII.GetString(sbMsgData);
                                //}
                                ////else if (e.data[26] == 11)
                                //else if (e.data[40 + 8] == 1)//（NAS）根据最新版本接口修改RrcOrNas
                                //{
                                //    byte[] pOut = new byte[30000];
                                //    byte[] chMsgType = new byte[2000];
                                //    byte[] data = new byte[e.data.Length - (44 + 8)];
                                //    System.Buffer.BlockCopy(e.data, 44 + 8, data, 0, e.data.Length - (44 + 8));

                                //    long xx = dissect_nas_msg(pOut, data.Length, data, chMsgType);

                                //    //-----------------------------------------------------------------
                                //    decodeDataView = new byte[data.Length];
                                //    System.Buffer.BlockCopy(data, 0, decodeDataView, 0, data.Length);
                                //    //-------------------------------------------------------------------

                                //    string stemp = System.Text.Encoding.ASCII.GetString(pOut);
                                //    decodeView.richTextBox1.Text += stemp.Replace("\n", "\r\n");
                                //}
                                    #endregion
                                    #endregion
                                // break;
                            }
                            #endregion
                            //decodeView.richTextBox1.Text += str;
                            //decodeText = decodeView.richTextBox1.Text;
                            #endregion
                        }
                        break;
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L1_AG_PROTOCOL_DATA_MSG_TYPE:
                        {
                            #region 将数据转换成结构体 L1_AG_PROTOCOL_DATA
                            L1_PROTOCOL_DATA stu_L1_PROTOCOL_DATA = new L1_PROTOCOL_DATA();
                            int size_L1_PROTOCOL_DATA = Marshal.SizeOf(stu_L1_PROTOCOL_DATA);
                            if (size_L1_PROTOCOL_DATA > e.data.Length)
                            {
                                return null;
                            }
                            IntPtr intptr_L1_PROTOCOL_DATA = Marshal.AllocHGlobal(size_L1_PROTOCOL_DATA);
                            Marshal.Copy(e.data, 0, intptr_L1_PROTOCOL_DATA, size_L1_PROTOCOL_DATA);
                            stu_L1_PROTOCOL_DATA = (L1_PROTOCOL_DATA)Marshal.PtrToStructure(intptr_L1_PROTOCOL_DATA, typeof(L1_PROTOCOL_DATA));
                            Marshal.FreeHGlobal(intptr_L1_PROTOCOL_DATA);
                            #endregion

                            #region 将数组转换成L1_AG_PROTOCOL_DATA结构体数组
                            byte[] data_L1_PROTOCOL_DATA = new byte[e.data.Length - size_L1_PROTOCOL_DATA];
                            System.Buffer.BlockCopy(e.data, size_L1_PROTOCOL_DATA, data_L1_PROTOCOL_DATA, 0, e.data.Length - size_L1_PROTOCOL_DATA);
                            decodeData = data_L1_PROTOCOL_DATA;
                            #endregion

                            string str = null;
                            string UEIDData = "";
                            U8 mu8Direction = stu_L1_PROTOCOL_DATA.mstL1ProtocolDataHeader.mu8Direction;
                            U8 mu8UeNum = stu_L1_PROTOCOL_DATA.mu8UeNum;
                            if (mu8Direction == 0)//UpLink
                            {
                                #region Uplink

                                L1_UL_UE_MEAS stu_L1_UL_UE_MEAS = new L1_UL_UE_MEAS();
                                int size_L1_UL_UE_MEAS = Marshal.SizeOf(stu_L1_UL_UE_MEAS);
                                IntPtr intptr_L1_UL_UE_MEAS = Marshal.AllocHGlobal(size_L1_UL_UE_MEAS);
                                if (size_L1_UL_UE_MEAS > data_L1_PROTOCOL_DATA.Length)
                                {
                                    return null;
                                }

                                for (int i = 0; i < mu8UeNum; i++)
                                {
                                    Marshal.Copy(data_L1_PROTOCOL_DATA, 100 + i * size_L1_UL_UE_MEAS, intptr_L1_UL_UE_MEAS, size_L1_UL_UE_MEAS);
                                    stu_L1_UL_UE_MEAS = (L1_UL_UE_MEAS)Marshal.PtrToStructure(intptr_L1_UL_UE_MEAS, typeof(L1_UL_UE_MEAS));

                                    UEIDData += "+- PhyChannel:: " + i.ToString() + "\r\n";
                                    UEIDData += "  +- ChannelType ::= ".PadRight(5, ' ');

                                    if (stu_L1_UL_UE_MEAS.muChType == 7) //PUCCH  enum ChTypeOptions{ PDCCH = 0, PDSCH, PBCH, PCFICH, PHICH, PUSCH, PRACH, PUCCH, MEASUREMENT}
                                    {
                                        UEIDData += "PUCCH";

                                        switch (stu_L1_UL_UE_MEAS.mu8PucchFormat)
                                        {
                                            case 0:
                                                UEIDData += "1";
                                                break;
                                            case 1:
                                                UEIDData += "1a";
                                                break;
                                            case 2:
                                                UEIDData += "1b";
                                                break;
                                            case 3:
                                                UEIDData += "2";
                                                break;
                                            case 4:
                                                UEIDData += "2a";
                                                break;
                                            case 5:
                                                UEIDData += "2b";
                                                break;
                                            default:
                                                UEIDData += "xx";
                                                break;
                                        }

                                        UEIDData += "\r\n";

                                    }
                                    else if (stu_L1_UL_UE_MEAS.muChType == 5) //PUSCH
                                    {
                                        UEIDData += "PUSCH\r\n";
                                    }
                                    else
                                    {
                                        UEIDData += "error\r\n";
                                    }

                                    UEIDData += "  +- RNTI ::= ".PadRight(5, ' ');
                                    UEIDData += stu_L1_UL_UE_MEAS.mu16UeIndValue.ToString() + "(0x" + stu_L1_UL_UE_MEAS.mu16UeIndValue.ToString("X") + ")\r\n";
                                    UEIDData += "  +- Ta ::= ".PadRight(5, ' ');
                                    UEIDData += stu_L1_UL_UE_MEAS.mu16Ta.ToString() + " (16Ts)\r\n";
                                    UEIDData += "  +- Power ::= ".PadRight(5, ' ');
                                    UEIDData += (stu_L1_UL_UE_MEAS.ms16Power * 0.125).ToString("f2") + "dBm\r\n";
                                    UEIDData += "  +- SINR ::= ".PadRight(5, ' ');
                                    UEIDData += (stu_L1_UL_UE_MEAS.ms8Sinr * 0.5).ToString("f1") + "dB\r\n";

                                }

                                Marshal.FreeHGlobal(intptr_L1_UL_UE_MEAS);

                                //SINR per RB
                                //rbSinr = new SByte[100];
                                //System.Buffer.BlockCopy(data_L1_PROTOCOL_DATA, 0, rbSinr, 0, 100);
                                S8 sinr;
                                UEIDData += "+- SINR per RB:: \r\n";
                                for (int i = 0; i < 100; i++)
                                {
                                    sinr = (S8)data_L1_PROTOCOL_DATA[i];

                                    if (-128 == sinr)
                                    {
                                        UEIDData += "  +- RB[" + i.ToString() + "] ::= --- \r\n";
                                    }
                                    else
                                    {
                                        UEIDData += "  +- RB[" + i.ToString() + "] ::= " + (sinr * 0.5).ToString("f1") + "dB\r\n";
                                    }
                                }

                                #endregion
                            }
                            else if (mu8Direction == 1)//DownLink
                            {
                                #region Downlink
                                int dstOffset = 0;
                                L1_DL_UE_MEAS stu_L1_DL_UE_MEAS = new L1_DL_UE_MEAS();
                                int size_L1_DL_UE_MEAS = Marshal.SizeOf(stu_L1_DL_UE_MEAS);
                                IntPtr intptr_L1_DL_UE_MEAS = Marshal.AllocHGlobal(size_L1_DL_UE_MEAS);
                                if (size_L1_DL_UE_MEAS > data_L1_PROTOCOL_DATA.Length)
                                {
                                    return null;
                                }
                                Marshal.Copy(data_L1_PROTOCOL_DATA, dstOffset, intptr_L1_DL_UE_MEAS, size_L1_DL_UE_MEAS);
                                dstOffset = dstOffset + size_L1_DL_UE_MEAS;
                                stu_L1_DL_UE_MEAS = (L1_DL_UE_MEAS)Marshal.PtrToStructure(intptr_L1_DL_UE_MEAS, typeof(L1_DL_UE_MEAS));
                                Marshal.FreeHGlobal(intptr_L1_DL_UE_MEAS);

                                U8 mu8MeasSelect = stu_L1_DL_UE_MEAS.mu8MeasSelect;
                                if ((mu8MeasSelect & 0x1) == 1)//UE_RSRPQI_INFO
                                {
                                    UE_RSRPQI_INFO str_UE_RSRPQI_INFO = new UE_RSRPQI_INFO();
                                    int size_UE_RSRPQI_INFO = Marshal.SizeOf(str_UE_RSRPQI_INFO);
                                    IntPtr intptr_UE_RSRPQI_INFO = Marshal.AllocHGlobal(size_UE_RSRPQI_INFO);
                                    if (size_UE_RSRPQI_INFO > data_L1_PROTOCOL_DATA.Length - dstOffset)
                                    {
                                        return null;
                                    }
                                    Marshal.Copy(data_L1_PROTOCOL_DATA, dstOffset, intptr_UE_RSRPQI_INFO, size_UE_RSRPQI_INFO);
                                    dstOffset = dstOffset + size_UE_RSRPQI_INFO;
                                    str_UE_RSRPQI_INFO = (UE_RSRPQI_INFO)Marshal.PtrToStructure(intptr_UE_RSRPQI_INFO, typeof(UE_RSRPQI_INFO));
                                    Marshal.FreeHGlobal(intptr_UE_RSRPQI_INFO);


                                    UEIDData += "+- UE RSRPQI INFO\r\n";
                                    UEIDData += "  +- Ue RSSI ::= ".PadRight(5, ' ');
                                    UEIDData += (str_UE_RSRPQI_INFO.ms16DrsRssi * 0.125).ToString("f2") + "dBm\r\n";
                                    UEIDData += "  +- Ue RSRP ::= ".PadRight(5, ' ');
                                    UEIDData += (str_UE_RSRPQI_INFO.ms16UeRsrp * 0.125).ToString("f2") + "dBm\r\n";
                                    UEIDData += "  +- Ue RSSQ ::= ".PadRight(5, ' ');
                                    UEIDData += (str_UE_RSRPQI_INFO.ms16DrsRssq * 0.0625).ToString("f2") + "dB\r\n";
                                    UEIDData += "  +- DRS RSSI ::= ".PadRight(5, ' ');
                                    UEIDData += (str_UE_RSRPQI_INFO.ms16DrsRssi * 0.125).ToString("f2") + "dBm\r\n";
                                    UEIDData += "  +- DRS RSSP ::= ".PadRight(5, ' ');
                                    UEIDData += (str_UE_RSRPQI_INFO.ms16DrsRssp * 0.125).ToString("f2") + "dBm\r\n";
                                    UEIDData += "  +- DRS RSSQ ::= ".PadRight(5, ' ');
                                    UEIDData += (str_UE_RSRPQI_INFO.ms16DrsRssq * 0.0625).ToString("f2") + "dB\r\n";

                                }
                                if ((mu8MeasSelect & 0x2) == 2)//UE_SINR_INFO
                                {
                                    UE_SINR_INFO str_UE_SINR_INFO = new UE_SINR_INFO();
                                    int size_UE_SINR_INFO = Marshal.SizeOf(str_UE_SINR_INFO);
                                    IntPtr intptr_UE_SINR_INFO = Marshal.AllocHGlobal(size_UE_SINR_INFO);
                                    if (size_UE_SINR_INFO > data_L1_PROTOCOL_DATA.Length - dstOffset)
                                    {
                                        return null;
                                    }
                                    Marshal.Copy(data_L1_PROTOCOL_DATA, dstOffset, intptr_UE_SINR_INFO, size_UE_SINR_INFO);
                                    dstOffset = dstOffset + size_UE_SINR_INFO;
                                    str_UE_SINR_INFO = (UE_SINR_INFO)Marshal.PtrToStructure(intptr_UE_SINR_INFO, typeof(UE_SINR_INFO));
                                    Marshal.FreeHGlobal(intptr_UE_SINR_INFO);

                                    UEIDData += "+- UE SINR INFO\r\n";
                                    UEIDData += "  +- UeSinr\r\n";
                                    int UeSinrCount = 1;
                                    foreach (var item in str_UE_SINR_INFO.mas16UeSinr)
                                    {
                                        UEIDData += "  | +- Sinr[ " + UeSinrCount.ToString() + " ] ::= ".PadRight(5, ' ');
                                        UEIDData += item.ToString() + "\r\n";
                                        UeSinrCount++;
                                    }

                                    UEIDData += "  +- DrsSinr ::= ".PadRight(5, ' ');
                                    UEIDData += str_UE_SINR_INFO.ms16DrsSinr.ToString() + "\r\n";
                                }
                                if ((mu8MeasSelect & 0x8) == 8)//UE_POWER_INFO
                                {
                                    UE_POWER_INFO stu_UE_POWER_INFO = new UE_POWER_INFO();
                                    int size_UE_POWER_INFO = Marshal.SizeOf(stu_UE_POWER_INFO);
                                    IntPtr intptr_UE_POWER_INFO = Marshal.AllocHGlobal(size_UE_POWER_INFO);
                                    if (size_UE_POWER_INFO > data_L1_PROTOCOL_DATA.Length - dstOffset)
                                    {
                                        return null;
                                    }
                                    Marshal.Copy(data_L1_PROTOCOL_DATA, dstOffset, intptr_UE_POWER_INFO, size_UE_POWER_INFO);
                                    dstOffset = dstOffset + size_UE_POWER_INFO;
                                    stu_UE_POWER_INFO = (UE_POWER_INFO)Marshal.PtrToStructure(intptr_UE_POWER_INFO, typeof(UE_POWER_INFO));
                                    Marshal.FreeHGlobal(intptr_UE_POWER_INFO);

                                    UEIDData += "+- UE　POWER　INFO\r\n";
                                    UEIDData += "  +- PdschPower_a ::= ".PadRight(5, ' ');
                                    UEIDData += stu_UE_POWER_INFO.ms16PdschPower_a.ToString() + "DB\r\n";
                                    UEIDData += "  +- PdschPower_ｂ ::= ".PadRight(5, ' ');
                                    UEIDData += stu_UE_POWER_INFO.ms16PdschPower_b.ToString() + "DB\r\n";
                                }
                                #endregion
                            }
                            str += UEIDData + "\r\n";

                            //decodeView.richTextBox1.Text += str;
                            //decodeText = decodeView.richTextBox1.Text;
                        }
                        break;
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L1_PHY_COMMEAS_IND_MSG_TYPE:
                        {
                            #region 将数据转换成结构体 L1_PHY_COMMEAS_IND
                            L1_PHY_COMMEAS_IND stu_L1_PHY_COMMEAS_IND = new L1_PHY_COMMEAS_IND();
                            //得到结构体的大小
                            int size_L1_PHY_COMMEAS_IND = Marshal.SizeOf(stu_L1_PHY_COMMEAS_IND);
                            //比较结构体大小，看数据长度是否正确
                            if (size_L1_PHY_COMMEAS_IND > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr intptr_L1_PHY_COMMEAS_IND = Marshal.AllocHGlobal(size_L1_PHY_COMMEAS_IND);
                            //讲byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, intptr_L1_PHY_COMMEAS_IND, size_L1_PHY_COMMEAS_IND);
                            //讲内存空间转换为目标结构体
                            stu_L1_PHY_COMMEAS_IND = (L1_PHY_COMMEAS_IND)Marshal.PtrToStructure(intptr_L1_PHY_COMMEAS_IND, typeof(L1_PHY_COMMEAS_IND));
                            //释放内存空间
                            Marshal.FreeHGlobal(intptr_L1_PHY_COMMEAS_IND);
                            #endregion

                            #region 将数组转换成COMMEAS_IND结构体数组
                            byte[] data_L1_PHY_COMMEAS_IND = new byte[e.data.Length - size_L1_PHY_COMMEAS_IND];
                            System.Buffer.BlockCopy(e.data, size_L1_PHY_COMMEAS_IND, data_L1_PHY_COMMEAS_IND, 0, e.data.Length - size_L1_PHY_COMMEAS_IND);
                            decodeData = data_L1_PHY_COMMEAS_IND;
                            #endregion

                            string str = null;
                            string UEIDType = "";
                            string UEIDData = "";
                            int dstOffset = 0;
                            UInt32 mu32MeasSelect = stu_L1_PHY_COMMEAS_IND.mstL1PHYComentIndHeader.mu32MeasSelect;
                            #region  按测量类型解析
                            if ((mu32MeasSelect & 0x1001) == 0x1001)
                            {
                                PSSSSS_RSRPQI_INFO stu_PSSSSS_RSRPQI_INFO = new PSSSSS_RSRPQI_INFO();
                                int size_PSSSSS_RSRPQI_INFO = Marshal.SizeOf(stu_PSSSSS_RSRPQI_INFO);
                                IntPtr intptr_PSSSSS_RSRPQI_INFO = Marshal.AllocHGlobal(size_PSSSSS_RSRPQI_INFO);
                                Marshal.Copy(data_L1_PHY_COMMEAS_IND, dstOffset, intptr_PSSSSS_RSRPQI_INFO, size_PSSSSS_RSRPQI_INFO);
                                dstOffset = dstOffset + size_PSSSSS_RSRPQI_INFO;
                                stu_PSSSSS_RSRPQI_INFO = (PSSSSS_RSRPQI_INFO)Marshal.PtrToStructure(intptr_PSSSSS_RSRPQI_INFO, typeof(PSSSSS_RSRPQI_INFO));
                                Marshal.FreeHGlobal(intptr_PSSSSS_RSRPQI_INFO);

                                //UEIDType += "PssRsrpqiInfo, ";
                                UEIDData += "+- PssRsrpqiInfo\r\n";
                                UEIDData += "  +- RSSI ::= ".PadRight(5, ' ');
                                UEIDData += (stu_PSSSSS_RSRPQI_INFO.mstPssRsrpqiInfo.ms16PSS_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- RSRP ::= ".PadRight(5, ' ');
                                UEIDData += (stu_PSSSSS_RSRPQI_INFO.mstPssRsrpqiInfo.ms16PSS_RP * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- RSRQ:".PadRight(5, ' ');
                                UEIDData += (stu_PSSSSS_RSRPQI_INFO.mstPssRsrpqiInfo.ms16PSS_RQ * 0.0625).ToString("f2") + "dB\r\n";

                                //UEIDType += "SssRsrpqiInfo, ";
                                UEIDData += "+- SssRsrpqiInfo\r\n";
                                UEIDData += "  +- RSSI ::= ".PadRight(5, ' ');
                                UEIDData += (stu_PSSSSS_RSRPQI_INFO.mstSssRsrpqiInfo.ms16SSS_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- RSRP ::= ".PadRight(5, ' ');
                                UEIDData += (stu_PSSSSS_RSRPQI_INFO.mstSssRsrpqiInfo.ms16SSS_RP * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- RSRQ ::= ".PadRight(5, ' ');
                                UEIDData += (stu_PSSSSS_RSRPQI_INFO.mstSssRsrpqiInfo.ms16SSS_RQ * 0.0625).ToString("f2") + "dB\r\n";
                            }
                            if ((mu32MeasSelect & 0x2001) == 0x2001)
                            {
                                CRS_RSRPQI_INFO stu_CRS_RSRPQI_INFO = new CRS_RSRPQI_INFO();
                                int size_CRS_RSRPQI_INFO = Marshal.SizeOf(stu_CRS_RSRPQI_INFO);
                                IntPtr intptr_CRS_RSRPQI_INFO = Marshal.AllocHGlobal(size_CRS_RSRPQI_INFO);
                                Marshal.Copy(data_L1_PHY_COMMEAS_IND, dstOffset, intptr_CRS_RSRPQI_INFO, size_CRS_RSRPQI_INFO);
                                dstOffset = dstOffset + size_CRS_RSRPQI_INFO;
                                stu_CRS_RSRPQI_INFO = (CRS_RSRPQI_INFO)Marshal.PtrToStructure(intptr_CRS_RSRPQI_INFO, typeof(CRS_RSRPQI_INFO));
                                Marshal.FreeHGlobal(intptr_CRS_RSRPQI_INFO);

                                // UEIDType += "CRS0, ";
                                UEIDData += "+- CRS0\r\n";
                                UEIDData += "  +- RSSI ::= ".PadRight(5, ' ');
                                UEIDData += (stu_CRS_RSRPQI_INFO.mstCrs0RsrpqiInfo.ms16CRS_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- RSRP ::= ".PadRight(5, ' ');
                                UEIDData += (stu_CRS_RSRPQI_INFO.mstCrs0RsrpqiInfo.ms16CRS_RP * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- RSRQ ::= ".PadRight(5, ' ');
                                UEIDData += (stu_CRS_RSRPQI_INFO.mstCrs0RsrpqiInfo.ms16CRS_RQ * 0.0625).ToString("f2") + "dB\r\n";


                                //UEIDType += "CRS1, ";
                                UEIDData += "+- CRS1\r\n";
                                UEIDData += "  +- RSSI ::= ".PadRight(5, ' ');
                                UEIDData += (stu_CRS_RSRPQI_INFO.mstCrs1RsrpqiInfo.ms16CRS_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- RSRP ::= ".PadRight(5, ' ');
                                UEIDData += (stu_CRS_RSRPQI_INFO.mstCrs1RsrpqiInfo.ms16CRS_RP * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- RSRQ ::= ".PadRight(5, ' ');
                                UEIDData += (stu_CRS_RSRPQI_INFO.mstCrs1RsrpqiInfo.ms16CRS_RQ * 0.0625).ToString("f2") + "dB\r\n";
                            }
                            if ((mu32MeasSelect & 0x101) == 0x101)
                            {
                                PBCH_RSRPQI_INFO stu_PBCH_RSRPQI_INFO = new PBCH_RSRPQI_INFO();
                                int size_PBCH_RSRPQI_INFO = Marshal.SizeOf(stu_PBCH_RSRPQI_INFO);
                                IntPtr intptr_PBCH_RSRPQI_INFO = Marshal.AllocHGlobal(size_PBCH_RSRPQI_INFO);
                                Marshal.Copy(data_L1_PHY_COMMEAS_IND, dstOffset, intptr_PBCH_RSRPQI_INFO, size_PBCH_RSRPQI_INFO);
                                dstOffset = dstOffset + size_PBCH_RSRPQI_INFO;
                                stu_PBCH_RSRPQI_INFO = (PBCH_RSRPQI_INFO)Marshal.PtrToStructure(intptr_PBCH_RSRPQI_INFO, typeof(PBCH_RSRPQI_INFO));
                                Marshal.FreeHGlobal(intptr_PBCH_RSRPQI_INFO);

                                UEIDData += "+- PBCH RSRPQI\r\n";
                                UEIDData += "  +- Cell specific RS RSSI ::= ".PadRight(5, ' ');
                                UEIDData += (stu_PBCH_RSRPQI_INFO.ms16_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- Cell specific RS RP ::= ".PadRight(5, ' ');
                                UEIDData += (stu_PBCH_RSRPQI_INFO.ms16_RSRP * 0.125).ToString("f2") + "dBm\r\n";
                                UEIDData += "  +- Cell specific RS RQ ::= ".PadRight(5, ' ');
                                UEIDData += (stu_PBCH_RSRPQI_INFO.ms16_RRSRQ * 0.0625).ToString("f2") + "dB\r\n";
                            }
                            #endregion
                            if (UEIDType.Length > 2)
                            {
                                UEIDType = UEIDType.Remove(UEIDType.Length - 2);
                            }

                            // str += UEIDType + "\r\n";
                            str += UEIDData + "\r\n";

                           ///decodeView.richTextBox1.Text += str;
                            //decodeText = decodeView.richTextBox1.Text;
                        }
                        break;
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.AG_PC_SPECIFIED_CELL_SCAN_FINISH_IND_MSG_TYPE:
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.AG_PC_UNSPECIFIED_CELL_SCAN_FINISH_IND_MSG_TYPE:
                        return null;
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L1_AG_SPECIFIED_CELL_SCAN_DATA_MSG_TYPE:
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L1_UNSPECIFIED_CELL_SCAN_DATA_MSG_TYPE:
                        {

                            #region 将数据转换成结构体 L1_UNSPECIFIED_CELL_SCAN_DATA_MSG_TYPE
                            L1_CELL_SCAN_DATA stru_L1_CELL_SCAN_DATA = new L1_CELL_SCAN_DATA();
                            int size_L1_CELL_SCAN_DATA = Marshal.SizeOf(stru_L1_CELL_SCAN_DATA);
                            //比较结构体大小，看数据长度是否正确
                            if (size_L1_CELL_SCAN_DATA > e.data.Length)
                            {
                                return null;
                            }
                            //分配结构体大小的内存空间
                            IntPtr intptr_L1_CELL_SCAN_DATA = Marshal.AllocHGlobal(size_L1_CELL_SCAN_DATA);
                            //讲byte数组拷贝到内存空间
                            Marshal.Copy(e.data, 0, intptr_L1_CELL_SCAN_DATA, size_L1_CELL_SCAN_DATA);
                            //讲内存空间转换为目标结构体
                            stru_L1_CELL_SCAN_DATA = (L1_CELL_SCAN_DATA)Marshal.PtrToStructure(intptr_L1_CELL_SCAN_DATA, typeof(L1_CELL_SCAN_DATA));
                            //释放内存空间
                            Marshal.FreeHGlobal(intptr_L1_CELL_SCAN_DATA);
                            #endregion
                            //decodeDataView = e.data;

                            #region 解析结构体 L1_CELL_SCAN_DATA
                            string str = "";
                            string strhead = "";
                            //===========================================
                            strhead += "+- L1 CELL SCAN DATA\r\n";
                            strhead += "  +- EARFCN ::= ".PadRight(5, ' ');
                            strhead += stru_L1_CELL_SCAN_DATA.EARFCN.ToString() + "\r\n";
                            strhead += "  +- PCI ::= ".PadRight(5, ' ');
                            strhead += stru_L1_CELL_SCAN_DATA.PCI.ToString() + "\r\n";
                            strhead += "  +- Syn Status ::= ".PadRight(5, ' ');
                            strhead += stru_L1_CELL_SCAN_DATA.SynStatus.ToString() + "\r\n";
                            if (stru_L1_CELL_SCAN_DATA.SynStatus == 0)
                            {
                                strhead += "  +- Antenna Port Number ::= ".PadRight(5, ' ');
                                strhead += stru_L1_CELL_SCAN_DATA.AntennaPortNumber.ToString() + "\r\n";
                                strhead += "  +- CP Type ::= ".PadRight(5, ' ');
                                if (stru_L1_CELL_SCAN_DATA.CP_Type == 0)
                                {
                                    strhead += "NORMAL\r\n";
                                }
                                else
                                {
                                    strhead += "EXTEND\r\n";
                                }
                                strhead += "  +- GPS Valid Flag ::= ".PadRight(5, ' ');
                                if (stru_L1_CELL_SCAN_DATA.gpsValidFlag == 0)
                                {
                                    strhead += "INVALID\r\n";
                                }
                                else
                                {
                                    strhead += "VALID\r\n";
                                    strhead += "  +- Timing offset ::= ".PadRight(5, ' ');
                                    strhead += stru_L1_CELL_SCAN_DATA.Timing_offset.ToString() + "Ts\r\n";
                                }
                                strhead += "  +- MeasureMask ::= ".PadRight(5, ' ');
                                //str += stru_L1_CELL_SCAN_DATA.MeasureMask.ToString() + "\r\n";

                                //MeausreMask
                                List<string> Lmytypes = new List<string>();
                                string mytypes = "";
                                Dictionary<UInt16, string> Dmsgtype = new Dictionary<UInt16, string>();
                                Dmsgtype.Add(0x0001, "PSS");
                                Dmsgtype.Add(0x0002, "SSS");
                                Dmsgtype.Add(0x0004, "CRS");
                                Dmsgtype.Add(0x0008, "PBCH");
                                Dmsgtype.Add(0x0010, "CRS_SINR");
                                Dmsgtype.Add(0x0020, "OFDM_SYMBOL_POWER");
                                Dmsgtype.Add(0x0040, "PBCH_EVM");
                                Dmsgtype.Add(0x0080, "PBCH_BLER");
                                Dmsgtype.Add(0x0100, "SUBFRAME_RSSI");
                                Dmsgtype.Add(0x0200, "SLOT_RSSI");
                                Dmsgtype.Add(0x0400, "FRAME_RSSI");
                                Dmsgtype.Add(0x0800, "RB_RSSI");
                                int Moffset = size_L1_CELL_SCAN_DATA;
                                foreach (var item in Dmsgtype)
                                {
                                    if ((stru_L1_CELL_SCAN_DATA.MeasureMask & item.Key) == item.Key)
                                    {
                                        mytypes = item.Value;
                                        Lmytypes.Add(item.Value);
                                        switch (mytypes)
                                        {
                                            case "PSS":
                                                #region PSS_MEASUREMENT
                                                PSS_MEASUREMENT stru_PSS_MEASUREMENT = new PSS_MEASUREMENT();
                                                int size_PSS_MEASUREMENT = Marshal.SizeOf(stru_PSS_MEASUREMENT);
                                                //比较结构体大小，看数据长度是否正确
                                                if (size_PSS_MEASUREMENT > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                //分配结构体大小的内存空间
                                                IntPtr intptr_PSS_MEASUREMENT = Marshal.AllocHGlobal(size_PSS_MEASUREMENT);
                                                //讲byte数组拷贝到内存空间
                                                Marshal.Copy(e.data, Moffset, intptr_PSS_MEASUREMENT, size_PSS_MEASUREMENT);
                                                Moffset = Moffset + size_PSS_MEASUREMENT;
                                                //讲内存空间转换为目标结构体
                                                stru_PSS_MEASUREMENT = (PSS_MEASUREMENT)Marshal.PtrToStructure(intptr_PSS_MEASUREMENT, typeof(PSS_MEASUREMENT));
                                                //释放内存空间
                                                Marshal.FreeHGlobal(intptr_PSS_MEASUREMENT);
                                                str += "  +- PSS_MEASUREMENT\r\n";
                                                str += "     +- RSSI ::= ".PadRight(5, ' ');
                                                str += (stru_PSS_MEASUREMENT.ms16PSS_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                                str += "     +- RSRP ::= ".PadRight(5, ' ');
                                                str += (stru_PSS_MEASUREMENT.ms16PSS_RP * 0.125).ToString("f2") + "dBm\r\n";
                                                str += "     +- RSRQ ::= ".PadRight(5, ' ');
                                                str += (stru_PSS_MEASUREMENT.ms16PSS_RQ * 0.0625).ToString("f2") + "dB\r\n";
                                                #endregion
                                                break;
                                            case "SSS":
                                                #region SSS_MEASUREMENT
                                                SSS_MEASUREMENT stru_SSS_MEASUREMENT = new SSS_MEASUREMENT();
                                                int size_SSS_MEASUREMENT = Marshal.SizeOf(stru_SSS_MEASUREMENT);
                                                //比较结构体大小，看数据长度是否正确
                                                if (size_SSS_MEASUREMENT > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                //分配结构体大小的内存空间
                                                IntPtr intptr_SSS_MEASUREMENT = Marshal.AllocHGlobal(size_SSS_MEASUREMENT);
                                                //讲byte数组拷贝到内存空间
                                                Marshal.Copy(e.data, Moffset, intptr_SSS_MEASUREMENT, size_SSS_MEASUREMENT);
                                                Moffset = Moffset + size_SSS_MEASUREMENT;
                                                //讲内存空间转换为目标结构体
                                                stru_SSS_MEASUREMENT = (SSS_MEASUREMENT)Marshal.PtrToStructure(intptr_SSS_MEASUREMENT, typeof(SSS_MEASUREMENT));
                                                //释放内存空间
                                                Marshal.FreeHGlobal(intptr_SSS_MEASUREMENT);
                                                str += "  +- SSS_MEASUREMENT\r\n";
                                                str += "     +- RSSI ::= ".PadRight(5, ' ');
                                                str += (stru_SSS_MEASUREMENT.ms16SSS_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                                str += "     +- RSRP ::= ".PadRight(5, ' ');
                                                str += (stru_SSS_MEASUREMENT.ms16SSS_RP * 0.125).ToString("f2") + "dBm\r\n";
                                                str += "     +- RSRQ ::= ".PadRight(5, ' ');
                                                str += (stru_SSS_MEASUREMENT.ms16SSS_RQ * 0.0625).ToString("f2") + "dB\r\n";
                                                #endregion
                                                break;
                                            case "CRS":
                                                #region CRS_MEASUREMENT
                                                CRS_MEASUREMENT stru_CRS_MEASUREMENT = new CRS_MEASUREMENT();
                                                int size_CRS_MEASUREMENT = Marshal.SizeOf(stru_CRS_MEASUREMENT);
                                                if (size_CRS_MEASUREMENT > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_CRS_MEASUREMENT = Marshal.AllocHGlobal(size_CRS_MEASUREMENT);
                                                Marshal.Copy(e.data, Moffset, intptr_CRS_MEASUREMENT, size_CRS_MEASUREMENT);
                                                Moffset = Moffset + size_CRS_MEASUREMENT;
                                                stru_CRS_MEASUREMENT = (CRS_MEASUREMENT)Marshal.PtrToStructure(intptr_CRS_MEASUREMENT, typeof(CRS_MEASUREMENT));
                                                Marshal.FreeHGlobal(intptr_CRS_MEASUREMENT);
                                                str += "  +- CRS_MEASUREMENT\r\n";
                                                str += "     +- RSSI ::= ".PadRight(5, ' ');
                                                str += (stru_CRS_MEASUREMENT.ms16CRS_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                                str += "     +- RSRP ::= ".PadRight(5, ' ');
                                                str += (stru_CRS_MEASUREMENT.ms16CRS_RP * 0.125).ToString("f2") + "dBm\r\n";
                                                str += "     +- RSRQ ::= ".PadRight(5, ' ');
                                                str += (stru_CRS_MEASUREMENT.ms16CRS_RQ * 0.0625).ToString("f2") + "dB\r\n";
                                                #endregion
                                                break;
                                            case "PBCH":
                                                #region PBCH_MEASUREMENT
                                                PBCH_MEASUREMENT stru_PBCH_MEASUREMENT = new PBCH_MEASUREMENT();
                                                int size_PBCH_MEASUREMENT = Marshal.SizeOf(stru_PBCH_MEASUREMENT);
                                                if (size_PBCH_MEASUREMENT > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_PBCH_MEASUREMENT = Marshal.AllocHGlobal(size_PBCH_MEASUREMENT);
                                                Marshal.Copy(e.data, Moffset, intptr_PBCH_MEASUREMENT, size_PBCH_MEASUREMENT);
                                                Moffset = Moffset + size_PBCH_MEASUREMENT;
                                                stru_PBCH_MEASUREMENT = (PBCH_MEASUREMENT)Marshal.PtrToStructure(intptr_PBCH_MEASUREMENT, typeof(PBCH_MEASUREMENT));
                                                Marshal.FreeHGlobal(intptr_PBCH_MEASUREMENT);
                                                str += "  +- PBCH_MEASUREMENT\r\n";
                                                str += "     +- RSRP ::= ".PadRight(5, ' ');
                                                str += (stru_PBCH_MEASUREMENT.ms16PBCH_RP * 0.125).ToString("f2") + "dBm\r\n";
                                                str += "     +- RSRQ ::= ".PadRight(5, ' ');
                                                str += (stru_PBCH_MEASUREMENT.ms16PBCH_RQ * 0.0625).ToString("f2") + "dB\r\n";
                                                #endregion
                                                break;
                                            case "CRS_SINR":
                                                #region CRS_SINR
                                                CRS_SINR stru_CRS_SINR = new CRS_SINR();
                                                int size_CRS_SINR = Marshal.SizeOf(stru_CRS_SINR);
                                                if (size_CRS_SINR > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_CRS_SINR = Marshal.AllocHGlobal(size_CRS_SINR);
                                                Marshal.Copy(e.data, Moffset, intptr_CRS_SINR, size_CRS_SINR);
                                                Moffset = Moffset + size_CRS_SINR;
                                                stru_CRS_SINR = (CRS_SINR)Marshal.PtrToStructure(intptr_CRS_SINR, typeof(CRS_SINR));
                                                Marshal.FreeHGlobal(intptr_CRS_SINR);
                                                str += "  +- CRS_SINR_MEASUREMENT\r\n";
                                                str += "     +- Cell specific RS SINR ::= ".PadRight(5, ' ');
                                                str += (stru_CRS_SINR.ms16CRS_SINR * 0.0625).ToString("f2") + "dB\r\n";
                                                #endregion
                                                break;
                                            case "OFDM_SYMBOL_POWER":
                                                #region OFDM_SYMBOL_POWER
                                                OFDM_SYMBOL_POWER stru_OFDM_SYMBOL_POWER = new OFDM_SYMBOL_POWER();
                                                int size_OFDM_SYMBOL_POWER = Marshal.SizeOf(stru_OFDM_SYMBOL_POWER);
                                                if (size_OFDM_SYMBOL_POWER > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_OFDM_SYMBOL_POWER = Marshal.AllocHGlobal(size_OFDM_SYMBOL_POWER);
                                                Marshal.Copy(e.data, Moffset, intptr_OFDM_SYMBOL_POWER, size_OFDM_SYMBOL_POWER);
                                                Moffset = Moffset + size_OFDM_SYMBOL_POWER;
                                                stru_OFDM_SYMBOL_POWER = (OFDM_SYMBOL_POWER)Marshal.PtrToStructure(intptr_OFDM_SYMBOL_POWER, typeof(OFDM_SYMBOL_POWER));
                                                Marshal.FreeHGlobal(intptr_OFDM_SYMBOL_POWER);

                                                //===========================================
                                                str += "  +- OFDM_SYMBOL_POWER_MEASUREMENT\r\n";
                                                for (int i = 0; i < 10; i++)
                                                {
                                                    for (int j = 0; j < 10; j++)
                                                    {
                                                        str += "     +- POWER[" + i + "][" + j + "] ::= ".PadRight(5, ' ');
                                                        str += (stru_OFDM_SYMBOL_POWER.ofdmOFDM[i].mas16OFDM_Symbol_Power[j] * 0.125).ToString("f2") + "dBm\r\n";
                                                    }

                                                }
                                                //===========================================
                                                #endregion
                                                break;
                                            case "PBCH_EVM":
                                                #region PBCH_EVM
                                                PBCH_EVM stru_PBCH_EVM = new PBCH_EVM();
                                                int size_PBCH_EVM = Marshal.SizeOf(stru_PBCH_EVM);
                                                if (size_PBCH_EVM > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_PBCH_EVM = Marshal.AllocHGlobal(size_PBCH_EVM);
                                                Marshal.Copy(e.data, Moffset, intptr_PBCH_EVM, size_PBCH_EVM);
                                                Moffset = Moffset + size_PBCH_EVM;
                                                stru_PBCH_EVM = (PBCH_EVM)Marshal.PtrToStructure(intptr_PBCH_EVM, typeof(PBCH_EVM));
                                                Marshal.FreeHGlobal(intptr_PBCH_EVM);
                                                str += "  +- PBCH_EVM_MEASUREMENT\r\n";
                                                str += "     +- PBCH EVM ::= ".PadRight(5, ' ');
                                                str += stru_PBCH_EVM.mu16PBCH_EVM.ToString() + "\r\n";
                                                #endregion
                                                break;
                                            case "PBCH_BLER":
                                                #region PBCH_BLER
                                                PBCH_BLER stru_PBCH_BLER = new PBCH_BLER();
                                                int size_PBCH_BLER = Marshal.SizeOf(stru_PBCH_BLER);
                                                if (size_PBCH_BLER > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_PBCH_BLER = Marshal.AllocHGlobal(size_PBCH_BLER);
                                                Marshal.Copy(e.data, Moffset, intptr_PBCH_BLER, size_PBCH_BLER);
                                                Moffset = Moffset + size_PBCH_BLER;
                                                stru_PBCH_BLER = (PBCH_BLER)Marshal.PtrToStructure(intptr_PBCH_BLER, typeof(PBCH_BLER));
                                                Marshal.FreeHGlobal(intptr_PBCH_BLER);
                                                str += "  +- PBCH_BLER_MEASUREMENT\r\n";
                                                str += "     +- PBCH_BLER ::= ".PadRight(5, ' ');
                                                str += stru_PBCH_BLER.mu16PBCH_BLER.ToString() + "\r\n";
                                                #endregion
                                                break;
                                            case "SUBFRAME_RSSI":
                                                #region SUBFRAME_RSSI
                                                SUBFRAME_RSSI stru_SUBFRAME_RSSI = new SUBFRAME_RSSI();
                                                int size_SUBFRAME_RSSI = Marshal.SizeOf(stru_SUBFRAME_RSSI);
                                                if (size_SUBFRAME_RSSI > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_SUBFRAME_RSSI = Marshal.AllocHGlobal(size_SUBFRAME_RSSI);
                                                Marshal.Copy(e.data, Moffset, intptr_SUBFRAME_RSSI, size_SUBFRAME_RSSI);
                                                Moffset = Moffset + size_SUBFRAME_RSSI;
                                                stru_SUBFRAME_RSSI = (SUBFRAME_RSSI)Marshal.PtrToStructure(intptr_SUBFRAME_RSSI, typeof(SUBFRAME_RSSI));
                                                Marshal.FreeHGlobal(intptr_SUBFRAME_RSSI);
                                                str += "  +- SUBFRAME_RSSI_MEASUREMENT\r\n";
                                                str += "     +- SUBFRAME RSSI \r\n";
                                                for (int i = 0; i < 10; i++)
                                                {
                                                    if (stru_SUBFRAME_RSSI.mas16SubFrame_RSSI[i] != 0x7FFF)
                                                    {
                                                        str += "        +- RSSI[" + i + "] ::= ".PadRight(5, ' ');
                                                        str += (stru_SUBFRAME_RSSI.mas16SubFrame_RSSI[i] * 0.125).ToString("f2") + "dBm\r\n";
                                                    }
                                                }
                                                #endregion
                                                break;
                                            case "SLOT_RSSI":
                                                #region SLOT_RSSI
                                                SLOT_RSSI stru_SLOT_RSSI = new SLOT_RSSI();
                                                int size_SLOT_RSSI = Marshal.SizeOf(stru_SLOT_RSSI);
                                                if (size_SLOT_RSSI > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_SLOT_RSSI = Marshal.AllocHGlobal(size_SLOT_RSSI);
                                                Marshal.Copy(e.data, Moffset, intptr_SLOT_RSSI, size_SLOT_RSSI);
                                                Moffset = Moffset + size_SLOT_RSSI;
                                                stru_SLOT_RSSI = (SLOT_RSSI)Marshal.PtrToStructure(intptr_SLOT_RSSI, typeof(SLOT_RSSI));
                                                Marshal.FreeHGlobal(intptr_SLOT_RSSI);
                                                str += "  +- SLOT_RSSI_MEASUREMENT\r\n";
                                                str += "     +- Slot RSSI \r\n";
                                                for (int i = 0; i < 20; i++)
                                                {
                                                    if (stru_SLOT_RSSI.mas16SlotRSSI[i] != 0x7FFF)
                                                    {
                                                        str += "      +- RSSI[" + i + "] ::= ".PadRight(5, ' ');
                                                        str += (stru_SLOT_RSSI.mas16SlotRSSI[i] * 0.125).ToString("f2") + "dBm\r\n";
                                                    }
                                                }
                                                #endregion
                                                break;
                                            case "FRAME_RSSI":
                                                #region FRAME_RSSI
                                                FRAME_RSSI stru_FRAME_RSSI = new FRAME_RSSI();
                                                int size_FRAME_RSSI = Marshal.SizeOf(stru_FRAME_RSSI);
                                                if (size_FRAME_RSSI > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_FRAME_RSSI = Marshal.AllocHGlobal(size_FRAME_RSSI);
                                                Marshal.Copy(e.data, Moffset, intptr_FRAME_RSSI, size_FRAME_RSSI);
                                                Moffset = Moffset + size_FRAME_RSSI;
                                                stru_FRAME_RSSI = (FRAME_RSSI)Marshal.PtrToStructure(intptr_FRAME_RSSI, typeof(FRAME_RSSI));
                                                Marshal.FreeHGlobal(intptr_FRAME_RSSI);
                                                str += "  +- FRAME_RSSI_MEASUREMENT\r\n";
                                                str += "     +- FRAME RSSI ::= ".PadRight(5, ' ');
                                                str += (stru_FRAME_RSSI.mu16Frame_RSSI * 0.125).ToString("f2") + "dBm\r\n";
                                                #endregion
                                                break;
                                            case "RB_RSSI":
                                                #region RB_RSSI
                                                RB_RSSI stru_RB_RSSI = new RB_RSSI();
                                                int size_RB_RSSI = Marshal.SizeOf(stru_RB_RSSI);
                                                if (size_RB_RSSI > e.data.Length - Moffset)
                                                {
                                                    return null;
                                                }
                                                IntPtr intptr_RB_RSSI = Marshal.AllocHGlobal(size_RB_RSSI);
                                                Marshal.Copy(e.data, Moffset, intptr_RB_RSSI, size_RB_RSSI);
                                                Moffset = Moffset + size_RB_RSSI;
                                                stru_RB_RSSI = (RB_RSSI)Marshal.PtrToStructure(intptr_RB_RSSI, typeof(RB_RSSI));
                                                Marshal.FreeHGlobal(intptr_RB_RSSI);

                                                str += "  +- RB_RSSI_MEASUREMENT\r\n";
                                                //===========================================
                                                str += "     +- RB_RSSI\r\n";
                                                for (int i = 0; i < 100; i++)
                                                {
                                                    for (int j = 0; j < 20; j++)
                                                    {
                                                        str += "         +- POWER[" + i + "][" + j + "] ::= ".PadRight(5, ' ');
                                                        str += (stru_RB_RSSI.mas16SlotRSSI[i].mas16Slot[j] * 0.125).ToString("f2") + "dBm\r\n";
                                                    }

                                                }
                                                //===========================================
                                                #endregion
                                                break;
                                            default: break;
                                        }
                                    }
                                }
                                string mesuretype = "";
                                foreach (string type in Lmytypes)
                                {
                                    if (mesuretype == "")
                                    {
                                        mesuretype += type;
                                    }
                                    else
                                    {
                                        mesuretype += "," + type;
                                    }
                                }
                                //decodeView.richTextBox1.Text += strhead + mesuretype + "\r\n" + str;

                            }
                            //===========================================
                            #endregion
                        }
                        return null;
                    case COM.ZCTT.AGI.Common.AGIMsgDefine.L2P_AG_UNSPECIFIED_CELL_SCAN_DATA_MSG_TYPE:
                        {
                            byte[] sbMsgData = new byte[30000];
                            byte[] sbMsgName = new byte[2000];
                            if (e.data.Length > 1)
                            {
                                byte[] data = new byte[e.data.Length - 1];
                                System.Buffer.BlockCopy(e.data, 1, data, 0, data.Length);
                                //decodeDataView = new byte[e.data.Length];
                                //decodeDataView = data;
                                //UInt32 xx = DecodeDataStreamWithPosInfo(0x002e, sbMsgData, e.data, e.data.Length, sbMsgName);
                                //UInt32 xx = DecodeDataStream(0x002e, sbMsgData, e.data, e.data.Length, sbMsgName);
                                //decodeView.richTextBox1.Text += System.Text.Encoding.ASCII.GetString(sbMsgData);
                            }
                        }
                        return null;
                    default:
                        break;
                }
                //decodeText = decodeView.richTextBox1.Text;
                return result;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message=" + ex.Message + "\r\nStacktrace:" + ex.StackTrace);

            }
            return null;
        }

        /*static byte[] testData = new byte[35] 
        {0x03,0x68,0x13,0x98,0x08,0xfd,0xce,0x01,0x83,0xb1,
         0xfa,0x73,0x1f,0x44,0x0a,0x03,0x00,0x1f,0xfa,0x92,
         0xb9,0x86,0x14,0xc6,0xcc,0x00,0x01,0x23,0x00,0x81,
         0x40,0x14,0x00,0x01,0xc0  };*/
        //static byte[] testData = new byte[7]{0x04,0x53,0x14,0x97,0xb7,0x8c,0x32};
        //static byte[] testData = new byte[6] {0x53,0xA4,0x02,0x48,0x78,0xE6 };
        static byte[] testData = new byte[]{
        0x02,0x22,0x20,0x87,0x55,0x02,0x3f,0x17,0xa5,0xad,
        0x87,0xfc,0x11,0x07,0x41,0x11,0x0b,0xf6,0x64,0xf0,
        0x80,0x87,0x55,0x02,0xc0,0xb3,0x00,0x3a,0x04,0xe0,
        0xe0,0x00,0x00,0x00,0x1d,0x02,0x01,0xd0,0x11,0x27,
        0x17,0x80,0x80,0x21,0x10,0x01,0x01,0x00,0x10,0x81,
        0x06,0x00,0x00,0x00,0x00,0x83,0x06,0x00,0x00,0x00,
        0x00,0x00,0x0a,0x00,0x52,0x64,0xf0,0x80,0x00,0x03};

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] sbMsgData = new byte[2000];
            byte[] sbMsgName = new byte[2000];

            //UInt32 xx = DecodeDataStreamWithPosInfo(0x035, sbMsgData, testData, 70, sbMsgName);
            //UInt32 xx = DecodeDataStream(0x035, sbMsgData, testData, 70, sbMsgName);
            //richTextBox1.Text = System.Text.Encoding.ASCII.GetString(sbMsgData);
        }
       
    }
}
