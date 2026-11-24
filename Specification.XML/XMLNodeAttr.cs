namespace Specification.XML;

/// <summary>Represents an XML attribute.</summary>
public abstract class XMLNodeAttr : XMLNode
{
  /// <summary>The attributes for this XML object.</summary>
  public Collection<XMLAttr> Attributes { get; init; } = [];
  public Dictionary<string, string> Namespaces { get; init; } = [];
  public string? DefaultNamespace { get; init; }
}
