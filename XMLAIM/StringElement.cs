using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLAIM
{
	class StringElement
	{
		/*
		 * <col key="yes" def="s255">ISString</col>
		<col key="yes" def="s50">ISLanguage_</col>
		<col def="S0">Value</col>
		<col def="I2">Encoded</col>
		<col def="S0">Comment</col>
		<col def="I4">TimeStamp</col>
		 */
		public string ISString;
		public string ISLanguage;
		public string Value;
		public string Encoded;
		public string Comment;
		public string TimeStamp;

		public StringElement(string String, string Language, string Val, string Enc, string Com, string Time)
		{
			ISString = String;
			ISLanguage = Language;
			Value = Val;
			Encoded = Enc;
			Comment = Com;
			TimeStamp = Time;
		}
	}
}
