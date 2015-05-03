using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class IsmTable
	{
		private XmlDocument xDoc;
		private Config conf;
		private int iFilesProcessed;
		private int iFilesToProcess;
		private IsmDatabase ismDb;
		//private XPathNavigator xNav;
		//private string ismFile;
		private GuidMapping Map;
		private string Module;

		public IsmTable(XmlDocument doc, int fileCount, IsmDatabase Db)
		{
			xDoc = doc;
			string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			Console.WriteLine(path);
			conf = new Config(path + "\\config.xml");
			iFilesProcessed = 0;
			iFilesToProcess = fileCount;
			ismDb = Db;
			//ismFile = File;
		}

		public IsmTable(IsmDatabase Db)
		{
			ismDb = Db;
		}

		public void AddMap(GuidMapping map)
		{
			Map = map;
		}

		public void AddModule(string module)
		{
			Module = module;
		}

		public void UpdateStrings(stringOperator so, string moduleString)
		{
			
			//for the base and each element in the moduleString we will need to do the following loop
			//changing the ISString value.
			foreach (int i in so.Strings.Keys)
			{
				StringElement se = so.Strings[i];
				string keyString = se.ISString;
				if (moduleString != "")
				{
					keyString += moduleString;
				}
				IsmRow Row = ismDb.getRow("ISString", keyString + "," + se.ISLanguage);
				if (Row != null)
				{
					Row.Row["Value"] = se.Value;
				}
				else
				{
					IsmRow StringRow = new IsmRow();

					StringRow.Row.Add("ISString", keyString);
					StringRow.Row.Add("ISLanguage_", se.ISLanguage);
					StringRow.Row.Add("Value", se.Value);
					StringRow.Row.Add("Encoded", "0");
					StringRow.Row.Add("Comment", "Added through XMLAIM");
					StringRow.Row.Add("TimeStamp", "");


					ismDb.addRow("ISString", StringRow);
				}
			}

			/*char[] splitstring = {','};
			foreach (string s in moduleString.Split(splitstring))
			{
				foreach (int i in so.Strings.Keys)
				{
					StringElement se = so.Strings[i];
					IsmRow Row = ismDb.getRow("ISString", se.ISString + s + "," + se.ISLanguage);
					if (Row != null)
					{
						Row.Row["Value"] = se.Value;
					}
					else
					{
						IsmRow StringRow = new IsmRow();

						StringRow.Row.Add("ISString", se.ISString + s);
						StringRow.Row.Add("ISLanguage_", se.ISLanguage);
						StringRow.Row.Add("Value", se.Value);
						StringRow.Row.Add("Encoded", "0");
						StringRow.Row.Add("Comment", "Added through XMLAIM");
						StringRow.Row.Add("TimeStamp", "");


						ismDb.addRow("ISString", StringRow);
					}
				}
			}*/
			//so.Strings
		}

		public void UpdateFileTable(string file)
		{
			
		}

		public void AddDeletes(XmlDocument Doc, DeleteElement de)
		{
			XmlElement root = Doc.DocumentElement;

			foreach (int i in de.deleteHash.Keys)
			{
				/*IsmRow Row = new IsmRow();

				Row.Row.Add("UID", count.ToString());
				Row.Row.Add("InstallLocation", inst.installPath);
				Row.Row.Add("ExtractLocation", inst.extractLocation);
				Row.Row.Add("Sequence", inst.extractSequence);
				Row.Row.Add("Delete", inst.extractDelete);
				Row.Row.Add("ComponentName", component);

				ismDb.addRow("BackupTable", Row);*/
				//XmlNode updateNode = root.SelectSingleNode("/msi/table[@name='ExtractWar']");

				/*XmlElement newRow = xDoc.CreateElement("row");
				
				newRow.InnerXml = "<td>" + count + "</td>" + //UID
									"<td>" + inst.installPath + "</td>" + //InstallLocation
									"<td>" + inst.extractLocation + "</td>" + //ExtractLocation
									"<td>" + inst.extractSequence + "</td>" + //Sequence
									"<td>" + inst.extractDelete + "</td>" + //Delete
									"<td>" + component + "</td>"; //ComponentName
				updateNode.AppendChild((XmlNode)newRow);

				return inst.installPath;
				 * */
			}

		}

		public void AddProperties(PropertyElement pe)
		{

			IsmRow fetchRow = ismDb.getRow("Property", pe.Property);
			if (fetchRow != null)
			{
				fetchRow.Row["Value"] = pe.Value;
			}
			else
			{
				IsmRow Row = new IsmRow();

				Row.Row.Add("Property", pe.Property);
				Row.Row.Add("Value", pe.Value);
				Row.Row.Add("ISComments", "");

				ismDb.addRow("Property", Row);
			}

		}

		public void AddBackups(XmlDocument Doc, BackupElement be)
		{
			XmlElement root = Doc.DocumentElement;
			//XmlNode fileNode = root.SelectSingleNode("/msi/table[@name='BackupTable']");

			foreach (int i in be.BackupHash.Keys)
			{
				Backup back = (Backup) be.BackupHash[i];

				string installLocation = back.installLocation;
				string upgradeFrom = back.upgradeFrom;

				int count = 1;
				bool notFound = true;
				while (notFound)
				{

					//Hashtable checkHash = parseTableRow(root, "BackupTable", count.ToString(), "UID");
					IsmRow checkRow = ismDb.getRow("BackupTable", count.ToString());

					if (checkRow != null)
					{
						if (upgradeFrom == checkRow.Row["UpgradeFrom"] && installLocation == checkRow.Row["FileName"])
						{
							notFound = false;
						}
						else
						{
							count++;
						}
					}
					else
					{
						//XmlNode updateNode = root.SelectSingleNode("/msi/table[@name='BackupTable']");

						//XmlElement newFile = xDoc.CreateElement("row");
						/*<col key="yes" def="s10">UID</col>
						<col def="s255">FileName</col>
						<col def="s255">UpgradeFrom</col>*/
						IsmRow Row = new IsmRow();

						Row.Row.Add("UID", count.ToString());
						Row.Row.Add("FileName", installLocation);
						Row.Row.Add("UpgradeFrom", upgradeFrom);

						ismDb.addRow("BackupTable", Row);
						notFound = false;
					}
				}
			}
		}

		public void AddFileVersion(string component, string version)
		{
			IsmRow checkRow = ismDb.getRow("PatchFileVersion", component);

			if (checkRow != null)
			{
				checkRow.Row["Version"] = version;
			}
			else
			{
				IsmRow Row = new IsmRow();
				Row.Row.Add("Component", component);
				Row.Row.Add("Version", version);

				ismDb.addRow("PatchFileVersion", Row);
			}
		}

		public void ExportGuids(string output)
		{
			Dictionary<string, IsmRow> ComponentTable = ismDb.getRows("Component");

			XmlDocument xmlDoc = new XmlDocument();
			XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
			XmlElement rootNode = xmlDoc.CreateElement("mapping");
			xmlDoc.InsertBefore(xmlDec, xmlDoc.DocumentElement);
			xmlDoc.AppendChild(rootNode);

			foreach (string componentId in ComponentTable.Keys)
			{
				//XmlElement featureNode = xmlDoc.CreateElement("feature");
				

				IsmRow ComponentRow = ComponentTable[componentId];
				IsmRow FileRow = ismDb.getRow("File", componentId);
				IsmRow FeatureRow = ismDb.getRow("FeatureComponents", componentId);
				//things needed to do comparisson:
				//install path
				//guid
				//
				if (FileRow != null)
				{
					string installPath = determineDirectoryPath(ComponentRow.Row["Directory_"]);
					string componentGuid = ComponentRow.Row["ComponentId"];
					string fileName = FileRow.Row["FileName"];
					string featureName = FeatureRow.Row["Feature_"];

					fileName = Regex.Replace(fileName, @".*\|", @"");

					XmlElement fileNode = xmlDoc.CreateElement("file");

					XmlElement installNode = xmlDoc.CreateElement("install");
					XmlText installText = xmlDoc.CreateTextNode(installPath + "\\" + fileName);
					installNode.AppendChild(installText);

					XmlElement guidNode = xmlDoc.CreateElement("guid");
					XmlText guidText = xmlDoc.CreateTextNode(componentGuid);
					guidNode.AppendChild(guidText);

					XmlElement featureNode = xmlDoc.CreateElement("feature");
					XmlText featureText = xmlDoc.CreateTextNode(featureName);
					featureNode.AppendChild(featureText);

					fileNode.AppendChild(installNode);
					fileNode.AppendChild(guidNode);
					fileNode.AppendChild(featureNode);

					rootNode.AppendChild(fileNode);
				}

				//File.Add("InstallPath", installPath);
				//File.Add("Guid", componentGuid);

				//Components.Add(componentId, File);
			}

			xmlDoc.Save(output);
		}

		public void ExportIsm()
		{

			//feature --> component --> file
			Dictionary<string, Dictionary<string, Dictionary<string, string>>> featureElements = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
			Dictionary<string, IsmRow> ComponentTable = ismDb.getRows("Component");
			//for each component I need to determine which feature it belongs to
			foreach (string key in ComponentTable.Keys)
			{
				Dictionary<string, string> fileElement = new Dictionary<string,string>();
				Dictionary<string, Dictionary<string, string>> componentElement = new Dictionary<string, Dictionary<string, string>>();
				
				IsmRow ComponentRow = ComponentTable[key];
				string Component = ComponentRow.Row["Component"];
				string ComponentGuid = ComponentRow.Row["ComponentId"];
				//determine the feature for the component
				IsmRow FeatureComponentsRow = ismDb.getRow("FeatureComponents", Component);
				string Feature = FeatureComponentsRow.Row["Feature_"];
				/*if (!featureElements.ContainsKey(Feature))
				{
					featureElements.Add(Feature, fileElement);
				}*/
				
				//Things I need for the manifest:
				//file
				//install path
				//stage path
				//shortcut path
				//shortcut Icon path
				//component guid
				//
				//Get file row associated with component
				string File;
				string FileName;
				IsmRow FileRow = ismDb.getRow("File", Component);
				if (FileRow != null)
				{
					File = FileRow.Row["File"];
					FileName = FileRow.Row["FileName"];

					FileName = Regex.Replace(FileName, @".*\|", @"");

					//IsmRow ComponentRow = ismDb.getRow("Component", FileRow.Row["Component_"]);
					string installPath = determineDirectoryPath(ComponentRow.Row["Directory_"]);

					installPath = Regex.Replace(installPath, @".*\|", @"");

					string stagePath = FileRow.Row["ISBuildSourcePath"];
					stagePath = Regex.Replace(stagePath, @"<", @"[");
					stagePath = Regex.Replace(stagePath, @">", @"]");

					fileElement.Add("File", FileName);
					fileElement.Add("InstallPath", installPath);
					fileElement.Add("StagePath", stagePath);
					fileElement.Add("Guid", ComponentGuid);

					

					if (featureElements.ContainsKey(Feature))
					{
						featureElements[Feature].Add(File, fileElement);
					}
					else
					{
						componentElement.Add(File, fileElement);
						featureElements.Add(Feature, componentElement);
					}
					

					
					//featureElements[Feature].Add(
				}

				//featureElements[""]
 



			}

			//create the xml document

			XmlDocument xmlDoc = new XmlDocument();
			XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
			XmlElement rootNode = xmlDoc.CreateElement("manifest");
			xmlDoc.InsertBefore(xmlDec, xmlDoc.DocumentElement);

			foreach (string feature in featureElements.Keys)
			{
				
				

				xmlDoc.AppendChild(rootNode);

				XmlElement featureNode = xmlDoc.CreateElement("feature");
				featureNode.SetAttribute("id", feature);
				//xmlDoc.DocumentElement.PrependChild(featureNode);
				Dictionary<string, Dictionary<string, string>> componentElement = new Dictionary<string, Dictionary<string, string>>();

				componentElement = featureElements[feature];

				foreach (string component in componentElement.Keys)
				{

					Dictionary<string, string> fileElement = new Dictionary<string, string>();

					fileElement = componentElement[component];
					XmlElement fileNode = xmlDoc.CreateElement("file");

					XmlElement stageNode = xmlDoc.CreateElement("stage");
					XmlText stageText = xmlDoc.CreateTextNode(fileElement["StagePath"]);
					stageNode.AppendChild(stageText);

					XmlElement installNode = xmlDoc.CreateElement("install");
					XmlText installText = xmlDoc.CreateTextNode(fileElement["InstallPath"]+ "\\" + fileElement["File"]);
					installNode.AppendChild(installText);

					//installNode.SetAttribute("guid", fileElement["Guid"]);

					//XmlElement guidNode = xmlDoc.CreateElement("guid");
					//XmlText guidText = xmlDoc.CreateTextNode(fileElement["Guid"]);
					//guidNode.AppendChild(guidText);

					fileNode.AppendChild(stageNode);
					fileNode.AppendChild(installNode);
					//fileNode.AppendChild(guidNode);

					featureNode.AppendChild(fileNode);
				}

				rootNode.AppendChild(featureNode);
				
			}
			//need to update this to use the manifest value not this hard coded path.
			xmlDoc.Save("D:\\cm\\BM10R1\\Suite\\REG\\ClientInstall\\Manifest.xml");
		}

		public string determineDirectoryPath(string directoryKey)
		{
			string path = "";
			string tempPath = "";
			IsmRow DirectoryRow = ismDb.getRow("Directory", directoryKey);
			if (DirectoryRow != null)
			{
				if (DirectoryRow.Row["DefaultDir"] == "." || DirectoryRow.Row["Directory_Parent"] == "ProgramFilesFolder" || DirectoryRow.Row["Directory_Parent"] == "ProgramMenuFolder")
				{
					path = "[" + DirectoryRow.Row["Directory"] + "]";
					//path = tempPath;
				}
				else
				{
					tempPath = determineDirectoryPath(DirectoryRow.Row["Directory_Parent"]);
					string defaultdir = Regex.Replace(DirectoryRow.Row["DefaultDir"], @".*\|", @"");
					path = tempPath + "\\" + defaultdir;
				}	
			}
			return path;
		}

		public void AddFiles(XmlDocument xDoc, FileElement fe)
		{
			XmlElement root = xDoc.DocumentElement;
			
			//This loop is required due to the way we invisioned supporting multiple files and or destinations per file node. If
			//this support was removed this loop would probably not be neccessary.
			//foreach (int i in fe.installsHash.Keys)
			//{
				Install inst = (Install)fe.installElement;
				//<row><td>_a_action.xml</td><td>_a_action.xml</td><td>_A_ACTION.xml</td><td>0</td><td>09.03.00343</td><td/><td/><td>1</td><td>&lt;STAGING&gt;\Manager\Common\jboss405\server\default\deploy\mashupmgr.war\backbase\3.3.1dev\tools\reference\reference\attribute\_A_ACTION.xml</td><td>17</td><td/></row>
				//Boolean notUnique = true;
				string fileName = inst.installFileName;
				string version = inst.Version;
				string staging = inst.stagePath;
				//while (notUnique)
				//{
				iFilesProcessed++;

				Console.WriteLine("Processing " + iFilesProcessed + " of " + iFilesToProcess + ":" + inst.installPath);

				string component = parseComponents(root, inst);

				if (UtilityCore.getVersionMode == UtilityCore.VersionMode.On)
				{
					AddFileVersion(component, inst.Version);
				}
				//Hashtable fileRow = parseTableRow(root, "File", component, "File");
				IsmRow fileRow = ismDb.getRow("File", component);
				//if duplicate file check version if version is great update

				if (fileRow != null)
				{
					//if (component == fileRow["File"].ToString())
					//{
					//as we are removing support for this option I will remove this for now.
					//This removal is related to the issue we have with major upgrade. 2/21/2011
					/*if (isVersionGreater(fileRow.Row["Version"], version))
					{
						updateFilePatch(component, version);
						updateComponentPatch(component);
					}
					else
					{
						//duplicate file no need to update.
					}*/
					//}
				}
				else
				{
					if (UtilityCore.getMergeModuleMode == UtilityCore.MergeModuleMode.Off)
					{
						createFeatureComponent(root, inst.Feature, component);
					}
					//string tempStaging = Regex.Replace(staging, @"\[", "&lt;");
					//tempStaging = Regex.Replace(tempStaging, @"\]", "&gt;");

					createFile(root, component, component, fileName, "0", version, "", "", "1", staging, "1", "");
				}

				if (inst.shortcutName != "" && inst.shortcutPath != "")
				{
					string shortcutName = addShortcut(root, inst, component);
				}
				if (inst.extractLocation != "")
				{
					string extractValue = addExtractWar(root, inst, component);
				}

				//This will only work if there is a single file element which multiple need to be removed.
				//Add shortcuts new way
				if (fe.shortcuts.Count > 0)
				{
					foreach (string key in fe.shortcuts.Keys)
					{
						if (fe.shortcuts[key].Type.ToLower() == "primary")
						{
							string shortcutName = addShortcut(root, fe.shortcuts[key], component);
						}
						else
						{
							/*XmlElement root,string Component, string ComponentId, string Directory_,
			string Attributes, string Condition, string KeyPath, string ISAttributes, string ISComments, string ISScanAtBuildFile,
			string ISRegFileToMergeAtBuild*/
							string guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
							string Directory = parseDirectories(root, fe.shortcuts[key].Path );

							string componentName = component + fe.shortcuts[key].Name;

							componentName = Regex.Replace(componentName, @"#", @"");

							if (componentName.Length > 65)
							{
								int halfNameLength = componentName.Length / 2;
								int amountOver = componentName.Length - 65;
								int remove = amountOver / 2;
								string startName = componentName.Substring(0, halfNameLength - remove);
								string endName = componentName.Substring(halfNameLength + remove);
								componentName = startName + "88" + endName;
							}

							component = createComponent(root,
														componentName, 
														guid, 
														Directory, 
														"0", 
														fe.shortcuts[key].Condition, 
														"", 
														"0", 
														"AIM Component", "", "");							
							string shortcutName = addShortcut(root, fe.shortcuts[key], component);
						}
					}
				}

			//}
			
		}

		public string addExtractWar(XmlElement root, Install inst, string component)
		{
			//string ExtractWarName = "";
			int count = 0;

			IsmRow ExtractWarRow = ismDb.getRow("ExtractWar", component);

			if (ExtractWarRow != null)
			{
				return component;
			}
			else
			{
				string installLocation = inst.installPath;

				IsmRow Row = new IsmRow();

				//Row.Row.Add("UID", count.ToString());
				Row.Row.Add("ComponentName", component);
				Row.Row.Add("InstallLocation", installLocation);
				Row.Row.Add("ExtractLocation", inst.extractLocation);
				Row.Row.Add("Sequence", inst.extractSequence);
				Row.Row.Add("Delete", inst.extractDelete);

				ismDb.addRow("ExtractWar", Row);

				return component;

			}
		}

		public string addShortcut(XmlElement root, Shortcut shortcut, string component)
		{
			//temp 
			//string shortCutNameUnique = "";

			string Directory = parseDirectories(root, shortcut.Path);

			string shortCutIconName = "";
			if (shortcut.Icon != "")
			{
				shortCutIconName = addShortcutIcon(root, shortcut);
			}
			string shortCutNameUnique = shortcut.Name;
			bool notFound = true;
			int count = 1;

			while (notFound)
			{
				//Hashtable shortcutHash = parseTableRow(root, "Shortcut", shortCutNameUnique, "Shortcut");
				IsmRow shortcutRow = ismDb.getRow("Shortcut", shortCutNameUnique);
				if (shortcutRow != null)
				{
					//determine if the shortcut already exist in the database
					//compaire stagepath of target with
					if (component == shortcutRow.Row["Component_"] && shortcutRow.Row["Directory_"] == Directory)
					{
						return shortcut.Name;
					}
					else
					{
						shortCutNameUnique = shortcut.Name + count;
						count++;
					}
				}
				else
				{
					IsmRow Row = new IsmRow();

					Row.Row.Add("Shortcut", shortCutNameUnique);
					Row.Row.Add("Directory_", Directory);
					Row.Row.Add("Name", shortcut.Name);
					Row.Row.Add("Component_", component);
					Row.Row.Add("Target", shortcut.Target);
					Row.Row.Add("Arguments", "");
					Row.Row.Add("Description", "");
					Row.Row.Add("Hotkey", "");
					Row.Row.Add("Icon_", shortCutIconName);
					Row.Row.Add("IconIndex", "");
					Row.Row.Add("ShowCmd", "");
					Row.Row.Add("WkDir", "");
					Row.Row.Add("DisplayResourceDLL", "");
					Row.Row.Add("DisplayResourceId", "");
					Row.Row.Add("DescriptionResourceDLL", "");
					Row.Row.Add("DescriptionResourceId", "");
					Row.Row.Add("ISComments", "");
					Row.Row.Add("ISShortcutName", "");
					Row.Row.Add("ISAttributes", "");

					ismDb.addRow("Shortcut", Row);

					return shortCutNameUnique;
				}
			}
			return shortCutNameUnique;
		}

		public string addShortcut(XmlElement root, Install inst, string component)
		{
			string Directory = parseDirectories(root, inst.shortcutPath);

			string shortCutIconName = "";
			if (inst.shortcutIcon != "")
			{
				shortCutIconName = addShortcutIcon(root, inst);
			}
			string shortCutNameUnique = inst.shortcutName;
			bool notFound = true;
			int count = 1;

			while (notFound)
			{
				//Hashtable shortcutHash = parseTableRow(root, "Shortcut", shortCutNameUnique, "Shortcut");
				IsmRow shortcutRow = ismDb.getRow("Shortcut", shortCutNameUnique);
				if (shortcutRow != null)
				{
					//determine if the shortcut already exist in the database
					//compaire stagepath of target with
					//string instpath = Regex.Replace(inst.installPath, @"\\", @"\");
					//string targetpath = shortcutRow.Row["Target"];
					if (component == shortcutRow.Row["Component_"] && shortcutRow.Row["Directory_"] == Directory)
					{
						return inst.shortcutName;
					}
					else
					{
						shortCutNameUnique = inst.shortcutName + count;
						count++;
					}
				}
				else
				{
					IsmRow Row = new IsmRow();

					Row.Row.Add("Shortcut", shortCutNameUnique);
					Row.Row.Add("Directory_", Directory);
					Row.Row.Add("Name", inst.shortcutName);
					Row.Row.Add("Component_", component);
					Row.Row.Add("Target", inst.installPath);
					Row.Row.Add("Arguments", "");
					Row.Row.Add("Description", "");
					Row.Row.Add("Hotkey", "");
					Row.Row.Add("Icon_", shortCutIconName);
					Row.Row.Add("IconIndex", "");
					Row.Row.Add("ShowCmd", "");
					Row.Row.Add("WkDir", "");
					Row.Row.Add("DisplayResourceDLL", "");
					Row.Row.Add("DisplayResourceId", "");
					Row.Row.Add("DescriptionResourceDLL", "");
					Row.Row.Add("DescriptionResourceId", "");
					Row.Row.Add("ISComments", "");
					Row.Row.Add("ISShortcutName", "");
					Row.Row.Add("ISAttributes", "");
					
					ismDb.addRow("Shortcut", Row);
					/*XmlNode iconNode = root.SelectSingleNode("/msi/table[@name='Shortcut']");

					//XmlElement newFile = xDoc.CreateElement("row");
					XmlElement newRow = xDoc.CreateElement("row");
					newRow.InnerXml = "<td>" + shortCutNameUnique + "</td>" + //Shortcut
										"<td>" + Directory + "</td>" + //Directory_
										"<td>" + inst.shortcutName + "</td>" + //Name
										"<td>" + component + "</td>" + //Component_
										"<td>" + inst.installPath + "</td>" + //Target
										"<td/>" + //Arguments
										"<td/>" + //Description
										"<td/>" + //Hotkey
										"<td>" + shortCutIconName + "</td>" + //Icon_
										"<td/>" + //IconIndex
										"<td/>" + //ShowCmd
										"<td/>" + //WkDir
										"<td/>" + //DisplayResourceDLL
										"<td/>" + //DisplayResourceId
										"<td/>" + //DescriptionResourceDLL
										"<td/>" + //DescriptionResourceId
										"<td/>" + //ISComments
										"<td/>" + //ISShortcutName
										"<td/>"; //ISAttributes
										

					iconNode.AppendChild((XmlNode)newRow);*/

					return shortCutNameUnique;
				}
			}
			return shortCutNameUnique;
			/*<col key="yes" def="s72">Shortcut</col> inst.shortcutName
		<col def="s72">Directory_</col>Directory
		<col def="l128">Name</col>inst.shortcutName
		<col def="s72">Component_</col>component
		<col def="s255">Target</col>inst.installPath
		<col def="S255">Arguments</col>
		<col def="L255">Description</col>
		<col def="I2">Hotkey</col>
		<col def="S72">Icon_</col>shortCutName
		<col def="I2">IconIndex</col>
		<col def="I2">ShowCmd</col>
		<col def="S72">WkDir</col>
		<col def="S255">DisplayResourceDLL</col>
		<col def="I2">DisplayResourceId</col>
		<col def="S255">DescriptionResourceDLL</col>
		<col def="I2">DescriptionResourceId</col>
		<col def="S255">ISComments</col>
		<col def="S255">ISShortcutName</col>
		<col def="I4">ISAttributes</col>*/
		}

		public string addShortcutIcon(XmlElement root, Shortcut sc)
		{
			//string iconName = "";
			int count = 1;
			string iconName = sc.Name;

			while (true)
			{
				//Hashtable shortcutIconHash = parseTableRow(root, "Icon", iconName, "Name");
				IsmRow shortcutIconrow = ismDb.getRow("Icon", iconName);

				if (shortcutIconrow != null)
				{
					if (sc.Icon == shortcutIconrow.Row["ISBuildSourcePath"])
					{
						return shortcutIconrow.Row["Name"];
					}
					else
					{
						iconName = sc.Name + count;
						count++;
					}
				}
				else
				{
					IsmRow Row = new IsmRow();

					Row.Row.Add("Name", iconName);
					Row.Row.Add("Data", "");
					Row.Row.Add("ISBuildSourcePath", sc.Icon);
					Row.Row.Add("ISIconIndex", "0");

					ismDb.addRow("Icon", Row);

					return iconName;
				}
			}

			//return iconName;
		}

		public string addShortcutIcon(XmlElement root, Install inst)
		{
			//string iconName = "";
			int count = 1;
			string iconName = inst.shortcutName;

			while (true)
			{
				//Hashtable shortcutIconHash = parseTableRow(root, "Icon", iconName, "Name");
				IsmRow shortcutIconrow = ismDb.getRow("Icon", iconName);

				if (shortcutIconrow != null)
				{
					if (inst.shortcutIcon == shortcutIconrow.Row["ISBuildSourcePath"])
					{
						return shortcutIconrow.Row["Name"];
					}
					else
					{
						iconName = inst.shortcutName + count;
						count++;
					}
				}
				else
				{
					IsmRow Row = new IsmRow();

					Row.Row.Add("Name", iconName);
					Row.Row.Add("Data", "");
					Row.Row.Add("ISBuildSourcePath", inst.shortcutIcon);
					Row.Row.Add("ISIconIndex", "0");
					
					ismDb.addRow("Icon", Row);

					return iconName;
				}
			}

			//return iconName;
		}

		public void updateFilePatch(string file, string version)
		{
			IsmRow Row = ismDb.getRow("File", file);
			if (Row.Row != null)
			{
				Row.Row["Version"] = version;
			}
			//XmlNode fileNode = root.SelectSingleNode("/msi/table[@name='File']/row[td[1]='" + file + "']/td[5]");
			//fileNode.InnerXml = version;

		}

		public void updateComponentPatch(string component)
		{
			IsmRow Row = ismDb.getRow("Component", component);
			if (Row.Row != null)
			{
				//Need to check if the attribute contains 128 if it does reverse it if it does not
				//do nothing.
				int curr = int.Parse( Row.Row["Attributes"] );
				int test = (curr ^ 128);
				if (test == 0)
				{
					int attribute = curr ^ 128;
					Row.Row["Attributes"] = attribute.ToString();
				}
			}
			/*XmlNode fileNode = root.SelectSingleNode("/msi/table[@name='Component']/row[td[1]='" + component + "']/td[4]");
			string currentAttribute = fileNode.InnerText;

			int curr = int.Parse(fileNode.InnerText);

			int attribute = curr ^ 128;

			fileNode.InnerXml = attribute.ToString();*/

			//fileNode.InnerXml = version;
			/*my $attribute = 0;
        if ($compRow ) {
            #Case 2,3, or 4
            $attribute = $compRow->StringData(1);
            $attribute = $attribute ^ 128;
        }*/

		}

		public bool isVersionGreater(string baseVersion, string newVersion)
		{
			bool versionGreater = false;

			int baseMajor = 0;
			int baseMinor = 0;
			int basePatch = 0;
			int baseBuild = 0;

			int newMajor = 0;
			int newMinor = 0;
			int newPatch = 0;
			int newBuild = 0;

			//10.01.03244
			string pattern = @"(\d\d)\.(\d\d)\.(\d\d)(\d\d\d)";
			Match match = Regex.Match(baseVersion, pattern);
			
			if (match.Success)
			{
				baseMajor = int.Parse(match.Groups["1"].Value);
				baseMinor = int.Parse(match.Groups["2"].Value);
				basePatch = int.Parse(match.Groups["3"].Value);
				baseBuild = int.Parse(match.Groups["4"].Value);
			}

			match = Regex.Match(newVersion, pattern);

			if (match.Success)
			{
				newMajor = int.Parse(match.Groups["1"].Value);
				newMinor = int.Parse(match.Groups["2"].Value);
				newPatch = int.Parse(match.Groups["3"].Value);
				newBuild = int.Parse(match.Groups["4"].Value);
			}

			if (newMajor > baseMajor) // || newMinor >= baseMinor || newPatch >= basePatch && newBuild > baseBuild)
			{
				versionGreater = true;
			}
			else
			{
				if (newMajor == baseMajor && newMinor > baseMinor)
				{
					versionGreater = true;
				}
				else
				{
					if (newMajor == baseMajor && newMinor == baseMinor && newPatch > basePatch)
					{
						versionGreater = true;
					}
					else
					{
						if (newMajor == baseMajor && newMinor == baseMinor && newPatch == basePatch && newBuild > baseBuild)
						{
							versionGreater = true;
						}
					}
				}
			}

			return versionGreater;
		}

		public void createFile(XmlElement root, string File, string Component_, string FileName,
			string FileSize, string Version, string Language, string Attributes, string Sequence, string ISBuildSourcePath,
			string ISAttributes, string ISComponentSubFolder_)
		{
			IsmRow Row = new IsmRow();

			Row.Row.Add("File", File);
			Row.Row.Add("Component_", Component_);
			Row.Row.Add("FileName", FileName);
			Row.Row.Add("FileSize", FileSize);
			Row.Row.Add("Version", Version);
			Row.Row.Add("Language", Language);
			Row.Row.Add("Attributes", Attributes);
			Row.Row.Add("Sequence", Sequence);
			Row.Row.Add("ISBuildSourcePath", ISBuildSourcePath);
			Row.Row.Add("ISAttributes", ISAttributes);
			Row.Row.Add("ISComponentSubFolder_", ISComponentSubFolder_);
			
			ismDb.addRow("File", Row);
		}

		public string parseComponents(XmlElement root, Install inst)
		{
			string component = "";
			string Directory = parseDirectories(root, inst.installRootPath);
			bool notFound = true;
			bool newComponent = true;
			string fileNameTemp;
			string fileName = inst.installFileName;
			int componentAttribute = 0;
			int ISAttribute = 1;
			fileNameTemp = Regex.Replace(fileName, @"\s", "_");
			/* # Replace non-alpha chars with underscores.  
    $genericID =~ s|[^\w\.]|_|g;
    # If the file starts with a digit, prepend the name with an underscore.  
    if($genericID =~ m|(^\d)|) {
        $genericID = "_".$genericID;
    }*/
			fileName = Regex.Replace(fileName, @"\.", "_");
			fileName = Regex.Replace(fileName, @"^\d", "_");
			fileName = Regex.Replace(fileName, @"-", "_");
			fileName = Regex.Replace(fileName, @"&", "_");
			fileName = Regex.Replace(fileName, @"\s", "_");
			//
			fileName = fileName.ToLower();

			//Check to see if file is a reserved work if it is prepend underscore.
			if (conf.isReservedWord(fileName))
			{
				fileName = "_" + fileName;
			}

			if (UtilityCore.getMergeModuleMode == UtilityCore.MergeModuleMode.On)
			{
				fileName = fileName + Module;
			}

			fileNameTemp = fileName;

			if (fileNameTemp.Length > 65)
			{
				int halfNameLength = fileNameTemp.Length / 2;
				int amountOver = fileNameTemp.Length - 65;
				int remove = amountOver / 2;
				string startName = fileNameTemp.Substring(0, halfNameLength - remove);
				string endName = fileNameTemp.Substring(halfNameLength + remove);
				fileNameTemp = startName + "88" + endName;
			}

			int count = 1;
			while (notFound)
			{
				IsmRow componentRow = ismDb.getRow("Component", fileNameTemp);
				IsmRow fileRow = ismDb.getRow("File", fileNameTemp);
				if (UtilityCore.getMergeModuleMode == UtilityCore.MergeModuleMode.On)
				{
					if (componentRow != null)
					{
						if (componentRow.Row["Directory_"] == Directory)
						{
							if (fileRow.Row["ISBuildSourcePath"] == inst.stagePath)
							{
								//if they are the same return the component id
								//return fileNameTemp;
								notFound = false;
								newComponent = false;
							}
						}
					}
					else
					{
						notFound = false;
					}
				}
				else
				{
					IsmRow featureRow = ismDb.getRow("FeatureComponents", inst.Feature + "," + fileNameTemp);
					if (componentRow != null)
					{
						//check to see if the feature row exist.
						if (featureRow != null)
						{
							//if it exist check to see that it is the same by compairing the directories and the features
							if (componentRow.Row["Directory_"] == Directory && featureRow.Row["Feature_"] == inst.Feature)
							{
								//if they are the same return the component id
								//return fileNameTemp;
								notFound = false;
								newComponent = false;
							}
						}
						/*else
						{
							notFound = false;
						}*/
					}
					else
					{
						notFound = false;
					}
				}

				if (notFound)
				{
					fileNameTemp = fileName + count;
					count++;
				}

			}

			if (newComponent)
			{
				ISAttribute = ISAttribute + inst.ISAttributes;
				componentAttribute = componentAttribute ^ inst.ComponentAttributes;

				string guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

				if (Map != null)
				{
					string tempGuid = Map.getGuid(inst.Feature, inst.installPath);
					if (tempGuid != null)
					{
						guid = tempGuid;
					}
				}

				if (inst.Conditions != null)
				{
					//Console.WriteLine("cheese");
				}
				component = createComponent(root, fileNameTemp, guid, Directory, componentAttribute.ToString(), inst.Conditions, fileNameTemp, ISAttribute.ToString(), "AIM Component", "", "");
				//return component;
			}
			else
			{
				component = fileNameTemp;
			}

			return component;
		}

		public void createFeatureComponent(XmlElement root, string Feature_, string Component_)
		{
			/*<col key="yes" def="s38">Feature_</col>
		<col key="yes" def="s72">Component_</col>
			 <row><td>AE_Registry</td><td>{AF1FBD73-96BA-4FED-BB23-BBE940056E88}</td><td>INSTALLDIR</td><td>8</td><td/><td/><td>17</td><td/><td/><td/><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td></row>*/
			IsmRow Row = new IsmRow();

			Row.Row.Add("Feature_", Feature_);
			Row.Row.Add("Component_", Component_);

			ismDb.addRow("FeatureComponents", Row);
			/*XmlNode fileNode = root.SelectSingleNode("/msi/table[@name='FeatureComponents']");

			//XmlElement newFile = xDoc.CreateElement("row");
			XmlElement newFeatureComponent = xDoc.CreateElement("row");
			newFeatureComponent.InnerXml = "<td>" + Feature_ + "</td>" + //Component
								"<td>" + Component_ + "</td>"; //ISDotNetInstallerArgsRollback

			fileNode.AppendChild((XmlNode)newFeatureComponent);*/
			
		}

		public string createComponent(XmlElement root,string Component, string ComponentId, string Directory_,
			string Attributes, string Condition, string KeyPath, string ISAttributes, string ISComments, string ISScanAtBuildFile,
			string ISRegFileToMergeAtBuild)
		{
			/*<col key="yes" def="s72">Component</col>
					<col def="S38">ComponentId</col>
					<col def="s72">Directory_</col>
					<col def="i2">Attributes</col>
					<col def="S255">Condition</col>
					<col def="S72">KeyPath</col>
					<col def="I4">ISAttributes</col>
					<col def="S255">ISComments</col>
					<col def="S255">ISScanAtBuildFile</col>
					<col def="S255">ISRegFileToMergeAtBuild</col>
					<col def="S0">ISDotNetInstallerArgsInstall</col>
					<col def="S0">ISDotNetInstallerArgsCommit</col>
					<col def="S0">ISDotNetInstallerArgsUninstall</col>
					<col def="S0">ISDotNetInstallerArgsRollback</col>
			 <row><td>AE_Registry</td><td>{AF1FBD73-96BA-4FED-BB23-BBE940056E88}</td><td>INSTALLDIR</td><td>8</td><td/><td/><td>17</td><td/><td/><td/><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td></row>*/

			IsmRow Row = new IsmRow();

			Row.Row.Add("Component", Component);
			Row.Row.Add("ComponentId", ComponentId);
			Row.Row.Add("Directory_", Directory_);
			Row.Row.Add("Attributes", Attributes);
			Row.Row.Add("Condition", Condition);
			Row.Row.Add("KeyPath", KeyPath);
			Row.Row.Add("ISAttributes", ISAttributes);
			Row.Row.Add("ISComments", ISComments);
			Row.Row.Add("ISScanAtBuildFile", ISScanAtBuildFile);
			Row.Row.Add("ISRegFileToMergeAtBuild", ISRegFileToMergeAtBuild);
			Row.Row.Add("ISDotNetInstallerArgsInstall", "/LogFile=");
			Row.Row.Add("ISDotNetInstallerArgsCommit", "/LogFile=");
			Row.Row.Add("ISDotNetInstallerArgsUninstall", "/LogFile=");
			Row.Row.Add("ISDotNetInstallerArgsRollback", "/LogFile=");

			ismDb.addRow("Component", Row);
			
			return Component;
		}

		public string determineUniqueFileName(XmlElement root, Install inst)
		{
			string uniqueName = "";
			XmlNode fileSearchNode = root.SelectSingleNode("/msi/table[@name='File']/row[td[1]='" + inst.installFileName + "']");
			if (fileSearchNode != null)
			{
				//file already exist
				//determine if files path matches that of the existing file
				string Directory = parseDirectories(root, inst.installRootPath);
				//check to see if the file relates to a component with the directory that matches.
				Hashtable componentDetails = parseTableRow(root, "Component", inst.installFileName, "Component");
				Hashtable fileDetails = parseTableRow(root, "File", inst.installFileName, "FileName");
				//if(fileDetails["

				//fileSearchNode
				//currentDirHash = parseDirectory(root, currentDirUppercount, "1");
			}
			else
			{
				uniqueName = inst.installFileName;
			}
			return uniqueName;
		}

		public Hashtable parseTableRow(XmlElement root, string table, string value, string attribute)
			//int td)
		{
			Hashtable TableRow = new Hashtable();
			Hashtable AttributesHash = new Hashtable();
			int td = -1;

			XmlNodeList tableSearchNodes = root.SelectNodes("/msi/table[@name='" + table + "']/col");
			
			int attributeCount = 0;
			if (tableSearchNodes != null)
			{
				foreach (XmlNode node in tableSearchNodes)
				{
					AttributesHash.Add(attributeCount, node.InnerText);
					if (attribute == node.InnerText)
					{
						td = attributeCount + 1;
					}
					attributeCount++;
				}
			}
			else
			{
				throw new Exception("No table was found: " + table);
			}

			if (td == -1)
			{
				throw new Exception("Attribute does not match any in the Table");
			}

			XmlNode fileSearchNode = root.SelectSingleNode("/msi/table[@name='"+ table + "']/row[td[" + td.ToString() + "]='" + value + "']");
			//"/msi/table[@name='File']/row[td[5]='value']"
			if (fileSearchNode != null)
			{
				XmlNodeList nodeList = fileSearchNode.ChildNodes;
				int nodeCount = 0;
				foreach (XmlNode node in nodeList)
				{
					TableRow.Add(AttributesHash[nodeCount], nodeList[nodeCount].InnerText);
					nodeCount++;
				}
				
			}
			return TableRow;
		}

		public void createDirectory(XmlElement root, string Directory, string Directory_Parent, string DefaultDir, string ISDescription, string ISAttributes, string ISFolderName)
		{
			/*<col key="yes" def="s72">Directory</col>
		<col def="S72">Directory_Parent</col>
		<col def="l255">DefaultDir</col>
		<col def="S255">ISDescription</col>
		<col def="I4">ISAttributes</col>
		<col def="S255">ISFolderName</col>
			 <row><td>3.3.1DEV</td><td>BACKBASE</td><td>3.3.1dev</td><td/><td>0</td><td/></row>*/
			IsmRow Row = new IsmRow();

			Row.Row.Add("Directory", Directory);
			Row.Row.Add("Directory_Parent", Directory_Parent);
			Row.Row.Add("DefaultDir", DefaultDir);
			Row.Row.Add("ISDescription", ISDescription);
			Row.Row.Add("ISAttributes", ISAttributes);
			Row.Row.Add("ISFolderName", ISFolderName);
			//Row.Row.Add("ISAttributes", ISAttributes);
		
			ismDb.addRow("Directory", Row);
			/*XmlNode fileNode = root.SelectSingleNode("/msi/table[@name='Directory']");

			//XmlElement newFile = xDoc.CreateElement("row");
			XmlElement newDir = xDoc.CreateElement("row");
			newDir.InnerXml = "<td>" + Dir + "</td>" + //Directory
								"<td>" + Parent + "</td>" + //Directory_Parent
								"<td>" + Def + "</td>" + //DefaultDir
								"<td>" + Desc + "</td>" + //ISDescription
								"<td>" + ISAtt + "</td>" + //ISAttributes
								"<td>" + ISfolder + "</td>"; //ISFolderName

			fileNode.AppendChild((XmlNode)newDir);*/
		}

		//root is no longer required or used and can be removed.
		public string parseDirectories(XmlElement root, string directory)
		{
			Hashtable currentDirHash;
			Hashtable parentDirHash;
			Hashtable currParentDirHash;
			//XmlNode directorySearchNode;
			directory = directory.Trim();
			directory = Regex.Replace(directory, @"/", "\\");
			//seperate the parent path with the current position.
			string pattern = @"(.*)\\(.*)";
			Match match = Regex.Match(directory, pattern);
			string currentDir = directory;
			string rootDir = "";
			string currentParent = "";
			if (match.Success)
			{
				rootDir = match.Groups["1"].Value;
				currentDir = match.Groups["2"].Value;
			}

			

			//Check to see if the current position is a predefined directory
			pattern = @"^\[(.*)\]$";
			match = Regex.Match(currentDir, pattern);
			if (match.Success)
			{
				currentDir = match.Groups["1"].Value;
				//currentDirHash = parseDirectory(root, currentDir, "1");
				//parseDirectories(root, currentDirHash["Directory"].ToString(), remainingDir);
				return currentDir;
			}
			

			string currentDirUpper = currentDir.ToUpper();
			currentDirUpper = Regex.Replace(currentDirUpper,@"\s","_");

			//in the case that this is a merge module the module name needs to be appended to keep it unique
			//from the installer it will be merged with.
			if (UtilityCore.getMergeModuleMode == UtilityCore.MergeModuleMode.On)
			{
				pattern = @"##.*##";
				//currentDirUpper = currentDirUpper + Module;
				match = Regex.Match(currentDirUpper, pattern);
				if (match.Success)
				{
					currentDirUpper = Regex.Replace(currentDirUpper, @"##", "");
					currentDirUpper = "##" + currentDirUpper + Module + "##";

					//Because the directory will need to be replaced with a string it needs to be changed to be the 
					//module string time which appends the module name. (damn windows installer bug)
					currentDir = Regex.Replace(currentDir, @"##", "");
					currentDir = "##" + currentDir + Module + "##";
				}
				else
				{
					currentDirUpper = currentDirUpper + Module;
				}
				//if(currentDirUpper 
				
			}

			/*if (parent != "")
			{
				parentDirHash = parseDirectory(root, parent, "1");
			}*/
			
			currentParent = parseDirectories(root, rootDir);
			
			string currentDirUppercount = currentDirUpper;
			int count = 1;
			bool keepGoing = true;
			while (keepGoing)
			{
				//currentDirHash = parseTableRow(root, "Directory", currentDirUppercount, "Directory");
				//currentDirHash = parseTableRow(root, "Directory", currentDirUppercount, "Directory");
				//parseTableRow(
				IsmRow Row = ismDb.getRow("Directory", currentDirUppercount);
				//parseDirectory(root, currentDirUppercount, "1");
				
				if (Row != null)
				{
					//currentParent = parseDirectories(root, rootDir);
					//currParentDirHash = parseTableRow(root, "Directory", currentParent, "Directory");
						//parseDirectory(root, currentParent, "1");
					if (currentDir == Row.Row["DefaultDir"] && Row.Row["Directory_Parent"] == currentParent)
					{
						//this is the case that the defaultdir of the currentdirhash is equal to the current dir.
						return currentDirUppercount;
					}
					else
					{
						currentDirUppercount = currentDirUpper + count.ToString();
						count++;
					}
					/*if (currentParent == currParentDirHash["Directory"].ToString())
					{
						return currentDirUppercount;
					}
					else
					{
						currentDirUppercount = currentDirUpper + count.ToString();
						count++;
					}*/
				}
				else
				{
					//currentParent = parseDirectories(root, rootDir);
					createDirectory(root, currentDirUppercount, currentParent, currentDir, "", "0", "");
					return currentDirUppercount;
				}
			}
			return "nodirectory";

		}

		/*public void ParseTable(string ismFile, string Table)
		{
			
			xDoc = new XPathDocument(ismFile);
			xNav = xDoc.CreateNavigator();

			XPathExpression xExpr;

			xExpr = xNav.Compile("/msi/table[@name='" + Table + "']/col");
			XPathNodeIterator xIterator = xNav.Select(xExpr);
			string createTableString = "";
			while (xIterator.MoveNext())
			{
				XPathNavigator nav2 = xIterator.Current.Clone();

				if (createTableString.Length > 0)
				{
					createTableString = createTableString + ", ";
				}

				string value = nav2.InnerXml.ToString();
				string attribute = nav2.GetAttribute("def","");

				attribute = attribute.ToLower();
				string pattern = @"s(?<Size>\d*)";
				
				Match match = Regex.Match(attribute, pattern);

				if (match.Success)
				{
					int temp = 255; // match.Groups["Size"].Value;
					createTableString = createTableString + value + " nvarchar(" + temp.ToString() + ")";
				}

				pattern = @"i(?<Size>\d*)";

				match = Regex.Match(attribute, pattern);

				if (match.Success)
				{
					int temp = 255; //string temp = match.Groups["Size"].Value;
					createTableString = createTableString + value + " nvarchar(" + temp.ToString() + ")";
				}
			}

			createTableString = "CREATE TABLE " + Table + "_ism ( " + createTableString + " )";
			//createTableString = "CREATE TABLE File_ism (Id int, test nvarchar(50))";
			//createTableString = "CREATE TABLE Persons (P_Id int,LastName nvarchar(255),FirstName nvarchar(255),Address nvarchar(255),City nvarchar(255))";
			Console.WriteLine(createTableString);

			string conString = Properties.Settings.Default.ISMDataConnectionString;

			using (SqlCeConnection con = new SqlCeConnection(conString))
			{
				con.Open();

				/*using (SqlCeCommand com = new SqlCeCommand("DROP TABLE test", con))
				{
					com.ExecuteNonQuery();
				}
				*//*
				try
				{
					using (SqlCeCommand com = new SqlCeCommand(createTableString, con))
					{
						com.ExecuteNonQuery();
					}
				}
				catch (Exception Ex)
				{
					Console.WriteLine(Ex.ToString());
					using (SqlCeCommand com = new SqlCeCommand("DROP TABLE " + Table, con))
					{
						com.ExecuteNonQuery();
					}
					using (SqlCeCommand com = new SqlCeCommand(createTableString, con))
					{
						com.ExecuteNonQuery();
					}
				}

				/*using (SqlCeCommand com = new SqlCeCommand("INSERT INTO test VALUES (1,2,3)", con))
				{
					com.ExecuteNonQuery();
				}

				using (SqlCeCommand com = new SqlCeCommand("SELECT * FROM test", con))
				{
					SqlCeDataReader reader = com.ExecuteReader();
					while (reader.Read())
					{
						int num = reader.GetInt32(1);
						Console.WriteLine(num);
					}
				}*/
			//}
		//}
	}
}