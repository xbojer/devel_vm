using System;
using System.Xml;
using System.Windows.Forms;

namespace Devel_VM
{
    class updater
    {
        public static string xmlURL = "http://00.info.pl/devel/ov.xml";
        public static void go()
        {
            Version newVersion = null;
            string url = "";
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(xmlURL);
                reader.MoveToContent();
                string elementName = "";
                if ((reader.NodeType == XmlNodeType.Element) &&
                    (reader.Name == "develvm"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) &&
                                (reader.HasValue))
                            {
                                switch (elementName)
                                {
                                    case "version":
                                        newVersion = new Version(reader.Value);
                                        break;
                                    case "url":
                                        url = reader.Value;
                                        break;
                                }
                            }
                        }
                    }
                    Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    if (curVersion.CompareTo(newVersion) < 0)
                    {
                        // ask the user if he would like
                        // to download the new version
                        //string title = "Fotka Devel VM Manager Update";
                        //string question = "Czy chcesz teraz pobrać nową wersję?";
                        //if (DialogResult.Yes == MessageBox.Show(question, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        //{
                            System.Diagnostics.Process.Start(url);
                        //}
                    }

                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (reader != null) reader.Close();
            }

        }
    }
}
