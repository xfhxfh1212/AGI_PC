using COM.ZCTT.AGI.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Text;

namespace COM.ZCTT.AGI
{
    ///<filename>ToolBar.cs</filename>
    ///<summary>该文件主要用于创建ToolBar类
    ///</summary>
    ///<author>chunyang1</author>
    ///<author>chunyang2</author>
    ///<date>2013/03/20</date>
    ///<version> version 1.0 该文件主要创建了一个ToolBar类并实现类型成员的信息显示</version>
    ///<details >
    ///该文件创建一个ToolBar类并实现其方法
    ///</ details >


    ///<class>ToolBar</class>
    /// <summary>
    /// 
    /// </summary>
    /// <date>2013/03/20</date>
    /// <details >
    ///
    /// </details> 
    public class ToolBar
    {
        private string commandName;
        private string icon;
        private string command;
        private string dllName;
        private List<ToolBar> childToolbar;

        public string CommandName
        {
            get
            {
                return commandName;
            }
            set
            {
                commandName = value;
            }
        }

        public string Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
            }
        }
        public string Command
        {
            get
            {
                return command;
            }
            set
            {
                command = value;
            }
        }
        public string DllName
        {
            get
            {
                return dllName;
            }
            set
            {
                dllName = value;
            }
        }

        public List<ToolBar> ChildToolBar
        {
            get
            {
                return childToolbar;
            }
           set
           {
                childToolbar = value;
            }
        }

        //private IPlugin realCommand;
        public IPlugin GetCommand()
        {
            return PluginHelper.GetPlugin(this);
            //return 0;
        }

        public ToolBar()
        {

        }

        public static ToolBar instance;
        public static ToolBar Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ToolBar();
                }
                return instance;
            }
        }

        public static void GenerateXML()
        {

        }

        /// <name>GenerateFormXml</name>
        /// <summary>
        /// 
        /// </summary>
        /// <param>无</param>
        /// <exception cref="Exception">无</exception>
        /// <return>p</return>
        public static List<ToolBar> GenerateFormXml()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<ToolBar>));
            string path = System.Windows.Forms.Application.StartupPath +"\\Toolbar.xml";
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            List<ToolBar> p = (List<ToolBar>)xs.Deserialize(stream);
            return p;
        }
    }
}