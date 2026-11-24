#pragma warning disable CA1710 // Identifiers should have correct suffix

using System.Collections;
using System.Text.Json;

namespace Specification.JSON;

public sealed class JSONArray : IJSONNode, IEnumerable<IJSONNode>
{
  public JsonValueKind Type => JsonValueKind.Array;
  object? IJSONNode.Value => Values;
  public Collection<IJSONNode> Values { get; init; } = [];

  public int Count => Values.Count;

  public IJSONNode this[int index]
  {
    get => Values[index];
    set => Values[index] = value;
  }
  public IEnumerator<IJSONNode> GetEnumerator () => Values.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  public void Add (IJSONNode child) => Values.Add(child);
  public void AddRange (IEnumerable<IJSONNode> children)
  {
    children.ThrowIfNull();
    foreach (IJSONNode child in children)
    {
      Add(child);
    }
  }

  /// <summary>Gets the JSON of this node.</summary>
  /// <returns>The JSON as a string.</returns>
  public override string? ToString ()
  {
    string result = SE;

    foreach (IJSONNode item in Values)
    {
      if (result.IsNotEmpty)
        result += ",";
      result += item.ToString();
    }

    return "[" + result + "]";
  }
}
