#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Parser.Tokens;

public interface IToken : IEquatable<IToken>, Common.NodeTree.IMultiToken
{
  Spec Spec { get; }
  /// <summary>Assigned at match, cleared if match fails, kept if passes.</summary>
  TokenRef? AssignTo { get; set; }
  string Type { get; }
  bool HasType { get; }
  IToken? Parent { get; set; }
  IList<IToken> Children { get; }
  string Content => Children.Select(static t => t.Content).TextJoin(" ");
  string ContentNoNewLine => Content.Replace(["\n", "\r"], ["<LF>", "<CR>"]);
  string ToString (int indent);
  void Print (int indent);
}
