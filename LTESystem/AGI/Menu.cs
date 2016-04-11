using COM.ZCTT.AGI.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace COM.ZCTT.AGI
{
    ///<summary>
    ///该文件主要用于创建菜单Menu类
    ///</summary>
    ///<author>chunyang</author>
    ///<author>zhouyang</author>
    ///<date>2013/03/29</date>
    ///<version> version 1.0 该文件件主要用于创建菜单Menu类，并实现其方法</version>
    ///<details >
    ///该文件件主要用于创建菜单Menu类，并实现其方法
    ///</ details >


    ///<class>Menu</class>
    /// <summary>
    /// 菜单类
    /// </summary>
    /// <date>2013/03/20</date>
    /// <details >
    /// 该类主要用来定义并且实现菜单
    /// </details> 
    public class Menu// : System.Windows.Forms.ToolStripMenuItem 
    {
        #region 字段属性
        private string commandName;
        private string commandIcon;
        private string command;
        private string dllName;
        private List<Menu> childMenu;
        //添加
        private List<Menu> gardenSonMenu;

        public string CommandName
        {
            get { return commandName; }
            set { commandName = value; }
        }

        public string CommandIcon
        {
            get { return commandIcon; }
            set { commandIcon = value; }
        }

        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        public string DllName
        {
            get { return dllName; }
            set { dllName = value; }
        }

        public List<Menu> ChildMenu
        {
            get { return childMenu; }
            set { childMenu = value; }
        }
        //修改
        public List<Menu> Gradenson
        {
            get { return gardenSonMenu; }
            set { gardenSonMenu = value; }
        }

        #endregion


        //private IPlugin realCommand;
        ///<name>IPlugin</name>
        /// <summary>
        ///得到命令接口
        /// </summary>
        public IPlugin GetCommand()
        {
            return PluginHelper.GetPlugin(this);
        }

        #region 单例
        public Menu()
        {

        }
        private static Menu instance;
        public static Menu Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Menu();
                }
                return instance;
            }
        }


        #endregion


        #region 序列化测试

        ///<name>GenerateXML</name>
        /// <summary>
        /// 建立和得到XML List容器
        /// </summary>
        /// <param>无</param>
        /// <exception></exception>
        /// <return>无</return>
        public static void GenerateXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Menu>));
            string path = System.Windows.Forms.Application.StartupPath + "\\Menu.xml";
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

            Menu menu1 = new Menu()
            {
                CommandName = "文件",
                //Command = "COM.ZCTT.AGI.OpenFileCommand"
            };

            List<Menu> lst1 = new List<Menu>();
            lst1.Add(new Menu()
            {
                CommandName = "打开",
                Command = "COM.ZCTT.AGI.OpenFileCommand"
            });
            lst1.Add(new Menu()
            {
                CommandName = "保存",
                Command = "COM.ZCTT.AGI.SaveFileCommand"
            });
            lst1.Add(new Menu()
            {
                CommandName = "退出",
                Command = "COM.ZCTT.AGI.CloseFileCommand"
            });

            menu1.ChildMenu = lst1;

            List<Menu> lst = new List<Menu>();
            lst.Add(menu1);
            xs.Serialize(stream, lst);
            stream.Close();
        }

        ///<name>GenerateFromXml</name>
        /// <summary>
        /// 建立和得到XML List容器
        /// </summary>
        /// <param>无</param>
        /// <exception>无</exception>
        /// <return>p</return>
        public static List<Menu> GenerateFromXml()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Menu>));
            string path = System.Windows.Forms.Application.StartupPath + "\\Menu.xml";
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            List<Menu> p = (List<Menu>)xs.Deserialize(stream);
            return p;
        }
        #endregion
        //    #region 加载命令
        //    void toolSubItem_Click(object sender, EventArgs e)
        //    {
        //        //创建菜单调用方法类的实例
        //        MenuMethod menuMethod = new MenuMethod();
        //        Type type = menuMethod.GetType();
        //        //动态获取方法对象
        //        MethodInfo mi = type.GetMethod(((ToolStripMenuItem)sender).Name);
        //        //调用指定方法
        //        mi.Invoke(menuMethod, null);

        //    }
        //    #endregion
    }
}
