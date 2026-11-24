#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public sealed class JSONObjectNode : Node
{
  private static string SerializeProperty (KeyValuePair<string, INode> kvp) => $"\"{kvp.Key}\":{kvp.Value.Serialize()}";
  private string SerializeProperties () => Properties.Select(SerializeProperty).TextJoin(",");
  public override string Serialize () => $"[{SerializeProperties()}]";

  public bool IsRoot => Data.TryGetValue("IsRoot", out object? obj) && obj is true;
}
