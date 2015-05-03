using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class Manifest
	{
		private string XMLPath;
		public Hashtable featureTable = new Hashtable();
		public Hashtable fileElements = new Hashtable();
		public Hashtable deleteElements = new Hashtable();
		public Hashtable backupElements = new Hashtable();
		public Dictionary<int, PropertyElement> propertyElements = new Dictionary<int, PropertyElement>();
		public Dictionary<int, String> signElements = new Dictionary<int, string>();
		public Dictionary<string, string> variableElement = new Dictionary<string, string>();
		public string temp;
		public AIMLogger logger;
		public int iFileCount;
		XmlDocument xDoc;
		XmlElement root;
		bool useStaging;

		public Manifest(string xml)
		{
			//logger = log;
			XMLPath = xml;

			XmlTextReader reader = new XmlTextReader(XMLPath);
			xDoc = new XmlDocument();
			xDoc.Load(reader);
			reader.Close();

			root = xDoc.DocumentElement;
			

			useStaging = false;

			iFileCount = 0;
		}

		public Manifest(string xml, bool useStage)
		{
			XMLPath = xml;

			XmlTextReader reader = new XmlTextReader(XMLPath);
			xDoc = new XmlDocument();
			xDoc.Load(reader);
			reader.Close();

			root = xDoc.DocumentElement;
			//parseEnvironmentVariables(root);
			useStaging = useStage;
			iFileCount = 0;
		}

		private string parseEnvironmentVariables(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				Regex regex = new Regex(@"(\$\{env\:)(.*)(\})", RegexOptions.IgnoreCase);
				MatchCollection mcoll = regex.Matches(text);
				if (mcoll.Count > 0)
				{
					foreach (Match match in mcoll)
					{
						string envvarname = match.Groups[2].Value;
						string envvarval = System.Environment.GetEnvironmentVariable(envvarname, EnvironmentVariableTarget.User);
						if (string.IsNullOrEmpty(envvarval))
							envvarval = System.Environment.GetEnvironmentVariable(envvarname, EnvironmentVariableTarget.Process);
						if (string.IsNullOrEmpty(envvarval))
							envvarval = System.Environment.GetEnvironmentVariable(envvarname, EnvironmentVariableTarget.Machine);
						if (!string.IsNullOrEmpty(envvarval))
						{
							string matchPattern = @"\$\{env\:" + envvarname + @"\}";
							text = Regex.Replace(text, matchPattern, envvarval);
						}
						else
						{
							
						}
						
					}
				}
				regex = new Regex(@"(\$\{)(.*)(\})", RegexOptions.IgnoreCase);
				mcoll = regex.Matches(text);
				if (mcoll.Count > 0)
				{
					foreach (Match match in mcoll)
					{
						string varname = match.Groups[2].Value;
						if (variableElement.ContainsKey(varname))
						{
							string varvalue = variableElement[varname];
							if (!string.IsNullOrEmpty(varvalue))
							{
								string matchPattern = @"\$\{" + varname + @"\}";
								text = Regex.Replace(text, matchPattern, varvalue);
							}
						}
						
					}
				}
			}

			return text;
		}

		private void parseEnvironmentVariables(XmlElement node)
		{

			if (node == null)
				return;

			if (node.NodeType == XmlNodeType.Element)
			{
				//string temp = node.;

				if (node.Value != null)
				{
					// Expand environment variables in the node text
					node.Value = parseEnvironmentVariables(node.Value);
				}

				//Used to swap out characters that are not supported in xml. This probably needs to be a list of characters to swap out.
				string tempString = node.InnerXml.Replace("&amp;", "&");
				if (node.InnerText == tempString)
				{
					node.InnerText = parseEnvironmentVariables(node.InnerText);
				}
				
				// Expand environment variables in all node attributes
				XmlAttributeCollection attributes = node.Attributes;
				if (attributes != null && attributes.Count > 0)
				{
					foreach (XmlAttribute attribute in attributes)
					{
						attribute.Value = parseEnvironmentVariables(attribute.Value);
					}
				}

				// Expand environment variables in all node children elements
				XmlNodeList childrenNodeList = node.ChildNodes;
				if (childrenNodeList.Count > 0)
				{
					foreach (XmlNode childNode in childrenNodeList)
					{
						if (childNode.NodeType == XmlNodeType.Element)
						{
							parseEnvironmentVariables((XmlElement)childNode);
						}
					}
				}
			}
		}

		public void parseIncludes(XmlElement root, string parentPath)
		{
			//XPathExpression xExpr;
			//xExpr = xNav.Compile("/manifest/include");
			//XPathNodeIterator xIterator = xNav.Select(xExpr);


			try
			{
				XmlNodeList IncludeNodes = root.SelectNodes("/manifest/include");
				foreach (XmlNode IncludeNode in IncludeNodes)
				{
					//stageRootDir = Regex.Replace(stageRootDir, @"&lt;.*&gt;", srcRoot);
					string includePath = Path.GetDirectoryName(parentPath) + "\\" + Regex.Replace(IncludeNode.Attributes["manifest"].Value, @"/", "\\");
					Console.WriteLine("Parsing include : " + includePath);
					XmlTextReader reader = new XmlTextReader(includePath);
					XmlDocument lDoc = new XmlDocument();
					lDoc.Load(reader);
					reader.Close();

					XmlElement lRoot = lDoc.DocumentElement;
					parseVariables(lRoot);
					parseEnvironmentVariables(lRoot);
					//XPathDocument xDoc = new XPathDocument(includePath);
					//XPathNavigator xNav2 = xDoc.CreateNavigator();
					
					parseIncludes(lRoot, includePath);
					parseFeatures2(lRoot);
					parseDeletes(lRoot);
					parseBackups(lRoot);

					parseProperties(lRoot);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error while parsing includes: " + ex.Message);
				throw ex;
				//logger.log("parseIncludes Exeception: " + ex.Message);
			}
		}

		public void parseBackups(XmlElement xRoot)
		{
			XmlNodeList backupNodes = xRoot.SelectNodes("/manifest/feature/backup");
			try
			{
				foreach (XmlNode node in backupNodes)
				{
					BackupElement de = new BackupElement(node);
					backupElements.Add(backupElements.Count + 1, de);
				}
			}
			catch (Exception ex)
			{
				//logger.log("loadManifest Exeception: " + ex.Message);
			}
		}

		public void parseDeletes(XmlElement xRoot)
		{
			XmlNodeList deleteNodes = xRoot.SelectNodes("/manifest/feature/delete");
			try
			{
				foreach (XmlNode node in deleteNodes)
				{
					DeleteElement de = new DeleteElement(node);
					deleteElements.Add(deleteElements.Count + 1, de);
				}
			}
			catch (Exception ex)
			{
				//logger.log("loadManifest Exeception: " + ex.Message);
			}

		}

		public void parseProperties(XmlElement xRoot)
		{
			XmlNodeList propertyNodes = xRoot.SelectNodes("/manifest/property");
			try
			{
				foreach (XmlNode node in propertyNodes)
				{
					PropertyElement pe = new PropertyElement(node);
					propertyElements.Add(propertyElements.Count + 1, pe);
				}
			}
			catch (Exception ex)
			{
				//logger.log("loadManifest Exeception: " + ex.Message);
			}
		}

		public void parseVariables(XmlElement xRoot)
		{
			XmlNodeList varNodes = xRoot.SelectNodes("/manifest/var");
			try
			{
				foreach (XmlNode node in varNodes)
				{

					string Var = node.Attributes["id"].Value;
					string Value = parseEnvironmentVariables( node.Attributes["value"].Value );
					//VariableElement ve = new VariableElement(node);
					//VariableElements.A .Add(propertyElements.Count + 1, pe);
					variableElement.Add(Var, Value);
				}
			}
			catch (Exception ex)
			{
				//logger.log("loadManifest Exeception: " + ex.Message);
			}
		}

		public void parseFeatures2(XmlElement froot)
		{
			//XPathExpression xExpr;
			//xExpr = xNav.Compile("/manifest/feature/file");
			//XPathNodeIterator xIterator = xNav.Select(xExpr);


			try
			{
				XmlNodeList featureNodes = froot.SelectNodes("/manifest/feature");

				foreach (XmlNode fnode in featureNodes)
				{
					Dictionary<string, string> attributes = new Dictionary<string, string>();
					//through this mechanism we can add attributes to features elements.
					string feature = fnode.Attributes["id"].Value;
					string componentConditions = null;
					foreach (XmlAttribute attrib in fnode.Attributes)
					{

						switch (attrib.Name)
						{
							case "bit64":
								attributes.Add(attrib.Name, attrib.Value);
								break;
							case "cond":
								componentConditions = attrib.Value;
								break;
						}

					}
					XmlNodeList fileNodes = froot.SelectNodes("/manifest/feature[@id='" + feature + "']/file");
					foreach (XmlNode node in fileNodes)
					{

						FileElement fe = new FileElement(node, useStaging);
						fe.setComponentAttribute(attributes);
						if (componentConditions != null)
						{
							fe.setConditions(componentConditions);
						}
						fe.populateInstalls();

						//int count = 
						fileElements.Add(fileElements.Count + 1, fe);

						iFileCount = iFileCount + 1;

						foreach (XmlAttribute attrib in node.Attributes)
						{

							switch (attrib.Name)
							{
								case "signfile":

									//foreach (int i in fe.installsHash.Keys)
									//{
									Install installTemp = (Install)fe.installElement;
									signElements.Add(signElements.Count + 1, installTemp.stagePath);
									//}

									//signElements.Add(signElements.Count + 1, 
									//attributes.Add(attrib.Name, attrib.Value);
									break;
							}

						}
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occured while parsing manifest: " + ex.Message);
				throw ex;
				//logger.log("loadManifest Exeception: " + ex.Message);
			}
		}

		/*public void parseFeatures(XPathNavigator xNav)
		{
			XPathExpression xExpr;
			xExpr = xNav.Compile("/manifest/feature");
			XPathNodeIterator xIterator = xNav.Select(xExpr);

			try
			{
				while (xIterator.MoveNext())
				{
					XPathNavigator nav2 = xIterator.Current.Clone();

					//temp = temp + "\n" + nav2.GetAttribute("id", "");
					string featureId = nav2.GetAttribute("id", "");
					//Main.updateLogField("Adding feature: " + featureId);
					FeatureElement newFeature = new FeatureElement(featureId);
					newFeature.parseFiles(nav2);

					if (featureTable.Contains(featureId) == false)
					{
						featureTable.Add(featureId, newFeature);
						temp = temp + newFeature.Temp;
					}
				}
			}
			catch (Exception ex)
			{
				//logger.log("loadManifest Exeception: " + ex.Message);
			}
		}*/

		public void loadManifest()
		{
			//path = XmlPath;

			//XPathDocument xDoc = new XPathDocument(path);
			//XPathNavigator xNav = xDoc.CreateNavigator();
			try
			{
				parseVariables(root);

				parseEnvironmentVariables(root);

				parseIncludes(root, XMLPath);
				parseFeatures2(root);
				parseDeletes(root);
				parseBackups(root);

				parseProperties(root);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error while trying to load manifest: " + ex.Message);
				throw ex;
			}

		}
	}
}
