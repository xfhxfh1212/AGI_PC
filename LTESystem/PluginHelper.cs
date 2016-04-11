using COM.ZCTT.AGI.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace COM.ZCTT.AGI.Plugin
{
    public class PluginHelper
    {
        private static Dictionary<string, Assembly> plugins = new Dictionary<string, Assembly>();
        private static string basePath = Application.StartupPath + "\\";
        public static void Load(Menu menu)
        {
            //加载程序集 
            Assembly assembly = null;
            if(string.IsNullOrEmpty(menu.DllName))
            {
                if (menu.ChildMenu != null)
                {
                    foreach (Menu childMenu in menu.ChildMenu)
                    {
                        //递归调用，加载子菜单
                        Load(childMenu);
                    }
                }
            }
            else
            {
                if (plugins.ContainsKey(menu.DllName))
                {
                    plugins.TryGetValue(menu.DllName, out assembly);
                }
                else
                {
                    string dllFile = basePath + menu.DllName;
                    assembly = Assembly.LoadFile(dllFile);
                    plugins.Add(menu.DllName, assembly);
                }

            }
        }
        public static void Load(List<Menu> menus)
        {

            foreach (Menu menu in menus)
            {
                Load(menu);
            }
        }
        public static IPlugin GetPlugin(Menu menu)
        {
            Assembly assembly = null;
            if (plugins.ContainsKey(menu.DllName))
            {
                plugins.TryGetValue(menu.DllName, out assembly);
                
            }           
            IPlugin plugin = assembly.CreateInstance(menu.Command) as IPlugin;
            return plugin;
        }
        public static IPlugin GetPlugin(string dllName, string command)
        {
            Assembly assembly = null;
            if (plugins.ContainsKey(dllName))
            {
                plugins.TryGetValue(dllName, out assembly);

            }
            IPlugin plugin = assembly.CreateInstance(command) as IPlugin;
            return plugin;
        }

        public static void Load(ToolBar toolbar)
        {
            //加载程序集 
            Assembly assembly = null;
            if (string.IsNullOrEmpty(toolbar.DllName))
            {
                if (toolbar.ChildToolBar != null)
                {
                    foreach (ToolBar childtoolbar in toolbar.ChildToolBar)
                    {
                        //递归调用，加载子菜单
                        Load(childtoolbar);
                    }
                }
            }
            else
            {
                if (plugins.ContainsKey(toolbar.DllName))
                {
                    plugins.TryGetValue(toolbar.DllName, out assembly);
                }
                else
                {
                    string dllFile = basePath + toolbar.DllName;
                    assembly = Assembly.LoadFile(dllFile);
                    plugins.Add(toolbar.DllName, assembly);
                }

            }
        }

        public static void Load(List<ToolBar> toolbars)
        {

            foreach (ToolBar toolbar in toolbars)
            {
                Load(toolbar);
            }
        }


        public static IPlugin GetPlugin(ToolBar toolbar)
        {
            Assembly assembly = null;
            if (plugins.ContainsKey(toolbar.DllName))
            {
                plugins.TryGetValue(toolbar.DllName, out assembly);
            }

            IPlugin plugin = assembly.CreateInstance(toolbar.Command) as IPlugin;
            return plugin;
        }

    }
}
