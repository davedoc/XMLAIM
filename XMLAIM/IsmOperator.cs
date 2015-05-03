using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace XMLAIM
{
    class IsmOperator
    {
        private string ismFile;
        //public AIMLogger logger;
        //private XPathDocument xDoc;
        //private XPathNavigator xNav;
		XmlDocument doc;
		IsmDatabase iDb;

        public IsmOperator( string File)
        {
            ismFile = File;
            //logger = log;
        }

		public IsmOperator(string File, IsmDatabase db)
		{
			ismFile = File;
			iDb = db;
		}

        public void loadISM()
        {

            //xDoc = new XPathDocument(ismFile);
            //xNav = xDoc.CreateNavigator();
			XmlTextReader reader = new XmlTextReader(ismFile);
			doc = new XmlDocument();
			doc.Load(reader);
			reader.Close();

        }

        /*public void processManifest_old(Manifest Man)
        {
            //string name in phones.Keys
            foreach (string feature in Man.featureTable.Keys)
            {
                //logger.log("feature: " + feature);
                FeatureElement fe = (FeatureElement)Man.featureTable[feature];
                foreach (string file in fe.fileElements.Keys)
                {
                    //logger.log("file: " + file );
					FileElement fi = (FileElement)fe.fileElements[file];
                    processFile(fi);
                    
                }
                
            }
        }*/

		//GuidMapping may not really be needed anmore it was added to support the creation of a guid map file
		//which was a requirement due to a bug
		public void processManifest(Manifest Man, String Module)
		{
			IsmTable ism = new IsmTable(doc, Man.iFileCount, iDb);

			if (Module != "")
			{
				ism.AddModule(Module);
			}
			//string name in phones.Keys
			foreach (int i in Man.fileElements.Keys)
			{
				FileElement fe = (FileElement) Man.fileElements[i];
				
				ism.AddFiles(doc, fe);
			}

			foreach (int i in Man.deleteElements.Keys)
			{
				DeleteElement de = (DeleteElement) Man.deleteElements[i];
				
				ism.AddDeletes(doc, de);
			}

			foreach (int i in Man.backupElements.Keys)
			{
				BackupElement be = (BackupElement)Man.backupElements[i];
				ism.AddBackups(doc, be);
			}

			foreach (int i in Man.propertyElements.Keys)
			{
				PropertyElement pe = Man.propertyElements[i];
				ism.AddProperties(pe);
			}
		}

		public void updateProperties(Manifest Man)
		{
			IsmTable ism = new IsmTable(doc, Man.iFileCount, iDb);
			foreach (int i in Man.propertyElements.Keys)
			{
				PropertyElement pe = Man.propertyElements[i];
				ism.AddProperties(pe);
			}
		}

        public void processFile(FileElement fi)
        {
            /*
             *  Steps to perform on File
             * - Case 1: New file: does not exist in the database
			   - Case 2: Duplicate new file: file name already exist but this is still a new file.
			   - Case 3: existing file patch: new version of file
		       - Case 4: existing file: a repeate file same name and location as a file that already exist in the database.
             * */

            /*File Table:
            <row><td>_a_action.xml</td><td>_a_action.xml</td><td>_A_ACTION.xml</td><td>0</td><td>09.03.00343</td><td/><td/><td>1</td><td>&lt;STAGING&gt;\Manager\Common\jboss405\server\default\deploy\mashupmgr.war\backbase\3.3.1dev\tools\reference\reference\attribute\_A_ACTION.xml</td><td>17</td><td/></row>
            */

			//IsmTable ism = new IsmTable(doc);

			//Check to see if file exist in file table.
			//	If file name exist
			//		Is File duplicate?
			//			if it is Check to see if it is an update
			//				if not an update do not add
			//				if it is update file table
			//			if it is not then add a incremented number to the end and add the file.

			//ism.
			
			//ism.AddFiles(doc,fi);
			//ism.AddDeletes(doc,







            /* Case 1: New File
             *  
             */
            /*try
            {
                XPathExpression xExpr;
                //<table name="File">
                xExpr = xNav.Compile("/msi/table[@name='File']/row");
                XPathNodeIterator xIterator = xNav.Select(xExpr);

                while (xIterator.MoveNext())
                {
                    XPathNavigator nav2 = xIterator.Current.Clone();

                    string includePath = nav2.InnerXml.ToString();
                    logger.log("row: " + includePath);
                    //XPathDocument xDoc = new XPathDocument(includePath);
                    //XPathNavigator xNav2 = xDoc.CreateNavigator();

                    //parseIncludes(xNav2);
                    //parseFeatures(xNav2);

                }
            }
            catch (Exception e)
            {
                logger.log(e.Message.ToString());
            }*/
            
        }
    }
}
