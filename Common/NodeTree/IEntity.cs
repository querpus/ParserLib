#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Xml.Linq;

namespace Common.NodeTree;

public static class NodeFactory
{
  public static Node CreateNode (XElement origin)
  {
    var newNode = new XMLElementNode()
    {
      Origin = origin.ToString(),
    };
    newNode.Properties.AddRange(origin.Attributes().ToDictionary(a => a.Name.LocalName, a => new QString(a.Value, a.Value.Contains('"') ? "'" : "\"")));
    return newNode;
  }

  public static Node CreateNode (string? origin, IEnumerable<INode> children) => new GenericNode { Origin = origin, Children = [.. children] };
  public static Node CreateNode (string? origin, IEnumerable<INode> children, Dictionary<string, INode> properties) => new GenericNode { Origin = origin, Children = [.. children], Properties = properties };
  public static Node CreateNode (string? origin, IEnumerable<INode> children, Dictionary<string, INode> properties, Dictionary<string, object> data) => new GenericNode { Origin = origin, Children = [.. children], Properties = properties, Data = data };
}

public class QString : ITextSerializer, IEquatable<string>, IComparable<string>
{
  public string Value { get; set; } = SE;
  public string Quote { get; set; } = Chars.QTs;
  public QString () { }
  public QString (string value, string quote = Chars.QTs)
  {
    Value = value;
    Quote = quote;
  }

  public override string ToString () => $"{Quote}{Value}{Quote}";
  public string Serialize () => $"{this}";
  public int CompareTo (string? other) => Value.CompareTo(new QString(other ?? SE), SCO);
  public bool Equals (string? other) => Serialize().Equals(other, SCO) || Value.Equals(other, SCO);

  public static implicit operator QString (string? value) => value is null
      ? new QString()
      : value.StartsWithAny(["\"", "'"]) && value[0] == value[^1]
      ? new QString(value[1..^1], $"{value[0]}")
      : new QString(value);

  public static implicit operator string (QString qstring) => $"{qstring}";
}

public sealed class XMLElementNode : Node
{
  private static string SerializeAttribute (KeyValuePair<string, ITextSerializer> kvp) => $"{kvp.Key}={kvp.Value.Serialize()}";
  private string SerializeAttributes () => Properties.Select(SerializeAttribute).TextJoin(" ");
  public override string Serialize ()
  {
    string? name = Data.TryGetValue("Name", out object? value) && value is string n ? n : null;
    bool isSingle = Children.Count == 0;
    bool isHeader = name is "xml";

    string result = $"<{(isHeader ? "?" : "")}{name} {SerializeAttributes()}{(isSingle ? " /" : "")}{(isHeader ? " ?" : "")}>";

    if (!isSingle)
    {
      result += Children.Select(c => c.Serialize()).TextJoin("");
      result += $"</{name}>";
    }

    return result;
  }
  /// <summary>Gets the element name and namespace.</summary>
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
  public Dictionary<string, ITextSerializer> Attributes => Properties;

}

public abstract class Node : INode
{
  public Collection<ITextSerializer> Children { get; } = [];
  public Dictionary<string, ITextSerializer> Properties { get; } = [];
  public Dictionary<string, object> Data { get; } = [];
  public string? Origin { get; set; }
  public override string ToString () => $"Node(Origin: {Origin}, Children: {Children.Count}, Properties: {Properties.Count}, Data: {Data.Count})";
  public abstract string Serialize ();
}

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
