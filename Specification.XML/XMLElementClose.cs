namespace Specification.XML;

/// <summary>Represents a closing XML element.</summary>
public class XMLElementClose () : XMLNode, IXMLObject
{
  public override string Serialize () => $"</{Tag}>";
}
