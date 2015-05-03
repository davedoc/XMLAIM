using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

namespace XMLAIM
{
	class DeleteElement
	{
		public Hashtable deleteHash;

		public DeleteElement(XmlNode node)
		{
			//string Feature = node.ParentNode.Attributes["id"].Value;
			XmlNodeList installNodes = node.SelectNodes("./install");

			deleteHash = new Hashtable();

			if (installNodes.Count > 0)
			{
				foreach (XmlNode stageNode in installNodes)
				{
					string deleteLocation = stageNode.InnerText;
					deleteHash.Add(deleteHash.Count + 1, deleteLocation);
				}
			}
			else
			{
				string deleteLocation = node.Attributes["install"].Value;
				deleteHash.Add(deleteHash.Count + 1, deleteLocation);
			}
		}
	}
}
