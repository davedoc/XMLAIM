using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class IsmTableN
	{
		private Dictionary<int, string> Attributes;
		public Dictionary<string, IsmRow> Rows;
		private string iKey;
		private string tableName;
		public IsmTableN(XmlNode node, string table)
		{
			tableName = table;
			iKey = "0";
			Attributes = getColumns(node);
			Rows = getRows(node);
			
		}

		public IsmTableN(XmlNode node, string Key, string table)
		{
			iKey = Key;
			Attributes = getColumns(node);
			Rows = getRows(node);
			tableName = table;
		}

		private Dictionary<int, string> getColumns(XmlNode node)
		{
			XmlNodeList tableSearchNodes = node.SelectNodes("./col");

			Dictionary<int, string> Attributes = new Dictionary<int, string>();

			int attributeCount = 0;
			if (tableSearchNodes != null)
			{
				foreach (XmlNode tnode in tableSearchNodes)
				{
					Attributes.Add(attributeCount, tnode.InnerText);
					attributeCount++;
				}
			}

			return Attributes;
		}

		private Dictionary<string, IsmRow> getRows(XmlNode node)
		{
			Dictionary<string, IsmRow> Rows = new Dictionary<string, IsmRow>();

			XmlNodeList tableSearchNodes = node.SelectNodes("./row");

			try
			{
				foreach (XmlNode tNode in tableSearchNodes)
				{
					IsmRow row = new IsmRow(Attributes, tNode);

					Rows.Add(getRowKey(row), row);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: Could not add " + this.tableName);
			}

			return Rows;
		}

		private string getRowKey(IsmRow row){
			string[] substrings = Regex.Split(iKey, ",");
			string key = "";
			foreach (string sub in substrings)
			{
				if(key != ""){
					key = key + "," + row.Row[Attributes[int.Parse(sub)].ToString()];
				}else{
					key = row.Row[Attributes[int.Parse(sub)].ToString()];
				}
				
			}
			return key;
		}

		public IsmRow getRow(string query)
		{
			IsmRow row = null;
			if (Rows.ContainsKey(query))
			{
				row = Rows[query];
			}
			return row;
		}

		public void addRow(IsmRow Row)
		{
			int c = 0;
			foreach (string Key in Row.Row.Keys)
			{
				if (Attributes.ContainsValue(Key))
				{
					c++;
					
				}
				else
				{
					throw new Exception("Attribute does not have key: " + Key);
				}
			}

			if (Attributes.Count != c)
			{
				throw new Exception("Attribute does not contain all the Keys required!");
			}
			else
			{
				Rows.Add(getRowKey(Row), Row);
			}
		}

		public XmlDocument updateXML(XmlDocument xDoc)
		{
			XmlElement xeRoot = xDoc.DocumentElement;

			XmlNode updateNode = xeRoot.SelectSingleNode("/msi/table[@name='" + tableName + "']");

			XmlNode savedNode = updateNode.Clone();
			XmlNodeList nodeList = savedNode.SelectNodes("./col");

			
			updateNode.RemoveAll();
			
			foreach (XmlNode n in nodeList){
				updateNode.AppendChild(n);
			}

			int i = 0;
			while (i < savedNode.Attributes.Count)
			{
				//XmlAttribute att = c;
				updateNode.Attributes.Append(savedNode.Attributes[i]);
			}

			//string RowXml = "";

			foreach (string rowName in Rows.Keys)
			{
				xDoc = Rows[rowName].updateXML(xDoc, tableName);
			}

			//updateNode.
			return xDoc;

		}
	}
}
