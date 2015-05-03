using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace XMLAIM
{
    class FeatureElement
    {
        string featureName;
        public Hashtable fileElements = new Hashtable();
        public string Temp;
        public AIMLogger logger;

        public FeatureElement(string name, AIMLogger log)
        {
            featureName = name;
            logger = log;
        }

        public void parseFiles(XPathNavigator nav)
        {
            XPathExpression xExpr;

            xExpr = nav.Compile("//file");

            XPathNodeIterator xIterator = nav.Select(xExpr);

            try
            {
                while (xIterator.MoveNext())
                {
                    XPathNavigator nav2 = xIterator.Current.Clone();

					//FileElement fe = new FileElement(nav2);

                    /*string fileStage = nav2.GetAttribute("stage", "");
                    string fileInstall = nav2.GetAttribute("install", "");
                    //Main.updateLogField("Creating Fileelement: " + fileStage + "," + fileInstall);
                    FileElement fe = new FileElement(fileStage, fileInstall);*/
					//fileElements.Add(fileElements.Count + 1, fe);

                }
            }
            catch (Exception ex)
            {
                logger.log("parseFiles Exeception: " + ex.Message);
            }
        }
    }
}
