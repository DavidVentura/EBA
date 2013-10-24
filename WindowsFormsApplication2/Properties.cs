using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
namespace EBA
{
    static class Props
    {
        internal static bool imprimir = true;
        internal static string ip = "";
        internal static void Initialize()
        {
            XmlDocument xmlDoc = new XmlDocument(); //* create an xml document object.
            try
            {
                xmlDoc.Load("config.xml");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(1);
            }
            XmlNodeList imprimirNodes = xmlDoc.GetElementsByTagName("imprimir");
            XmlNodeList baseNodes = xmlDoc.GetElementsByTagName("base");

            imprimir = imprimirNodes[0].InnerText.ToUpper() == "SI";
            ip = baseNodes[0].InnerText;
        }
    }
}
