using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLAIM.Export
{
	class IsmComponent
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
		<col def="S0">ISDotNetInstallerArgsRollback</col>*/

		public string Component;
		public string ComponentId;
		public string Directory_;
		public string Attributes;
		public string Condition;
		public string KeyPath;
		public string ISAttributes;
		public string ISComments;
		public string ISScanAtBuildFile;
		public string ISRegFileToMergeAtBuild;
		public string ISDotNetInstallerArgsInstall;
		public string ISDotNetInstallerArgsCommit;
		public string ISDotNetInstallerArgsUninstall;
		public string ISDotNetInstallerArgsRollback;

		public IsmComponent()
		{

		}
	}
}
