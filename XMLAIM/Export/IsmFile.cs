using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLAIM.Export
{
	class IsmFile
	{
		/*<col key="yes" def="s72">File</col>
		<col def="s72">Component_</col>
		<col def="s255">FileName</col>
		<col def="i4">FileSize</col>
		<col def="S72">Version</col>
		<col def="S20">Language</col>
		<col def="I2">Attributes</col>
		<col def="i2">Sequence</col>
		<col def="S255">ISBuildSourcePath</col>
		<col def="I4">ISAttributes</col>
		<col def="S72">ISComponentSubFolder_</col>*/
		public string File;
		public string Component_;
		public string FileName;
		public string FileSize;
		public string Version;
		public string Language;
		public string Attributes;
		public string Sequence;
		public string ISBuildSourcePath;
		public string ISAttributes;
		public string ISComponentSubFolder_;

		public IsmFile(string sFile, string sComponent_, string sFileName, string sFileSize, string sVersion, string sLanguage, string sAttributes,
			string sSequence, string sISBuildSourcePath, string sISAttributes, string sISComponentSubFolder_)
		{
			File = sFile;
			Component_ = sComponent_;
			FileName = sFileName;
			FileSize = sFileSize;
			Version = sVersion;
			Language = sLanguage;
			Attributes = sAttributes;
			Sequence = sSequence;
			ISBuildSourcePath = sISBuildSourcePath;
			ISAttributes = sISAttributes;
			ISComponentSubFolder_ = sISComponentSubFolder_;
		}
	}
}
