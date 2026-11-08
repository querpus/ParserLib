#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting



namespace Parser.Nodes;
public sealed class INICommentNode : Node
{
  public override string Serialize () => "; " + Content;
  public string? Content => Data.TryGetValue("Content", out object? value) && value is string name ? name : null;
}
