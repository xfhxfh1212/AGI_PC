using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace COM.ZCTT.AGI
{
    ///<summary>
    ///该文件主要用于创建NavigatorBar类
    ///</summary>
    ///<author>chunyang1</author>
    ///<author>chunyang2</author>
    ///<date>2013/03/20</date>
    ///<version> version 1.0 该文件主要创建了一个NavigatorBar并实现类型成员的信息显示</version>
    ///<details >
    ///该文件创建一个NavigatorBar并实现其方法
    ///</details >

    ///<class>NavigatorBar</class>
    /// <summary>
    /// 该类是用来实现导航栏动态加载
    /// </summary>
    /// <date>2013/03/20</date>
    /// <details >
    /// 该类是用来实现导航栏动态加载
    /// </details>
    class NavigatorBar
    {
        ///<name>InitTreeView</name>
        /// <summary>
        /// 初始化TreeView
        /// </summary>
        /// <param name="nodes" type="TreeNodeCollection">TreeView的根节点</param>
        /// <param name="filename" type="string">Xml文件路径</param>
        /// <exception cref="Exception"></exception>
        /// <return>无</return>
        public void InitTreeView(TreeNodeCollection nodes, string filename)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                TreeNode parentNode = new TreeNode();
               // parentNode = null;
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    XmlElement xe = node as XmlElement;
                    if (xe.Attributes["Name"] != null)
                    {
                        TreeNode newNode = new TreeNode();
                        newNode.Text = xe.Attributes["Name"].Value;
                        try
                        {
                            newNode.Tag = xe.Attributes["FilePath"].Value;
                        }
                        catch
                        {
 
                        }
                        
                        SerchXmlDoc(node, newNode);
                        parentNode.Nodes.Add(newNode);
                    }
                    else
                    {
                        SerchXmlDoc(node, parentNode);
                    }
                }
                nodes.Add(parentNode.FirstNode);
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        ///<name>SerchXmlDoc()</name>
        /// <summary>
        /// 查找和添加子节点
        /// </summary>
        /// <param name="xmlNode" type="xmlNode">Xml节点</param>
        /// <param name="treeNode" type="TreeNode">TreeNode的父节点</param>
        /// <return>无</return>
        private static void SerchXmlDoc(XmlNode xmlNode,TreeNode treeNode)
        {
            if (xmlNode.ChildNodes.Count == 0)
            {
                return;
            }
            else
            {
                foreach(XmlNode node in xmlNode)
                {
                    try
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            XmlElement xe = node as XmlElement;

                            if (xe.Attributes["Name"] != null)
                            {
                                TreeNode newNode = new TreeNode();
                                newNode.Text = xe.Attributes["Name"].Value;
                                try
                                {
                                    newNode.Tag = xe.Attributes["FilePath"].Value;
                                }
                                catch
                                {
 
                                }                                

                                SerchXmlDoc(node, newNode);
                                treeNode.Nodes.Add(newNode);
                            }
                            else
                            {
                                SerchXmlDoc(node, treeNode);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }
        }
    }
}


