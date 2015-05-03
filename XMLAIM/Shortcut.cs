using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XMLAIM
{
	class Shortcut
	{
		public string Icon;
		public string Name;
		public string Path;
		public string Type;
		public string Condition;
		public string Target;

		public Shortcut(string shortcutIcon,
						string shortcutName, 
						string shortcutPath,
						string shortcutTarget,
						string shortcutType, 
						string shortcutCondition)
		{
			
								
			Icon = shortcutIcon.Trim();
			Name = shortcutName.Trim();
			Path = shortcutPath.Trim();
			Target = shortcutTarget.Trim();
			Type = shortcutType.Trim();
			Condition = shortcutCondition.Trim();

			
			
		}

	}
}
