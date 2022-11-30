using System.Xml;
using System.Xml.Linq;

namespace Bitcraft.StateMachineTool.Core;

public static class ParsingUtility
{
    public static bool ElementContentToBoolean(XElement? elem, bool defaultValue)
    {
        if (elem == null || elem.FirstNode == null)
            return defaultValue;

        string? strValue = null;

        if (elem.FirstNode.NodeType == XmlNodeType.CDATA)
            strValue = ((XCData)elem.FirstNode).Value;
        else if (elem.FirstNode.NodeType == XmlNodeType.Text)
            strValue = ((XText)elem.FirstNode).Value;

        try
        {
            return IntegerToBoolean(strValue);
        }
        catch (FormatException fex)
        {
            var info = (IXmlLineInfo)elem.FirstNode;
            throw new FormatException(string.Format("Invalid value at line {0} position {1}.", info.LineNumber, info.LinePosition), fex);
        }
    }

    public static bool IntegerToBoolean(string? integerAsString)
    {
        if (int.TryParse(integerAsString, out int intValue) == false)
            throw new FormatException("Invalid boolean value.");

        return intValue != 0;
    }
}
