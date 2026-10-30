#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class JSONArrayNode : Node
{
  public override string Prefix => "[";
  public override string Suffix => "]";
  public override string ChildDelim => ",";
  private static string SerializeChild (ISerializable node) => $"{node.Serialize()}";
  private string SerializeChildren () => Children.Select(SerializeChild).TextJoin(ChildDelim);
  public override string Serialize () => Prefix + SerializeChildren() + Suffix;

  public bool IsRoot => Data.TryGetValue("IsRoot", out object? obj) && obj is true;
}
