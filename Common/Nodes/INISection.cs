#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class INISection : Node
{
  public override string Serialize () => "[" + Name + "]";
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
}
