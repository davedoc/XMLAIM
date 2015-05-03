using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace XMLAIM
{
	class IsmDatabase
	{
		private string sIsmFile;
		public Dictionary<string, IsmTableN> dIsmTable;
		XmlDocument xDoc;
		XmlElement xeRoot;
		//int iKey = 0;

		public IsmDatabase(string ism)
		{
			sIsmFile = ism;
			dIsmTable = new Dictionary<string, IsmTableN>();
			XmlTextReader reader = new XmlTextReader(sIsmFile);
			xDoc = new XmlDocument();
			xDoc.Load(reader);
			reader.Close();
			//xDoc.PreserveWhitespace = true;
			
			xeRoot = xDoc.DocumentElement;
		}

		//public Dictionary<strin

		public void LoadTable(string tableName, string primaryKey)
		{
			XmlNode updateNode = xeRoot.SelectSingleNode("/msi/table[@name='" + tableName + "']");
			IsmTableN newTable = new IsmTableN(updateNode, primaryKey, tableName);
			dIsmTable.Add(tableName, newTable);
			//newTable
		}

		public IsmRow getRow(string tableName, string search)
		{
			//IsmRow Row = 

			IsmRow Row = dIsmTable[tableName].getRow(search);

			return Row;
		}

		public Dictionary<string, IsmRow> getRows(string tableName)
		{
			IsmTableN table = dIsmTable[tableName];
			if(table != null){
				return table.Rows;
			}

			return null;
			
		}

		public void addRow(string tableName, IsmRow Row)
		{
			try
			{
				dIsmTable[tableName].addRow(Row);
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR Could not add Row: " + ex.Message);
			}
		}

		public void writeData()
		{
			foreach (string table in dIsmTable.Keys)
			{
				//XmlNode updateNode = xeRoot.SelectSingleNode("/msi/table[@name='" + table + "']");
				xDoc = dIsmTable[table].updateXML(xDoc);
			}

			/*
			 * XmlWriterSettings settings = new XmlWriterSettings();
    settings.Indent = true;
    settings.IndentChars = "  ";
    settings.NewLineChars = "\r\n";
    settings.NewLineHandling = NewLineHandling.Replace;
    XmlWriter writer = XmlWriter.Create(sb, settings);
			 * */
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.NewLineHandling = NewLineHandling.None;
			settings.Indent = true;
			settings.IndentChars = "  ";
			settings.NewLineChars = "\r\n";
			XmlWriter w = XmlWriter.Create(sIsmFile, settings);
			//XmlTextWriter writer = new XmlTextWriter(sIsmFile, null);
			//writer.Settings = new XmlWriterSettings();
			//writer.Settings.NewLineHandling = NewLineHandling.Entitize;
			//writer.Formatting = Formatting.Indented;
			//writer.Settings.NewLineHandling = 
			xDoc.Save(w);
			//xDoc.
			//xDoc.Save(sIsmFile);
			
		}

	}
}
