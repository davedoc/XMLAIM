using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XMLAIM
{
	class GuidMapping
	{
		string mapFile;
		private Dictionary<string, string> mapping;
		public GuidMapping(string map)
		{
			mapFile = map;
			mapping = new Dictionary<string, string>();
		}

		public void loadMappedFile()
		{
			try
			{
				XmlTextReader reader = new XmlTextReader(mapFile);
				XmlDocument xDoc = new XmlDocument();
				xDoc.Load(reader);
				reader.Close();
				//xDoc.PreserveWhitespace = true;

				XmlElement xeRoot = xDoc.DocumentElement;

				XmlNodeList fileList = xeRoot.SelectNodes("/mapping/file");

				foreach (XmlNode node in fileList)
				{
					Console.WriteLine(node.InnerText.ToString());

					XmlNode installNode = node.SelectSingleNode("./install");
					XmlNode guidNode = node.SelectSingleNode("./guid");
					XmlNode featureNode = node.SelectSingleNode("./feature");

					string key = featureNode.InnerText + "_" + installNode.InnerText;
					if (mapping.ContainsKey(key) == true)
					{
						//Console.WriteLine("cheese");
					}
					mapping.Add(key, guidNode.InnerText);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Could not load Mapped File: " + ex.Message);
			}
		}

		public string getGuid(string feature, string component)
		{
			string guid = null;
			string key = feature + "_" + component;

			if (mapping.ContainsKey(key))
			{
				guid = mapping[key];
			}

			return guid;
		}
	}
}
