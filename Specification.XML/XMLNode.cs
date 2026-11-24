namespace Specification.XML;

/// <summary>An XML node.</summary>
public abstract class XMLNode : IXMLObject
{
  /// <summary>The name of the element.</summary>
  public string Tag { get; set; } = SE;
  /// <summary>The namespace of the element.</summary>
  public string? XMLNamespace { get; set; }
  public abstract string Serialize ();
}
