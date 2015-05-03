using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class BackupElement
	{
		public Hashtable BackupHash;

		public BackupElement(XmlNode node)
		{
			//string Feature = node.ParentNode.Attributes["id"].Value;
			string upgradefrom = node.Attributes["upgradefrom"].Value;

			XmlNodeList installNodes = node.SelectNodes("./install");

			BackupHash = new Hashtable();

			if (installNodes.Count > 0)
			{
				foreach (XmlNode installNode in installNodes)
				{
					string backupLocation = installNode.InnerText.Trim();
					backupLocation = Regex.Replace(backupLocation, @"\$INSTALL_ROOT", "[INSTALLDIR]");
					BackupHash.Add(BackupHash.Count + 1, new Backup(backupLocation, upgradefrom));
				}
			}
			else
			{
				string backupLocation = node.Attributes["install"].Value;
				backupLocation = Regex.Replace(backupLocation, @"\$INSTALL_ROOT", "[INSTALLDIR]");
				BackupHash.Add(BackupHash.Count + 1, new Backup(backupLocation, upgradefrom));
			}
		}
	}
}
