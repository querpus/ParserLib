namespace Specification.XML;

/// <summary>Represents an XML structure</summary>
public interface IXMLObject : ISerializable
{
  /// <summary>A tag name.</summary>
  string Tag { get; }
}
