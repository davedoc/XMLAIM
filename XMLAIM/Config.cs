using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

namespace XMLAIM
{
	class Config
	{
		string ConfigFile;
		private XmlDocument xDoc;
		private Hashtable reservedWords = new Hashtable();
		private Dictionary<string, string> signElements = new Dictionary<string, string>();
		
		public Config(string configFile)
		{
			ConfigFile = configFile;
			LoadConfig();
		}

		private void LoadConfig()
		{
			XmlTextReader reader = new XmlTextReader(ConfigFile);
			xDoc = new XmlDocument();
			xDoc.Load(reader);
			reader.Close();

			XmlElement root = xDoc.DocumentElement;

			//load reserved words
			XmlNodeList wordNodes = root.SelectNodes("/config/reservedwords/word");
			foreach (XmlNode wordNode in wordNodes)
			{
				reservedWords.Add(wordNode.InnerText.ToLower(), 1);
			}

			//load file signing details
			XmlNode signNode = root.SelectSingleNode("/config/filesigning");
			foreach (XmlNode node in signNode.ChildNodes)
			{
				string elementName = node.Name;
				string elementValue = node.InnerText;
				signElements.Add(elementName, elementValue);
				Console.WriteLine("cheese" + node.ToString());
			}
				
		}

		public string getSignValue(string name)
		{
			string value = signElements[name];

			return value;
		}

		public bool isReservedWord(string word)
		{
			bool isReserved = false;

			if (reservedWords.ContainsKey(word))
			{
				isReserved = true;
			}

			return isReserved;
		}
	}
}
