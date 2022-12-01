using System.Xml;
using System.Xml.Linq;

namespace Bitcraft.StateMachineTool.yWorks;

public interface IGraphObject
{
    string Identifier { get; }
    string Description { get; }
}

public class GraphObject : IGraphObject
{
    public string Identifier { get; }
    public string Description { get; internal set; }

    protected GraphObject(string identifier, string description)
    {
        Identifier = identifier;
        Description = description;
    }

    public static GraphObject Load(XElement element, KeyMapping keyMapping)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));
        if (keyMapping == null)
            throw new ArgumentNullException(nameof(keyMapping));

        string? identifier = element.Attribute("id")?.Value;

        if (identifier == null)
            throw new InvalidDataException($"An XML node doesn't have an 'id' attribute. [{element}]");

        identifier = identifier.Trim();

        var dataElements = element
            .Elements(XName.Get("data", element.GetDefaultNamespace().NamespaceName));

        XElement? descriptionContent = (from x in dataElements
                                        let attributeValue = x.Attribute("key")?.Value
                                        where attributeValue == keyMapping.NodeDescrionId || attributeValue == keyMapping.GraphDescrionId || attributeValue == keyMapping.EdgeDescrionId
                                        select x).FirstOrDefault();

        string? description = null;

        if (descriptionContent != null && descriptionContent.FirstNode != null)
        {
            if (descriptionContent.FirstNode.NodeType == XmlNodeType.CDATA)
                description = ((XCData)descriptionContent.FirstNode).Value;
            else if (descriptionContent.FirstNode.NodeType == XmlNodeType.Text)
                description = ((XText)descriptionContent.FirstNode).Value;
        }

        if (description == null)
            throw new InvalidDataException($"An XML node with 'id' attribute value '{identifier}' doesn't have content. [{element}]");

        description = description.Trim();

        return new GraphObject(identifier, description);
    }

    protected static void CheckIdProperty(IXmlLineInfo element, IGraphObject graphObject)
    {
        bool hasId = string.IsNullOrWhiteSpace(graphObject.Identifier) == false;

        if (hasId)
            return;

        bool hasDescription = string.IsNullOrWhiteSpace(graphObject.Description) == false;

        string msg = string.Format(
            "The node [description: {0}] at line {1} position {2} is incomplete.",
            hasDescription ? graphObject.Description : "<missing>",
            element.LineNumber,
            element.LinePosition
        );

        throw new FormatException(msg);
    }

    protected static void CheckDescriptionProperty(IXmlLineInfo element, IGraphObject graphObject)
    {
        bool hasDescription = string.IsNullOrWhiteSpace(graphObject.Description) == false;

        if (hasDescription)
            return;

        bool hasId = string.IsNullOrWhiteSpace(graphObject.Identifier) == false;

        string msg = string.Format(
            "The node [id: {0}] at line {1} position {2} is incomplete.",
            hasId ? graphObject.Identifier : "<missing>",
            element.LineNumber,
            element.LinePosition
        );

        throw new FormatException(msg);
    }

    public override string ToString()
    {
        return $"[{Identifier}] {Description}";
    }
}
