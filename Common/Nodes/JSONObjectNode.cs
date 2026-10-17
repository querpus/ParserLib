#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class JSONObjectNode : Node
{
  public override string ChildDelim => ",";
  public override string Prefix => "{";
  public override string Suffix => "}";
  private static string SerializeProperty (KeyValuePair<string, ISerializable> kvp) => $"\"{kvp.Key}\":{kvp.Value.Serialize()}";
  private string SerializeProperties () => Properties.Select(SerializeProperty).TextJoin(",");
  public override string Serialize () => Prefix + SerializeProperties() + Suffix;

  public bool IsRoot => Data.TryGetValue("IsRoot", out object? obj) && obj is true;
}
