#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class JSONContentNode : Node
{
  public override string Serialize () => Content;
  /// <summary>This contains the quoted string, unquoted boolean, null, undefined, or unquoted number.</summary>
  public string Content
  {
    get => Data.TryGetValue("Content", out object? obj) ? obj as string ?? SE : SE;
    set => Data["Content"] = value ?? throw new InvalidOperationException("Tried to assign null to Content.");
  }
}
