#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class XMLContentNode : Node
{
  public override string Serialize () => Content ?? SE;
  /// <summary>Gets or sets the content of this node.</summary>
  public string? Content
  {
    get => Data.TryGetValue("Content", out object? obj) ? obj as string : null;
    set => Data["Content"] = value ?? throw new InvalidOperationException("Tried to assign null to Content.");
  }
}
