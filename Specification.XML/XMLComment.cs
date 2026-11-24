namespace Specification.XML;

public sealed class XMLComment () : IXMLObject
{
  /// <summary>Comment content.</summary>
  public string Content { get; set; } = SE;
  string IXMLObject.Tag => Content;

  public string Serialize () => $"<!-- {Content} -->";
  public override string ToString () => Serialize();
}
