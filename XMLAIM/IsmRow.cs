using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace XMLAIM
{
	class IsmRow
	{
		public Dictionary<string, string> Row;

		public IsmRow(Dictionary<int, string> Attributes, XmlNode node)
		{
			
			Row = new Dictionary<string, string>();
			XmlNodeList tableSearchNodes = node.SelectNodes("./td");
			int i = 0;
			foreach (XmlNode tNode in tableSearchNodes)
			{
				Row.Add(Attributes[i].ToString(), tNode.InnerText);
				i++;
			}

		}

		public IsmRow()
		{
			Row = new Dictionary<string, string>();
		}

		public XmlDocument updateXML(XmlDocument xDoc, string tableName)
		{
			//XmlDocument xDoc = new XmlDocument();
			//XmlElement newRow = new XmlElement( //xDoc.CreateElement("row");
			//XmlElement newRow = new XmlElement;
			
			//XmlElement newtd = xDoc.CreateElement("td");
			//newtd.InnerText = iconName;
			//newRow.AppendChild((XmlNode)newtd);

			/*string xml = "<row>";
			

			foreach (string rowName in Row.Keys)
			{
				if (Row[rowName] != "")
				{
					xml = xml + "<td>" + Row[rowName] + "</td>";
				}
				else
				{
					xml = xml + "<td/>";
				}
			}*/
			XmlElement xeRoot = xDoc.DocumentElement;

			XmlNode updateNode = xeRoot.SelectSingleNode("/msi/table[@name='" + tableName + "']");

			XmlElement newRow = xDoc.CreateElement("row");

			foreach (string rowName in Row.Keys)
			{
				XmlElement xmlnewtd = xDoc.CreateElement("td");
				if (Row[rowName] != "")
				{	
					xmlnewtd.InnerText = Row[rowName];	
				}
				newRow.AppendChild((XmlNode)xmlnewtd);

			}

			updateNode.AppendChild((XmlNode)newRow);

			return xDoc;
		}
	}
}
