namespace Specification.XML;

/// <summary>Represents an opening XML tag.</summary>
public class XMLElementOpen () : XMLNodeAttr, IXMLObject
{
  public override string Serialize () => $"<{Tag}{(Attributes.Count > 0 ? " " : "")}{Attributes.TextJoin(" ")}>";
  public override string ToString () => Serialize();
  public XMLElementClose ClosingElement => new() { Tag = Tag, XMLNamespace = XMLNamespace };
}
