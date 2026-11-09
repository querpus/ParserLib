#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public sealed class JSONArrayNode : Node
{
  private static string SerializeChild (ISerializable node) => $"{node.Serialize()}";
  private string SerializeChildren () => Children.Select(SerializeChild).TextJoin(", ");
  public override string Serialize () => $"[{SerializeChildren()}]";

  public bool IsRoot => Data.TryGetValue("IsRoot", out object? obj) && obj is true;
}
