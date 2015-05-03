using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class stageOperator
	{
		Manifest stageMan;
		string srcRoot;
		string dstRoot;
		public stageOperator(Manifest man, string src, string dst)
		{
			stageMan = man;
			srcRoot = src;
			dstRoot = dst;
		}

		public void stageManifest()
		{
			try
			{
				/*if (System.IO.Directory.Exists(dstRoot))
				{
					System.IO.Directory.Delete(dstRoot, true);
				}*/
				string copyErrors = "";
				foreach (int i in stageMan.fileElements.Keys)
				{
					FileElement fe = (FileElement)stageMan.fileElements[i];

					
					//foreach (int j in fe.installsHash.Keys)
					//{
						Install inst = (Install)fe.installElement;
						string stageRootDir = inst.stageRootPath;
						string stageFileName = inst.stageFileName;
						string instRootDir = inst.installRootPath;
						string instFileName = inst.installFileName;

						stageRootDir = Regex.Replace(stageRootDir, @"<.*>", srcRoot);
						instRootDir = Regex.Replace(instRootDir, @"\[.*\]", dstRoot + "\\" + inst.Feature);
						//&lt;STAGING&gt;

						//Before the copy is done update the manifest to include the following:
						//  file size
						//FileInfo i1 = new FileInfo(stageRootDir + "\\" + stageFileName);
						//long s1 = i1.Length;
						//  date time stamp of the file
						
						if (!System.IO.Directory.Exists(instRootDir))
						{
							System.IO.Directory.CreateDirectory(instRootDir);
						}
						Console.WriteLine("COPY : " + stageRootDir + "\\" + stageFileName + " --> "
							+ instRootDir + "\\" + instFileName);
						try
						{

							System.IO.File.Copy(stageRootDir + "\\" + stageFileName, instRootDir + "\\" + instFileName, true);
						}
						catch (Exception copyex)
						{
							copyErrors += "COPY : " + stageRootDir + "\\" + stageFileName + " --> "
							+ instRootDir + "\\" + instFileName + " FAILED: " + copyex.Message + "\n";
						}
					//}
					//stageMan.fileElements[i];
				}
				if (copyErrors != "")
				{
					throw new Exception(copyErrors);
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Error during manifest stage operations. Error: " + ex.Message);
				throw new Exception("Could not complete file staging: " + ex.Message);
			}
		}
	}
}
