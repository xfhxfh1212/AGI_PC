using COM.ZCTT.AGI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DeviceMangerModule;
using Plugin;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;
using AGIInterface;
using COM.ZCTT.AGI.Common;

namespace LTESystem
{
    public partial class frmLTESystem : Form
    {
        public frmLTESystem()
        {
            InitializeComponent();

            CellSearchDockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;
            FindSTMSIDockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;
            OrientionDockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;
            
            //dockPanel1.DefaultFloatWindowSize = new Size(800, 600);
            CellSearchDockPanel.DockRightPortion = 0.4;
            FindSTMSIDockPanel.DockRightPortion = 0.4;
            OrientionDockPanel.DockRightPortion = 0.4;
            this.WindowState = FormWindowState.Maximized;
            Global.testMode = Global.TestMode.RealTime;
            Global.ReadyModule = "ProtocolTrace";

            //statusColor.BackColor = System.Drawing.Color.Red;
            this.Load += Form_Load;
            this.Load += MenuStrip_Load;

            Global.tempClass.SendStatusToMainFormEvent += new Class1.DeviceSendStatusToMainForm(StatusChanged);
            statusList.AddRange(new List<ToolStripStatusLabel>{status0,status1,status2,status3,status4});
        }
        private static List<ToolStripStatusLabel> statusList = new List<ToolStripStatusLabel>();
        private void frmLTESystem_FormClosed(object sender, FormClosedEventArgs e)
        {
            Global.tempClass.CloseDisplayForm();
            Application.Exit();
        }

        private void frmLTESystem_Resize(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 主界面加载，初始化dockpanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Form_Load(object sender, EventArgs e)
        {
            CellSearchDockPanel.Width = FindSTMSIDockPanel.Width = OrientionDockPanel.Width = this.Width / 3;
            FindSTMSIDockPanel.Left = CellSearchDockPanel.Right;
            OrientionDockPanel.Left = FindSTMSIDockPanel.Right;

            GloabeControl.RightContent = this;
            GloabeControl.cellSearchDockPanel = this.CellSearchDockPanel;
            GloabeControl.findSTMSIDockPanel = this.FindSTMSIDockPanel;
            GloabeControl.oriFindingDockPanel = this.OrientionDockPanel;
            
        }

        /// <summary>
        /// menu菜单加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuStrip_Load(object sender, EventArgs e)
        {
            List<COM.ZCTT.AGI.Menu> lst = COM.ZCTT.AGI.Menu.GenerateFromXml();

            COM.ZCTT.AGI.Plugin.PluginHelper.Load(lst);

            foreach (COM.ZCTT.AGI.Menu menu in lst)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = menu.CommandName;

                MenuStrip.Items.Add(menuItem);
                menuItem.Name = menu.CommandName;

                foreach (COM.ZCTT.AGI.Menu child in menu.ChildMenu)
                {
                    ToolStripSeparator separator = new ToolStripSeparator();
                    ToolStripMenuItem menuSubItem = new ToolStripMenuItem();
                    menuSubItem.Tag = child;
                    menuSubItem.Text = child.CommandName;

                    menuSubItem.Click += MenuItem_Click;
                    menuItem.DropDownItems.Add(menuSubItem);

                    switch(menuSubItem.Text)
                    {
                        case "开始测向":
                            menuSubItem.Enabled = true;
                            break;
                        case "重新测向":
                            menuSubItem.Enabled = false;
                            break;
                        //case "停止测向":
                        //    menuSubItem.Enabled = false;
                        //    break;
                        case "系统设置":
                            //menuItem.DropDownItems.Add(separator);
                            break;
                        case "小区搜索":
                        case "寻找目标号码":
                        case "测向":
                            menuSubItem.Visible = false;
                            break;
                        default:
                            break;
                    }
                }
                
            }

            
        }

        /// <summary>
        /// menu菜单点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                COM.ZCTT.AGI.Menu menu = (sender as ToolStripMenuItem).Tag as COM.ZCTT.AGI.Menu;
                if (menu.Command != null)
                    menu.GetCommand().Load();

                switch(menu.CommandName)
                {
                    case "开始测向":
                        COM.ZCTT.AGI.Plugin.PluginHelper.GetPlugin("CellSearchAndMonitor.dll", "CellSearchAndMonitor.CellSearchAndMonitor").Load();
                        
                        COM.ZCTT.AGI.Plugin.PluginHelper.GetPlugin("CellSearchAndMonitor.dll", "CellSearchAndMonitor.OrientationFinding").Load();
                        COM.ZCTT.AGI.Plugin.PluginHelper.GetPlugin("CellSearchAndMonitor.dll", "CellSearchAndMonitor.FindTargetSTMSI").Load();
                        MenuItemSetEnabled("开始测向", false);
                        MenuItemSetEnabled("重新测向", true);
                        Global.isReboot = false;
                        break;
                    case "重新测向":
                        Global.tempClass.CloseDisplayForm();
                        COM.ZCTT.AGI.Plugin.PluginHelper.GetPlugin("CellSearchAndMonitor.dll", "CellSearchAndMonitor.CellSearchAndMonitor").Load();
                        COM.ZCTT.AGI.Plugin.PluginHelper.GetPlugin("CellSearchAndMonitor.dll", "CellSearchAndMonitor.OrientationFinding").Load();
                        COM.ZCTT.AGI.Plugin.PluginHelper.GetPlugin("CellSearchAndMonitor.dll", "CellSearchAndMonitor.FindTargetSTMSI").Load();
                        break;
                    //case "停止测向":
                    //    MenuItemSetEnabled("开始测向", true);
                    //    MenuItemSetEnabled("停止测向", false);
                    //    Global.tempClass.CloseDisplayForm();
                    //    break;
                    case "系统设置":
                        //MenuItemSetEnabled("开始测向", true);
                        //MenuItemSetEnabled("停止测向", false);
                        //foreach (ToolStripMenuItem menuItem in MenuStrip.Items)
                        //{
                        //    if(menuItem.Text=="测向")
                        //    {
                        //        foreach (ToolStripMenuItem menuSubItem in menuItem.DropDownItems)
                        //        {
                        //            switch (menuSubItem.Text)
                        //            {
                        //                case "开始测向":
                        //                    menuSubItem.Enabled = true;
                        //                    break;
                        //                case "停止测向":                                            
                        //                    break;
                        //                default:
                        //                    break;
                        //            }
                        //        }
                        //    }
                            
                        //}
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message=" + ex.Message + "\r\nStacktrace:" + ex.StackTrace);
            } 
        }

        private void MenuItemSetEnabled(string MenuItemTest,bool enabled)
        {
            foreach (ToolStripMenuItem menuItem in MenuStrip.Items)
            {
                if (menuItem.Text == "测向")
                {
                    foreach (ToolStripMenuItem menuSubItem in menuItem.DropDownItems)
                    {
                        switch (menuSubItem.Text)
                        {
                            case "开始测向":
                                if (MenuItemTest == "开始测向")
                                    menuSubItem.Enabled = enabled;
                                break;
                            case "重新测向":
                                if (MenuItemTest == "重新测向")
                                    menuSubItem.Enabled = enabled;
                                break;
                            //case "停止测向":
                            //    if (MenuItemTest == "停止测向")
                            //        menuSubItem.Enabled = enabled;
                            //    break;
                            default:
                                break;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 改变状态栏状态事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusChanged(object sender, CustomDataEvtArg e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Class1.DeviceSendStatusToMainForm(StatusChanged), sender, e);
                }
                catch
                {

                }
            }
            else
            {
                int index = 0;
                int connCount = 0;
                int idleCount = 0;
                foreach (Device temdev in DeviceManger.deviceList)
                {
                    if (temdev.ConnectionState == 0)
                    {
                        //statusList[index].Text = " ";
                        statusList[index].Image = imageList1.Images[0];
                        connCount++;
                        if (temdev.DeviceInstrumentState == 0)
                            idleCount++;
                    }
                    else
                    {
                        //statusList[index].Text = " ";
                        statusList[index].Image = imageList1.Images[1];
                    }
                    
                    index++;
                }
                if (connCount == 0)
                {
                    Device.Text = "设备状态：";
                    DeviceColor.BackColor = System.Drawing.SystemColors.Control;
                }
                else if (idleCount == connCount)
                {
                    Device.Text = "设备状态：IDLE";
                    DeviceColor.BackColor = System.Drawing.Color.Green;
                }
                else
                {
                    Device.Text = "设备状态：Working";
                    DeviceColor.BackColor = System.Drawing.Color.Red;
                }
                
            }
        }

        //protected override void WndProc(ref Message m)
        //{
        //    //if (m.Msg != 0xA3 && m.Msg != 0x003 && m.WParam != (IntPtr)0xF012)
        //    //    base.WndProc(ref m);
        //}

    }
}
