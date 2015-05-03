using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLAIM
{
	class UtilityCore
	{
		public static OperationMode eOperationMode = OperationMode.Standard;
		public static MergeModuleMode eMergeModuleMode = MergeModuleMode.Off;
		public static VersionMode eVersionMode = VersionMode.Off;

		public static void setOperationMode(OperationMode mode)
		{
			eOperationMode = mode;
		}

		public static void setMergeModuleMode(MergeModuleMode mode)
		{
			eMergeModuleMode = mode;
		}

		public static void setVersionMode(VersionMode mode)
		{
			eVersionMode = mode;
		}

		public enum MergeModuleMode
		{
			Off = 0,
			On = 1
		}

		public enum VersionMode
		{
			Off = 0,
			On = 1
		}

		public enum OperationMode
		{
			Standard = 0,
			StageManifest = 1,
			UpdateIsm = 2,
			UpdateStrings = 3,
			ExportIsm = 4,
			ExportGuid = 5,
			SignFiles = 6,
			UpdateIsmPropsOnly = 7

		}

		public static OperationMode getOperationMode { get { return eOperationMode; } }

		public static MergeModuleMode getMergeModuleMode { get { return eMergeModuleMode; } }

		public static VersionMode getVersionMode { get { return eVersionMode; } }

		public static string ExecuteCommandLine(string file, string command)
		{
			try
			{
				System.Diagnostics.Process p = new System.Diagnostics.Process();
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.FileName = file;
				p.StartInfo.Arguments = command;
				p.Start();
				string output = p.StandardOutput.ReadToEnd();
				p.WaitForExit();
				Console.WriteLine(output);

				return output;
			
			}
			catch (Exception ex)
			{
				throw ex;
				// Log the exception
			}
			
		}

		
	}
}
