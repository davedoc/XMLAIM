using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class stringOperator
	{
		public string brandingXML;
		XmlDocument xDoc;
		public Dictionary <int, StringElement> Strings;
		string Language;

		public stringOperator(string branding, string Lang)
		{
			brandingXML = branding;
			Language = Lang;
			Strings = new Dictionary<int,StringElement>();
		}

		public void processBranding()
		{
			XmlTextReader reader = new XmlTextReader(brandingXML);
			xDoc = new XmlDocument();
			xDoc.Load(reader);
			reader.Close();

			XmlElement root = xDoc.DocumentElement;

			XmlNodeList stringNodes = root.SelectNodes("/Branding/Install/*");

			foreach (XmlNode node in stringNodes)
			{
				string Key = node.Name.ToString();
				string Value = node.InnerText.ToString();

				string pattern = @"\$\{(.*)\}";

				Match match = Regex.Match(Value, pattern);
				while (match.Success)
				{
					XmlNode findNode = root.SelectSingleNode(match.Groups["1"].Value);
					string tempValue = findNode.InnerText.ToString();


					string localPattern = @"\$\{" + match.Groups["1"].Value + @"\}";
					Value = Regex.Replace(Value, localPattern, tempValue);
					/*Match localMatch = Regex.Match(Value, localPattern);
					if (localMatch.Success)
					{
						Value
					}*/
					//Value = findNode.InnerText.ToString();
					match = match.NextMatch();
				}
				//${/Branding/CompanyName}
				//Match match = Regex.Match(Value, pattern);
				/*if (match.Success)
				{
					pattern = Value;
					pattern = Regex.Replace(pattern, @"\$", @"\$");
					pattern = Regex.Replace(pattern, @"\{", @"\{(");
					pattern = Regex.Replace(pattern, @"\}", @")\}");
					//pattern = @"(" + Value + ")";
					match = Regex.Match(Value, pattern);

					if (match.Success)
					{
						XmlNode findNode = root.SelectSingleNode(match.Groups["1"].Value);
						Value = findNode.InnerText.ToString();

					}
				}*/

				StringElement se = new StringElement(Key, Language, Value, "0", "", "");

				Strings.Add(Strings.Count + 1, se);

				Console.WriteLine(Key + " " + Value);
			}

		}

		public void populateStrings( IsmDatabase iDb)
		{
			
		}
	}
}
