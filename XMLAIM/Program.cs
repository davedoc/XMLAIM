using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace XMLAIM
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			Manifest Man;

			string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			Config conf = new Config(path + "\\config.xml");

			if (args.Length > 0)
			{
				string sArgs = string.Join(" ", args);
				Console.WriteLine("XMLAIM started with command line options: " + sArgs);
			}

			int iArgumentCount = 0;
			bool silentMode = false;
			bool stageManifest = false;
			bool updateIsm = false;
			bool useStaging = false;
			string manifest = "";
			string ismFile = "";
			string stageArea = "";
			string srcArea = "";
			string branding = "";
			string lang = "";
			string guidMap = "";
			string moduleString = "";
			while (iArgumentCount < args.Length)
			{
				switch (args[iArgumentCount].ToLower())
				{
					case "/s":
					case "/silent":
						silentMode = true;
						break;
					case "/stage":
						if (iArgumentCount + 2 < args.Length)
						{
							stageArea = args[iArgumentCount + 1];
							srcArea = args[iArgumentCount + 2];
							iArgumentCount++;
							//stageManifest = true;
							UtilityCore.setOperationMode(UtilityCore.OperationMode.StageManifest);
						}
						break;
					case "/man":
						if (iArgumentCount + 1 < args.Length)
						{
							manifest = args[iArgumentCount + 1];
							iArgumentCount++;
						}
						break;
					case "/ism":
						if (iArgumentCount + 1 < args.Length)
						{
							ismFile = args[iArgumentCount + 1];
							UtilityCore.setOperationMode(UtilityCore.OperationMode.UpdateIsm);
							//updateIsm = true;
							iArgumentCount++;
						}
						break;
					case "/sign":
						if (iArgumentCount + 2 < args.Length)
						{
							manifest = args[iArgumentCount + 1];
							stageArea = args[iArgumentCount + 2];
							iArgumentCount++;
							UtilityCore.setOperationMode(UtilityCore.OperationMode.SignFiles);
						}
						break;
					case "/su":
						if (iArgumentCount + 2 < args.Length)
						{
							branding = args[iArgumentCount + 1];
							lang = args[iArgumentCount + 2];
							ismFile = args[iArgumentCount + 3];
							iArgumentCount++;
							UtilityCore.setOperationMode(UtilityCore.OperationMode.UpdateStrings);
						}
						break;
					case "/use_stage":
						useStaging = true;
						break;
					case "/ms":
						if (iArgumentCount + 1 < args.Length)
						{
							UtilityCore.setMergeModuleMode(UtilityCore.MergeModuleMode.On);
							moduleString = args[iArgumentCount + 1];
							iArgumentCount++;
						}
						break;
					case "/mm":
						//This flag is used when building a merge module ism.
						UtilityCore.setMergeModuleMode(UtilityCore.MergeModuleMode.On);
						break;
					case "/export":
						UtilityCore.setOperationMode(UtilityCore.OperationMode.ExportIsm);
						ismFile = args[iArgumentCount + 1];
						manifest = args[iArgumentCount + 2];
						iArgumentCount++;
						break;
					case "/expguid":
						UtilityCore.setOperationMode(UtilityCore.OperationMode.ExportGuid);
						ismFile = args[iArgumentCount + 1];
						manifest = args[iArgumentCount + 2];
						break;
					case "/version":
						//This flag is used to populate the PatchFileVersion table in the ism.
						//This flag would only be used for minor upgrades.
						UtilityCore.setVersionMode(UtilityCore.VersionMode.On);
						break;
					case "/updateprops":
						//The purpose of this flag is to update the ism but only the properties.
						if (iArgumentCount + 1 < args.Length)
						{
							ismFile = args[iArgumentCount + 1];
							UtilityCore.setOperationMode(UtilityCore.OperationMode.UpdateIsmPropsOnly);
							//updateIsm = true;
							iArgumentCount++;
						}
						break;
					case "/guidmap":
						guidMap = args[iArgumentCount + 1];
						break;
				}
				iArgumentCount++;
			}

			if (manifest != "")
			{
				manifest = Regex.Replace(manifest, @"/", @"\");
			}

			if (silentMode)
			{
				switch (UtilityCore.getOperationMode)
				{
					case UtilityCore.OperationMode.StageManifest:
						try
						{
							Console.WriteLine("Staging Manifest");
							Man = new Manifest(manifest, useStaging);
							Man.loadManifest();

							stageOperator so = new stageOperator(Man, stageArea, srcArea);
							so.stageManifest();
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error while trying stage manifest: " + ex.Message);
							return 1;
						}
						break;
					case UtilityCore.OperationMode.SignFiles:
						try
						{
							Man = new Manifest(manifest, useStaging);
							Man.loadManifest();

							SignFiles(conf, Man, stageArea);
							
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error while trying sign manifest: " + ex.Message);
							return 1;
						}
						break;
					case UtilityCore.OperationMode.UpdateIsm:
						try
						{
							IsmDatabase db = new IsmDatabase(ismFile);
							db.LoadTable("File", "0");
							db.LoadTable("Directory", "0");
							db.LoadTable("Component", "0");
							db.LoadTable("Shortcut", "0");
							db.LoadTable("Icon", "0");
							db.LoadTable("ExtractWar", "0");
							if (UtilityCore.getVersionMode == UtilityCore.VersionMode.On)
							{
								db.LoadTable("PatchFileVersion", "0");
							}
							String Module = "";
							if (UtilityCore.getMergeModuleMode == UtilityCore.MergeModuleMode.Off)
							{

								db.LoadTable("BackupTable", "0");
								db.LoadTable("FeatureComponents", "0,1");
							}
							else
							{
								string pattern = @".*\\(.*).xml";

								Match match = Regex.Match(manifest, pattern);

								if (match.Success)
								{
									Module = match.Groups["1"].Value;
								}
								//Module = "";
							}
							
							//db.LoadTable("Upgrade","0,1,2,3,4");
							db.LoadTable("Property", "0");

							Man = new Manifest(manifest, useStaging);
							Man.loadManifest();
							/*GuidMapping gm = null;

							if (guidMap != "")
							{
								gm = new GuidMapping(guidMap);
								gm.loadMappedFile();
							}*/

							IsmOperator op = new IsmOperator(ismFile, db);
							op.loadISM();
							op.processManifest(Man, Module);

							db.writeData();

							
							//if the manifest contains a sign file directory then sign the files in that directory.
							//use signElements Dictionary to sign files.

						}
						catch (Exception ex)
						{
							Console.WriteLine("Error while trying to run console update ISM: " + ex.Message);
							return 1;
						}
						break;
					case UtilityCore.OperationMode.UpdateIsmPropsOnly:
						try
						{
							IsmDatabase db = new IsmDatabase(ismFile);
							String Module = "";
							//db.LoadTable("File", "0");
							//db.LoadTable("Directory", "0");
							//db.LoadTable("Component", "0");
							//db.LoadTable("Shortcut", "0");
							//db.LoadTable("Icon", "0");
							//db.LoadTable("ExtractWar", "0");
							/*if (UtilityCore.getVersionMode == UtilityCore.VersionMode.On)
							{
								db.LoadTable("PatchFileVersion", "0");
							}
							
							if (UtilityCore.getMergeModuleMode == UtilityCore.MergeModuleMode.Off)
							{

								db.LoadTable("BackupTable", "0");
								db.LoadTable("FeatureComponents", "0,1");
							}
							else
							{
								string pattern = @".*\\(.*).xml";

								Match match = Regex.Match(manifest, pattern);

								if (match.Success)
								{
									Module = match.Groups["1"].Value;
								}
								//Module = "";
							}
							*/
							//db.LoadTable("Upgrade","0,1,2,3,4");
							db.LoadTable("Property", "0");

							Man = new Manifest(manifest, useStaging);
							Man.loadManifest();
							
							IsmOperator op = new IsmOperator(ismFile, db);
							op.loadISM();
							op.updateProperties(Man);

							db.writeData();


							//if the manifest contains a sign file directory then sign the files in that directory.
							//use signElements Dictionary to sign files.

						}
						catch (Exception ex)
						{
							Console.WriteLine("Error while trying to run console update ISM: " + ex.Message);
							return 1;
						}
						break;

					case UtilityCore.OperationMode.UpdateStrings:
						stringOperator sopp = new stringOperator(branding, lang);
						
						sopp.processBranding();

						IsmDatabase ismdb = new IsmDatabase(ismFile);
						ismdb.LoadTable("ISString", "0,1");

						IsmTable ism = new IsmTable(ismdb);
						ism.UpdateStrings(sopp, moduleString);
						//sopp.populateStrings(ismdb);

						//IsmOperator opp = new IsmOperator(ismFile, ismdb);
						//opp.loadISM();

						//opp.
						ismdb.writeData();

						break;
					case UtilityCore.OperationMode.ExportIsm:
						IsmDatabase expDb = new IsmDatabase(ismFile);
						expDb.LoadTable("File", "1");
						expDb.LoadTable("Directory", "0");
						expDb.LoadTable("Component", "0");
						expDb.LoadTable("Shortcut", "3");
						expDb.LoadTable("Icon", "0");
						expDb.LoadTable("FeatureComponents", "1");

						IsmTable exism = new IsmTable(expDb);
						exism.ExportIsm();
						break;
					case UtilityCore.OperationMode.ExportGuid:
						IsmDatabase expgDb = new IsmDatabase(ismFile);
						expgDb.LoadTable("File", "1");
						expgDb.LoadTable("Directory", "0");
						expgDb.LoadTable("Component", "0");
						expgDb.LoadTable("FeatureComponents", "1");
						IsmTable exgism = new IsmTable(expgDb);
						exgism.ExportGuids(manifest);
						break;
				}
				/*if (stageManifest)
				{
					Console.WriteLine("Staging Manifest");
					Man = new Manifest(manifest,useStaging);
					Man.loadManifest();

					stageOperator so = new stageOperator(Man, stageArea, srcArea);
					so.stageManifest();

				}*/

				/*if (updateIsm)
				{
					try
					{
						IsmDatabase db = new IsmDatabase(ismFile);
						db.LoadTable("File", "0");
						db.LoadTable("Directory", "0");
						db.LoadTable("Component", "0");
						db.LoadTable("Shortcut", "0");
						db.LoadTable("Icon", "0");
						db.LoadTable("ExtractWar", "0");
						db.LoadTable("BackupTable", "0");
						db.LoadTable("FeatureComponents", "0,1");
						//db.LoadTable("Upgrade","0,1,2,3,4");
						db.LoadTable("Property", "0");

						Man = new Manifest(manifest, useStaging);
						Man.loadManifest();

						IsmOperator op = new IsmOperator(ismFile, db);
						op.loadISM();
						op.processManifest(Man);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error while trying to run console update ISM: " + ex.Message);
						return 1;
					}
				}*/
				
			}
			else
			{
				Application.Run(new Main());
			}

			return 0;
            
        }

		static void SignFiles(Config conf, Manifest man, string stageArea)
		{
			try
			{
				Console.WriteLine("Signing the following files: \n");
				foreach (int i in man.signElements.Keys)
				{
					Console.WriteLine(man.signElements[i]);
				}
				string errors = "";
				foreach (int i in man.signElements.Keys)
				{
					string fileLocation = man.signElements[i];
					fileLocation = Regex.Replace(fileLocation, @"\<STAGING\>", stageArea);
					//{SIGNTOOL} sign /f serena_authenticode.pfx {MEDIA_ROOT}/{SBM}/disk1/{Mashup_suite}/setup.msi
					string signCommand = conf.getSignValue("signparams") + " \""
						+ conf.getSignValue("certificate") + "\" \""
						+ fileLocation + "\"";
					Console.WriteLine( conf.getSignValue("signtool") + signCommand);
					string result = UtilityCore.ExecuteCommandLine(conf.getSignValue("signtool"), signCommand);

					 /* 
						Successfully signed: D:\cm\BM10R1\Suite\REG\MSIInstall\stage\AE\Application Engi
						ne\webservices\bin\aemmwebservices71.dll
					 * */

					string pattern = @".*Successfully signed.*";

					Match match = Regex.Match(result, pattern);

					if (match.Success == false)
					{
						//The signing was not successful
						errors += "Could not sign file: " + fileLocation + "\n" ;
					}
					

					
					//{SIGNTOOL} verify /v /a /pa {MEDIA_ROOT}/{SBM}/disk1/{Mashup_suite}/setup.msi
					string validateCommand = conf.getSignValue("verifyparams") + " \""
						+ fileLocation + "\"";
					Console.WriteLine(conf.getSignValue("signtool") + validateCommand);
					result = UtilityCore.ExecuteCommandLine(conf.getSignValue("signtool"), validateCommand);

					pattern = @".*Successfully verified.*";

					match = Regex.Match(result, pattern);

					if (match.Success == false)
					{
						//The signing was not successful
						errors += "Could not verify that file was signed: " + fileLocation;
						
					}
					}
				if (errors != "")
				{
					throw new Exception("Error during signing " + errors);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		static void SignFile(string signtool, string signCommand)
		{
			
					



		}
    }
}
