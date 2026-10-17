#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class REGSection : Node
{
  public override string Serialize () => "[" + Name + "]";
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
  public string? Key => Name?[(Name.LastIndexOf("\\", SCO) + 1)..];
  public string? Path => Name?[..Name.LastIndexOf("\\", SCO)];
}
