#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

/// <summary>An interface for storing nodes.</summary>
public interface INode : ISerializable, IEquatable<INode>
{
  Collection<ISerializable> Children { get; }
  Dictionary<string, ISerializable> Properties { get; }
  Dictionary<string, object> Data { get; }
  /// <summary>Gets the origin of the parsed entity.</summary>
  string? Prefix { get; }
  string? Suffix { get; }
  string ChildDelim { get; }
  bool IEquatable<INode>.Equals (INode? other) => Equals(other);
  bool Equals (object? obj) => obj is INode node && Prefix == node.Prefix && Children.SequenceEqual(node.Children) && Properties.SequenceEqual(node.Properties) && Data.SequenceEqual(node.Data);
  int GetHashCode () => HashCode.Combine(Prefix, Suffix, Children, Properties, Data);
  /// <summary>Adds a child to the children list.</summary>
  /// <param name="child">The child to add.</param>
  void AddChild (INode child) => Children.Add(child);
  void AddChildren (IEnumerable<INode> children) => children.Foreach(Children.Add);
  void AddData (string key, object data) => Data[key] = data;
  void AddData (Dictionary<string, object> data) => Data.AddRange(data);
  void AppendToPrefix (string data);
  void AppendToSuffix (string data);
  void AppendToPrefix (IToken data);
  void AppendToSuffix (IToken data);
}
