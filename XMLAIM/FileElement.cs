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
	class FileElement
	{
		public string stageAttribute;
		public string installAttribute;
		public string versionAttribute;
		public Install installElement;
		//public Hashtable installsHash = new Hashtable();
		public Dictionary<string, Shortcut> shortcuts = new Dictionary<string, Shortcut>();
		public bool useStaging = false;
		private int ComponentAttributes = 0;
		private string ComponentConditions;
		private int ISAttributes = 0;
		private XmlNode Node;

		//Depricated?
		public FileElement(string stage, string install, string version)
		{
			stageAttribute = stage;
			installAttribute = install;
			versionAttribute = version;
		}

		//Depricated?
		public FileElement(string stage, string install)
		{
			stageAttribute = stage;
			installAttribute = install;
		}

		public FileElement(XmlNode node, bool useStage)
		{
			Node = node;
			useStaging = useStage;
		}

		public void populateInstalls()
		{
			//TODO: need to change this hash to a dictionary
			Hashtable stageHash = new Hashtable();
			string Feature = Node.ParentNode.Attributes["id"].Value;
			string Version = "";
			string shortcutIcon = "";
			string shortcutName = "";
			string shortcutPath = "";
			string extractLocation = "";
			string extractSequence = "";
			string extractDelete = "";

			//TODO: if this condition is not used it need to be removed.
			string conditions = "";

			
			try
			{
				Version = Node.Attributes["version"].Value;
			}
			catch (Exception ex)
			{

			}
			
			try
			{
				XmlNode stageNode = Node.SelectSingleNode("./stage");
				
				//check to see if this is new format or old.
				if (stageNode != null)
				{
					//These are the steps to follow if it is the new format.

					//This loop is here for supporting multiple stage location which really does not make sense.
					//foreach (XmlNode stageNode in stageNodes)
					//{
						string stageLocation = stageNode.InnerText;

						stageLocation = Regex.Replace(stageLocation, @"\[", "<");
						stageLocation = Regex.Replace(stageLocation, @"\]", ">");

						//string stagePlatform = "";

						/*try
						{
							stagePlatform = stageNode.Attributes["plat"].Value;
						}
						catch (Exception ex)
						{

						}*/
						/*if (stageNodes.Count == 1 && stagePlatform == "")
						{
							stagePlatform = "ALL";
						}
						else
						{
							if (stagePlatform == "")
							{
								Console.WriteLine("Cant have mixture of plats and no plats!");
								throw new Exception("Cant have mixture of plats and no plats!");
							}
						}*/

						/*if (!stageHash.ContainsKey(stagePlatform))
						{
							stageHash.Add(stagePlatform, stage);
						}
						else
						{
							Console.WriteLine("Cant place a duplicate platform object!");
							throw new Exception("Cant place a duplicate platform object!");
						}*/
					//}

					XmlNode installNode = Node.SelectSingleNode("./install");

					//foreach (XmlNode installNode in installNodes)
					//{
						string install = installNode.InnerText;
						string installPlatform = "";
						

						foreach (XmlAttribute attrib in installNode.Attributes)
						{
							try
							{
								//string attribName = attrib.Name;
								//string attribValue = attrib.Value;
								switch (attrib.Name)
								{
									case "plat":
										installPlatform = attrib.Value;
										if (attrib.Value.ToUpper() == "X64")
										{
											ComponentAttributes = ComponentAttributes | 256;
										}
										break;
									case "bit64":
										if (attrib.Value.ToLower() == "true")
										{
											ComponentAttributes = ComponentAttributes | 256;
										}
										break;
									case "comext":
										if (attrib.Value.ToLower() == "true")
										{
											ISAttributes = ISAttributes + 3;
										}
										break;
									case "cond":
										if (ComponentConditions == null)
										{
											ComponentConditions = attrib.Value.ToString();
										}
										break;
									case "reevalcond":
										if (attrib.Value.ToLower() == "true")
										{
											ComponentAttributes = ComponentAttributes | 64;
										}
										break;
									case "permanent":
										if (attrib.Value.ToLower() == "true")
										{
											ComponentAttributes = ComponentAttributes | 16;
										}
										break;
									case "shortcutIcon":
										shortcutIcon = attrib.Value;
										shortcutIcon = Regex.Replace(shortcutIcon, @"\[", "<");
										shortcutIcon = Regex.Replace(shortcutIcon, @"\]", ">");
										shortcutIcon = Regex.Replace(shortcutIcon, @"\$INSTALL_ROOT", "[STAGING]");
										/*if (useStaging)
										{
											shortcutIcon = Regex.Replace(shortcutIcon, @"\<STAGING\>", "<STAGING>\\" + Feature);
										}*/

										break;
									case "shortcutName":
										shortcutName = attrib.Value;
										break;
									case "shortcutPath":
										shortcutPath = attrib.Value;
										shortcutIcon = Regex.Replace(shortcutIcon, @"\$STARTMENUE", "[ProgramFilesFolder]");
										//$STARTMENUE
										break;
									case "extractLocation":
										extractLocation = attrib.Value;
										extractLocation = Regex.Replace(extractLocation, @"\$INSTALL_ROOT", "[INSTALLDIR]");
										//extractLocation = Regex.Replace(extractLocation, @"\[.*\]", "");
										break;
									case "extractSequence":
										extractSequence = attrib.Value;
										break;
									case "extractDelete":
										extractDelete = attrib.Value;
										break;

								}
							}
							catch (Exception ex)
							{

							}
						}

						

						//There is a good chance that this installPlatform is not used anymore. need to do more testing to determine if this is the case.
						//Either way this whole section of code can be simplified.
						//One change that can probably be made is removing the use of the installHash or at least changing how it is used.
						if (installPlatform == "")
						{
							//plat = "ALL";
							//foreach (string platform in stageHash.Keys)
							//{
								//string stageLocation = stageHash[platform].ToString();
								if (useStaging)
								{
									stageLocation = install;
									//old format which include the bit magic should probably be removed as this was commented out.
									/*
									stageLocation = Regex.Replace(stageLocation, @"\$STAGE", "<STAGING>/" + Feature + platform);
									stageLocation = Regex.Replace(stageLocation, @"\[.*\]", "<STAGING>/" + Feature + platform);
									*/
									stageLocation = Regex.Replace(stageLocation, @"\$STAGE", "<STAGING>/" + Feature);
									stageLocation = Regex.Replace(stageLocation, @"\[.*\]", "<STAGING>/" + Feature);
									
									//stageLocation = Regex.Replace(stageLocation, @"\]", "&gt;");
								}
								/*if (platform != "ALL")
								{
									installsHash.Add(installsHash.Count + 1, new Install(stageLocation,
																							install,
																							platform,
																							Feature + platform,
																							Version,
																							ComponentAttributes,
																							ISAttributes,
																							ComponentConditions,
																							shortcutIcon,
																							shortcutName,
																							shortcutPath,
																							extractLocation,
																							extractSequence,
																							extractDelete));
								}*/
								//else
								//{
									installElement = new Install(stageLocation,
																							install,
																							installPlatform,
																							Feature,
																							Version,
																							ComponentAttributes,
																							ISAttributes,
																							ComponentConditions,
																							shortcutIcon,
																							shortcutName,
																							shortcutPath,
																							extractLocation,
																							extractSequence,
																							extractDelete);
								//}
							//}
						}
						else
						{
							//string stageLocation = stageHash[installPlatform].ToString();
							if (useStaging)
							{
								stageLocation = install;
								stageLocation = Regex.Replace(stageLocation, @"\$STAGE", "<STAGING>/" + Feature + installPlatform);
								stageLocation = Regex.Replace(stageLocation, @"\[.*\]", "<STAGING>/" + Feature + installPlatform);
									
								//stageLocation = Regex.Replace(stageLocation, @"\]", "&gt;");
							}
							installElement = new Install(stageLocation,
																							install,
																							installPlatform,
																							Feature + installPlatform,
																							Version,
																							ComponentAttributes,
																							ISAttributes,
																							ComponentConditions,
																							shortcutIcon,
																							shortcutName,
																							shortcutPath,
																							extractLocation,
																							extractSequence,
																							extractDelete);
						}

					//}

					//Parse Shortcut nodes
					try
					{
						XmlNodeList shortcutNodes = Node.SelectNodes("./shortcut");
						foreach (XmlNode shortcutNode in shortcutNodes)
						{
							XmlNode tempNode = shortcutNode.SelectSingleNode("./icon");
							shortcutIcon = tempNode.InnerText;

							tempNode = shortcutNode.SelectSingleNode("./name");
							shortcutName = tempNode.InnerText;
							//path
							tempNode = shortcutNode.SelectSingleNode("./path");
							shortcutPath = tempNode.InnerText;

							tempNode = shortcutNode.SelectSingleNode("./condition");
							string shortcutCondition = "";
							if (tempNode != null)
							{
								shortcutCondition = tempNode.InnerText;
							}
							tempNode = shortcutNode.SelectSingleNode("./type");
							string shortcutType = tempNode.InnerText;

							//with the changes to the manifest to support the refactor the use staging seciton needs to be removed
							/*if (useStaging)
							{
								shortcutIcon = Regex.Replace(shortcutIcon, @"\[.*\]", "<STAGING>/" + Feature);
							}
							else
							{*/
								shortcutIcon = Regex.Replace(shortcutIcon, @"\[.*\]", "<STAGING>");
							//}

							if (!shortcuts.ContainsKey(shortcutPath))
							{
								shortcuts.Add(shortcutPath, new Shortcut(shortcutIcon, shortcutName, shortcutPath, install, shortcutType, shortcutCondition));
							}
							else
							{
								Console.WriteLine("Value " + shortcutPath + " already exist.");
								throw new Exception("Value " + shortcutPath + " already exist.");
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Something went wrong while parsing shortcuts: " + ex.Message);
					}

				}
				else
				{
					string installLocation = "";
					string stageLocation = "";
					//XmlNodeList fileAttributeNodes = node.SelectNodes("./install");

					foreach (XmlAttribute attrib in Node.Attributes)
					{
						try
						{
							//string attribName = attrib.Name;
							//string attribValue = attrib.Value;
							switch (attrib.Name)
							{
								case "install":
									installLocation = attrib.Value;
									//required to support old style of manifest
									installLocation = Regex.Replace(installLocation, @"\$INSTALL_ROOT", "[INSTALLDIR]");
									break;
								case "stage":
									stageLocation = attrib.Value;
									//required to support old style of manifest
									stageLocation = Regex.Replace(stageLocation, @"\$STAGE", "[STAGING]");
									break;
								case "bit64":
									if (attrib.Value.ToLower() == "true")
									{
										ComponentAttributes = ComponentAttributes | 256;
									}
									break;
								case "comext":
									if (attrib.Value.ToLower() == "true")
									{
										ISAttributes = ISAttributes + 3;
									}
									break;
								case "cond":
									if (ComponentConditions == null)
									{
										ComponentConditions = attrib.Value.ToString();
									}
									break;
								case "reevalcond":
									if (attrib.Value.ToLower() == "true")
									{
										ComponentAttributes = ComponentAttributes | 64;
									}
									break;
								case "permanent":
									if (attrib.Value.ToLower() == "true")
									{
										ComponentAttributes = ComponentAttributes | 16;
									}
									break;
								case "shortcutIcon":
									shortcutIcon = attrib.Value;
									shortcutIcon = Regex.Replace(shortcutIcon, @"\$INSTALL_ROOT", "[STAGING]\\" + Feature);
									
									shortcutIcon = Regex.Replace(shortcutIcon, @"\[STAGING\]", "[STAGING]\\" + Feature);
									
									shortcutIcon = Regex.Replace(shortcutIcon, @"\[", "<");
									shortcutIcon = Regex.Replace(shortcutIcon, @"\]", ">");
									shortcutIcon = Regex.Replace(shortcutIcon, @"/", "\\");
									Console.WriteLine("shortcutIcon = " + shortcutIcon);

									break;
								case "shortcutName":
									shortcutName = attrib.Value;
									break;
								case "shortcutPath":
									shortcutPath = attrib.Value;
									shortcutPath = Regex.Replace(shortcutPath, @"\$STARTMENUE", "[BUSINESS_MASHUPS1]");
									break;
								case "extractLocation":
									extractLocation = attrib.Value;
									extractLocation = Regex.Replace(extractLocation, @"\$INSTALL_ROOT", "[INSTALLDIR]");
									//extractLocation = Regex.Replace(extractLocation, @"\[.*\]", "");
									break;
								case "extractSequence":
									extractSequence = attrib.Value;
									break;
								case "extractDelete":
									extractDelete = attrib.Value;
									break;

							}
						}
						catch (Exception ex)
						{

						}
					}

					if (stageLocation != "" && installLocation != "")
					{
						if (useStaging)
						{
							stageLocation = installLocation;
							stageLocation = Regex.Replace(stageLocation, @"\[.*\]", "<STAGING>\\" + Feature);
									
						}
						else
						{
							stageLocation = Regex.Replace(stageLocation, @"\[", "<");
							stageLocation = Regex.Replace(stageLocation, @"\]", ">");
						}
						installElement = new Install(stageLocation,
																				installLocation,
																				"",
																				Feature,
																				Version,
																				ComponentAttributes,
																				ISAttributes,
																				ComponentConditions,
																				shortcutIcon,
																				shortcutName,
																				shortcutPath,
																				extractLocation,
																				extractSequence,
																				extractDelete);
					}
					else
					{
						Console.WriteLine("File element requires stage and install be defined.");
					}
				}
			
				


			}
			catch (Exception ex)
			{
				Console.WriteLine("parseFiles Exeception: " + ex.Message);
			}
		}

		public void setComponentAttribute(Dictionary <string, string> attributes)
		{
			foreach (string key in attributes.Keys)
			{
				switch (key)
				{
					case "bit64":
						switch (attributes[key])
						{
							case "true":
								ComponentAttributes = ComponentAttributes | 256;
								break;
							case "false":
							default:
								int tempValue = ComponentAttributes & 256;
								if (tempValue == 256)
								{
									ComponentAttributes = ComponentAttributes | 256;
								}
								break;
						}
						break;
				}
			}
		}

		public void setConditions(string cond)
		{
			ComponentConditions = cond;
		}
	}
}
