using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections;


//Not currently using. will delete if it stays that way.
namespace XMLAIM
{
	class VariableElement
	{
		//public Dictionary <string, string> PropertyHash;
		public string Var;
		public string Value;

		public VariableElement(XmlNode node)
		{
			//string Feature = node.ParentNode.Attributes["id"].Value;
			Var = node.Attributes["id"].Value;
			Value = node.Attributes["value"].Value;

			//XmlNodeList installNodes = node.SelectNodes("./install");

			//PropertyHash = new Dictionary<string,string>();

			//PropertyHash.Add(Property

			/*if (installNodes.Count > 0)
			{
				foreach (XmlNode installNode in installNodes)
				{
					string backupLocation = installNode.InnerText;
					PropertyHash.Add(PropertyHash.Count + 1, new Backup(backupLocation, upgradefrom));
				}
			}
			else
			{
				string backupLocation = node.Attributes["install"].Value;
				PropertyHash.Add(PropertyHash.Count + 1, new Backup(backupLocation, upgradefrom));
			}*/
		}
	}
}
