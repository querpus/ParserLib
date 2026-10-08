#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

/// <summary>An interface for storing nodes.</summary>
public interface INode : ITextSerializer
{
  Collection<ITextSerializer> Children { get; }
  Dictionary<string, ITextSerializer> Properties { get; }
  Dictionary<string, object> Data { get; }
  /// <summary>Gets the origin of the parsed entity.</summary>
  string? Origin { get; set; }
  /// <summary>Static equality method.</summary>
  /// <param name="obj_a">Entity 'A'.</param>
  /// <param name="obj_b">Entity 'B'.</param>
  /// <returns><see langword="true"/> if 'A' is equal to 'B', <see langword="false"/> otherwise.</returns>
  static bool Equals (INode? obj_a, INode? obj_b) =>
    (obj_a is null && obj_b is null) || (obj_a is not null && obj_b is not null && obj_a.Equals(obj_b));
  bool Equals (object? obj) => obj is Node node && Origin == node.Origin && Children.SequenceEqual(node.Children) && Properties.SequenceEqual(node.Properties) && Data.SequenceEqual(node.Data);
  int GetHashCode () => HashCode.Combine(Origin, Children, Properties, Data);
  /// <summary>Adds a child to the children list.</summary>
  /// <param name="child">The child to add.</param>
  void AddChild (INode child) => Children.Add(child);
  void AddChildren (IEnumerable<INode> children) => children.Foreach(Children.Add);
  void AddData (string key, object data) => Data[key] = data;
}
