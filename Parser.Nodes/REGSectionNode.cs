#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public sealed class REGSectionNode : Node
{
  private int LastSeparator => Name?.LastIndexOf('\\', SCO) ?? -1;
  private int PathStart => Name is null ? -1 : RemoveKey ? 1 : 0;
  public override string Serialize () => "[" + Name + "]";
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
  public string? Key => Name?[(LastSeparator + 1)..];
  public string? Path => Name?[PathStart..LastSeparator];
  public bool RemoveKey => Name?.StartsWith('-', SCO) ?? false;
}
