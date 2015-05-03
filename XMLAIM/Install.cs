using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class Install
	{
		public string stagePath;
		public string installPath;
		public string platformType;
		public string Feature;
		public string installRootPath;
		public string installFileName;
		public string stageRootPath;
		public string stageFileName;
		public string Version;
		public int ComponentAttributes;
		public int ISAttributes;
		public string Conditions;
		public string shortcutIcon;
		public string shortcutName;
		public string shortcutPath;
		public string extractLocation;
		public string extractSequence;
		public string extractDelete;

		public Install(string stage, string install, string platform, string feature, string version,
			int compAttribs, int IsAttribs , string cond, string scIcon, string scName, string scPath, string eLoc, string eSeq, string eDel)
		{
			stage = Regex.Replace(stage, @"\/", "\\");
			stage = stage.Trim();
			install = Regex.Replace(install, @"\/", "\\");
			install = Regex.Replace(install, @"\\\\", "\\");
			install = install.Trim();
			scPath = Regex.Replace(scPath, @"\/", "\\");
			scIcon = Regex.Replace(scIcon, @"\/", "\\");
			eLoc = Regex.Replace(eLoc, @"\/", "\\");

			stagePath = stage;
			installPath = install;
			platformType = platform;
			Version = version;
			Conditions = cond;
			/*if (platform == "ALL")
			{
				Feature = feature;
			}
			else
			{
				Feature = feature + platform;
			}*/
			Feature = feature;
			ComponentAttributes = compAttribs;
			ISAttributes = IsAttribs;
			shortcutIcon = scIcon;
			shortcutName = scName;
			shortcutPath = scPath;
			extractLocation = eLoc;
			extractSequence = eSeq;
			extractDelete = eDel;

			setInstallRootPath();
			setInstallFileName();
			setStageFileName();
			setStageRootPath();
		}

		private void setInstallRootPath()
		{
			string pattern = @"(.*)\\";

			Match match = Regex.Match(installPath, pattern);

			if (match.Success)
			{
				installRootPath = match.Groups["1"].Value;
			}
		}

		private void setInstallFileName()
		{
			string pattern = @".*\\(.*)";

			Match match = Regex.Match(installPath, pattern);

			if (match.Success)
			{
				installFileName = match.Groups["1"].Value;
			}
		}

		private void setStageRootPath()
		{
			string pattern = @"(.*)\\";

			Match match = Regex.Match(stagePath, pattern);

			if (match.Success)
			{
				stageRootPath = match.Groups["1"].Value;
			}
		}

		private void setStageFileName()
		{
			string pattern = @".*\\(.*)";

			Match match = Regex.Match(stagePath, pattern);

			if (match.Success)
			{
				stageFileName = match.Groups["1"].Value;
			}
		}


	}
}
