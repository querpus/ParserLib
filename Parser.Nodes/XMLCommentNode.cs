#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public sealed class XMLCommentNode : Node
{
  public override string Serialize () => "<!--" + Content + "-->";
  /// <summary>Gets the content of this node.</summary>
  public string Content => Data.TryGetValue("Content", out object? obj) ? obj as string ?? SE : SE;
}
