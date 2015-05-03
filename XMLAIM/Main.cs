using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace XMLAIM
{
    public partial class Main : Form
    {
        //static TextBox logTextbox;
        private Manifest Man;
        private AIMLogger logger;
        public Main()
        {
            InitializeComponent();
        }

        private void ManifstXMLbutton_Click(object sender, EventArgs e)
        {
            string curFile = ManifestXMLlabel.Text;
            //Console.WriteLine(File.Exists(curFile) ? "File exists." : "File does not exist.");

            //XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(ManifestXMLlabel.Text);

            //StreamReader sr = new StreamReader(curFile);
            //string s = sr.ReadToEnd();

            //ManifesttreeView.

            /*TreeNode Node = new TreeNode("Test1");

            ManifesttreeView.Nodes.Add(Node);
            Node = new TreeNode("Test2");
            TreeNode Node2 = new TreeNode("anothertest");
            Node.Nodes.Add(Node2);
            ManifesttreeView.Nodes.Add(Node);*/

            logger = new AIMLogger(logTextbox);

			Man = new Manifest(curFile);
            Man.loadManifest();

            

            logger.log(File.Exists(curFile).ToString() + " " + Man.temp);
        }

        private void LoadIsmButton_Click(object sender, EventArgs e)
        {
			//IsmTable T = new IsmTable(IsmTextBox.Text);
			//T.ParseTable2(IsmTextBox.Text, "File");
            IsmOperator op = new IsmOperator(IsmTextBox.Text);
            op.loadISM();
            op.processManifest(Man, null);
        }

        /*public static void updateLogField(string log)
        {
            if (logTextbox.Visible == true)
            {
                logTextbox.Text = logTextbox.Text + "\r\n" + log;
            }
            else
            {
                logTextbox.Visible = true;
                logTextbox.Text = log;
            }
        }*/
    }
}
