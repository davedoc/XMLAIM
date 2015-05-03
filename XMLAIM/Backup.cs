using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class Backup
	{
		public string installLocation;
		public string upgradeFrom;

		public Backup(string inst, string upgrfrom)
		{
			installLocation = inst;
			upgradeFrom = upgrfrom;
			installLocation = Regex.Replace(installLocation, @"\/", "\\");
			upgradeFrom = Regex.Replace(upgradeFrom, @"\/", "\\");
		}
	}
}
