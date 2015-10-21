using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Reflection;

namespace Adekc
{
    public partial class Form1 : Form
    {
        List<MenuNode> menuTree = new List<MenuNode>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadTree();            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectNode = ((TreeView)sender).SelectedNode;
            if (selectNode.GetNodeCount(true) == 0)
            {                
            }
            else
            {
                selectNode.Expand();
            }

            
            Uri uri = new Uri(Environment.CurrentDirectory + "\\" + findMenuNode(menuTree, selectNode.Name).rc);            
            webBrowser1.Navigate(uri);

            this.toolStripStatusLabel1.Text = selectNode.Name;
            
        }
        private string getMenuFileName(string cultureName)
        {
            if (cultureName.Contains("zh-"))
            {
                return "menutree_zh-CN.xml";
            }

            return "menutree.xml";
        }
        private void LoadTree()
        {            
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(getMenuFileName(System.Globalization.CultureInfo.CurrentUICulture.Name));
            }
            catch (System.Exception ex)
            {
                ex.ToString();
            }

            XmlNode rootNode = xmlDoc.SelectSingleNode("/menutree/nodes");

            ParseNodes(rootNode, menuTree);

            GenTree(null, menuTree, "");
        }

        private void ParseNodes(XmlNode nodes, List<MenuNode> list)
        {
            XmlNode iter = nodes.FirstChild;

            while (iter != null)
            {
                MenuNode node = ParseNode(iter);

                if (node != null)
                {
                    list.Add(node);
                }

                iter = iter.NextSibling;
            }
        }

        private MenuNode ParseNode(XmlNode node)
        {
            XmlNode iter = node.FirstChild;
            MenuNode mn = new MenuNode();

            while (iter != null)
            {
                switch (iter.Name)
                {
                    case "name":
                        mn.name = iter.InnerText;
                        break;
                    case "rc":
                        if (!iter.InnerText.Trim().Equals(""))
                        {
                            mn.rc = iter.InnerText;
                        }                        
                        break;
                    case "nodes":
                        mn.childNodes = new List<MenuNode>();
                        ParseNodes(iter, mn.childNodes);
                        break;
                    default :
                        break;
                }
                iter = iter.NextSibling;
            }
            return mn;
        }

        private void GenTree(TreeNode node, List<MenuNode> menu, string parentName)
        {
            int i = 0;
            TreeNode childNode = null;
            string nodeName;

            while (i < menu.Count)
            {
                if (node == null)
                {
                    childNode = treeView1.Nodes.Add(menu[i].name, menu[i].name);
                    nodeName = menu[i].name;
                }
                else
                {
                    childNode = node.Nodes.Add(parentName + "." + menu[i].name, menu[i].name);
                    childNode.ImageIndex = 1;
                    nodeName = parentName + "." + menu[i].name;
                }                
                
                if (menu[i].childNodes != null)
                {
                    GenTree(childNode, menu[i].childNodes, nodeName);
                }

                i++;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(null);
        }

        private MenuNode findMenuNode(List<MenuNode> menu, string name)
        {            
            int i = 0;
            bool isLeaf = false;
            string n;
            MenuNode node = null;

            if (name.IndexOf('.') == -1)
            {
                isLeaf = true;
                n = name;
            }
            else
            {
                n = name.Substring(0, name.IndexOf('.'));
            }
         
            while (i < menu.Count)
            {
                if (menu[i].name.Equals(n))
                {
                    node = menu[i];

                    if (isLeaf)
                    {
                        return node;
                    }
                    else if (node.childNodes != null)
                    {
                        return findMenuNode(node.childNodes, name.Substring(name.IndexOf('.') + 1));
                    }                    
                }

                i++;                           
            }

            return null;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }

        private void toolStripTextBox1_Enter(object sender, EventArgs e)
        {
            this.searchTextBox.Text = "";
            this.searchTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
        }

        private void toolStripTextBox1_Leave(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            resources.ApplyResources(this.searchTextBox, this.searchTextBox.Name);
            this.searchTextBox.ForeColor = System.Drawing.SystemColors.ControlDark;
        }      

    }
}