using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Bitcraft.StateMachineTool.Core
{
    public static class ParsingUtility
    {
        public static bool ElementContentToBoolean(XElement elem, bool defaultValue)
        {
            if (elem == null || elem.FirstNode == null)
                return defaultValue;

            string strValue = null;
            if (elem.FirstNode.NodeType == System.Xml.XmlNodeType.CDATA)
                strValue = ((XCData)elem.FirstNode).Value;
            else if (elem.FirstNode.NodeType == System.Xml.XmlNodeType.Text)
                strValue = ((XText)elem.FirstNode).Value;

            try
            {
                return IntegerToBoolean(strValue, defaultValue);
            }
            catch (FormatException fex)
            {
                var info = (IXmlLineInfo)elem.FirstNode;
                throw new FormatException(string.Format("Invalid value at line {0} position {1}.", info.LineNumber, info.LinePosition), fex);
            }
        }

        public static bool IntegerToBoolean(string integerAsString, bool defaultValue)
        {
            int intValue;
            if (int.TryParse(integerAsString, out intValue) == false)
                throw new FormatException("Invalid boolean value.");

            return intValue != 0;
        }
    }
}
