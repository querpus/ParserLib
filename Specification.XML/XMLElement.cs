namespace Specification.XML;

/// <summary>Represents a paired XML element open and close with content.</summary>
public class XMLElement () : XMLNodeAttr, IXMLObject
{
  public bool IsRoot => Parent is null;
  public XMLElement? Parent { get; set; }
  public void AddChild (XMLElement child)
  {
    Content.Add(child);
    child.Parent = this;
  }
  /// <summary>The contents of this XML node.</summary>
  public Collection<IXMLObject> Content { get; } = [];
  public override string Serialize () => $"<{Tag} {Attributes.TextJoin(" ")}>{Content.TextJoin(" ")}</{Tag}>";
  public override string ToString () => Serialize();
}
